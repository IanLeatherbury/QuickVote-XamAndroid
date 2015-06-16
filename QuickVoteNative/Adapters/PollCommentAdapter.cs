using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.Util;
using System.IO;


using System.Threading.Tasks;
using System.Net;
using Splat;
using Android.Graphics.Drawables;
using QuickVoteNative.Activities;

namespace QuickVoteNative
{
    public class PollCommentAdapter : RecyclerView.Adapter
    {
        public const string TAG = "PollComment";
        private string[] dataSet;

        // Provide a reference to the type of views that you are using (custom ViewHolder)
        public class ViewHolder : RecyclerView.ViewHolder
        {
            private TextView textView;

            public TextView TextView
            {
                get{ return textView; }
            }

            public ViewHolder(View v) : base(v)
            {
                textView = (TextView) v.FindViewById(Resource.Id.textView);
            }
        }

        // Initialize the dataset of the Adapter
        public PollCommentAdapter(string[] dataSet)
        {
            this.dataSet = dataSet;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup viewGroup, int position)
        {
            View v = LayoutInflater.From (viewGroup.Context)
                .Inflate (Resource.Layout.comment, viewGroup, false);
            ViewHolder vh = new ViewHolder (v);

            var profilePicture = v.FindViewById<ImageView>(Resource.Id.communityPicture);
            GetSetImage().ContinueWith(task => {
                profilePicture.SetImageDrawable(task.Result);
            });

            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            Log.Debug (TAG, "\tElement " + position + " set.");

            // Get element from your dataset at this position and replace the contents of the view
            // with that element
            (viewHolder as ViewHolder).TextView.SetText(dataSet [position],TextView.BufferType.Normal);
        }

        // Return the size of your dataset (invoked by the layout manager)
        public override int ItemCount
        {
            get{ return dataSet.Length; }
        }

        async Task<Drawable> GetSetImage()
        {
            var wc = new WebClient();
            byte[] imageBytes = await wc.DownloadDataTaskAsync(MainActivity.ImgUrl);

            MemoryStream stream = new MemoryStream(imageBytes);

            IBitmap profileImage = await BitmapLoader.Current.Load(stream, 400, 400);

            return profileImage.ToNative();
        }
    }
}

