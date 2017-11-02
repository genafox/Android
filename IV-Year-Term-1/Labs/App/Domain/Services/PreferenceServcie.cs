using Android.App;
using Android.Content;
using App.Domain.Interfaces;

namespace App.Domain.Services
{
    public class PreferenceServcie : ISettingsService
    {
        private const string PreferencesFileName = "LabsAppPreferences";

        private const string ThemeKey = "Theme";
        private const string FontSizeKey = "FontSize";
        private const string FontPathKey = "FontPath";

        public SettingsModel Get()
        {
            SettingsModel settings;
            using (ISharedPreferences settingsStorage = Application.Context.GetSharedPreferences(PreferencesFileName, FileCreationMode.Private))
            {
                int theme = settingsStorage.GetInt(ThemeKey, Resource.Style.MainTheme);
                int fontSize = settingsStorage.GetInt(FontSizeKey, 14);
                string fontPath = settingsStorage.GetString(FontPathKey, "Fonts/OpenSans-Regular.ttf");

                settings = new SettingsModel(theme, fontSize, fontPath);
            }

            return settings;
        }

        public void Save(SettingsModel model)
        {
            using (ISharedPreferences settingsStorage = Application.Context.GetSharedPreferences(PreferencesFileName, FileCreationMode.Private))
            {
                using (ISharedPreferencesEditor editTransaction = settingsStorage.Edit())
                {
                    editTransaction.PutInt(ThemeKey, model.Theme);
                    editTransaction.PutInt(FontSizeKey, model.FontSize);
                    editTransaction.PutString(FontPathKey, model.FontPath);

                    editTransaction.Commit();
                }
            }
        }
    }
}