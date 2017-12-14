using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using App.Domain.Database.Models;
using App.Services.Binders;

namespace App.Services
{
    [Service]
    [IntentFilter(new string[] { "musicplayer.AudioService" })]
    public class AudioService : Service, MediaPlayer.IOnPreparedListener, MediaPlayer.IOnErrorListener, MediaPlayer.IOnCompletionListener
    {
        private MediaPlayer player;

        public IList<SongViewModel> Songs { set; get; }

        private IBinder musicBind;

        public int SongPosition { set; get; }

        public bool IsRepeat { set; get; }

        public Action ResetSeekBar { get; internal set; }

        public bool IsPlaying => this.player.IsPlaying;

        public int Duration => this.player.Duration;

        public int CurrentPosition => this.player.CurrentPosition;

        public override void OnCreate()
        {
            base.OnCreate();

            this.SongPosition = 0;

            this.musicBind = new AudioServiceBinder(this);

            this.player = new MediaPlayer();
            this.player.SetWakeMode(ApplicationContext, WakeLockFlags.Full);
            this.player.SetAudioStreamType(Stream.Music);

            this.player.SetOnPreparedListener(this);
            this.player.SetOnCompletionListener(this);
            this.player.SetOnErrorListener(this);
        }

        public override IBinder OnBind(Intent intent)
        {
            return musicBind;
        }

        public override bool OnUnbind(Intent intent)
        {
            this.player.Stop();
            this.player.Release();

            return false;
        }

        public void OnCompletion(MediaPlayer mp)
        {
            if (SongPosition < Songs.Count - 1)
            {
                this.SongPosition += 1;
                this.PlaySong();
                this.ResetSeekBar();
            }
            else if (this.IsRepeat)
            {
                this.SongPosition = 0;
                this.PlaySong();
                this.ResetSeekBar();
            }
        }

        public bool OnError(MediaPlayer mp, [GeneratedEnum] MediaError what, int extra)
        {
            return true;
        }

        public void OnPrepared(MediaPlayer mp)
        {
            mp.Start();
        }

        public void SetSong(int songIndex)
        {
            if (songIndex < 0)
            {
                this.SongPosition = 0;
            }
            else if (songIndex < Songs.Count)
            {
                this.SongPosition = songIndex;
            }
            else if (this.IsRepeat)
            {
                this.SongPosition = 0;
            }
        }

        public void PauseSong()
        {
            this.player.Pause();
        }

        public void ContinueSong()
        {
            this.player.Start();
        }

        public void PlaySong()
        {
            this.player.Reset();

            var playSong = this.Songs[SongPosition];
            long currentSongId = playSong.Id;
            var trackUri = ContentUris.WithAppendedId(MediaStore.Audio.Media.ExternalContentUri, currentSongId);

            try
            {
                this.player.SetDataSource(this.ApplicationContext, trackUri);
            }
            catch (Exception e)
            {
                Console.WriteLine($"MUSIC SERVICE: Error setting data source. {e.Message}");
            }

            this.player.Prepare();
        }
    }
}