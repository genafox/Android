using Android.Content;
using Android.Views;
using Android.Widget;
using NoteWidget.DataAccess;
using System.Collections.Generic;

namespace NoteWidget.DataBinding
{
    public class NotesListAdapter : BaseAdapter<NoteModel>
    {
        private readonly IList<NoteModel> notes;
        private readonly Context context;

        public NotesListAdapter(IList<NoteModel> notes, Context context)
        {
            this.notes = notes;
            this.context = context;
        }

        public override NoteModel this[int position] => notes[position];

        public override int Count => notes.Count;

        public override long GetItemId(int position)
        {
            return this[position].Id;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            // Re-use an existing view, if one is supplied
            View view = convertView;

            // Otherwise create a new one
            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
            }

            view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = items[position];

            return view;
        }
    }
}