using iIpos_core.Data;
using iIpos_core.Dto.Feedback;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace iIpos_core.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly MyDbContext _context;

        public FeedbackController(MyDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SubmitFeedback([FromBody] FeedbackDto feedbackDto)
        {
            var table = await _context.TableInfos.AsNoTracking()
                .FirstOrDefaultAsync(t => t.Token == feedbackDto.Token);

            if (table == null)
            {
                return NotFound("Table not found for the given token.");
            }

            var feedback = new Feedback
            {
                BranchId = table.BranchId,
                Rating = feedbackDto.Rating,
                Comments = feedbackDto.Comments,
                CustomerPhoneNumber = feedbackDto.CustomerPhoneNumber,
                NegativeFeedbackTags = string.Join(", ", feedbackDto.NegativeFeedbackTags)
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            return Ok("Feedback submitted successfully.");
        }
    }
}
