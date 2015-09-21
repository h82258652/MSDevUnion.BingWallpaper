using AVOSCloud;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace AVOSCloud.Internal
{
    internal class QiniuBlock
    {
        private LinkedList<QiniuChunk> _chunks;
        private IList<QiniuChunk> unprocessChunks;

        internal long BlockSize { get; set; }

        internal string Host { get; set; }

        internal byte[] BlockData { get; set; }

        internal int BlockIndex { get; set; }

        internal int SentChunk { get; set; }

        internal int ChunkSize { get; set; }

        internal LinkedList<QiniuChunk> Chunks
        {
            get
            {
                if (this._chunks == null)
                    this._chunks = this.EnqueueChunks();
                return this._chunks;
            }
        }

        internal IList<QiniuChunk> UnprocessChunks
        {
            get
            {
                if (this.unprocessChunks == null)
                {
                    QiniuChunk[] array = new QiniuChunk[this._chunks.Count];
                    this._chunks.CopyTo(array, 0);
                    this.unprocessChunks = (IList<QiniuChunk>)new List<QiniuChunk>((IEnumerable<QiniuChunk>)array);
                    this.unprocessChunks.RemoveAt(0);
                }
                return this.unprocessChunks;
            }
        }

        internal QiniuBlock()
        {
            this.Host = AVFile.UP_HOST;
        }

        public QiniuBlock(int blockIndexInFile, byte[] blockData, int chunkSize)
          : this()
        {
            int length = blockData.Length;
            this.BlockData = new byte[length];
            blockData.CopyTo((Array)this.BlockData, 0);
            this.BlockSize = (long)length;
            this.BlockIndex = blockIndexInFile;
            this.ChunkSize = chunkSize;
        }

        internal void Append(QiniuChunk qc)
        {
            if (this.Chunks.Count == 0)
                this.Chunks.AddFirst(qc);
            else
                this.Chunks.AddAfter(this.Chunks.Last, qc);
        }

        internal LinkedList<QiniuChunk> EnqueueChunks()
        {
            LinkedList<QiniuChunk> ll = new LinkedList<QiniuChunk>();
            IList<byte[]> list = InternalExtensions.Cut<byte>(this.BlockData, this.ChunkSize);
            int count = list.Count;
            for (int chunkIndexInBlock = 0; chunkIndexInBlock < count; ++chunkIndexInBlock)
            {
                QiniuChunk qiniuChunk = new QiniuChunk(chunkIndexInBlock, list[chunkIndexInBlock]);
                InternalExtensions.Append<QiniuChunk>(ll, qiniuChunk);
            }
            return ll;
        }
    }
}