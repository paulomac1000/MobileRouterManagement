using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MobileRouterManagement.Core.Connection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MobileRouterManagement.Views
{
    [Activity(Label = "FirewallActivity")]
    public class FirewallActivity : Activity
    {
        private Dictionary<string, string> firewallNames;
        private Button addNewRuleButton;
        private ListView firewall;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Firewall);

            findViewsWith();
            bindData();
            handleEvents();
        }

        public override void OnBackPressed()
        {
            StartActivity(typeof(MenuActivity));
            Finish();
        }

        private void findViewsWith()
        {
            firewall = FindViewById<ListView>(Resource.Id.firewallListView);
            addNewRuleButton = FindViewById<Button>(Resource.Id.addNewRuleButton);
        }

        private void bindData()
        {
            firewallNames = FirewallConnection.Get_AllFirewallRestrictionRules()
                .Select((ruleName, friendlyName) => new { ruleName.RuleName, ruleName.FriendlyName })
                .ToDictionary(it => it.RuleName, x => x.FriendlyName);

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, firewallNames.Select(rule => rule.Value).ToArray());
            if (adapter.Count <= 0) return;
            firewall.Adapter = adapter;
        }

        private void handleEvents()
        {
            addNewRuleButton.Click += addNewRuleButton_ItemClick;
            firewall.ItemClick += firewallRulesNames_ItemClick;
        }

        private void addNewRuleButton_ItemClick(object sender, EventArgs e)
        {
            StartActivity(typeof(AddFirewallRuleActivity));
            Finish();
        }

        private void firewallRulesNames_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var position = e.Position;

            var showRuleActivity = new Intent(this, typeof(ShowFirewallRule));
            showRuleActivity.PutExtra("RuleName", firewallNames.ElementAt(position).Key);
            StartActivity(showRuleActivity);

            Finish();
        }
    }
}