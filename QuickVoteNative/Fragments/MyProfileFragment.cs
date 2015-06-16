using System;
using Android.App;
using Android.Views;
using Android.OS;
using Android.Widget;
using Xamarin.Facebook;

namespace QuickVoteNative
{
    public class MyProfileFragment : Fragment
    {
        Button loginFbButton;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_my_profile, null);
            view.FindViewById<TextView>(Resource.Id.textView1).SetText(Resource.String.helloMyProfile);

            return view;
        }
    }
}

