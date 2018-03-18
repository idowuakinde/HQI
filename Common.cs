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
using System.Web.Services.Protocols;
using System.Globalization;
using Android.Content.Res;
using Android.Webkit;
using Android.Views.InputMethods;
using HQI.WSHQIWebService;

namespace HQI
{
	public class Common
	{
		public static string newLine = System.Environment.NewLine;	

		// choose a Web Service location, depending on whether or not we're ready to go live...
		public static string webServiceURL = "http://50.56.227.124/hqiwebservice/default.asmx";
//		public static string webServiceURL = "http://192.168.1.8/hqiwebservice/default.asmx";
		public static bool loggedIn = false;
		public static int patientId;
		public static string patientRef;
		public static string title;
		public static string surname;
		public static string firstName;
		public static DateTime birthDate;
		public static string nationalIdentificationNumber;
		public static string contactEmail;
		public static string contactAddress;
		public static string contactAddressState;
		public static string contactTelephone;
		public static string contactTelephone2;
		public static string contactTelephone3;
		public static string occupation;
		public static string insuranceIdentificationNumber;
		public static string photograph;
		public static string signature;
		public static string password;
		public static string gender;
		public static string remark3;
		public static string remark4;
		public static string remark5;
		public static string remark6;
		public static string remark7;
		public static string remark8;
		public static string remark9;
		// 
		public static string patientName;
		public static string currentLocation;
		// Constants

		private static void GetCurrentLocation()
		{
			try
			{
				Common.currentLocation = "Ikeja";
			}
			catch	(Exception ex)
			{
				throw ex;
			}
		}
		public static void GetPatientDetails(string _patientRef)
		{
			try {
				// set the webservice object and define its header properties...
				HQIWebService _webService = new HQIWebService (Common.webServiceURL);

				// define the parameters, if any, required by this Web Service method
				WS_Patient_GetByPatientRef_Request _patientProfileRequest = new WS_Patient_GetByPatientRef_Request ();
				_patientProfileRequest.patientRef = _patientRef;

				// connect! and grab the response code from hitting the web service...
				WS_Patient_GetByPatientRef_Response _patientProfileResponse = new WS_Patient_GetByPatientRef_Response ();
				_patientProfileResponse = _webService.route_Patient_GetByPatientRef (_patientProfileRequest);
				if (_patientProfileResponse.responseCode > 0) 
				{
                    Common.patientId = _patientProfileResponse.patient.patientId;
					Common.title = _patientProfileResponse.patient.title;
					Common.surname = _patientProfileResponse.patient.surname;
					Common.firstName = _patientProfileResponse.patient.firstName;
					Common.birthDate = _patientProfileResponse.patient.birthDate;
					Common.nationalIdentificationNumber = _patientProfileResponse.patient.nationalIdentificationNumber;
					Common.contactEmail = _patientProfileResponse.patient.contactEmail;
					Common.contactAddress = _patientProfileResponse.patient.contactAddress;
					Common.contactAddressState = _patientProfileResponse.patient.contactAddressState;
					Common.contactTelephone = _patientProfileResponse.patient.contactTelephone;
					Common.contactTelephone2 = _patientProfileResponse.patient.contactTelephone2;
					Common.contactTelephone3 = _patientProfileResponse.patient.contactTelephone3;
					Common.occupation = _patientProfileResponse.patient.occupation;
					Common.insuranceIdentificationNumber = _patientProfileResponse.patient.insuranceIdentificationNumber;
					Common.photograph = _patientProfileResponse.patient.photograph;
					Common.signature = _patientProfileResponse.patient.signature;
					Common.password = _patientProfileResponse.patient.password;
					Common.gender = _patientProfileResponse.patient.gender;
					Common.remark3 = _patientProfileResponse.patient.remark3;
					Common.remark4 = _patientProfileResponse.patient.remark4;
					Common.remark5 = _patientProfileResponse.patient.remark5;
					Common.remark6 = _patientProfileResponse.patient.remark6;
					Common.remark7 = _patientProfileResponse.patient.remark7;
					Common.remark8 = _patientProfileResponse.patient.remark8;
					Common.remark9 = _patientProfileResponse.patient.remark9;
					// get header information (CurrentDrawInfo)
					Common.patientName = (title + " " + firstName + " " + surname);		//.ToUpper();
					// retrieve Player's current location as well
					GetCurrentLocation();
				} else {
					// do nothing
					throw new Exception("Your Profile could not be retrieved at this time. Please try again later.");
				}
			} catch (Exception ex) {
				throw ex;
			}
		}
		public static List<String> GetAllHospitals()
		{
			List<String> ret = new List<String> {"Please choose a hospital {0}"};

			try
			{
				// set the webservice object and define its header properties...
				HQIWebService _webService = new HQIWebService (Common.webServiceURL);

				// define the parameters, if any, required by this Web Service method
				// no input parameter

				// connect! and grab the response code from hitting the web service...
				WS_Hospitals_GetAll_Response _hospitalsResponse = new WS_Hospitals_GetAll_Response();
				_hospitalsResponse = _webService.route_Hospitals_GetAll();
				int hospitalsCount = _hospitalsResponse.responseCode;
				if (hospitalsCount > 0) 
				{
					for (int i = 0; i < hospitalsCount; i++) 
					{
						ret.Add(_hospitalsResponse.hospitals[i].hospitalDescription);
					}
					return ret;
				} 
				else 
				{
					// do nothing
					throw new Exception("The Hospitals Database is currently empty. Please try again at a later time.");
				}
			}
			catch (Exception genEx)
			{
				throw genEx;
			}
		}
		public static void Logout()
		{
			try 
			{
				loggedIn = false;
				Common.patientId = 0;
				Common.patientRef = "";
				Common.title = "";
				Common.surname = "";
				Common.firstName = "";
				Common.birthDate = DateTime.MinValue;
				Common.nationalIdentificationNumber = "";
				Common.contactEmail = "";
				Common.contactAddress = "";
				Common.contactAddressState = "";
				Common.contactTelephone = "";
				Common.contactTelephone2 = "";
				Common.contactTelephone3 = "";
				Common.occupation = "";
				Common.insuranceIdentificationNumber = "";
				Common.photograph = "";
				Common.signature = "";
				Common.password = "";
				Common.gender = "";
				Common.remark3 = "";
				Common.remark4 = "";
				Common.remark5 = "";
				Common.remark6 = "";
				Common.remark7 = "";
				Common.remark8 = "";
				Common.remark9 = "";

				Common.currentLocation = "";
			} 
			catch (Exception ex) 
			{
				throw ex;
			}
		}
		public static string PadNumber(string numToPad, Int32 desiredChars)
		{
			string ret;
			if (numToPad.Length < desiredChars) 
			{
				numToPad = "0" + numToPad;
				numToPad = PadNumber (numToPad, desiredChars);
				ret = numToPad;
			}
			else
			{
				ret = numToPad;
			}

			return ret;
		}
//		public static int PixelsToDp(int pixels)
//		{
//			return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, pixels, Resources.DisplayMetrics);
//		}
		public static string GetFriendlyDate(DateTime _date)
		{
			string ret;
			if (_date == DateTime.MinValue) 
			{
				ret = "N/A (Not available)";
			}
			else
			{
				Int32 _dateMonth = _date.Month;
				string _dateMonthName = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName (_dateMonth);
				Int32 _dateDay = _date.Day;
				string _dateWeekDay = _date.DayOfWeek.ToString ();
				// ret = "Aug 1 (Wednesday)";
				ret = _dateMonthName + " " + PadNumber (_dateDay.ToString(), 2) + " (" + _dateWeekDay + ")";
			}

			return ret;
		}
		public static string GetFriendlyDateShort(DateTime _date)
		{
			string ret;
			if (_date == DateTime.MinValue) 
			{
				ret = "N/A (Not Available)";
			}
			else
			{
				Int32 _dateMonth = _date.Month;
				string _dateMonthName = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName (_dateMonth);
				Int32 _dateDay = _date.Day;
				// ret = "Aug 1";
				ret = _dateMonthName + " " + PadNumber (_dateDay.ToString(), 2);
			}

			return ret;
		}
		public static string GetFriendlyDateWithYear(DateTime _date)
		{
			string ret;
			if (_date == DateTime.MinValue) 
			{
				ret = "N/A (Not Available)";
			}
			else
			{
				Int32 _dateMonth = _date.Month;
				string _dateMonthName = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName (_dateMonth);
				Int32 _dateDay = _date.Day;
				Int32 _dateYear = _date.Year;
				string _dateWeekday = _date.DayOfWeek.ToString ();
				//ret = "Aug 01, 2011 (Wednesday)";
				ret = _dateMonthName + " " + PadNumber (_dateDay.ToString(), 2) + ", " + PadNumber (_dateYear.ToString(), 4) + " (" + _dateWeekday + ")";
			}

			return ret;
		}
		public static string GetFriendlyDateWithYearShort(DateTime _date)
		{
			string ret;
			if (_date == DateTime.MinValue)
			{
				ret = "N/A (not Available)";
			}
			else
			{
				Int32 _dateMonth = _date.Month;
				string _dateMonthName = DateTimeFormatInfo.CurrentInfo.GetAbbreviatedMonthName (_dateMonth);
				Int32 _dateDay = _date.Day;
				Int32 _dateYear = _date.Year;
				//ret = "Aug 01, 2011";
				ret = _dateMonthName + " " + PadNumber (_dateDay.ToString(), 2) + ", " + PadNumber (_dateYear.ToString(), 4);
			}

			return ret;
		}
		public static bool SendEmail(string textBody, string subject, string[] emailAddresses)
		{
			try
			{
				var emailLauncher = new Intent (Android.Content.Intent.ActionSend);
				emailLauncher.PutExtra (Android.Content.Intent.ExtraEmail, emailAddresses);
				emailLauncher.PutExtra (Android.Content.Intent.ExtraSubject, subject);
				emailLauncher.PutExtra (Android.Content.Intent.ExtraText, textBody);

				// tell the Android OS to automatically launch the default e-mail app; and if none has been defined, present the user with options from which to choose one.
				emailLauncher.SetType ("message/rfc822");

				// return a value indicating success to the caller of this function.
				return true;
			}
			catch (Exception) 
			{
				return false;
			}
		}
		//
		public static int GetSpinnerIndex(Spinner mySpinner, String myString)
		{
			int index = 0;

			for (int i = 0; i < mySpinner.Count; i++)
			{
				if (mySpinner.GetItemAtPosition(i).ToString() == myString)
				{
					index = i;
					i = mySpinner.Count;//will stop the loop, kind of break, by making condition false
				}
			}
			return index;
		}
		// 
		public static string GetRatingResponseDescription(string ratingResponse)
		{
			string ret;

			switch (ratingResponse) 
			{
			case "1":
				ret = "Highly Dissatisfied";
				break;
			case "2":
				ret = "Somewhat Dissatisfied";
				break;
			case "3":
				ret = "Neutral - Neither Satisfied Nor Dissatisfied";
				break;
			case "4":
				ret = "Somewhat Satisfied";
				break;
			case "5":
				ret = "Very Satisfied";
				break;
			default:
				ret = "Unknown";
				break;
			}

			return ret;
		}
	}
	//
	public static class ActivityExtensions
	{
		public static void HideSoftKeyboard(this Activity activity)
		{
			new Handler().Post(delegate
			                   {
				var view = activity.CurrentFocus;
				if (view != null)
				{
					InputMethodManager manager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
					manager.HideSoftInputFromWindow(view.WindowToken, 0);
				}
			});
		}

		public static void ShowSoftKeyboard(this Activity activity, View view = null, int delay = 200)
		{
			new Handler().PostDelayed(delegate
			                          {
				view = view ?? activity.CurrentFocus;
				if (view != null)
				{
					if (view.HasFocus)
						view.ClearFocus(); //bug fix for older versions of android

					view.RequestFocus();
					InputMethodManager manager = (InputMethodManager)activity.GetSystemService(Context.InputMethodService);
					manager.ShowSoftInput(view, 0);
				}
			}, delay);
		}
	}
	//
	public class StandardListItem 
	{
		public string Text 
		{ 
			get; 
			set; 
		}
	}
	public class SurveyHistoryItem 
	{
		public string hospitalName 
		{ 
			get; 
			set; 
		}
		public string hospitalRef 
		{
			get; 
			set; 
		}
		public string patientRef 
		{
			get;
			set;
		}
		public string surveyDate 
		{
			get;
			set;
		}
		public DateTime surveyDateRaw 
		{
			get;
			set;
		}
	}
	public class SurveyHistoryDetailItem 
	{
		public int hospitalRatingResponseId
		{
			get;
			set;
		}
		public string hospitalRatingResponseRef
		{
			get;
			set;
		}
		public string hospitalRef
		{
			get;
			set;
		}
		public string patientRef
		{
			get;
			set;
		}
		public string hospitalRatingTitle 
		{ 
			get; 
			set; 
		}
		public string hospitalRatingResponse 
		{
			get; 
			set; 
		}
		public string remark1
		{
			get;
			set;
		}
		public DateTime remark1ConvertedToDate
		{
			get
			{
				return DateTime.Parse (remark1);
			}
		}
		public string remark2
		{
			get;
			set;
		}
		public string remark3
		{
			get;
			set;
		}
	}
//	public class NewSurveyDetailItem 
//	{
//		public string hospitalRef
//		{
//			get;
//			set;
//		}
//		public string patientRef
//		{
//			get;
//			set;
//		}
//		public string hospitalRatingTitle 
//		{ 
//			get; 
//			set; 
//		}
//		public string hospitalRatingResponse 
//		{
//			get; 
//			set; 
//		}
//	}
	public class SurveyQuestionItem 
	{
		public int hospitalRatingId
		{
			get;
			set;
		}
		public string hospitalRatingGroup
		{
			get;
			set;
		}
		public string hospitalRatingDepartment
		{
			get;
			set;
		}
		public string hospitalRatingTitle 
		{ 
			get; 
			set; 
		}
		public string remark1
		{
			get;
			set;
		}
		public string remark2
		{
			get;
			set;
		}
		public string remark3
		{
			get;
			set;
		}
	}
	public class HQIWebViewClient : WebViewClient
	{
		public override bool ShouldOverrideUrlLoading (WebView view, string url)
		{
			view.LoadUrl (url);
			return true;
		}
	}
}
