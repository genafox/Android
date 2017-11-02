namespace App.Domain
{
    public class SettingsModel
    {
        public SettingsModel(int theme, int fontSize)
        {
            this.Theme = theme;
            this.FontSize = fontSize;
        }

        public int Theme { get; }

        public int FontSize { get; }
    }
}