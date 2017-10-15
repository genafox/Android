using Android.Views;

namespace App.DataBinding
{
	public class NoteViewHolderClickEventArgs
	{
		public View ItemView { get; set; }

		public int Position { get; set; }
	}
}