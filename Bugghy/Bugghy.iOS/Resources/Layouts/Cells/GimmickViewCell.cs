using System;

using Foundation;
using UIKit;

namespace AdMaiora.Bugghy
{
    public partial class GimmickViewCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString ("GimmickViewCell");
        public static readonly UINib Nib;

        static GimmickViewCell ()
        {
            Nib = UINib.FromName ("GimmickViewCell", NSBundle.MainBundle);
        }

        protected GimmickViewCell (IntPtr handle) : base (handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }
    }
}
