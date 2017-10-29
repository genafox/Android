using Android.App;
using Android.OS;
using Android.Appwidget;
using Android.Content;

namespace NoteWidget
{
    [BroadcastReceiver(Label = "@string/app_name")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/appwidgetprovider")]
    public class AppWidget : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
        }

        public override void OnReceive(Context context, Intent intent)
        {
        }
    }
}

