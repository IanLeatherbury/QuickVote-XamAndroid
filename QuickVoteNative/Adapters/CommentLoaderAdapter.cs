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
//using com.refractored.monodroidtoolkit.Commentloader;
using QuickVoteNative.Models;
using QuickVoteNative.Imageloaders;
using QuickVoteNative.ViewModels;
using QuickVoteNative.Commentloaders;

namespace QuickVoteNative.Adapters
{
    public class CommentLoaderWrapper : Java.Lang.Object
    {
        public TextView Name { get; set; }
        public ImageView Image { get; set; }
        public TextView Question {get; set;}
    }
    public class CommentLoaderAdapter : BaseAdapter<PollViewModel>
    {
        private Activity context;
        private CommentLoader commentLoader;

        List<PollViewModel> friends = new List<PollViewModel>();
        public List<PollViewModel> Friends
        {
            get
            {
                return friends;
            }
            set
            {
                friends = value ?? new List<PollViewModel>();
                this.NotifyDataSetChanged();
            }
        }

        public CommentLoaderAdapter(Activity context, CommentLoader commentLoader)
        {
            this.commentLoader = commentLoader;
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
            CommentLoaderWrapper wrapper = null;
            var view = convertView;
            if (convertView == null)
            {
                view = context.LayoutInflater.Inflate(Resource.Layout.comment, null);
                wrapper = new CommentLoaderWrapper();
                wrapper.Name = view.FindViewById<TextView>(Resource.Id.name);
                wrapper.Image = view.FindViewById<ImageView>(Resource.Id.image);
                wrapper.Question = view.FindViewById<TextView>(Resource.Id.comment);
                view.Tag = wrapper;
            }
            else
            {
                wrapper = convertView.Tag as CommentLoaderWrapper;
            }

            var friend = friends[position];
            wrapper.Name.Text = friend.UserName;
            wrapper.Question.Text = friend.Title;

            commentLoader.DisplayComment(friend.Image, wrapper.Image, -1);

            return view;
        }

        public override PollViewModel this[int position]
        {
            get { return friends[position]; }
        }

        public override int Count
        {
            get { return friends.Count; }
        }

        public override long GetItemId(int position)
        {
            return position;
        }
    }
}