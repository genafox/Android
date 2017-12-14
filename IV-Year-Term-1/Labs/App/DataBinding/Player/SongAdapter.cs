using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Android.Views;
using Android.Support.V7.Widget;
using App.Domain.Database.Models;

namespace App.DataBinding.Player
{
    public class SongAdapter : RecyclerView.Adapter
    {
        private ObservableCollection<SongViewModel> songs;

        public event EventHandler<AdapterClickEventArgs<SongViewModel>> ItemClick;

        public SongAdapter(ObservableCollection<SongViewModel> data)
        {
            this.SetData(data);
        }

        public override int ItemCount => this.songs.Count;


        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = LayoutInflater
                .From(parent.Context)
                .Inflate(Resource.Layout.layout_SongItem, parent, false);

            return new SongAdapterViewHolder(itemView, this.OnClick);
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            SongViewModel song = this.songs[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as SongAdapterViewHolder;
            if (holder != null)
            {
                holder.TitleTextView.Text = song.Title;
                holder.ArtistTextView.Text = song.Artist;
            }
        }

        public void SetData(ObservableCollection<SongViewModel> data)
        {
            this.songs = data;
            this.songs.CollectionChanged += OnNotesCollectionChanged;
            this.NotifyDataSetChanged();
        }

        private void OnClick(ViewHolderClickEventArgs args)
        {
            this.ItemClick?.Invoke(this, new AdapterClickEventArgs<SongViewModel>
            {
                ItemView = args.ItemView,
                Item = this.songs[args.Position],
                Position = args.Position
            });
        }

        private void OnNotesCollectionChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            switch (eventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    this.NotifyItemInserted(eventArgs.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    this.NotifyItemRemoved(eventArgs.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    this.NotifyItemChanged(eventArgs.OldStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Move:
                    this.NotifyItemMoved(eventArgs.OldStartingIndex, eventArgs.NewStartingIndex);
                    break;
            }
        }
    }
}