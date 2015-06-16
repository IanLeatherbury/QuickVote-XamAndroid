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
//using com.refractored.monodroidtoolkit.imageloader;
using QuickVoteNative.Models;
using QuickVoteNative.Imageloaders;
using QuickVoteNative.ViewModels;

namespace QuickVoteNative.Adapters
{
    public class ImageLoaderWrapper : Java.Lang.Object
    {
        public TextView Name { get; set; }
        public ImageView Image { get; set; }
        public TextView Question {get; set;}
        public string OptionOne {get; set;}
        public string OptionTwo {get; set;}
        public string OptionThree {get; set;}
        public string OptionFour {get; set;}

    }
    public class ImageLoaderAdapter : BaseAdapter<PollViewModel>
    {
        private Activity context;
        private ImageLoader imageLoader;

        List<PollViewModel> polls = new List<PollViewModel>();
        public List<PollViewModel> Polls
        {
            get
            {
                return polls;
            }
            set
            {
                polls = value ?? new List<PollViewModel>();
                this.NotifyDataSetChanged();
            }
        }

        public ImageLoaderAdapter(Activity context, ImageLoader imageLoader)
        {
            this.imageLoader = imageLoader;
            this.context = context;
        }

        public ListView Parent
        {
            get
            {
                return parent.Target as ListView;
            }
            set
            {
                parent = new WeakReference( value);
            }
        }
        WeakReference parent;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ImageLoaderWrapper wrapper = null;
            var view = convertView;
            if (convertView == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.friend, null);
                wrapper = new ImageLoaderWrapper();
                wrapper.Name = view.FindViewById<TextView>(Resource.Id.name);
                wrapper.Image = view.FindViewById<ImageView>(Resource.Id.image);
                wrapper.Question = view.FindViewById<TextView>(Resource.Id.question);
                view.Tag = wrapper;
            }
            else
            {
                wrapper = convertView.Tag as ImageLoaderWrapper;
            }

            var poll = polls[position];
            wrapper.Name.Text = poll.UserName;
            wrapper.Question.Text = poll.Title;

            //null checks on options. needs to be refactored.
            if (poll.OptionOne != null)
                wrapper.OptionOne = poll.OptionOne;
            else
                wrapper.OptionOne = "";

            if (poll.OptionTwo != null)
                wrapper.OptionTwo = poll.OptionTwo;
            else
                wrapper.OptionTwo = "";

            if (poll.OptionThree != null)
                wrapper.OptionThree = poll.OptionThree;
            else
                wrapper.OptionThree = "";

            if (poll.OptionFour != null)
                wrapper.OptionFour = poll.OptionFour;
            else
                wrapper.OptionFour = "";

            if(poll.Image != null)
                imageLoader.DisplayImage(poll.Image, wrapper.Image, -1);

            return view;
        }

        public override PollViewModel this[int position]
        {
            get { return polls[position]; }
        }

        public override int Count
        {
            get { return polls.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }
    }
}