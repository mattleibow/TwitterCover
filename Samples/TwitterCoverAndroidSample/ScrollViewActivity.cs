using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;

using TwitterCover;

namespace TwitterCoverAndroidSample
{
    [Activity(Label = "TwitterCoverAndroidSample", MainLauncher = true, Icon = "@drawable/ic_launcher")]
    public class ScrollViewActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.ScrollViewLayout);

            var listView = FindViewById<TwitterCoverScrollView>(Resource.Id.scrollView);
            var headerView = FindViewById<FrameLayout>(Resource.Id.headerView);
            var showListView = FindViewById<Button>(Resource.Id.showListView);

            listView.SetHeaderImage(BitmapFactory.DecodeResource(Resources, Resource.Drawable.cover));
            listView.SetHeaderView(headerView);

            showListView.Click += delegate
            {
                StartActivity(typeof(ListViewActivity));
            };
        }
    }
}
