using System;
using Android.Widget;
using System.Collections.Generic;
using Android.App;
using QuickVoteNative.Models;
using QuickVoteNative.ViewModels;
using Android.Views;
using Microsoft.WindowsAzure.MobileServices;
using Android.Graphics;
using Android.Graphics.Drawables;
using System.Net;
using QuickVoteNative.Activities;
using QuickVoteNative.Adapters;
using QuickVoteNative.Imageloaders;

namespace QuickVoteNative
{
    public class CommunityAdapter : BaseAdapter
    {
        List<Poll> items;
        Activity context;

        public CommunityAdapter(Activity context, List<Poll> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }


        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.CommunityListItem, null);
            }

            view.FindViewById<TextView>(Resource.Id.userName).Text = item.UserName;
            view.FindViewById<TextView>(Resource.Id.question).Text = item.PollName;

            var imageView = view.FindViewById<ImageView>(Resource.Id.communityPicture);

            if (item.ImageUrl != null)
                imageView.SetImageDrawable(new CircleDrawable(GetImageBitmapFromUrl(item.ImageUrl)));

            return view;
        }

        public override int Count
        {
            get { return items.Count; }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            // could wrap a Contact in a Java.Lang.Object
            // to return it here if needed
            var pollName = new MyWrapper<Poll>(items[position]);
            return null;
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            if (url != null)
            {
                using (var webClient = new WebClient())
                {
                    var imageBytes = webClient.DownloadData(url);
                    if (imageBytes != null && imageBytes.Length > 0)
                    {
                        imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                    }
                }
            }
            else
            {
                return null;
            }

            return imageBitmap;
        }



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

//                public override Java.Lang.Object this[int position]
//                {
//                    get { return items[position]; }
//                }
    }

    class MyWrapper<T> : Java.Lang.Object 
    {
        private T _value;
        public MyWrapper(T managedValue)
        {
            _value = managedValue;
        }

        public T Value { get { return _value; } }
    }
}

