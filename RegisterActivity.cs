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
	[Activity]
	public class RegisterActivity : Activity
	{
		// Create variables to hold user-supplied values
		//string patientRef="";
		string password;
		string title;
		string lastName;
		string firstName;
		string gender = "Female";
		DateTime dateOfBirth;
		string nationalIdentificationNumber;
		string contactEmail;
		string contactAddress;
		string contactAddressState;
		string contactAddressCountry;
		string contactTelephone;
		string contactTelephone2;
		string contactTelephone3;
		string occupation;
		string insuranceIdentificationNumber;
		string photograph="";
		string signature="";
		bool agreedToTerms;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			RequestWindowFeature(WindowFeatures.CustomTitle);
			SetContentView (Resource.Layout.Register);
			Window.SetFeatureInt (WindowFeatures.CustomTitle, Resource.Layout._Custom_Title);

			// Set spinner properties
			Spinner spinnerTitle = FindViewById<Spinner> (Resource.Id.r_spinnerTitle);
			var questionsAdapter = ArrayAdapter.CreateFromResource (this, Resource.Array.registration_title_array, Android.Resource.Layout.SimpleSpinnerItem);
			questionsAdapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinnerTitle.Adapter = questionsAdapter;
			Spinner spinnerStates = FindViewById<Spinner> (Resource.Id.r_spinnerState);
			var statesAdapter = ArrayAdapter.CreateFromResource (this, Resource.Array.registration_states_array, Android.Resource.Layout.SimpleSpinnerItem);
			statesAdapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
			spinnerStates.Adapter = statesAdapter;

			// set spinner callback
			spinnerTitle.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinnerTitle_ItemSelected);
			spinnerStates.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs> (spinnerStates_ItemSelected);

			// Set radioGroup callback
			RadioButton rdoFemale = FindViewById<RadioButton>(Resource.Id.r_rdoGenderF); rdoFemale.Click += RadioButtonClick;
			RadioButton rdoMale = FindViewById<RadioButton>(Resource.Id.r_rdoGenderM); rdoMale.Click += RadioButtonClick;

			CheckBox chkAgreeTerms = FindViewById<CheckBox> (Resource.Id.chkAgreeTerms);
			string legalText = "By checking this box, I certify that I have read, and agree to be bound by, the Terms and Conditions and Privacy Policy of Aesulapius HealthCare Consultants." + Common.newLine + Common.newLine;
			legalText += "I also certify that I am at least 18 years of age.";
			chkAgreeTerms.Text = legalText;

			// Get the Register button from the layout resource, and attach an event to it
			Button btnNextStageOfRegistration = FindViewById<Button> (Resource.Id.r_btnNext);
			btnNextStageOfRegistration.Click += delegate {

				// grab user-supplied values
				// patientRef wil be generated at run-time and supplied server-side
				password = FindViewById<EditText> (Resource.Id.r_editTextPassword1).Text.Trim();
				// title ==> Whenever this selection is changed in the UI, a SelectedItem event is fired for the Spinner object...
				lastName = FindViewById<EditText> (Resource.Id.r_editTextSurname).Text.Trim();
				firstName = FindViewById<EditText> (Resource.Id.r_editTextFirstName).Text.Trim();
				// gender ==> Whenever this selection is changed in the UI, a Click event is fired for the RadioGroup object...
				dateOfBirth = FindViewById<DatePicker> (Resource.Id.r_datePickerDateOfBirth).DateTime;
				nationalIdentificationNumber = "";	// FindViewById<EditText> (Resource.Id.r_editTextNationalIdentificationNumber).Text.Trim();
				contactEmail = FindViewById<EditText> (Resource.Id.r_editTextEmail).Text.Trim();
				contactAddress = FindViewById<EditText> (Resource.Id.r_editTextAddressLine1).Text.Trim();
				// contactAddressState ==> Whenever this selection is changed in the UI, a SelectedItem event is fired for the Spinner object...
				contactAddressCountry = FindViewById<EditText> (Resource.Id.r_editTextCountry).Text.Trim();
				contactTelephone = FindViewById<EditText> (Resource.Id.r_editTextTelephone).Text.Trim();
				contactTelephone2 = FindViewById<EditText> (Resource.Id.r_editTextTelephone2).Text.Trim();
				contactTelephone3 = FindViewById<EditText> (Resource.Id.r_editTextTelephone3).Text.Trim();
				occupation = FindViewById<EditText> (Resource.Id.r_editTextOccupation).Text.Trim();
				insuranceIdentificationNumber = "";	// FindViewById<EditText> (Resource.Id.r_editTextInsuranceIdentificationNumber).Text.Trim();
				// photograph ==> unused, for now
				// signature ==> unused, for now
				agreedToTerms = FindViewById<CheckBox> (Resource.Id.chkAgreeTerms).Checked;

				try
				{
//					new AlertDialog.Builder(this)
//						.SetTitle("Info")
//							.SetMessage("Gender|State|Security is currently: "+ gender + "|" + state + "|" + securityQuestion.ToString())
//							.SetPositiveButton("Close", (s, e) => { })
//							.Show();
					if (agreedToTerms)
					{
						// set the webservice object and define its header properties...
						HQIWebService _webService = new HQIWebService(Common.webServiceURL);

						// define the player registration parameters
						WS_Patient_AddNew_Request _registerPatientRequest = new WS_Patient_AddNew_Request();
						_registerPatientRequest.password = password;
						_registerPatientRequest.title = title;
						_registerPatientRequest.surname = lastName;
						_registerPatientRequest.firstName = firstName;
						_registerPatientRequest.gender = gender;
						_registerPatientRequest.birthDate = dateOfBirth;
						_registerPatientRequest.nationalIdentificationNumber = nationalIdentificationNumber;
						_registerPatientRequest.contactEmail = contactEmail;
						_registerPatientRequest.contactAddress = contactAddress;
						_registerPatientRequest.contactAddressState = contactAddressState;
						_registerPatientRequest.contactTelephone = contactTelephone;
						_registerPatientRequest.contactTelephone2 = contactTelephone2;
						_registerPatientRequest.contactTelephone3 = contactTelephone3;
						_registerPatientRequest.occupation = occupation;
						_registerPatientRequest.insuranceIdentificationNumber = insuranceIdentificationNumber;
						_registerPatientRequest.photograph = photograph;
						_registerPatientRequest.signature = signature;

						// connect! and grab the response code from hitting the web service...
						WS_Patient_AddNew_Response _registerPatientResponse = new WS_Patient_AddNew_Response();
						_registerPatientResponse = _webService.route_Patient_AddNew(_registerPatientRequest);
						if (_registerPatientResponse.responseCode > 0)
						{
							new AlertDialog.Builder(this)
								.SetTitle("Success")
									.SetMessage("Congrats.\n\nRegistration was successful.")
									.SetPositiveButton("Close", (s, e) => {
										var dataToTransfer = new Intent(this, typeof(MainActivity));
										dataToTransfer.PutExtra("go", "Login");
										StartActivity (dataToTransfer);
									})
									.Show();
						}
						else
						{
							new AlertDialog.Builder(this)
								.SetTitle("Error")
									.SetMessage("Sorry.\n\nRegistration was NOT successful.\n\nUnexpected Response Code received.")
									.SetPositiveButton("Close", (s, e) => { })
									.Show();
						}
					}
					else
					{
						// User did not click checkbox to agree to terms of registration
						new AlertDialog.Builder(this)
							.SetTitle("Info")
								.SetMessage("You must check the box above to tell us that you have agreed to the Terms and Conditions and Privacy Policy governing the HQI Service." + Common.newLine + Common.newLine + "We cannot process your registration unless you agree." + Common.newLine + Common.newLine + "Please check the box and try again.")
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
		private void spinnerTitle_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinnerTitle = (Spinner)sender;
			string selectedValue = spinnerTitle.GetItemAtPosition (e.Position).ToString ();
			title = selectedValue;
		}
		private void spinnerStates_ItemSelected (object sender, AdapterView.ItemSelectedEventArgs e)
		{
			Spinner spinnerStates = (Spinner)sender;
			string selectedValue = spinnerStates.GetItemAtPosition (e.Position).ToString ();
			contactAddressState = selectedValue;
		}
		private void RadioButtonClick (object sender, EventArgs e)
		{
			RadioButton rb = (RadioButton)sender;
			string selectedValue = rb.Text;
			gender = selectedValue;
		} 
	}
}
