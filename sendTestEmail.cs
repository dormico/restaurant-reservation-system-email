using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using System.Collections.Generic;
using System.Threading;

namespace Email
{
    public static class sendTestEmail
    {
        [FunctionName("sendTestEmail")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string userEmail = req.Query["email"];

            // This code demonstrates how to fetch your connection string
            // from an environment variable.
            string connectionString = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING");
            EmailClient emailClient = new EmailClient(connectionString);

            //Replace with your domain and modify the content, recipient details as required

            //var userEmail = "odeak@edu.bme.hu";

            EmailContent emailContent = new EmailContent("Allegro - Your restaurant order details");
            //emailContent.Html = "Thank you for your purchase. Let us know, what do you think about our service. Give feedback here: <a>https://ambitious-desert-085d2d503.1.azurestaticapps.net/user/90930/feedback</a>";
            emailContent.Html = "Szia! <img src=\"https://www.rd.com/wp-content/uploads/2021/02/GettyImages-1208220393-e1612811442160.jpg\" width=360/>";
            List<EmailAddress> emailAddresses = new List<EmailAddress> { new EmailAddress(userEmail) };
            EmailRecipients emailRecipients = new EmailRecipients(emailAddresses);
            EmailMessage emailMessage = new EmailMessage("DoNotReply@099f9b44-a79d-45a3-a060-495c48b7b3e6.azurecomm.net", emailContent, emailRecipients);
            SendEmailResult emailResult = emailClient.Send(emailMessage, CancellationToken.None);

            Console.WriteLine($"MessageId = {emailResult.MessageId}");

            var responseMessage = "Email sent.";
            return new OkObjectResult(responseMessage);
        }
    }
}
