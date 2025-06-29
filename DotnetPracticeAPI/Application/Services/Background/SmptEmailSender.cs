using Hangfire;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Application.Services.Background
{
    public class SmptEmailSender : IEmailSender
    {
        private readonly ILogger<SmptEmailSender> _logger;

        public SmptEmailSender(ILogger<SmptEmailSender> logger)
        {
            _logger = logger;
        }

        [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.LogInformation($"[Pendiente] Email para {to}. Subject: {subject}");
            await Task.Delay(3000); // Simula envio

            _logger.LogInformation($"[Completado] Email enviado a {to} (Tiempo: {3000}ms)");
        }
    }
}
