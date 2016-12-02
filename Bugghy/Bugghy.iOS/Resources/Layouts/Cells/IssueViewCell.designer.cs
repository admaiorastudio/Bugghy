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
	[Register ("IssueViewCell")]
	partial class IssueViewCell
	{
		[Outlet]
		UIKit.UIView CalloutLayout { get; set; }

		[Outlet]
		UIKit.UILabel CodeLabel { get; set; }

		[Outlet]
		UIKit.UIView ContentLayout { get; set; }

		[Outlet]
		UIKit.UIView DetailsLayout { get; set; }

		[Outlet]
		UIKit.UIImageView TypeImage { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CalloutLayout != null) {
				CalloutLayout.Dispose ();
				CalloutLayout = null;
			}

			if (ContentLayout != null) {
				ContentLayout.Dispose ();
				ContentLayout = null;
			}

			if (TypeImage != null) {
				TypeImage.Dispose ();
				TypeImage = null;
			}

			if (DetailsLayout != null) {
				DetailsLayout.Dispose ();
				DetailsLayout = null;
			}

			if (CodeLabel != null) {
				CodeLabel.Dispose ();
				CodeLabel = null;
			}
		}
	}
}
