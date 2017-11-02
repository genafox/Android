using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using App.Domain;
using Android.App;

namespace App.Helpers
{
    public class AppearanceHelper
    {
        public static void ApplySettings(Activity activity, SettingsModel settings)
        {
            ViewGroup root = activity.Window.DecorView.FindViewById<ViewGroup>(Android.Resource.Id.Content);

            foreach (TextView control in GetViewsByType<TextView>(root))
            {
                control.TextSize = settings.FontSize;
            }
        }

        public static void ApplySettings(View view, SettingsModel settings)
        {
            var rootView = view as ViewGroup;
            if(rootView != null)
            {
                foreach (TextView control in GetViewsByType<TextView>(rootView))
                {
                    control.TextSize = settings.FontSize;
                }
            }
        }

        private static IEnumerable<T> GetViewsByType<T>(ViewGroup root) where T : View
        {
            var children = root.ChildCount;
            var views = new List<T>();
            for (var i = 0; i < children; i++)
            {
                var child = root.GetChildAt(i);
                if (child is T myChild)
                    views.Add(myChild);
                else if (child is ViewGroup viewGroup)
                    views.AddRange(GetViewsByType<T>(viewGroup));
            }
            return views;
        }
    }
}