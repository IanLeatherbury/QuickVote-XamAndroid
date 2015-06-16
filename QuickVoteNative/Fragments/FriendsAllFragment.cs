using System;
using System.Collections.Generic;

using Android.Content;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

using QuickVoteNative.Activities;
using QuickVoteNative.Adapters;
using QuickVoteNative.Models;

namespace QuickVoteNative.Fragments
{
    public class FriendsAllFragment : Fragment
    {
//        public FriendsAllFragment();
//        {
//            this.RetainInstance = true;
//        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Android.OS.Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.newPoll_fragment, null);
            return view;
        }
    }
}