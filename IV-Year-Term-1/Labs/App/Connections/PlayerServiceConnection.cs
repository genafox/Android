using System;
using System.Collections.Generic;
using Android.Content;
using Android.OS;
using App.Activities;
using App.Domain.Database.Models;
using App.Services;
using App.Services.Binders;

namespace App.Connections
{
    public class PlayerServiceConnection : Java.Lang.Object, IServiceConnection
    {
        private PlayerActivity activity;
        private IList<SongViewModel> songs;
        private Action resetSeekBar;

        public AudioService AudioService { set; get; }

        public PlayerServiceConnection(PlayerActivity activity, IList<SongViewModel> songs, Action resetSeekBar)
        {
            this.activity = activity;
            this.songs = songs;
            this.resetSeekBar = resetSeekBar;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var binder = (AudioServiceBinder)service;

            this.AudioService = binder.GetAudioService();
            this.AudioService.Songs = songs;
            this.AudioService.ResetSeekBar = resetSeekBar;

            this.activity.AudioService = this.AudioService;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
        }
    }
}