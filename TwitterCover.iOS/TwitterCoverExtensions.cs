using System;
using System.Runtime.InteropServices;

using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace TwitterCover
{
    public static class TwitterCoverExtensions
    {
        private static NSObject coverViewKey = new NSObject();

        public static void AddTwitterCover(this UIScrollView scrollView, UIImage image, UIView topView)
        {
            var twitterCoverImageView = new TwitterCoverImageView(new CGRect(0, 0, 320, TwitterCoverImageView.CHTwitterCoverViewHeight), topView)
            {
                BackgroundColor = UIColor.Clear,
                Image = image,
                ScrollView = scrollView
            };
            scrollView.AddSubview(twitterCoverImageView);
            if (topView != null)
            {
                scrollView.AddSubview(topView);
            }
            scrollView.SetTwitterCoverView(twitterCoverImageView);
        }

        public static void AddTwitterCover(this UIScrollView scrollView, UIImage image)
        {
            scrollView.AddTwitterCover(image, null);
        }

        public static void RemoveTwitterCover(this UIScrollView scrollView)
        {
            scrollView.GetTwitterCoverView().RemoveFromSuperview();
            scrollView.SetTwitterCoverView(null);
        }

        public static TwitterCoverImageView GetTwitterCoverView(this UIScrollView scrollView)
        {
            var ptr = objc_getAssociatedObject(scrollView.Handle, coverViewKey.Handle);
            return (TwitterCoverImageView)Runtime.GetNSObject(ptr);
        }

        public static void SetTwitterCoverView(this UIScrollView scrollView, TwitterCoverImageView coverView)
        {
            objc_setAssociatedObject(scrollView.Handle, coverViewKey.Handle, coverView.Handle, AssociationPolicy.RETAIN);
        }

        [DllImport("/usr/lib/libobjc.dylib")]
        private static extern IntPtr objc_getAssociatedObject(IntPtr @object, IntPtr key);

        [DllImport("/usr/lib/libobjc.dylib")]
        private static extern void objc_setAssociatedObject(IntPtr @object, IntPtr key, IntPtr value, AssociationPolicy policy);

        private enum AssociationPolicy
        {
            ASSIGN,
            RETAIN_NONATOMIC,
            COPY_NONATOMIC = 3,
            RETAIN = 1401,
            COPY = 1403
        }
    }
}