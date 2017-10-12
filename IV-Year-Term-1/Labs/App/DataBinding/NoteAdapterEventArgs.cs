using System;
using Android.Views;

namespace App.DataBinding
{
    public class NoteAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }

        public int Position { get; set; }
    }
}