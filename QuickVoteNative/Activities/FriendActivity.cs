using System;
using System.Collections.Generic;

using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

using QuickVoteNative.Activities;
using QuickVoteNative.Models;
using QuickVoteNative.ViewModels;
using QuickVoteNative.Adapters;
using QuickVoteNative.Imageloaders;

namespace QuickVoteNative.Activities
{
    [Activity(Label = "Friend",ParentActivity = typeof(MainActivity))]
    [MetaData("android.support.PARENT_ACTIVITY", Value = "navdrawer.activities.HomeView")]
    public class FriendActivity : BaseActivity
    {
        private List<PollViewModel> _friends;
        private ImageLoader m_ImageLoader;

        protected override int LayoutResource {
            get {
                return Resource.Layout.page_friend;
            }
        }

        protected override void OnCreate(Android.OS.Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            m_ImageLoader = new ImageLoader(this);


//            _friends = Util.GenerateFriends();
            _friends.RemoveRange(0, _friends.Count - 2);
            var title = Intent.GetStringExtra("Title");
            var image = Intent.GetStringExtra("Image");

            title = string.IsNullOrWhiteSpace(title) ? "New Friend" : title;
            this.Title = title;

            if (string.IsNullOrWhiteSpace(image))
                image = _friends[0].Image;


            m_ImageLoader.DisplayImage(image, this.FindViewById<ImageView>(Resource.Id.friend_image), -1);
            this.FindViewById<TextView>(Resource.Id.friend_description).Text = title;

            var grid = FindViewById<GridView>(Resource.Id.grid);
            grid.Adapter = new MonkeyAdapter(this, _friends);
            grid.ItemClick += GridOnItemClick;

            //set title here
            SupportActionBar.Title = Title;
        }

        private void GridOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            var intent = new Intent(this, typeof(FriendActivity));
            intent.PutExtra("Title", _friends[itemClickEventArgs.Position].Title);
            intent.PutExtra("Image", _friends[itemClickEventArgs.Position].Image);
            StartActivity(intent);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)           
            {
                case Android.Resource.Id.Home:

                    NavUtils.NavigateUpFromSameTask(this);

                    break;
            }

            return base.OnOptionsItemSelected(item);
        }
    }
}