using DiscogsClient.Data.Query;
using DiscogsClient.Internal;
using IF.Lastfm.Core.Api;
using Microsoft.Extensions.Logging;
using RestSharpHelper.OAuth1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TidalWrapped.Models;
using TidalWrapped.Services;

namespace TidalWrapped
{
    public class TidalWrappedLogic
    {
        private readonly DataService _dataService;
        private readonly ILogger<TidalWrappedLogic> _logger;
        private readonly EmailService _emailService;

        private readonly LastfmClient client;
        public TidalWrappedLogic(DataService dataService, ILogger<TidalWrappedLogic> logger, EmailService emailService)
        {
            _dataService = dataService;
            _logger = logger;
            var apiKey = Environment.GetEnvironmentVariable("TidalApiKey", EnvironmentVariableTarget.User);
            var apiSecret = Environment.GetEnvironmentVariable("TidalApiSecret", EnvironmentVariableTarget.User);
            client = new LastfmClient(apiKey, apiSecret);
            _emailService = emailService;
        }
        public void mainLogic()
        {

            var Username = Environment.GetEnvironmentVariable("TidalUsername", EnvironmentVariableTarget.User);
            var Password = Environment.GetEnvironmentVariable("TidalPassword", EnvironmentVariableTarget.User);
            var response = client.Auth.GetSessionTokenAsync(Username, Password);



            Console.WriteLine("Auth Status: " + response.Status + "\n");

            DateTimeOffset lastT = new DateTimeOffset(DateTime.Now - new TimeSpan(24, 0, 10000));

            Console.WriteLine(lastT.ToString());

            var recent = client.User.GetRecentScrobbles(Username, lastT, DateTimeOffset.Now, true, 1, 100).Result;

            var tracks = recent.Select(async x =>
            new TrackModel
            {
                song = x.Name,
                artist = x.ArtistName,
                album = x.AlbumName,
                trackPage = x.Url.ToString(),
                whenPlayed = x.TimePlayed.HasValue ? x.TimePlayed.Value.DateTime.ToLocalTime() : DateTime.MinValue,
            });

            var trackList = tracks.ToList();

            _dataService.InsertData(trackList);

        }
    }
}
