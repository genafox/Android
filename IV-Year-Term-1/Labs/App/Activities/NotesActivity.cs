using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using App.DataBinding;
using App.Domain.Interfaces;
using App.Domain.Models;
using App.Domain.Repositories;
using App.Fragments;
using SupportFragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace App.Activities
{
    [Activity(Label = "@string/notes_label", MainLauncher = false)]
    public class NotesActivity : AppCompatActivity
    {
        private INoteRepository noteRepository;

        private FrameLayout fragmentContainer;
        private SupportToolbar toolbar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.noteRepository = new InMemoryNoteRepository();

            this.SetContentView(Resource.Layout.Activity_Notes);

            // Initialize Fragment Container
            this.fragmentContainer = this.FindViewById<FrameLayout>(Resource.Id.fragmentContainer);

            // Initialize Toolbar
            this.toolbar = this.FindViewById<SupportToolbar>(Resource.Id.notes_toolbar);
            SetSupportActionBar(toolbar);
            toolbar.MenuItemClick += ToolbarOnMenuItemClick;

            //Initialize RecyclerView
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

        private void ToolbarOnMenuItemClick(object sender, SupportToolbar.MenuItemClickEventArgs menuItemClickEventArgs)
        {
            if (menuItemClickEventArgs.Item.ItemId == Resource.Id.search_action)
            {
            }

            if (menuItemClickEventArgs.Item.ItemId == Resource.Id.add_note_action)
            {
                this.StartAddNoteActivity();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            this.MenuInflater.Inflate(Resource.Menu.action_bar, menu);

            return base.OnCreateOptionsMenu(menu);
        }

        public override void OnBackPressed()
        {
            if (this.SupportFragmentManager.BackStackEntryCount > 0)
            {
                this.fragmentContainer.Visibility = ViewStates.Gone;
            }

            base.OnBackPressed();
        }

        private void OnNoteClick(object sender, NoteAdapterClickEventArgs eventArgs)
        {
            this.ShowNoteDetailsFragment(eventArgs.Note);
        }

        private void OnNoteLongClick(object sender, NoteAdapterClickEventArgs noteAdapterClickEventArgs)
        {

        }

        private void ShowNoteDetailsFragment(Note note)
        {
            var noteDetailsFragment = NoteDetailsFragment.FromNote(note);
            this.ShowFragment(noteDetailsFragment);
        }

        private void StartAddNoteActivity()
        {
            var activity = new Intent(this, typeof(AddNoteActivity));
            StartActivity(activity);
        }

        private void ShowFragment(SupportFragment fragment)
        {
            // Create a new fragment and a transaction.
            SupportFragmentTransaction fragmentTx = this.SupportFragmentManager.BeginTransaction();

            // Replace the fragment that is in the View fragment_container (if applicable).
            fragmentTx.Replace(Resource.Id.fragmentContainer, fragment);

            // Add the transaction to the back stack.
            fragmentTx.AddToBackStack(null);

            // Commit the transaction.
            fragmentTx.Commit();

            this.fragmentContainer.Visibility = ViewStates.Visible;
        }
    }
}