using AVOSCloud.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AVOSCloud
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class AVRelationBase
	{
		private AVObject parent;

		private string key;

		private string targetClassName;

		internal string TargetClassName
		{
			get
			{
				return this.targetClassName;
			}
			set
			{
				this.targetClassName = value;
			}
		}

		internal AVRelationBase(AVObject parent, string key)
		{
			this.EnsureParentAndKey(parent, key);
		}

		internal AVRelationBase(AVObject parent, string key, string targetClassName) : this(parent, key)
		{
			this.targetClassName = targetClassName;
		}

		internal void Add(AVObject obj)
		{
			AVRelationOperation aVRelationOperation = new AVRelationOperation(new AVObject[] { obj }, null);
			this.parent.PerformOperation(this.key, aVRelationOperation);
			this.targetClassName = aVRelationOperation.TargetClassName;
		}

		internal static AVRelationBase CreateRelation(AVObject parent, string key, string targetClassName)
		{
			Type type = AVObject.GetType(targetClassName);
			Expression<Func<AVRelation<AVObject>>> expression = () => AVRelationBase.CreateRelation<AVObject>(parent, key, targetClassName);
			MethodInfo methodInfo = ((MethodCallExpression)expression.Body).Method.GetGenericMethodDefinition().MakeGenericMethod(new Type[] { type });
			object[] objArray = new object[] { parent, key, targetClassName };
			return (AVRelationBase)methodInfo.Invoke(null, objArray);
		}

		private static AVRelation<T> CreateRelation<T>(AVObject parent, string key, string targetClassName)
		where T : AVObject
		{
			return new AVRelation<T>(parent, key, targetClassName);
		}

		internal void EnsureParentAndKey(AVObject parent, string key)
		{
			this.parent = this.parent ?? parent;
			this.key = this.key ?? key;
		}

		internal AVQuery<T> GetQuery<T>()
		where T : AVObject
		{
			return (new AVQuery<T>(this.targetClassName)).WhereRelatedTo(this.parent, this.key);
		}

		internal void Remove(AVObject obj)
		{
			AVRelationOperation aVRelationOperation = new AVRelationOperation(null, new AVObject[] { obj });
			this.parent.PerformOperation(this.key, aVRelationOperation);
			this.targetClassName = aVRelationOperation.TargetClassName;
		}

		internal IDictionary<string, object> ToJSON()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("__type", "Relation");
			dictionary.Add("className", this.targetClassName);
			return dictionary;
		}
	}
}