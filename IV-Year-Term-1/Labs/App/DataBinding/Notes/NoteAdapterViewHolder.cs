using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace App.DataBinding.Notes
{
    public class NoteAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView NameTextView { get; private set; }

        public TextView CreationDateTextView { get; private set; }

        public ImageView ImportanceImageView { get; private set; }

        public ImageView IconImageView { get; private set; }

        public NoteAdapterViewHolder(
            View itemView,
            Action<ViewHolderClickEventArgs> clickListener,
            Action<ViewHolderClickEventArgs> longClickListener) : base(itemView)
        {
            this.NameTextView = itemView.FindViewById<TextView>(Resource.Id.nameTextView);
            this.CreationDateTextView = itemView.FindViewById<TextView>(Resource.Id.creationDateTextView);
            this.ImportanceImageView = itemView.FindViewById<ImageView>(Resource.Id.importanceImageView);
            this.IconImageView = itemView.FindViewById<ImageView>(Resource.Id.iconImageView);

            itemView.Click += (sender, e) => clickListener(new ViewHolderClickEventArgs
            {
                ItemView = itemView,
                Position = this.AdapterPosition
            });

            itemView.LongClick += (sender, e) => longClickListener(new ViewHolderClickEventArgs
            {
                ItemView = itemView,
                Position = this.AdapterPosition
            });
        }
    }
}