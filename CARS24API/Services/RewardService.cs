using Cars24API.Models;
using MongoDB.Driver;

namespace Cars24API.Services;

public class RewardService
{
    private readonly IMongoCollection<Reward> _rewards;
    private readonly IMongoCollection<RewardRedemption> _redemptions;
    private readonly UserService _userService;

    public RewardService(IConfiguration config, UserService userService)
    {
        _userService = userService;

        var client = new MongoClient(config.GetConnectionString("Cars24DB"));
        var database = client.GetDatabase(config["MongoDB:DatabaseName"]);

        _rewards = database.GetCollection<Reward>("Rewards");
        _redemptions = database.GetCollection<RewardRedemption>("RewardRedemptions");
    }

    public async Task<List<Reward>> GetRewardsAsync() =>
        await _rewards.Find(_ => true).ToListAsync();

    public async Task<Reward?> GetRewardByIdAsync(string id) =>
        await _rewards.Find(reward => reward.Id == id).FirstOrDefaultAsync();

    public async Task CreateRewardAsync(Reward reward) =>
        await _rewards.InsertOneAsync(reward);

    public async Task RedeemRewardAsync(string userId, string rewardId)
    {
        var reward = await GetRewardByIdAsync(rewardId)
            ?? throw new InvalidOperationException("Reward not found.");

        await _userService.DeductWalletPointsAsync(userId, reward.PointsRequired);

        await _redemptions.InsertOneAsync(new RewardRedemption
        {
            UserId = userId,
            RewardId = reward.Id!,
            PointsSpent = reward.PointsRequired,
            RedeemedAt = DateTime.UtcNow,
            Status = "Redeemed"
        });
    }
}
