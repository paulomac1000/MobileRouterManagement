using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using Android.Views.InputMethods;
using MobileRouterManagement.Core.Connection;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Android.Content;
using Exception = System.Exception;

namespace MobileRouterManagement.Views
{
    //TODO impelment SQLite and repositories and save credinental in db onclick save button
    //TODO implemet functionality to login from saved credinentals
    //TODO implemement "advaned login options" where is possible to type username and deselect checking host adress by pinging it
    [Activity(Label = "MobileRouterManagement", MainLauncher = true, Icon = "@drawable/icon", NoHistory = true)]
    public class LoginActivity : Activity
    {
        private ImageView logoImageView;
        private EditText ipEditText;
        private EditText passwordEditText;
        private Button loginButton;
        private View loadingPanel;
        private bool isLogging = false;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Login);

            findViewsWith();
            bindData();
            handleEvents();
        }

        public override void OnBackPressed()
        {
            Finish();
            Process.KillProcess(Process.MyPid());
        }

        private void findViewsWith()
        {
            logoImageView = FindViewById<ImageView>(Resource.Id.logoImageView);
            ipEditText = FindViewById<EditText>(Resource.Id.ipEditText);
            passwordEditText = FindViewById<EditText>(Resource.Id.passwordEditText);
            loginButton = FindViewById<Button>(Resource.Id.loginButton);
            loadingPanel = FindViewById(Resource.Id.loadingPanel);
        }

        private void bindData()
        {
            logoImageView.SetImageResource(Resource.Drawable.Logo);
            loadingPanel.Visibility = ViewStates.Invisible;
        }

        private void handleEvents()
        {
            loginButton.Click += loginButton_Click;
        }

        private async void loginButton_Click(object sender, EventArgs e)
        {
            //TODO check:
            //ip by regex (possible port!)
            //if username is typed
            //password not have to be typed

            //dissallow to multiple pressing login button
            if (isLogging) return;

            //force hide keyboard
            var inputMethodManager = (InputMethodManager)GetSystemService(InputMethodService);
            inputMethodManager.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);

            loadingPanel.Visibility = ViewStates.Visible;
            Toast.MakeText(this, $"Connecting...", ToastLength.Long).Show();

            try
            {
                await tryConnect();
                StartActivity(typeof(MenuActivity));
            }
            catch (Exception exception)
            {
                Toast.MakeText(this, exception.Message, ToastLength.Long).Show();
            }
            finally
            {
                loadingPanel.Visibility = ViewStates.Invisible;
            }
        }

        private async Task tryConnect()
        {
            await Task.Run(() =>
            {
                isLogging = true;

                //checking adress is neceserry because is faster than try to connect to bad IP
                using (var ping = new Ping())
                {
                    if (ping.Send(ipEditText.Text).Status != IPStatus.Success)
                    {
                        isLogging = false;
                        throw new Exception("Host is unreachable");
                    }
                }

                try
                {
                    SshConnection.Connect(ipEditText.Text, "root", passwordEditText.Text);
                }
                catch (Exception ex)
                {
                    isLogging = false;
                    throw new Exception($"Can not connect to router. {ex.Message}");
                }

                isLogging = false;

                StartActivity(typeof(MenuActivity));
            });
        }
    }
}