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
                    tp.Selected = true;
                    //SelectedPlayers.Add(tp);
                }

                AllPlayers.Add(tp);
            }
        }
        public Event TournamentEvent { get; set; }

        public Tournament Tournament { get; set; }

        public ObservableCollection<TournamentPlayer> AllPlayers{ get; set;}

        public ObservableCollection<TournamentPlayer> SelectedPlayers { get; set;}
    }
}