using App.Domain.Models;

namespace App.Helpers
{
	public static class NoteImportanceExtensions
	{
		public static int GetIconResource(this NoteImportance importance)
		{
			switch (importance)
			{
				case NoteImportance.High:
					return Resource.Drawable.note_importance_high_icon;
				case NoteImportance.Medium:
					return Resource.Drawable.note_importance_medium_icon;
				case NoteImportance.Low:
				default:
					return Resource.Drawable.note_importance_low_icon;
			}
		}
	}
}