using Android.Content;
using Android.Database;
using NotesUriConstants = ApiContracts.Notes.UriConstants;
using NotesInterfaceConstants = ApiContracts.Notes.InterfaceConstants;
using System.Collections.Generic;

namespace NoteWidget.DataAccess
{
    public static class NotesContentResolver
    {
        public static IList<NoteModel> Get(Context context)
        {
            var notes = new List<NoteModel>();

            var uri = NotesUriConstants.ContentUri;
            string[] projection = {
                NotesInterfaceConstants.Id,
                NotesInterfaceConstants.Name
            };

            var loader = new CursorLoader(
                context,
                uri,
                projection,
                selection: null,
                selectionArgs: null,
                sortOrder: null);

            var cursor = loader.LoadInBackground() as ICursor;
            if(cursor != null)
            {
                // If cursore is not empty
                if (cursor.MoveToFirst())
                {
                    do
                    {
                        notes.Add(new NoteModel
                        {
                            Id = cursor.GetInt(cursor.GetColumnIndex(projection[0])),
                            Name = cursor.GetString(cursor.GetColumnIndex(projection[1]))
                        });
                    } while (cursor.MoveToNext());
                }
            }

            cursor.Close();

            return notes;
        }
    }
}