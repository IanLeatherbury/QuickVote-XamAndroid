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
using System.Net;
using System.IO;
using Splat;
using QuickVoteNative.Activities;
using System.Threading.Tasks;
using Android.Graphics.Drawables;
using QuickVoteNative.Imageloaders;
using QuickVoteNative.Adapters;
using Microsoft.WindowsAzure.MobileServices;
using Java.Security;
using QuickVoteNative.ViewModels;
using QuickVoteNative.Commentloaders;
using Android.Views.InputMethods;
using QuickVote.Shared;
using Android.Locations;

namespace QuickVoteNative
{
    public class PollCommentFragment : Android.Support.V4.App.Fragment
    {
        View view;
        string imgUrl;
        string pollId;
        string userName;
        EditText commentInput;
        ImageButton postCommentButton;

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

        CommentLoader commentLoader;
        CommentLoaderAdapter commentLoaderAdapter;

        List<PollViewModel> localComments = new List<PollViewModel>();
        List<Comment> pollComments;

        public static MobileServiceClient Client = new MobileServiceClient(
                                                       Constants.Url, Constants.Key);

        public string ImgUrl
        {
            get
            {
                return this.imgUrl;
            }
            set
            {
                imgUrl = value;
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_poll_comment, null);

            return view;
        }

        public override void OnActivityCreated(Bundle saveInstanceState)
        {
            base.OnActivityCreated(saveInstanceState);
            //get user name from global prefs
            ISharedPreferences settings = Activity.GetSharedPreferences("QuickVotePreferences", 0);
            userName = settings.GetString("userName", "name");
            imgUrl = settings.GetString("imgUrl", "imgUrl");

            var commentListView = view.FindViewById<ListView>(Resource.Id.android_commentList);
//            commentListView.ItemClick += OnListItemClick;

            commentLoader = new CommentLoader(this.Activity, 64, 40);

            commentLoaderAdapter = new CommentLoaderAdapter(Activity, commentLoader)
            {
                Parent = commentListView
            };

            commentListView.Adapter = commentLoaderAdapter;

            commentListView.EmptyView = view.FindViewById<TextView>(Resource.Id.emptyViewComment);

            GetComments();

            //find comment controls
            commentInput = view.FindViewById<EditText>(Resource.Id.enter_comment);
            postCommentButton = view.FindViewById<ImageButton>(Resource.Id.post_comment);

          

            postCommentButton.Click += (o, s) =>
            {
                    AddNewComment();
            };
        }

        public async void GetComments()
        {
            //get comments
            try
            {
                pollComments = await Client.GetTable<Comment>().Where(x => x.PollId == ((CommentActivity)Activity).PollId).ToListAsync();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Azure error adding myPolls: " + exc);
            } 

            if (pollComments != null)
            {
                var pollNotAlreadyInLocalList = pollComments.Where(p => !localComments.Any(lc => lc.Title == p.MyComment)).ToList();
//                localComments.AddRange(pollComments.Select(c => new PollViewModel
                localComments.AddRange(pollNotAlreadyInLocalList.Select(c => new PollViewModel//pollComments.Where(p => !localComments.Any(lc => lc.Title == p.MyComment))
                        {
                            Title = c.MyComment,
                            UserName = c.UserName,
                            Image = c.ImageUrl
                        }).ToArray());
                commentLoaderAdapter.Friends = localComments;
            }
        }

        public void AddNewComment()
        {
            //save comment to poll
            localComments.Add(new PollViewModel
                {
                    UserName = userName,
                    Image = imgUrl,
                    Title = commentInput.Text,
                });

            ((CommentActivity)Activity).UpdateComment(commentInput.Text);
            commentLoaderAdapter.Friends = localComments;
            commentInput.Text = "";
        }


        public void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //do something
        }

        void NavigateBack()
        {
//            var intent = new Intent(Activity, typeof(ResultActivity));
//            StartActivity(intent);
            ((CommentActivity)Activity).OnBackPressed();
        }
    }
}

