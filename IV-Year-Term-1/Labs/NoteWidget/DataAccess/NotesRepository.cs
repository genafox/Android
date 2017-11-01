using Android.Content;
using Android.Database;
using NotesUriConstants = ApiContracts.Notes.UriConstants;
using NotesInterfaceConstants = ApiContracts.Notes.InterfaceConstants;
using System.Collections.Generic;
using System;

namespace NoteWidget.DataAccess
{
    public static class NotesRepository
    {
        public static IList<NoteModel> Get(Context context)
        {
            var notes = new List<NoteModel>();

            var uri = NotesUriConstants.ContentUri;
            string[] projection = {
                NotesInterfaceConstants.Id,
                NotesInterfaceConstants.Name
            };

            using (var cursor = context.ContentResolver.Query(
                uri,
                projection,
                selection: $"WHERE ExpirationDate >= {DateTime.Now.Ticks}",
                selectionArgs: null,
                sortOrder: null))
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

            return notes;
        }
    }
}