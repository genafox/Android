using Android.Telephony;
using App.Activities;

namespace App.Listeners
{
    public class PhoneCallListener : PhoneStateListener
    {
        private readonly PlayerActivity activity;

        public PhoneCallListener(PlayerActivity activity)
        {
            this.activity = activity;
        }

        public override void OnCallStateChanged(CallState state, string incomingNumber)
        {
            base.OnCallStateChanged(state, incomingNumber);
            this.activity.UpdateCallState(state, incomingNumber);
        }
    }
}