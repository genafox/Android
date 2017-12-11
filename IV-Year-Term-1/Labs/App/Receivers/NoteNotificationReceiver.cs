using Android.Content;
using Android.App;
using App.Activities;
using Newtonsoft.Json;
using App.Domain.Database.Models;

namespace App.Receivers
{
    [BroadcastReceiver]
    public class NoteNotificationReceiver : BroadcastReceiver
    {
        public const string NoteKey = "noteKey";

        public override void OnReceive(Context context, Intent intent)
        {
            string jsonNote = intent.Extras.GetString(NoteKey);
            var noteData = JsonConvert.DeserializeObject<Note>(jsonNote);

            this.ShowNotification(noteData, context);
        }

        private void ShowNotification(Note note, Context context)
        {
            // Set up an intent so that tapping the notifications returns to this app:
            Intent notesActivityIntent = new Intent(context, typeof(NotesActivity));
            notesActivityIntent.SetFlags(ActivityFlags.ClearTop);

            int pendingIntentId = 1;
            var pendingIntent = PendingIntent.GetActivity(context, pendingIntentId, notesActivityIntent, PendingIntentFlags.OneShot);

            Java.Util.Calendar calendar = Java.Util.Calendar.Instance;
            var expDate = note.ExpirationDate;
            calendar.Set(expDate.Year, expDate.Month - 1, expDate.Day, expDate.Hour, expDate.Minute, expDate.Second);

            // Instantiate the builder and set notification elements:
            Notification.Builder builder = new Notification.Builder(context)
                .SetContentIntent(pendingIntent)
                .SetContentTitle(note.Name)
                .SetContentText(note.Description)
                .SetSmallIcon(Resource.Drawable.noteItem_icon)
                .SetDefaults(NotificationDefaults.All)
                .SetWhen(calendar.TimeInMillis);

            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            int notificationId = 1;
            notificationManager.Notify(notificationId, notification);
        }
    }
}