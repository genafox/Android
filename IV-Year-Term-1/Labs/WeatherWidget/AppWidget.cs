using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using System;
using System.Threading.Tasks;
using WeatherWidget;
using WeatherWidget.Services;

namespace TestApp.Utils
{
    [BroadcastReceiver(Label = "Weather Widget")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/widget")]
    public class AppWidget : AppWidgetProvider
    {
        private IWeatherService weatherService = new WeatherService();

        public override async void OnUpdate(Context context,
            AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            var me = new ComponentName(context, Java.Lang.Class.FromType(typeof(AppWidget)).Name);
            appWidgetManager.UpdateAppWidget(me, await BuildRemoteViews(context, appWidgetIds));
        }

        private async Task<RemoteViews> BuildRemoteViews(Context context, int[] appWidgetIds)
        {
            var widgetView = new RemoteViews(context.PackageName, Resource.Layout.Widget);

            await SetViewText(widgetView);
            RegisterClicks(context, appWidgetIds, widgetView);

            return widgetView;
        }

        private async Task SetViewText(RemoteViews widgetView)
        {
            var weather = await this.weatherService.GetWeather(DateTime.Now);

            widgetView.SetTextViewText(Resource.Id.resultsTitleViewText, $"Weather: {weather.Title}");
            widgetView.SetTextViewText(Resource.Id.tempViewText, $"Temperature: {weather.Temperature}C");
            widgetView.SetTextViewText(Resource.Id.windViewText, $"Wind Speed: {weather.Wind} km/h");
            widgetView.SetTextViewText(Resource.Id.visibilityViewText, $"Visibility:  {weather.Visibility}/10");
            widgetView.SetTextViewText(Resource.Id.humidityViewText, $"Humidity: {weather.Humidity}%");
        }

        private void RegisterClicks(Context context, int[] appWidgetIds, RemoteViews widgetView)
        {
            var intent = new Intent(context, typeof(AppWidget));
            intent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, appWidgetIds);

            // Register click event for the Background
            var piBackground = PendingIntent.GetBroadcast(context, 0, intent, PendingIntentFlags.UpdateCurrent);
            widgetView.SetOnClickPendingIntent(Resource.Id.widgetBackground, piBackground);
        }

        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);
        }
    }
}