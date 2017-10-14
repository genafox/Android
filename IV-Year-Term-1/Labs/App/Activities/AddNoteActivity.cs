using Android.App;
using Android.OS;
using Android.Support.V7.App;

namespace App.Activities
{
    [Activity(Label = "@string/add_note")]
    public class AddNoteActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Activity_AddNote);
        }
    }
}