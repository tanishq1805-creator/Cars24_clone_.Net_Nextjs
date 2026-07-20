using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Cars24API.Models;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string Phone { get; set; } = string.Empty;

    [BsonRepresentation(BsonType.ObjectId)]

    public List<string> BookingId { get; set; } = new List<string>();

    [BsonRepresentation(BsonType.ObjectId)]

    public List<string> AppointmentId { get; set; } = new List<string>();


    // User's unique referral code
    public string ReferralCode { get; set; } = "";

    // User who referred this account
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ReferredBy { get; set; }

    // Prevents rewarding twice
    public bool ReferralRewardClaimed { get; set; } = false;

    // Total successful referrals
    public int SuccessfulReferrals { get; set; } = 0;

    // Wallet balance (cached for fast access)
    public int WalletPoints { get; set; } = 0;
}