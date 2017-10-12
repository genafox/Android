using Android.App;
using Android.OS;
using Android.Views;
using App.Domain.Models;
using Newtonsoft.Json;

namespace App.Fragments
{
    public class NoteDetailsFragment : Fragment
    {
        private const string BundleNoteKey = "note_item";

        public static NoteDetailsFragment FromNote(Note note)
        {
            var fragment = new NoteDetailsFragment();
            var args = new Bundle();

            string jsonNote = JsonConvert.SerializeObject(note);
            args.PutString(BundleNoteKey, jsonNote);

            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
             return inflater.Inflate(Resource.Layout.NoteDetails, container, false);
        }

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutInt("current_choice", _currentCheckPosition);
        }
    }
}