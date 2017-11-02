namespace App.Domain.Interfaces
{
    public interface ISettingsService
    {
        SettingsModel Get();

        void Save(SettingsModel model);
    }
}