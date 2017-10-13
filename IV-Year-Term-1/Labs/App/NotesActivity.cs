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

			this.SetContentView(Resource.Layout.Notes);

			// Get our RecyclerView layout:
			var recyclerView = this.FindViewById<RecyclerView>(Resource.Id.notesListRecyclerView);

			// Instantiate the adapter and pass in its data source:
			var adapter = new NoteAdapter(this.noteRepository.GetAll().ToArray());
			adapter.ItemClick += this.OnNoteClick;
			adapter.ItemLongClick += this.OnNoteLongClick;

			// Plug the adapter into the RecyclerView:
			recyclerView.SetAdapter(adapter);

			// Instantiate the layout manager:
			var layoutManager = new LinearLayoutManager(this);
			recyclerView.SetLayoutManager(layoutManager);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			this.MenuInflater.Inflate(Resource.Menu.action_bar, menu);

			return true;
		}

		private void OnNoteClick(object sender, NoteAdapterClickEventArgs eventArgs)
		{
			// Create a new fragment and a transaction.
			FragmentTransaction fragmentTx = this.FragmentManager.BeginTransaction();
			var noteDetailsFragment = NoteDetailsFragment.FromNote(eventArgs.Note);

			// Replace the fragment that is in the View fragment_container (if applicable).
			fragmentTx.Replace(Resource.Layout.);

			// Add the transaction to the back stack.
			fragmentTx.AddToBackStack(null);

			// Commit the transaction.
			fragmentTx.Commit();
		}

		private void OnNoteLongClick(object sender, NoteAdapterClickEventArgs noteAdapterClickEventArgs)
		{

		}
	}
}