using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AVClassNameAttribute : Attribute
	{
		public string ClassName
		{
			get;
			private set;
		}

		public AVClassNameAttribute(string className)
		{
			this.ClassName = className;
		}
	}
}