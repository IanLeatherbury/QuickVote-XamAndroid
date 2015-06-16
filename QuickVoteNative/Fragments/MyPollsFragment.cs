using System;
using Android.App;
using Android.Views;
using Android.OS;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices;
using QuickVote.Shared;
using QuickVoteNative.Activities;
using System.Threading.Tasks;
using QuickVoteNative.Imageloaders;
using QuickVoteNative.Adapters;
using System.Collections.Generic;
using QuickVoteNative.ViewModels;
using System.Linq;
using Android.Content;

namespace QuickVoteNative
{
    public class MyPollsFragment : Android.Support.V4.App.Fragment
    {
        string[] values;
        View view;

        ImageLoader imageLoader;
        ImageLoaderAdapter imageLoaderAdapter;

        List<PollViewModel> pollViewModelList;

        string userName;
        string imgUrl;

        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                userName = value;
            }
        }

        public static MobileServiceClient Client = new MobileServiceClient(
                                                       Constants.Url, Constants.Key);

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_my_polls, null);
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            ISharedPreferences settings = Activity.GetSharedPreferences("QuickVotePreferences", 0);
            userName = settings.GetString("userName", "name");


            UserName = userName;

            var myPollsListView = view.FindViewById<ListView>(Resource.Id.myPollsListView);
            myPollsListView.ItemClick += OnListItemClick;

            imageLoader = new ImageLoader(Activity, 64, 40);

            imageLoaderAdapter = new ImageLoaderAdapter(this.Activity, imageLoader)
            {
                Parent = myPollsListView,
            };

            myPollsListView.Adapter = imageLoaderAdapter;

            myPollsListView.EmptyView = view.FindViewById<TextView>(Resource.Id.emptyView);

            GetMostRecentPublicPolls();
        }

        public async Task<List<MyPolls>> GetMyPolls()
        {
            try
            {
                var myPolls = await Client.GetTable<MyPolls>().Where(n => n.UserName == UserName).ToListAsync();
                return myPolls;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Azure error pulling polls" + exc);
            }
            return new List<MyPolls>();
        }

        public async void GetMostRecentPublicPolls()
        {
            //Get most recent public polls, and sets them as the image loader's List<PollViewModel>
            try
            {
                //Turn polls into poll view models
                var polls = await GetMyPolls();

                pollViewModelList = new List<PollViewModel>();

                pollViewModelList.AddRange(polls.Select(p => new PollViewModel
                        { 
                            Title = p.PollName, 
                            Image = MainActivity.ImgUrl,
                            UserName = p.UserName,
                            PollId = p.PollId
                        }).ToArray());

                imageLoaderAdapter.Polls = pollViewModelList;
            }
            catch (Exception exc)
            {
                Console.WriteLine("Azure error pulling polls" + exc);
            }
        }

        public void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Console.WriteLine("Clicked: " + e.Position);

            var resultActivity = new Intent(Activity, typeof(ResultActivity));
            resultActivity.PutExtra("pollId", pollViewModelList[e.Position].PollId);
//            resultActivity.PutExtra("imgUrl", MainActivity.ImgUrl);
            resultActivity.PutExtra("pollTitle", pollViewModelList[e.Position].Title);
            StartActivity(resultActivity);
        }
    }
}

