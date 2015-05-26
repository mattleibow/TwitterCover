using System.Linq;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Widget;

using TwitterCover;

namespace TwitterCoverAndroidSample
{
    [Activity(Label = "TwitterCoverAndroidSample", MainLauncher = true, Icon = "@drawable/ic_launcher")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.MainLayout);

            var listView = FindViewById<TwitterCoverListView>(Resource.Id.listView);
            listView.HeaderImage = BitmapFactory.DecodeResource(Resources, Resource.Drawable.cover);

            var items = Enumerable.Range(1, 20).Select(x => "Item " + x).ToArray();
            listView.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, items);
        }
    }
}
