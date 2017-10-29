using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Database.Sqlite;
using Android.Database;
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