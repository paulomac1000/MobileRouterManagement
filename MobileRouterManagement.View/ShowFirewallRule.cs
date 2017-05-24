using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MobileRouterManagement.Core.Connection;
using System;
using System.Linq;

namespace MobileRouterManagement.Views
{
    [Activity(Label = "ShowFirewallRule")]
    public class ShowFirewallRule : Activity
    {
        private string ruleName;
        private TextView showRuleNameTextView;
        private TextView showRuleSourceMacTextView;
        private TextView showRuleSourceIpTextView;
        private TextView showRuleSourcePortTextView;
        private TextView showRuleDestinationIpTextView;
        private TextView showRuleDestinationPortTextView;
        private TextView showRuleEnabledTextView;
        private Button editRuleButton;
        private Button deleteRuleButton;
        private Button exitShowRuleButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ShowFirewallRule);

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
            showRuleNameTextView = FindViewById<TextView>(Resource.Id.showRuleNameTextView);
            showRuleSourceMacTextView = FindViewById<TextView>(Resource.Id.showRuleSourceMacTextView);
            showRuleSourceIpTextView = FindViewById<TextView>(Resource.Id.showRuleSourceIpTextView);
            showRuleSourcePortTextView = FindViewById<TextView>(Resource.Id.showRuleSourcePortTextView);
            showRuleDestinationIpTextView = FindViewById<TextView>(Resource.Id.showRuleDestinationIpTextView);
            showRuleDestinationPortTextView = FindViewById<TextView>(Resource.Id.showRuleDestinationPortTextView);
            showRuleEnabledTextView = FindViewById<TextView>(Resource.Id.showRuleEnabledTextView);
            editRuleButton = FindViewById<Button>(Resource.Id.editRuleButton);
            deleteRuleButton = FindViewById<Button>(Resource.Id.deleteRuleButton);
            exitShowRuleButton = FindViewById<Button>(Resource.Id.exitShowRuleButton);
        }

        private void bindData()
        {
            var firewallRule = FirewallConnection.Get_FirewallRuleByName(ruleName);
            showRuleNameTextView.Text = firewallRule.FriendlyName;
            showRuleSourceMacTextView.Text = (firewallRule.Src_mac != null) ? string.Join(", ", firewallRule.Src_mac) : string.Empty;
            showRuleSourceIpTextView.Text = (firewallRule.Src_ip != null) ? string.Join(", ", firewallRule.Src_ip) : string.Empty;
            showRuleSourcePortTextView.Text = (firewallRule.Src_port != null) ? string.Join(", ", firewallRule.Src_port) : string.Empty;
            showRuleDestinationIpTextView.Text = (firewallRule.Dest_ip != null) ? string.Join(", ", firewallRule.Dest_ip) : string.Empty;
            showRuleDestinationPortTextView.Text = (firewallRule.Dest_port != null) ? string.Join(", ", firewallRule.Dest_port) : string.Empty;

            showRuleEnabledTextView.Text = firewallRule.Enabled.Contains('1') ? "true" : "false";
        }

        private void handleEvents()
        {
            editRuleButton.Click += editRuleButton_Click;
            deleteRuleButton.Click += deleteRuleButton_Click;
            exitShowRuleButton.Click += exitShowRule_Click;
        }

        private void editRuleButton_Click(object sender, EventArgs e)
        {
            var showRuleActivity = new Intent(this, typeof(EditFirewallRuleActivity));
            showRuleActivity.PutExtra("RuleName", ruleName);
            StartActivity(showRuleActivity);
        }

        private void deleteRuleButton_Click(object sender, EventArgs e)
        {
            try
            {
                FirewallConnection.Send_DeleteFirewallRule(ruleName);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, $"Error occured when deleting firewall rule. Error content: {ex.Message}", ToastLength.Short).Show();
            }
            StartActivity(typeof(FirewallActivity));
            Finish();
        }

        private void exitShowRule_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(FirewallActivity));
            Finish();
        }
    }
}