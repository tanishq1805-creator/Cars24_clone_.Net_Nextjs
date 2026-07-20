using Microsoft.AspNetCore.Mvc;
using Cars24API.Models;
using Cars24API.Services;
using BCrypt.Net;

namespace Cars24API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserAuthController : ControllerBase
{
    private readonly UserService _userService;
    private readonly ReferralService _referralService;
    private readonly NotificationService _notificationService;

    public UserAuthController(
        UserService userService,
        ReferralService referralService,
        NotificationService notificationService)
    {
        _userService = userService;
        _referralService = referralService;
        _notificationService = notificationService;
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
            return NotFound("User not found.");

        return Ok(user);
    }
    [HttpPost("signup")]
    public async Task<IActionResult> Signup([FromBody] User user)
    {
        var existingUser = await _userService.GetByEmailAsync(user.Email);

        if (existingUser != null)
            return BadRequest(new { message = "User already exists." });

        var suppliedReferralCode = user.ReferralCode?.Trim().ToUpperInvariant();
        if (!string.IsNullOrWhiteSpace(suppliedReferralCode))
        {
            var referrer = await _userService.GetByReferralCodeAsync(suppliedReferralCode);

            if (referrer == null)
            {
                return BadRequest(new
                {
                    message = "Invalid referral code."
                });
            }

            user.ReferredBy = referrer.Id;
        }

        string generatedReferralCode;
        do
        {
            generatedReferralCode = _referralService.GenerateReferralCode();
        } while (await _userService.GetByReferralCodeAsync(generatedReferralCode) != null);

        user.ReferralCode = generatedReferralCode;

        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        await _userService.CreateAsync(user);

        await _userService.CreateWalletAsync(user.Id!);

        if (!string.IsNullOrWhiteSpace(user.ReferredBy))
        {
            await _userService.AwardReferralSignupAsync(user.ReferredBy, user.Id!);
        }

        return Ok(new
        {
            message = "Signup successful",
            user = new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                phone = user.Phone,
                referralCode = user.ReferralCode
            }
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.GetByEmailAsync(request.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            return Unauthorized(new { message = "Invalid credentials" });

        // A token must have been registered by an earlier sign-in before this
        // server-side push can be delivered.
        if (!string.IsNullOrWhiteSpace(user.Id))
        {
            await _notificationService.SendNotificationAsync(
                user.Id,
                "Welcome back to Cars24",
                "You have logged in successfully.");
        }

        return Ok(new
        {
            message = "Login successful",
            user = new
            {
                id = user.Id,
                fullName = user.FullName,
                email = user.Email,
                phone = user.Phone,
                referralCode = user.ReferralCode
            }
        });
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    [HttpGet("wallet/{userId}")]
    public async Task<IActionResult> GetWallet(string userId)
    {
        var wallet = await _userService.GetWalletAsync(userId);

        if (wallet == null)
        {
            var user = await _userService.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "User not found." });

            // Create a wallet for accounts that existed before wallets were added.
            await _userService.CreateWalletAsync(userId);
            wallet = await _userService.GetWalletAsync(userId);
        }

        return Ok(wallet);
    }

    [HttpGet("wallet/{userId}/transactions")]
    public async Task<IActionResult> GetWalletTransactions(string userId)
    {
        var user = await _userService.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found." });

        var transactions = await _userService.GetWalletTransactionsAsync(userId);
        return Ok(transactions);
    }

    [HttpPost("wallet/add-points/{userId}")]
    public async Task<IActionResult> AddPoints(string userId)
    {
        await _userService.AddWalletPointsAsync(userId, 500);

        return Ok(new
        {
            Message = "500 points added successfully."
        });
    }
    [HttpGet("referral/{userId}")]
    public async Task<IActionResult> GetReferral(string userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);

        if (user == null)
            return NotFound(new { message = "User not found." });

        return Ok(new
        {
            referralCode = user.ReferralCode,
            successfulReferrals = user.SuccessfulReferrals,
            walletPoints = user.WalletPoints
        });
    }
}
