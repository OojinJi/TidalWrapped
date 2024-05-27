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
        public DataService(ILogger<DataService> logger) 
        {
            _logger = logger;
        }
        public void InsertData(List<Task<TrackModel>> tracks)
        {

            using(var db = new TidalDbContext())
            {
                int counter = 0;
                var trackList = tracks.ToList();
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
                        db.SaveChanges();
                        _logger.LogInformation("Song added: " + track.song + ", " + track.artist + ", " + track.album + ", " + track.whenPlayed);
                        counter++;
                    }
                    _logger.LogInformation(counter.ToString() + " added to database");
                }

            }
        }
    }
}
