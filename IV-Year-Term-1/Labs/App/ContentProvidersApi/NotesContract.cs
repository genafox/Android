using Android.Content;
using System;
using AndroidUri = Android.Net.Uri;

namespace App.ContentProvidersApi
{
    public static class NotesContract
    {
        private static string BasePath = "notes";

        public const string Authority = "com.xamarin.sample.NotesContentProvider";

        public static readonly AndroidUri ContentUri = AndroidUri.Parse($"content://{Authority}/{BasePath}");

        // MIME types used for getting a list, or a single note
        public const string NotesMimeType = ContentResolver.CursorDirBaseType + "/vnd.com.xamarin.sample.Vegetables";
        public const string SingleNoteMimeType = ContentResolver.CursorItemBaseType + "/vnd.com.xamarin.sample.Vegetables";

        // Column names
        public const string Id = "_id";
        public const string Name = "name";
    }
}