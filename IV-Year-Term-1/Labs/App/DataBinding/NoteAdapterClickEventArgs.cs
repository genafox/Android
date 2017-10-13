using System;
using App.Domain.Models;

namespace App.DataBinding
{
	public class NoteAdapterClickEventArgs : EventArgs
	{
		public Note Note { get; set; }

		public int Position { get; set; }
	}
}