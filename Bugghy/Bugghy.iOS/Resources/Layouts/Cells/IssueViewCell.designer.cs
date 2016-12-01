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
		UIKit.UIView ContentLayout { get; set; }
		
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
		}
	}
}
