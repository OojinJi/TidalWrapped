using IF.Lastfm.Core.Api;
using IF.Lastfm.Core.Api.Enums;
using IF.Lastfm.Core.Objects;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using TidalWrapped.Models;

public class Program
{
    private static async Task Main(string[] args)
    {

        var apiKey = Environment.GetEnvironmentVariable("TidalApiKey", EnvironmentVariableTarget.User);
        var apiSecret = Environment.GetEnvironmentVariable("TidalApiSecret", EnvironmentVariableTarget.User);

        var Username = Environment.GetEnvironmentVariable("TidalUsername", EnvironmentVariableTarget.User);
        var Password = Environment.GetEnvironmentVariable("TidalPassword", EnvironmentVariableTarget.User);

        var client = new LastfmClient(apiKey, apiSecret );

        var response = await client.Auth.GetSessionTokenAsync(Username, Password);

        Console.WriteLine("Auth Status: " + response.Status + "\n");

        DateTimeOffset lastT = new DateTimeOffset(DateTime.Now - new TimeSpan(24, 0 ,0));

        Console.WriteLine(lastT.ToString());

        var recent = await client.User.GetRecentScrobbles(Username, lastT, DateTimeOffset.Now, true);

        var tracks = recent.Select(x => 
        new TrackModel
        {
            song = x.Name,
            artist = x.ArtistName,
            album = x.AlbumName,
            songDuration = (long)client.Track.GetInfoAsync(x.Name ?? "test", x.ArtistName).Result.Content.Duration.GetValueOrDefault().TotalMilliseconds,
            albumArt = client.Album.GetInfoAsync(x.ArtistName, x.AlbumName).Result.Content.Images.Large.ToString(),
            whenPlayed = x.TimePlayed.HasValue ? x.TimePlayed.Value.DateTime.ToLocalTime() : DateTime.MinValue,
            //releaseDate = client.Album.GetInfoAsync(x.ArtistName, x.AlbumName).Result.Content.ReleaseDateUtc.GetValueOrDefault().DateTime.Date,
        });
        Console.WriteLine("recent:");
        var count = 1;
        foreach (var x in tracks)
        {
            Console.WriteLine(count + ". " + x.song + ", " + x.artist + ", " + x.album + ", " + x.albumArt + ", " + x.songDuration + ", " + x.whenPlayed);
            count++;
        }


    }
}