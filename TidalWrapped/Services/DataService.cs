using IF.Lastfm.Core.Api;
using Microsoft.Extensions.Logging;
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
                        db.Tracks.Add(trackDb);
                        inserted.Add(trackDb);
                        db.SaveChanges();
                        _logger.LogInformation("Song added: " + track.song + ", " + track.artist + ", " + track.album + ", " + track.whenPlayed);
                        counter++;
                    }
                    _logger.LogInformation(counter.ToString() + " added to database");
                }
                _emailService.sendUpdate(TrackInfo(inserted)).Wait();
            }
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
