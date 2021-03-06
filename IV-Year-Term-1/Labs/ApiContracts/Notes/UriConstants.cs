﻿using Android.Content;
using AndroidUri = Android.Net.Uri;

namespace ApiContracts.Notes
{
    public static class UriConstants
    {
        public const string BasePath = "notes";

        public const string Authority = "com.xamarin.sample.NotesContentProvider";

        public static readonly AndroidUri ContentUri = AndroidUri.Parse($"content://{Authority}/{BasePath}");

        // MIME types used for getting a list, or a single note
        public const string NotesMimeType = ContentResolver.CursorDirBaseType + "/vnd.com.xamarin.sample.Vegetables";
        public const string SingleNoteMimeType = ContentResolver.CursorItemBaseType + "/vnd.com.xamarin.sample.Vegetables";

        // URIs
        public const int GetAll = 0;
        public const int GetOne = 1;
    }
}