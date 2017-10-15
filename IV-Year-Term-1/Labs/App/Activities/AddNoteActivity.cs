using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using App.Domain.Exceptions;
using App.Domain.Interfaces;
using App.Domain.Models;
using App.Domain.Repositories;
using Uri = Android.Net.Uri;

namespace App.Activities
{
    [Activity(Label = "@string/add_note", MainLauncher = false)]
    public class AddNoteActivity : AppCompatActivity
    {
        private const int SelectNoteIconRequestCode = 1;

        private const int MaxNoteNameLength = 50;

        private KeyValuePair<NoteImportance, string>[] NoteImportanceSource;

        private ImageView noteIconInput;
        private Spinner noteImportanceInput;

        private EditText noteNameInput;
        private TextInputLayout noteNameInputLayout;

        private EditText noteExpirationDateInput;
        private TextInputLayout noteExpirationDateInputLayout;

        private EditText noteDescriptionInput;
        private TextInputLayout noteDescriptionInputLayout;

        private Button saveNoteBtn;

        private Note noteData;
        private INoteRepository noteRepository;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.noteData = new Note();
            this.noteRepository = new InMemoryNoteRepository();

            this.SetContentView(Resource.Layout.Activity_AddNote);

            // Initialize Note Icon Input
            this.noteIconInput = this.FindViewById<ImageView>(Resource.Id.noteIconInput);
            this.noteIconInput.Click += OnNoteIconInputClick;

            // Initialize Note Name Input
            this.noteNameInput = this.FindViewById<EditText>(Resource.Id.noteNameInput);
            this.noteNameInputLayout = this.FindViewById<TextInputLayout>(Resource.Id.noteNameInputLayout);
            this.noteNameInput.TextChanged += OnNoteNameInputTextChanged;

            // Initialize Note Expiration Date Input
            this.noteExpirationDateInput = this.FindViewById<EditText>(Resource.Id.noteExpirationDateInput);
            this.noteExpirationDateInputLayout = this.FindViewById<TextInputLayout>(Resource.Id.noteExpirationDateInputLayout);
            this.noteExpirationDateInput.FocusChange += OnNoteExpirationDateInputFocusChange;

            // Initialize Note Description Input
            this.noteDescriptionInput = this.FindViewById<EditText>(Resource.Id.noteDescriptionInput);
            this.noteDescriptionInputLayout = this.FindViewById<TextInputLayout>(Resource.Id.noteDescriptionInputLayout);
            this.noteDescriptionInput.TextChanged += OnNoteDescriptionInputTextChanged;

            // Initialize Note Name Input
            this.noteImportanceInput = this.FindViewById<Spinner>(Resource.Id.noteImportanceInput);
            this.NoteImportanceSource = new[]
            {
                new KeyValuePair<NoteImportance, string>(NoteImportance.Low,
                    GetString(Resource.String.note_importance_low)),
                new KeyValuePair<NoteImportance, string>(NoteImportance.Low,
                    GetString(Resource.String.note_importance_medium)),
                new KeyValuePair<NoteImportance, string>(NoteImportance.Low,
                    GetString(Resource.String.note_importance_high))
            };
            var spinnerAdapter = new ArrayAdapter<string>(
                this, 
                Android.Resource.Layout.SimpleSpinnerItem,
                this.NoteImportanceSource.Select(kvp => kvp.Value).ToArray());
            spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            this.noteImportanceInput.Adapter = spinnerAdapter;
            this.noteImportanceInput.ItemSelected += OnNoteImportanceInputItemSelected;

            // Initialize Save Button
            this.saveNoteBtn = this.FindViewById<Button>(Resource.Id.saveNoteBtn);
            this.saveNoteBtn.Click += OnSaveNoteBtnClick;
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (resultCode == Result.Ok)
            {
                if (requestCode == SelectNoteIconRequestCode)
                {
                    this.HandleChooseIconResult(data.Data);
                }
            }
        }

        private void OnNoteIconInputClick(object sender, EventArgs eventArgs)
        {
            var intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);
            this.StartActivityForResult(
                Intent.CreateChooser(intent, this.GetString(Resource.String.choose_note_icon_label)),
                SelectNoteIconRequestCode);
        }

        private void HandleChooseIconResult(Uri iconUri)
        {
            this.noteData.IconPath = iconUri.ToString();

            //Stream iconStream = this.ContentResolver.OpenInputStream(iconUri);
            //this.noteIconInput.SetImageBitmap(BitmapFactory.DecodeStream(iconStream));
            this.noteIconInput.SetImageURI(iconUri);
        }

        private void OnNoteNameInputTextChanged(object sender, TextChangedEventArgs eventArgs)
        {
            if (eventArgs.AfterCount > MaxNoteNameLength)
            {
                this.SetInputError(this.noteNameInputLayout, Resource.String.max_note_name_lenght_error_message, MaxNoteNameLength);

                return;
            }

            this.noteNameInputLayout.SetErrorEnabled(false);
            this.noteData.Name = eventArgs.Text.ToString();
        }

        private void OnNoteExpirationDateInputFocusChange(object sender, View.FocusChangeEventArgs focusChangeEventArgs)
        {
            if (!focusChangeEventArgs.HasFocus)
            {
                if (DateTime.TryParse(this.noteExpirationDateInput.Text, out DateTime expirationDate))
                {
                    this.noteData.ExpirationDate = expirationDate;
                    this.noteExpirationDateInputLayout.SetErrorEnabled(false);
                }
                else
                {
                    this.SetInputError(this.noteExpirationDateInputLayout, Resource.String.wrong_date_format_error_message);
                }
            }
        }

        private void OnNoteDescriptionInputTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            this.noteData.Description = textChangedEventArgs.Text.ToString();
        }

        private void OnNoteImportanceInputItemSelected(object sender, AdapterView.ItemSelectedEventArgs itemSelectedEventArgs)
        {
            this.noteData.Importance = this.NoteImportanceSource[itemSelectedEventArgs.Position].Key;
        }

        private void OnSaveNoteBtnClick(object sender, EventArgs eventArgs)
        {
            try
            {
                this.noteData.CreationDate = DateTime.Now;
                this.noteRepository.Create(this.noteData);
                this.SetResult(Result.Ok);
                this.Finish();
            }
            catch (EntryAlreadyExistsException ex)
            {
                this.SetInputError(this.noteNameInputLayout, Resource.String.unique_note_name_error_message);
            }
        }

        private void SetInputError(TextInputLayout inputLayout, int errorMessageId, params object[] messageParams)
        {
            inputLayout.SetErrorEnabled(true);
            inputLayout.SetError(string.Format(this.GetString(errorMessageId), messageParams));
        }
    }
}