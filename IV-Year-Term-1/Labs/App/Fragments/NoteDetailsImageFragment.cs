using Android.OS;
using Android.Support.V4.App;
using Android.Views;

namespace App.Fragments
{
    public class NoteDetailsImageFragment : Fragment
    {
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
            //this.RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View fragmentView = inflater.Inflate(Resource.Layout.Fragment_NoteDetails_WithImage, container, false);

            return fragmentView;
        }
    }
}