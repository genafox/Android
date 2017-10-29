using System;
using Android.Content;
using Android.Database;
using AndroidUri = Android.Net.Uri;
using App.Domain.Interfaces;
using App.IoC;

namespace App.ContentProvidersApi
{
    [ContentProvider(new string[] { NotesContract.Authority })]
    public class NotesProvider : ContentProvider
    {
        private readonly DependencyResolver dependencyResolver;

        private NotesDataSet dataSet;
        private INoteRepository noteRepository;

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
            throw new NotImplementedException();
        }

        public override AndroidUri Insert(AndroidUri uri, ContentValues values)
        {
            throw new NotImplementedException();
        }

        public override ICursor Query(AndroidUri uri, string[] projection, string selection, string[] selectionArgs, string sortOrder)
        {
            throw new NotImplementedException();
        }

        public override int Update(AndroidUri uri, ContentValues values, string selection, string[] selectionArgs)
        {
            throw new NotImplementedException();
        }

        public override int Delete(AndroidUri uri, string selection, string[] selectionArgs)
        {
            throw new NotImplementedException();
        }
    }
}