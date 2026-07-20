using Cars24API.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cars24API.Services;

public class UserService
{
    private readonly IMongoCollection<User> _users;
    private readonly IMongoCollection<ReferralWallet> _wallets;
    private readonly IMongoCollection<WalletTransaction> _walletTransactions;
    private readonly IMongoCollection<ReferralReward> _rewards;
    private readonly IMongoCollection<ReferralReward> _referralRewards;

    public UserService(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("Cars24DB"));


        var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _users = database.GetCollection<User>("Users");

        _wallets = database.GetCollection<ReferralWallet>("ReferralWallets");

        _walletTransactions = database.GetCollection<WalletTransaction>("WalletTransactions");

        _rewards = database.GetCollection<ReferralReward>("ReferralRewards");

        _referralRewards = database.GetCollection<ReferralReward>("ReferralRewards");
    }

    public async Task<User?> GetByEmailAsync(string email) =>
        await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

    public async Task<User?> GetByReferralCodeAsync(string referralCode) =>
        await _users.Find(u => u.ReferralCode == referralCode).FirstOrDefaultAsync();

    public async Task CreateAsync(User user)
    {
        Console.WriteLine($"Saving ReferralCode = {user.ReferralCode}");

        await _users.InsertOneAsync(user);
    }
    public async Task<User?> GetByIdAsync(string id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }
    public async Task UpdateAsync(string id, User user)
    {
        await _users.ReplaceOneAsync(u => u.Id == id, user);
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        return await _users.Find(u => u.Id == userId)
                           .FirstOrDefaultAsync();
    }

    public async Task CreateWalletAsync(string userId)
    {
        var wallet = new ReferralWallet
        {
            UserId = userId,
            Balance = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _wallets.InsertOneAsync(wallet);
    }

    public async Task AssignMissingReferralCodesAsync(ReferralService referralService)
    {
        var missingCodeFilter = new BsonDocumentFilterDefinition<User>(
            new BsonDocument("$or", new BsonArray
            {
                new BsonDocument(nameof(User.ReferralCode), BsonNull.Value),
                new BsonDocument(nameof(User.ReferralCode), string.Empty)
            }));

        var usersWithoutCodes = await _users.Find(missingCodeFilter).ToListAsync();

        foreach (var user in usersWithoutCodes)
        {
            if (string.IsNullOrWhiteSpace(user.Id))
                continue;

            string referralCode;
            do
            {
                referralCode = referralService.GenerateReferralCode();
            } while (await GetByReferralCodeAsync(referralCode) != null);

            await _users.UpdateOneAsync(
                existingUser => existingUser.Id == user.Id,
                Builders<User>.Update.Set(existingUser => existingUser.ReferralCode, referralCode));
        }
    }

    public async Task<ReferralWallet?> GetWalletAsync(string userId)
    {
        return await _wallets.Find(w => w.UserId == userId)
                             .FirstOrDefaultAsync();
    }

    public async Task<List<WalletTransaction>> GetWalletTransactionsAsync(string userId)
    {
        return await _walletTransactions
            .Find(transaction => transaction.UserId == userId)
            .SortByDescending(transaction => transaction.CreatedAt)
            .ToListAsync();
    }

    public async Task AddWalletPointsAsync(
        string userId,
        int points,
        string type = "Wallet Credit",
        string description = "Wallet points added")
    {
        var wallet = await GetWalletAsync(userId);

        if (wallet == null)
        {
            await CreateWalletAsync(userId);
            wallet = await GetWalletAsync(userId);
        }

        if (wallet == null)
            throw new InvalidOperationException("Unable to create a wallet for this user.");

        wallet.Balance += points;
        wallet.UpdatedAt = DateTime.UtcNow;

        await _wallets.ReplaceOneAsync(
            w => w.Id == wallet.Id,
            wallet);

        // Update user's wallet points
        var update = Builders<User>.Update.Inc(u => u.WalletPoints, points);

        await _users.UpdateOneAsync(
            u => u.Id == userId,
            update);

        await AddWalletTransactionAsync(userId, points, type, description);
    }

    public async Task AwardReferralSignupAsync(string referrerId, string referredUserId)
    {
        const int referrerPoints = 500;

        var alreadyRewarded = await _rewards
            .Find(reward => reward.ReferredUserId == referredUserId && reward.Trigger == "Signup")
            .AnyAsync();

        if (alreadyRewarded)
            return;

        await AddWalletPointsAsync(
            referrerId,
            referrerPoints,
            "Referral Reward",
            $"Reward for referring user {referredUserId}");

        await _users.UpdateOneAsync(
            user => user.Id == referrerId,
            Builders<User>.Update.Inc(user => user.SuccessfulReferrals, 1));

        await _users.UpdateOneAsync(
            user => user.Id == referredUserId,
            Builders<User>.Update.Set(user => user.ReferralRewardClaimed, true));

        await _rewards.InsertOneAsync(new ReferralReward
        {
            ReferrerId = referrerId,
            ReferredUserId = referredUserId,
            ReferrerPoints = referrerPoints,
            ReferredUserPoints = 0,
            Trigger = "Signup"
        });

    }

    public async Task AddWalletTransactionAsync(
    string userId,
    int points,
    string type,
    string description)
    {
        var transaction = new WalletTransaction
        {
            UserId = userId,
            Points = points,
            Type = type,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        await _walletTransactions.InsertOneAsync(transaction);
    }

    public async Task RewardReferralAsync(string referredUserId)
    {
        // Get the referred user
        var referredUser = await _users.Find(u => u.Id == referredUserId)
                                       .FirstOrDefaultAsync();

        if (referredUser == null)
            throw new Exception("User not found.");

        // User wasn't referred by anyone
        if (string.IsNullOrEmpty(referredUser.ReferredBy))
            return;

        // Reward already claimed
        if (referredUser.ReferralRewardClaimed)
            return;

        // Get the referrer
        var referrer = await _users.Find(u => u.Id == referredUser.ReferredBy)
                                   .FirstOrDefaultAsync();

        if (referrer == null)
            return;

        const int rewardPoints = 500;

        // Reward both users
        await AddWalletPointsAsync(referrer.Id!, rewardPoints);
        await AddWalletPointsAsync(referredUser.Id!, rewardPoints);

        // Mark reward as claimed
        var update = Builders<User>.Update
            .Set(u => u.ReferralRewardClaimed, true)
            .Inc(u => u.SuccessfulReferrals, 1);

        await _users.UpdateOneAsync(
            u => u.Id == referredUser.Id,
            update);
    }

    public async Task CreateReferralRewardAsync(
    string referrerId,
    string referredUserId,
    int points)
    {
        var reward = new ReferralReward
        {
            ReferrerId = referrerId,
            ReferredUserId = referredUserId,
            ReferrerPoints = points,
            ReferredUserPoints = 0,
            Trigger = "Referral"
        };

        await _referralRewards.InsertOneAsync(reward);
    }

    public async Task DeductWalletPointsAsync(string userId, int points)
    {
        var wallet = await GetWalletAsync(userId);

        if (wallet == null)
            throw new Exception("Wallet not found.");

        if (wallet.Balance < points)
            throw new Exception("Insufficient wallet balance.");

        wallet.Balance -= points;
        wallet.UpdatedAt = DateTime.UtcNow;

        await _wallets.ReplaceOneAsync(
            w => w.Id == wallet.Id,
            wallet);

        var update = Builders<User>.Update
            .Inc(u => u.WalletPoints, -points);

        await _users.UpdateOneAsync(
            u => u.Id == userId,
            update);

        await AddWalletTransactionAsync(
            userId,
            -points,
            "Debit",
            "Reward Redeemed");
    }

}
