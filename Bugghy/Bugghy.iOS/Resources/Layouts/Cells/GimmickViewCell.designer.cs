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
	[Register ("GimmickViewCell")]
	partial class GimmickViewCell
	{
		[Outlet]
		public UIKit.UIView CalloutLayout { get; set; }

		[Outlet]
        public UIKit.UIView ContentLayout { get; set; }

		[Outlet]
        public UIKit.UIView DetailsLayout { get; set; }

		[Outlet]
        public UIKit.UILabel NameLabel { get; set; }

		[Outlet]
        public UIKit.UILabel OfLabel { get; set; }

		[Outlet]
        public UIKit.UILabel OwnerLabel { get; set; }

		[Outlet]
        public UIKit.UIImageView ThumbImage { get; set; }
		
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

			if (ThumbImage != null) {
				ThumbImage.Dispose ();
				ThumbImage = null;
			}

			if (DetailsLayout != null) {
				DetailsLayout.Dispose ();
				DetailsLayout = null;
			}

			if (NameLabel != null) {
				NameLabel.Dispose ();
				NameLabel = null;
			}

			if (OfLabel != null) {
				OfLabel.Dispose ();
				OfLabel = null;
			}

			if (OwnerLabel != null) {
				OwnerLabel.Dispose ();
				OwnerLabel = null;
			}
		}
	}
}
