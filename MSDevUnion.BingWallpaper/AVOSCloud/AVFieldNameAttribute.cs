using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud
{
    [System.AttributeUsage(System.AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class AVFieldNameAttribute : Attribute
	{
		public string FieldName
		{
			get;
			private set;
		}

		public AVFieldNameAttribute(string fieldName)
		{
			this.FieldName = fieldName;
		}
	}
}