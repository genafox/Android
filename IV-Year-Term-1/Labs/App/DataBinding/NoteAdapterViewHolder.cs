using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace App.DataBinding
{
    public class NoteAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView NameTextView { get; private set; }

        public TextView CreationDateTextView { get; private set; }

        public ImageView ImportanceImageView { get; private set; }

        public ImageView IconImageView { get; private set; }

        public NoteAdapterViewHolder(
            View itemView, 
            Action<NoteAdapterClickEventArgs> clickListener,
            Action<NoteAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            NameTextView = itemView.FindViewById<TextView>(Resource.Id.nameTextView);
            CreationDateTextView = itemView.FindViewById<TextView>(Resource.Id.creationDateTextView);
            ImportanceImageView = itemView.FindViewById<ImageView>(Resource.Id.importanceImageView);
            IconImageView = itemView.FindViewById<ImageView>(Resource.Id.iconImageView);

            itemView.Click += (sender, e) => clickListener(new NoteAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new NoteAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
        }
    }
}