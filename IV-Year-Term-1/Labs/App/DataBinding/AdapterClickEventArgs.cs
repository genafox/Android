using System;
using Android.Views;

namespace App.DataBinding
{
    public class AdapterClickEventArgs<T> : EventArgs
    {
        public View ItemView { get; set; }

        public T Item { get; set; }

        public int Position { get; set; }
    }
}