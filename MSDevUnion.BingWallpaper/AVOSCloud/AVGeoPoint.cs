using System;
using System.Collections.Generic;

namespace AVOSCloud
{
	public struct AVGeoPoint
	{
		private double latitude;

		private double longitude;

		public double Latitude
		{
			get
			{
				return this.latitude;
			}
			set
			{
				if (value > 90 || value < -90)
				{
					throw new ArgumentOutOfRangeException("value", "Latitude must be within the range [-90, 90]");
				}
				this.latitude = value;
			}
		}

		public double Longitude
		{
			get
			{
				return this.longitude;
			}
			set
			{
				if (value > 180 || value < -180)
				{
					throw new ArgumentOutOfRangeException("value", "Longitude must be within the range [-180, 180]");
				}
				this.longitude = value;
			}
		}

		public AVGeoPoint(double latitude, double longitude)
		{
			AVGeoPoint aVGeoPoint = new AVGeoPoint()
			{
				Latitude = latitude,
				Longitude = longitude
			};
			this = aVGeoPoint;
		}

		public AVGeoDistance DistanceTo(AVGeoPoint point)
		{
			double num = 0.0174532925199433;
			double latitude = this.Latitude * num;
			double num1 = this.longitude * num;
			double latitude1 = point.Latitude * num;
			double longitude = point.Longitude * num;
			double num2 = latitude - latitude1;
			double num3 = num1 - longitude;
			double num4 = Math.Sin(num2 / 2);
			double num5 = Math.Sin(num3 / 2);
			double num6 = num4 * num4 + Math.Cos(latitude) * Math.Cos(latitude1) * num5 * num5;
			num6 = Math.Min(1, num6);
			return new AVGeoDistance(2 * Math.Asin(Math.Sqrt(num6)));
		}

		internal IDictionary<string, object> ToJSON()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("__type", "GeoPoint");
			dictionary.Add("latitude", this.Latitude);
			dictionary.Add("longitude", this.Longitude);
			return dictionary;
		}
	}
}