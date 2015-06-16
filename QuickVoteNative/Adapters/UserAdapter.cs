using System;
using Android.Widget;
using QuickVoteNative.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Support.V4.App;
using QuickVoteNative.Activities;
using Xamarin.Facebook;
using Android.Locations;
using Android.Graphics;
using Android.Graphics.Drawables;
using System.Net;
using Android.Media;

namespace QuickVoteNative
{
    public class UserAdapter : BaseAdapter<User>
    {
        View view;
        Activity context;
        List<User> list;

        public UserAdapter(Activity a, List<User> l)
            : base()
        {
            this.context = a;
            this.list = l;
        }

        public override int Count
        {
            get { return list.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override User this [int index]
        {
            get { return list[index]; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            view = convertView; 

            // re-use an existing view, if one is available
            // otherwise create a new one
            if (view == null)
                view = context.LayoutInflater.Inflate(Resource.Layout.profile_pic_item, parent, false);

            User item = this[position];
            view.FindViewById<TextView>(Resource.Id.contactName).Text = MainActivity.UserNameForPoll;
            GetProfilePicture();

            return view;
        }

        public void GetProfilePicture()
        {
            //get/set preferences
            ISharedPreferences settings = context.GetSharedPreferences("QuickVotePreferences", 0);
            var imgUrl = settings.GetString("imgUrl", "imgUrl");

            var imageBitmap = GetImageBitmapFromUrl(imgUrl);

            var imageView = view.FindViewById<ImageView>(Resource.Id.profilePictureCircle);

            if (imageView != null)
                imageView.SetImageDrawable(new CircleDrawable(imageBitmap));
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
                    return (int)Android.Graphics.Format.Opaque;
                }
            }

            public override void SetColorFilter(ColorFilter cf)
            {

            }
        }
    }
}

