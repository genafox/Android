using Android.OS;

namespace App.Services.Binders
{
    public class AudioServiceBinder : Binder
    {
        AudioService service;

        public AudioServiceBinder(AudioService service)
        {
            this.service = service;
        }

        public AudioService GetAudioService()
        {
            return service;
        }
    }
}