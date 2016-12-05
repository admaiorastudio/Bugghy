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
		public UIKit.UIView CalloutLayout { get; set; }

		[Outlet]
        public UIKit.UILabel CodeLabel { get; set; }

		[Outlet]
        public UIKit.UIView ContentLayout { get; set; }

		[Outlet]
        public UIKit.UILabel CreatedDateLabel { get; set; }

		[Outlet]
        public UIKit.UILabel CreatedLabel { get; set; }

		[Outlet]
        public UIKit.UILabel DescriptionLabel { get; set; }

		[Outlet]
        public UIKit.UIView DetailsLayout { get; set; }

		[Outlet]
        public UIKit.UILabel OfLabel { get; set; }

		[Outlet]
        public UIKit.UILabel SenderLabel { get; set; }

		[Outlet]
        public UIKit.UILabel StatusDescriptionLabel { get; set; }

		[Outlet]
        public UIKit.UILabel StatusLabel { get; set; }

		[Outlet]
        public UIKit.UILabel TitleLabel { get; set; }

		[Outlet]
        public UIKit.UIImageView TypeImage { get; set; }
		
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

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (OfLabel != null) {
				OfLabel.Dispose ();
				OfLabel = null;
			}

			if (SenderLabel != null) {
				SenderLabel.Dispose ();
				SenderLabel = null;
			}

			if (DescriptionLabel != null) {
				DescriptionLabel.Dispose ();
				DescriptionLabel = null;
			}

			if (CreatedLabel != null) {
				CreatedLabel.Dispose ();
				CreatedLabel = null;
			}

			if (CreatedDateLabel != null) {
				CreatedDateLabel.Dispose ();
				CreatedDateLabel = null;
			}

			if (StatusLabel != null) {
				StatusLabel.Dispose ();
				StatusLabel = null;
			}

			if (StatusDescriptionLabel != null) {
				StatusDescriptionLabel.Dispose ();
				StatusDescriptionLabel = null;
			}
		}
	}
}
