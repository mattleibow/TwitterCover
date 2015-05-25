using System;
using System.Drawing;

using CoreGraphics;
using Foundation;
using UIKit;

using TwitterCover;

namespace TwitterCoveriOSSample
{
    public partial class TableViewController : UITableViewController
    {
        private UIView topView;

        public TableViewController()
        {
            Title = "UITableView Demo";

            EdgesForExtendedLayout = UIRectEdge.None;
            AutomaticallyAdjustsScrollViewInsets = false;
        }

        public TableViewController(UIView topView)
            : this()
        {
            this.topView = topView;
        }

        protected override void Dispose(bool disposing)
        {
            TableView.RemoveTwitterCover();

            base.Dispose(disposing);
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            TableView.AddTwitterCover(UIImage.FromBundle("cover.png"), topView);
            TableView.TableHeaderView = new UIView(new CGRect(0, 0, 320, TwitterCoverImageView.CHTwitterCoverViewHeight + topView.Bounds.Size.Height));
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return 20;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = TableView.DequeueReusableCell("Cell");
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, "Cell");
            }
            cell.TextLabel.Text = "Cell " + (indexPath.Row + 1);
            return cell;
        }
    }
}