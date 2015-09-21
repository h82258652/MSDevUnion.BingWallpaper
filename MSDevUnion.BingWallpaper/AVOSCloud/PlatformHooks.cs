using AVOSCloud.Internal;
using AVOSCloud.Phone;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel;
using Windows.Management.Deployment;
using Windows.Networking.Connectivity;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace AVOSCloud
{
    internal class PlatformHooks : IPlatformHooks
    {
        internal static readonly object mutex = new object();

        private static string OSVersion
        {
            get
            {
                Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation deviceInfo = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
                var firmwareVersion = deviceInfo.SystemFirmwareVersion;
                if (string.IsNullOrEmpty(firmwareVersion))
                    return "10240";
                return firmwareVersion;
            }
        }

        private static  IDictionary<string, object> device;

    

        static PlatformHooks()
        {
            device =
            new Dictionary<string, object>()
            {
                {
                    "app_version",
                    Application.Current.GetType().GetTypeInfo().Assembly.GetName().Version.ToString(4)
                },
                {
                    "display_name",
                    Package.Current.DisplayName
                },
                {
                    "os_version",
                    OSVersion
                },
                {
                    "os",
                    (object) "UWP"
                },
                {
                    "language",
                     CultureInfo.CurrentCulture.EnglishName
                },
                {
                    "package_name",
                     Package.Current.DisplayName//(object) AVInstallation.GetAppAttribute("Title")
                },
                {
                    "resolution",
                    (object) PlatformHooks.GetScreenResolution()
                },
                {
                    "device_model",
                    (object) PlatformHooks.GetDeviceModel()
                },
                {
                    "timezone",
                    (object) PlatformHooks.getCurrentTimeOffset()
                },
                {
                    "device_id",
                    (object) PlatformHooks.GetDeviceId()
                }
            };
        }
        private const string AVCACHE_Path = "AVCache";
        private Popup my_popup_cs;

        private EventHandler<AVCompleteAuthorizationEventArgs> completeAuthorizationHandler { get; set; }

        public string SDKName
        {
            get { return "uwp"; }
        }

        public IDictionary<string, object> DeviceHook
        {
            get { return PlatformHooks.device; }
        }

        public IDictionary<string, object> ApplicationSettings
        {
            get { return PlatformHooks.SettingsWrapper.Wrapper; }
        }

        public void PopupWebBrowser(string uri,
            EventHandler<AVCompleteAuthorizationEventArgs> CompleteAuthorizationHandler)
        {
            this.completeAuthorizationHandler = CompleteAuthorizationHandler;
            this.my_popup_cs = new Popup();
            double actualHeight = Window.Current.Bounds.Height;
            double actualWidth = Window.Current.Bounds.Width;
            Border border = new Border();
            StackPanel stackPanel1 = new StackPanel();
            ((Panel) stackPanel1).Background=((Brush) new SolidColorBrush(Colors.LightGray));
            stackPanel1.Orientation=((Orientation) 0);
            StackPanel stackPanel2 = new StackPanel();
            stackPanel2.Orientation=((Orientation) 1);
             (stackPanel1).Children.Add( stackPanel2);
            WebView webView = new WebView();
            webView.Width=(actualWidth);
            webView.Height=(actualHeight);
            //webView.set_IsScriptEnabled(true);
            webView.ScriptNotify += (this.wb_ScriptNotify);
            webView.Source=(new Uri(uri));
            ( stackPanel1).Children.Add(webView);
            border.Child=stackPanel1;
            this.my_popup_cs.Child=((UIElement) border);
            this.my_popup_cs.VerticalOffset=(10.0);
            this.my_popup_cs.HorizontalOffset=(10.0);
            this.my_popup_cs.IsOpen=(true);
            
            //this.my_popup_cs.IsOpen=(true);
        }

        private void WebView_ScriptNotify(object sender, NotifyEventArgs e)
        {
           
        }

        private void wb_ScriptNotify(object sender, NotifyEventArgs e)
        {
            IDictionary<string, object> dictionary =
                Json.Parse(WebUtility.UrlDecode(e.Value)) as IDictionary<string, object>;
            AVCompleteAuthorizationEventArgs e1 = new AVCompleteAuthorizationEventArgs()
            {
                AuthData = dictionary
            };
            this.completeAuthorizationHandler(sender, e1);
            this.my_popup_cs.IsOpen=(false);
            this.my_popup_cs = (Popup) null;
        }

        private static string getCurrentTimeOffset()
        {
            TimeZoneInfo local = TimeZoneInfo.Local;
            return string.Format("{0}{1}:{2:00} ({3})", new object[4]
            {
                local.BaseUtcOffset >= TimeSpan.Zero ? (object) "" : (object) "-",
                (object) Math.Abs(local.BaseUtcOffset.Hours),
                (object) Math.Abs(local.BaseUtcOffset.Minutes),
                (object) TimeZoneInfo.Local.DisplayName
            });
        }

        private static string GetScreenResolution()
        {
            return Window.Current.Bounds.Width + "*" +
                   Window.Current.Bounds.Height;
        }

        private static string GetDeviceModel()
        {
            string str = string.Empty;
            try
            {
                str =
                    PhoneNameResolver.Resolve(DeviceStatus.DeviceManufaturer, DeviceStatus.DeviceName)
                        .FullCanonicalName;
            }
            catch
            {
            }
            return str;
        }

        private static string GetDeviceId()
        {
            string str = string.Empty;
            try
            {

                var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                Object value = localSettings.Values["uniqueDeviceId"];
                if (value==null)
                {
                    value = Guid.NewGuid();
                    localSettings.Values["uniqueDeviceId"] = value;
                }
                str = value.ToString();
                str = Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
            }
            catch
            {
            }
            return str;
        }

        public Task<bool> SetCache(string key, object value)
        {
            string str = "AVCache";
            return this.SaveFile(key + ".cache", AVClient.SerializeJsonString(value as IDictionary<string, object>), str);
        }

        public async Task<IDictionary<string, object>> GetCache(string key)
        {
            IDictionary<string, object> dictionary = (IDictionary<string, object>) null;
            string str = "AVCache";
            string file =await this.GetFile(key + ".cache", str);
            if (!string.IsNullOrEmpty(file))
                dictionary = AVClient.DeserializeJsonString(file);
            return dictionary;
        }

        public bool RemoveCache(string key)
        {
            bool flag = true;
            string str = "AVCache";
            string fileName = key + ".cache";
            try
            {
                this.RemoveFile(fileName, str);
            }
            catch
            {
                flag = false;
            }
            return flag;
        }

        public void Initialize()
        {
            AVObject.RegisterSubclass<AVInstallation>();
        }

        public void ClearInMemoryInstallation()
        {
            AVInstallation.ClearInMemoryInstallation();
        }

        public IEnumerable<AVNetworkProfile> RetrieveNetwork()
        {
            List<AVNetworkProfile> list = new List<AVNetworkProfile>();
            switch (NetworkInformation.GetInternetConnectionProfile().NetworkAdapter.IanaInterfaceType)
            {
                case 71U:
                case 6U:
                    list.Add(AVNetworkProfile.WIFI);
                    break;
                case 243U:
                case 244U:
                    list.Add(AVNetworkProfile.Mobile_3G);
                    break;
            }
            return (IEnumerable<AVNetworkProfile>) list;
        }

        public async Task<bool> SaveFile(string fileName, string content, params string[] paths)
        {
            try
            {
                if (paths == null||paths.Length==0)
                {
                    IStorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                    await FileIO.WriteTextAsync(file, content);
                }
                else
                {
                    StorageFolder folder = ApplicationData.Current.LocalFolder;
                    for (int i = 0; i < paths.Length; i++)
                    {
                        var fileItem = await folder.TryGetItemAsync(paths[i]);
                        if (fileItem == null)
                        {
                            folder = await folder.CreateFolderAsync(paths[i]);
                        }
                        else
                        {
                            folder =await folder.GetFolderAsync(paths[i]);
                        }
                        if (i == paths.Length - 1)
                        {
                            IStorageFile file = await folder.CreateFileAsync(paths[i], CreationCollisionOption.ReplaceExisting);
                            await FileIO.WriteTextAsync(file, content);
                        }
                    }
                }
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> GetFile(string fileName, params string[] paths)
        {
            string str1 = string.Empty;
            if (paths == null || paths.Length == 0)
            {
                var fileitem = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
                if (fileitem != null)
                {
                    var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                    str1 = await FileIO.ReadTextAsync(file);
                }
            }
            else
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                for (int i = 0; i < paths.Length; i++)
                {
                    var fileItem = await folder.TryGetItemAsync(paths[i]);
                    if (fileItem == null)
                    {
                        folder = await folder.CreateFolderAsync(paths[i]);
                    }
                    else
                    {
                        folder = await folder.GetFolderAsync(paths[i]);
                    }
                    if (i == paths.Length - 1)
                    {
                        var fileitem = await folder.TryGetItemAsync(fileName);
                        if (fileitem != null)
                        {
                            var file = await ApplicationData.Current.LocalFolder.GetFileAsync(fileName);
                            str1 = await FileIO.ReadTextAsync(file);
                        }
                    }
                }
            }
            return str1;
        }

        public async void RemoveFile(string fileName, params string[] paths)
        {
            try
            {
                string path1 = string.Empty;
                if (paths != null)
                {
                    path1 = paths.Aggregate(path1, (current, path2) => Path.Combine(current, path2));
                }
                try
                {
                   var file=await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///" + path1 + "/" + fileName));
                   await file.DeleteAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task<Tuple<HttpStatusCode, string>> UploadAsync(Uri uri,
            IList<KeyValuePair<string, string>> headers, Stream data, IProgress<AVUploadProgressEventArgs> progress,
            CancellationToken cancellationToken)
        {
            if (progress == null)
                progress = (IProgress<AVUploadProgressEventArgs>) new Progress<AVUploadProgressEventArgs>();
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(uri);
            //req.AllowWriteStreamBuffering = false;
            req.Method = "POST";
            req.Headers[HttpRequestHeader.UserAgent]= AVClient.versionString;
            cancellationToken.Register((Action) (() => ((WebRequest) req).Abort()));
            if (headers != null)
            {
                foreach (
                    KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) headers)
                {
                    if (keyValuePair.Key == "Content-Type")
                        req.ContentType = keyValuePair.Value;
                    else
                        (req.Headers)[keyValuePair.Key] = keyValuePair.Value;
                }
            }
            cancellationToken.ThrowIfCancellationRequested();
            long totalLength = -1L;
            long readSoFar = 0L;
            try
            {
                totalLength = data.Length;
            }
            catch (NotSupportedException ex)
            {
            }
            if (totalLength == -1L)
            {
                MemoryStream memStream = new MemoryStream();
                await data.CopyToAsync((Stream) memStream);
                memStream.Seek(0L, SeekOrigin.Begin);
                totalLength = memStream.Length;
                req.Headers[HttpRequestHeader.ContentLength]= totalLength.ToString();
                data = (Stream) memStream;
            }
            req.Headers[HttpRequestHeader.ContentLength] = totalLength.ToString();
            progress.Report(new AVUploadProgressEventArgs()
            {
                Progress = 0.0
            });
            using (
                Stream stream =
                    await
                        new TaskFactory<Stream>(cancellationToken).FromAsync(
                            ((WebRequest) req).BeginGetRequestStream,
                            ((WebRequest) req).EndGetRequestStream,
                            (object) TaskCreationOptions.None))
            {
                int bytesRead = 0;
                byte[] buffer = new byte[Math.Max(totalLength/1000L, 25600L)];
                do
                {
                    bytesRead = await data.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                    await stream.WriteAsync(buffer, 0, bytesRead, cancellationToken);
                    readSoFar += (long) bytesRead;
                    progress.Report(new AVUploadProgressEventArgs()
                    {
                        Progress = 1.0*(double) readSoFar/(double) totalLength
                    });
                } while (bytesRead > 0);
            }
            HttpWebResponse response;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                response =
                    (HttpWebResponse)
                        await
                            new TaskFactory<WebResponse>(cancellationToken).FromAsync(
                                new Func<AsyncCallback, object, IAsyncResult>(((WebRequest) req).BeginGetResponse),
                                new Func<IAsyncResult, WebResponse>(((WebRequest) req).EndGetResponse),
                                (object) TaskCreationOptions.None);
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse) ex.Response;
            }
            Tuple<HttpStatusCode, string> tuple;
            using (response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8, true);
                string responseString = await reader.ReadToEndAsync();
                tuple = new Tuple<HttpStatusCode, string>(response.StatusCode, responseString);
            }
            return tuple;
        }

        public async Task<Tuple<HttpStatusCode, string>> RequestAsync(Uri uri, string method,
            IList<KeyValuePair<string, string>> headers, Stream data, string contentType,
            CancellationToken cancellationToken)
        {
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(uri);
            req.Method = method;
            req.Headers[HttpRequestHeader.UserAgent] = AVClient.versionString;
            cancellationToken.Register((Action) (() => ((WebRequest) req).Abort()));
            if (headers != null)
            {
                foreach (
                    KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) headers)
                    (req.Headers)[keyValuePair.Key] = keyValuePair.Value;
            }
            if (data != null)
            {
                if (contentType != null)
                    req.ContentType = contentType;
                cancellationToken.ThrowIfCancellationRequested();
                using (
                    Stream destination =
                        await
                            new TaskFactory<Stream>(cancellationToken).FromAsync(
                                ((WebRequest) req).BeginGetRequestStream,
                                ((WebRequest) req).EndGetRequestStream,
                                (object) TaskCreationOptions.None))
                    await data.CopyToAsync(destination);
            }
            HttpWebResponse response;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                response =
                    (HttpWebResponse)
                        await
                            new TaskFactory<WebResponse>(cancellationToken).FromAsync(
                                ((WebRequest) req).BeginGetResponse,
                                ((WebRequest) req).EndGetResponse,
                                (object) TaskCreationOptions.None);
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse) ex.Response;
            }
            string str = string.Empty;
            Tuple<HttpStatusCode, string> tuple;
            using (StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8, true))
            {
                str = streamReader.ReadToEnd();
                tuple = new Tuple<HttpStatusCode, string>(response.StatusCode, str);
            }
            return tuple;
        }

        public async Task<Tuple<HttpStatusCode, byte[]>> DownloadAysnc(Uri uri,
            IList<KeyValuePair<string, string>> headers, IProgress<AVDownloadProgressEventArgs> progress,
            CancellationToken cancellationToken)
        {
            HttpWebRequest req = (HttpWebRequest) WebRequest.Create(uri);
            req.Method = "GET";
            if (headers != null)
            {
                foreach (
                    KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) headers)
                     req.Headers[keyValuePair.Key] = keyValuePair.Value;
            }
            HttpWebResponse response;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                response =
                    (HttpWebResponse)
                        await
                            new TaskFactory<WebResponse>(cancellationToken).FromAsync(
                                ((WebRequest) req).BeginGetResponse,
                                ((WebRequest) req).EndGetResponse,
                                (object) TaskCreationOptions.None);
            }
            catch (WebException ex)
            {
                response = (HttpWebResponse) ex.Response;
            }
            Tuple<HttpStatusCode, byte[]> tuple;
            using (new StreamReader(response.GetResponseStream(), Encoding.UTF8, true))
            {
                byte[] buffer = new byte[response.GetResponseStream().Length];
                response.GetResponseStream().Read(buffer, 0, (int) response.GetResponseStream().Length);
                tuple = new Tuple<HttpStatusCode, byte[]>(response.StatusCode, buffer);
            }
            return tuple;
        }

        private class SettingsWrapper : IDictionary<string, object>, ICollection<KeyValuePair<string, object>>,
            IEnumerable<KeyValuePair<string, object>>, IEnumerable
        {
            private static readonly string prefix = "AVOSCloud.";
            private static PlatformHooks.SettingsWrapper wrapper;
            private IDictionary<string, object> data;

            public static PlatformHooks.SettingsWrapper Wrapper
            {
                get
                {
                    PlatformHooks.SettingsWrapper.wrapper = PlatformHooks.SettingsWrapper.wrapper ??
                                                            new PlatformHooks.SettingsWrapper();
                    return PlatformHooks.SettingsWrapper.wrapper;
                }
            }

            public ICollection<string> Keys
            {
                get
                {
                    return
                        (ICollection<string>)
                            Enumerable.ToList<string>(
                                Enumerable.Select<KeyValuePair<string, object>, string>(
                                    (IEnumerable<KeyValuePair<string, object>>) this,
                                    (Func<KeyValuePair<string, object>, string>) (kvp => kvp.Key)));
                }
            }

            public ICollection<object> Values
            {
                get
                {
                    return
                        this.Select(kvp => kvp.Value).ToList<object>();
                }
            }

            public object this[string key]
            {
                get { return this.data[PlatformHooks.SettingsWrapper.prefix + key]; }
                set { this.data[PlatformHooks.SettingsWrapper.prefix + key] = value; }
            }

            public int Count
            {
                get { return this.Keys.Count; }
            }

            public bool IsReadOnly
            {
                get { return this.data.IsReadOnly; }
            }

            private SettingsWrapper()
            {
                this.data = (IDictionary<string, object>) ApplicationData.Current.LocalSettings.Values;
            }

            public void Add(string key, object value)
            {
                this.data.Add(PlatformHooks.SettingsWrapper.prefix + key, value);
            }

            public bool ContainsKey(string key)
            {
                return this.data.ContainsKey(PlatformHooks.SettingsWrapper.prefix + key);
            }

            public bool Remove(string key)
            {
                return this.data.Remove(PlatformHooks.SettingsWrapper.prefix + key);
            }

            public bool TryGetValue(string key, out object value)
            {
                return this.data.TryGetValue(PlatformHooks.SettingsWrapper.prefix + key, out value);
            }

            public void Add(KeyValuePair<string, object> item)
            {
                this.data.Add(new KeyValuePair<string, object>(PlatformHooks.SettingsWrapper.prefix + item.Key,
                    item.Value));
            }

            public void Clear()
            {
                foreach (string key in (IEnumerable<string>) this.Keys)
                    this.data.Remove(key);
            }

            public bool Contains(KeyValuePair<string, object> item)
            {
                return
                    this.data.Contains(new KeyValuePair<string, object>(
                        PlatformHooks.SettingsWrapper.prefix + item.Key, item.Value));
            }

            public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
            {
                Enumerable.ToList<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>) this)
                    .CopyTo(array, arrayIndex);
            }

            public bool Remove(KeyValuePair<string, object> item)
            {
                return
                    this.data.Remove(new KeyValuePair<string, object>(PlatformHooks.SettingsWrapper.prefix + item.Key,
                        item.Value));
            }

            public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
            {
                return
                    this.data.Where(kvp => kvp.Key.StartsWith(prefix)).Select(kvp =>
                        new KeyValuePair<string, object>(
                            kvp.Key.Substring(prefix.Length), kvp.Value))
                        .GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return (IEnumerator) this.GetEnumerator();
            }
        }
    }
}