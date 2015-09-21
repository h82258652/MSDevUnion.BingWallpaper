using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using static System.Reflection.IntrospectionExtensions;

namespace AVOSCloud.Internal
{
	internal static class ReflectionHelpers
	{
        internal static ConstructorInfo FindConstructor(this Type self, params Type[] parameterTypes)
        {
            IEnumerable<ConstructorInfo> source = from constructor in self.GetConstructors()
                let parameters = constructor.GetParameters()
                let types =parameters.Select(p =>p.ParameterType)
                where types.SequenceEqual(parameterTypes)
                select constructor;
            return source.Single();
        }

        internal static IEnumerable<ConstructorInfo> GetConstructors(this Type type)
		{
            return type.GetTypeInfo().DeclaredConstructors;
        }

		internal static Type[] GetGenericTypeArguments(this Type type)
		{
			return type.GenericTypeArguments;
		}

		//internal static IEnumerable<Type> GetInterfaces(this Type type)
		//{
  //          return type.GetTypeInfo().ImplementedInterfaces;
  //      }

		//internal static MethodInfo GetMethod(this Type type, string name, Type[] parameters)
		//{
  //          return type.GetRuntimeMethod(name, parameters);
  //      }

		//internal static IEnumerable<PropertyInfo> GetProperties(this Type type)
		//{
  //          return type.GetRuntimeProperties();
      //  }

		//internal static PropertyInfo GetProperty(this Type type, string name)
		//{
  //          return type.GetRuntimeProperty(name);
  //      }

		internal static bool IsConstructedGenericType(this Type type)
		{
            return type.IsConstructedGenericType;
        }

		internal static bool IsNullable(this Type t)
		{
            return t.IsConstructedGenericType && t.GetGenericTypeDefinition() == null && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

		internal static bool IsPrimitive(this Type type)
		{
			return type.GetTypeInfo().IsPrimitive;
		}
	}
}