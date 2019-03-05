using System;
using System.Globalization;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Amazon;
using System.Collections.Generic;
using WebApi.Entities;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp;
using RestSharp.Authenticators;
using Microsoft.Extensions.Configuration;

namespace WebApi.Helpers
{
    public class MailgunAPI
    {
        private static string _mailgunApiKey;

        // Static constructor to populate static roperty
        static MailgunAPI()
        {
            _mailgunApiKey = Environment.GetEnvironmentVariable("MAILGUNAPI");
        }

        public MailgunAPI()
        {
        }       

        public static string SendApplicationEmailMessage(Application application)
        {
            RestClient client = new RestClient("https://api.eu.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api", _mailgunApiKey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", "darbointerviu.lt", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Darbo Interviu <support@darbointerviu.lt>");
            request.AddParameter("to", application.CandidateEmail);
            request.AddParameter("subject", "Video Interviu - " + application.Title);


            string htmlBody = $@"<html>
<head></head>
<body>
    <h3>Jums yra paruoštas ""{application.Title}"" video interviu.</h3>
    <p>Norint pradėti interviu prisijunkite prie: <a href='https://www.darbointerviu.lt/candidate'>https://www.darbointerviu.lt/candidate</a></p>

    <table border=""0"">
        <tr>
            <td>Jūsų prisijungimo vardas (tai yra Jūsų elektroninio pašto adresas)</td>
            <td><strong>{application.CandidateEmail}</strong></td>
        </tr>
        <tr>
            <td>Jūsų slaptažodis:</td>
            <td><strong>{application.CandidateSecret}</strong></td>
        </tr>
        <tr>
            <td>Interviu turite užbaigti iki:</td>
            <td><strong>{application.Expiration.ToString("yyyy-MM-dd HH:mm")}</strong></td>
        </tr>
    </table> 
    <br>
    <br>
    <p><a href='https://www.darbointerviu.lt/candidate/unsubscribe?email={application.CandidateEmail}'>Unsubscribe</a></p>    
</body>
</html>";

            request.AddParameter("html", htmlBody);
            request.Method = Method.POST;

            IRestResponse response = client.Execute(request);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info(response.Content);

            return response.Content;
        }

        public static string SendApplicationShareEmailMessage(Application application, string partnerEmail)
        {
            RestClient client = new RestClient("https://api.eu.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api", _mailgunApiKey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", "darbointerviu.lt", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Darbo Interviu <support@darbointerviu.lt>");
            request.AddParameter("to", partnerEmail);
            request.AddParameter("subject", "Video Interviu - " + application.CandidateName);


            string htmlBody = $@"<html>
<head></head>
<body>
    <h3>Jūsų partneris pasidalino su Jumis video interviu įrašu.</h3>
    <table border=""0"">
        <tr>
            <td>Interviu pavadinimas:</td>
            <td><strong>{application.Title}</strong></td>
        </tr>
        <tr>
            <td>Atliko:</td>
            <td><strong>{application.CandidateName}</strong></td>
        </tr>
    </table>
    <p>Įrašą galite peržiūrėti prisijungę prie: <a href='https://www.darbointerviu.lt/employer'>https://www.darbointerviu.lt/employer</a> svetainės</p>
</body>
</html>";

            request.AddParameter("html", htmlBody);
            request.Method = Method.POST;

            IRestResponse response = client.Execute(request);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info(response.Content);

            return response.Content;
        }

        public static string SendTestMessage(string title)
        {

            var application = new Application
            {
                Title = title,
                CandidateEmail = "test@gmail.com",
                CandidateSecret = "1234",
                Expiration = DateTime.Now
            };

            RestClient client = new RestClient("https://api.eu.mailgun.net/v3");
            client.Authenticator =
                new HttpBasicAuthenticator("api", _mailgunApiKey);

            RestRequest request = new RestRequest();
            request.AddParameter("domain", "darbointerviu.lt", ParameterType.UrlSegment);
            request.Resource = "{domain}/messages";
            request.AddParameter("from", "Darbo Interviu <support@darbointerviu.lt>");
            request.AddParameter("to", "jzilcov@gmail.com");
            request.AddParameter("subject", "Video Interviu - " + application.Title);

            string htmlBody = $@"<html>
<head></head>
<body>
    <h3>Jums yra paruoštas ""{application.Title}"" video interviu.</h3>
    <p>Norint pradėti interviu prisijunkite prie: <a href='https://www.darbointerviu.lt/candidate'>https://www.darbointerviu.lt/candidate</a></p>

    <table border=""0"">
        <tr>
            <td>Jūsų prisijungimo vardas (tai yra Jūsų elektroninio pašto adresas)</td>
            <td><strong>{application.CandidateEmail}</strong></td>
        </tr>
        <tr>
            <td>Jūsų slaptažodis:</td>
            <td><strong>{application.CandidateSecret}</strong></td>
        </tr>
        <tr>
            <td>Interviu turite užbaigti iki:</td>
            <td><strong>{application.Expiration.ToString("yyyy-MM-dd HH:mm")}</strong></td>
        </tr>
    </table> 
    <br>
    <br>
    <p><a href='https://www.darbointerviu.lt/candidate/unsubscribe?email={application.CandidateEmail}'>Unsubscribe</a></p>

    
</body>
</html>";

            request.AddParameter("html", htmlBody);
            request.Method = Method.POST;

            IRestResponse response = client.Execute(request);

            var logger = NLog.LogManager.GetCurrentClassLogger();
            logger.Info(response.Content);

            return response.Content;
        }

    }
}