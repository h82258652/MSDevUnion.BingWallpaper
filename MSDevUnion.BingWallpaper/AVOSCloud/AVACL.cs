using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AVOSCloud
{
	public class AVACL
	{
		private const string publicName = "*";

		private readonly ICollection<string> readers = new HashSet<string>();

		private readonly ICollection<string> writers = new HashSet<string>();

		public bool PublicReadAccess
		{
			get
			{
				return this.GetAccess(AVACL.AccessKind.Read, "*");
			}
			set
			{
				this.SetAccess(AVACL.AccessKind.Read, "*", value);
			}
		}

		public bool PublicWriteAccess
		{
			get
			{
				return this.GetAccess(AVACL.AccessKind.Write, "*");
			}
			set
			{
				this.SetAccess(AVACL.AccessKind.Write, "*", value);
			}
		}

        internal AVACL(IDictionary<string, object> jsonObject)
        {
            this.readers = new HashSet<string>(jsonObject.Where(pair => ((IDictionary<string, object>)pair.Value).ContainsKey("read")).Select(pair => pair.Key));
            this.writers = new HashSet<string>(jsonObject.Where(pair => ((IDictionary<string, object>)pair.Value).ContainsKey("write")).Select(pair => pair.Key));
        }

        public AVACL()
		{
		}

		public AVACL(AVUser owner)
		{
			this.SetReadAccess(owner, true);
			this.SetWriteAccess(owner, true);
		}

		private bool GetAccess(AVACL.AccessKind kind, string userId)
		{
			if (userId == null)
			{
				throw new ArgumentException("Cannot get access for an unsaved user or role.");
			}
			switch (kind)
			{
				case AVACL.AccessKind.Read:
				{
					return this.readers.Contains(userId);
				}
				case AVACL.AccessKind.Write:
				{
					return this.writers.Contains(userId);
				}
			}
			throw new NotImplementedException("Unknown AccessKind");
		}

		public bool GetReadAccess(string userId)
		{
			return this.GetAccess(AVACL.AccessKind.Read, userId);
		}

		public bool GetReadAccess(AVUser user)
		{
			return this.GetReadAccess(user.ObjectId);
		}

		public bool GetRoleReadAccess(string roleName)
		{
			return this.GetAccess(AVACL.AccessKind.Read, string.Concat("role:", roleName));
		}

		public bool GetRoleReadAccess(AVRole role)
		{
			return this.GetRoleReadAccess(role.Name);
		}

		public bool GetRoleWriteAccess(string roleName)
		{
			return this.GetAccess(AVACL.AccessKind.Write, string.Concat("role:", roleName));
		}

		public bool GetRoleWriteAccess(AVRole role)
		{
			return this.GetRoleWriteAccess(role.Name);
		}

		public bool GetWriteAccess(string userId)
		{
			return this.GetAccess(AVACL.AccessKind.Write, userId);
		}

		public bool GetWriteAccess(AVUser user)
		{
			return this.GetWriteAccess(user.ObjectId);
		}

		private void SetAccess(AVACL.AccessKind kind, string userId, bool allowed)
		{
			if (userId == null)
			{
				throw new ArgumentException("Cannot set access for an unsaved user or role.");
			}
			ICollection<string> collection = null;
			switch (kind)
			{
				case AVACL.AccessKind.Read:
				{
					collection = this.readers;
					break;
				}
				case AVACL.AccessKind.Write:
				{
					collection = this.writers;
					break;
				}
				default:
				{
					throw new NotImplementedException("Unknown AccessKind");
				}
			}
			if (allowed)
			{
				collection.Add(userId);
				return;
			}
			collection.Remove(userId);
		}

		public void SetReadAccess(string userId, bool allowed)
		{
			this.SetAccess(AVACL.AccessKind.Read, userId, allowed);
		}

		public void SetReadAccess(AVUser user, bool allowed)
		{
			this.SetReadAccess(user.ObjectId, allowed);
		}

		public void SetRoleReadAccess(string roleName, bool allowed)
		{
			this.SetAccess(AVACL.AccessKind.Read, string.Concat("role:", roleName), allowed);
		}

		public void SetRoleReadAccess(AVRole role, bool allowed)
		{
			this.SetRoleReadAccess(role.Name, allowed);
		}

		public void SetRoleWriteAccess(string roleName, bool allowed)
		{
			this.SetAccess(AVACL.AccessKind.Write, string.Concat("role:", roleName), allowed);
		}

		public void SetRoleWriteAccess(AVRole role, bool allowed)
		{
			this.SetRoleWriteAccess(role.Name, allowed);
		}

		public void SetWriteAccess(string userId, bool allowed)
		{
			this.SetAccess(AVACL.AccessKind.Write, userId, allowed);
		}

		public void SetWriteAccess(AVUser user, bool allowed)
		{
			this.SetWriteAccess(user.ObjectId, allowed);
		}

		internal IDictionary<string, object> ToJSON()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			foreach (string str in Enumerable.Union<string>(this.readers, this.writers))
			{
				Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
				if (this.readers.Contains(str))
				{
					dictionary1["read"]= true;
				}
				if (this.writers.Contains(str))
				{
					dictionary1["write"]= true;
				}
				dictionary[str]=dictionary1;
			}
			return dictionary;
		}

		private enum AccessKind
		{
			Read,
			Write
		}
	}
}