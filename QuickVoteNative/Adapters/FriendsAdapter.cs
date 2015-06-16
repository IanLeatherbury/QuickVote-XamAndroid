using System;
using Android.Support.V4.App;
using QuickVoteNative.Fragments; 

namespace QuickVoteNative.Adapters
{
    class SectionAdapter: FragmentPagerAdapter
    {
        private static readonly string[] Content = new[]
            {
                "Community", "New Poll", "My Polls"
            };

        public SectionAdapter(FragmentManager p0) 
            : base(p0) 
        { }

        public override int Count
        {
            get { return Content.Length; }
        }

        public override Fragment GetItem(int index)
        {
            switch (index)
            {
                case 0:
                    return new CommunityFragment();
                case 1:
                    return new  NewPollFragment();
            }

            return new MyPollsFragment();
        }

        public override Java.Lang.ICharSequence GetPageTitleFormatted(int p0) { return new Java.Lang.String(Content[p0 % Content.Length].ToUpper()); }
    }
}

