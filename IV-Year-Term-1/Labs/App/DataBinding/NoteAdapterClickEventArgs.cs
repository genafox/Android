using System;
using Android.Views;
using App.Domain.Models;

namespace App.DataBinding
{
    public class NoteAdapterClickEventArgs : EventArgs
    {
        public View ItemView { get; set; }

        public Note Note { get; set; }

        public int Position { get; set; }
    }
}