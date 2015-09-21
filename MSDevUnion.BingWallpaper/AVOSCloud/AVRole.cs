using System;
using System.Text.RegularExpressions;

namespace AVOSCloud
{
	[AVClassName("_Role")]
	public class AVRole : AVObject
	{
		private readonly static Regex namePattern;

		[AVFieldName("name")]
		public string Name
		{
			get
			{
				return base.GetProperty<string>("Name");
			}
			set
			{
				base.SetProperty<string>(value, "Name");
			}
		}

		public static AVQuery<AVRole> Query
		{
			get
			{
				return new AVQuery<AVRole>();
			}
		}

		[AVFieldName("roles")]
		public AVRelation<AVRole> Roles
		{
			get
			{
				return base.GetRelationProperty<AVRole>("Roles");
			}
		}

		[AVFieldName("users")]
		public AVRelation<AVUser> Users
		{
			get
			{
				return base.GetRelationProperty<AVUser>("Users");
			}
		}

		static AVRole()
		{
			AVRole.namePattern = new Regex("^[0-9a-zA-Z_\\- ]+$");
		}

		public AVRole()
		{
		}

		public AVRole(string name, AVACL acl) : this()
		{
			this.Name = name;
			base.ACL = acl;
		}

		internal override void OnSettingValue(ref string key, ref object value)
		{
			base.OnSettingValue(ref key, ref value);
			if (key == "name")
			{
				if (base.ObjectId != null)
				{
					throw new InvalidOperationException("A role's name can only be set before it has been saved.");
				}
				if (!(value is string))
				{
					throw new ArgumentException("A role's name must be a string.", "value");
				}
				if (!AVRole.namePattern.IsMatch((string)value))
				{
					throw new ArgumentException("A role's name can only contain alphanumeric characters, _, -, and spaces.", "value");
				}
			}
		}
	}
}