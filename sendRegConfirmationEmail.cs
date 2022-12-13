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

namespace Email;
public static class sendRegConfirmationEmail
{
    [FunctionName("sendRegConfirmationEmail")]
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
        dynamic data = JsonConvert.DeserializeObject<EmailRegConfirmation>(requestBody);
        EmailRegConfirmation emailData = data;
        string userEmail = emailData.Email;


        try
        {
            string connectionString = Environment.GetEnvironmentVariable("COMMUNICATION_SERVICES_CONNECTION_STRING");
            EmailClient emailClient = new EmailClient(connectionString);
            EmailContent emailContent = new EmailContent("Allegro - Confirm your registration");

            emailContent.Html = "<table style=\"height: 100%; width: 100%; border-collapse: collapse; background-color: #5e9ca0; margin-left: auto; margin-right: auto;\" border=\"0\" cellpadding=\"20%\">" +
            "<table style=\"height: 50%; width: 50%; border-collapse: collapse; background-color: #e0e0e0; margin-left: auto; margin-right: auto;\" border=\"1\" cellpadding=\"20%\">" +
            "<p>We got a new registration attempt with your e-mail. To make sure it was you, please type in the following security code: </p>" +
            "<h3 style=\"text-align: center;\"><strong>" + emailData.Code + "</strong></h3>" +
            "<p>If you already have an account with this e-mail address, it's data will be lost!</p><p>All the best,</p><p>Allegro Team</p></table></table>";
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
