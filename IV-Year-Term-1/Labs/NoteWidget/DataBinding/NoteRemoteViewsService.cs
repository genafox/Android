using Android.App;
using Android.Content;
using Android.Widget;

namespace NoteWidget.DataBinding
{
    [Service(Permission = "android.permission.BIND_REMOTEVIEWS", Exported = false)]
    public class NoteRemoteViewsService : RemoteViewsService
    {
        public override IRemoteViewsFactory OnGetViewFactory(Intent intent)
        {
            return new NoteRemoteViewsFactory(this.ApplicationContext);
        }
    }
}