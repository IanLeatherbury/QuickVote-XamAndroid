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
using QuickVoteNative.Activities;
using QuickVoteNative.ViewModels;
using QuickVote.Shared;
using Microsoft.WindowsAzure.MobileServices;
using QuickVoteNative.Models;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace QuickVoteNative
{
    [Activity(Label = "VoteActivity", LaunchMode = Android.Content.PM.LaunchMode.SingleTop, NoHistory = true)]			
    public class VoteActivity : BaseActivity, View.IOnClickListener
    {
        Poll poll = new Poll();
        string pollId;
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

        public static MobileServiceClient Client = new MobileServiceClient(
                                                       Constants.Url, Constants.Key);
        
        List<string> voteOptions = new List<string>();

        public string[] voteArray;

        public string[] VoteArray
        {
            get
            {
                return this.voteArray;
            }
            set
            {
                voteArray = value;
            }
        }

        protected override int LayoutResource
        {
            get { return Resource.Layout.page_vote_activity; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //create toolbar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarVote);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Vote";
            toolbar.SetNavigationIcon(Resource.Drawable.abc_ic_ab_back_mtrl_am_alpha);
            toolbar.SetNavigationOnClickListener(this);

            //set pollId
            pollId = Intent.GetStringExtra("pollId");
            PollId = pollId;

            //Get poll title from PollViewModel
            var pollTitle = FindViewById<TextView>(Resource.Id.poll_title_vote);
            pollTitle.Text = Intent.GetStringExtra("pollTitle");

            //Get options from PollViewModel
            if (Intent.GetStringExtra("optionOne") != null)
                voteOptions.Add(Intent.GetStringExtra("optionOne"));
            else
                voteOptions.Add("");

            if (Intent.GetStringExtra("optionTwo") != null)
                voteOptions.Add(Intent.GetStringExtra("optionTwo"));
            else
                voteOptions.Add("");

            if (Intent.GetStringExtra("optionThree") != null)
                voteOptions.Add(Intent.GetStringExtra("optionThree"));
            else
                voteOptions.Add("");

            if (Intent.GetStringExtra("optionFour") != null)
                voteOptions.Add(Intent.GetStringExtra("optionFour"));
            else
                voteOptions.Add("");

            voteArray = voteOptions.ToArray();
                
            //load Vote fragment
            var transaction = SupportFragmentManager.BeginTransaction();
            var fragment = new VoteFragment();
            fragment.DataSet = voteArray;
//            fragment.PollId = Intent.GetStringExtra("pollId");
            fragment.Title = Intent.GetStringExtra("pollTitle");
            transaction.Replace(Resource.Id.vote_fragment, fragment);
            transaction.Commit();

        }

        public void OnClick(View v)
        {            
            base.OnBackPressed();
        }

        public async void UpdatePoll(int selectedOption)
        {
            //get poll to update
            try
            {
                var pollQuery = await Client.GetTable<Poll>().Where(x => x.Id == pollId).ToListAsync();
                poll = pollQuery[0];
            }
            catch (Exception exc)
            {
                Console.WriteLine("Azure error query for poll: " + exc);
            }

            //update MyPolls
            MyPolls myPolls = new MyPolls { PollId = pollId, UserName = poll.UserName, PollName = poll.PollName };
            try
            {
                await Client.GetTable<MyPolls>().InsertAsync(myPolls);
            }
            catch (Exception exc)
            {
                System.Console.WriteLine("Azure error adding myPolls: " + exc);
            }

            //Cast vote and update poll
            switch (selectedOption)
            {
                case 0:
                    {
                        try
                        {
                            //Update Poll
                            JObject pollUpdate = new JObject();
                            pollUpdate.Add("id", pollId);
                            pollUpdate.Add("Option1Count", poll.Option1Count + 1);
                            var updateOption = await Client.GetTable<Poll>().UpdateAsync(pollUpdate);//Where(x => x.Id == Intent.GetStringExtra("pollId")).ToListAsync();
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("Azure error update poll: " + exc);
                        }
                        return;
                    }
                case 1:
                    {
                        try
                        {
                            //Update Poll
                            JObject pollUpdate = new JObject();
                            pollUpdate.Add("id", pollId);
                            pollUpdate.Add("Option2Count", poll.Option2Count + 1);
                            var updateOption = await Client.GetTable<Poll>().UpdateAsync(pollUpdate);//Where(x => x.Id == Intent.GetStringExtra("pollId")).ToListAsync();
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("Azure error update poll:: " + exc);
                        }
                        return;
                    }
                case 2:
                    {
                        try
                        {
                            //Update Poll
                            JObject pollUpdate = new JObject();
                            pollUpdate.Add("id", pollId);
                            pollUpdate.Add("Option3Count", poll.Option3Count + 1);
                            var updateOption = await Client.GetTable<Poll>().UpdateAsync(pollUpdate);//Where(x => x.Id == Intent.GetStringExtra("pollId")).ToListAsync();
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("Azure error update poll:: " + exc);
                        }
                        return;
                    }
                case 3:
                    {
                        try
                        {
                            //Update Poll
                            JObject pollUpdate = new JObject();
                            pollUpdate.Add("id", pollId);
                            pollUpdate.Add("Option4Count", poll.Option4Count + 1);
                            var updateOption1 = await Client.GetTable<Poll>().UpdateAsync(pollUpdate);//Where(x => x.Id == Intent.GetStringExtra("pollId")).ToListAsync();
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("Azure error update poll: " + exc);
                        }
                        return;
                    }
            }
        }
    }
}

