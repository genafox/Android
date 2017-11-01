using Android.Content;
using Android.Database;
using App.Domain.Interfaces;
using App.IoC;
using Java.Lang;
using App.Domain.Database;
using ApiContracts.Notes;
using AndroidUri = Android.Net.Uri;

namespace App.ContentProvidersApi
{
    [ContentProvider(new string[] { UriConstants.Authority }, Enabled = true, Exported = true)]
    public class NotesProvider : ContentProvider
    {
        private static readonly UriMatcher UriMatcher;
        private readonly DependencyResolver dependencyResolver;

        private NotesDataSet dataSet;
        private INoteRepository noteRepository;

        static NotesProvider()
        {
            UriMatcher = new UriMatcher(UriMatcher.NoMatch);
            UriMatcher.AddURI(UriConstants.Authority, UriConstants.BasePath, UriConstants.GetAll);
            UriMatcher.AddURI(UriConstants.Authority, UriConstants.BasePath + "/#", UriConstants.GetOne);
        }

        public NotesProvider()
        {
            this.dependencyResolver = new DependencyResolver();
        }

        ~NotesProvider()
        {
            this.dependencyResolver.Dispose();
        }

        public override bool OnCreate()
        {
            this.dataSet = new NotesDataSet(this.Context);
            this.noteRepository = dependencyResolver.Resolve<INoteRepository>();

            return true;
        }

        public override string GetType(AndroidUri uri)
        {
            switch (UriMatcher.Match(uri))
            {
                case UriConstants.GetAll:
                    return UriConstants.NotesMimeType;
                case UriConstants.GetOne:
                    return UriConstants.SingleNoteMimeType;
                default:
                    throw new IllegalArgumentException("Unknown Uri: " + uri);
            }
        }

        public override ICursor Query(AndroidUri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
        {
            switch (UriMatcher.Match(uri))
            {
                case UriConstants.GetAll:
                    return GetAllNotes(selection);
                case UriConstants.GetOne:
                    return GetNoteById(uri);
                default:
                    throw new IllegalArgumentException("Unknown Uri: " + uri);
            }
        }

        public override AndroidUri Insert(AndroidUri uri, ContentValues values)
        {
            throw new UnsupportedOperationException();
        }

        public override int Update(AndroidUri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            throw new UnsupportedOperationException();
        }

        public override int Delete(AndroidUri uri, string selection, string[] selectionArgs)
        {
            throw new UnsupportedOperationException();
        }

        private ICursor GetAllNotes(string selection = null)
        {
            string getAllQuery = $@"SELECT Id, Name FROM {DbConstants.NotesTableName} {selection ?? string.Empty}";
            ICursor data = this.dataSet.ReadableDatabase.RawQuery(getAllQuery, null);

            return data;
        }

        private ICursor GetNoteById(AndroidUri uri)
        {
            var id = uri.LastPathSegment;
            string getByIdQuery = $"SELECT Id, Name FROM {DbConstants.NotesTableName} WHERE Id = {id}";
            ICursor data = this.dataSet.ReadableDatabase.RawQuery(getByIdQuery, null);

            return data;
        }
    }
}