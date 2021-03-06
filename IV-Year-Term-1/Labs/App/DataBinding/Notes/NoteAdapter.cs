﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Android.Views;
using Android.Support.V7.Widget;
using App.Domain.Database.Models;
using App.Helpers;
using App.IoC;
using App.Domain.Interfaces;
using Uri = Android.Net.Uri;

namespace App.DataBinding.Notes
{
    public class NoteAdapter : RecyclerView.Adapter
    {
        private ObservableCollection<Note> notes;

        public event EventHandler<AdapterClickEventArgs<Note>> ItemClick;
        public event EventHandler<AdapterClickEventArgs<Note>> ItemLongClick;

        public NoteAdapter(ObservableCollection<Note> data)
        {
            this.SetData(data);
        }

        public override int ItemCount => this.notes.Count;


        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            //Setup your layout here
            View itemView = LayoutInflater
                .From(parent.Context)
                .Inflate(Resource.Layout.Layout_NoteItem, parent, false);

            using (var dr = new DependencyResolver())
            {
                var settings = dr.Resolve<ISettingsService>().Get();
                AppearanceHelper.ApplySettings(itemView, parent.Context, settings);
            }

            return new NoteAdapterViewHolder(itemView, this.OnClick, this.OnLongClick);
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            Note note = this.notes[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as NoteAdapterViewHolder;
            if (holder != null)
            {
                holder.NameTextView.Text = note.Name;
                holder.CreationDateTextView.Text = note.CreationDate.ToShortDateString();
                holder.ImportanceImageView.SetImageResource(note.Importance.GetIconResource());

                if (!string.IsNullOrEmpty(note.IconPath))
                {
                    Uri iconUri = Uri.Parse(note.IconPath);
                    holder.IconImageView.SetImageURI(iconUri);
                }
            }
        }

        public void SetData(ObservableCollection<Note> data)
        {
            this.notes = data;
            this.notes.CollectionChanged += OnNotesCollectionChanged;
            this.NotifyDataSetChanged();
        }

        private void OnClick(ViewHolderClickEventArgs args)
        {
            this.ItemClick?.Invoke(this, new AdapterClickEventArgs<Note>
            {
                ItemView = args.ItemView,
                Item = this.notes[args.Position],
                Position = args.Position
            });
        }

        private void OnLongClick(ViewHolderClickEventArgs args)
        {
            this.ItemLongClick?.Invoke(this, new AdapterClickEventArgs<Note>
            {
                ItemView = args.ItemView,
                Item = this.notes[args.Position],
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