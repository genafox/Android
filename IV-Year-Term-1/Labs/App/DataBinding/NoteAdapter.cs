using System;

using Android.Views;
using Android.Support.V7.Widget;
using App.Domain.Models;

namespace App.DataBinding
{
    public class NoteAdapter : RecyclerView.Adapter
    {
        private readonly Note[] notes;

        public event EventHandler<NoteAdapterClickEventArgs> ItemClick;
        public event EventHandler<NoteAdapterClickEventArgs> ItemLongClick;

        public NoteAdapter(Note[] data)
        {
            this.notes = data;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView =  LayoutInflater
                .From(parent.Context)
                .Inflate(Resource.Layout.NoteItem, parent, false);

            return new NoteAdapterViewHolder(itemView, this.OnClick, this.OnLongClick);
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            Note note = notes[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as NoteAdapterViewHolder;
            if (holder != null)
            {
                holder.NameTextView.Text = note.Name;
                holder.CreationDateTextView.Text = note.CreationDate.ToShortDateString();
                holder.ImportanceImageView.SetImageResource(GetImportanceIconResource(note.Importance));
                //holder.IconImageView.SetImageBitmap();
            }
        }

        public override int ItemCount => notes.Length;

        void OnClick(NoteAdapterClickEventArgs args)
        {
            ItemClick?.Invoke(this, args);
        }

        void OnLongClick(NoteAdapterClickEventArgs args)
        {
            ItemLongClick?.Invoke(this, args);
        }

        private int GetImportanceIconResource(NoteImportance importance)
        {
            switch (importance)
            {
                case NoteImportance.High:
                    return Resource.Drawable.note_importance_high_icon;
                case NoteImportance.Medium:
                    return Resource.Drawable.note_importance_medium_icon;
                case NoteImportance.Low:
                default:
                    return Resource.Drawable.note_importance_low_icon;
            }
        }
    }
}