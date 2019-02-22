using System;
using System.Globalization;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Amazon;
using System.Collections.Generic;
using WebApi.Entities;

namespace WebApi.Helpers
{
    public class AmazonAPI
    {
        public AmazonAPI()
        {
        }

        public static string SendApplicationEmailMessage(Application application)
        {
            var logger = NLog.LogManager.GetCurrentClassLogger();

            string senderAddress = "support@darbointerviu.lt";
            string receiverAddress = "jzilcov@gmail.com"; //TODO: application.CandidateEmail;
            string configSet = "ConfigSet";
            string subject = "Online Interview 2";
            string htmlBody = $@"<html>
<head></head>
<body>
    <h3>""{application.Title}"" video interview is prepared for you.</h3>
    <p>To start go to: <a href='https://www.darbointerviu.lt/candidate'>https://www.darbointerviu.lt/candidate</a></p>

    <table border=""0"">
        <tr>
            <td>Your user name is:</td>
            <td><strong>{application.CandidateEmail}</strong></td>
        </tr>
        <tr>
            <td>Your secret is:</td>
            <td><strong>{application.CandidateSecret}</strong></td>
        </tr>
        <tr>
            <td>Complete interview before:</td>
            <td><strong>{application.Expiration.ToString("yyyy/MM/dd HH:mm")}</strong></td>
        </tr>
    </table> 
        
    
</body>
</html>";
            var sendRequest = new SendEmailRequest
            {
                Source = senderAddress,
                Destination = new Destination
                {
                    ToAddresses =
                        new List<string> { receiverAddress }
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = htmlBody
                        }
                    }
                },
                // If you are not using a configuration set, comment
                // or remove the following line 
                //ConfigurationSetName = configSet
            };

            using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.EUWest1))
            {
                string statusCode = "501";

                try
                {
                    logger.Info("Sending email via Amzon SES to the candidate: " + application.CandidateEmail);
                    var response = client.SendEmailAsync(sendRequest);
                    statusCode = response.Result.HttpStatusCode.ToString(); //Must be OK
                    logger.Info("Amazon Response code: " + statusCode);
                }
                catch (Exception ex)
                {
                    logger.Info("Exception happened while sending email via Amzon SES: " + ex.Message);
                    logger.Info("Message content: " + htmlBody);
                }

                return statusCode;
            }
        }

        public static string SendEmailMessage(string Message)
        {

            var logger = NLog.LogManager.GetCurrentClassLogger();

            string senderAddress = "support@darbointerviu.lt";
            string receiverAddress = "jzilcov@gmail.com";
            string configSet = "ConfigSet";
            string subject = "Amazon SES - " + Message;
            string htmlBody = @"<html>
<head></head>
<body>
  <h3>Video Interviu</h3>
  <p>Go to:
    <a href='https://www.darbointerviu.lt/candidate'>Candidate</a>
  </p>
</body>
</html>";
            var sendRequest = new SendEmailRequest
            {
                Source = senderAddress,
                Destination = new Destination
                {
                    ToAddresses =
                        new List<string> { receiverAddress }
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = htmlBody
                        }
                    }
                },
                // If you are not using a configuration set, comment
                // or remove the following line 
                //ConfigurationSetName = configSet
            };

            using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.EUWest1))
            {
                string statusCode = "501";

                try
                {
                    logger.Info("Sending email via Amzon SES");
                    var response = client.SendEmailAsync(sendRequest);
                    statusCode = response.Result.HttpStatusCode.ToString();
                    logger.Info("Amazon Response code: " + statusCode);
                }
                catch (Exception ex)
                {
                    logger.Info("Exception happened while sending email via Amzon SES: " + ex.Message);
                }

                return statusCode;
            }
        }
    }
}