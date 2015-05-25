using System;
using System.Collections.Generic;

using CoreGraphics;
using CoreImage;
using Foundation;
using UIKit;

namespace TwitterCover
{
    public class TwitterCoverImageView : UIImageView
    {
        public const int CHTwitterCoverViewHeight = 200;

        private UIScrollView scrollView;
        private UIView topView;
        private List<UIImage> blurredImages;

        public TwitterCoverImageView(CGRect frame)
            : this(frame, null)
        {
        }

        public TwitterCoverImageView(CGRect frame, UIView topView)
            : base(frame)
        {
            this.topView = topView;

            blurredImages = new List<UIImage>(20);
            ContentMode = UIViewContentMode.ScaleAspectFill;
            ClipsToBounds = true;
        }

        public UIScrollView ScrollView
        {
            get { return scrollView; }
            set
            {
                if (ScrollView != null)
                {
                    ScrollView.RemoveObserver(this, "contentOffset");
                }
                scrollView = value;
                ScrollView.AddObserver(this, "contentOffset", NSKeyValueObservingOptions.New, IntPtr.Zero);
            }
        }

        public override UIImage Image
        {
            get { return base.Image; }
            set
            {
                base.Image = value;
                PrepareForBlurImages();
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (ScrollView.ContentOffset.Y < 0)
            {
                var nfloat = -ScrollView.ContentOffset.Y;

                topView.Frame = new CGRect(0, -nfloat, 320, topView.Bounds.Size.Height);
                Frame = new CGRect(-nfloat, -nfloat + topView.Bounds.Size.Height, 320 + nfloat * 2, 200 + nfloat);

                int num = (int)nfloat / 10;
                if (num < 0)
                {
                    num = 0;
                }
                else if (num >= blurredImages.Count)
                {
                    num = blurredImages.Count - 1;
                }
                var uIImage = blurredImages[num];
                if (Image != uIImage)
                {
                    base.Image = uIImage;
                }
            }
            else
            {
                topView.Frame = new CGRect(0, 0, 320, topView.Bounds.Size.Height);
                Frame = new CGRect(0, topView.Bounds.Size.Height, 320, 200);

                var uIImage = blurredImages[0];
                if (Image != uIImage)
                {
                    base.Image = uIImage;
                }
            }
        }

        public override void RemoveFromSuperview()
        {
            ScrollView.RemoveObserver(this, "contentOffset");
            topView.RemoveFromSuperview();
            base.RemoveFromSuperview();
        }

        public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
        {
            SetNeedsLayout();
        }

        private void PrepareForBlurImages()
        {
            blurredImages.Clear();
            float num = 0.1f;
            blurredImages.Add(Image);
            for (int i = 0; i < 20; i++)
            {
                blurredImages.Add(Blur(Image, num));
                num += 0.04f;
            }
        }

        private UIImage Blur(UIImage image, float blurRadius)
        {
            UIImage result = null;
            if (image != null)
            {
                using (var imageClone = new CIImage(image))
                using (var blur = new CIGaussianBlur())
                {
                    blur.Image = imageClone;
                    blur.Radius = blurRadius;

                    using (var outputImage = blur.OutputImage)
                    using (var context = CIContext.FromOptions(new CIContextOptions { UseSoftwareRenderer = false }))
                    using (var cgImage = context.CreateCGImage(outputImage, new CGRect(new CGPoint(0, 0), image.Size)))
                    {
                        result = UIImage.FromImage(cgImage);
                    }
                }
            }
            return result;
        }
    }
}
