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

        public override void OnBackPressed(){}

        private void MenuItems_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            var position = (MenuItems)e.Position;
            switch (position)
            {
                case MenuItems.Status:
                    Toast.MakeText(this, "Function is not implemented yet.", ToastLength.Short).Show();
                    break;

                case MenuItems.Wireless:
                    StartActivity(typeof(ShowWirelessActivity));
                    break;

                case MenuItems.DHCP:
                    Toast.MakeText(this, "Function is not implemented yet.", ToastLength.Short).Show();
                    break;

                case MenuItems.Firewall:
                    Toast.MakeText(this, "Function is not implemented yet.", ToastLength.Short).Show();
                    break;

                case MenuItems.Logs:
                    Toast.MakeText(this, "Function is not implemented yet.", ToastLength.Short).Show();
                    break;

                case MenuItems.Logout:
                    StartActivity(typeof(LogoutActivity));
                    break;

                default:
                    Toast.MakeText(this, "Function is not recognized.", ToastLength.Short).Show();
                    break;
            }

            Finish();
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