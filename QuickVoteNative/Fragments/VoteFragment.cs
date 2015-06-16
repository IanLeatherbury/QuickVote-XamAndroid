using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using QuickVoteNative.Activities;
using QuickVoteNative.ViewModels;
using Android.Renderscripts;
using Microsoft.WindowsAzure.MobileServices;
using QuickVoteNative.Models;
using QuickVote.Shared;

namespace QuickVoteNative
{
    public class VoteFragment : Android.Support.V4.App.Fragment
    {
        private const string TAG = "RecycleViewFragment";
        public RecyclerView recyclerView;
        public VoteAdapter adapter;
        public RecyclerView.LayoutManager layoutManager;

        public static MobileServiceClient Client = new MobileServiceClient(
            Constants.Url, Constants.Key);

        public string[] dataSet;
        public string[] DataSet
        {
            get
            {
                return this.dataSet;
            }
            set
            {
                dataSet = value;
            }
        }

        public string pollId;
        public string PollId
        {
            get
            {
                return this.pollId;
            }
            set
            {
                pollId = value;
            }
        }

        public string userName;
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

        public string title;
        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                title = value;
            }
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var rootView = inflater.Inflate(Resource.Layout.fragment_vote_public, container, false);
            rootView.SetTag(rootView.Id, TAG);
            recyclerView = rootView.FindViewById<RecyclerView>(Resource.Id.recyclerViewVote);

            // user linear layout manager
            layoutManager = new LinearLayoutManager(Activity);
            recyclerView.SetLayoutManager(layoutManager);

            //create adapter
            adapter = new VoteAdapter(dataSet);

            //add click event
            adapter.ItemClick += OnItemClick;

            // Set CustomAdapter as the adapter for RecycleView
            recyclerView.SetAdapter(adapter);

            return rootView;
        }



        void OnItemClick(object sender, int position)//string option)
        {
//            Toast.MakeText(Activity, "This is option " + option, ToastLength.Short).Show();

            ((VoteActivity)Activity).UpdatePoll(position);

            var intent = new Intent(Activity, typeof(ResultActivity));
            intent.PutExtra("pollTitle", Title);
            intent.PutExtra("pollId", ((VoteActivity)Activity).PollId);
            StartActivity(intent);
        }

    }
}

