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
	public class MySurveysActivity : Activity
	{
		List<SurveyHistoryItem> surveyHistoryItems = new List<SurveyHistoryItem>();
		ListView lstSurveyHistoryItems;
		HospitalRatingResponseBatch[] _responseBatches;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			try
			{
				RequestWindowFeature(WindowFeatures.CustomTitle);
				SetContentView (Resource.Layout.MySurveysLayout);
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

				lstSurveyHistoryItems = FindViewById<ListView>(Resource.Id.List5); // get reference to the ListView in the layout

				// populate the listview with data
				// set the webservice object and define its header properties...
				HQIWebService _webService = new HQIWebService(Common.webServiceURL);

				// define the parameters, if any, required by this Web Service method
				WS_HospitalRatingResponseBatches_GetFromSinglePatient_Request _responseBatchesRequest = new WS_HospitalRatingResponseBatches_GetFromSinglePatient_Request();
				_responseBatchesRequest.patientRef = Common.patientRef;

				// connect! and grab the response code from hitting the web service...
				WS_HospitalRatingResponseBatches_GetFromSinglePatient_Response _responseBatchesResponse = new WS_HospitalRatingResponseBatches_GetFromSinglePatient_Response();
				_responseBatchesResponse = _webService.route_HospitalRatingResponseBatches_GetFromSinglePatient(_responseBatchesRequest);
				if (_responseBatchesResponse.responseCode > 0)
				{
					//communicate success to the user
					_responseBatches = _responseBatchesResponse.hospitalRatingResponseBatches;
					int batchCount = _responseBatches.Length;
					if (batchCount > 0)
					{
						int desiredHeight = (200 * batchCount);
						lstSurveyHistoryItems.LayoutParameters.Height = desiredHeight;

						for (int i = 0; i < batchCount; i++) 
						{
							surveyHistoryItems.Add(new SurveyHistoryItem() { hospitalRef = _responseBatches[i].hospitalRef, hospitalName = _responseBatches[i].hospitalName, patientRef = _responseBatches[i].patientRef, surveyDate = _responseBatches[i].dateTimeStamp, surveyDateRaw = _responseBatches[i].dateTimeStampRaw });
						}
						surveyHistoryItems = surveyHistoryItems.OrderByDescending (a => a.surveyDateRaw).ToList();
						lstSurveyHistoryItems.Adapter = new SurveyHistoryAdapter(this, surveyHistoryItems);
						lstSurveyHistoryItems.ItemClick += OnListItemClick;
						// Sometimes, the parent ScrollView object interferes with the scrollability of child ListViews.
//						lstSurveyHistoryItems.Touch += ListViewOnTouch;
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
			//Android.Widget.Toast.MakeText(this, "You are viewing your previous Survey with Id {" + (e.Position + 1) + "}", ToastLength.Short).Show();
			var moveToSurveyReviewScreen = new Intent (this, typeof(MySurveysDetailActivity));
			moveToSurveyReviewScreen.PutExtra("selectedHospitalRef", _responseBatches[e.Position].hospitalRef);
			moveToSurveyReviewScreen.PutExtra("selectedHospitalName", _responseBatches[e.Position].hospitalName);
			moveToSurveyReviewScreen.PutExtra("selectedPatientRef", _responseBatches[e.Position].patientRef);
			moveToSurveyReviewScreen.PutExtra("selectedDateTimeStamp", _responseBatches[e.Position].dateTimeStamp);
			moveToSurveyReviewScreen.PutExtra("selectedDateTimeStampRaw", _responseBatches[e.Position].dateTimeStampRaw.ToString());
			StartActivity (moveToSurveyReviewScreen);
		}

		private void ListViewOnTouch(object sender, View.TouchEventArgs e)
		{
			View view = (View)sender;
			view.Parent.RequestDisallowInterceptTouchEvent (true);
			e.Handled = false;
		}
	}
}
