using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using App.Domain;
using Android.App;
using Android.Graphics;
using Android.Content;

namespace App.Helpers
{
    public class AppearanceHelper
    {
        public static void ApplySettings(Activity activity, SettingsModel settings)
        {
            ViewGroup root = activity.Window.DecorView.FindViewById<ViewGroup>(Android.Resource.Id.Content);

            ApplySettingsForControls(root, activity.ApplicationContext, settings);
        }

        public static void ApplySettings(View view, Context context, SettingsModel settings)
        {
            var rootView = view as ViewGroup;
            if(rootView != null)
            {
                ApplySettingsForControls(rootView, context, settings);
            }
        }

        private static void ApplySettingsForControls(ViewGroup rootView, Context context, SettingsModel settings)
        {
            foreach (TextView control in GetViewsByType<TextView>(rootView))
            {
                control.TextSize = settings.FontSize;

                var font = Typeface.CreateFromAsset(context.Assets, settings.FontPath);
                control.Typeface = font;
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