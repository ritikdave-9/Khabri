using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Data.Entity;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;
using System;
using System.Collections.Generic;

namespace Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendNewsNotificationAsync(User user, News news)
        {
            var smtpSection = _configuration.GetSection("Smtp");
            var portString = smtpSection["Port"];
            var enableSslString = smtpSection["EnableSsl"];
            var fromString = smtpSection["From"];

            if (string.IsNullOrWhiteSpace(portString) ||
                string.IsNullOrWhiteSpace(enableSslString) ||
                string.IsNullOrWhiteSpace(fromString))
            {
                throw new InvalidOperationException("SMTP configuration is missing required values.");
            }

            var smtpClient = new SmtpClient(smtpSection["Host"])
            {
                Port = int.Parse(portString),
                Credentials = new NetworkCredential(smtpSection["Username"], smtpSection["Password"]),
                EnableSsl = bool.Parse(enableSslString)
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromString),
                Subject = $"Khabri: New News Matching Your Subscription - {news.Title}",
                Body = $"Hello {user.FirstName},\n\nA new news item matching your interests has been published:\n\n" +
                       $"Title: {news.Title}\n" +
                       $"Description: {news.Description}\n" +
                       $"Read more: {news.Url}\n\n" +
                       $"Best regards,\nKhabri Team",
                IsBodyHtml = false
            };
            mailMessage.To.Add(user.Email);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendNewsDigestAsync(User user, List<News> newsList)
        {
            var smtpSection = _configuration.GetSection("Smtp");
            var portString = smtpSection["Port"];
            var enableSslString = smtpSection["EnableSsl"];
            var fromString = smtpSection["From"];

            if (string.IsNullOrWhiteSpace(portString) ||
                string.IsNullOrWhiteSpace(enableSslString) ||
                string.IsNullOrWhiteSpace(fromString))
            {
                throw new InvalidOperationException("SMTP configuration is missing required values.");
            }

            var smtpClient = new SmtpClient(smtpSection["Host"])
            {
                Port = int.Parse(portString),
                Credentials = new NetworkCredential(smtpSection["Username"], smtpSection["Password"]),
                EnableSsl = bool.Parse(enableSslString)
            };

            var body = $"Hello {user.FirstName},\n\nHere is your news digest:\n\n";
            foreach (var news in newsList)
            {
                body += $"- {news.Title}\n  {news.Description}\n  Read more: {news.Url}\n\n";
            }
            body += "Best regards,\nKhabri Team";

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromString),
                Subject = "Khabri: Your News Digest",
                Body = body,
                IsBodyHtml = false
            };
            mailMessage.To.Add(user.Email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
