using System;
using Android.App;
using Android.Widget;
using Android.OS;
using MobileRouterManagement.Core.Connection;

namespace MobileRouterManagement
{
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
            //empty here
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            var connection = new SshConnection();
            //TODO
        }
    }
}

