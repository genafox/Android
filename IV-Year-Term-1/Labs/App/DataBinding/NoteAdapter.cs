using System;
using Android.Views;
using Android.Support.V7.Widget;
using App.Domain.Models;
using App.Helpers;

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

		public override int ItemCount => this.notes.Length;


		// Create new views (invoked by the layout manager)
		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			//Setup your layout here
			View itemView = LayoutInflater
				.From(parent.Context)
				.Inflate(Resource.Layout.Layout_NoteItem, parent, false);

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
				//holder.IconImageView.SetImageBitmap();
			}
		}
		private void OnClick(NoteViewHolderClickEventArgs args)
		{
			this.ItemClick?.Invoke(this, new NoteAdapterClickEventArgs
			{
				Note = this.notes[args.Position],
				Position = args.Position
			});
		}

		private void OnLongClick(NoteViewHolderClickEventArgs args)
		{
			this.ItemLongClick?.Invoke(this, new NoteAdapterClickEventArgs
			{
				Note = this.notes[args.Position],
				Position = args.Position
			});
		}
	}
}