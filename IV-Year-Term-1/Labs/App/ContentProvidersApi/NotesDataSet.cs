using System;
using Android.Content;
using Android.Database.Sqlite;
using App.Domain.Database;

namespace App.ContentProvidersApi
{
    public class NotesDataSet : SQLiteOpenHelper
    {
        public NotesDataSet(Context context) : base(
            context,
            Database.Name,
            null, 
            Database.Version)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
            throw new NotImplementedException();
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            throw new NotImplementedException();
        }
    }
}