// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace AdMaiora.Bugghy
{
	[Register ("IssuesViewController")]
	partial class IssuesViewController
	{
		[Outlet]
		UIKit.UIButton FilterClosedButton { get; set; }

		[Outlet]
		UIKit.UIView FilterLayout { get; set; }

		[Outlet]
		UIKit.UIButton FilterOpenedButton { get; set; }

		[Outlet]
		UIKit.UIButton FilterWorkingButton { get; set; }

		[Outlet]
		AdMaiora.AppKit.UI.UIItemListView IssueList { get; set; }

		[Outlet]
		UIKit.UILabel NoItemsLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (FilterLayout != null) {
				FilterLayout.Dispose ();
				FilterLayout = null;
			}

			if (FilterOpenedButton != null) {
				FilterOpenedButton.Dispose ();
				FilterOpenedButton = null;
			}

			if (FilterWorkingButton != null) {
				FilterWorkingButton.Dispose ();
				FilterWorkingButton = null;
			}

			if (FilterClosedButton != null) {
				FilterClosedButton.Dispose ();
				FilterClosedButton = null;
			}

			if (NoItemsLabel != null) {
				NoItemsLabel.Dispose ();
				NoItemsLabel = null;
			}

			if (IssueList != null) {
				IssueList.Dispose ();
				IssueList = null;
			}
		}
	}
}
