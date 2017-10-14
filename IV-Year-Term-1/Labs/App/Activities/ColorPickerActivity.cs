using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;

namespace App.Activities
{
    [Activity(Label = "@string/color_picker_label")]
    public class ColorPickerActivity : AppCompatActivity
    {
        private TextView colorTextVeiw;
        private SeekBar redSeekBar;
        private SeekBar greenSeekBar;
        private SeekBar blueSeekBar;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Activity_ColorPicker);

            this.colorTextVeiw = FindViewById<TextView>(Resource.Id.colorTextVeiw);
            this.redSeekBar = FindViewById<SeekBar>(Resource.Id.redSeekBar);
            this.greenSeekBar = FindViewById<SeekBar>(Resource.Id.greenSeekBar);
            this.blueSeekBar = FindViewById<SeekBar>(Resource.Id.blueSeekBar);

            this.redSeekBar.ProgressChanged += OnSeekBarProgressChanged;
            this.greenSeekBar.ProgressChanged += OnSeekBarProgressChanged;
            this.blueSeekBar.ProgressChanged += OnSeekBarProgressChanged;

            this.UpdateTextViewColor();
        }

        private void OnSeekBarProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            this.UpdateTextViewColor();
        }

        private void UpdateTextViewColor()
        {
            int red = this.redSeekBar.Progress;
            int green = this.greenSeekBar.Progress;
            int blue = this.blueSeekBar.Progress;

            var color = new Color(red, green, blue);
            this.colorTextVeiw.SetBackgroundColor(color);
        }
    }
}