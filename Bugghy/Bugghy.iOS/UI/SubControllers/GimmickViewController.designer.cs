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
	[Register ("GimmickViewController")]
	partial class GimmickViewController
	{
		[Outlet]
		UIKit.UIButton AddButton { get; set; }

		[Outlet]
		UIKit.UIView ClosedLayout { get; set; }

		[Outlet]
		UIKit.UILabel ClosedNumberLabel { get; set; }

		[Outlet]
		UIKit.UIView DetailsLayout { get; set; }

		[Outlet]
		UIKit.UIView IssuesLayout { get; set; }

		[Outlet]
		UIKit.UILabel NameLabel { get; set; }

		[Outlet]
		UIKit.UIView OpenedLayout { get; set; }

		[Outlet]
		UIKit.UILabel OpenedNumberLabel { get; set; }

		[Outlet]
		UIKit.UILabel OwnerLabel { get; set; }

		[Outlet]
		UIKit.UIImageView ThumbImage { get; set; }

		[Outlet]
		UIKit.UIView ThumbLayout { get; set; }

		[Outlet]
		UIKit.UIButton ViewClosedButton { get; set; }

		[Outlet]
		UIKit.UIButton ViewOpenedButton { get; set; }

		[Outlet]
		UIKit.UIButton ViewWorkingButton { get; set; }

		[Outlet]
		UIKit.UIView WorkingLayout { get; set; }

		[Outlet]
		UIKit.UILabel WorkingNumberLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (DetailsLayout != null) {
				DetailsLayout.Dispose ();
				DetailsLayout = null;
			}

			if (ThumbLayout != null) {
				ThumbLayout.Dispose ();
				ThumbLayout = null;
			}

			if (ThumbImage != null) {
				ThumbImage.Dispose ();
				ThumbImage = null;
			}

			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}

			if (OwnerLabel != null) {
				OwnerLabel.Dispose ();
				OwnerLabel = null;
			}

			if (AddButton != null) {
				AddButton.Dispose ();
				AddButton = null;
			}

			if (IssuesLayout != null) {
				IssuesLayout.Dispose ();
				IssuesLayout = null;
			}

			if (OpenedLayout != null) {
				OpenedLayout.Dispose ();
				OpenedLayout = null;
			}

			if (OpenedNumberLabel != null) {
				OpenedNumberLabel.Dispose ();
				OpenedNumberLabel = null;
			}

			if (ViewOpenedButton != null) {
				ViewOpenedButton.Dispose ();
				ViewOpenedButton = null;
			}

			if (WorkingLayout != null) {
				WorkingLayout.Dispose ();
				WorkingLayout = null;
			}

			if (WorkingNumberLabel != null) {
				WorkingNumberLabel.Dispose ();
				WorkingNumberLabel = null;
			}

			if (ViewWorkingButton != null) {
				ViewWorkingButton.Dispose ();
				ViewWorkingButton = null;
			}

			if (ClosedLayout != null) {
				ClosedLayout.Dispose ();
				ClosedLayout = null;
			}

			if (ClosedNumberLabel != null) {
				ClosedNumberLabel.Dispose ();
				ClosedNumberLabel = null;
			}

			if (ViewClosedButton != null) {
				ViewClosedButton.Dispose ();
				ViewClosedButton = null;
			}
		}
	}
}
