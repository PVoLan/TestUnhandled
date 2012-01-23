using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace TestUncaughtException
{
	[Activity (Label = "TestUncaughtException", MainLauncher = true)]
	public class Activity1 : Activity
	{
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			SetContentView (Resource.Layout.Main);
			Button button = FindViewById<Button> (Resource.Id.myButton);
			
			//Trying to catch exception through Mono environment			
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
				Console.WriteLine("Exception caught through Mono");
				//Emulate sending our exception to remote statistics server. 
				//This takes long time, so dalvik VM kills process before we can finish request
				for(int i=0; i<10000; i++){
					Console.Write(i+" ");
				}
				Console.WriteLine("Mono: Exception handling completed");
			};
			
			//Trying to catch exception through Java environment. Doesn't catch anything. Same in pure Java worked properly			
			Java.Lang.Thread.DefaultUncaughtExceptionHandler = new JavaErrorCatcher();
			
			//This doesn't help too
			Java.Lang.Thread.CurrentThread().UncaughtExceptionHandler = new JavaErrorCatcher();
						
			button.Click += (sender, e) => {				
				//And even this
				Java.Lang.Thread.CurrentThread().UncaughtExceptionHandler = new JavaErrorCatcher();
				throw new ArithmeticException();			
			};
		}
		
		
		class JavaErrorCatcher : Java.Lang.Object, Java.Lang.Thread.IUncaughtExceptionHandler{
			
			public void UncaughtException (Java.Lang.Thread thread, Java.Lang.Throwable ex)
			{
				Console.WriteLine("Wanna get this!");
				
				Console.WriteLine("Exception caught through Java");
				//Emulate sending our exception to remote statistics server. 
				//This worked in pure Java very nice, but I can't launch it from Mono
				for(int i=0; i<10000; i++){
					Console.Write(i+" ");
				}
				Console.WriteLine("Java: Exception handling completed");
			}
		}
	}
}


