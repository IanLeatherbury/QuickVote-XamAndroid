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
using QuickVoteNative.Activities;
using BarChart;
using Android.Graphics;
using System.Runtime.InteropServices;
using Android.Support.V4.View;
using Java.Security;
using Android.Media;
using Akavache;
using System.Threading.Tasks;
using Splat;
using Android.Graphics.Drawables;
using System.Net;
using System.IO;
using Android.Support.V7.Widget;
using System.Drawing;
using QuickVoteNative.Commentloaders;
using Microsoft.WindowsAzure.MobileServices;
using QuickVoteNative.Models;
using QuickVote.Shared;
using com.refractored.monodroidtoolkit.imageloader;
using Newtonsoft.Json.Linq;
using Android.Views.TextService;

namespace QuickVoteNative.Activities
{
    [Activity(Label = "Here's your results!")]			
    public class ResultActivity : BaseActivity, View.IOnClickListener
    {
        string pollId;
        int width, height;
        Poll thisPoll;
        LinearLayout commentLayout;
        List<Comment> pollComments, localComments = new List<Comment>();
        string imgUrl;

        protected override int LayoutResource
        {
            get { return Resource.Layout.page_result_activity; }
        }

        public static MobileServiceClient Client = new MobileServiceClient(
                                                       Constants.Url, Constants.Key);

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            OverridePendingTransition(Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_fade_out);


            //get shared preferences
            ISharedPreferences settings = GetSharedPreferences("QuickVotePreferences", 0);
            imgUrl = settings.GetString("imgUrl", "imgUrl");

            //get poll Id & get poll data
            pollId = Intent.GetStringExtra("pollId");
            GetPollData();

            //find comment layout & retrieve comments, if any
            commentLayout = (LinearLayout)FindViewById(Resource.Id.comment_linear_layout);
            GetComments();

            //create toolbar
            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbarResult);
            SetSupportActionBar(toolbar);
            SupportActionBar.Title = "Results";
            toolbar.SetNavigationIcon(Resource.Drawable.abc_ic_ab_back_mtrl_am_alpha);
            toolbar.SetNavigationOnClickListener(this);

            //set poll title
            var pollTitle = FindViewById<TextView>(Resource.Id.poll_header);
            pollTitle.Text = Intent.GetStringExtra("pollTitle");

            var commentButton = FindViewById<Button>(Resource.Id.comment);
            commentButton.Click += (object sender, EventArgs e) =>
            {
                var commentActivity = new Intent(this, typeof(CommentActivity));
                commentActivity.PutExtra("pollId", pollId);
                commentActivity.PutExtra("imgUrl", Intent.GetStringExtra("imgUrl"));
                StartActivity(commentActivity);
            };
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
//            MenuInflater.Inflate(Resource.Menu.results, menu);
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {   
            Toast.MakeText(this, "Top ActionBar pressed: " + item.TitleFormatted, ToastLength.Short).Show();
            return base.OnOptionsItemSelected(item);
        }

        public void OnClick(View v)
        {            
            OnBackPressed();
        }

        protected override void OnResume()
        {
            base.OnResume();
            GetComments();
        }

        void SetUpBarChart()
        {
            
            if (thisPoll != null)
            {
//                //set values in bar model
//                var option1 = new BarModel
//                {
//                    Value = thisPoll.Option1Count,
//                    Legend = "1"
//                };
//
//                var option2 = new BarModel
//                {
//                    Value = thisPoll.Option2Count,
//                    Legend = "2"
//                };
//                var option3 = new BarModel
//                {
//                    Value = thisPoll.Option3Count,
//                    Legend = "3"
//                };
//                var option4 = new BarModel
//                {
//                    Value = thisPoll.Option4Count,
//                    Legend = "4"
//                };

                //dummy data
                #region Options
                var option1 = new BarModel
                {
                    Value = 37,
                    Legend = "1"
                };
                
                var option2 = new BarModel
                {
                    Value = 34,
                    Legend = "2"
                };
                var option3 = new BarModel
                {
                    Value = 9,
                    Legend = "3"
                };
                var option4 = new BarModel
                {
                    Value = 20,
                    Legend = "4"
                };
                #endregion

                //make data array
                var data = new [] { option1, option2, option3, option4 };

                //add data to bar chart
                var barChart = FindViewById<BarChartView>(Resource.Id.barChart);
                barChart.ItemsSource = data;

                // get window width / height
                Display display = WindowManager.DefaultDisplay;
                Android.Graphics.Point size = new Android.Graphics.Point();
                display.GetSize(size);
                width = size.X;
                height = size.Y;

                //set height of bar chart
                var barChartView = FindViewById<BarChartView>(Resource.Id.barChart);
                barChartView.LayoutParameters.Height = (int)(height * .35);

                //set some bar chart attributes
                barChart.GridHidden = true;
                barChart.AutoLevelsEnabled = false;
                barChart.LegendColor = Android.Graphics.Color.ParseColor("#2c3e50");

                barChart.BarWidth = (width - width * (float).25) / 4;  
            } 


        }

        void SetUpLabels()
        {
            //text labels
            var option1Label = FindViewById<TextView>(Resource.Id.option1);
            option1Label.Text = "1: " + thisPoll.Option1;

            var option2Label = FindViewById<TextView>(Resource.Id.option2);
            option2Label.Text = "2: " + thisPoll.Option2;

            var option3Label = FindViewById<TextView>(Resource.Id.option3);
            option3Label.Text = "3: " + thisPoll.Option3;

            var option4Label = FindViewById<TextView>(Resource.Id.option4);
            option4Label.Text = "4: " + thisPoll.Option4;

            //percent labels
            var option1Percent = FindViewById<TextView>(Resource.Id.option1Percent);
            int percent1 = (int)(((double)thisPoll.Option1Count) / ((double)thisPoll.Option1Count + thisPoll.Option2Count +
                           (double)thisPoll.Option3Count + (double)thisPoll.Option4Count) * 100);
            option1Percent.Text = percent1.ToString() + "%";

            var option2Percent = FindViewById<TextView>(Resource.Id.option2Percent);
            int percent2 = (int)(((double)thisPoll.Option2Count) / ((double)thisPoll.Option1Count + thisPoll.Option2Count +
                           (double)thisPoll.Option3Count + (double)thisPoll.Option4Count) * 100);
            option2Percent.Text = percent2.ToString() + "%";

            var option3Percent = FindViewById<TextView>(Resource.Id.option3Percent);
            int percent3 = (int)(((double)thisPoll.Option3Count) / ((double)thisPoll.Option1Count + thisPoll.Option2Count +
                           (double)thisPoll.Option3Count + (double)thisPoll.Option4Count) * 100);
            option3Percent.Text = percent3.ToString() + "%";

            var option4Percent = FindViewById<TextView>(Resource.Id.option4Percent);
            int percent4 = (int)(((double)thisPoll.Option4Count) / ((double)thisPoll.Option1Count + thisPoll.Option2Count +
                           (double)thisPoll.Option3Count + (double)thisPoll.Option4Count) * 100);
            option4Percent.Text = percent4.ToString() + "%";
        }

        public async void GetComments()
        {
            try
            {
                pollComments = await Client.GetTable<Comment>().Where(x => x.PollId == pollId).ToListAsync();
            }
            catch (Exception exc)
            {
                Console.WriteLine("Azure error adding myPolls: " + exc);
            }  

            if (pollComments != null)
            {
                //get me all of the polls that are not in the local list, and add them to the local list
                //if they are already in the local list, don't add them
                var pollNotAlreadyInLocalList = pollComments.Where(p => !localComments.Any(lc => lc.MyComment == p.MyComment)).ToList();
                if (localComments != null)
                    localComments.AddRange(pollNotAlreadyInLocalList);

                //add comments to screen
//                if(localComments !=null)
                if ((commentLayout.ChildCount < localComments.Count) && (localComments != null))
                    foreach (var c in localComments)
                    {
                        //Inflate comment view
                        View comment = View.Inflate(this, Resource.Layout.comment, null);
                        comment.SetMinimumHeight(25);

                        //add my comment
                        TextView commentText = (TextView)comment.FindViewById(Resource.Id.comment);
                        commentText.Text = c.MyComment;

                        //add name
                        TextView nameText = (TextView)comment.FindViewById(Resource.Id.name);
                        nameText.Text = c.UserName;

                        //add picture
                        ImageView image = (ImageView)comment.FindViewById(Resource.Id.image);
                        var imageBitmap = GetImageBitmapFromUrl(c.ImageUrl);
                        image.SetImageDrawable(new CircleDrawable(imageBitmap));

                        commentLayout.AddView(comment);
                    }
            }
        }

        async void GetPollData()
        {
            try
            {
                var pollList = await Client.GetTable<Poll>().Where(x => x.Id == pollId).ToListAsync();
                thisPoll = pollList[0];
            }
            catch (Exception exc)
            {
                Console.WriteLine("Error getting poll, result activity " + exc);
            }

            if (thisPoll != null)
            {
                SetUpBarChart();
                SetUpLabels();
            }
            else
            {
                Console.WriteLine("thisPoll was null!");
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

        // Circle image stuff
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

