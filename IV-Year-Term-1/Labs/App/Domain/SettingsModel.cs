namespace App.Domain
{
    public class SettingsModel
    {
        public SettingsModel(int theme, int fontSize, string fontPath)
        {
            this.Theme = theme;
            this.FontSize = fontSize;
            this.FontPath = fontPath;
        }

        public int Theme { get; }

        public int FontSize { get; }

        public string FontPath { get; }
    }
}