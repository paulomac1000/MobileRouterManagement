using Android.App;
using Android.OS;
using Android.Widget;
using MobileRouterManagement.Core.Connection;
using MobileRouterManagement.Core.Models.ViewModels;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace MobileRouterManagement.Views
{
    [Activity(Label = "AddFirewallRuleActivity")]
    public class AddFirewallRuleActivity : Activity
    {
        private EditText addRuleNameEditText;
        private EditText addRuleSourceMacEditText;
        private EditText addRuleSourceIpEditText;
        private EditText addRuleSourcePortEditText;
        private EditText addRuleDestinationIpEditText;
        private EditText addRuleDestinationPortEditText;
        private CheckBox addRuleEnabledCheckBox;
        private Button saveModifiedRuleButton;
        private Button exitEditRuleButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.AddFirewallRule);

            findViewsWith();
            bindData();
            handleEvents();
        }

        public override void OnBackPressed()
        {
            StartActivity(typeof(FirewallActivity));
            Finish();
        }

        private void findViewsWith()
        {
            addRuleNameEditText = FindViewById<EditText>(Resource.Id.addRuleNameEditText);
            addRuleSourceMacEditText = FindViewById<EditText>(Resource.Id.addRuleSourceMacEditText);
            addRuleSourceIpEditText = FindViewById<EditText>(Resource.Id.addRuleSourceIpEditText);
            addRuleSourcePortEditText = FindViewById<EditText>(Resource.Id.addRuleSourcePortEditText);
            addRuleDestinationIpEditText = FindViewById<EditText>(Resource.Id.addRuleDestinationIpEditText);
            addRuleDestinationPortEditText = FindViewById<EditText>(Resource.Id.addRuleDestinationPortEditText);
            addRuleEnabledCheckBox = FindViewById<CheckBox>(Resource.Id.addRuleEnabledCheckBox);
            saveModifiedRuleButton = FindViewById<Button>(Resource.Id.saveModifedRuleButton);
            exitEditRuleButton = FindViewById<Button>(Resource.Id.exitEditRuleButton);
        }

        private void bindData(){}

        private void handleEvents()
        {
            saveModifiedRuleButton.Click += SaveModifiedRuleButtonClick;
            exitEditRuleButton.Click += exitEditRuleButton_Click;
        }

        private void SaveModifiedRuleButtonClick(object sender, EventArgs e)
        {
            var valid = true;

            if (FirewallConnection.Get_RestrictionRulesNames().Contains(addRuleNameEditText.Text))
            {
                Toast.MakeText(this, $"Rule with this name already exist.", ToastLength.Short).Show();
                valid = false;
            }

            if (string.IsNullOrEmpty(addRuleNameEditText.Text))
            {
                Toast.MakeText(this, $"Rule name can't be empty.", ToastLength.Short).Show();
                valid = false;
            }

            if (!addRuleSourceMacEditText.Text.Any() &&
                !addRuleSourceIpEditText.Text.Any() &&
                !addRuleSourcePortEditText.Text.Any() &&
                !addRuleDestinationIpEditText.Text.Any() &&
                !addRuleDestinationPortEditText.Text.Any())
            {
                Toast.MakeText(this, $"You have to type at least one condition.", ToastLength.Short).Show();
                valid = false;
            }

            const string validateMacPattern = @"^$|^(((([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})[,])*)(([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})))$";
            const string validateIpPattern = @"^$|^((((([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])[,])*)((([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])))$";
            const string validatePortPattern = @"^$|^((([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])[,])|((([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])([-])([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5]))[,]))*((([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5]))|(([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])([-])([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])))$";

            if (!Regex.Match(addRuleSourceMacEditText.Text, validateMacPattern, RegexOptions.IgnoreCase).Success)
            {
                Toast.MakeText(this, $"You have typed source Mac in not proper format.", ToastLength.Short).Show();
                valid = false;
            }

            if (!Regex.Match(addRuleSourceIpEditText.Text, validateIpPattern, RegexOptions.IgnoreCase).Success)
            {
                Toast.MakeText(this, $"You have typed source IP in not proper format.", ToastLength.Short).Show();
                valid = false;
            }

            if (!Regex.Match(addRuleSourcePortEditText.Text, validatePortPattern, RegexOptions.IgnoreCase).Success)
            {
                Toast.MakeText(this, $"You have typed source port in not proper format.", ToastLength.Short).Show();
                valid = false;
            }

            if (!Regex.Match(addRuleDestinationIpEditText.Text, validateIpPattern, RegexOptions.IgnoreCase).Success)
            {
                Toast.MakeText(this, $"You have typed destination IP in not proper format.", ToastLength.Short).Show();
                valid = false;
            }

            if (!Regex.Match(addRuleDestinationPortEditText.Text, validatePortPattern, RegexOptions.IgnoreCase).Success)
            {
                Toast.MakeText(this, $"You have typed destination port in not proper format.", ToastLength.Short).Show();
                valid = false;
            }

            if (!valid) return;

            var newRule = new AddFirewallRuleViewModel
            {
                RuleName = null,
                FriendlyName = addRuleNameEditText.Text,
                SourceMacs = addRuleSourceMacEditText.Text,
                SourceIPs = addRuleSourceIpEditText.Text,
                SourcePorts = addRuleSourcePortEditText.Text,
                DestinationIPs = addRuleDestinationIpEditText.Text,
                DestinationPorts = addRuleDestinationPortEditText.Text,
                Enabled = addRuleEnabledCheckBox.Enabled ? "1" : "0"
            };
            FirewallConnection.Send_SaveFirewallRule(newRule);

            StartActivity(typeof(FirewallActivity));
            Finish();
        }

        private void exitEditRuleButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(FirewallActivity));
            Finish();
        }
    }
}