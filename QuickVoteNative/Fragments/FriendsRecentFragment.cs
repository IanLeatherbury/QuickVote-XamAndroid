using System;
using System.Collections.Generic;

using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

using QuickVoteNative.Activities;
using QuickVoteNative.Adapters;
using QuickVoteNative.Models;
using QuickVoteNative.ViewModels;

namespace QuickVoteNative.Fragments
{
    public class FriendsRecentFragment : Fragment
    {
        public FriendsRecentFragment()
        {
            this.RetainInstance = true;
        }

        private List<PollViewModel> _friends;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_friends_recent, null);
            var grid = view.FindViewById<GridView>(Resource.Id.grid);
//            _friends = Util.GenerateFriends();
            _friends.RemoveRange(0, _friends.Count - 4);
            grid.Adapter = new MonkeyAdapter(Activity, _friends);

            grid.ItemClick += GridOnItemClick;
            return view;
        }

        private void GridOnItemClick(object sender, AdapterView.ItemClickEventArgs itemClickEventArgs)
        {
            var intent = new Intent(Activity, typeof(FriendActivity));
            intent.PutExtra("Title", _friends[itemClickEventArgs.Position].Title);
            intent.PutExtra("Image", _friends[itemClickEventArgs.Position].Image);
            StartActivity(intent);
        }
    }
}
