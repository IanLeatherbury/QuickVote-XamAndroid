
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
using Android.Support.V7.App;
using QuickVoteNative.Helpers;
using Android.Support.V4.Widget;
using Xamarin.Facebook.Login.Widget;

namespace QuickVoteNative
{
    [Activity(Label = "Test", Theme = "@style/Theme.AppCompat")]			
    public class Test : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView (Resource.Layout.test_layout);
        }
    }
}

