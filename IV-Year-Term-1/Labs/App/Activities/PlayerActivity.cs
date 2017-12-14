
using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Support.V7.Widget;
using Android.Telephony;
using Android.Widget;
using App.Connections;
using App.DataBinding;
using App.DataBinding.Player;
using App.Domain.Database.Models;
using App.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Timers;

namespace App.Activities
{
    [Activity(Label = "@string/player_label", MainLauncher = false)]
    public class PlayerActivity : Activity
    {
        private readonly Random random = new Random();

        private PlayerServiceConnection playerServiceConnection;
        private Intent playerServiceIntent;
        private Notification.Builder playerNotificationBuilder;

        private IList<SongViewModel> songs;

        // Recycler View
        private SongAdapter songAdapter;

        // Progress Bar
        private SeekBar audioSeekBar;
        private TextView currentSongTitleTextView;

        // Controls
        private ImageButton repeatBtn;
        private ImageButton previousTrackBtn;
        private ImageButton playPauseBtn;
        private ImageButton nextTrackBtn;
        private ImageButton shuffleBtn;

        public void UpdateCallState(CallState state, string incomingNumber)
        {
            if (this.AudioService != null)
            {
                switch (state)
                {
                    case CallState.Ringing:
                        this.AudioService.PauseSong();
                        break;
                    case CallState.Offhook:
                        this.AudioService.PauseSong();
                        break;
                    case CallState.Idle:
                        this.AudioService.ContinueSong();
                        this.ReloadSeekBar();
                        break;
                }
            }
        }

        public AudioService AudioService { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.Activity_Player);

            this.playerNotificationBuilder = new Notification.Builder(this);

            //Initialize RecyclerView
            var recyclerView = this.FindViewById<RecyclerView>(Resource.Id.songsListRecyclerView);

            // Instantiate the adapter and pass in its data source:
            this.songAdapter = new SongAdapter(new ObservableCollection<SongViewModel>());
            this.songAdapter.ItemClick += this.OnSongClick;

            // Plug the adapter into the RecyclerView:
            recyclerView.SetAdapter(this.songAdapter);

            // Instantiate the layout manager:
            var layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);

            this.songs = this.GetSongs();
            this.songAdapter.SetData(new ObservableCollection<SongViewModel>(this.songs));

            // Progress Bar
            this.audioSeekBar = this.FindViewById<SeekBar>(Resource.Id.audioSeekBar);
            this.currentSongTitleTextView = this.FindViewById<TextView>(Resource.Id.currentSongTitleTextView);

            //Controls
            this.repeatBtn = this.FindViewById<ImageButton>(Resource.Id.repeatBtn);
            this.repeatBtn.Click += RepeatBtn_Click;

            this.previousTrackBtn = this.FindViewById<ImageButton>(Resource.Id.previousTrackBtn);
            this.previousTrackBtn.Click += PreviousTrackBtn_Click;

            this.playPauseBtn = this.FindViewById<ImageButton>(Resource.Id.playPauseBtn);
            this.playPauseBtn.Click += PlayPauseBtn_Click;

            this.nextTrackBtn = this.FindViewById<ImageButton>(Resource.Id.nextTrackBtn);
            this.nextTrackBtn.Click += NextTrackBtn_Click;

            this.shuffleBtn= this.FindViewById<ImageButton>(Resource.Id.shuffleBtn);
            this.shuffleBtn.Click += ShuffleBtn_Click;
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (playerServiceIntent == null)
            {
                var playerServiceIntent = new Intent(this, typeof(AudioService));
                playerServiceConnection = new PlayerServiceConnection(this, songs, ReloadSeekBar);
                this.BindService(playerServiceIntent, playerServiceConnection, Bind.AutoCreate);
                this.StartService(playerServiceIntent);
            }
        }

        protected override void OnDestroy()
        {
            this.StopService(playerServiceIntent);
            this.AudioService = null;
            base.OnDestroy();
        }

        private void ShuffleBtn_Click(object sender, EventArgs e)
        {
            this.ShuffleSongs();
        }

        private void NextTrackBtn_Click(object sender, EventArgs e)
        {
            this.AudioService.SetSong(this.AudioService.SongPosition + 1);
            this.AudioService.PlaySong();
            ReloadSeekBar();
        }

        private void PlayPauseBtn_Click(object sender, EventArgs e)
        {
            if (this.AudioService.IsPlaying)
            {
                this.playPauseBtn.SetImageResource(Resource.Drawable.pause_icon);
                this.AudioService.PauseSong();
            }
            else
            {
                this.playPauseBtn.SetImageResource(Resource.Drawable.player_icon);
                this.AudioService.ContinueSong();
                this.ReloadSeekBar();
            }
        }

        private void PreviousTrackBtn_Click(object sender, EventArgs e)
        {
            this.AudioService.SetSong(this.AudioService.SongPosition - 1);
            this.AudioService.PlaySong();
            ReloadSeekBar();
        }

        private void RepeatBtn_Click(object sender, EventArgs e)
        {
            if (this.AudioService.IsRepeat)
            {
                this.AudioService.IsRepeat = false;
                this.repeatBtn.SetImageResource(Resource.Drawable.repeat_icon);
            }
            else
            {
                this.AudioService.IsRepeat = true;
                this.repeatBtn.SetImageResource(Resource.Drawable.repeatActive_icon);
            }
        }

        private void OnSongClick(object sender, AdapterClickEventArgs<SongViewModel> e)
        {
            this.playPauseBtn.SetImageResource(Resource.Drawable.player_icon);
            this.AudioService.SetSong(e.Position);
            this.AudioService.PlaySong();
            this.ReloadSeekBar();
        }

        private IList<SongViewModel> GetSongs()
        {
            var songsModels = new List<SongViewModel>();

            var musicUri = MediaStore.Audio.Media.ExternalContentUri;
            var musicCursor = this.ContentResolver.Query(musicUri, null, null, null, null);

            if (musicCursor != null && musicCursor.MoveToFirst())
            {
                int titleColumn = musicCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Title);
                int idColumn = musicCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Id);
                int artistColumn = musicCursor.GetColumnIndex(MediaStore.Audio.Media.InterfaceConsts.Artist);

                do
                {
                    var thisId = musicCursor.GetLong(idColumn);
                    var thisTitle = musicCursor.GetString(titleColumn);
                    var thisArtist = musicCursor.GetString(artistColumn);

                    songsModels.Add(new SongViewModel()
                    {
                        Id = thisId,
                        Title = thisTitle,
                        Artist = thisArtist
                    });
                }
                while (musicCursor.MoveToNext());
            }

            return songsModels.OrderBy(x => x.Title).ToList();
        }

        private void ShuffleSongs()
        {
            int n = songs.Count;

            while (n > 1)
            {
                n--;
                int k = this.random.Next(n + 1);
                var value = songs[k];
                songs[k] = songs[n];
                songs[n] = value;
            }

            this.AudioService.Songs = songs;
            this.AudioService.SetSong(0);
            this.AudioService.PlaySong();
            ReloadSeekBar();
        }

        public void ReloadSeekBar()
        {
            SongViewModel currentSong = this.AudioService.Songs[this.AudioService.SongPosition];
            this.currentSongTitleTextView.Text = currentSong.Title;

            this.audioSeekBar.Max = this.AudioService.Duration;

            var timer = new Timer();
            timer.Interval = 100;
            timer.Elapsed += (o, e) =>
            {
                this.audioSeekBar.Progress = this.AudioService.CurrentPosition;
                this.UpdatePlayerNotification();
            };

            timer.Enabled = true;
        }

        private void UpdatePlayerNotification()
        {
            var currentSong = this.AudioService.Songs[this.AudioService.SongPosition];

            playerNotificationBuilder.SetContentTitle(currentSong.Title)
                .SetContentText(currentSong.Artist)
                .SetSmallIcon(Resource.Drawable.player_icon)
                .SetProgress(this.AudioService.Duration, this.AudioService.CurrentPosition, false);

            // Build the notification:
            Notification notification = playerNotificationBuilder.Build();

            // Get the notification manager:
            var notificationManager = this.GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = 1;
            notificationManager.Notify(notificationId, notification);
        }
    }
}