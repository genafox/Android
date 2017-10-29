using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using App.Domain.Exceptions;
using App.Domain.Interfaces;
using App.Domain.Database.Models;
using App.Domain.Repositories;
using Newtonsoft.Json;
using Uri = Android.Net.Uri;
using App.IoC;

namespace App.Activities
{
    [Activity(Label = "@string/add_note", MainLauncher = false)]
    public class NoteConfigurationActivity : AppCompatActivity
    {
        private const string BundleNoteDataKey = "note_item_data";
        private const string ExpirationDateFormat = "dd/MM/yy hh:mm:ss";
        private const int SelectNoteIconRequestCode = 1;

        private const int MaxNoteNameLength = 50;

        private KeyValuePair<NoteImportance, string>[] noteImportanceSource;

        private ImageView noteIconInput;
        private Spinner noteImportanceInput;

        private EditText noteNameInput;
        private TextInputLayout noteNameInputLayout;

        private EditText noteExpirationDateInput;
        private TextInputLayout noteExpirationDateInputLayout;

        private EditText noteDescriptionInput;
        private TextInputLayout noteDescriptionInputLayout;

        private Button saveNoteBtn;

        private bool isEditState;
        private Note noteData;

        private DependencyResolver dependencyResolver;
        private INoteRepository noteRepository;

        public static Intent FromNote(Note note, Context context)
        {
            var activity = new Intent(context, typeof(NoteConfigurationActivity));

            string jsonNote = JsonConvert.SerializeObject(note);
            activity.PutExtra(BundleNoteDataKey, jsonNote);

            return activity;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.noteData = new Note();
            this.dependencyResolver = AppContainer.GetDependencyResolver();
            this.noteRepository = this.dependencyResolver.Resolve<INoteRepository>();

            this.SetContentView(Resource.Layout.Activity_NoteConfiguration);

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
            this.noteImportanceSource = new[]
            {
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
            this.noteImportanceInput.Adapter = spinnerAdapter;
            this.noteImportanceInput.ItemSelected += OnNoteImportanceInputItemSelected;

            // Initialize Save Button
            this.saveNoteBtn = this.FindViewById<Button>(Resource.Id.saveNoteBtn);
            this.saveNoteBtn.Click += OnSaveNoteBtnClick;

            // If Edit - initialize Note Data
            string jsonNoteData = this.Intent.GetStringExtra(BundleNoteDataKey);
            this.isEditState = !string.IsNullOrEmpty(jsonNoteData);
            if (this.isEditState)
            {
                this.noteData = JsonConvert.DeserializeObject<Note>(jsonNoteData);

                if (!string.IsNullOrEmpty(this.noteData.IconPath))
                {
                    Uri iconUri = Uri.Parse(this.noteData.IconPath);
                    this.noteIconInput.SetImageURI(iconUri);
                }

                this.noteImportanceInput.SetSelection(Array.FindIndex(this.noteImportanceSource, kvp => kvp.Key == this.noteData.Importance));
                this.noteNameInput.Text = this.noteData.Name;
                this.noteExpirationDateInput.Text = this.noteData.ExpirationDate.ToString(ExpirationDateFormat);
                this.noteDescriptionInput.Text = this.noteData.Description;
            }
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

        protected override void OnDestroy()
        {
            this.dependencyResolver.Dispose();
            base.OnDestroy();
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
                if (DateTime.TryParseExact(
                    this.noteExpirationDateInput.Text, 
                    ExpirationDateFormat, 
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime expirationDate))
                {
                    this.noteData.ExpirationDate = expirationDate;
                    this.noteExpirationDateInputLayout.SetErrorEnabled(false);
                }
                else
                {
                    this.SetInputError(this.noteExpirationDateInputLayout, Resource.String.wrong_date_format_error_message, ExpirationDateFormat);
                }
            }
        }

        private void OnNoteDescriptionInputTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            this.noteData.Description = textChangedEventArgs.Text.ToString();
        }

        private void OnNoteImportanceInputItemSelected(object sender, AdapterView.ItemSelectedEventArgs itemSelectedEventArgs)
        {
            this.noteData.Importance = this.noteImportanceSource[itemSelectedEventArgs.Position].Key;
        }

        private void OnSaveNoteBtnClick(object sender, EventArgs eventArgs)
        {
            Action<Note> saveNoteAction = this.noteRepository.Create;
            if (this.isEditState)
            {
                saveNoteAction = this.noteRepository.Update;
            }

            try
            {
                saveNoteAction(this.noteData);
                this.SetResult(Result.Ok);
                this.Finish();
            }
            catch (EntryAlreadyExistsException ex)
            {
                this.SetInputError(this.noteNameInputLayout, Resource.String.unique_note_name_error_message, this.noteData.Name);
            }
            catch (EntryNotFoundException ex)
            {
                this.SetInputError(this.noteNameInputLayout, Resource.String.note_not_found_error_message, this.noteData.Name);
            }
        }

        private void SetInputError(TextInputLayout inputLayout, int errorMessageId, params object[] messageParams)
        {
            inputLayout.SetErrorEnabled(true);
            inputLayout.SetError(string.Format(this.GetString(errorMessageId), messageParams));
        }
    }
}