using Microsoft.EntityFrameworkCore.Metadata;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TidalWrapped.Data.Models;

namespace TidalWrapped.Services
{
    public class EmailService
    {
        public async Task sendUpdate(List<string> tracks, int total = 0, HourCount mostPlayed = null)
        {
            var apiKey = Environment.GetEnvironmentVariable("smtpApiKey", EnvironmentVariableTarget.User);
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(Environment.GetEnvironmentVariable("EmailFrom", EnvironmentVariableTarget.User), "TidalScraper");
            var subject = "Daily LastFm scrape complete for " + DateTime.Now.Date.AddDays(-1).ToShortDateString();
            var to = new EmailAddress(Environment.GetEnvironmentVariable("EmailTo", EnvironmentVariableTarget.User), "Oojin Ji");
            string msg = "";
            if(tracks.Count > 0)
            {
                msg = "Total Listens: " + total + "<br> " +
                    " Most active hour: " + mostPlayed.hour + ":00 with " + mostPlayed.count + " plays. <br>" +
                    "Songs added: <br>";
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
            Console.WriteLine(response.StatusCode);

        }
    }
}
