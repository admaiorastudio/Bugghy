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
	[Register ("IssueViewController")]
	partial class IssueViewController
	{
		[Outlet]
		UIKit.UILabel CodeLabel { get; set; }

		[Outlet]
		UIKit.UIView DescriptionLayout { get; set; }

		[Outlet]
		UIKit.UITextView DescriptionText { get; set; }

		[Outlet]
		UIKit.UIButton EditButton { get; set; }

		[Outlet]
		UIKit.UIView HeaderLayout { get; set; }

		[Outlet]
		UIKit.UIImageView ShadowBottomImage { get; set; }

		[Outlet]
		UIKit.UIImageView ShadowTopImage { get; set; }

		[Outlet]
		UIKit.UILabel TapToChangeTypeLabel { get; set; }

		[Outlet]
		UIKit.UIView TitleLineView { get; set; }

		[Outlet]
		UIKit.UITextField TitleText { get; set; }

		[Outlet]
		UIKit.UIImageView TypeImage { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (HeaderLayout != null) {
				HeaderLayout.Dispose ();
				HeaderLayout = null;
			}

			if (TypeImage != null) {
				TypeImage.Dispose ();
				TypeImage = null;
			}

			if (TapToChangeTypeLabel != null) {
				TapToChangeTypeLabel.Dispose ();
				TapToChangeTypeLabel = null;
			}

			if (CodeLabel != null) {
				CodeLabel.Dispose ();
				CodeLabel = null;
			}

			if (TitleText != null) {
				TitleText.Dispose ();
				TitleText = null;
			}

			if (TitleLineView != null) {
				TitleLineView.Dispose ();
				TitleLineView = null;
			}

			if (DescriptionLayout != null) {
				DescriptionLayout.Dispose ();
				DescriptionLayout = null;
			}

			if (DescriptionText != null) {
				DescriptionText.Dispose ();
				DescriptionText = null;
			}

			if (ShadowTopImage != null) {
				ShadowTopImage.Dispose ();
				ShadowTopImage = null;
			}

			if (ShadowBottomImage != null) {
				ShadowBottomImage.Dispose ();
				ShadowBottomImage = null;
			}

			if (EditButton != null) {
				EditButton.Dispose ();
				EditButton = null;
			}
		}
	}
}
