using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views.Animations;
using Android.Widget;
using Android.Views;
using Android.Support.V8.Renderscript;
using Android.Content.Res;

namespace TwitterCover
{
    public class TwitterCoverListView : ListView
    {
        private TwitterCoverImplementor implementor;

        public TwitterCoverListView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            // get the default
            var coverHeight = context.Resources.GetDimensionPixelSize(Resource.Dimension.twitterCover_default_coverHeight);
            // get the default
            var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.TwitterCover, defStyle, 0);
            coverHeight = a.GetDimensionPixelSize(Resource.Styleable.TwitterCover_coverHeight, coverHeight);
            a.Recycle();

            Init(context, coverHeight);
        }

        public TwitterCoverListView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            // get the default
            var coverHeight = context.Resources.GetDimensionPixelSize(Resource.Dimension.twitterCover_default_coverHeight);
            // get the local
            var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.TwitterCover);
            coverHeight = a.GetDimensionPixelSize(Resource.Styleable.TwitterCover_coverHeight, coverHeight);
            a.Recycle();

            Init(context, coverHeight);
        }

        public TwitterCoverListView(Context context, int coverHeight)
            : base(context)
        {
            Init(context, coverHeight);
        }

        public TwitterCoverListView(Context context)
            : base(context)
        {
            // get the default
            var coverHeight = context.Resources.GetDimensionPixelSize(Resource.Dimension.twitterCover_default_coverHeight);

            Init(context, coverHeight);
        }

        private void Init(Context context, int coverHeight)
        {
            OverScrollMode = OverScrollMode.Never;
            
            implementor = new TwitterCoverImplementor(context, coverHeight);

            var header = new RelativeLayout(context);
            header.LayoutParameters = new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            header.AddView(implementor.CoverContainer);
            AddHeaderView(header);
        }

        public virtual void SetHeaderImage(Bitmap value)
        {
            implementor.SetHeaderImage(value);
        }

        protected override void OnScrollChanged(int l, int t, int oldl, int oldt)
        {
            base.OnScrollChanged(l, t, oldl, oldt);

            implementor.HandleScrollChanged();
        }

        protected override bool OverScrollBy(int deltaX, int deltaY, int scrollX, int scrollY, int scrollRangeX, int scrollRangeY, int maxOverScrollX, int maxOverScrollY, bool isTouchEvent)
        {
            implementor.HandleOverScrollBy(deltaY, isTouchEvent);
            
            return base.OverScrollBy(deltaX, deltaY, scrollX, scrollY, scrollRangeX, scrollRangeY, maxOverScrollX, maxOverScrollY, isTouchEvent);
        }

        public override bool OnTouchEvent(MotionEvent ev)
        {
            if (ev.Action == MotionEventActions.Up)
            {
                implementor.HandleRelease();
            }
            return base.OnTouchEvent(ev);
        }
    }

    public class TwitterCoverImplementor
    {
        private FrameLayout mCoverContainer;
        private ImageView mCoverView;
        private ImageView mCoverMaskView;
        private Context mContext;
        private int mCoverImageViewMaxHeight;
        private int mCoverImageViewHeight;

        public TwitterCoverImplementor(Context context, int coverHeight)
        {
            mContext = context;

            mCoverImageViewHeight = coverHeight;
            mCoverImageViewMaxHeight = coverHeight * 2;

            mCoverContainer = new FrameLayout(context);
            mCoverContainer.LayoutParameters = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, mCoverImageViewHeight);

            mCoverView = new ImageView(context);
            mCoverView.LayoutParameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            mCoverView.SetScaleType(ImageView.ScaleType.CenterCrop);
            mCoverContainer.AddView(mCoverView);

            mCoverMaskView = new ImageView(context);
            mCoverMaskView.LayoutParameters = new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            mCoverMaskView.SetScaleType(ImageView.ScaleType.CenterCrop);
            mCoverContainer.AddView(mCoverMaskView);
        }

        public View CoverContainer
        {
            get { return mCoverContainer; }
        }

        public virtual void SetHeaderImage(Bitmap value)
        {
                mCoverView.SetImageBitmap(value);
                mCoverMaskView.SetImageBitmap(value);
                mCoverMaskView.Alpha = 0;

                mCoverMaskView.SetImageBitmap(RenderScriptBlur(value, 25));
        }

        public virtual void HandleScrollChanged()
        {
            int containerHeight = mCoverContainer.Height;
            int diff = containerHeight - mCoverImageViewHeight;
            if (diff > 0)
            {
                double factor = (diff + (double)mCoverImageViewHeight / 10.0) * 2 / (double)(mCoverImageViewMaxHeight - mCoverImageViewHeight);
                mCoverMaskView.Alpha = Math.Max(0, Math.Min((int)(factor * 255), 255));

            }
            else if (mCoverMaskView.Alpha != 0)
            {
                mCoverMaskView.Alpha = 0;
            }
        }

        public virtual void HandleOverScrollBy(int deltaY, bool isTouchEvent)
        {
            if (mCoverContainer.Height <= mCoverImageViewMaxHeight && isTouchEvent)
            {
                int destImageViewHeight = mCoverContainer.Height - deltaY / 2;

                mCoverContainer.LayoutParameters.Height = Math.Min(mCoverImageViewMaxHeight, Math.Max(destImageViewHeight, mCoverImageViewHeight));
                mCoverContainer.RequestLayout();
            }
        }

        public virtual void HandleRelease()
        {
                if (mCoverImageViewHeight < mCoverContainer.Height)
                {
                    var animation = new ReleaseAnimimation(mCoverContainer, mCoverImageViewHeight);
                    animation.Duration = 300;

                    mCoverContainer.StartAnimation(animation);
                }
        }

        public virtual Bitmap RenderScriptBlur(Bitmap bitmap, int radius)
        {
            var outBitmap = Bitmap.CreateBitmap(bitmap.Width, bitmap.Height, Bitmap.Config.Argb8888);

            var rs = RenderScript.Create(mContext);
            var blurScript = ScriptIntrinsicBlur.Create(rs, Element.U8_4(rs));
            var allIn = Allocation.CreateFromBitmap(rs, bitmap);
            var allOut = Allocation.CreateFromBitmap(rs, outBitmap);
            blurScript.SetRadius(radius);
            blurScript.SetInput(allIn);
            blurScript.ForEach(allOut);
            allOut.CopyTo(outBitmap);
            rs.Destroy();

            return outBitmap;
        }

        public virtual Bitmap StackBlur(Bitmap sentBitmap, int radius)
        {
            // Stack Blur v1.0 from
            // http://www.quasimondo.com/StackBlurForCanvas/StackBlurDemo.html
            //
            // Java Author: Mario Klingemann <mario at quasimondo.com>
            // http://incubator.quasimondo.com
            // created Feburary 29, 2004
            // Android port : Yahel Bouaziz <yahel at kayenko.com>
            // http://www.kayenko.com
            // ported april 5th, 2012

            // This is a compromise between Gaussian Blur and Box blur
            // It creates much better looking blurs than Box Blur, but is
            // 7x faster than my Gaussian Blur implementation.
            //
            // I called it Stack Blur because this describes best how this
            // filter works internally: it creates a kind of moving stack
            // of colors whilst scanning through the image. Thereby it
            // just has to add one new block of color to the right side
            // of the stack and remove the leftmost color. The remaining
            // colors on the topmost layer of the stack are either added on
            // or reduced by one, depending on if they are on the right or
            // on the left side of the stack.
            //
            // If you are using this algorithm in your code please add
            // the following line:
            //
            // Stack Blur Algorithm by Mario Klingemann <mario@quasimondo.com>

            Bitmap bitmap = sentBitmap.Copy(sentBitmap.GetConfig(), true);

            if (radius < 1)
            {
                return (null);
            }

            int w = bitmap.Width;
            int h = bitmap.Height;

            int[] pix = new int[w * h];
            bitmap.GetPixels(pix, 0, w, 0, 0, w, h);

            int wm = w - 1;
            int hm = h - 1;
            int wh = w * h;
            int div = radius + radius + 1;

            int[] r = new int[wh];
            int[] g = new int[wh];
            int[] b = new int[wh];
            int rsum, gsum, bsum, x, y, i, p, yp, yi, yw;
            int[] vmin = new int[Math.Max(w, h)];

            int divsum = (div + 1) >> 1;
            divsum *= divsum;
            int[] dv = new int[256 * divsum];
            for (i = 0; i < 256 * divsum; i++)
            {
                dv[i] = (i / divsum);
            }

            yw = yi = 0;

            //JAVA TO C# CONVERTER NOTE: The following call to the 'RectangularArrays' helper class reproduces the rectangular array initialization that is automatic in Java:
            //ORIGINAL LINE: int[][] stack = new int[div][3];
            int[][] stack = ReturnRectangularIntArray(div, 3);
            int stackpointer;
            int stackstart;
            int[] sir;
            int rbs;
            int r1 = radius + 1;
            int routsum, goutsum, boutsum;
            int rinsum, ginsum, binsum;

            for (y = 0; y < h; y++)
            {
                rinsum = ginsum = binsum = routsum = goutsum = boutsum = rsum = gsum = bsum = 0;
                for (i = -radius; i <= radius; i++)
                {
                    p = pix[yi + Math.Min(wm, Math.Max(i, 0))];
                    sir = stack[i + radius];
                    sir[0] = (p & 0xff0000) >> 16;
                    sir[1] = (p & 0x00ff00) >> 8;
                    sir[2] = (p & 0x0000ff);
                    rbs = r1 - Math.Abs(i);
                    rsum += sir[0] * rbs;
                    gsum += sir[1] * rbs;
                    bsum += sir[2] * rbs;
                    if (i > 0)
                    {
                        rinsum += sir[0];
                        ginsum += sir[1];
                        binsum += sir[2];
                    }
                    else
                    {
                        routsum += sir[0];
                        goutsum += sir[1];
                        boutsum += sir[2];
                    }
                }
                stackpointer = radius;

                for (x = 0; x < w; x++)
                {

                    r[yi] = dv[rsum];
                    g[yi] = dv[gsum];
                    b[yi] = dv[bsum];

                    rsum -= routsum;
                    gsum -= goutsum;
                    bsum -= boutsum;

                    stackstart = stackpointer - radius + div;
                    sir = stack[stackstart % div];

                    routsum -= sir[0];
                    goutsum -= sir[1];
                    boutsum -= sir[2];

                    if (y == 0)
                    {
                        vmin[x] = Math.Min(x + radius + 1, wm);
                    }
                    p = pix[yw + vmin[x]];

                    sir[0] = (p & 0xff0000) >> 16;
                    sir[1] = (p & 0x00ff00) >> 8;
                    sir[2] = (p & 0x0000ff);

                    rinsum += sir[0];
                    ginsum += sir[1];
                    binsum += sir[2];

                    rsum += rinsum;
                    gsum += ginsum;
                    bsum += binsum;

                    stackpointer = (stackpointer + 1) % div;
                    sir = stack[(stackpointer) % div];

                    routsum += sir[0];
                    goutsum += sir[1];
                    boutsum += sir[2];

                    rinsum -= sir[0];
                    ginsum -= sir[1];
                    binsum -= sir[2];

                    yi++;
                }
                yw += w;
            }
            for (x = 0; x < w; x++)
            {
                rinsum = ginsum = binsum = routsum = goutsum = boutsum = rsum = gsum = bsum = 0;
                yp = -radius * w;
                for (i = -radius; i <= radius; i++)
                {
                    yi = Math.Max(0, yp) + x;

                    sir = stack[i + radius];

                    sir[0] = r[yi];
                    sir[1] = g[yi];
                    sir[2] = b[yi];

                    rbs = r1 - Math.Abs(i);

                    rsum += r[yi] * rbs;
                    gsum += g[yi] * rbs;
                    bsum += b[yi] * rbs;

                    if (i > 0)
                    {
                        rinsum += sir[0];
                        ginsum += sir[1];
                        binsum += sir[2];
                    }
                    else
                    {
                        routsum += sir[0];
                        goutsum += sir[1];
                        boutsum += sir[2];
                    }

                    if (i < hm)
                    {
                        yp += w;
                    }
                }
                yi = x;
                stackpointer = radius;
                for (y = 0; y < h; y++)
                {
                    // Preserve alpha channel: ( 0xff000000 & pix[yi] )
                    pix[yi] = (unchecked((int)0xff000000) & pix[yi]) | (dv[rsum] << 16) | (dv[gsum] << 8) | dv[bsum];

                    rsum -= routsum;
                    gsum -= goutsum;
                    bsum -= boutsum;

                    stackstart = stackpointer - radius + div;
                    sir = stack[stackstart % div];

                    routsum -= sir[0];
                    goutsum -= sir[1];
                    boutsum -= sir[2];

                    if (x == 0)
                    {
                        vmin[y] = Math.Min(y + r1, hm) * w;
                    }
                    p = x + vmin[y];

                    sir[0] = r[p];
                    sir[1] = g[p];
                    sir[2] = b[p];

                    rinsum += sir[0];
                    ginsum += sir[1];
                    binsum += sir[2];

                    rsum += rinsum;
                    gsum += ginsum;
                    bsum += binsum;

                    stackpointer = (stackpointer + 1) % div;
                    sir = stack[stackpointer];

                    routsum += sir[0];
                    goutsum += sir[1];
                    boutsum += sir[2];

                    rinsum -= sir[0];
                    ginsum -= sir[1];
                    binsum -= sir[2];

                    yi += w;
                }
            }

            bitmap.SetPixels(pix, 0, w, 0, 0, w, h);

            return (bitmap);
        }

        public class ReleaseAnimimation : Animation
        {
            private readonly View mView;
            private readonly int mTargetHeight;
            private readonly int mExtraHeight;

            protected internal ReleaseAnimimation(View view, int targetHeight)
            {
                mView = view;
                mTargetHeight = targetHeight;
                mExtraHeight = mTargetHeight - view.Height;
            }

            protected override void ApplyTransformation(float interpolatedTime, Transformation t)
            {
                mView.LayoutParameters.Height = (int)(mTargetHeight - mExtraHeight * (1 - interpolatedTime));
                mView.RequestLayout();
            }
        }

        internal static int[][] ReturnRectangularIntArray(int size1, int size2)
        {
            int[][] newArray;
            if (size1 > -1)
            {
                newArray = new int[size1][];
                if (size2 > -1)
                {
                    for (int array1 = 0; array1 < size1; array1++)
                    {
                        newArray[array1] = new int[size2];
                    }
                }
            }
            else
                newArray = null;

            return newArray;
        }
    }
}
