using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using App.Domain;
using App.Domain.Interfaces;
using App.Helpers;
using App.IoC;
using System;
using System.Linq;

namespace App.Activities
{
    [Activity(Label = "@string/settings")]
    public class SettingsActivity : AppCompatActivity
    {
        private int themeId;
        private string fontPath;

        private RadioButton lightThemeRadioBtn;
        private RadioButton darkThemeRadioBtn;
        private SeekBar fontSizeSeekBar;
        private TextView fontSizeTextView;
        private Spinner fontFamiliySpinner;
        private TextView fontFamilyTextView;

        private Button applyBtn;

        private DependencyResolver dependencyResolver;
        private ISettingsService settingsService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            this.dependencyResolver = new DependencyResolver();
            this.settingsService = dependencyResolver.Resolve<ISettingsService>();
            SettingsModel settings = this.settingsService.Get();

            this.SetTheme(settings.Theme);

            base.OnCreate(savedInstanceState);

            this.SetContentView(Resource.Layout.Activity_Settings);

            // Themes
            this.lightThemeRadioBtn = this.FindViewById<RadioButton>(Resource.Id.lightThemeBtn);
            this.darkThemeRadioBtn = this.FindViewById<RadioButton>(Resource.Id.darkThemeBtn);

            this.lightThemeRadioBtn.Click += (sender, e) => 
            {
                this.themeId = Resource.Style.MainTheme;
            };

            this.darkThemeRadioBtn.Click += (sender, e) =>
            {
                this.themeId = Resource.Style.DarkMainTheme;
            };

            // Font size
            this.fontSizeSeekBar = this.FindViewById<SeekBar>(Resource.Id.fontSizeSeekBar);
            this.fontSizeSeekBar.ProgressChanged += (sender, e) => this.UpdateDisplayFontSize(e.Progress);
            this.fontSizeTextView = this.FindViewById<TextView>(Resource.Id.fontSizeTextView);

            // Font Family
            // Localization
            this.fontFamilyTextView = this.FindViewById<TextView>(Resource.Id.fontFamilyTextView);
            this.fontFamiliySpinner = this.FindViewById<Spinner>(Resource.Id.fontFamiliySpinner);
            string[] supportedFonts = new[]
                {
                    "OpenSans-Bold",
                    "OpenSans-BoldItalic",
                    "OpenSans-ExtraBold",
                    "OpenSans-ExtraBoldItalic",
                    "OpenSans-Italic",
                    "OpenSans-Light",
                    "OpenSans-LightItalic",
                    "OpenSans-Regular",
                    "OpenSans-Semibold",
                    "OpenSans-SemiboldItalic"
                };
            var spinnerAdapter = new ArrayAdapter<string>(
                this,
                Android.Resource.Layout.SimpleSpinnerItem,
                supportedFonts);
            spinnerAdapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            this.fontFamiliySpinner.Adapter = spinnerAdapter;
            this.fontFamiliySpinner.SetSelection(Array.IndexOf(supportedFonts, settings.FontPath.Split('/')[1].Split('.')[0]));
            this.fontFamiliySpinner.ItemSelected += (sender, args) => this.UpdateDisplayFont(supportedFonts[args.Position]);

            // Init state
            this.themeId = settings.Theme;
            this.fontPath = settings.FontPath;
            this.FillControlsState(settings);

            // Apply
            this.applyBtn = this.FindViewById<Button>(Resource.Id.applyBtn);
            this.applyBtn.Click += (sender, e) => 
            {
                var newSettigs = new SettingsModel(this.themeId, this.fontSizeSeekBar.Progress, this.fontPath);

                this.settingsService.Save(newSettigs);

                this.RestartApp();
            };

            // Apply Appearance
            AppearanceHelper.ApplySettings(this, settings);
        }

        protected override void OnDestroy()
        {
            this.dependencyResolver.Dispose();

            base.OnDestroy();
        }

        private void FillControlsState(SettingsModel settings)
        {
            switch (settings.Theme)
            {
                case Resource.Style.MainTheme:
                    this.lightThemeRadioBtn.Checked = true;
                    break;
                case Resource.Style.DarkMainTheme:
                    this.darkThemeRadioBtn.Checked = true;
                    break;
            }

            this.fontSizeSeekBar.Progress = settings.FontSize;
            this.UpdateDisplayFontSize(settings.FontSize);
        }

        private void UpdateDisplayFontSize(int fontSize)
        {
            this.fontSizeTextView.Text = this.GetString(Resource.String.font_size) + " " + fontSize;
            this.fontSizeTextView.TextSize = fontSize;
        }

        private void UpdateDisplayFont(string fontName)
        {
            this.fontPath = $"Fonts/{fontName}.ttf";
            var font = Typeface.CreateFromAsset(Assets, this.fontPath);
            this.fontFamilyTextView.Text = this.GetString(Resource.String.font_family) + " " + fontName;
            this.fontFamilyTextView.Typeface = font;
        }

        private void RestartApp()
        {
            this.Finish();
            Intent intent = new Intent(this, typeof(MainActivity));
            this.StartActivity(intent);
        }
    }
}