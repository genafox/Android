using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using App.Domain.Database;

namespace App.ContentProvidersApi
{
    public class NotesDataSet : SQLiteOpenHelper
    {
        public NotesDataSet(Context context) : base(
            context,
            Database.DatabaseFullPath,
            null, 
            Database.Version)
        {
        }

        public override void OnCreate(SQLiteDatabase db)
        {
        }

        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
        }
    }
}