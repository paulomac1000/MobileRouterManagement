﻿using Android.App;
using Android.OS;
using Android.Widget;
using MobileRouterManagement.Core.Connection;
using System;

namespace MobileRouterManagement.Views
{
    [Activity(Label = "LogoutActivity")]
    public class LogoutActivity : Activity
    {
        private Button loginAgainButton;
        private Button closeButton;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SshConnection.Disconnect();

            SetContentView(Resource.Layout.Logout);

            findViewsWith();
            bindData();
            handleEvents();
        }

        public override void OnBackPressed()
        {
            Process.KillProcess(Process.MyPid());
        }

        private void findViewsWith()
        {
            loginAgainButton = FindViewById<Button>(Resource.Id.loginAgainButton);
            closeButton = FindViewById<Button>(Resource.Id.closeButton);
        }

        private void bindData()
        {
        }

        private void handleEvents()
        {
            loginAgainButton.Click += loginAgainButton_Click;
            closeButton.Click += closeButton_Click;
        }

        private void loginAgainButton_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(LoginActivity));
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Process.KillProcess(Process.MyPid());
        }
    }
}