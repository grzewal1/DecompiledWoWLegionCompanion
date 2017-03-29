using System;
using System.IO;
using System.Runtime.CompilerServices;

public class PositionStream : Stream
{
	private Stream stream;

	public int BytesRead
	{
		get;
		private set;
	}

	public override bool CanRead
	{
		get
		{
			return true;
		}
	}

	public override bool CanSeek
	{
		get
		{
			return false;
		}
	}

	public override bool CanWrite
	{
		get
		{
			return false;
		}
	}

	public override long Length
	{
		get
		{
			return this.stream.Length;
		}
	}

	public override long Position
	{
		get
		{
			return (long)this.BytesRead;
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public PositionStream(Stream baseStream)
	{
		this.stream = baseStream;
	}

	public override void Close()
	{
		base.Close();
	}

	protected override void Dispose(bool disposing)
	{
		this.stream.Dispose();
		base.Dispose(disposing);
	}

	public override void Flush()
	{
		throw new NotImplementedException();
	}

	public override int Read(byte[] buffer, int offset, int count)
	{
		int num = this.stream.Read(buffer, offset, count);
		PositionStream bytesRead = this;
		bytesRead.BytesRead = bytesRead.BytesRead + num;
		return num;
	}

	public override int ReadByte()
	{
		int num = this.stream.ReadByte();
		PositionStream bytesRead = this;
		bytesRead.BytesRead = bytesRead.BytesRead + 1;
		return num;
	}

	public override long Seek(long offset, SeekOrigin origin)
	{
		throw new NotImplementedException();
	}

	public override void SetLength(long value)
	{
		throw new NotImplementedException();
	}

	public override void Write(byte[] buffer, int offset, int count)
	{
		throw new NotImplementedException();
	}
}