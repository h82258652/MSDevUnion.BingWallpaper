using AVOSCloud;
using System;
using System.Runtime.CompilerServices;

namespace AVOSCloud.Internal
{
	internal class QiniuChunk
	{
		internal string Checksum
		{
			get;
			set;
		}

		internal byte[] ChunkData
		{
			get;
			set;
		}

		internal long Crc32
		{
			get;
			set;
		}

		internal string Ctx
		{
			get;
			set;
		}

		internal string Host
		{
			get;
			set;
		}

		internal long IndexInBlock
		{
			get;
			set;
		}

		internal long Offset
		{
			get;
			set;
		}

		internal long Size
		{
			get;
			set;
		}

		internal QiniuChunk()
		{
			this.Host = AVFile.UP_HOST;
		}

		internal QiniuChunk(int chunkIndexInBlock, byte[] chunkData)
		{
			this.ChunkData = new byte[(int)chunkData.Length];
			chunkData.CopyTo(this.ChunkData, 0);
			this.IndexInBlock = (long)chunkIndexInBlock;
		}
	}
}