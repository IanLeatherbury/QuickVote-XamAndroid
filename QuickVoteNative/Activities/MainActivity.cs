using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Util;
using Microsoft.WindowsAzure.MobileServices;
using System.Linq;

using Xamarin.Facebook;
using Android.Content.PM;
using Java.Security;
using Android.Support.V4.Widget;
using QuickVoteNative.Helpers;
using QuickVoteNative.Fragments;
using QuickVoteNative.Activities;
using Android.Content.Res;
using System.Collections.Generic;
using QuickVoteNative.Models;
using Android.Graphics.Drawables;
using Xamarin.Facebook.Login;
using Android.Graphics;
using System.Threading;
using System.Net;
using Java.IO;
using System.IO;
using Android.Media;
using System.Reactive.Linq;
using Akavache;
using Newtonsoft.Json;

//using Android.Transitions;
using Xamarin.Facebook.Login.Widget;
using Xamarin;

[assembly:MetaData("com.facebook.sdk.ApplicationId", Value = "@string/app_id")]

namespace QuickVoteNative.Activities
{
    [Activity(Label = "QuickVote", Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleTop, WindowSoftInputMode = SoftInput.StateHidden)]
    public class MainActivity : BaseActivity//, IStatusCallback, Request.IGraphUserCallback
    {
        string drawerTitle, title, pollId;
        static string imgUrl, userNameForPoll;

        ImageView imageView;
        MyActionBarDrawerToggle drawerToggle;
        DrawerLayout drawerLayout;
        ListView drawerListView, drawerListView2;
        ProfilePictureView profilePictureView;
        TextView userName;

        protected override int LayoutResource
        {
            get { return Resource.Layout.page_home_view; }
        }

        public static MobileServiceClient Client = new MobileServiceClient(
                                                       Constants.Url, Constants.Key);

        static readonly string[] Sections =
            {
                "Vote!", "Logout"
            };


        public static string ImgUrl
        {
            get
            {
                return imgUrl;
            }
            set
            {
                imgUrl = value;
            }
        }

        public static string UserNameForPoll
        {
            get
            {
                return userNameForPoll;
            }
            set
            {
                userNameForPoll = value;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            UserNameForPoll = Intent.GetStringExtra("userName");
            ImgUrl = Intent.GetStringExtra("imgUrl");

            //save name and url to shared global preferences
            if (UserNameForPoll != null)
            {
                ISharedPreferences settings = GetSharedPreferences("QuickVotePreferences", 0);
                ISharedPreferencesEditor editor = settings.Edit();
                editor.PutString("userName", UserNameForPoll);
                editor.PutString("imgUrl", ImgUrl);
                editor.Commit();
            }

            OverridePendingTransition(Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_fade_out);

            Insights.Initialize("d1101336ba502d377358840f1fa4e42d312801a4", this);
            Insights.Identify(UserNameForPoll, null);

            BlobCache.ApplicationName = "QuickVote";
            BlobCache.EnsureInitialized();

            List<User> list = new List<User>();
            User ian = new User{ Name = "Ian" };
            list.Add(ian);

            title = drawerTitle = Title;

            drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawerListView = FindViewById<ListView>(Resource.Id.left_drawer_list);

            drawerListView2 = FindViewById<ListView>(Resource.Id.left_drawer_account_info);

            //Create Adapter for drawer List
            drawerListView.Adapter = new ArrayAdapter<string>(this, Resource.Layout.item_menu, Sections);
            drawerListView2.Adapter = new UserAdapter(this, list);

            //Set click handler when item is selected
            drawerListView.ItemClick += (sender, args) => ListItemClicked(args.Position);

            //Set Drawer Shadow
            drawerLayout.SetDrawerShadow(Resource.Drawable.drawer_shadow_dark, (int)GravityFlags.Start);

            //DrawerToggle is the animation that happens with the indicator next to the actionbar
            drawerToggle = new MyActionBarDrawerToggle(this, drawerLayout,
                Toolbar,
                Resource.String.drawer_open,
                Resource.String.drawer_close);

            //Display the current fragments title and update the options menu
            drawerToggle.DrawerClosed += (o, args) =>
            {
                SupportActionBar.Title = title;
                InvalidateOptionsMenu();
            };

            //Display the drawer title and update the options menu
            drawerToggle.DrawerOpened += (o, args) =>
            {
                SupportActionBar.Title = drawerTitle;
                InvalidateOptionsMenu();
            };

            //Set the drawer lister to be the toggle.
            drawerLayout.SetDrawerListener(drawerToggle);

            //if first time you will want to go ahead and click first item.
            if (savedInstanceState == null)
            {
                ListItemClicked(0);
            }
        }

        public string FetchUserName()
        {

            return Intent.GetStringExtra("userName");
        }

        private void ListItemClicked(int position)
        {
            Android.Support.V4.App.Fragment fragment = null;
            switch (position)
            {
                case 0:
                    fragment = new FriendsFragment();
                    break;
                case 1:
                    fragment = new FriendsFragment();
                    break;
            }

            SupportFragmentManager.BeginTransaction()
                .Replace(Resource.Id.content_frame, fragment)
                .Commit();

            drawerListView.SetItemChecked(position, true);
            SupportActionBar.Title = title = "QuickVote";
            drawerLayout.CloseDrawers();
        }

        public override bool OnPrepareOptionsMenu(IMenu menu)
        {

            var drawerOpen = drawerLayout.IsDrawerOpen((int)GravityFlags.Left);
            //when open don't show anything
            for (int i = 0; i < menu.Size(); i++)
                menu.GetItem(i).SetVisible(!drawerOpen);


            return base.OnPrepareOptionsMenu(menu);
        }

        protected override void OnPostCreate(Bundle savedInstanceState)
        {
            base.OnPostCreate(savedInstanceState);
            drawerToggle.SyncState();
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            base.OnConfigurationChanged(newConfig);
            drawerToggle.OnConfigurationChanged(newConfig);
        }

        // Pass the event to ActionBarDrawerToggle, if it returns
        // true, then it has handled the app icon touch event
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (drawerToggle.OnOptionsItemSelected(item))
                return true;

            return base.OnOptionsItemSelected(item);
        }

        async void SaveProfilePictureToFile(string url)
        {
            try
            {
                await BlobCache.LocalMachine.LoadImageFromUrl(url, false, 200, 200);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("Akavache Exception: " + e);
            }
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }


        // Circle image implementation
        public class CircleDrawable : Drawable
        {
            Bitmap bmp;
            BitmapShader bmpShader;
            Paint paint;
            RectF oval;

            public CircleDrawable(Bitmap bmp)
            {
                this.bmp = bmp;
                this.bmpShader = new BitmapShader(bmp, Shader.TileMode.Clamp, Shader.TileMode.Clamp);
                this.paint = new Paint() { AntiAlias = true };
                this.paint.SetShader(bmpShader);
                this.oval = new RectF();
            }

            public override void Draw(Canvas canvas)
            {
                canvas.DrawOval(oval, paint);
            }

            protected override void OnBoundsChange(Rect bounds)
            {
                base.OnBoundsChange(bounds);
                oval.Set(0, 0, bounds.Width(), bounds.Height());
            }

            public override int IntrinsicWidth
            {
                get
                {
                    return bmp.Width;
                }
            }

            public override int IntrinsicHeight
            {
                get
                {
                    return bmp.Height;
                }
            }

            public override void SetAlpha(int alpha)
            {

            }

            public override int Opacity
            {
                get
                {
                    return (int)Format.Opaque;
                }
            }

            public override void SetColorFilter(ColorFilter cf)
            {

            }
        }
    }
}