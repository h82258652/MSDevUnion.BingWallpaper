using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.Phone
{
	internal class CanonicalPhoneName
	{
		public string CanonicalManufacturer
		{
			get;
			set;
		}

		public string CanonicalModel
		{
			get;
			set;
		}

		public string Comments
		{
			get;
			set;
		}

		public string FullCanonicalName
		{
			get
			{
				return string.Concat(this.CanonicalManufacturer, " ", this.CanonicalModel);
			}
		}

		public bool IsResolved
		{
			get;
			set;
		}

		public string ReportedManufacturer
		{
			get;
			set;
		}

		public string ReportedModel
		{
			get;
			set;
		}

		public CanonicalPhoneName()
		{
		}
	}
}