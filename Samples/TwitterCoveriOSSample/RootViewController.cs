using System;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

namespace TwitterCoveriOSSample
{
    public partial class RootViewController : UITableViewController
    {
        public RootViewController()
        {
            Title = "TwitterCover Demos";
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 3;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = TableView.DequeueReusableCell("Cell");
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, "Cell");
            }
            var label = cell.TextLabel;
            label.Font = UIFont.SystemFontOfSize(15);
            switch (indexPath.Row)
            {
                case 0:
                    label.Text = "UIScrollView Demo";
                    break;
                case 1:
                    label.Text = "UITableView Demo";
                    break;
                case 2:
                    label.Text = "UITableView with Top offset Demo";
                    break;
                default:
                    break;
            }
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            TableView.DeselectRow(indexPath, true);
            switch (indexPath.Row)
            {
                case 0:
                    NavigationController.PushViewController(new ScrollViewController(), true);
                    break;
                case 1:
                    NavigationController.PushViewController(new TableViewController(), true);
                    break;
                case 2:
                    var label = new UILabel()
                    {
                        Frame = new CGRect(0, 0, 320, 100),
                        BackgroundColor = UIColor.Clear,
                        Lines = 0,
                        Font = UIFont.BoldSystemFontOfSize(20),
                        Text = "This is a header view, This is a header view, This is a header view, This is a header view."
                    };
                    NavigationController.PushViewController(new TableViewController(label), true);
                    break;
                default:
                    break;
            }
        }
    }
}