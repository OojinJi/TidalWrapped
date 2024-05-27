using Microsoft.EntityFrameworkCore.Metadata;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace TidalWrapped.Services
{
    public class EmailService
    {
        public async Task sendUpdate(List<string> tracks)
        {
            var apiKey = Environment.GetEnvironmentVariable("smtpApiKey", EnvironmentVariableTarget.User);
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(Environment.GetEnvironmentVariable("EmailFrom", EnvironmentVariableTarget.User), "TidalScraper");
            var subject = "Daily LastFm scrape complete";
            var to = new EmailAddress(Environment.GetEnvironmentVariable("EmailTo", EnvironmentVariableTarget.User), "Oojin Ji");
            string msg = "Songs added: <br>";
            if(tracks.Count > 0)
            {
                foreach (string track in tracks)
                {
                    msg += (track);
                }
            }
            else
            {
                msg = "No songs today";
            }


            var sentMail = MailHelper.CreateSingleEmail(from, to, subject, "", msg);

            var response = await client.SendEmailAsync(sentMail);
            Console.WriteLine(response.ToString());

        }
    }
}
