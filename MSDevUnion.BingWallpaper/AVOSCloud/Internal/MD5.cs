using System;
using System.IO;
using System.Text;

namespace AVOSCloud.Internal
{
    internal class MD5 : IDisposable
    {
        private uint[] state = new uint[4];
        private uint[] count = new uint[2];
        private byte[] buffer = new byte[64];
        protected int HashSizeValue = 128;
        private const byte S11 = (byte)7;
        private const byte S12 = (byte)12;
        private const byte S13 = (byte)17;
        private const byte S14 = (byte)22;
        private const byte S21 = (byte)5;
        private const byte S22 = (byte)9;
        private const byte S23 = (byte)14;
        private const byte S24 = (byte)20;
        private const byte S31 = (byte)4;
        private const byte S32 = (byte)11;
        private const byte S33 = (byte)16;
        private const byte S34 = (byte)23;
        private const byte S41 = (byte)6;
        private const byte S42 = (byte)10;
        private const byte S43 = (byte)15;
        private const byte S44 = (byte)21;
        private static byte[] PADDING;
        protected byte[] HashValue;
        protected int State;

        public virtual bool CanReuseTransform
        {
            get
            {
                return true;
            }
        }

        public virtual bool CanTransformMultipleBlocks
        {
            get
            {
                return true;
            }
        }

        public virtual byte[] Hash
        {
            get
            {
                if (this.State != 0)
                    throw new InvalidOperationException();
                else
                    return (byte[])this.HashValue.Clone();
            }
        }

        public virtual int HashSize
        {
            get
            {
                return this.HashSizeValue;
            }
        }

        public virtual int InputBlockSize
        {
            get
            {
                return 1;
            }
        }

        public virtual int OutputBlockSize
        {
            get
            {
                return 1;
            }
        }

        static MD5()
        {
            byte[] numArray = new byte[64];
            numArray[0] = 128;
            MD5.PADDING = numArray;
        }

        internal MD5()
        {
            this.Initialize();
        }

        public static MD5 Create(string hashName)
        {
            if (hashName == "MD5")
                return new MD5();
            else
                throw new NotSupportedException();
        }

        public static string GetMd5String(string source)
        {
            byte[] hash = MD5.Create().ComputeHash(new UTF8Encoding().GetBytes(source));
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte num in hash)
                stringBuilder.Append(num.ToString("x2"));
            return ((object)stringBuilder).ToString();
        }

        public static MD5 Create()
        {
            return new MD5();
        }

        private static uint F(uint x, uint y, uint z)
        {
            return (uint)((int)x & (int)y | ~(int)x & (int)z);
        }

        private static uint G(uint x, uint y, uint z)
        {
            return (uint)((int)x & (int)z | (int)y & ~(int)z);
        }

        private static uint H(uint x, uint y, uint z)
        {
            return x ^ y ^ z;
        }

        private static uint I(uint x, uint y, uint z)
        {
            return y ^ (x | ~z);
        }

        private static uint ROTATE_LEFT(uint x, byte n)
        {
            return x << (int)n | x >> 32 - (int)n;
        }

        private static void FF(ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac)
        {
            a += MD5.F(b, c, d) + x + ac;
            a = MD5.ROTATE_LEFT(a, s);
            a += b;
        }

        private static void GG(ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac)
        {
            a += MD5.G(b, c, d) + x + ac;
            a = MD5.ROTATE_LEFT(a, s);
            a += b;
        }

        private static void HH(ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac)
        {
            a += MD5.H(b, c, d) + x + ac;
            a = MD5.ROTATE_LEFT(a, s);
            a += b;
        }

        private static void II(ref uint a, uint b, uint c, uint d, uint x, byte s, uint ac)
        {
            a += MD5.I(b, c, d) + x + ac;
            a = MD5.ROTATE_LEFT(a, s);
            a += b;
        }

        public virtual void Initialize()
        {
            this.count[0] = this.count[1] = 0U;
            this.state[0] = 1732584193U;
            this.state[1] = 4023233417U;
            this.state[2] = 2562383102U;
            this.state[3] = 271733878U;
        }

        protected virtual void HashCore(byte[] input, int offset, int count)
        {
            int dstOffset = (int)(this.count[0] >> 3) & 63;
            if ((this.count[0] += (uint)(count << 3)) < (uint)(count << 3))
                ++this.count[1];
            this.count[1] += (uint)count >> 29;
            int count1 = 64 - dstOffset;
            int num;
            if (count >= count1)
            {
                Buffer.BlockCopy((Array)input, offset, (Array)this.buffer, dstOffset, count1);
                this.Transform(this.buffer, 0);
                num = count1;
                while (num + 63 < count)
                {
                    this.Transform(input, offset + num);
                    num += 64;
                }
                dstOffset = 0;
            }
            else
                num = 0;
            Buffer.BlockCopy((Array)input, offset + num, (Array)this.buffer, dstOffset, count - num);
        }

        protected virtual byte[] HashFinal()
        {
            byte[] output = new byte[16];
            byte[] numArray = new byte[8];
            MD5.Encode(numArray, 0, this.count, 0, 8);
            int num = (int)(this.count[0] >> 3) & 63;
            int count = num < 56 ? 56 - num : 120 - num;
            this.HashCore(MD5.PADDING, 0, count);
            this.HashCore(numArray, 0, 8);
            MD5.Encode(output, 0, this.state, 0, 16);
            this.count[0] = this.count[1] = 0U;
            this.state[0] = 0U;
            this.state[1] = 0U;
            this.state[2] = 0U;
            this.state[3] = 0U;
            this.Initialize();
            return output;
        }

        private void Transform(byte[] block, int offset)
        {
            uint a1 = this.state[0];
            uint a2 = this.state[1];
            uint a3 = this.state[2];
            uint a4 = this.state[3];
            uint[] output = new uint[16];
            MD5.Decode(output, 0, block, offset, 64);
            MD5.FF(ref a1, a2, a3, a4, output[0], (byte)7, 3614090360U);
            MD5.FF(ref a4, a1, a2, a3, output[1], (byte)12, 3905402710U);
            MD5.FF(ref a3, a4, a1, a2, output[2], (byte)17, 606105819U);
            MD5.FF(ref a2, a3, a4, a1, output[3], (byte)22, 3250441966U);
            MD5.FF(ref a1, a2, a3, a4, output[4], (byte)7, 4118548399U);
            MD5.FF(ref a4, a1, a2, a3, output[5], (byte)12, 1200080426U);
            MD5.FF(ref a3, a4, a1, a2, output[6], (byte)17, 2821735955U);
            MD5.FF(ref a2, a3, a4, a1, output[7], (byte)22, 4249261313U);
            MD5.FF(ref a1, a2, a3, a4, output[8], (byte)7, 1770035416U);
            MD5.FF(ref a4, a1, a2, a3, output[9], (byte)12, 2336552879U);
            MD5.FF(ref a3, a4, a1, a2, output[10], (byte)17, 4294925233U);
            MD5.FF(ref a2, a3, a4, a1, output[11], (byte)22, 2304563134U);
            MD5.FF(ref a1, a2, a3, a4, output[12], (byte)7, 1804603682U);
            MD5.FF(ref a4, a1, a2, a3, output[13], (byte)12, 4254626195U);
            MD5.FF(ref a3, a4, a1, a2, output[14], (byte)17, 2792965006U);
            MD5.FF(ref a2, a3, a4, a1, output[15], (byte)22, 1236535329U);
            MD5.GG(ref a1, a2, a3, a4, output[1], (byte)5, 4129170786U);
            MD5.GG(ref a4, a1, a2, a3, output[6], (byte)9, 3225465664U);
            MD5.GG(ref a3, a4, a1, a2, output[11], (byte)14, 643717713U);
            MD5.GG(ref a2, a3, a4, a1, output[0], (byte)20, 3921069994U);
            MD5.GG(ref a1, a2, a3, a4, output[5], (byte)5, 3593408605U);
            MD5.GG(ref a4, a1, a2, a3, output[10], (byte)9, 38016083U);
            MD5.GG(ref a3, a4, a1, a2, output[15], (byte)14, 3634488961U);
            MD5.GG(ref a2, a3, a4, a1, output[4], (byte)20, 3889429448U);
            MD5.GG(ref a1, a2, a3, a4, output[9], (byte)5, 568446438U);
            MD5.GG(ref a4, a1, a2, a3, output[14], (byte)9, 3275163606U);
            MD5.GG(ref a3, a4, a1, a2, output[3], (byte)14, 4107603335U);
            MD5.GG(ref a2, a3, a4, a1, output[8], (byte)20, 1163531501U);
            MD5.GG(ref a1, a2, a3, a4, output[13], (byte)5, 2850285829U);
            MD5.GG(ref a4, a1, a2, a3, output[2], (byte)9, 4243563512U);
            MD5.GG(ref a3, a4, a1, a2, output[7], (byte)14, 1735328473U);
            MD5.GG(ref a2, a3, a4, a1, output[12], (byte)20, 2368359562U);
            MD5.HH(ref a1, a2, a3, a4, output[5], (byte)4, 4294588738U);
            MD5.HH(ref a4, a1, a2, a3, output[8], (byte)11, 2272392833U);
            MD5.HH(ref a3, a4, a1, a2, output[11], (byte)16, 1839030562U);
            MD5.HH(ref a2, a3, a4, a1, output[14], (byte)23, 4259657740U);
            MD5.HH(ref a1, a2, a3, a4, output[1], (byte)4, 2763975236U);
            MD5.HH(ref a4, a1, a2, a3, output[4], (byte)11, 1272893353U);
            MD5.HH(ref a3, a4, a1, a2, output[7], (byte)16, 4139469664U);
            MD5.HH(ref a2, a3, a4, a1, output[10], (byte)23, 3200236656U);
            MD5.HH(ref a1, a2, a3, a4, output[13], (byte)4, 681279174U);
            MD5.HH(ref a4, a1, a2, a3, output[0], (byte)11, 3936430074U);
            MD5.HH(ref a3, a4, a1, a2, output[3], (byte)16, 3572445317U);
            MD5.HH(ref a2, a3, a4, a1, output[6], (byte)23, 76029189U);
            MD5.HH(ref a1, a2, a3, a4, output[9], (byte)4, 3654602809U);
            MD5.HH(ref a4, a1, a2, a3, output[12], (byte)11, 3873151461U);
            MD5.HH(ref a3, a4, a1, a2, output[15], (byte)16, 530742520U);
            MD5.HH(ref a2, a3, a4, a1, output[2], (byte)23, 3299628645U);
            MD5.II(ref a1, a2, a3, a4, output[0], (byte)6, 4096336452U);
            MD5.II(ref a4, a1, a2, a3, output[7], (byte)10, 1126891415U);
            MD5.II(ref a3, a4, a1, a2, output[14], (byte)15, 2878612391U);
            MD5.II(ref a2, a3, a4, a1, output[5], (byte)21, 4237533241U);
            MD5.II(ref a1, a2, a3, a4, output[12], (byte)6, 1700485571U);
            MD5.II(ref a4, a1, a2, a3, output[3], (byte)10, 2399980690U);
            MD5.II(ref a3, a4, a1, a2, output[10], (byte)15, 4293915773U);
            MD5.II(ref a2, a3, a4, a1, output[1], (byte)21, 2240044497U);
            MD5.II(ref a1, a2, a3, a4, output[8], (byte)6, 1873313359U);
            MD5.II(ref a4, a1, a2, a3, output[15], (byte)10, 4264355552U);
            MD5.II(ref a3, a4, a1, a2, output[6], (byte)15, 2734768916U);
            MD5.II(ref a2, a3, a4, a1, output[13], (byte)21, 1309151649U);
            MD5.II(ref a1, a2, a3, a4, output[4], (byte)6, 4149444226U);
            MD5.II(ref a4, a1, a2, a3, output[11], (byte)10, 3174756917U);
            MD5.II(ref a3, a4, a1, a2, output[2], (byte)15, 718787259U);
            MD5.II(ref a2, a3, a4, a1, output[9], (byte)21, 3951481745U);
            this.state[0] += a1;
            this.state[1] += a2;
            this.state[2] += a3;
            this.state[3] += a4;
            for (int index = 0; index < output.Length; ++index)
                output[index] = 0U;
        }

        private static void Encode(byte[] output, int outputOffset, uint[] input, int inputOffset, int count)
        {
            int num = outputOffset + count;
            int index1 = inputOffset;
            int index2 = outputOffset;
            while (index2 < num)
            {
                output[index2] = (byte)(input[index1] & (uint)byte.MaxValue);
                output[index2 + 1] = (byte)(input[index1] >> 8 & (uint)byte.MaxValue);
                output[index2 + 2] = (byte)(input[index1] >> 16 & (uint)byte.MaxValue);
                output[index2 + 3] = (byte)(input[index1] >> 24 & (uint)byte.MaxValue);
                ++index1;
                index2 += 4;
            }
        }

        private static void Decode(uint[] output, int outputOffset, byte[] input, int inputOffset, int count)
        {
            int num = inputOffset + count;
            int index1 = outputOffset;
            int index2 = inputOffset;
            while (index2 < num)
            {
                output[index1] = (uint)((int)input[index2] | (int)input[index2 + 1] << 8 | (int)input[index2 + 2] << 16 | (int)input[index2 + 3] << 24);
                ++index1;
                index2 += 4;
            }
        }

        public void Clear()
        {
            this.Dispose(true);
        }

        public byte[] ComputeHash(byte[] buffer)
        {
            return this.ComputeHash(buffer, 0, buffer.Length);
        }

        public byte[] ComputeHash(byte[] buffer, int offset, int count)
        {
            this.Initialize();
            this.HashCore(buffer, offset, count);
            this.HashValue = this.HashFinal();
            return (byte[])this.HashValue.Clone();
        }

        public byte[] ComputeHash(Stream inputStream)
        {
            this.Initialize();
            byte[] numArray = new byte[4096];
            int count;
            while (0 < (count = inputStream.Read(numArray, 0, 4096)))
                this.HashCore(numArray, 0, count);
            this.HashValue = this.HashFinal();
            return (byte[])this.HashValue.Clone();
        }

        public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset)
        {
            if (inputBuffer == null)
                throw new ArgumentNullException("inputBuffer");
            if (inputOffset < 0)
                throw new ArgumentOutOfRangeException("inputOffset");
            if (inputCount < 0 || inputCount > inputBuffer.Length)
                throw new ArgumentException("inputCount");
            if (inputBuffer.Length - inputCount < inputOffset)
                throw new ArgumentOutOfRangeException("inputOffset");
            if (this.State == 0)
            {
                this.Initialize();
                this.State = 1;
            }
            this.HashCore(inputBuffer, inputOffset, inputCount);
            if (inputBuffer != outputBuffer || inputOffset != outputOffset)
                Buffer.BlockCopy((Array)inputBuffer, inputOffset, (Array)outputBuffer, outputOffset, inputCount);
            return inputCount;
        }

        public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount)
        {
            if (inputBuffer == null)
                throw new ArgumentNullException("inputBuffer");
            if (inputOffset < 0)
                throw new ArgumentOutOfRangeException("inputOffset");
            if (inputCount < 0 || inputCount > inputBuffer.Length)
                throw new ArgumentException("inputCount");
            if (inputBuffer.Length - inputCount < inputOffset)
                throw new ArgumentOutOfRangeException("inputOffset");
            if (this.State == 0)
                this.Initialize();
            this.HashCore(inputBuffer, inputOffset, inputCount);
            this.HashValue = this.HashFinal();
            byte[] numArray = new byte[inputCount];
            Buffer.BlockCopy((Array)inputBuffer, inputOffset, (Array)numArray, 0, inputCount);
            this.State = 0;
            return numArray;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                return;
            this.Initialize();
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}