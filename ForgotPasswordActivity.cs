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

namespace HQI
{
	[Activity]
	public class ForgotPasswordActivity : Activity
	{
		// Create variables to hold user-supplied values
		string contactEmail;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Load the UI created in Main.axml
			RequestWindowFeature(WindowFeatures.CustomTitle);
			SetContentView (Resource.Layout.ForgotPasswordLayout);
			Window.SetFeatureInt (WindowFeatures.CustomTitle, Resource.Layout._Custom_Title);
									
			Button btnLogin = FindViewById<Button> (Resource.Id.fgp_btnLogin);
			btnLogin.Click += delegate {
				var dataToTransfer = new Intent(this, typeof(MainActivity));
				dataToTransfer.PutExtra("go", "Login");
				StartActivity (dataToTransfer);
			};

			// Get the button from the layout resource, and attach an event to it
			Button btnRetrieve = FindViewById<Button> (Resource.Id.fgp_btnRetrieve);
			btnRetrieve.Click += delegate {

			// grab user-supplied values
			contactEmail = FindViewById<EditText> (Resource.Id.txtEmail).Text.Trim();

				try
				{
					// set the webservice object and define its header properties...
					HQIWebService _webService = new HQIWebService(Common.webServiceURL);

					// define the sign-on parameters: essentially username and password
					WS_Patient_GetByEmail_Request _getPatientByEmailRequest = new WS_Patient_GetByEmail_Request();
					_getPatientByEmailRequest.contactEmail = contactEmail;

					// connect! and grab the response code from hitting the web service...
					WS_Patient_GetByEmail_Response _getPatientByEmailResponse = new WS_Patient_GetByEmail_Response();
					_getPatientByEmailResponse = _webService.route_Patient_GetByEmail(_getPatientByEmailRequest);
					if (_getPatientByEmailResponse.responseCode > 0)
					{
						// send the retrieved password, by e-mail, to user
						string emailText = "Congrats.\n\nYour password was successfully retrieved.\n\nYour password is " + _getPatientByEmailResponse.patient.password + ".\n\nKind regards,\n\nThe HQI Team.\n\n<a href='http://www.aesculapiusvn.com'>www.aesculapiusvn.com</a>";
						string[] emailAddresses = new string[] {_getPatientByEmailResponse.patient.contactEmail};
						string emailSubject = "HQI - Your password retrieval request.";
						if (Common.SendEmail(emailText, emailSubject, emailAddresses))
						{
							// communicate success to the user
							new AlertDialog.Builder(this)
								.SetTitle("Success")
									.SetMessage("Congrats.\n\nYour password was successfully retrieved, and has been sent to the email address you supplied (" + contactEmail + ").")
									.SetPositiveButton("Close", (s, e) => { })
									.Show();
						}
						else
						{
							new AlertDialog.Builder(this)
								.SetTitle("Error")
									.SetMessage("Sorry.\n\nAn error occurred that prevented us from sending your retrieved password to you by e-mail.\n\nPlease try again.")
									.SetPositiveButton("Close", (s, e) => { })
									.Show();
						}
					}
					else
					{
						new AlertDialog.Builder(this)
							.SetTitle("Error")
								.SetMessage("Sorry.\n\nAn error occurred that prevented the retrieval of your password.")
								.SetPositiveButton("Close", (s, e) => { })
								.Show();
					}
				}
				catch	(Exception ex)
				{
					//Console.WriteLine("This is the InnerException message: " + ((SoapException) ex).Detail.InnerXml.ToString());
					new AlertDialog.Builder(this)
						.SetTitle ("Error")
							.SetMessage ("Sorry.\n\nAn unexpected error occurred.\n\nError Message: " + ex.Message + "\n\nStack Trace: " + ex.StackTrace + ".\n\nPlease contact " + Resources.GetString(Resource.String.customer_support_email) + ".")
							.SetPositiveButton("Close", (s, e) => { })
							.Show ();
				}
			};
		}
	}
}
