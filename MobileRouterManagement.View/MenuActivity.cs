using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileRouterManagement.Views
{
    [Activity(Label = "MenuActivity")]
    public class MenuActivity : Activity
    {
        private ListView menuListView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Menu);
            menuListView = FindViewById<ListView>(Resource.Id.menuListView);

            var menuItems = new List<string> { "Status", "Wireless", "DHCP", "Firewall", "Logs", "Logout" };
            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, menuItems);
            menuListView.Adapter = adapter;
            menuListView.ItemClick += MenuItems_ItemClick;
        }

        private void MenuItems_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Toast.MakeText(this, e.Position.ToString(), ToastLength.Short).Show();
        }
    }
}