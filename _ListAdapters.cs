using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace HQI 
{
	public class StandardListAdapter : BaseAdapter<StandardListItem> 
	{
		List<StandardListItem> items;
		Activity context;
		public StandardListAdapter(Activity context, List<StandardListItem> items)
			: base()
		{
			this.context = context;
			this.items = items;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override StandardListItem this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];

			View view = convertView;
			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout._ListView_StandardFont, null);
			view.FindViewById<TextView>(Resource.Id.Text1).Text = item.Text;

			return view;
		}
	}

	public class SurveyHistoryAdapter : BaseAdapter<SurveyHistoryItem> 
	{
		List<SurveyHistoryItem> items;
		Activity context;
		public SurveyHistoryAdapter(Activity context, List<SurveyHistoryItem> items)
			: base()
		{
			this.context = context;
			this.items = items;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override SurveyHistoryItem this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];

			View view = convertView;
			if (view == null) // no view to re-use, create new
			view = context.LayoutInflater.Inflate(Resource.Layout._ListView_SurveyHistory, null);
			view.FindViewById<TextView> (Resource.Id.txtSurveyNumber).Text = "Survey #:\t\t\t\t" + (position + 1);
			view.FindViewById<TextView> (Resource.Id.txtHospitalRef).Text = "Hospital Ref:\t\t" + item.hospitalRef.Substring(0,23) + "...";
			view.FindViewById<TextView>(Resource.Id.txtHospitalName).Text = "Hospital Name:\t" + item.hospitalName;
			view.FindViewById<TextView>(Resource.Id.txtSurveyDate).Text = "Date of Survey:\t" + item.surveyDate.Substring(3);
			return view;
		}
	}
	
	public class SurveyHistoryDetailAdapter : BaseAdapter<SurveyHistoryDetailItem> 
	{
		List<SurveyHistoryDetailItem> items;
		Activity context;
		public SurveyHistoryDetailAdapter(Activity context, List<SurveyHistoryDetailItem> items)
			: base()
		{
			this.context = context;
			this.items = items;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override SurveyHistoryDetailItem this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];

			View view = convertView;
			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout._ListView_SurveyHistoryDetail, null);
			view.FindViewById<TextView> (Resource.Id.txtSerial).Text = (position + 1).ToString();	// + ".";
			view.FindViewById<TextView> (Resource.Id.txtRatingText).Text = "Our Question:\n" + item.hospitalRatingTitle + "\n";
			view.FindViewById<TextView> (Resource.Id.txtRatingResponseText).Text = "Your Response:\t" + item.hospitalRatingResponse + " {" + Common.GetRatingResponseDescription(item.hospitalRatingResponse) + "}";
			return view;
		}
	}
	
	public class NewSurveyDetailAdapter : BaseAdapter<SurveyQuestionItem> 
	{
		List<SurveyQuestionItem> items;
		Activity context;
		public NewSurveyDetailAdapter(Activity context, List<SurveyQuestionItem> items)
			: base()
		{
			this.context = context;
			this.items = items;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override SurveyQuestionItem this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];

			View view = convertView;
			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout._ListView_NewSurveyDetail, null);
			view.FindViewById<TextView> (Resource.Id.txtSerial).Text = (position + 1).ToString();
			view.FindViewById<TextView> (Resource.Id.txtRatingText).Text = "Our Question:\n" + item.hospitalRatingTitle + "\n";
			view.FindViewById<TextView> (Resource.Id.txtRatingResponseText).Text = "Your Response:";
			view.FindViewById<RadioButton> (Resource.Id.r_rdoResponse1).Checked = true;
			return view;
		}
	}
}