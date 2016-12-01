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
	[Register ("ChatViewController")]
	partial class ChatViewController
	{
		[Outlet]
		UIKit.UIImageView ArrowImage { get; set; }

		[Outlet]
		UIKit.UIView BackLayout { get; set; }

		[Outlet]
		UIKit.UIImageView CalloutImage { get; set; }

		[Outlet]
		UIKit.UIView DetailsLayout { get; set; }

		[Outlet]
		UIKit.UIView HeaderLayout { get; set; }

		[Outlet]
		UIKit.UIView InputLayout { get; set; }

		[Outlet]
		AdMaiora.AppKit.UI.UIItemListView MessageList { get; set; }

		[Outlet]
		UIKit.UITextView MessageText { get; set; }

		[Outlet]
		UIKit.UILabel NoMessagesLabel { get; set; }

		[Outlet]
		UIKit.UIButton SendButton { get; set; }

		[Outlet]
		UIKit.UILabel StatusLabel { get; set; }

		[Outlet]
		UIKit.UILabel TitleLabel { get; set; }

		[Outlet]
		UIKit.UIImageView TypeImage { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (ArrowImage != null) {
				ArrowImage.Dispose ();
				ArrowImage = null;
			}

			if (BackLayout != null) {
				BackLayout.Dispose ();
				BackLayout = null;
			}

			if (CalloutImage != null) {
				CalloutImage.Dispose ();
				CalloutImage = null;
			}

			if (DetailsLayout != null) {
				DetailsLayout.Dispose ();
				DetailsLayout = null;
			}

			if (HeaderLayout != null) {
				HeaderLayout.Dispose ();
				HeaderLayout = null;
			}

			if (InputLayout != null) {
				InputLayout.Dispose ();
				InputLayout = null;
			}

			if (MessageList != null) {
				MessageList.Dispose ();
				MessageList = null;
			}

			if (MessageText != null) {
				MessageText.Dispose ();
				MessageText = null;
			}

			if (SendButton != null) {
				SendButton.Dispose ();
				SendButton = null;
			}

			if (StatusLabel != null) {
				StatusLabel.Dispose ();
				StatusLabel = null;
			}

			if (TitleLabel != null) {
				TitleLabel.Dispose ();
				TitleLabel = null;
			}

			if (TypeImage != null) {
				TypeImage.Dispose ();
				TypeImage = null;
			}

			if (NoMessagesLabel != null) {
				NoMessagesLabel.Dispose ();
				NoMessagesLabel = null;
			}
		}
	}
}
