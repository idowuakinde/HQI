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

namespace HQI
{
//	[Activity (Label = "PlentyMillions! - Contact Us")]
	[Activity]
	public class InfoContactActivity : Activity
	{
		string url;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			try 
			{
				// Load the UI created in Main.axml
				RequestWindowFeature(WindowFeatures.CustomTitle);
				SetContentView (Resource.Layout.InfoContactLayout);
				Window.SetFeatureInt (WindowFeatures.CustomTitle, Resource.Layout._Custom_Title);

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

				// set some header information
				string patientName;
				patientName = Common.patientName;
				FindViewById<TextView> (Resource.Id.account_number).Text = "Logged In: " + Common.patientName;

				url = Intent.GetStringExtra("url");

				string introText = "<p>You can find additional information about HQI by visiting <a href='" + GetString(Resource.String.company_website) + "'>" + GetString(Resource.String.company_website).Replace("http://", "") + "</a>.</p>" + Common.newLine + Common.newLine;
				introText += "You may also contact us directly with specific questions and feedback using the email links below:";
				FindViewById<TextView> (Resource.Id.txtTitle).TextFormatted = Html.FromHtml(introText);
				FindViewById<TextView> (Resource.Id.txtTitle).MovementMethod = LinkMovementMethod.Instance;

				FindViewById<TextView> (Resource.Id.txtCR).TextFormatted = Html.FromHtml("<a href='#'>Customer Relations</a>");
				FindViewById<TextView> (Resource.Id.txtCR).Click += delegate {
					SendOrderByEmail("customercare@aesculapiusvn.com", "Customer Relations Enquiry");
				};

				FindViewById<TextView> (Resource.Id.txtSales).TextFormatted = Html.FromHtml("<a href='#'>Sales</a>");
				FindViewById<TextView> (Resource.Id.txtSales).Click += delegate {
					SendOrderByEmail("sales@aesculapiusvn.com", "Sales Enquiry");
				};

				FindViewById<TextView> (Resource.Id.txtCS).TextFormatted = Html.FromHtml("<a href='#'>Customer Support</a>");
				FindViewById<TextView> (Resource.Id.txtCS).Click += delegate {
					SendOrderByEmail("support@aesculapiusvn.com", "Customer Support Enquiry");
				};
			} 
			catch (Exception ex) 
			{
				//Console.WriteLine("This is the InnerException message: " + ((SoapException) ex).Detail.InnerXml.ToString());
				new AlertDialog.Builder(this)
					.SetTitle ("Error")
						.SetMessage ("Sorry.\n\nAn unexpected error occurred.\n\nError Message: " + ex.Message + "\n\nStack Trace: " + ex.StackTrace + ".\n\nPlease contact " + Resources.GetString(Resource.String.customer_support_email) + ".")
						.SetPositiveButton("Close", (s, e) => { })
						.Show ();
			}
		}

		private void SendOrderByEmail(string emailAddress, string subject)
		{
			try
			{
				var emailLauncher = new Intent (Android.Content.Intent.ActionSend);
				emailLauncher.PutExtra (Android.Content.Intent.ExtraEmail, new string[] { emailAddress });
				emailLauncher.PutExtra (Android.Content.Intent.ExtraSubject, subject);

				// tell the Android OS to automatically launch the default e-mail app; and if none has been defined, present the user with options from which to choose one.
				emailLauncher.SetType ("message/rfc822");

				// transfer execution control to the OS e-mail app
				StartActivity (emailLauncher);
			}
			catch (Exception ex) 
			{
				new AlertDialog.Builder(this)
					.SetTitle ("Error")
						.SetMessage ("Sorry.\n\nAn unexpected error occurred.\n\nError Message: " + ex.Message + "\n\nStack Trace: " + ex.StackTrace + ".\n\nPlease contact " + Resources.GetString(Resource.String.customer_support_email) + ".")
						.SetPositiveButton("Close", (s, e) => { })
						.Show ();
			}
		}

	}
}

