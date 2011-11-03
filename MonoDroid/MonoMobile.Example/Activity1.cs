using System;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MonoMobile.Extensions;

namespace MonoMobile.Example
{
	[Activity(Label = "MonoMobile Android Example", MainLauncher = true)]
	public class Activity1 : Activity
	{
        IGeolocation location;
        bool watching = false;
        PositionListener listener;
        TextView locationTextView;
        Button watchButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			
			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			location = new Geolocation (this);
		    //
			// Get our button from the layout resource,
			// and attach an event to it
            Button getLocationButton = FindViewById<Button>(Resource.Id.GetLocationButton);
            
		    watchButton = FindViewById<Button>(Resource.Id.WatchButton);

		    locationTextView = FindViewById<TextView>(Resource.Id.LocationTextView);

		    getLocationButton.Click += delegate
			                    {
			                        LogDeviceInfo();
			                        GetCurrentPosition();
			                    };

		    watchButton.Click += delegate { ToggleWatch(); };
		}

	    private void GetCurrentPosition()
	    {
	    	location.GetCurrentPosition().ContinueWith (t =>
	    	{
	    		if (t.IsCanceled || t.IsFaulted)
	    			return;

	    		CurrentPositionSuccess (t.Result);
	    	});
	    }
	    
	    private void ToggleWatch()
	    {
	        if (!watching)
	        {   
	            listener = location.GetPositionListener();
	            listener.Subscribe (new LocationObserver (WatchSuccess, ToggleWatch, ex => { }));
	            watchButton.Text = GetString (Resource.String.watchStop);
	        }
	        else
	        {
	            listener.Dispose();
	            watchButton.Text = GetString (Resource.String.watchStart);
	        }
	        watching = !watching;
	    }

	    private void LogDeviceInfo()
	    {
	        var device = new MonoMobile.Extensions.Device(this);
	        Android.Util.Log.Info("MonoMobile.Extensions", "Device Name: {0}", device.Name);
	        Android.Util.Log.Info("MonoMobile.Extensions", "Device Platform: {0}", device.Platform);
	        Android.Util.Log.Info("MonoMobile.Extensions", "Device UUID: {0}", device.UUID);
	        Android.Util.Log.Info("MonoMobile.Extensions", "Device Version: {0}", device.Version);
	        Android.Util.Log.Info("MonoMobile.Extensions", "MonoMobile Version: {0}", device.MonoMobileVersion);
	    }

	    private void CurrentPositionSuccess(Position obj)
	    {
	        string message = string.Format("GetCurrentPosition location: {0} {1}-{2} [{3}]",obj.Timestamp, obj.Latitude,
	                                       obj.Longitude, obj.Accuracy);
            Android.Util.Log.Info("MonoMobile.Extensions",message);
	        locationTextView.Text = message;
	    }

	    private void WatchSuccess(Position obj)
	    {
            string message = string.Format("WatchPosition location: {0} {1}-{2} [{3}]",obj.Timestamp, obj.Latitude,
                                           obj.Longitude, obj.Accuracy);
            Android.Util.Log.Info("MonoMobile.Extension", message);
	        locationTextView.Text = message;
	    }

		private class LocationObserver
			: IObserver<Position>
		{
			private readonly Action<Position> onNext;
			private readonly Action onCompleted;
			private readonly Action<Exception> onError;

			public LocationObserver (Action<Position> onNext, Action onCompleted, Action<Exception> onError)
			{
				this.onNext = onNext;
				this.onCompleted = onCompleted;
				this.onError = onError;
			}

			public void OnCompleted()
			{
				this.onCompleted();
			}

			public void OnError (Exception error)
			{
				this.onError (error);
			}

			public void OnNext (Position value)
			{
				this.onNext (value);
			}
		}
	}
}


