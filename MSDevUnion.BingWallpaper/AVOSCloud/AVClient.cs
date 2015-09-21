using AVOSCloud.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AVOSCloud
{
	public static class AVClient
	{
		private static object mutex;

		internal readonly static string versionString;

		internal readonly static string dateFormatString;

		private static string[] assemblyNames;

		internal readonly static IPlatformHooks platformHooks;

		internal static Guid? installationId;

		private readonly static Dictionary<Tuple<Type, Type>, Type> interfaceLookupCache;

		private readonly static string APIAddressCN;

		private readonly static string APIAddressUS;

		private static string _channel;

		private static string _bundleIdentifier;

		private static string _bundleVersion;

		internal static string APIVersionString
		{
			get;
			set;
		}

		internal static string AppKey
		{
			get;
			set;
		}

		internal static string ApplicationId
		{
			get;
			set;
		}

		internal static IDictionary<string, object> ApplicationSettings
		{
			get
			{
				return AVClient.platformHooks.ApplicationSettings;
			}
		}

		internal static string BundelVersion
		{
			get
			{
				if (string.IsNullOrEmpty(AVClient._bundleVersion))
				{
					AVClient._bundleVersion = "1.0";
				}
				return AVClient._bundleVersion;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					AVClient._bundleVersion = value;
				}
			}
		}

		internal static string bundleDisplayName
		{
			get;
			set;
		}

		internal static string BundleIdentifier
		{
			get
			{
				if (string.IsNullOrEmpty(AVClient._bundleIdentifier))
				{
					AVClient._bundleIdentifier = "com.Company.ProductName";
				}
				return AVClient._bundleIdentifier;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					AVClient._bundleIdentifier = value;
				}
			}
		}

		internal static string Channel
		{
			get
			{
				if (string.IsNullOrEmpty(AVClient._channel))
				{
					AVClient._channel = "undefined";
				}
				return AVClient._channel;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					AVClient._channel = value;
				}
			}
		}

		internal static IDictionary<string, object> DeviceHook
		{
			get
			{
				return AVClient.platformHooks.DeviceHook;
			}
		}

		internal static Uri HostName
		{
			get;
			set;
		}

		internal static Guid? InstallationId
		{
			get
			{
				object obj = null;
				Guid? nullable;
				lock (AVClient.mutex)
				{
					if (AVClient.installationId.HasValue)
					{
						nullable = AVClient.installationId;
					}
					else
					{
						AVClient.ApplicationSettings.TryGetValue("InstallationId", out obj);
						try
						{
							AVClient.installationId = new Guid?(new Guid((string)obj));
						}
						catch (Exception exception)
						{
							AVClient.installationId = null;
						}
						nullable = AVClient.installationId;
					}
				}
				return nullable;
			}
			set
			{
				object str;
				lock (AVClient.mutex)
				{
					IDictionary<string, object> applicationSettings = AVClient.ApplicationSettings;
					if (!value.HasValue)
					{
						str = null;
					}
					else
					{
						str = value.ToString();
					}
					applicationSettings["InstallationId"]= str;
					AVClient.installationId = value;
				}
			}
		}

		internal static string MasterKey
		{
			get;
			set;
		}

		internal static System.Version Version
		{
			get
			{
				AssemblyName assemblyName = new AssemblyName(IntrospectionExtensions.GetTypeInfo(typeof(AVClient)).Assembly.FullName);
				return assemblyName.Version;
			}
		}

		static AVClient()
		{
			AVClient.APIAddressCN = "https://api.leancloud.cn";
			AVClient.APIAddressUS = "https://avoscloud.us/";
			AVClient.mutex = new object();
			AVClient.dateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'";
			string[] strArray = new string[] { "AVOSCloud.UWP","AVOSCloud.Phone", "AVOSCloud.Windows.Phone.App", "AVOSCloud.WinRT", "AVOSCloud.NetFx45", "AVOSCloud.iOS", "AVOSCloud.Android", "AVOSCloud.Unity", "Assembly-CSharp" };
			AVClient.assemblyNames = strArray;
			AVClient.interfaceLookupCache = new Dictionary<Tuple<Type, Type>, Type>();
			Type aVClientType = AVClient.GetAVClientType("PlatformHooks");
			if (aVClientType == null)
			{
				throw new InvalidOperationException("You must include a reference to a platform-specific AVOSCloud library.");
			}
		    AVClient.platformHooks = Activator.CreateInstance(aVClientType) as IPlatformHooks;
			AVClient.versionString = string.Concat("net-", AVClient.platformHooks.SDKName, AVClient.Version);
			AVClient.platformHooks.DeviceHook.Add("sdk_version", AVClient.versionString);
		}

		internal static T As<T>(object value)
		where T : class
		{
			return (T)(AVClient.ConvertTo<T>(value) as T);
		}

        internal static string BuildQueryString(IDictionary<string, object> parameters)
        {
            return string.Join("&", parameters.Select(pair => new
            {
                pair = pair,
                valueString = pair.Value as string
            }).Select(hTransparentIdentifier2 =>
                $"{Uri.EscapeDataString(hTransparentIdentifier2.pair.Key)}={Uri.EscapeDataString(string.IsNullOrEmpty(hTransparentIdentifier2.valueString) ? Json.Encode(hTransparentIdentifier2.pair.Value) : hTransparentIdentifier2.valueString)}").ToArray());
        }

        internal static void ClearInMemoryInstallation()
		{
			AVClient.platformHooks.ClearInMemoryInstallation();
		}

		internal static object ConvertTo<T>(object value)
		{
			if (value is T || value == null)
			{
				return value;
			}
			if (typeof(T).IsPrimitive())
			{
				return (T)Convert.ChangeType(value, typeof(T));
			}
			if (typeof(T).IsConstructedGenericType())
			{
				if (typeof(T).IsNullable())
				{
					Type genericTypeArguments = typeof(T).GetGenericTypeArguments()[0];
					if (genericTypeArguments.IsPrimitive())
					{
						return (T)Convert.ChangeType(value, genericTypeArguments);
					}
				}
				Type interfaceType = AVClient.GetInterfaceType(value.GetType(), typeof(IList));
				if (interfaceType != null && (object)typeof(T).GetGenericTypeDefinition() == (object)typeof(IList))
				{
					Type type = typeof(FlexibleListWrapper<,>);
					Type[] typeArray = new Type[] { typeof(T).GetGenericTypeArguments()[0], interfaceType.GetGenericTypeArguments()[0] };
					Type type1 = type.MakeGenericType(typeArray);
					return Activator.CreateInstance(type1, new object[] { value });
				}
				Type interfaceType1 = AVClient.GetInterfaceType(value.GetType(), typeof(IDictionary));
				if (interfaceType1 != null && (object)typeof(T).GetGenericTypeDefinition() == (object)typeof(IDictionary))
				{
					Type type2 = typeof(FlexibleDictionaryWrapper<,>);
					Type[] genericTypeArguments1 = new Type[] { typeof(T).GetGenericTypeArguments()[1], interfaceType1.GetGenericTypeArguments()[1] };
					Type type3 = type2.MakeGenericType(genericTypeArguments1);
					return Activator.CreateInstance(type3, new object[] { value });
				}
			}
			return value;
		}

        internal static object Decode(object data)
        {
            if (data == null)
            {
                return null;
            }
            IDictionary<string, object> dictionary = data as IDictionary<string, object>;
            if (dictionary == null)
            {
                IList<object> list = data as IList<object>;
                if (list == null)
                {
                    return data;
                }
                return list.Select(item => AVClient.Decode(item)).ToList();
            }
            else
            {
                if (dictionary.ContainsKey("__op"))
                {
                    return AVFieldOperations.Decode(dictionary);
                }
                object obj;
                dictionary.TryGetValue("__type", out obj);
                string text = obj as string;
                if (text == null)
                {
                    Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
                    foreach (var d in dictionary)
                    {
                        dictionary2[d.Key] = Decode(d.Value);
                    }
                    return dictionary2;
                }
                if (text == "Date")
                {
                    return AVClient.ParseDate(dictionary["iso"] as string);
                }
                if (text == "Bytes")
                {
                    return Convert.FromBase64String(dictionary["base64"] as string);
                }
                if (text == "Pointer")
                {
                    //return AVObject.CreateWithoutData(dictionary["className"] as string, dictionary["objectId"] as string);
                    AVObject aVObject = AVObject.CreateWithoutData(dictionary["className"] as string, null);
                    aVObject.MergeAfterFetch(dictionary);
                    return aVObject;
                }
                if (text == "File")
                {
                    return new AVFile(dictionary["name"] as string, new Uri(dictionary["url"] as string));
                }
                if (text == "GeoPoint")
                {
                    return new AVGeoPoint((double)AVClient.ConvertTo<double>(dictionary["latitude"]), (double)AVClient.ConvertTo<double>(dictionary["longitude"]));
                }
                if (text == "Object")
                {
                    AVObject aVObject = AVObject.CreateWithoutData(dictionary["className"] as string, null);
                    aVObject.MergeAfterFetch(dictionary);
                    return aVObject;
                }
                if (text == "Relation")
                {
                    return AVRelationBase.CreateRelation(null, null, dictionary["className"] as string);
                }
                Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
                foreach (var d in dictionary)
                {
                    dictionary3[d.Key] = Decode(d.Value);
                }
                return dictionary3;
            }
        }

        internal static IDictionary<string, string> DecodeQueryString(string queryString)
		{
			string str;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string[] strArray = queryString.Split(new char[] { '&' });
			for (int i = 0; i < (int)strArray.Length; i++)
			{
				string str1 = strArray[i];
				char[] chrArray = new char[] { '=' };
				string[] strArray1 = str1.Split(chrArray, 2);
				Dictionary<string, string> dictionary1 = dictionary;
				string str2 = strArray1[0];
				if ((int)strArray1.Length != 2)
				{
					str = null;
				}
				else
				{
					str = Uri.UnescapeDataString(strArray1[1].Replace("+", " "));
				}
				dictionary1[str2]= str;
			}
			return dictionary;
		}

		internal static IEnumerable<object> DeepTraversal(object root, bool traverseAVObjects, bool yieldRoot)
		{
			HashSet<object> hashSet = new HashSet<object>(new IdentityEqualityComparer<object>());
			IEnumerable<object> enumerable = AVClient.DeepTraversalInternal(root, traverseAVObjects, hashSet);
			if (!yieldRoot)
			{
				return enumerable;
			}
			return new object[] { root }.Concat<object>(enumerable);
		}

		private static IEnumerable<object> DeepTraversalInternal(object root, bool traverseAVObjects, ICollection<object> seen)
		{
			//AVClient.<>c__DisplayClassb variable = null;
			seen.Add(root);
			IEnumerable<object> values = null;
			IDictionary<string, object> dictionary = AVClient.As<IDictionary<string, object>>(root);
			if (dictionary == null)
			{
				IList<object> list = AVClient.As<IList<object>>(root);
				if (list != null)
				{
					values = list;
				}
				else if (traverseAVObjects)
				{

                    AVObject obj = root as AVObject;
                    if (obj != null)
                    {
                        values = obj.Keys.ToList().Select(k => obj[k]);
                    }
                }
			}
			else
			{
				values = dictionary.Values;
			}
			if (values != null)
			{
				foreach (object value in values)
				{
					if (seen.Contains(value))
					{
						continue;
					}
					yield return value;
					foreach (object obj in AVClient.DeepTraversalInternal(value, traverseAVObjects, seen))
					{
						yield return obj;
					}
				}
			}
		}

		internal static IDictionary<string, object> DeserializeJsonString(string jsonData)
		{
			return Json.Parse(jsonData) as IDictionary<string, object>;
		}

		internal static Task<Tuple<HttpStatusCode, byte[]>> DownloadAysnc(Uri uri, IList<KeyValuePair<string, string>> headers, IProgress<AVDownloadProgressEventArgs> progress, CancellationToken cancellationToken)
		{
			return AVClient.platformHooks.DownloadAysnc(uri, headers, progress, cancellationToken);
		}

		private static object EncodeJSONArray(IList<object> list, bool allowAVObjects)
		{
			List<object> list1 = new List<object>();
			foreach (object obj in list)
			{
				if (!AVClient.IsValidType(obj))
				{
					throw new ArgumentException("Invalid type for value in an array");
				}
				list1.Add(AVClient.MaybeEncodeJSONObject(obj, allowAVObjects));
			}
			return list1;
		}

		private static IDictionary<string, object> EncodeJSONObject(object value, bool allowAVObjects)
		{
			if (value is DateTime)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				DateTime dateTime = (DateTime)value;
				dictionary.Add("iso", dateTime.ToString(AVClient.dateFormatString));
				dictionary.Add("__type", "Date");
				return dictionary;
			}
			byte[] numArray = value as byte[];
			if (numArray != null)
			{
				Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
				dictionary1.Add("__type", "Bytes");
				dictionary1.Add("base64", Convert.ToBase64String(numArray));
				return dictionary1;
			}
			AVObject aVObjects = value as AVObject;
			if (aVObjects != null)
			{
				if (!allowAVObjects)
				{
					throw new ArgumentException("AVObjects not allowed here.");
				}
				if (aVObjects.ObjectId == null)
				{
					throw new ArgumentException("Cannot create a pointer to an object without an objectId");
				}
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				dictionary2.Add("__type", "Pointer");
				dictionary2.Add("className", aVObjects.ClassName);
				dictionary2.Add("objectId", aVObjects.ObjectId);
				return dictionary2;
			}
			AVFile aVFile = value as AVFile;
			if (aVFile != null)
			{
				return aVFile.ToJSON();
			}
			if (value is AVGeoPoint)
			{
				return ((AVGeoPoint)value).ToJSON();
			}
			AVACL aVACL = value as AVACL;
			if (aVACL != null)
			{
				return aVACL.ToJSON();
			}
			IDictionary<string, object> dictionary3 = AVClient.ConvertTo<IDictionary<string, object>>(value) as IDictionary<string, object>;
			if (dictionary3 == null)
			{
				AVRelationBase aVRelationBase = value as AVRelationBase;
				if (aVRelationBase == null)
				{
					return null;
				}
				return aVRelationBase.ToJSON();
			}
			Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
			foreach (KeyValuePair<string, object> keyValuePair in dictionary3)
			{
				dictionary4[keyValuePair.Key]= AVClient.MaybeEncodeJSONObject(keyValuePair.Value, allowAVObjects);
			}
			return dictionary4;
		}

		internal static Type GetAVClientType(string name)
		{
			string[] strArray = AVClient.assemblyNames;
			for (int i = 0; i < (int)strArray.Length; i++)
			{
				string str = string.Format("AVOSCloud.{0},{1}", name,strArray[i]);
				Type type = Type.GetType(str);
				if (type != null)
				{
					return type;
				}
			}
			return null;
		}

		private static Type GetInterfaceType(Type objType, Type genericInterfaceType)
		{
			if (genericInterfaceType.IsConstructedGenericType())
			{
				genericInterfaceType = genericInterfaceType.GetGenericTypeDefinition();
			}
			Tuple<Type, Type> tuple = new Tuple<Type, Type>(objType, genericInterfaceType);
			if (AVClient.interfaceLookupCache.ContainsKey(tuple))
			{
				return AVClient.interfaceLookupCache[tuple];
			}
			Type[] array = TypeExtensions.GetInterfaces(objType);
			for (int i = 0; i < (int)array.Length; i++)
			{
				Type type = array[i];
				if (type.IsConstructedGenericType() && (object)type.GetGenericTypeDefinition() == (object)genericInterfaceType)
				{
					Type type1 = type;
					Type type2 = type1;
					AVClient.interfaceLookupCache[tuple]= type1;
					return type2;
				}
			}
			return null;
		}

		public static void Initialize(string applicationId, string appKey)
		{
			AVClient.Initialize(applicationId, appKey, AVClient.APIAddressCN);
		}

		public static void Initialize(string applicationId, string appKey, AVRegion region)
		{
			string[] aPIAddressCN = new string[] { AVClient.APIAddressCN, AVClient.APIAddressUS };
			int num = (int)region;
			if (num < 0 || num > (int)aPIAddressCN.Length)
			{
				throw new Exception("所选的地区不在 AVOS Cloud 的支持范围之内，请查阅文档。");
			}
			AVClient.Initialize(applicationId, appKey, aPIAddressCN[num]);
		}

		internal static void Initialize(string applicationId, string appKey, string apiHost)
		{
			lock (AVClient.mutex)
			{
				try
				{
					AVClient.HostName = AVClient.HostName ?? new Uri(apiHost.Trim());
					AVClient.APIVersionString = "/1.1";
					AVClient.ApplicationId = applicationId;
					AVClient.AppKey = appKey;
					AVObject.RegisterSubclass<AVUser>();
					AVObject.RegisterSubclass<AVRole>();
					AVClient.platformHooks.Initialize();
					if (!AVClient.InstallationId.HasValue)
					{
						AVClient.InstallationId = new Guid?(Guid.NewGuid());
					}
				}
				catch (Exception exception)
				{
					throw exception;
				}
			}
		}

		internal static bool IsContainerObject(object value)
		{
			if (value is AVACL || value is AVGeoPoint || AVClient.ConvertTo<IDictionary<string, object>>(value) is IDictionary<string, object>)
			{
				return true;
			}
			return AVClient.ConvertTo<IList<object>>(value) is IList<object>;
		}

		internal static bool IsValidType(object value)
		{
			if (value == null || value.GetType().IsPrimitive() || value is string || value is AVObject || value is AVACL || value is AVFile || value is AVGeoPoint || value is AVRelationBase || value is DateTime || value is byte[] || AVClient.ConvertTo<IDictionary<string, object>>(value) is IDictionary<string, object>)
			{
				return true;
			}
			return AVClient.ConvertTo<IList<object>>(value) is IList<object>;
		}

		internal static object MaybeEncodeJSONObject(object value, bool allowAVObjects)
		{
			IDictionary<string, object> dictionary = AVClient.EncodeJSONObject(value, allowAVObjects);
			if (dictionary != null)
			{
				return dictionary;
			}
			IList<object> list = AVClient.ConvertTo<IList<object>>(value) as IList<object>;
			if (list != null)
			{
				return AVClient.EncodeJSONArray(list, allowAVObjects);
			}
			IAVFieldOperation aVFieldOperation = value as IAVFieldOperation;
			if (aVFieldOperation == null)
			{
				return value;
			}
			return aVFieldOperation.Encode();
		}

		internal static DateTime ParseDate(string input)
		{
			return DateTime.Parse(input);
		}

		internal static Tuple<HttpStatusCode, IDictionary<string, object>> ReponseResolve(Tuple<HttpStatusCode, string> response, CancellationToken cancellationToken)
		{
			IDictionary<string, object> dictionary;
			Tuple<HttpStatusCode, string> tuple = response;
			string item2 = tuple.Item2;
			HttpStatusCode item1 = tuple.Item1;
			if (item2 == null)
			{
				cancellationToken.ThrowIfCancellationRequested();
				return new Tuple<HttpStatusCode, IDictionary<string, object>>(item1, null);
			}
			IDictionary<string, object> dictionary1 = null;
			try
			{
				if (!item2.StartsWith("["))
				{
					dictionary = AVClient.DeserializeJsonString(item2);
				}
				else
				{
					Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
					dictionary2.Add("results", Json.Parse(item2));
					dictionary = dictionary2;
				}
				dictionary1 = dictionary;
			}
			catch (Exception exception)
			{
				throw new AVException(AVException.ErrorCode.OtherCause, "Invalid response from server", exception);
			}
			int num = (int)item1;
			if (num > 203 || num < 200)
			{
				throw new AVException((AVException.ErrorCode)((int)((dictionary1.ContainsKey("code") ? (long)dictionary1["code"] : (long)-1))), (dictionary1.ContainsKey("error") ? dictionary1[("error")] as string : item2), null);
			}
			cancellationToken.ThrowIfCancellationRequested();
			return new Tuple<HttpStatusCode, IDictionary<string, object>>(item1, dictionary1);
		}

		internal static Task<Tuple<HttpStatusCode, string>> RequestAsync(Uri uri, string method, IList<KeyValuePair<string, string>> headers, Stream data, string contentType, CancellationToken cancellationToken)
		{
			if (method == null)
			{
				if (data == null)
				{
					method = "GET";
				}
				else
				{
					method = "POST";
				}
			}
			return AVClient.platformHooks.RequestAsync(uri, method, headers, data, contentType, cancellationToken);
		}

		internal static Task<Tuple<HttpStatusCode, IDictionary<string, object>>> RequestAsync(string method, string relativeUri, string sessionToken, IDictionary<string, object> data, CancellationToken cancellationToken)
		{
			return AVClient.RequestAsync(method, new Uri(relativeUri, UriKind.Relative), sessionToken, data, cancellationToken);
		}

        internal static Task<Tuple<HttpStatusCode, IDictionary<string, object>>> RequestAsync(string method, Uri relativeUri, string sessionToken, IDictionary<string, object> data, CancellationToken cancellationToken)
        {
            if (AVClient.ApplicationId == null)
            {
                throw new InvalidOperationException("必须显式调用 AVClient.Initialize() 初始化 SDK之后，才能使用 SDK。");
            }
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            list.Add(new KeyValuePair<string, string>("X-AVOSCloud-Application-Id", AVClient.ApplicationId));
            list.Add(new KeyValuePair<string, string>("X-AVOSCloud-Application-Key", AVClient.AppKey));
            List<KeyValuePair<string, string>> list2 = list;
            Guid? arg_5D_0 = AVClient.InstallationId;
            List<KeyValuePair<string, string>> list3 = list2;
            if (sessionToken != null)
            {
                list3.Add(new KeyValuePair<string, string>("X-AVOSCloud-Session-Token", sessionToken));
            }
            Stream data2 = null;
            string contentType = null;
            if (data != null)
            {
                string text = AVClient.SerializeJsonString(data);
                data2 = new MemoryStream(Encoding.UTF8.GetBytes(text));
                contentType = "application/json";
            }
            return AVClient.RequestAsync(new Uri(AVClient.HostName, AVClient.APIVersionString + relativeUri), method, list3, data2, contentType, cancellationToken).OnSuccess(t => AVClient.ReponseResolve(t.Result, cancellationToken));
        }


        internal static string SerializeJsonString(IDictionary<string, object> jsonData)
		{
			return Json.Encode(jsonData);
		}
	}
}