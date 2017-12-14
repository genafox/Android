using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace App.DataBinding.Player
{
    public class SongAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView TitleTextView { get; private set; }

        public TextView ArtistTextView { get; private set; }

        public SongAdapterViewHolder(
            View itemView,
            Action<ViewHolderClickEventArgs> clickListener) : base(itemView)
        {
            this.TitleTextView = itemView.FindViewById<TextView>(Resource.Id.songTitleTextView);
            this.ArtistTextView = itemView.FindViewById<TextView>(Resource.Id.artistTextView);

            itemView.Click += (sender, e) => clickListener(new ViewHolderClickEventArgs
            {
                ItemView = itemView,
                Position = this.AdapterPosition
            });
        }
    }
}