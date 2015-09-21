using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud
{
	public struct AVGeoDistance
	{
		private const double EarthMeanRadiusKilometers = 6371;

		private const double EarthMeanRadiusMiles = 3958.8;

		public double Kilometers
		{
			get
			{
				return this.Radians * 6371;
			}
		}

		public double Miles
		{
			get
			{
				return this.Radians * 3958.8;
			}
		}

		public double Radians
		{
			get;
			private set;
		}

		public AVGeoDistance(double radians)
		{
			this = new AVGeoDistance()
			{
				Radians = radians
			};
		}

		public static AVGeoDistance FromKilometers(double kilometers)
		{
			return new AVGeoDistance(kilometers / 6371);
		}

		public static AVGeoDistance FromMiles(double miles)
		{
			return new AVGeoDistance(miles / 3958.8);
		}

		public static AVGeoDistance FromRadians(double radians)
		{
			return new AVGeoDistance(radians);
		}
	}
}