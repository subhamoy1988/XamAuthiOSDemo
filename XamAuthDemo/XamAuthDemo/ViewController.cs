using System;
using System.Diagnostics;
using UIKit;
using Xamarin.Auth;

namespace XamAuthDemo
{
	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			var auth = new OAuth2Authenticator
				(
					 clientId: "925876362988-364920elqm3n3u1ub93oqc5bkd93beoo.apps.googleusercontent.com",
						  clientSecret: "s2MTbHnqqMj4gmdtoyw0yrVq",
						  scope: "openid email profile",
						  authorizeUrl: new Uri("https://accounts.google.com/o/oauth2/auth"),
						  redirectUrl: new Uri("https://gsb.stanford.edu/oauth2callback"),
						  accessTokenUrl: new Uri("https://accounts.google.com/o/oauth2/token"), 
					getUsernameAsync: null,
					// Native UI API switch
					// Default - false
					// will be switched to true in the near future 2017-04
					//      true    - NEW native UI support 
					//              - Android - Chrome Custom Tabs 
					//              - iOS SFSafariViewController
					//              - WORK IN PROGRESS
					//              - undocumented
					//      false   - OLD embedded browser API 
					//              - Android - WebView 
					//              - iOS - UIWebView
					isUsingNativeUI: true
				);

			auth.AllowCancel = true;
			UIViewController ui_controller = (UIViewController)auth.GetUI();
			PresentViewController(ui_controller, false, null);

			// If authorization succeeds or is canceled, .Completed will be fired.
			auth.Completed += Auth_Completed;
			//auth.Error += Auth_Error;
			auth.BrowsingCompleted += Auth_BrowsingCompleted;
		}

		void Auth_BrowsingCompleted(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}

		void Auth_Completed(object sender, AuthenticatorCompletedEventArgs e)
		{
			throw new NotImplementedException();
			var auth = sender as OAuth2Authenticator;
			if (e.IsAuthenticated)
			{
				System.Object ui_controller_as_object = auth.GetUI();
				if (auth.IsUsingNativeUI == true)
				{
					//=================================================================
					// Xamarin.Auth API - Native UI support 
					//      *   Android - [Chrome] Custom Tabs on Android       
					//          Android.Support.CustomTabs      
					//          and 
					//      *   iOS -  SFSafariViewController     
					//          SafariServices.SFSafariViewController
					// on 2014-04-20 google (and some other providers) will work only with this API
					//  
					//
					//  2017-03-25
					//      NEW UPCOMMING API undocumented work in progress
					//      soon to be default
					//      optional API in the future (for backward compatibility)
					//
					//  required part
					//  add 
					//     following code:
					SafariServices.SFSafariViewController c = null;
					c = (SafariServices.SFSafariViewController)ui_controller_as_object;
					//  add custom schema (App Linking) handling
					//    in AppDelegate.cs
					//         public override bool OpenUrl
					//                                (
					//                                    UIApplication application, 
					//                                    NSUrl url, 
					//                                    string sourceApplication, 
					//                                    NSObject annotation
					//                                )
					//
					//  NOTE[s]
					//  *   custom scheme support only
					//      xamarinauth://localhost
					//      xamarin-auth://localhost
					//      xamarin.auth://localhost
					//  *   no http[s] scheme support
					//------------------------------------------------------------
					// [OPTIONAL] UI customization 
					if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
					{
						//c.PreferredBarTintColor = color_xamarin_blue;
						c.PreferredControlTintColor = UIColor.White;
					}
					else
					{
						//c.View.TintColor = color_xamarin_blue;
					}

					Action view_controller_customization =
						() =>
						{
			//c.NavigationController.NavigationBar.TintColor = color_xamarin_blue;
		};
					//------------------------------------------------------------
					// [REQUIRED] launching SFSafariViewController
					PresentViewController(c, true, view_controller_customization);
					Debug.WriteLine("Authentication Success");
					//=================================================================
				}
				else
				{
					//=================================================================
					// Xamarin.Auth API - embedded browsers support 
					//     - Android - WebView 
					//     - iOS - UIWebView
					//
					// on 2014-04-20 google (and some other providers) will work only with this API
					//
					//  2017-03-25
					//      soon to be non-default
					//      optional API in the future (for backward compatibility)
					UIViewController c = (UIViewController)ui_controller_as_object;
					//------------------------------------------------------------
					// [REQUIRED] launching UIViewController with embedded UIWebView
					PresentViewController(c, true, null);
					//=================================================================
				}
				//#####################################################################
			}
		}
}
}
