using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using HQI.WSHQIWebService;
using System.Web.Services.Protocols;
using System.Globalization;
using Android.Text;
using Android.Text.Method;
using Android.Views.InputMethods;

namespace HQI
{
	[Activity (MainLauncher = true)]
	public class MainActivity : Activity
	{
		// Create variables to hold user-supplied values
		string username;
		string password;
		
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			// Load the UI created in Main.axml
			RequestWindowFeature(WindowFeatures.CustomTitle);
			SetContentView (Resource.Layout.Main);
			Window.SetFeatureInt (WindowFeatures.CustomTitle, Resource.Layout._Custom_Title);

			//hide the Soft Keyboard by default
			ActivityExtensions.HideSoftKeyboard (this);

			TextView btnRegister = FindViewById<TextView> (Resource.Id.btnRegister); btnRegister.Enabled = true;
			btnRegister.TextFormatted = Html.FromHtml ("<a href='#'>Register</a>");
//			btnRegister.MovementMethod = LinkMovementMethod.Instance;
// 			Move to the Registration page when the 'Register' button is tapped
			btnRegister.Click += delegate {
				var dataToTransfer = new Intent(this, typeof(RegisterActivity));
				dataToTransfer.PutExtra("go", "Register");
				StartActivity (dataToTransfer);
			};

			TextView btnForgotPassword = FindViewById<TextView> (Resource.Id.btnForgotPassword); btnForgotPassword.Enabled =  true;
			btnForgotPassword.TextFormatted = Html.FromHtml ("<a href='#'>Forgot Password?</a>");
//			btnForgotPassword.MovementMethod = LinkMovementMethod.Instance;
			btnForgotPassword.Click += delegate {
				var dataToTransfer = new Intent(this, typeof(ForgotPasswordActivity));
				dataToTransfer.PutExtra("go", "ForgotPassword");
				StartActivity (dataToTransfer);
			};

			Button btnNewSurvey = FindViewById<Button> (Resource.Id.games_button); btnNewSurvey.Enabled = true;
			btnNewSurvey.Click += delegate {
				if (Common.loggedIn)
				{
					var dataToTransfer = new Intent(this, typeof(NewSurveyActivity));
					dataToTransfer.PutExtra("go", "Games");
					StartActivity (dataToTransfer);
				}
				else
				{
					new AlertDialog.Builder(this)
						.SetTitle("Error")
							.SetMessage("Sorry. You are not logged in.\n\nYou need to log in before you can perform that action.")
							.SetPositiveButton("Close", (s, e) => { })
							.Show();
				}
			};

			Button btnSurveys = FindViewById<Button> (Resource.Id.account_button); btnSurveys.Enabled = true;
			btnSurveys.Click += delegate {
				if (Common.loggedIn)
				{
					var dataToTransfer = new Intent(this, typeof(UpdateProfileActivity));
					dataToTransfer.PutExtra("go", "MyAccount");
					StartActivity (dataToTransfer);
				}
				else
				{
					new AlertDialog.Builder(this)
						.SetTitle("Error")
							.SetMessage("Sorry. You are not logged in.\n\nYou need to log in before you can perform that action.")
							.SetPositiveButton("Close", (s, e) => { })
							.Show();
				}
			};

			Button btnProfile = FindViewById<Button> (Resource.Id.result_button); btnProfile.Enabled = true;
			btnProfile.Click += delegate {
				if (Common.loggedIn)
				{
					var dataToTransfer = new Intent(this, typeof(MySurveysActivity));
					dataToTransfer.PutExtra("go", "Results");
					StartActivity (dataToTransfer);
				}
				else
				{
					new AlertDialog.Builder(this)
						.SetTitle("Error")
							.SetMessage("Sorry. You are not logged in.\n\nYou need to log in before you can perform that action.")
							.SetPositiveButton("Close", (s, e) => { })
							.Show();
				}
			};

			Button btnInfo = FindViewById<Button> (Resource.Id.info_button); btnInfo.Enabled = true;
			btnInfo.Click += delegate {
				if (Common.loggedIn)
				{
					var dataToTransfer = new Intent(this, typeof(InfoContactActivity));
					dataToTransfer.PutExtra("go", "Info");
					StartActivity (dataToTransfer);
				}
				else
				{
					new AlertDialog.Builder(this)
						.SetTitle("Error")
							.SetMessage("Sorry. You are not logged in.\n\nYou need to log in before you can perform that action.")
							.SetPositiveButton("Close", (s, e) => { })
							.Show();
				}
			};

			Button btnLogout = FindViewById<Button> (Resource.Id.logout_button); btnLogout.Enabled = true;
			btnLogout.Click += delegate {
				if (Common.loggedIn)
				{
					//set all user variables to null and return to the login screen
					Common.Logout();
					var dataToTransfer = new Intent(this, typeof(MainActivity));
					dataToTransfer.PutExtra("go", "Login");
					StartActivity (dataToTransfer);
					this.Finish();
				}
				else
				{
					new AlertDialog.Builder(this)
						.SetTitle("Error")
							.SetMessage("Sorry. You are not logged in.\n\nYou need to log in before you can perform that action.")
							.SetPositiveButton("Close", (s, e) => { })
							.Show();
				}
			};

			Button btnLogin = FindViewById<Button> (Resource.Id.btnLogIn);
			if (Common.loggedIn)
			{
				btnLogin.Visibility = ViewStates.Invisible;
			}
			else
			{
				btnLogin.Visibility = ViewStates.Visible;
			}
			btnLogin.Click += delegate {
				// grab user-supplied values
				username = FindViewById<EditText> (Resource.Id.editText1).Text.Trim();
				password = FindViewById<EditText> (Resource.Id.editText2).Text.Trim();

				try
				{
					// set the webservice object and define its header properties...
					HQIWebService _webService = new HQIWebService(Common.webServiceURL);

					// define the parameters, if any, required by this Web Service method
					WS_Login_Request _loginRequest = new WS_Login_Request ();
					_loginRequest.username = username;
					_loginRequest.password = password;

					// connect! and grab the response code from hitting the web service...
					WS_Login_Response _loginResponse = new WS_Login_Response();
					_loginResponse = _webService.route_Login(_loginRequest);
					if (_loginResponse.responseCode > 0)
					{
						Common.loggedIn = true;
						Common.patientRef = _loginResponse.patientRef;
						Common.contactEmail = username;
						Common.password = password;
						// Call GetUserProfile web method and populate the appropriate fields in Common.cs
						Common.GetPatientDetails(Common.patientRef);
						// set some header information
						//communicate success to the user
						CarryOutLoginSuccessAction();
						//						new AlertDialog.Builder(this)
						//							.SetTitle("Success")
						//							.SetMessage("Congrats.\n\nLogin was successful.\n\n" + GetString(Resource.String.alert_dismissal_phrase) + ".")
						//							.SetPositiveButton("Close", (s, e) => { })
						//							.Show();
						btnLogin.Enabled = false;
						btnLogin.Text = "You are logged in.";
						btnLogin.Visibility = ViewStates.Invisible;
					}
					else
					{
						new AlertDialog.Builder(this)
							.SetTitle("Error")
								.SetMessage("Sorry.\n\nInvalid login credentials.")
								.SetPositiveButton("Close", (s, e) => { })
								.Show();
					}
				}
				catch	(Exception ex)
				{
					if (ex.Message.Contains("ConnectFailure"))
					{
						new AlertDialog.Builder(this)
							.SetTitle("Error")
								.SetMessage("Sorry.\n\nIt appears your device is not connected at this time." + Common.newLine + Common.newLine + "Please verify that your device is connected and try again." + Common.newLine + Common.newLine + "Hint: Check your Wi-Fi and/or 3G connection(s).")
								.SetPositiveButton("Close", (s, e) => { })
								.Show();
					}
					else
					{
						new AlertDialog.Builder(this)
							.SetTitle ("Error")
								.SetMessage ("Sorry.\n\nAn unexpected error occurred.\n\nError Message: " + ex.Message + "\n\nStack Trace: " + ex.StackTrace + ".\n\nPlease contact " + Resources.GetString(Resource.String.customer_support_email) + ".")
								.SetPositiveButton("Close", (s, e) => { })
								.Show ();
					}
				}				
			};
		}

		private void CarryOutLoginSuccessAction()
		{
			new AlertDialog.Builder(this)
				.SetTitle ("Login")
					.SetMessage ("You were successfully logged in.")
					.SetPositiveButton("Close", (s, e) => { })
					.Show ();
		}
	}
}
