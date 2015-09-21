using AVOSCloud;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace AVOSCloud.Internal
{
	internal interface IPlatformHooks
	{
		IDictionary<string, object> ApplicationSettings
		{
			get;
		}

		IDictionary<string, object> DeviceHook
		{
			get;
		}

		string SDKName
		{
			get;
		}

		void ClearInMemoryInstallation();

		Task<Tuple<HttpStatusCode, byte[]>> DownloadAysnc(Uri uri, IList<KeyValuePair<string, string>> headers, IProgress<AVDownloadProgressEventArgs> progress, CancellationToken cancellationToken);

		Task<IDictionary<string, object>> GetCache(string key);

		void Initialize();

		void PopupWebBrowser(string uri, EventHandler<AVCompleteAuthorizationEventArgs> CompleteAuthorizationHandler);

		bool RemoveCache(string key);

		Task<Tuple<HttpStatusCode, string>> RequestAsync(Uri uri, string method, IList<KeyValuePair<string, string>> headers, Stream data, string contentType, CancellationToken cancellationToken);

		IEnumerable<AVNetworkProfile> RetrieveNetwork();

		Task<bool> SetCache(string key, object value);

		Task<Tuple<HttpStatusCode, string>> UploadAsync(Uri uri, IList<KeyValuePair<string, string>> headers, Stream data, IProgress<AVUploadProgressEventArgs> progress, CancellationToken cancellationToken);
	}
}