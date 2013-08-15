using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Npgsql
{
	internal class ChunkedMemoryStream : Stream
	{
		private const int BlockSize = 65536;
		private List<byte[]> Blocks = new List<byte[]>();
		private int CurrentPosition;
		private int BlockRemaining;
		private int TotalSize;

		public ChunkedMemoryStream()
		{
			Position = 0;
		}

		public ChunkedMemoryStream(Stream another)
		{
			Position = 0;
			var buf = new byte[BlockSize];
			int read;
			while ((read = another.Read(buf, 0, BlockSize)) > 0)
			{
				Write(buf, 0, read);
			}
			Position = 0;
		}

		public ChunkedMemoryStream(Stream another, int size)
		{
			Position = 0;
			var buf = new byte[BlockSize];
			int read;
			while (size > 0)
			{
				read = another.Read(buf, 0, BlockSize < size ? BlockSize : size);
				Write(buf, 0, read);
				size -= read;
			}
			Position = 0;
		}

		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return false; } }
		public override bool CanWrite { get { return true; } }
		public override void Flush() { }
		public override long Length { get { return TotalSize; } }

		public override long Position
		{
			get { return CurrentPosition; }
			set { CurrentPosition = (int)value; }
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			var off = CurrentPosition % BlockSize;
			var pos = CurrentPosition / BlockSize;
			var min = Math.Min(BlockSize - off, Math.Min(TotalSize - CurrentPosition, count));
			Buffer.BlockCopy(Blocks[pos], off, buffer, offset, min);
			CurrentPosition += min;
			return min;
		}

		public override long Seek(long offset, SeekOrigin origin) { return 0; }
		public override void SetLength(long value) { }

		public ChunkedMemoryStream Write(char value)
		{
			if (value < 256)
				WriteByte((byte)value);
			else
			{
				WriteByte((byte)(value >> 4));
				WriteByte((byte)(value & 0xff));
			}
			return this;
		}

		public ChunkedMemoryStream Write(string value)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			Write(bytes, 0, bytes.Length);
			return this;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			int cur = count;
			while (cur > 0)
			{
				if (BlockRemaining == 0)
				{
					Blocks.Add(new byte[BlockSize]);
					BlockRemaining = BlockSize;
				}
				var min = cur < BlockRemaining ? cur : BlockRemaining;
				Buffer.BlockCopy(buffer, offset + count - cur, Blocks[Blocks.Count - 1], BlockSize - BlockRemaining, min);
				cur -= min;
				BlockRemaining -= min;
				CurrentPosition += min;
				TotalSize += min;
			}
		}
	}
}
