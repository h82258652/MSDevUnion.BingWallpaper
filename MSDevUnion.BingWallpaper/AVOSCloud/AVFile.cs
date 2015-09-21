using AVOSCloud.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;

namespace AVOSCloud
{
    public class AVFile
    {
        private object mutex = new object();
        private TaskQueue taskQueue = new TaskQueue();
        private int chunkBits = 16;
        private int chunkSize = 65536;
        private IList<QiniuBlock> qiniuBlocks = (IList<QiniuBlock>) new List<QiniuBlock>();
        internal static string UP_HOST = "http://upload.qiniu.com";
        private static int BLOCKSIZE = 4194304;

        private static readonly Dictionary<string, string> MIMETypesDictionary = new Dictionary<string, string>()
        {
            {
                "ai",
                "application/postscript"
            },
            {
                "aif",
                "audio/x-aiff"
            },
            {
                "aifc",
                "audio/x-aiff"
            },
            {
                "aiff",
                "audio/x-aiff"
            },
            {
                "asc",
                "text/plain"
            },
            {
                "atom",
                "application/atom+xml"
            },
            {
                "au",
                "audio/basic"
            },
            {
                "avi",
                "video/x-msvideo"
            },
            {
                "bcpio",
                "application/x-bcpio"
            },
            {
                "bin",
                "application/octet-stream"
            },
            {
                "bmp",
                "image/bmp"
            },
            {
                "cdf",
                "application/x-netcdf"
            },
            {
                "cgm",
                "image/cgm"
            },
            {
                "class",
                "application/octet-stream"
            },
            {
                "cpio",
                "application/x-cpio"
            },
            {
                "cpt",
                "application/mac-compactpro"
            },
            {
                "csh",
                "application/x-csh"
            },
            {
                "css",
                "text/css"
            },
            {
                "dcr",
                "application/x-director"
            },
            {
                "dif",
                "video/x-dv"
            },
            {
                "dir",
                "application/x-director"
            },
            {
                "djv",
                "image/vnd.djvu"
            },
            {
                "djvu",
                "image/vnd.djvu"
            },
            {
                "dll",
                "application/octet-stream"
            },
            {
                "dmg",
                "application/octet-stream"
            },
            {
                "dms",
                "application/octet-stream"
            },
            {
                "doc",
                "application/msword"
            },
            {
                "docx",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
            },
            {
                "dotx",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.template"
            },
            {
                "docm",
                "application/vnd.ms-word.document.macroEnabled.12"
            },
            {
                "dotm",
                "application/vnd.ms-word.template.macroEnabled.12"
            },
            {
                "dtd",
                "application/xml-dtd"
            },
            {
                "dv",
                "video/x-dv"
            },
            {
                "dvi",
                "application/x-dvi"
            },
            {
                "dxr",
                "application/x-director"
            },
            {
                "eps",
                "application/postscript"
            },
            {
                "etx",
                "text/x-setext"
            },
            {
                "exe",
                "application/octet-stream"
            },
            {
                "ez",
                "application/andrew-inset"
            },
            {
                "gif",
                "image/gif"
            },
            {
                "gram",
                "application/srgs"
            },
            {
                "grxml",
                "application/srgs+xml"
            },
            {
                "gtar",
                "application/x-gtar"
            },
            {
                "hdf",
                "application/x-hdf"
            },
            {
                "hqx",
                "application/mac-binhex40"
            },
            {
                "htm",
                "text/html"
            },
            {
                "html",
                "text/html"
            },
            {
                "ice",
                "x-conference/x-cooltalk"
            },
            {
                "ico",
                "image/x-icon"
            },
            {
                "ics",
                "text/calendar"
            },
            {
                "ief",
                "image/ief"
            },
            {
                "ifb",
                "text/calendar"
            },
            {
                "iges",
                "model/iges"
            },
            {
                "igs",
                "model/iges"
            },
            {
                "jnlp",
                "application/x-java-jnlp-file"
            },
            {
                "jp2",
                "image/jp2"
            },
            {
                "jpe",
                "image/jpeg"
            },
            {
                "jpeg",
                "image/jpeg"
            },
            {
                "jpg",
                "image/jpeg"
            },
            {
                "js",
                "application/x-javascript"
            },
            {
                "kar",
                "audio/midi"
            },
            {
                "latex",
                "application/x-latex"
            },
            {
                "lha",
                "application/octet-stream"
            },
            {
                "lzh",
                "application/octet-stream"
            },
            {
                "m3u",
                "audio/x-mpegurl"
            },
            {
                "m4a",
                "audio/mp4a-latm"
            },
            {
                "m4b",
                "audio/mp4a-latm"
            },
            {
                "m4p",
                "audio/mp4a-latm"
            },
            {
                "m4u",
                "video/vnd.mpegurl"
            },
            {
                "m4v",
                "video/x-m4v"
            },
            {
                "mac",
                "image/x-macpaint"
            },
            {
                "man",
                "application/x-troff-man"
            },
            {
                "mathml",
                "application/mathml+xml"
            },
            {
                "me",
                "application/x-troff-me"
            },
            {
                "mesh",
                "model/mesh"
            },
            {
                "mid",
                "audio/midi"
            },
            {
                "midi",
                "audio/midi"
            },
            {
                "mif",
                "application/vnd.mif"
            },
            {
                "mov",
                "video/quicktime"
            },
            {
                "movie",
                "video/x-sgi-movie"
            },
            {
                "mp2",
                "audio/mpeg"
            },
            {
                "mp3",
                "audio/mpeg"
            },
            {
                "mp4",
                "video/mp4"
            },
            {
                "mpe",
                "video/mpeg"
            },
            {
                "mpeg",
                "video/mpeg"
            },
            {
                "mpg",
                "video/mpeg"
            },
            {
                "mpga",
                "audio/mpeg"
            },
            {
                "ms",
                "application/x-troff-ms"
            },
            {
                "msh",
                "model/mesh"
            },
            {
                "mxu",
                "video/vnd.mpegurl"
            },
            {
                "nc",
                "application/x-netcdf"
            },
            {
                "oda",
                "application/oda"
            },
            {
                "ogg",
                "application/ogg"
            },
            {
                "pbm",
                "image/x-portable-bitmap"
            },
            {
                "pct",
                "image/pict"
            },
            {
                "pdb",
                "chemical/x-pdb"
            },
            {
                "pdf",
                "application/pdf"
            },
            {
                "pgm",
                "image/x-portable-graymap"
            },
            {
                "pgn",
                "application/x-chess-pgn"
            },
            {
                "pic",
                "image/pict"
            },
            {
                "pict",
                "image/pict"
            },
            {
                "png",
                "image/png"
            },
            {
                "pnm",
                "image/x-portable-anymap"
            },
            {
                "pnt",
                "image/x-macpaint"
            },
            {
                "pntg",
                "image/x-macpaint"
            },
            {
                "ppm",
                "image/x-portable-pixmap"
            },
            {
                "ppt",
                "application/vnd.ms-powerpoint"
            },
            {
                "pptx",
                "application/vnd.openxmlformats-officedocument.presentationml.presentation"
            },
            {
                "potx",
                "application/vnd.openxmlformats-officedocument.presentationml.template"
            },
            {
                "ppsx",
                "application/vnd.openxmlformats-officedocument.presentationml.slideshow"
            },
            {
                "ppam",
                "application/vnd.ms-powerpoint.addin.macroEnabled.12"
            },
            {
                "pptm",
                "application/vnd.ms-powerpoint.presentation.macroEnabled.12"
            },
            {
                "potm",
                "application/vnd.ms-powerpoint.template.macroEnabled.12"
            },
            {
                "ppsm",
                "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"
            },
            {
                "ps",
                "application/postscript"
            },
            {
                "qt",
                "video/quicktime"
            },
            {
                "qti",
                "image/x-quicktime"
            },
            {
                "qtif",
                "image/x-quicktime"
            },
            {
                "ra",
                "audio/x-pn-realaudio"
            },
            {
                "ram",
                "audio/x-pn-realaudio"
            },
            {
                "ras",
                "image/x-cmu-raster"
            },
            {
                "rdf",
                "application/rdf+xml"
            },
            {
                "rgb",
                "image/x-rgb"
            },
            {
                "rm",
                "application/vnd.rn-realmedia"
            },
            {
                "roff",
                "application/x-troff"
            },
            {
                "rtf",
                "text/rtf"
            },
            {
                "rtx",
                "text/richtext"
            },
            {
                "sgm",
                "text/sgml"
            },
            {
                "sgml",
                "text/sgml"
            },
            {
                "sh",
                "application/x-sh"
            },
            {
                "shar",
                "application/x-shar"
            },
            {
                "silo",
                "model/mesh"
            },
            {
                "sit",
                "application/x-stuffit"
            },
            {
                "skd",
                "application/x-koan"
            },
            {
                "skm",
                "application/x-koan"
            },
            {
                "skp",
                "application/x-koan"
            },
            {
                "skt",
                "application/x-koan"
            },
            {
                "smi",
                "application/smil"
            },
            {
                "smil",
                "application/smil"
            },
            {
                "snd",
                "audio/basic"
            },
            {
                "so",
                "application/octet-stream"
            },
            {
                "spl",
                "application/x-futuresplash"
            },
            {
                "src",
                "application/x-wais-source"
            },
            {
                "sv4cpio",
                "application/x-sv4cpio"
            },
            {
                "sv4crc",
                "application/x-sv4crc"
            },
            {
                "svg",
                "image/svg+xml"
            },
            {
                "swf",
                "application/x-shockwave-flash"
            },
            {
                "t",
                "application/x-troff"
            },
            {
                "tar",
                "application/x-tar"
            },
            {
                "tcl",
                "application/x-tcl"
            },
            {
                "tex",
                "application/x-tex"
            },
            {
                "texi",
                "application/x-texinfo"
            },
            {
                "texinfo",
                "application/x-texinfo"
            },
            {
                "tif",
                "image/tiff"
            },
            {
                "tiff",
                "image/tiff"
            },
            {
                "tr",
                "application/x-troff"
            },
            {
                "tsv",
                "text/tab-separated-values"
            },
            {
                "txt",
                "text/plain"
            },
            {
                "ustar",
                "application/x-ustar"
            },
            {
                "vcd",
                "application/x-cdlink"
            },
            {
                "vrml",
                "model/vrml"
            },
            {
                "vxml",
                "application/voicexml+xml"
            },
            {
                "wav",
                "audio/x-wav"
            },
            {
                "wbmp",
                "image/vnd.wap.wbmp"
            },
            {
                "wbmxl",
                "application/vnd.wap.wbxml"
            },
            {
                "wml",
                "text/vnd.wap.wml"
            },
            {
                "wmlc",
                "application/vnd.wap.wmlc"
            },
            {
                "wmls",
                "text/vnd.wap.wmlscript"
            },
            {
                "wmlsc",
                "application/vnd.wap.wmlscriptc"
            },
            {
                "wrl",
                "model/vrml"
            },
            {
                "xbm",
                "image/x-xbitmap"
            },
            {
                "xht",
                "application/xhtml+xml"
            },
            {
                "xhtml",
                "application/xhtml+xml"
            },
            {
                "xls",
                "application/vnd.ms-excel"
            },
            {
                "xml",
                "application/xml"
            },
            {
                "xpm",
                "image/x-xpixmap"
            },
            {
                "xsl",
                "application/xml"
            },
            {
                "xlsx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            },
            {
                "xltx",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.template"
            },
            {
                "xlsm",
                "application/vnd.ms-excel.sheet.macroEnabled.12"
            },
            {
                "xltm",
                "application/vnd.ms-excel.template.macroEnabled.12"
            },
            {
                "xlam",
                "application/vnd.ms-excel.addin.macroEnabled.12"
            },
            {
                "xlsb",
                "application/vnd.ms-excel.sheet.binary.macroEnabled.12"
            },
            {
                "xslt",
                "application/xslt+xml"
            },
            {
                "xul",
                "application/vnd.mozilla.xul+xml"
            },
            {
                "xwd",
                "image/x-xwindowdump"
            },
            {
                "xyz",
                "chemical/x-xyz"
            },
            {
                "zip",
                "application/zip"
            }
        };

        private const int blockBits = 22;
        private const int blockMashk = 4194303;
        private const int TryTimes = 3;
        private string name;
        private Uri uri;
        private string token;
        private string bucket;
        private string bucketId;
        private bool isExternal;
        private Stream dataStream;
        private string objectId;
        private IDictionary<string, object> metaData;

        public bool IsDirty
        {
            get
            {
                lock (this.mutex)
                    return this.dataStream != null && this.objectId == null;
            }
        }

        [AVFieldName("name")]
        public string Name
        {
            get
            {
                lock (this.mutex)
                    return this.name;
            }
        }

        [AVFieldName("url")]
        public Uri Url
        {
            get
            {
                lock (this.mutex)
                    return this.uri;
            }
        }

        public bool IsExternal
        {
            get { return this.isExternal; }
        }

        public IDictionary<string, object> MetaData
        {
            get { return this.metaData; }
        }

        public string ObjectId
        {
            get
            {
                lock (this.mutex)
                    return this.objectId;
            }
        }

        public byte[] DataByte
        {
            get
            {
                byte[] buffer = new byte[16384];
                if (this.dataStream == null)
                    return buffer;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    this.dataStream.Seek(0L, SeekOrigin.Begin);
                    int count;
                    while ((count = this.dataStream.Read(buffer, 0, buffer.Length)) > 0)
                        memoryStream.Write(buffer, 0, count);
                    return memoryStream.ToArray();
                }
            }
        }

        public string LocalPath { get; private set; }

        internal long Size
        {
            get { return long.Parse(this.MetaData["size"].ToString()); }
        }

        internal string CloudName
        {
            get { return this.GenerateLocalFileName(); }
        }

        internal AVFile(string name, Uri uri)
        {
            this.name = name;
            this.uri = uri;
        }

        public AVFile(string name, string url)
        {
            this.isExternal = true;
            this.name = name;
            this.uri = new Uri(url);
        }

        public AVFile(string name, byte[] data)
            : this(name, (Stream) new MemoryStream(data), (IDictionary<string, object>) new Dictionary<string, object>()
                )
        {
        }

        public AVFile(string name, byte[] data, IDictionary<string, object> metaData)
            : this(name, (Stream) new MemoryStream(data), metaData)
        {
        }

        public AVFile(string name, Stream data, IDictionary<string, object> metaData)
        {
            this.name = name;
            this.dataStream = data;
            this.metaData = metaData;
        }

        public AVFile(string name, Stream data)
            : this(name, data, (IDictionary<string, object>) new Dictionary<string, object>())
        {
        }

        internal AVFile(string name, string url, IDictionary<string, object> metaData)
        {
            this.name = name;
            this.uri = new Uri(url);
            this.metaData = metaData;
        }

        private static string GetMIMEType(string fileName)
        {
            try
            {
                string key = Path.GetExtension(fileName).Remove(0, 1);
                if (!AVFile.MIMETypesDictionary.ContainsKey(key))
                    return "unknown/unknown";
                else
                    return AVFile.MIMETypesDictionary[key];
            }
            catch
            {
                return "unknown/unknown";
            }
        }

        internal void MergeFromJSON(IDictionary<string, object> jsonData)
        {
            lock (this.mutex)
            {
                string local_0 = jsonData["url"] as string;
                this.uri = new Uri(local_0, UriKind.Absolute);
                this.bucketId = this.FetchBucketId(local_0);
                this.token = jsonData["token"] as string;
                this.bucket = jsonData["bucket"] as string;
                this.objectId = jsonData["objectId"] as string;
            }
        }

        private string FetchBucketId(string url)
        {
            string[] strArray = url.Split('/');
            return strArray[strArray.Length - 1];
        }

        internal IDictionary<string, object> GetMetaData()
        {
            IDictionary<string, object> dic = (IDictionary<string, object>) new Dictionary<string, object>();
            if (this.metaData != null)
            {
                foreach (
                    KeyValuePair<string, object> keyValuePair in
                        (IEnumerable<KeyValuePair<string, object>>) this.metaData)
                    dic.Add(keyValuePair.Key, keyValuePair.Value);
            }
            this.MergeDic(dic, "size", (object) this.dataStream.Length);
            this.MergeDic(dic, "_checksum", (object) AVFile.GetMD5Code(this.dataStream));
            if (AVUser.CurrentUser != null && AVUser.CurrentUser.ObjectId != null)
                this.MergeDic(dic, "owner", (object) AVUser.CurrentUser.ObjectId);
            if (this.IsExternal)
                this.MergeDic(dic, "__source", (object) "external");
            return dic;
        }

        internal void MergeDic(IDictionary<string, object> dic, string key, object value)
        {
            if (dic.ContainsKey(key))
                dic[key] = value;
            else
                dic.Add(key, value);
        }

        internal static string GetMD5Code(Stream data)
        {
            byte[] hash = new MD5CryptoServiceProvider().ComputeHash(data);
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < hash.Length; ++index)
                stringBuilder.Append(hash[index].ToString("x2"));
            return ((object) stringBuilder).ToString();
        }

        public Task SaveAsync()
        {
            return this.SaveAsync((IProgress<AVUploadProgressEventArgs>) null, CancellationToken.None);
        }

        public Task SaveAsync(CancellationToken cancellationToken)
        {
            return this.SaveAsync((IProgress<AVUploadProgressEventArgs>) null, cancellationToken);
        }

        public Task SaveAsync(IProgress<AVUploadProgressEventArgs> progress)
        {
            return this.SaveAsync(progress, CancellationToken.None);
        }

        public Task SaveAsync(IProgress<AVUploadProgressEventArgs> progress, CancellationToken cancellationToken)
        {
            return
                this.taskQueue.Enqueue<Task>(
                    (Func<Task, Task>) (toAwait => this.SaveAsync(toAwait, progress, cancellationToken)),
                    cancellationToken);
        }

        private Task SaveAsync(Task toAwait, IProgress<AVUploadProgressEventArgs> progress,
            CancellationToken cancellationToken)
        {
            lock (this.mutex)
            {
                string currentSessionToken = AVUser.CurrentSessionToken;
            }
            long position = this.dataStream.Position;
            return toAwait.OnSuccess<Task>(t => {
                Task task1 = null;
                lock (this.mutex)
                {
                    if (!this.IsDirty)
                    {
                        task1 = Task.FromResult<int>(0);
                    }
                    else
                    {
                        string str = Guid.NewGuid().ToString();
                        str = string.Concat(str, Path.GetExtension(this.name));
                        task1 = this.GetQiniuToken(str, cancellationToken).ContinueWith(s => {
                            this.MergeFromJSON(s.Result.Item2);
                            return this.ResumableUploadToQiniu(progress, str, cancellationToken);
                        }, cancellationToken).Unwrap().ContinueWith<Task>(d => {
                            Task task = null;
                            task = (d.Result.Item1 == HttpStatusCode.OK ? Task.FromResult<int>(0) : this.DeleteAsync(cancellationToken));
                            return task;
                        }, cancellationToken).Unwrap();
                    }
                }
                return task1;
            }).Unwrap().ContinueWith<Task>((Task t) => {
                if ((t.IsFaulted || t.IsCanceled) && this.dataStream.CanSeek)
                {
                    this.dataStream.Seek(position, SeekOrigin.Begin);
                }
                return t;
            }, cancellationToken).Unwrap();
        }

        public Task DeleteAsync()
        {
            return this.DeleteAsync(CancellationToken.None);
        }

        public Task DownloadAsync()
        {
            return this.DownloadAsync(string.Empty, (IProgress<AVDownloadProgressEventArgs>) null,
                CancellationToken.None);
        }

        public Task DownloadAsync(IProgress<AVDownloadProgressEventArgs> progress)
        {
            return this.DownloadAsync(string.Empty, progress, CancellationToken.None);
        }

        public Task DownloadAsync(string fileRename, IProgress<AVDownloadProgressEventArgs> progress,
            CancellationToken cancellationToken)
        {
            long fileSize = long.Parse(this.MetaData["size"].ToString());
            MemoryStream ms = new MemoryStream();
            this.SetChunkStrategy();
            IList<Tuple<long, long>> downloadChunks = this.GetDownloadChunks(fileSize, this.chunkSize, 0L);
            double completeCount = 0.0;
            double chunkCount = (double) downloadChunks.Count;
            Action<byte[]> afterLast = (Action<byte[]>) (chunkData =>
            {
                ms.Write(chunkData, 0, chunkData.Length);
                lock (this.mutex)
                {
                    ++completeCount;
                    if (progress == null)
                        return;
                    progress.Report(new AVDownloadProgressEventArgs()
                    {
                        Progress = this.CalcProgress(completeCount, chunkCount)
                    });
                }
            });
            return
                InternalExtensions.Chaining<Tuple<long, long>, byte[]>((IEnumerable<Tuple<long, long>>) downloadChunks,
                    new Func<Tuple<long, long>, Task<byte[]>>(this.DownloadChunkAsync),
                    (Func<byte[], Tuple<long, long>, Tuple<long, long>>) null, afterLast, (byte[]) null)
                    .ContinueWith((Action<Task<IEnumerable<byte[]>>>) (s =>
                    {
                        long length = ms.Length;
                        this.dataStream = (Stream) ms;
                    }));
        }

        private double CalcProgress(double already, double total)
        {
            return Math.Round(1.0*already/total, 3);
        }

        private IList<Tuple<long, long>> GetDownloadChunks(long fileSize, int chunkSize, long startPosition)
        {
            IList<Tuple<long, long>> list = (IList<Tuple<long, long>>) new List<Tuple<long, long>>();
            long num1 = (fileSize + (long) chunkSize + 1L)/(long) chunkSize;
            for (int index = 0; (long) index < num1; ++index)
            {
                int num2 = chunkSize;
                if ((long) index == num1 - 1L)
                    num2 = (int) (fileSize - (long) (index*chunkSize));
                long num3 = startPosition + (long) (index*chunkSize) + 1L;
                long num4 = startPosition + (long) (index*chunkSize) + (long) num2;
                if (index == 0)
                    num3 = startPosition;
                list.Add(new Tuple<long, long>(num3, num4));
            }
            return list;
        }

        internal Task<Tuple<int, byte[]>> DownloadChunkAsync(long startPosition, int index, long totalSize,
            int chunkCount)
        {
            long lastPosition = startPosition + (long) ((index + 1)*this.chunkSize);
            if (index == chunkCount - 1)
                lastPosition = totalSize - (startPosition + (long) ((chunkCount - 1)*this.chunkSize));
            return
                InternalExtensions.OnSuccess<byte[], Tuple<int, byte[]>>(
                    this.DownloadChunkAsync(startPosition + (long) (index*this.chunkSize), lastPosition,
                        CancellationToken.None),
                    (Func<Task<byte[]>, Tuple<int, byte[]>>) (t => new Tuple<int, byte[]>(index, t.Result)));
        }

        internal Task<byte[]> DownloadChunkAsync(Tuple<long, long> chunkTuple)
        {
            return this.DownloadChunkAsync(chunkTuple.Item1, chunkTuple.Item2, CancellationToken.None);
        }

        internal Task<byte[]> DownloadChunkAsync(long firstPosition, long lastPosition,
            CancellationToken cancellationToken)
        {
            string format = "bytes={0}-{1}";
            IList<KeyValuePair<string, string>> headers =
                (IList<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>();
            headers.Add(new KeyValuePair<string, string>("Range",
                string.Format(format, (object) firstPosition, (object) lastPosition)));
            return
                InternalExtensions.OnSuccess<Tuple<HttpStatusCode, byte[]>, byte[]>(
                    AVClient.DownloadAysnc(this.Url, headers, (IProgress<AVDownloadProgressEventArgs>) null,
                        cancellationToken), (Func<Task<Tuple<HttpStatusCode, byte[]>>, byte[]>) (t => t.Result.Item2));
        }

        private string GenerateLocalFileName()
        {
            string name = this.Name;
            string path;
            if (this.Url.IsFile)
            {
                path = Path.GetFileName(this.Url.LocalPath);
            }
            else
            {
                string extension = Path.GetExtension(name);
                path = Enumerable.Last<string>((IEnumerable<string>) this.Url.OriginalString.Split('/'));
                if (string.IsNullOrEmpty(Path.GetExtension(path)))
                    path = path + extension;
            }
            return path;
        }

        internal Task<byte[]> DownloadAsync(CancellationToken cancellationToken)
        {
            string currentSessionToken = AVUser.CurrentSessionToken;
            return
                InternalExtensions.OnSuccess<Tuple<HttpStatusCode, byte[]>, byte[]>(
                    AVClient.DownloadAysnc(this.Url, (IList<KeyValuePair<string, string>>) null,
                        (IProgress<AVDownloadProgressEventArgs>) null, cancellationToken),
                    (Func<Task<Tuple<HttpStatusCode, byte[]>>, byte[]>) (t => t.Result.Item2));
        }

        internal Task DeleteAsync(CancellationToken cancellationToken)
        {
            lock (this.mutex)
                return
                    (Task)
                        AVClient.RequestAsync("DELETE",
                            new Uri(string.Format("/files/{0}", (object) this.objectId), UriKind.Relative),
                            AVUser.CurrentSessionToken, (IDictionary<string, object>) null, cancellationToken);
        }

        public static Task<AVFile> GetFileWithObjectIdAsync(string objectId)
        {
            return AVFile.GetFileWithObjectIdAsync(objectId, CancellationToken.None);
        }

        public static async Task<AVFile> CreateFileWithLocalPath(string name, string path)
        {
            byte[] numArray;
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///" + path));
            using (var stream = await file.OpenStreamForReadAsync())
            {
                int length = (int)stream.Length;
                numArray = new byte[length];
                int offset = 0;
                int num;
                while ((num = stream.Read(numArray, offset, length - offset)) > 0)
                    offset += num;
            }
            return new AVFile(name, numArray);
        }

        public static Task<AVFile> GetFileWithObjectIdAsync(string objectId, CancellationToken cancellationToken)
        {
            string currentSessionToken = AVUser.CurrentSessionToken;
            return
                InternalExtensions.OnSuccess<Tuple<HttpStatusCode, IDictionary<string, object>>, AVFile>(
                    AVClient.RequestAsync("GET",
                        new Uri(string.Format("/files/{0}", (object) objectId), UriKind.Relative), currentSessionToken,
                        (IDictionary<string, object>) null, cancellationToken),
                    t =>
                    {
                        AVFile avFile = (AVFile) null;
                        if (t.Result.Item1 == HttpStatusCode.OK)
                        {
                            IDictionary<string, object> metaData =
                                t.Result.Item2["metaData"] as IDictionary<string, object>;
                            avFile = new AVFile(t.Result.Item2["name"] as string, t.Result.Item2["url"] as string,
                                metaData);
                        }
                        return avFile;
                    });
        }

        internal IDictionary<string, object> ToJSON()
        {
            if (this.IsDirty)
                throw new InvalidOperationException("AVFile must be saved before it can be serialized.");
            return (IDictionary<string, object>) new Dictionary<string, object>()
            {
                {
                    "__type",
                    (object) "File"
                },
                {
                    "id",
                    (object) this.objectId
                }
            };
        }

        internal Task<Tuple<HttpStatusCode, IDictionary<string, object>>> GetQiniuToken(string key,
            CancellationToken cancellationToken)
        {
            string currentSessionToken = AVUser.CurrentSessionToken;
            string fileName = this.name;
            IDictionary<string, object> data = (IDictionary<string, object>) new Dictionary<string, object>();
            data.Add("name", (object) this.Name);
            data.Add("key", (object) key);
            data.Add("__type", (object) "File");
            data.Add("mime_type", (object) AVFile.GetMIMEType(fileName));
            data.Add("metaData", (object) this.GetMetaData());
            return AVClient.RequestAsync("POST", new Uri("/qiniu", UriKind.Relative), currentSessionToken, data,
                cancellationToken);
        }

        private static IDictionary<string, object> getFormData(string upToken, string key)
        {
            IDictionary<string, object> dictionary = (IDictionary<string, object>) new Dictionary<string, object>();
            dictionary["token"] = (object) upToken;
            dictionary["key"] = (object) key;
            return dictionary;
        }

        internal void SetChunkStrategy()
        {
            try
            {
                if (
                    !Enumerable.Contains<AVNetworkProfile>(AVClient.platformHooks.RetrieveNetwork(),
                        AVNetworkProfile.WIFI))
                    return;
                this.chunkBits = 19;
                this.chunkSize = 1 << this.chunkBits;
            }
            catch
            {
            }
        }

        private int CalcBlockCount(long fsize)
        {
            return (int) (fsize + 4194303L >> 22);
        }

        private int CalcChunkCount(long fsize)
        {
            int num = (1 << this.chunkBits) - 1;
            return (int) (fsize + (long) num >> this.chunkBits);
        }

        internal Task<Tuple<HttpStatusCode, string>> ResumableUploadToQiniu(
            IProgress<AVUploadProgressEventArgs> progress, string key, CancellationToken cancellationToken)
        {
            long length = this.dataStream.Length;
            this.qiniuBlocks =
                (IList<QiniuBlock>) Enumerable.ToList<QiniuBlock>((IEnumerable<QiniuBlock>) this.CuttingBlock(length));
            int chunkCount = this.CalcChunkCount(length);
            double completedCount = 0.0;
            Action<QiniuChunk> reportFor = (Action<QiniuChunk>) (qb =>
            {
                if (progress == null)
                    return;
                lock (this.mutex)
                {
                    ++completedCount;
                    progress.Report(new AVUploadProgressEventArgs()
                    {
                        Progress = this.CalcProgress(completedCount, (double) chunkCount)
                    });
                }
            });
            return
                TaskExtensions.Unwrap<Tuple<HttpStatusCode, string>>(
                    InternalExtensions.Chaining<QiniuBlock, QiniuBlock>((IEnumerable<QiniuBlock>) this.qiniuBlocks,
                        new Func<QiniuBlock, Task<QiniuBlock>>(this.MakeBlock))
                        .ContinueWith<Task<Tuple<HttpStatusCode, string>>>(
                            (Func<Task<IEnumerable<QiniuBlock>>, Task<Tuple<HttpStatusCode, string>>>) (t =>
                            {
                                IEnumerable<QiniuBlock> result = t.Result;
                                List<Task> list = new List<Task>();
                                foreach (QiniuBlock qiniuBlock in result)
                                {
                                    Task<IEnumerable<QiniuChunk>> task =
                                        InternalExtensions.Chaining<QiniuChunk, QiniuChunk>(
                                            (IEnumerable<QiniuChunk>) qiniuBlock.UnprocessChunks,
                                            new Func<QiniuChunk, Task<QiniuChunk>>(this.PutChunk),
                                            new Func<QiniuChunk, QiniuChunk, QiniuChunk>(this.setCtc), reportFor,
                                            (QiniuChunk) null);
                                    list.Add((Task) task);
                                }
                                return
                                    TaskExtensions.Unwrap<Tuple<HttpStatusCode, string>>(
                                        Task.WhenAll((IEnumerable<Task>) list)
                                            .ContinueWith<Task<Tuple<HttpStatusCode, string>>>(
                                                (Func<Task, Task<Tuple<HttpStatusCode, string>>>)
                                                    (m =>
                                                        this.QiniuMakeFile(this.token, key, this.dataStream.Length,
                                                            Enumerable.ToArray<string>(
                                                                Enumerable.Select<QiniuBlock, string>(
                                                                    (IEnumerable<QiniuBlock>) this.qiniuBlocks,
                                                                    (Func<QiniuBlock, string>)
                                                                        (bl => bl.Chunks.Last.Value.Ctx))),
                                                            cancellationToken))));
                            })));
        }

        private QiniuChunk setCtc(QiniuChunk previous, QiniuChunk next)
        {
            if (previous != null)
            {
                next.Ctx = previous.Ctx;
                next.Offset = previous.Offset;
            }
            return next;
        }

        private LinkedList<QiniuBlock> CuttingBlock(long fsize)
        {
            LinkedList<QiniuBlock> ll = new LinkedList<QiniuBlock>();
            this.SetChunkStrategy();
            int num = this.CalcBlockCount(this.dataStream.Length);
            for (int blockIndexInFile = 0; blockIndexInFile < num; ++blockIndexInFile)
            {
                long length1 = this.dataStream.Length;
                long length2 = (long) AVFile.BLOCKSIZE;
                if (blockIndexInFile == num - 1)
                    length2 = length1 - (long) (blockIndexInFile*AVFile.BLOCKSIZE);
                byte[] numArray = new byte[length2];
                this.dataStream.Seek((long) (AVFile.BLOCKSIZE*blockIndexInFile), SeekOrigin.Begin);
                this.dataStream.Read(numArray, 0, (int) length2);
                QiniuBlock qiniuBlock = new QiniuBlock(blockIndexInFile, numArray, this.chunkSize);
                InternalExtensions.Append<QiniuBlock>(ll, qiniuBlock);
            }
            return ll;
        }

        private LinkedList<QiniuBlock> CuttingBlock()
        {
            return this.CuttingBlock(this.dataStream.Length);
        }

        private IList<KeyValuePair<string, string>> GetQiniuRequestHeaders()
        {
            IList<KeyValuePair<string, string>> list =
                (IList<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>();
            string str = "UpToken " + this.token;
            list.Add(new KeyValuePair<string, string>("Authorization", str));
            return list;
        }

        private Task<QiniuBlock> MakeBlock(QiniuBlock qbLocal)
        {
            return
                InternalExtensions.OnSuccess<Tuple<HttpStatusCode, string>, QiniuBlock>(
                    this.MakeBlock(qbLocal.Chunks.First.Value.ChunkData, qbLocal.BlockSize),
                    (Func<Task<Tuple<HttpStatusCode, string>>, QiniuBlock>) (t =>
                    {
                        Tuple<HttpStatusCode, IDictionary<string, object>> tuple = AVClient.ReponseResolve(t.Result,
                            CancellationToken.None);
                        long num1 = long.Parse(tuple.Item2["offset"].ToString());
                        string str1 = tuple.Item2["ctx"].ToString();
                        string str2 = tuple.Item2["host"].ToString();
                        string str3 = tuple.Item2["checksum"].ToString();
                        long num2 = long.Parse(tuple.Item2["crc32"].ToString());
                        qbLocal.Chunks.First.Value.Checksum = str3;
                        qbLocal.Chunks.First.Value.Crc32 = num2;
                        qbLocal.Chunks.First.Value.Ctx = str1;
                        qbLocal.Chunks.First.Value.Offset = num1;
                        qbLocal.Chunks.First.Value.Host = str2;
                        if (qbLocal.Chunks.First.Next != null)
                        {
                            qbLocal.Chunks.First.Next.Value.Ctx = str1;
                            qbLocal.Chunks.First.Next.Value.Offset = num1;
                        }
                        return qbLocal;
                    }));
        }

        private Task<QiniuChunk> PutChunk(QiniuChunk qChunkLocal)
        {
            return
                InternalExtensions.OnSuccess<Tuple<HttpStatusCode, string>, QiniuChunk>(
                    this.PutChunk(qChunkLocal.ChunkData, qChunkLocal.Ctx, (int) qChunkLocal.Offset),
                    (Func<Task<Tuple<HttpStatusCode, string>>, QiniuChunk>) (t =>
                    {
                        Tuple<HttpStatusCode, IDictionary<string, object>> tuple = AVClient.ReponseResolve(t.Result,
                            CancellationToken.None);
                        QiniuChunk qiniuChunk = new QiniuChunk();
                        long num1 = long.Parse(tuple.Item2["offset"].ToString());
                        string str1 = tuple.Item2["ctx"].ToString();
                        string str2 = tuple.Item2["host"].ToString();
                        string str3 = tuple.Item2["checksum"].ToString();
                        long num2 = long.Parse(tuple.Item2["crc32"].ToString());
                        qChunkLocal.Host = str2;
                        qChunkLocal.Offset = num1;
                        qChunkLocal.Checksum = str3;
                        qChunkLocal.Ctx = str1;
                        qChunkLocal.Crc32 = num2;
                        return qChunkLocal;
                    }));
        }

        private Task<Tuple<HttpStatusCode, string>> MakeBlock(byte[] firstChunkBinary, long blcokSize = 4194304L)
        {
            MemoryStream memoryStream = new MemoryStream(firstChunkBinary, 0, firstChunkBinary.Length);
            return
                AVClient.RequestAsync(
                    new Uri((string) (object) new Uri(AVFile.UP_HOST) +
                            (object) string.Format("mkblk/{0}", (object) blcokSize)), "POST",
                    this.GetQiniuRequestHeaders(), (Stream) memoryStream, "application/octet-stream",
                    CancellationToken.None);
        }

        private Task<Tuple<HttpStatusCode, string>> PutChunk(byte[] chunkBinary, string LastChunkctx,
            int currentChunkOffsetInBlock)
        {
            MemoryStream memoryStream = new MemoryStream(chunkBinary, 0, chunkBinary.Length);
            return
                AVClient.RequestAsync(
                    new Uri((string) (object) new Uri(AVFile.UP_HOST) +
                            (object)
                                string.Format("bput/{0}/{1}", (object) LastChunkctx, (object) currentChunkOffsetInBlock)),
                    "POST", this.GetQiniuRequestHeaders(), (Stream) memoryStream, "application/octet-stream",
                    CancellationToken.None);
        }

        internal Task<Tuple<HttpStatusCode, string>> QiniuMakeFile(string upToken, string key, long fsize,
            string[] ctxes, CancellationToken cancellationToken)
        {
            StringBuilder stringBuilder1 = new StringBuilder();
            stringBuilder1.AppendFormat("{0}/mkfile/{1}", new object[2]
            {
                (object) AVFile.UP_HOST,
                (object) fsize
            });
            if (key != null)
                stringBuilder1.AppendFormat("/key/{0}", new object[1]
                {
                    (object) AVFile.ToBase64URLSafe(key)
                });
            IDictionary<string, object> metaData = this.GetMetaData();
            StringBuilder stringBuilder2 = new StringBuilder();
            foreach (string index in (IEnumerable<string>) metaData.Keys)
                stringBuilder2.AppendFormat("/{0}/{1}", new object[2]
                {
                    (object) index,
                    (object) AVFile.ToBase64URLSafe(metaData[index].ToString())
                });
            stringBuilder1.Append(((object) stringBuilder2).ToString());
            IList<KeyValuePair<string, string>> headers =
                (IList<KeyValuePair<string, string>>) new List<KeyValuePair<string, string>>();
            string str = "UpToken " + upToken;
            headers.Add(new KeyValuePair<string, string>("Authorization", str));
            int length = ctxes.Length;
            Stream data = (Stream) new MemoryStream();
            for (int index = 0; index < length; ++index)
            {
                byte[] buffer = AVFile.StringToAscii(ctxes[index]);
                data.Write(buffer, 0, buffer.Length);
                if (index != length - 1)
                    data.WriteByte((byte) 44);
            }
            data.Seek(0L, SeekOrigin.Begin);
            return AVClient.RequestAsync(new Uri(((object) stringBuilder1).ToString()), "POST", headers, data,
                "text/plain", cancellationToken);
        }

        public static byte[] StringToAscii(string s)
        {
            byte[] numArray = new byte[s.Length];
            for (int index = 0; index < s.Length; ++index)
            {
                char ch = s[index];
                numArray[index] = (int) ch > (int) sbyte.MaxValue ? (byte) 63 : (byte) ch;
            }
            return numArray;
        }

        public static string Encode(string text)
        {
            if (string.IsNullOrEmpty(text))
                return "";
            else
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(text)).Replace('+', '-').Replace('/', '_');
        }

        internal static string GetNameFromUrl(string url)
        {
            string str = string.Empty;
            Uri uri = new Uri(url);
            if (uri.IsFile)
                str = Path.GetFileName(uri.LocalPath);
            return str;
        }

        public static string ToBase64URLSafe(string str)
        {
            return AVFile.Encode(str);
        }

        public static string Encode(byte[] bs)
        {
            if (bs == null || bs.Length == 0)
                return "";
            else
                return Convert.ToBase64String(bs).Replace('+', '-').Replace('/', '_');
        }
    }
}
