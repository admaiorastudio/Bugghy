using System;

using Foundation;
using UIKit;

namespace AdMaiora.Bugghy
{
    public partial class IssueViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("IssueViewCell");
        public static readonly UINib Nib;

        static IssueViewCell ()
        {
            Nib = UINib.FromName ("IssueViewCell", NSBundle.MainBundle);
        }

        protected IssueViewCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
