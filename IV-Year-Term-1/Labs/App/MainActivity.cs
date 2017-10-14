using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using App.Activities;

namespace App
{
    [Activity(Label = "@string/app_name", Icon = "@drawable/labs_launcher_cast_shedow_icon", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button colorPickerBtn;
        private Button calculatorBtn;
        private Button notesBtn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Main);

            this.colorPickerBtn = FindViewById<Button>(Resource.Id.colorPickerBtn);
            this.calculatorBtn = FindViewById<Button>(Resource.Id.calculatorBtn);
            this.notesBtn = FindViewById<Button>(Resource.Id.notesBtn);

            colorPickerBtn.Click += (sender, e) => {
                var activity = new Intent(this, typeof(ColorPickerActivity));
                StartActivity(activity);
            };

            calculatorBtn.Click += (sender, e) => {
                var activity = new Intent(this, typeof(CalculatorActivity));
                StartActivity(activity);
            };

            notesBtn.Click += (sender, e) => {
                var activity = new Intent(this, typeof(NotesActivity));
                StartActivity(activity);
            };
        }
    }
}

