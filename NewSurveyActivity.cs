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
	public class NewSurveyActivity : Activity
	{
		List<SurveyQuestionItem> newSurveyDetailItems = new List<SurveyQuestionItem>();
		ListView lstNewSurveyDetailItems;
		HospitalRating[] _ratings;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			try
			{
				RequestWindowFeature(WindowFeatures.CustomTitle);
				SetContentView (Resource.Layout.NewSurveyLayout);
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

				// populate the Hospitals spinner
				Spinner spinnerHospitals = FindViewById<Spinner> (Resource.Id.spinnerHospitals);
				List<String> hospitals = Common.GetAllHospitals();
				ArrayAdapter<String> hospitalsAdapter = new ArrayAdapter<String> (this, Android.Resource.Layout.SimpleSpinnerItem, hospitals);
				hospitalsAdapter.SetDropDownViewResource (Android.Resource.Layout.SimpleSpinnerDropDownItem);
				spinnerHospitals.Adapter = hospitalsAdapter;

				lstNewSurveyDetailItems = FindViewById<ListView>(Resource.Id.List5); // get reference to the ListView in the layout

				// populate the listview with data
				// set the webservice object and define its header properties...
				HQIWebService _webService = new HQIWebService(Common.webServiceURL);

				// define the parameters, if any, required by this Web Service method
				// no input parameter

				// connect! and grab the response code from hitting the web service...
				WS_HospitalRatings_GetAll_Response _ratingsResponse = new WS_HospitalRatings_GetAll_Response();
				_ratingsResponse = _webService.route_HospitalRatings_GetAll();
				if (_ratingsResponse.responseCode > 0)
				{
					// communicate success to the user
					_ratings = _ratingsResponse.hospitalRatings;
					int batchCount = _ratings.Length;
					if (batchCount > 0)
					{
						int desiredHeight = (280 * batchCount);
						lstNewSurveyDetailItems.LayoutParameters.Height = desiredHeight;

						for (int i = 0; i < batchCount; i++) 
						{
							newSurveyDetailItems.Add(new SurveyQuestionItem() {hospitalRatingId = _ratings[i].hospitalRatingId, hospitalRatingGroup = _ratings[i].hospitalRatingGroup, hospitalRatingDepartment = _ratings[i].hospitalRatingDepartment, hospitalRatingTitle = _ratings[i].hospitalRatingTitle, remark1 = _ratings[i].remark1, remark2 = _ratings[i].remark2, remark3 = _ratings[i].remark3});
						}
						newSurveyDetailItems = newSurveyDetailItems.OrderBy (a => a.hospitalRatingId).ToList();
						lstNewSurveyDetailItems.Adapter = new NewSurveyDetailAdapter(this, newSurveyDetailItems);
						// lstSurveyHistoryDetailItems.ItemClick += OnListItemClick;
						// Sometimes, the parent ScrollView object interferes with the scrollability of child ListViews.
						// lstsurveyHistoryDetailItems.Touch += ListViewOnTouch;
					}
				}
				else
				{
					new AlertDialog.Builder(this)
						.SetTitle("Info")
							.SetMessage("The Survey Questions Database is currently empty.\n\nPlease try again at a later time.")
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