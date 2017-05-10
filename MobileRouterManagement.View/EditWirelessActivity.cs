using Android.App;
using Android.OS;
using Android.Widget;
using MobileRouterManagement.Core.Connection;
using System;
using System.Linq;
using MobileRouterManagement.Core.Model;

namespace MobileRouterManagement.Views
{
    [Activity(Label = "EditWirelessActivity")]
    public class EditWirelessActivity : Activity
    {
        private EditText ssidEditText;
        private EditText keyEditText;
        private Spinner encryptionSpinner;
        private Spinner channelSpinner;
        private Spinner modeSpinner;
        private Spinner networkSpinner;
        private CheckBox disabledCheckbox;
        private Button backWirelessButton;
        private Button saveWirelessButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.EditWireless);

            findViewsWith();
            bindData();
            setDefaultValues();
            handleEvents();
        }

        public override void OnBackPressed()
        {
            Finish();
        }

        private void findViewsWith()
        {
            ssidEditText = FindViewById<EditText>(Resource.Id.ssidEditText);
            keyEditText = FindViewById<EditText>(Resource.Id.keyEditText);
            encryptionSpinner = FindViewById<Spinner>(Resource.Id.encryptionSpinner);
            channelSpinner = FindViewById<Spinner>(Resource.Id.channelSpinner);
            modeSpinner = FindViewById<Spinner>(Resource.Id.modeSpinner);
            networkSpinner = FindViewById<Spinner>(Resource.Id.networkSpinner);
            disabledCheckbox = FindViewById<CheckBox>(Resource.Id.disabledCheckbox);
            backWirelessButton = FindViewById<Button>(Resource.Id.backWirelessButton);
            saveWirelessButton = FindViewById<Button>(Resource.Id.saveWirelessButton);
        }

        private void bindData()
        {
            var channelItems = Enumerable.Range(1, 13).Select(it => Convert.ToString(it)).ToList();
            var channelAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, channelItems);
            channelSpinner.Adapter = channelAdapter;

            var encryptionItems = Enum.GetNames(typeof(Encryption));
            var encryptionAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, encryptionItems);
            encryptionSpinner.Adapter = encryptionAdapter;

            var modeItems = Enum.GetNames(typeof(Mode));
            var modeAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, modeItems);
            modeSpinner.Adapter = modeAdapter;

            var networkItems = Enum.GetNames(typeof(Network));
            var networkAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, networkItems);
            networkSpinner.Adapter = networkAdapter;
        }

        private void setDefaultValues()
        {
            var wirelessConfiguration = SshConnection.Get_Wireless();
            ssidEditText.Text = wirelessConfiguration.Ssid;
            keyEditText.Text = wirelessConfiguration.Key;
            encryptionSpinner.SetSelection((int)(Encryption)Enum.Parse(typeof(Encryption), wirelessConfiguration.Encryption.ToLower()));
            channelSpinner.SetSelection(Convert.ToInt32(wirelessConfiguration.Channel) - 1);
            modeSpinner.SetSelection((int)(Mode)Enum.Parse(typeof(Mode), wirelessConfiguration.Mode.ToLower()));
            networkSpinner.SetSelection((int)(Network)Enum.Parse(typeof(Network), wirelessConfiguration.Network.ToLower()));
            disabledCheckbox.Enabled = wirelessConfiguration.Disabled;
        }

        private void handleEvents()
        {
            backWirelessButton.Click += backWireless_Click;
            saveWirelessButton.Click += saveWireless_Click;
        }

        private void backWireless_Click(object sender, EventArgs e)
        {
            Finish();
        }

        private void saveWireless_Click(object sender, EventArgs e)
        {
            var newWirelessConfiguration = new Wireless
            {
                Ssid = ssidEditText.Text.Replace("\r", ""),
                Key = keyEditText.Text,
                Encryption = Enum.GetName(typeof(Encryption), encryptionSpinner.SelectedItemId),
                Channel = (channelSpinner.SelectedItemId + 1).ToString(),
                Mode = Enum.GetName(typeof(Mode), modeSpinner.SelectedItemId),
                Network = Enum.GetName(typeof(Network), networkSpinner.SelectedItemId),
                Disabled = disabledCheckbox.Enabled
            };
            SshConnection.Send_SaveWireless(newWirelessConfiguration);

            Toast.MakeText(this, "New configuration has been saved. Log in again to router.", ToastLength.Short).Show();
            StartActivity(typeof(LoginActivity));
            Finish();
        }
    }

    public enum Encryption
    {
        // ReSharper disable once InconsistentNaming
        wep,

        // ReSharper disable once InconsistentNaming
        psk,

        // ReSharper disable once InconsistentNaming
        psk2
    }

    public enum Mode
    {
        // ReSharper disable once InconsistentNaming
        sta,

        // ReSharper disable once InconsistentNaming
        ap
    }

    public enum Network
    {
        // ReSharper disable once InconsistentNaming
        lan,

        // ReSharper disable once InconsistentNaming
        wan
    }
}