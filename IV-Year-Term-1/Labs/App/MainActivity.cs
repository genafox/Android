using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using App.Activities;
using Java.Util;

namespace App
{
    [Activity(
        Label = "@string/app_name", 
        Icon = "@drawable/labs_launcher_cast_shedow_icon", 
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : AppCompatActivity
    {
        private KeyValuePair<string, string>[] supportedLanguages;

        private Button colorPickerBtn;
        private Button calculatorBtn;
        private Button notesBtn;
        
        private Spinner languageSpinner;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            // Color Picker
            this.colorPickerBtn = FindViewById<Button>(Resource.Id.colorPickerBtn);
            colorPickerBtn.Click += (sender, e) => {
                var activity = new Intent(this, typeof(ColorPickerActivity));
                StartActivity(activity);
            };

            // Calculator
            this.calculatorBtn = FindViewById<Button>(Resource.Id.calculatorBtn);
            calculatorBtn.Click += (sender, e) => {
                var activity = new Intent(this, typeof(CalculatorActivity));
                StartActivity(activity);
            };

            // Notes
            this.notesBtn = FindViewById<Button>(Resource.Id.notesBtn);
            notesBtn.Click += (sender, e) => {
                var activity = new Intent(this, typeof(NotesActivity));
                StartActivity(activity);
            };

            // Localization
            this.languageSpinner = this.FindViewById<Spinner>(Resource.Id.languageSpinner);
            this.supportedLanguages = new[]
            {
                new KeyValuePair<string, string>("en", this.GetString(Resource.String.en_language)),
                new KeyValuePair<string, string>("ru", this.GetString(Resource.String.ru_language))
            };
            var spinnerAdapter = new ArrayAdapter<string>(
                this,
                Android.Resource.Layout.SimpleSpinnerItem,
                this.supportedLanguages.Select(kvp => kvp.Value).ToArray());
            spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            this.languageSpinner.Adapter = spinnerAdapter;
            this.languageSpinner.SetSelection(Array.FindIndex(this.supportedLanguages, kvp => this.IsCurrentLanguage(kvp.Key)));
            this.languageSpinner.ItemSelected += (sender, args) => this.ChangeCulture();
        }

        private void ChangeCulture()
        {
            string lang = this.supportedLanguages[this.languageSpinner.SelectedItemPosition].Key;
            if (!this.IsCurrentLanguage(lang))
            {
                var locale = new Locale(lang);
                Configuration config = this.BaseContext.Resources.Configuration;
                config.Locale = locale;
                this.BaseContext.Resources.UpdateConfiguration(
                    config,
                    this.BaseContext.Resources.DisplayMetrics);

                this.Finish();
                Intent intent = new Intent(this, this.Class);
                this.StartActivity(intent);
            }
        }

        private bool IsCurrentLanguage(string lang)
        {
            string currntLang = this.Resources.Configuration.Locale.Language;

            return string.Equals(lang, currntLang, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}

