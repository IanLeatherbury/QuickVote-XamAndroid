using System;
using Android.App;
using Android.Views;
using Android.OS;
using Android.Widget;
using Android.Support.V4.App;
using System.Collections.Generic;
using QuickVoteNative.Models;
using Microsoft.WindowsAzure.MobileServices;
using QuickVoteNative.Activities;
using QuickVoteNative.Adapters;
using QuickVoteNative.Imageloaders;
using Android.Content;
using QuickVoteNative.ViewModels;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace QuickVoteNative.Fragments
{
	public class CommunityFragment : Android.Support.V4.App.Fragment
	{
		View view;
		string imgUrl;

		ImageLoader imageLoader;
		ImageLoaderAdapter imageLoaderAdapter;

		public static MobileServiceClient Client = new MobileServiceClient (
			                                                 Constants.Url, Constants.Key);

		public string ImgUrl {
			get {
				return this.imgUrl;
			}
			set {
				imgUrl = value;
			}
		}

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			view = inflater.Inflate (Resource.Layout.fragment_community, null);

			return view;
		}

		public override void OnActivityCreated (Bundle savedInstanceState)
		{
			base.OnActivityCreated (savedInstanceState);

			var communityListView = view.FindViewById<ListView> (Resource.Id.communityListView);
			communityListView.ItemClick += OnListItemClick;

			imageLoader = new ImageLoader (Activity, 64, 40);

			imageLoaderAdapter = new ImageLoaderAdapter (this.Activity, imageLoader) {
				Parent = communityListView,
			};

			communityListView.Adapter = imageLoaderAdapter;

			communityListView.EmptyView = view.FindViewById<TextView> (Resource.Id.emptyView);

			GetMostRecentPublicPolls ();
		}

		public void OnListItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			//get Poll data
			PollViewModel obj = (PollViewModel)this.imageLoaderAdapter.GetItem (e.Position);

			//start a new vote activity
			var voteActivity = new Intent (Activity, typeof(VoteActivity));

			voteActivity.PutExtra ("pollTitle", obj.Title); 
			voteActivity.PutExtra ("userName", obj.UserName); 

			voteActivity.PutExtra ("optionOne", obj.OptionOne); 
			voteActivity.PutExtra ("optionTwo", obj.OptionTwo);
			voteActivity.PutExtra ("optionThree", obj.OptionThree);
			voteActivity.PutExtra ("optionFour", obj.OptionFour);

			voteActivity.PutExtra ("pollId", obj.PollId);

			StartActivity (voteActivity);
		}

		public async void GetMostRecentPublicPolls ()
		{
			//Get most recent public polls, and sets them as the image loader's List<PollViewModel>
			//add akavache to get stored data

			try {
				imageLoaderAdapter.Polls = await Util.GetPolls ();
			} catch (Exception exc) {
				Console.WriteLine ("Azure error pulling polls" + exc);
			}
		}

		// Immediately return a cached version of an object if available, but *always*
		// also execute fetchFunc to retrieve the latest version of an object.
		IObservable<T> GetAndFetchLatest<T> (string key,
		                                    Func<Task<T>> fetchFunc,
		                                    Func<DateTimeOffset, bool> fetchPredicate = null,
		                                    DateTimeOffset? absoluteExpiration = null);
	}
}

