using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Communication.Email;
using Azure.Communication.Email.Models;
using System.Collections.Generic;
using System.Threading;

namespace Email
{
    public static class sendInvoiceEmail
    {
        [FunctionName("sendInvoiceEmail")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            IActionResult response = null;
            string requestBody = String.Empty;
            using (StreamReader streamReader = new StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
            }
            dynamic data = JsonConvert.DeserializeObject<EmailInvoice>(requestBody);
            EmailInvoice emailData = data;
            string userEmail = emailData.Email;
            try
            {


                string connectionString = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING");
                EmailClient emailClient = new EmailClient(connectionString);

                string dishes = "<table style=\"height: 36px; width: 50%; border-collapse: collapse; margin-left: auto; margin-right: auto; background-color: #ffffff;\" border=\"0\"><tbody>";
                foreach (var dish in emailData.Orders)
                {
                    var dishHtml = "<tr style=\"height: 18px;\"><td style=\"width: 50%; height: 18px; text-align: right;\">" + dish.Name + "</td>" +
                    "<td style=\"width: 50%; height: 18px; text-align: center;\">" + dish.Amount + " x " + dish.Price + " $</td></tr>";
                    dishes = dishes + dishHtml;
                }
                dishes = dishes + "</tbody></table>";

                EmailContent emailContent = new EmailContent("Allegro - Invoice");

                emailContent.Html = "<table style=\"height: 100%; width: 100%; border-collapse: collapse; background-color: #5e9ca0; margin-left: auto; margin-right: auto;\" border=\"0\" cellpadding=\"20%\">" +
                "<table style=\"height: 50%; width: 50%; border-collapse: collapse; background-color: #e0e0e0; margin-left: auto; margin-right: auto;\" border=\"1\" cellpadding=\"20%\">" +
                "<h2 style=\"color: #2e6c80; text-align: center;\">Invoice</h2><p style=\"color: #2e6c80; text-align: center;\">Your payment was successful!</p>" +
                dishes + "<table style=\"width: 50%; border-collapse: collapse; margin-left: auto; margin-right: auto; background-color: #ffffff; border: solid thin gray;\"><tbody>" +
                "<tr style=\"height: 18px;\"><td style=\"width: 50%; height: 18px; text-align: right;\"><h3 style=\"color: #2e6c80;\">SUM </td><td style=\"width: 50%; height: 18px; text-align: center;\"><h3 style=\"color: #2e6c80;\">"
                + emailData.SumPrice + " $</h3></td></tr></tbody></table>" +
                "<p>All the best,</p><p>Allegro Team</p><small>Can't make it? Contact " + emailData.RestaurantName + " here: " + emailData.Phone + "</small></table></table>";

                List<EmailAddress> emailAddresses = new List<EmailAddress> { new EmailAddress(userEmail) };
                EmailRecipients emailRecipients = new EmailRecipients(emailAddresses);
                EmailMessage emailMessage = new EmailMessage("DoNotReply@099f9b44-a79d-45a3-a060-495c48b7b3e6.azurecomm.net", emailContent, emailRecipients);
                SendEmailResult emailResult = emailClient.Send(emailMessage, CancellationToken.None);

                Console.WriteLine($"MessageId = {emailResult.MessageId}");
                response = new OkObjectResult(new { message = "Email sent." });
            }
            catch (Exception ex)
            {
                log.LogError($"Could not sen email. Exception thrown: {ex.Message}");
                response = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return response;
        }
    }
}
