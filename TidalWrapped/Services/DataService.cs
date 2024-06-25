using IF.Lastfm.Core.Api;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TidalWrapped.Data;
using TidalWrapped.Data.Models;
using TidalWrapped.Models;

namespace TidalWrapped.Services
{
    public class DataService
    {
        private readonly ILogger<DataService> _logger;
        private readonly EmailService _emailService;
        public DataService(ILogger<DataService> logger, EmailService emailService) 
        {
            _logger = logger;
            _emailService = emailService;
        }
        public void InsertData(List<Task<TrackModel>> tracks)
        {

            using(var db = new TidalDbContext())
            {
                int counter = 0;
                var trackList = tracks.ToList();
                List<Track> inserted = new List<Track>();
                foreach (var trackD in tracks)
                {
                    var track = trackD.Result;
                    var existing = db.Tracks.FirstOrDefault(x => x.song == track.song && x.artist == track.artist && x.album == track.album && x.whenPlayed == track.whenPlayed);
                    if(existing == null)
                    {
                        Track trackDb = new Track()
                        {
                            song = track.song,
                            album = track.album,
                            artist = track.artist,
                            whenPlayed = track.whenPlayed,
                            trackPage = track.trackPage
                        };
                        inserted.Add(trackDb);
                        _logger.LogInformation("Song added: " + track.song + ", " + track.artist + ", " + track.album + ", " + track.whenPlayed);
                        counter++;
                    }
                    _logger.LogInformation(counter.ToString() + " added to database");
                }
                if (inserted.Count > 0)
                {
                    var json = JsonConvert.SerializeObject(generateDaySum(inserted));
                    var totalL = tracks.Count();
                    DaySum toDay = new DaySum();
                    toDay.TotalListens = inserted.Count;
                    toDay.MostActive = json;
                    toDay.Tracks = inserted;
                    toDay.Date = DateTime.Now.Date;
                    db.DaySums.Add(toDay);
                    HourCount mostplayed = JsonConvert.DeserializeObject<List<HourCount>>(json)[0];
                    db.SaveChanges();
                    _emailService.sendUpdate(TrackInfo(inserted), toDay.TotalListens, mostplayed).Wait();
                }
                else
                {
                    _emailService.sendUpdate(TrackInfo(inserted)).Wait();
                }
            }
        }

        private List<HourCount> generateDaySum(List<Track> tracks)
        {
            var trackList = tracks.ToList();
            var numOfTracks = 0;
            var numOfTracksold = 0;
            var counter = 0;
            var lastHour = 0;
            tracks.OrderByDescending(x => x.whenPlayed);
            var totalL = tracks.Count();
            List<HourCount> MostActive = new List<HourCount>();
            var currentHour = tracks[0].whenPlayed.Hour;
            foreach (Track track in tracks)
            {
                var hour = track.whenPlayed.Hour;
                counter++;
                if (counter == totalL)
                {
                    numOfTracks++;
                    MostActive.Add(new HourCount() { count = numOfTracks, hour = hour });
                }
                else if (currentHour == hour)
                {
                    lastHour = hour;
                    numOfTracks++;
                }
                else
                {
                    MostActive.Add(new HourCount() { count = numOfTracks, hour = lastHour });
                    numOfTracksold = numOfTracks;
                    numOfTracks = 1;
                    currentHour = hour;
                }

            }
            return MostActive.OrderByDescending(x => x.count).ToList();
        }
        private List<string> TrackInfo(List<Track> tracks)
        {
            List<string> result = new List<string>();
            var counter = 1;
            foreach (var track in tracks)
            {
                result.Add(counter + ". " +track.song + ", " + track.artist + ", " + track.album + ", " + track.whenPlayed + "&nbsp; <a href=\"" + track.trackPage + "\">Track Page</a> <br><br>");
                counter++;
            }
            return result;
        }
    }
}
