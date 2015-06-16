using System;
using System.Drawing;

using Foundation;
using UIKit;

using FlyoutNavigation;
using MonoTouch.Dialog;
using System.Linq;

namespace QuickVote.iOS
{
    public partial class MainViewController : UIViewController
    {
        FlyoutNavigationController navigation;

        // Data we'll use to create our flyout menu and views:
        string[] Tasks =
        {
            "My account",
            "Vote!",
            "Logout"
        };

        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        public MainViewController()
            : base(UserInterfaceIdiomIsPhone ? "MainViewController_iPhone" : "MainViewController_iPad", null)
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();
			
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
			
            // Perform any additional setup after loading the view, typically from a nib.
            // Create the flyout view controller, make it large,
            // and add it as a subview:
            navigation = new FlyoutNavigationController();
            navigation.Position = FlyOutNavigationPosition.Left;
            navigation.View.Frame = UIScreen.MainScreen.Bounds;
            View.AddSubview(navigation.View);
            this.AddChildViewController(navigation);

            // Create the menu:
            navigation.NavigationRoot = new RootElement("QuickVote")
            {
                new Section("Task List")
                {
                    from page in Tasks
                                   select new StringElement(page) as Element
                }
            };

            // Create an array of UINavigationControllers that correspond to your
            // menu items:
            navigation.ViewControllers = Array.ConvertAll(Tasks, title =>
                new UINavigationController(new TabController(navigation, title))
            );
        }

        public class TabController : UITabBarController
        {

            UIViewController tab1, tab2, tab3;

            public TabController(FlyoutNavigationController navigation, string title)
            {
//                var hamburger = UIImage.FromFile("hamburger.png");
//                var button = UIButton.FromType(UIButtonType.Custom);
//                button.SetBackgroundImage(hamburger, UIControlState.Normal);
//                button.Frame = new RectangleF(0, 0, hamburger.Size.Width, image.Size.Height);

                string[] list = new string[]{"Jelly", "Bean", "Caramel"};
                NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIBarButtonSystemItem.Action, delegate
                    {
                        navigation.ToggleMenu();
                    });

                tab1 = new UIViewController();
                tab1.Add(new UITableView(View.Bounds));
                tab1.Title = "New Poll";
//                tab1.TabBarItem = new UITabBarItem(UITabBarSystemItem.More, 0);
                tab1.TabBarItem.Image = UIImage.FromFile ("newPoll.png");
                tab1.View.BackgroundColor = UIColor.White;

                tab2 = new UIViewController();
                //          tab2.Title = "Orange";
                tab2.TabBarItem = new UITabBarItem();
                tab2.Add(new UITableView(View.Bounds));
                tab2.TabBarItem.Image = UIImage.FromFile ("community.png");
//                tab2.TabBarItem = new UITabBarItem(UITabBarSystemItem.Contacts, 1);
                tab2.TabBarItem.Title = "Community";
//                tab2.View.BackgroundColor = UIColor.Orange;

                tab3 = new UIViewController();
                tab3.Title = "My Polls";
                tab3.Add(new UITableView(View.Bounds));
//                tab3.View.BackgroundColor = UIColor.Red;
                tab3.TabBarItem.BadgeValue = "1";
//                tab3.TabBarItem = new UITabBarItem(UITabBarSystemItem.History, 2);
                tab3.TabBarItem.Image = UIImage.FromFile ("myPolls.png");

                var tabs = new UIViewController[]
                {
                    tab1, tab2, tab3
                };

                ViewControllers = tabs;
            }
        }

    }
}

