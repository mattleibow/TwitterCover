using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

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
            coverHeight = a.GetDimensionPixelSize(Resource.Styleable.TwitterCover_twitterCoverHeight, coverHeight);
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
            coverHeight = a.GetDimensionPixelSize(Resource.Styleable.TwitterCover_twitterCoverHeight, coverHeight);
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
}
