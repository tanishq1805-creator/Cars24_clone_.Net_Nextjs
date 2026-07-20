using Cars24API.Models;
using Cars24API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cars24API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RewardController : ControllerBase
    {
        private readonly RewardService _rewardService;

        public RewardController(RewardService rewardService)
        {
            _rewardService = rewardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRewards()
        {
            return Ok(await _rewardService.GetRewardsAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateReward(Reward reward)
        {
            await _rewardService.CreateRewardAsync(reward);

            return Ok(new
            {
                message = "Reward created successfully."
            });
        }

        [HttpPost("redeem")]
        public async Task<IActionResult> Redeem(
            [FromBody] RewardRedeemRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.RewardId))
                return BadRequest(new { message = "A user ID and reward ID are required." });

            try
            {
                await _rewardService.RedeemRewardAsync(request.UserId, request.RewardId);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
            catch (Exception exception)
            {
                return BadRequest(new { message = exception.Message });
            }

            return Ok(new
            {
                message = "Reward redeemed successfully."
            });
        }
    }
}
