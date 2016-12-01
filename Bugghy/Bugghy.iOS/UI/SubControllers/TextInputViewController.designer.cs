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
	[Register ("TextInputViewController")]
	partial class TextInputViewController
	{
		[Outlet]
		UIKit.UITextView InputText { get; set; }

		[Outlet]
		UIKit.UIImageView TitleImage { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (InputText != null) {
				InputText.Dispose ();
				InputText = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TitleImage != null) {
				TitleImage.Dispose ();
				TitleImage = null;
			}
		}
	}
}
