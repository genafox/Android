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
using AlertDialog = Android.Support.V7.App.AlertDialog;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using SupportFragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace App.Activities
{
    [Activity(Label = "@string/notes_label", MainLauncher = false)]
    public class NotesActivity : AppCompatActivity
    {
        private const int CreateNoteRequestCode = 4;
        private const int UpdateNoteRequestCode = 5;

        private INoteRepository noteRepository;

        private NoteAdapter noteAdapter;

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
            this.noteAdapter = new NoteAdapter(this.noteRepository.GetAll().ToArray());
            this.noteAdapter.ItemClick += this.OnNoteClick;
            this.noteAdapter.ItemLongClick += this.OnNoteLongClick;

            // Plug the adapter into the RecyclerView:
            recyclerView.SetAdapter(this.noteAdapter);

            // Instantiate the layout manager:
            var layoutManager = new LinearLayoutManager(this);
            recyclerView.SetLayoutManager(layoutManager);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                switch (requestCode)
                {
                    case CreateNoteRequestCode:
                    case UpdateNoteRequestCode:
                        this.NotifyNotesDataChanged();
                        break;
                }
            }
        }

        private void NotifyNotesDataChanged()
        {
            this.noteAdapter.SetData(this.noteRepository.GetAll().ToArray());
            this.noteAdapter.NotifyDataSetChanged();
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
            this.MenuInflater.Inflate(Resource.Menu.action_bar_menu, menu);

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
            var contextMenu = new PopupMenu(this, noteAdapterClickEventArgs.ItemView);
            contextMenu.Inflate(Resource.Menu.note_item_context_menu);
            contextMenu.MenuItemClick += (o, args) =>
            {
                switch (args.Item.ItemId)
                {
                    case Resource.Id.edit_note_action:
                        this.StartEditNoteActivity(noteAdapterClickEventArgs.Note);
                        break;
                    case Resource.Id.delete_note_action:
                        this.StartDeleteNoteDialog(noteAdapterClickEventArgs.Note);
                        break;
                }
            };
            contextMenu.Show();
        }

        private void ShowNoteDetailsFragment(Note note)
        {
            var noteDetailsFragment = NoteDetailsFragment.FromNote(note);
            this.ShowFragment(noteDetailsFragment);
        }

        private void StartAddNoteActivity()
        {
            var activity = new Intent(this, typeof(NoteConfigurationActivity));
            StartActivityForResult(activity, CreateNoteRequestCode);
        }

        private void StartEditNoteActivity(Note note)
        {
            Intent activity = NoteConfigurationActivity.FromNote(note, this);
            StartActivityForResult(activity, UpdateNoteRequestCode);
        }

        private void StartDeleteNoteDialog(Note note)
        {
            var alertDialog = new AlertDialog.Builder(this);
            alertDialog.SetTitle(this.GetString(Resource.String.confirm_delete_note_title));
            alertDialog.SetMessage(string.Format(
                this.GetString(Resource.String.confirm_delete_note_message),
                note.Name));
            alertDialog.SetPositiveButton(this.GetString(Resource.String.yes), (sender, args) =>
            {
                this.noteRepository.Delete(note.Name);
                this.NotifyNotesDataChanged();
                alertDialog.Dispose();
            });
            alertDialog.SetNegativeButton(this.GetString(Resource.String.no), (sender, args) =>
            {
                alertDialog.Dispose();
            });

            alertDialog.Show();
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