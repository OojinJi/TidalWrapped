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
        public void gernerateDaySum(List<Task<TrackModel>> tracks)
        {
            List<DaySum> daySums = new List<DaySum>();
            using (var db = new TidalDbContext())
            {
                foreach (var trackd in tracks)
                {
                    var track = trackd.Result;
                    if(track.whenPlayed != DateTime.MinValue)
                    {
                        Track inTrack = new Track()
                        {
                            song = track.song,
                            artist = track.artist,
                            album = track.album,
                            whenPlayed = track.whenPlayed,
                            trackPage = track.trackPage
                        };

                        var existDayList = daySums.FirstOrDefault(x => x.Date == inTrack.whenPlayed.Date);
                        if (existDayList == null)
                        {
                            DaySum curDay = new DaySum()
                            {
                                Date = track.whenPlayed.Date,
                                Tracks = new List<Track>()
                            };
                            curDay.Tracks.Add(inTrack);
                            curDay.TotalListens++;
                            daySums.Add(curDay);

                        }
                        else if (existDayList != null)
                        {
                            existDayList.Tracks.Add(inTrack);
                            existDayList.TotalListens++;
                        }
                    }
                }
            }
            insertDaySums(daySums);
        }

        private void insertDaySums(List<DaySum> daySums)
        {
            using (var db = new TidalDbContext())
            {
                List<DaySum> testdaySums = new List<DaySum>();
                foreach (DaySum daySum in daySums)
                {
                    var dayExists = db.DaySums.FirstOrDefault(x => x.Date == daySum.Date);

                    DaySum insertDay = new DaySum();

                    if (dayExists == null)
                    {
                        testdaySums.Add(getMostActive(daySum));
                        daySum.MostActive = getMostActive(daySum).MostActive;
                        insertDay = daySum;
                        db.DaySums.Add(insertDay);
                    }
                    else
                    {
                        dayExists.Tracks = db.Tracks.Where(x => x.DayID == dayExists.Id).ToList();
                        foreach (Track track in daySum.Tracks)
                        {
                            var trackExists = db.Tracks.FirstOrDefault(x => x.song == track.song && x.artist == track.artist && x.album == track.album && x.whenPlayed == track.whenPlayed); ;
                            if (trackExists == null)
                            {
                                dayExists.Tracks.Add(track);
                                dayExists.TotalListens++;
                            }
                        }
                        dayExists.MostActive = getMostActive(dayExists).MostActive;
                        insertDay = dayExists;
                    }
                    if(insertDay.Date == DateTime.Now.Date.AddDays(-1))
                    {
                        if(insertDay.Tracks.Count > 0)
                        {
                            HourCount mostplayed = JsonConvert.DeserializeObject<List<HourCount>>(insertDay.MostActive).OrderByDescending(x => x.count).ToList()[0];
                            _emailService.sendUpdate(TrackInfo(insertDay.Tracks.OrderByDescending(x => x.whenPlayed).ToList()), insertDay.TotalListens, mostplayed).Wait();
                        }
                    }
                }
                db.SaveChanges();
            }
        }

        private DaySum getMostActive(DaySum daySum)
        {
            var tracks = daySum.Tracks.ToList();
            var numOfTracks = 0;
            var numOfTracksold = 0;
            var counter = 0;
            var lastHour = 0;
            tracks = tracks.OrderByDescending(x => x.whenPlayed).ToList();
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
            var mostActiveObj = JsonConvert.SerializeObject(MostActive);
            daySum.MostActive = mostActiveObj;
            return daySum;
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
