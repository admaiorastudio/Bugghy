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
	[Register ("Registration0ViewController")]
	partial class Registration0ViewController
	{
		[Outlet]
		UIKit.UILabel EmailLabel { get; set; }

		[Outlet]
		UIKit.UITextField EmailText { get; set; }

		[Outlet]
		UIKit.UIImageView HeadImage { get; set; }

		[Outlet]
		UIKit.UIView InputLayout { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (HeadImage != null) {
				HeadImage.Dispose ();
				HeadImage = null;
			}

			if (EmailLabel != null) {
				EmailLabel.Dispose ();
				EmailLabel = null;
			}

			if (EmailText != null) {
				EmailText.Dispose ();
				EmailText = null;
			}

			if (InputLayout != null) {
				InputLayout.Dispose ();
				InputLayout = null;
			}
		}
	}
}
