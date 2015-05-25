using System;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

using TwitterCover;

namespace TwitterCoveriOSSample
{
    public partial class ScrollViewController : UIViewController
    {
        private UIScrollView scrollView;

        public ScrollViewController()
        {
            Title = "UIScrollView Demo";

            EdgesForExtendedLayout = UIRectEdge.None;
            AutomaticallyAdjustsScrollViewInsets = false;
        }

        protected override void Dispose(bool disposing)
        {
            scrollView.RemoveTwitterCover();

            base.Dispose(disposing);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            nfloat CoverViewHeight = 200;
            View.BackgroundColor = UIColor.White;

            scrollView = new UIScrollView(View.Bounds)
            {
                ContentSize = new CGSize(View.Bounds.Size.Width, 600)
            };
            scrollView.AddTwitterCover(UIImage.FromBundle("cover.png"), CoverViewHeight);
            View.AddSubview(scrollView);

            var label = new UILabel(new CGRect(20, CoverViewHeight, View.Bounds.Size.Width - 40, 600 - CoverViewHeight))
            {
                Lines = 0,
                Font = UIFont.SystemFontOfSize(22),
                Text = "TwitterCover is a parallax top view with real time blur effect to any UIScrollView, inspired by Twitter for iOS.\n\nCompletely created using UIKit framework.\n\nEasy to drop into your project.\n\nYou can add this feature to your own project, TwitterCover is easy-to-use."
            };

            scrollView.AddSubview(label);
        }
    }
}