using System;
using System.Collections.Generic;

using Android.Content;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

using QuickVoteNative.Activities;
using QuickVoteNative.Adapters;
using QuickVoteNative.Models;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace QuickVoteNative.Fragments
{
    public class NewPollFragment : Fragment
    {
        // Connect to Azure
        public static MobileServiceClient Client = new MobileServiceClient(
                                                       Constants.Url, Constants.Key);

        EditText pollName, optionOne, optionTwo, optionThree, optionFour;
        Button savePoll;

        public NewPollFragment()
        {
            this.RetainInstance = true;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_new_poll, null);

            pollName = view.FindViewById<EditText>(Resource.Id.pollNameEntry);
            savePoll = view.FindViewById<Button>(Resource.Id.chooseFriendsButton);

            //options
            optionOne = view.FindViewById<EditText>(Resource.Id.option1Entry);
            optionTwo = view.FindViewById<EditText>(Resource.Id.option2Entry);
            optionThree = view.FindViewById<EditText>(Resource.Id.option3Entry);
            optionFour = view.FindViewById<EditText>(Resource.Id.option4Entry);

            //save
            savePoll.Click += SavePoll;

            return view;
        }

        async void SavePoll(object sender, EventArgs e)
        {
            var mainActivity = (MainActivity)this.Activity;
            var userName = mainActivity.FetchUserName();
            JObject pollJson = new JObject();
            pollJson.Add("username", userName);
            pollJson.Add("pollname", pollName.Text);
            pollJson.Add("option1", optionOne.Text);
            pollJson.Add("option2", optionTwo.Text);
            pollJson.Add("option3", optionThree.Text);
            pollJson.Add("option4", optionFour.Text);

            optionOne.Text="";
            optionTwo.Text="";
            optionThree.Text="";
            optionFour.Text="";
            pollName.Text = "";

            try
            {
                await Client.GetTable<Poll>().InsertAsync(pollJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update poll exception: {0}", ex);
            }

        }
    }
}