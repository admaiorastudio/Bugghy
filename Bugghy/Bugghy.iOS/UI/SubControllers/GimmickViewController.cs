using System;

using UIKit;

namespace AdMaiora.Bugghy
{
    public partial class GimmickViewController : UIViewController
    {
        public GimmickViewController () : base ("GimmickViewController", null)
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            // Perform any additional setup after loading the view, typically from a nib.
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}

