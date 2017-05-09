using Android.App;
using Android.OS;
using Android.Widget;
using MobileRouterManagement.Core.Connection;
using System;

namespace MobileRouterManagement.Views
{
    [Activity(Label = "WirelessActivity")]
    public class ShowWirelessActivity : Activity
    {
        private TextView ssidTextView;
        private TextView keyTextView;
        private TextView encryptionTextView;
        private TextView channelTextView;
        private TextView modeTextView;
        private TextView networkTextView;
        private TextView disabledTextView;
        private Button editWirelessButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ShowWireless);

            findViewsWith();
            bindData();
            handleEvents();
        }

        public override void OnBackPressed()
        {
            Finish();
        }

        private void findViewsWith()
        {
            ssidTextView = FindViewById<TextView>(Resource.Id.ssidTextView);
            keyTextView = FindViewById<TextView>(Resource.Id.keyTextView);
            encryptionTextView = FindViewById<TextView>(Resource.Id.encryptionTextView);
            channelTextView = FindViewById<TextView>(Resource.Id.channelTextView);
            modeTextView = FindViewById<TextView>(Resource.Id.modeTextView);
            networkTextView = FindViewById<TextView>(Resource.Id.networkTextView);
            disabledTextView = FindViewById<TextView>(Resource.Id.disabledTextView);
            editWirelessButton = FindViewById<Button>(Resource.Id.editWirelessButton);
        }

        private void bindData()
        {
            var wirelessConfiguration = SshConnection.Get_Wireless();
            ssidTextView.Text = wirelessConfiguration.Ssid;
            keyTextView.Text = wirelessConfiguration.Key;
            encryptionTextView.Text = wirelessConfiguration.Encryption;
            channelTextView.Text = wirelessConfiguration.Channel;
            modeTextView.Text = wirelessConfiguration.Mode;
            networkTextView.Text = wirelessConfiguration.Network;

            disabledTextView.Text = wirelessConfiguration.Disabled ? "true" : "false";
        }

        private void handleEvents()
        {
            editWirelessButton.Click += editWireless_Click;
        }

        private void editWireless_Click(object sender, EventArgs e)
        {
            Toast.MakeText(this, "EDIT", ToastLength.Short).Show();
        }
    }
}