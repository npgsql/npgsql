using System;
using System.IO;
using System.Text;

namespace Npgsql
{
	internal class StringBuilderStream : Stream
	{
		private readonly char[] CharBuf = new char[4096];
		private readonly StringBuilder Buffer;
		private readonly int BufLength;
		private int BufPosition;

		public StringBuilderStream(StringBuilder buffer)
		{
			this.Buffer = buffer;
			BufLength = buffer.Length;
		}

		public override bool CanRead { get { return BufPosition < BufLength; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanWrite { get { return false; } }
		public override void Flush() { }
		public override int Read(byte[] buffer, int offset, int count)
		{
			var size = Math.Min(buffer.Length / 2, Math.Min(2048, Math.Min(count, BufLength - BufPosition)));
			Buffer.CopyTo(BufPosition, CharBuf, 0, size);
			int len;
			while ((len = Encoding.UTF8.GetByteCount(CharBuf, 0, size)) > count)
			{
				size = size / 2;
				if (size < 2)
				{
					size = 1;
					break;
				}
			}
			BufPosition += size;
			return Encoding.UTF8.GetBytes(CharBuf, 0, size, buffer, offset);
		}

		public override long Seek(long offset, SeekOrigin origin) { return 0; }
		public override void SetLength(long value) { }
		public override void Write(byte[] buffer, int offset, int count) { }

		public override long Length { get { return BufLength; } }
		public override long Position
		{
			get { return BufPosition; }
			set { BufPosition = (int)value; }
		}
	}
}
