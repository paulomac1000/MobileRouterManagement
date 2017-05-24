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
    [Activity(Label = "EditFirewallRule")]
    public class EditFirewallRuleActivity : Activity
    {
        private string ruleName;
        private EditText editRuleNameEditText;
        private EditText editRuleSourceMacEditText;
        private EditText editRuleSourceIpEditText;
        private EditText editRuleSourcePortEditText;
        private EditText editRuleDestinationIpEditText;
        private EditText editRuleDestinationPortEditText;
        private CheckBox editRuleEnabledCheckBox;
        private Button saveModifiedRuleButton;
        private Button exitEditRuleButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.EditFirewallRule);

            ruleName = Intent.GetStringExtra("RuleName");

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
            editRuleNameEditText = FindViewById<EditText>(Resource.Id.editRuleNameEditText);
            editRuleSourceMacEditText = FindViewById<EditText>(Resource.Id.editRuleSourceMacEditText);
            editRuleSourceIpEditText = FindViewById<EditText>(Resource.Id.editRuleSourceIpEditText);
            editRuleSourcePortEditText = FindViewById<EditText>(Resource.Id.editRuleSourcePortEditText);
            editRuleDestinationIpEditText = FindViewById<EditText>(Resource.Id.editRuleDestinationIpEditText);
            editRuleDestinationPortEditText = FindViewById<EditText>(Resource.Id.editRuleDestinationPortEditText);
            editRuleEnabledCheckBox = FindViewById<CheckBox>(Resource.Id.editRuleEnabledCheckBox);
            saveModifiedRuleButton = FindViewById<Button>(Resource.Id.saveModifedRuleButton);
            exitEditRuleButton = FindViewById<Button>(Resource.Id.exitEditRuleButton);
        }

        private void bindData()
        {
            var firewallRule = FirewallConnection.Get_FirewallRuleByName(ruleName);
            editRuleNameEditText.Text = firewallRule.FriendlyName;
            editRuleSourceMacEditText.Text = (firewallRule.Src_mac != null) ? string.Join(", ", firewallRule.Src_mac) : string.Empty;
            editRuleSourceIpEditText.Text = (firewallRule.Src_ip != null) ? string.Join(", ", firewallRule.Src_ip) : string.Empty;
            editRuleSourcePortEditText.Text = (firewallRule.Src_port != null) ? string.Join(", ", firewallRule.Src_port) : string.Empty;
            editRuleDestinationIpEditText.Text = (firewallRule.Dest_ip != null) ? string.Join(", ", firewallRule.Dest_ip) : string.Empty;
            editRuleDestinationPortEditText.Text = (firewallRule.Dest_port != null) ? string.Join(", ", firewallRule.Dest_port) : string.Empty;

            editRuleEnabledCheckBox.Checked = firewallRule.Enabled.Contains('1');
        }

        private void handleEvents()
        {
            saveModifiedRuleButton.Click += SaveModifiedRuleButtonClick;
            exitEditRuleButton.Click += exitEditRuleButton_Click;
        }

        private void SaveModifiedRuleButtonClick(object sender, EventArgs e)
        {
            var valid = true;

            if (FirewallConnection.Get_RestrictionRulesNames().Contains(editRuleNameEditText.Text))
            {
                Toast.MakeText(this, $"Rule with this name already exist.", ToastLength.Short).Show();
                valid = false;
            }

            if (string.IsNullOrEmpty(editRuleNameEditText.Text))
            {
                Toast.MakeText(this, $"Rule name can't be empty.", ToastLength.Short).Show();
                valid = false;
            }

            if (!editRuleSourceMacEditText.Text.Any() &&
                !editRuleSourceIpEditText.Text.Any() &&
                !editRuleSourcePortEditText.Text.Any() &&
                !editRuleDestinationIpEditText.Text.Any() &&
                !editRuleDestinationPortEditText.Text.Any())
            {
                Toast.MakeText(this, $"You have to type at least one condition.", ToastLength.Short).Show();
                valid = false;
            }

            const string validateMacPattern = @"^$|^(((([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})[,])*)(([0-9A-Fa-f]{2}[:-]){5}([0-9A-Fa-f]{2})))$";
            const string validateIpPattern = @"^$|^((((([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])[,])*)((([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])))$";
            const string validatePortPattern = @"^$|^((([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])[,])|((([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])([-])([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5]))[,]))*((([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5]))|(([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])([-])([0-9]{1,4}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])))$";

            if (!Regex.Match(editRuleSourceMacEditText.Text, validateMacPattern, RegexOptions.IgnoreCase).Success)
            {
                Toast.MakeText(this, $"You have typed source Mac in not proper format.", ToastLength.Short).Show();
                valid = false;
            }

            if (!Regex.Match(editRuleSourceIpEditText.Text, validateIpPattern, RegexOptions.IgnoreCase).Success)
            {
                Toast.MakeText(this, $"You have typed source IP in not proper format.", ToastLength.Short).Show();
                valid = false;
            }

            if (!Regex.Match(editRuleSourcePortEditText.Text, validatePortPattern, RegexOptions.IgnoreCase).Success)
            {
                Toast.MakeText(this, $"You have typed source port in not proper format.", ToastLength.Short).Show();
                valid = false;
            }

            if (!Regex.Match(editRuleDestinationIpEditText.Text, validateIpPattern, RegexOptions.IgnoreCase).Success)
            {
                Toast.MakeText(this, $"You have typed destination IP in not proper format.", ToastLength.Short).Show();
                valid = false;
            }

            if (!Regex.Match(editRuleDestinationPortEditText.Text, validatePortPattern, RegexOptions.IgnoreCase).Success)
            {
                Toast.MakeText(this, $"You have typed destination port in not proper format.", ToastLength.Short).Show();
                valid = false;
            }

            if (!valid) return;

            FirewallConnection.Send_DeleteFirewallRule(ruleName);
            var modifiedRule = new AddFirewallRuleViewModel
            {
                RuleName = ruleName,
                FriendlyName = editRuleNameEditText.Text,
                SourceMacs = editRuleSourceMacEditText.Text,
                SourceIPs = editRuleSourceIpEditText.Text,
                SourcePorts = editRuleSourcePortEditText.Text,
                DestinationIPs = editRuleDestinationIpEditText.Text,
                DestinationPorts = editRuleDestinationPortEditText.Text,
                Enabled = editRuleEnabledCheckBox.Enabled ? "1" : "0"
            };
            FirewallConnection.Send_SaveFirewallRule(modifiedRule);

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