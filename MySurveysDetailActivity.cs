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
	public class MySurveysDetailActivity : Activity
	{
		List<SurveyHistoryDetailItem> surveyHistoryDetailItems = new List<SurveyHistoryDetailItem>();
		ListView lstSurveyHistoryDetailItems;
		HospitalRatingResponse[] _responses;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			try
			{
				RequestWindowFeature(WindowFeatures.CustomTitle);
				SetContentView (Resource.Layout.MySurveysDetailLayout);
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

				// retrieve the values that were passed to this Activity from preceding Activity
				string selectedHospitalRef = Intent.GetStringExtra("selectedHospitalRef");
				string selectedHospitalName = Intent.GetStringExtra("selectedHospitalName");
				string selectedPatientRef = Intent.GetStringExtra("selectedPatientRef");
				string selectedDateTimeStamp = Intent.GetStringExtra("selectedDateTimeStamp");
				DateTime selectedDateTimeStampRaw = DateTime.Parse(Intent.GetStringExtra("selectedDateTimeStampRaw"));

				FindViewById<TextView> (Resource.Id.survey_title).Text = selectedHospitalName.ToUpper() + ".\nYou assessed the above hospital on " + selectedDateTimeStamp.Substring(3) + ".\nPossible scores range from {1: Highly Dissatisfied} to {5: Very Satisfied}.";

				lstSurveyHistoryDetailItems = FindViewById<ListView>(Resource.Id.List5); // get reference to the ListView in the layout

				// populate the listview with data
				// set the webservice object and define its header properties...
				HQIWebService _webService = new HQIWebService(Common.webServiceURL);

				// define the parameters, if any, required by this Web Service method
				WS_HospitalRatingResponses_GetFromSingleHospital_Request _responsesRequest = new WS_HospitalRatingResponses_GetFromSingleHospital_Request();
				_responsesRequest.hospitalRef = selectedHospitalRef;
				_responsesRequest.patientRef = selectedPatientRef;
				_responsesRequest.dateTimeStamp = selectedDateTimeStampRaw;

				// connect! and grab the response code from hitting the web service...
				WS_HospitalRatingResponses_GetFromSingleHospital_Response _responsesResponse = new WS_HospitalRatingResponses_GetFromSingleHospital_Response();
				_responsesResponse = _webService.route_HospitalRatingResponses_GetFromSingleHospital(_responsesRequest);
				if (_responsesResponse.responseCode > 0)
				{
					//communicate success to the user
					_responses = _responsesResponse.hospitalRatingResponses;
					int batchCount = _responses.Length;
					if (batchCount > 0)
					{
						int desiredHeight = (200 * batchCount);
						lstSurveyHistoryDetailItems.LayoutParameters.Height = desiredHeight;

						for (int i = 0; i < batchCount; i++) 
						{
							surveyHistoryDetailItems.Add(new SurveyHistoryDetailItem() {hospitalRatingResponseId = _responses[i].hospitalRatingResponseId, hospitalRatingResponseRef = _responses[i].hospitalRatingResponseRef, hospitalRef = _responses[i].hospitalRef, patientRef = _responses[i].patientRef, hospitalRatingTitle = _responses[i].hospitalRatingTitle, hospitalRatingResponse = _responses[i].hospitalRatingResponse, remark1 = _responses[i].remark1, remark2 = _responses[i].remark2, remark3 = _responses[i].remark3});
						}
						surveyHistoryDetailItems = surveyHistoryDetailItems.OrderByDescending (a => a.remark1ConvertedToDate).ToList();
						lstSurveyHistoryDetailItems.Adapter = new SurveyHistoryDetailAdapter(this, surveyHistoryDetailItems);
						//						lstSurveyHistoryDetailItems.ItemClick += OnListItemClick;
						// Sometimes, the parent ScrollView object interferes with the scrollability of child ListViews.
						//						lstsurveyHistoryDetailItems.Touch += ListViewOnTouch;
					}
				}
				else
				{
					new AlertDialog.Builder(this)
						.SetTitle("Info")
							.SetMessage("Your Survey History is currently empty.\n\nPlease go to 'New Survey' to get started.")
							.SetPositiveButton("Close", (s, e) => { })
							.Show();
				}
			}
			catch (Exception Ex) 
			{
				//Console.WriteLine("This is the InnerException message: " + ((SoapException) ex).Detail.InnerXml.ToString());
				new AlertDialog.Builder(this)
					.SetTitle ("Error")
						.SetMessage ("Sorry.\n\nAn unexpected error occurred.\n\nError Message: " + Ex.Message + "\n\nStack Trace: " + Ex.StackTrace + ".\n\nPlease contact " + Resources.GetString(Resource.String.customer_support_email) + ".")
						.SetPositiveButton("Close", (s, e) => { })
						.Show ();
			}
		}

		protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
		{
			//var moveToSurveyReviewScreen = new Intent (this, typeof(MainActivity));
			//moveToSurveyReviewScreen.PutExtra( ("selectedSurveyDetails", _responseBatches[e.Position]); //[e.Position]);
			Android.Widget.Toast.MakeText(this, "You are viewing your previous Survey with Id {" + (e.Position + 1) + "}", ToastLength.Short).Show();
			// StartActivity (moveToSurveyReviewScreen);
		}

		private void ListViewOnTouch(object sender, View.TouchEventArgs e)
		{
			View view = (View)sender;
			view.Parent.RequestDisallowInterceptTouchEvent (true);
			e.Handled = false;
		}
	}
}
