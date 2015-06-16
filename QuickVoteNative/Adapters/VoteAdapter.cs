using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Content;
using QuickVoteNative.Activities;
using Android.App;

namespace QuickVoteNative
{
    public class VoteAdapter : RecyclerView.Adapter//, View.IOnClickListener
    {
        public const string TAG = "VoteAdapter";
        private string[] dataSet;

        // Provide a reference to the type of views that you are using (custom ViewHolder)
        public class ViewHolder : RecyclerView.ViewHolder
        {
            private TextView textView;

            public TextView TextView
            {
                get{ return textView; }
            }

            public ViewHolder(View v, Action<int> listener)
                : base(v)
            {
                textView = (TextView)v.FindViewById(Resource.Id.textView);

                // Detect user clicks on the item view and report which item
                // was clicked (by position) to the listener:
                v.Click += (sender, e) => listener (base.Position);
            }


        }

        // Initialize the dataset of the Adapter
        public VoteAdapter(string[] dataSet)
        {
            this.dataSet = dataSet;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup viewGroup, int position)
        {
            View v = LayoutInflater.From(viewGroup.Context)
                .Inflate(Resource.Layout.item_cast_vote, viewGroup, false);
//            v.SetOnClickListener(this);
            ViewHolder vh = new ViewHolder(v, OnClick);

            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            Log.Debug(TAG, "\tElement " + position + " set.");

            // Get element from your dataset at this position and replace the contents of the view
            // with that element
            (viewHolder as ViewHolder).TextView.SetText(dataSet[position], TextView.BufferType.Normal);
        }

        // Return the size of your dataset (invoked by the layout manager)
        public override int ItemCount
        {
            get{ return dataSet.Length; }
        }

        // Event handler for item clicks:
        public event EventHandler<int> ItemClick;

        // Raise an event when the item-click takes place:
        void OnClick (int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);//dataSet[position]);
        }
    }
}

