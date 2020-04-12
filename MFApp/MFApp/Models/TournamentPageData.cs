﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;

using MFApp.Services;

namespace MFApp.Models
{
    public class TournamentPageData
    {
        public TournamentPageData()
        {
            AllPlayers = new ObservableCollection<TournamentPlayer>();
            SelectedPlayers = new ObservableCollection<TournamentPlayer>();
        }
        public TournamentPageData(Tournament tournament)
        {
            Tournament = tournament;

            // get event
            // get Tournament
            IDataStore<Event> DataStoreEvent = DependencyService.Get<IDataStore<Event>>();
            var EventTask = DataStoreEvent.GetItemsAsync();
            List<Event> Events = EventTask.Result.ToList();

            foreach (Event e in Events)
            {
                if (e.Id == tournament.EventId)
                {
                    TournamentEvent = e;
                    break;
                }
            }

            // get course
            IDataStore<Course> DataStoreCourse = DependencyService.Get<IDataStore<Course>>();
            var CourseTask = DataStoreCourse.GetItemsAsync();
            List<Course> Courses = CourseTask.Result.ToList();

            foreach (Course c in Courses)
            {
                if (Tournament.CourseId == c.Id)
                {
                    Course = c;
                    break;
                }
            }

            // get list of tees
            IDataStore<Tee> DataStoreTee = DependencyService.Get<IDataStore<Tee>>();
            var TeeTask = DataStoreTee.GetItemsAsync();
            List<Tee> Tees = TeeTask.Result.ToList();

            TeeList = Tees.Where(x => x.CourseId == Course.Id).OrderBy(y => y.Name).ToList();

            AllPlayers = new ObservableCollection<TournamentPlayer>();
            SelectedPlayers = new ObservableCollection<TournamentPlayer>();

            // get current player
            Profile CurrentProfile = null;

            IDataStore<Profile> DataStoreProfile = DependencyService.Get<IDataStore<Profile>>();
            var profilesTask = DataStoreProfile.GetItemsAsync();
            var profiles = profilesTask.Result;

            foreach (Profile p in profiles)
            {
                CurrentProfile = p;
                break;
            }

            // find existing flight
            List<int> FlightPlayerIds = new List<int>();
            int flightNumber = Tournament.Id * 10000 + CurrentProfile.Id;

            IDataStore<Flight2Player> DataStoreFlight2Player = DependencyService.Get<IDataStore<Flight2Player>>();
            var Flight2PlayerTask = DataStoreFlight2Player.GetItemsAsync();
            List<Flight2Player> Flight2Players = Flight2PlayerTask.Result.ToList();

            FlightPlayerIds = Flight2Players.Where(x => x.FlightId == flightNumber).Select(x=>x.PlayerId).ToList();

            if(FlightPlayerIds.Count() == 0)
            {
                // no player in flight, add current player
                FlightPlayerIds.Add(CurrentProfile.Id);
            }
            // Fill All Players
            IDataStore<Player> DataStore = DependencyService.Get<IDataStore<Player>>();
            var PlayerTask = DataStore.GetItemsAsync();
            List<Player> players = PlayerTask.Result.ToList();

            foreach (Player p in players)
            {
                TournamentPlayer tp = new TournamentPlayer
                {
                    Id=p.Id,
                    UserName=p.UserName,
                    Name=p.Name,
                    Initials=p.Initials,
                    Handicap=p.Handicap,
                    Mail=p.Mail,
                    Gender=p.Gender,
                    CourseHandicap = (int)p.Handicap,
                    Selected=false
                };

                if(CurrentProfile.UserName.ToLower() == tp.UserName.ToLower())
                {
                    CurrentPlayer = p;
                }

                if (FlightPlayerIds.Count() > 0)
                {
                    foreach(int i in FlightPlayerIds)
                    {
                        if(i==tp.Id)
                        {
                            tp.Selected = true;
                            // add to selected players
                            SelectedPlayers.Add(tp);
                            break;
                        }
                    }
                }

                AllPlayers.Add(tp);
            }
        }

        public Player CurrentPlayer { get; set; }

        public Event TournamentEvent { get; set; }

        public Tournament Tournament { get; set; }

        public Course Course { get; set; }

        public List<Tee> TeeList { get; set; }

        public ObservableCollection<TournamentPlayer> AllPlayers{ get; set;}

        public ObservableCollection<TournamentPlayer> SelectedPlayers { get; set;}
    }
}
