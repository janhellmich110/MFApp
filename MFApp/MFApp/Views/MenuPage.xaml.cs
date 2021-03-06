﻿using MFApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MFApp.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();

            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Home, Title="Startseite" },
                new HomeMenuItem {Id = MenuItemType.Birdiebook, Title="Birdiebook" },
                new HomeMenuItem {Id = MenuItemType.Player, Title="Spieler" },
                new HomeMenuItem {Id = MenuItemType.Results, Title="Ergebnisse" },                
                new HomeMenuItem {Id = MenuItemType.LogOff, Title="Mein Profil" },
                new HomeMenuItem {Id = MenuItemType.About, Title="Infos" },
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemTapped += async (sender, e) =>
            {
                if (e.Item == null)
                    return;

                var id = (int)((HomeMenuItem)e.Item).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }
    }
}