using System;
using System.Linq;
using System.Runtime.Serialization;
using Android.App;
using Android.OS;
using Android.Provider;
using App.DataBinding;
using App.Domain.Interfaces;
using App.Domain.Repositories;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using App.Domain.Models;
using App.Fragments;

namespace App
{
    [Activity(Label = "@string/notes_label", MainLauncher = false)]
    public class NotesActivity : Activity
    {
        private INoteRepository noteRepository;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.noteRepository = new InMemoryNoteRepository();

            SetContentView(Resource.Layout.Notes);

            // Get our RecyclerView layout:
            var recyclerView = FindViewById<RecyclerView>(Resource.Id.notesListRecyclerView);

            // Instantiate the adapter and pass in its data source:
            var adapter = new NoteAdapter(this.noteRepository.GetAll().ToArray());
            adapter.ItemClick += OnNoteClick;
            adapter.ItemLongClick += OnNoteLongClick;

            // Plug the adapter into the RecyclerView:
            recyclerView.SetAdapter(adapter);

            // Instantiate the layout manager:
            var layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.action_bar, menu);

            return true;
        }

        private void OnNoteClick(object sender, NoteAdapterClickEventArgs noteAdapterClickEventArgs)
        {

        }

        private void OnNoteLongClick(object sender, NoteAdapterClickEventArgs noteAdapterClickEventArgs)
        {

        }
    }
}