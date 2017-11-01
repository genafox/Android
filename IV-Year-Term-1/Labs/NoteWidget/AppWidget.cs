using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using NoteWidget.DataBinding;
using System;
using AndroidUri = Android.Net.Uri;

namespace NoteWidget
{
    [BroadcastReceiver(Label = "@string/app_name")]
    [IntentFilter(new string[] { "android.appwidget.action.APPWIDGET_UPDATE" })]
    [MetaData("android.appwidget.provider", Resource = "@xml/widget")]
    public class AppWidget : AppWidgetProvider
    {
        private const string AddNoteClick = "AddNoteClick";
        private const string NoteItemClick = "NoteItemClick";

        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            base.OnUpdate(context, appWidgetManager, appWidgetIds);

            foreach (int id in appWidgetIds)
            {
                appWidgetManager.UpdateAppWidget(id, BuildRemoteViews(context, id));
                appWidgetManager.NotifyAppWidgetViewDataChanged(id, Resource.Id.notesListView);
            }
        }

        public override void OnReceive(Context context, Intent intent)
        {
            base.OnReceive(context, intent);
            
            if (intent.Action == NoteItemClick)
            {
                int noteId = intent.GetIntExtra(NoteRemoteViewsFactory.NoteItemIdKey, default(int));
                Toast.MakeText(
                        context,
                        string.Format(context.GetString(Resource.String.note_id_toast_message), noteId),
                        ToastLength.Short)
                    .Show();
            }
        }

        private RemoteViews BuildRemoteViews(Context context, int appWidgetId)
        {
            var widgetView = new RemoteViews(context.PackageName, Resource.Layout.Widget);

            // Setup Notes List
            Intent listViewDataIntent = new Intent(context, typeof(NoteRemoteViewsService));
            listViewDataIntent.SetPackage(context.PackageName);
            listViewDataIntent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, new[] { appWidgetId });

            // To make sure that the notes list adapter (NoteRemoteViewsService.NoteRemoteViewsFactory) is created per one widget
            AndroidUri data = AndroidUri.Parse(listViewDataIntent.ToUri(IntentUriType.AndroidAppScheme));
            listViewDataIntent.SetData(data);
            widgetView.SetRemoteAdapter(Resource.Id.notesListView, listViewDataIntent);

            // List item click intent
            Intent listViewClickIntent = new Intent(context, typeof(AppWidget));
            listViewClickIntent.SetAction(NoteItemClick);
            widgetView.SetPendingIntentTemplate(
                Resource.Id.notesListView,
                PendingIntent.GetBroadcast(context, 0, listViewClickIntent, 0));

            // Setup Sync Status
            widgetView.SetTextViewText(
                Resource.Id.syncStatusTextView,
                string.Format(context.GetString(Resource.String.synced_status), DateTime.Now));

            // Setup Sync Button
            var syncBtnIntent = new Intent(context, typeof(AppWidget));
            syncBtnIntent.SetAction(AppWidgetManager.ActionAppwidgetUpdate);
            syncBtnIntent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, new[] { appWidgetId });

            widgetView.SetOnClickPendingIntent(
                Resource.Id.syncNotesIcon,
                PendingIntent.GetBroadcast(context, 0, syncBtnIntent, PendingIntentFlags.UpdateCurrent));

            // Setup Add Note Button
            var addNoteBtnIntent = new Intent(context, typeof(AppWidget));
            addNoteBtnIntent.SetAction(AddNoteClick);
            var pendingSelfIntent = PendingIntent.GetBroadcast(context, 0, addNoteBtnIntent, 0);

            widgetView.SetOnClickPendingIntent(
                Resource.Id.addNoteIcon,
                pendingSelfIntent);

            return widgetView;
        }
    }
}

