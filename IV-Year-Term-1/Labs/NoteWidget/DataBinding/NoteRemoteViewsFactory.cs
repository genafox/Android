using Android.Content;
using Android.Widget;
using NoteWidget.DataAccess;
using System.Collections.Generic;
using Android.OS;
using IRemoteViewsFactory = Android.Widget.RemoteViewsService.IRemoteViewsFactory;

namespace NoteWidget.DataBinding
{
    public class NoteRemoteViewsFactory : Java.Lang.Object, IRemoteViewsFactory
    {
        public const string NoteItemIdKey = "NoteItemIdKey";

        private readonly Context context;
        private IList<NoteModel> notes;

        public NoteRemoteViewsFactory(Context context)
        {
            this.context = context;
            this.notes = NotesRepository.Get(this.context);
        }

        public int Count => this.notes.Count;

        // Indicates whether the item ids are stable across changes to the underlying data.
        // True if the same id always refers to the same object.
        public bool HasStableIds => true;

        // This allows for the use of a custom loading view which appears between the time that GetViewAt(int) is called and returns.
        public RemoteViews LoadingView => null;

        // Returns the number of types of Views that will be created by GetViewAt(int).
        // If the adapter always returns the same type of View for all items, this method should return 1.
        public int ViewTypeCount => 1;

        public long GetItemId(int position)
        {
            return this.notes[position].Id;
        }

        public RemoteViews GetViewAt(int position)
        {
            NoteModel note = this.notes[position];

            RemoteViews rv = new RemoteViews(this.context.PackageName, Resource.Layout.NoteListItem);
            rv.SetTextViewText(Resource.Id.noteNameTextView, note.Name);

            var extras = new Bundle();
            extras.PutInt(NoteItemIdKey, note.Id);

            Intent fillInIntent = new Intent();
            fillInIntent.PutExtras(extras);

            // This intent will fill the IntentTemplate of list item click with extras of the particular item
            rv.SetOnClickFillInIntent(Resource.Id.noteNameTextView, fillInIntent);

            return rv;
        }

        public void OnCreate()
        {
        }

        public void OnDataSetChanged()
        {
        }

        public void OnDestroy()
        {
        }
    }
}