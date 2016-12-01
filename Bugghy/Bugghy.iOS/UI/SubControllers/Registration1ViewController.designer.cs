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
	[Register ("Registration1ViewController")]
	partial class Registration1ViewController
	{
		[Outlet]
		UIKit.UIImageView HeadImage { get; set; }

		[Outlet]
		UIKit.UIView InputLayout { get; set; }

		[Outlet]
		UIKit.UILabel PasswordLabel { get; set; }

		[Outlet]
		UIKit.UITextField PasswordText { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (HeadImage != null) {
				HeadImage.Dispose ();
				HeadImage = null;
			}

			if (InputLayout != null) {
				InputLayout.Dispose ();
				InputLayout = null;
			}

			if (PasswordLabel != null) {
				PasswordLabel.Dispose ();
				PasswordLabel = null;
			}

			if (PasswordText != null) {
				PasswordText.Dispose ();
				PasswordText = null;
			}
		}
	}
}
