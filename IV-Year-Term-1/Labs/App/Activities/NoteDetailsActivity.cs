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
using SupportFragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using SupportFragment = Android.Support.V4.App.Fragment;
using Android.Support.V7.App;
using App.Fragments;
using App.Domain.Database.Models;
using Newtonsoft.Json;

namespace App.Activities
{
    [Activity(Label = "@string/notes_label", MainLauncher = false)]
    public class NoteDetailsActivity : AppCompatActivity
    {
        private const string BundleNoteDataKey = "BundleNoteKey";

        // Fragments
        private FrameLayout fragmentContainer;

        public static Intent FromNote(Note note, Context context)
        {
            var activity = new Intent(context, typeof(NoteDetailsActivity));

            string jsonNote = JsonConvert.SerializeObject(note);
            activity.PutExtra(BundleNoteDataKey, jsonNote);

            return activity;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.Activity_NoteDetails);

            // Initialize Fragment Container
            this.fragmentContainer = this.FindViewById<FrameLayout>(Resource.Id.detailsFragmentContainer);

            string jsonNoteData = this.Intent.GetStringExtra(BundleNoteDataKey);
            var noteData = JsonConvert.DeserializeObject<Note>(jsonNoteData);

            var noteDetailsFragment = NoteDetailsFragment.FromNote(noteData);
            this.ShowFragment(noteDetailsFragment);
        }

        private void ShowFragment(SupportFragment fragment)
        {
            // Create a new fragment and a transaction.
            SupportFragmentTransaction fragmentTx = this.SupportFragmentManager.BeginTransaction();

            // Replace the fragment that is in the View fragment_container (if applicable).
            fragmentTx.Replace(Resource.Id.detailsFragmentContainer, fragment);

            // Add the transaction to the back stack.
            fragmentTx.AddToBackStack(null);

            // Commit the transaction.
            fragmentTx.Commit();

            this.fragmentContainer.Visibility = ViewStates.Visible;
        }
    }
}