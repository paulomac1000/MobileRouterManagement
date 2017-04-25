using System;
using Android.App;
using Android.OS;
using Android.Widget;

namespace MobileRouterManagement.Views
{
    [Activity(Label = "MenuActivity")]
    public class MenuActivity : Activity
    {
        private readonly string[] menuItems = Enum.GetNames(typeof(MenuItems));
        private ListView menuListView;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Menu);
            menuListView = FindViewById<ListView>(Resource.Id.menuListView);

            var adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, menuItems);
            menuListView.Adapter = adapter;
            menuListView.ItemClick += MenuItems_ItemClick;
        }

        private void MenuItems_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Toast.MakeText(this, menuItems[e.Position], ToastLength.Short).Show();

            var assembly = typeof(MenuActivity).Assembly;
            var activityName = $"MobileRouterManagement.Views.{menuItems[e.Position]}Activity";
            var type = assembly.GetType(activityName);
            StartActivity(type);
        }
    }

    public enum MenuItems
    {
        Status,
        Wireless,
        // ReSharper disable once InconsistentNaming
        DHCP,
        Firewall,
        Logs,
        Logout
    }
}