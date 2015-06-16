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
using BarChart;
using Android.Graphics;
using System.Runtime.InteropServices;
using Android.Support.V4.View;
using Java.Security;
using Android.Media;
using Akavache;
using System.Threading.Tasks;
using Splat;
using Android.Graphics.Drawables;
using System.Net;
using System.IO;
using Android.Support.V7.Widget;
using System.Drawing;
using QuickVoteNative.Commentloaders;
using Android.Views.InputMethods;
using Microsoft.WindowsAzure.MobileServices;
using QuickVoteNative.Models;
using QuickVote.Shared;

namespace QuickVoteNative
{
    [Activity(Label = "Leave a comment...")]			
    public class CommentActivity : BaseActivity, View.IOnClickListener
    {
        protected override int LayoutResource
        {
            get { return Resource.Layout.page_comment_activity; }
        }

        public static MobileServiceClient Client = new MobileServiceClient(
            Constants.Url, Constants.Key);

        string imgUrl;

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

        Poll poll = new Poll();

        List<Comment> pollComments;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            //get image url
            ISharedPreferences settings = GetSharedPreferences("QuickVotePreferences", 0);
            imgUrl = settings.GetString("imgUrl", "imgUrl");

            pollId = Intent.GetStringExtra("pollId");
            PollId = pollId;

            //create toolbar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarComment);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Comment";
            toolbar.SetNavigationIcon(Resource.Drawable.abc_ic_ab_back_mtrl_am_alpha);
            toolbar.SetNavigationOnClickListener(this);

            var transaction = SupportFragmentManager.BeginTransaction();
            var fragment = new PollCommentFragment();
            transaction.Replace(Resource.Id.comment_fragment, fragment);
            transaction.Commit();
        }

        public void OnClick(View v)
        {            
            OnBackPressed();
        }

        public async void UpdateComment(string comment)
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

            //update comment for poll
            Comment myComment = new Comment
                {
                    PollId = poll.Id, 
                    UserName = poll.UserName, 
                    PollName = poll.PollName,
                    MyComment = comment,
                    ImageUrl=imgUrl
                };
            try
            {
                await Client.GetTable<Comment>().InsertAsync(myComment);
            }
            catch (Exception exc)
            {
                Console.WriteLine("Azure error adding myPolls: " + exc);
            }
        }

        public async void GetComments()
        {
            try
            {
                pollComments = await Client.GetTable<Comment>().Where(x => x.PollId == pollId).ToListAsync();

            }
            catch (Exception exc)
            {
                System.Console.WriteLine("Azure error adding myPolls: " + exc);
            } 
        }


    }
}

