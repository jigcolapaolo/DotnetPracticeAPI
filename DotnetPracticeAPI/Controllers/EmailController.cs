using Application.DTOs;
using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobs;

        public EmailController(IBackgroundJobClient backgroundJobs)
        {
            _backgroundJobs = backgroundJobs;
        }

        [HttpPost]
        public IActionResult SendEmail([FromBody] EmailRequest request)
        {
            var jobId = _backgroundJobs.Enqueue<IEmailSender>(x =>
                x.SendEmailAsync(request.To, request.Subject, request.Body
            ));

            return Accepted(new { JobId = jobId });
        }
    }
}
