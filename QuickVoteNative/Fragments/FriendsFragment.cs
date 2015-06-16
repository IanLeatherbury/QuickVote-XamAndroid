using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;

using QuickVoteNative.Adapters;

using com.refractored;
using Android.Graphics;

namespace QuickVoteNative.Fragments
{
    public class FriendsFragment : Fragment
    {
        private ViewPager m_ViewPager;
        private PagerSlidingTabStrip m_PageIndicator;
        private FragmentPagerAdapter m_Adapter;

        public FriendsFragment()
        {
            this.RetainInstance = true;
        }

        public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Bundle savedInstanceState)
        {
            var ignored = base.OnCreateView(inflater, container, savedInstanceState);
            var view = inflater.Inflate(Resource.Layout.fragment_friends, null);

            // Create your application here
            m_ViewPager = view.FindViewById<ViewPager>(Resource.Id.viewPager);
            m_ViewPager.OffscreenPageLimit = 1;
            m_PageIndicator = view.FindViewById<PagerSlidingTabStrip>(Resource.Id.tabs);
            m_PageIndicator.TabTextColor = Android.Content.Res.ColorStateList.ValueOf(Color.White);
			m_PageIndicator.UnderlineColor = Resource.Color.white;
			m_PageIndicator.TabTextColorSelected = Android.Content.Res.ColorStateList.ValueOf(Color.White);

            //Since we are a fragment in a fragment you need to pass down the child fragment manager!
            m_Adapter = new SectionAdapter(this.ChildFragmentManager);

            m_ViewPager.Adapter = this.m_Adapter;

            m_PageIndicator.SetViewPager(this.m_ViewPager);
            return view;
        }

    }
}