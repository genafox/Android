﻿using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using App.Domain.Models;
using App.Helpers;
using Newtonsoft.Json;

namespace App.Fragments
{
	public class NoteDetailsFragment : Fragment
	{
		private const string BundleNoteKey = "note_item";

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
			base.OnCreate(savedInstanceState);

			// Create your fragment here
			this.RetainInstance = true;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			View fragmentView = inflater.Inflate(Resource.Layout.NoteDetails, container, false);

			string jsonNote = savedInstanceState.GetString(BundleNoteKey);
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
			}

			return fragmentView;
		}

		public override void OnSaveInstanceState(Bundle outState)
		{
			base.OnSaveInstanceState(outState);

			string jsonNote = JsonConvert.SerializeObject(this.noteItem);
			outState.PutString("current_choice", jsonNote);
		}
	}
}