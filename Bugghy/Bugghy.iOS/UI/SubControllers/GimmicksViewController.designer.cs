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
	[Register ("GimmicksViewController")]
	partial class GimmicksViewController
	{
		[Outlet]
		AdMaiora.AppKit.UI.UIItemListView GimmickList { get; set; }

		[Outlet]
		UIKit.UILabel NoItemsLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (NoItemsLabel != null) {
				NoItemsLabel.Dispose ();
				NoItemsLabel = null;
			}

			if (GimmickList != null) {
				GimmickList.Dispose ();
				GimmickList = null;
			}
		}
	}
}