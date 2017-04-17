using System;
using System.Net.NetworkInformation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MobileRouterManagement.Core.Connection;

namespace MobileRouterManagement.Views
{
    //TODO impelment SQLite and repositories and save credinental in db onclick save button
    //TODO implemet functionality to login from saved credinentals
    //TODO implemement "advaned login options" where is possible to type username and deselect checking host adress by pinging it
    [Activity(Label = "MobileRouterManagement", MainLauncher = true, Icon = "@drawable/icon")]
    public class LoginActivity : Activity
    {
        private ImageView logoImageView;
        private EditText ipEditText;
        private EditText loginEditText;
        private EditText passwordEditText;
        private Button loginButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Login);

            findViewsWith();
            bindData();
            handleEvents();
        }

        private void findViewsWith()
        {
            logoImageView = FindViewById<ImageView>(Resource.Id.logoImageView);
            ipEditText = FindViewById<EditText>(Resource.Id.ipEditText);
            loginEditText = FindViewById<EditText>(Resource.Id.loginEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText);
            loginButton = FindViewById<Button>(Resource.Id.loginButton);
        }

        private void handleEvents()
        {
            loginButton.Click += loginButton_Click;
        }

        private void bindData()
        {
            logoImageView.SetImageResource(Resource.Drawable.Logo);
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            //TODO check:
            //ip by regex (possible port!)
            //if username is typed
            //password not have to be typed

            //TODO find how show below toast BEFORE Connect method is execuded
            Toast.MakeText(this, $"Connecting...", ToastLength.Long).Show();

            //checking adress is neceserry because is faster than try to connect to bad IP
            using (var ping = new Ping())
            {
                if (ping.Send(ipEditText.Text).Status != IPStatus.Success)
                {
                    Toast.MakeText(this, "Host is unreachable", ToastLength.Long).Show();
                    return;
                }
            }

            try
            {
                SshConnection.Connect(ipEditText.Text, loginEditText.Text, passwordEditText.Text);
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, $"Can not connect to router. {ex.Message}", ToastLength.Long).Show();
                return;
            }

            var intent = new Intent(this, typeof(MenuActivity));
            StartActivity(intent);
        }
    }
}

