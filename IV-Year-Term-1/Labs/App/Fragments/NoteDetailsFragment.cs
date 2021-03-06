﻿using Android.Net;
using Android.OS;
using Android.Views;
using Android.Widget;
using App.Domain.Database.Models;
using App.Helpers;
using Newtonsoft.Json;
using App.Domain.Interfaces;
using App.IoC;
using SupportFragment = Android.Support.V4.App.Fragment;

namespace App.Fragments
{
    public class NoteDetailsFragment : SupportFragment
    {
        private const string BundleNoteKey = "note_item";

        private DependencyResolver dependencyResolver;

        private Note noteItem;

        public static NoteDetailsFragment FromNote(Note note)
        {
            var fragment = new NoteDetailsFragment
            {
                Arguments = new Bundle()
            };

            string jsonNote = JsonConvert.SerializeObject(note);
            fragment.Arguments.PutString(BundleNoteKey, jsonNote);

            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            this.dependencyResolver = new DependencyResolver();

            base.OnCreate(savedInstanceState);

            // Create your fragment here
            //this.RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View fragmentView = inflater.Inflate(Resource.Layout.Fragment_NoteDetails, container, false);

            string jsonNote = string.Empty;
            if (savedInstanceState != null)
            {
                jsonNote = savedInstanceState.GetString(BundleNoteKey);
            }
            else if (Arguments != null)
            {
                jsonNote = this.Arguments.GetString(BundleNoteKey);
            }

            if (!string.IsNullOrEmpty(jsonNote))
            {
                this.noteItem = JsonConvert.DeserializeObject<Note>(jsonNote);

                fragmentView.FindViewById<TextView>(Resource.Id.detailsNameTextView)
                    .Text = this.noteItem.Name;
                fragmentView.FindViewById<TextView>(Resource.Id.detailsCreationDateTextView)
                    .Text = this.noteItem.CreationDate.ToLongDateString();
                fragmentView.FindViewById<TextView>(Resource.Id.detailsExpirationDateTextView)
                    .Text = this.noteItem.ExpirationDate.ToLongDateString();
                fragmentView.FindViewById<TextView>(Resource.Id.detailsDescriptionTextView)
                    .Text = this.noteItem.Description;
                fragmentView.FindViewById<ImageView>(Resource.Id.detailsImportanceImageView)
                    .SetImageResource(this.noteItem.Importance.GetIconResource());

                if (!string.IsNullOrEmpty(this.noteItem.IconPath))
                {
                    Uri iconUri = Uri.Parse(this.noteItem.IconPath);
                    //Stream iconStream = this.Activity.ContentResolver.OpenInputStream(iconUri);

                    fragmentView.FindViewById<ImageView>(Resource.Id.detailsIconImageView)
                        .SetImageURI(iconUri);
                }
            }

            var settings = this.dependencyResolver.Resolve<ISettingsService>().Get();
            AppearanceHelper.ApplySettings(fragmentView, this.Activity.ApplicationContext, settings);

            return fragmentView;
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            string jsonNote = JsonConvert.SerializeObject(this.noteItem);
            outState.PutString(BundleNoteKey, jsonNote);
        }

        public override void OnDestroy()
        {
            this.dependencyResolver.Dispose();
            base.OnDestroy();
        }
    }
}