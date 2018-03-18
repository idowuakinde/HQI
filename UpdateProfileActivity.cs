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
	public class UpdateProfileActivity : Activity
	{
		// Create variables to hold user-supplied values
		string email;
		string title;
		string surname;
		string firstName;
		DateTime dateOfBirth;
		string addressLine1;
		string addressState;
		string telephone1;
		string telephone2;
		string telephone3;
		string occupation;
		string gender;
		string insuranceIdentificationNumber;
		string nationalIdentificationNumber;
		string photograph;
		string signature;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Create your application here
			RequestWindowFeature(WindowFeatures.CustomTitle);
			SetContentView (Resource.Layout.UpdateProfileLayout);
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

			// Set spinner properties (Titles)
			Spinner spinnerTitles = FindViewById<Spinner> (Resource.Id.r_spinnerTitle);
			var adapterTitles = ArrayAdapter.CreateFromResource (this, Resource.Array.registration_title_array, Android.Resource.Layout.SimpleSpinnerItem);
			adapterTitles.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinnerTitles.Adapter = adapterTitles;
			// Set spinner properties (States)
			Spinner spinnerStates = FindViewById<Spinner> (Resource.Id.r_spinnerState);
			var adapterStates = ArrayAdapter.CreateFromResource (this, Resource.Array.registration_states_array, Android.Resource.Layout.SimpleSpinnerItem);
			adapterStates.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinnerStates.Adapter = adapterStates;

			// set spinner callback
			spinnerTitles.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinnerTitles_ItemSelected);
			spinnerStates.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinnerStates_ItemSelected);

			try 
			{
				// set the webservice object and define its header properties...
				HQIWebService _webService = new HQIWebService (Common.webServiceURL);

				// define the parameters, if any, required by this Web Service method
				WS_Patient_GetByPatientRef_Request _patientProfileRequest = new WS_Patient_GetByPatientRef_Request();
				_patientProfileRequest.patientRef = Common.patientRef;

				// connect! and grab the response code from hitting the web service...
				WS_Patient_GetByPatientRef_Response _patientProfileResponse = new WS_Patient_GetByPatientRef_Response();
				_patientProfileResponse = _webService.route_Patient_GetByPatientRef(_patientProfileRequest);
				if (_patientProfileResponse.responseCode > 0) 
				{
					// communicate success to the user
					FindViewById<TextView> (Resource.Id.upd_Email).Text = _patientProfileResponse.patient.contactEmail; email = _patientProfileResponse.patient.contactEmail;
					spinnerTitles.SetSelection(Common.GetSpinnerIndex(spinnerTitles, _patientProfileResponse.patient.title)); title = _patientProfileResponse.patient.title;
					FindViewById<EditText> (Resource.Id.upd_LastName).Text = _patientProfileResponse.patient.surname; surname = _patientProfileResponse.patient.surname;
					FindViewById<EditText> (Resource.Id.upd_FirstName).Text = _patientProfileResponse.patient.firstName; firstName = _patientProfileResponse.patient.firstName;
					FindViewById<EditText> (Resource.Id.upd_DateOfBirth).Text = _patientProfileResponse.patient.birthDate.ToShortDateString(); dateOfBirth = _patientProfileResponse.patient.birthDate;
					FindViewById<EditText> (Resource.Id.upd_AddressLine1).Text = _patientProfileResponse.patient.contactAddress; addressLine1 = _patientProfileResponse.patient.contactAddress;
					spinnerStates.SetSelection(Common.GetSpinnerIndex(spinnerStates, _patientProfileResponse.patient.contactAddressState)); addressState = _patientProfileResponse.patient.contactAddressState;
					FindViewById<EditText> (Resource.Id.upd_Telephone1).Text = _patientProfileResponse.patient.contactTelephone; telephone1 = _patientProfileResponse.patient.contactTelephone;
					FindViewById<EditText> (Resource.Id.upd_Telephone2).Text = _patientProfileResponse.patient.contactTelephone2; telephone2 = _patientProfileResponse.patient.contactTelephone2;
					FindViewById<EditText> (Resource.Id.upd_Telephone3).Text = _patientProfileResponse.patient.contactTelephone3; telephone3 = _patientProfileResponse.patient.contactTelephone3;
					FindViewById<EditText> (Resource.Id.upd_Occupation).Text = _patientProfileResponse.patient.occupation; occupation = _patientProfileResponse.patient.occupation;
					gender = _patientProfileResponse.patient.gender;
					insuranceIdentificationNumber = _patientProfileResponse.patient.insuranceIdentificationNumber;
					nationalIdentificationNumber = _patientProfileResponse.patient.nationalIdentificationNumber;
					photograph = _patientProfileResponse.patient.photograph;
					signature = _patientProfileResponse.patient.signature;
				} else {
					new AlertDialog.Builder(this)
						.SetTitle ("Error")
							.SetMessage ("Sorry.\n\nAn error occurred and your Profile could not be loaded at this time.")
							.SetPositiveButton("Close", (s, e) => { })
							.Show ();
				}
			} 
			catch (Exception ex) {
				//Console.WriteLine("This is the InnerException message: " + ((SoapException) ex).Detail.InnerXml.ToString());
				new AlertDialog.Builder(this)
					.SetTitle ("Error")
						.SetMessage ("Sorry.\n\nAn unexpected error occurred.\n\nError Message: " + ex.Message + "\n\nStack Trace: " + ex.StackTrace + ".\n\nPlease contact " + Resources.GetString(Resource.String.customer_support_email) + ".")
						.SetPositiveButton("Close", (s, e) => { })
						.Show ();
			}

			Button btnCancel = FindViewById<Button> (Resource.Id.upd_btnCancel);
			btnCancel.Click += delegate {
				this.Finish();
			};

			// ALL of the above was to retrieve the Player Profile on screen load. This block saves changes made to the player's profile details.
			Button btnSaveProfile = FindViewById<Button> (Resource.Id.upd_btnSaveProfile);
			btnSaveProfile.Click += delegate {
				// grab user-supplied values
				email = FindViewById<TextView> (Resource.Id.upd_Email).Text.Trim();
				title = spinnerTitles.SelectedItem.ToString();
				surname = FindViewById<EditText> (Resource.Id.upd_LastName).Text.Trim();
				firstName = FindViewById<EditText> (Resource.Id.upd_FirstName).Text.Trim();
				dateOfBirth = Convert.ToDateTime(FindViewById<EditText> (Resource.Id.upd_DateOfBirth).Text);
				addressLine1 = FindViewById<EditText> (Resource.Id.upd_AddressLine1).Text.Trim();
				addressState = spinnerStates.SelectedItem.ToString();
				telephone1 = FindViewById<EditText> (Resource.Id.upd_Telephone1).Text.Trim();
				telephone2 = FindViewById<EditText> (Resource.Id.upd_Telephone2).Text.Trim();
				telephone3 = FindViewById<EditText> (Resource.Id.upd_Telephone3).Text.Trim();
				occupation = FindViewById<EditText> (Resource.Id.upd_Occupation).Text.Trim();

				try
				{
					// set the webservice object and define its header properties...
					HQIWebService _webService = new HQIWebService(Common.webServiceURL);

					// define the parameters, if any, required by this Web Service method
					WS_Patient_Update_Request _editPlayerRequest = new WS_Patient_Update_Request();
					_editPlayerRequest.patientRef = Common.patientRef;
					_editPlayerRequest.password = Common.password;
					_editPlayerRequest.contactEmail = email;
					_editPlayerRequest.title = title;
					_editPlayerRequest.surname = surname;
					_editPlayerRequest.firstName = firstName;
					_editPlayerRequest.birthDate = dateOfBirth;
					_editPlayerRequest.contactAddress = addressLine1;
					_editPlayerRequest.contactAddressState = addressState;
					_editPlayerRequest.contactTelephone = telephone1;
					_editPlayerRequest.contactTelephone2 = telephone2;
					_editPlayerRequest.contactTelephone3 = telephone3;
					_editPlayerRequest.occupation = occupation;
					_editPlayerRequest.gender = gender;
					_editPlayerRequest.insuranceIdentificationNumber = insuranceIdentificationNumber;
					_editPlayerRequest.nationalIdentificationNumber = nationalIdentificationNumber;
					_editPlayerRequest.photograph = photograph;
					_editPlayerRequest.signature = signature;

					// connect! and grab the response code from hitting the web service...
					WS_Patient_Update_Response _editPlayerResponse  = new WS_Patient_Update_Response();
					_editPlayerResponse = _webService.route_Patient_Update(_editPlayerRequest);
					if (_editPlayerResponse.responseCode > 0)
					{
						new AlertDialog.Builder(this)
							.SetTitle("Success")
								.SetMessage("Congrats.\n\nProfile changes were successfully saved.")
								.SetPositiveButton("Close", (s, e) => { })
								.Show();
					}
					else
					{
						new AlertDialog.Builder(this)
							.SetTitle("Error")
								.SetMessage("Sorry.\n\nAn error occurred and the changes you requested could not be saved to your profile.")
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

		private void spinnerTitles_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinnerTitles = (Spinner)sender;
			string selectedValue = spinnerTitles.GetItemAtPosition (e.Position).ToString ();
		}

		private void spinnerStates_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinnerStates = (Spinner)sender;
			string selectedValue = spinnerStates.GetItemAtPosition (e.Position).ToString ();
		}
	}
}
