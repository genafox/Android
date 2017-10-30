using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using App.DataBinding;
using App.Domain.Interfaces;
using App.Domain.Database.Models;
using App.Fragments;
using AlertDialog = Android.Support.V7.App.AlertDialog;
using PopupMenu = Android.Support.V7.Widget.PopupMenu;
using SearchView = Android.Support.V7.Widget.SearchView;
using SupportFragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using SupportFragment = Android.Support.V4.App.Fragment;
using App.IoC;

namespace App.Activities
{
    [Activity(Label = "@string/notes_label", MainLauncher = false)]
    public class NotesActivity : AppCompatActivity
    {
        private const int CreateNoteRequestCode = 4;
        private const int UpdateNoteRequestCode = 5;

        private KeyValuePair<NoteImportance, string>[] noteImportanceSource;

        // Services & Data Access
        private DependencyResolver dependencyResolver;
        private INoteRepository noteRepository;

        // Recycler View
        private NoteAdapter noteAdapter;

        // Fragments
        private FrameLayout fragmentContainer;

        // Menu
        private SupportToolbar toolbar;
        private SearchView searchView;
        private Spinner importanceFilterSpinner;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.dependencyResolver = new DependencyResolver();
            this.noteRepository = this.dependencyResolver.Resolve<INoteRepository>();

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
            var notes = new ObservableCollection<Note>(this.noteRepository.GetAll());
            this.noteAdapter = new NoteAdapter(notes);
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

        protected override void OnDestroy()
        {
            this.dependencyResolver.Dispose();

            base.OnDestroy();
        }

        private void NotifyNotesDataChanged(Note[] notes = null)
        {
            notes = notes ?? this.noteRepository.GetAll().ToArray();
            this.noteAdapter.SetData(new ObservableCollection<Note>(notes));
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

            this.SetupMenuSearchAction(menu);

            this.SetupMenuImportanceFilterAction(menu);

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

        private void SetupMenuSearchAction(IMenu menu)
        {
            // Search action
            IMenuItem searchItem = menu.FindItem(Resource.Id.search_action);
            View view = MenuItemCompat.GetActionView(searchItem);

            this.searchView = view as SearchView;
            if (this.searchView != null)
            {
                this.searchView.QueryTextChange += (sender, args) => FilterNotes();

                this.searchView.QueryTextSubmit += (sender, args) =>
                {
                    // Hide keyboard
                    InputMethodManager inputManager = (InputMethodManager)this.GetSystemService(Context.InputMethodService);
                    if (this.CurrentFocus != null)
                    {
                        inputManager.HideSoftInputFromWindow(this.CurrentFocus.WindowToken, HideSoftInputFlags.None);
                    }

                    // Show count of found records
                    Toast
                        .MakeText(
                            this,
                            string.Format(this.GetString(Resource.String.found_notes_count), this.noteAdapter.ItemCount),
                            ToastLength.Short)
                        .Show();

                    args.Handled = true;
                };
            }
        }

        private void SetupMenuImportanceFilterAction(IMenu menu)
        {
            IMenuItem importanceFilterMenuItem = menu.FindItem(Resource.Id.importance_filter);
            View view = MenuItemCompat.GetActionView(importanceFilterMenuItem);

            this.importanceFilterSpinner = view as Spinner;
            if (this.importanceFilterSpinner != null)
            {
                this.noteImportanceSource = new[]
                {
                    new KeyValuePair<NoteImportance, string>(NoteImportance.All,
                        GetString(Resource.String.note_importance_all)),
                    new KeyValuePair<NoteImportance, string>(NoteImportance.Low,
                        GetString(Resource.String.note_importance_low)),
                    new KeyValuePair<NoteImportance, string>(NoteImportance.Medium,
                        GetString(Resource.String.note_importance_medium)),
                    new KeyValuePair<NoteImportance, string>(NoteImportance.High,
                        GetString(Resource.String.note_importance_high))
                };
                var spinnerAdapter = new ArrayAdapter<string>(
                    this,
                    Android.Resource.Layout.SimpleSpinnerItem,
                    this.noteImportanceSource.Select(kvp => kvp.Value).ToArray());
                spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                this.importanceFilterSpinner.Adapter = spinnerAdapter;
                this.importanceFilterSpinner.ItemSelected += (sender, args) => FilterNotes();
            }
        }

        private void FilterNotes()
        {
            string nameFilter = this.searchView.Query;
            NoteImportance importanceFilter = this.noteImportanceSource[this.importanceFilterSpinner.SelectedItemPosition].Key;

            IEnumerable<Note> filtered = this.noteRepository
                .GetAll()
                .Where(n => 
                    n.Name.IndexOf(nameFilter, StringComparison.InvariantCultureIgnoreCase) >= 0
                    && importanceFilter.HasFlag(n.Importance));
            this.noteAdapter.SetData(new ObservableCollection<Note>(filtered));
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