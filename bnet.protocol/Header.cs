using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol
{
	public class Header : IProtoBuf
	{
		public bool HasMethodId;

		private uint _MethodId;

		public bool HasObjectId;

		private ulong _ObjectId;

		public bool HasSize;

		private uint _Size;

		public bool HasStatus;

		private uint _Status;

		private List<ErrorInfo> _Error = new List<ErrorInfo>();

		public bool HasTimeout;

		private ulong _Timeout;

		public List<ErrorInfo> Error
		{
			get
			{
				return this._Error;
			}
			set
			{
				this._Error = value;
			}
		}

		public int ErrorCount
		{
			get
			{
				return this._Error.Count;
			}
		}

		public List<ErrorInfo> ErrorList
		{
			get
			{
				return this._Error;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint MethodId
		{
			get
			{
				return this._MethodId;
			}
			set
			{
				this._MethodId = value;
				this.HasMethodId = true;
			}
		}

		public ulong ObjectId
		{
			get
			{
				return this._ObjectId;
			}
			set
			{
				this._ObjectId = value;
				this.HasObjectId = true;
			}
		}

		public uint ServiceId
		{
			get;
			set;
		}

		public uint Size
		{
			get
			{
				return this._Size;
			}
			set
			{
				this._Size = value;
				this.HasSize = true;
			}
		}

		public uint Status
		{
			get
			{
				return this._Status;
			}
			set
			{
				this._Status = value;
				this.HasStatus = true;
			}
		}

		public ulong Timeout
		{
			get
			{
				return this._Timeout;
			}
			set
			{
				this._Timeout = value;
				this.HasTimeout = true;
			}
		}

		public uint Token
		{
			get;
			set;
		}

		public Header()
		{
		}

		public void AddError(ErrorInfo val)
		{
			this._Error.Add(val);
		}

		public void ClearError()
		{
			this._Error.Clear();
		}

		public void Deserialize(Stream stream)
		{
			Header.Deserialize(stream, this);
		}

		public static Header Deserialize(Stream stream, Header instance)
		{
			return Header.Deserialize(stream, instance, (long)-1);
		}

		public static Header Deserialize(Stream stream, Header instance, long limit)
		{
			instance.ObjectId = (ulong)0;
			instance.Size = 0;
			instance.Status = 0;
			if (instance.Error == null)
			{
				instance.Error = new List<ErrorInfo>();
			}
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						if (limit >= (long)0)
						{
							throw new EndOfStreamException();
						}
						break;
					}
					else if (num == 8)
					{
						instance.ServiceId = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 16)
					{
						instance.MethodId = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 24)
					{
						instance.Token = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 32)
					{
						instance.ObjectId = ProtocolParser.ReadUInt64(stream);
					}
					else if (num == 40)
					{
						instance.Size = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 48)
					{
						instance.Status = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 58)
					{
						instance.Error.Add(ErrorInfo.DeserializeLengthDelimited(stream));
					}
					else if (num == 64)
					{
						instance.Timeout = ProtocolParser.ReadUInt64(stream);
					}
					else
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
				}
				else
				{
					if (stream.Position != limit)
					{
						throw new ProtocolBufferException("Read past max limit");
					}
					break;
				}
			}
			return instance;
		}

		public static Header DeserializeLengthDelimited(Stream stream)
		{
			Header header = new Header();
			Header.DeserializeLengthDelimited(stream, header);
			return header;
		}

		public static Header DeserializeLengthDelimited(Stream stream, Header instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return Header.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Header header = obj as Header;
			if (header == null)
			{
				return false;
			}
			if (!this.ServiceId.Equals(header.ServiceId))
			{
				return false;
			}
			if (this.HasMethodId != header.HasMethodId || this.HasMethodId && !this.MethodId.Equals(header.MethodId))
			{
				return false;
			}
			if (!this.Token.Equals(header.Token))
			{
				return false;
			}
			if (this.HasObjectId != header.HasObjectId || this.HasObjectId && !this.ObjectId.Equals(header.ObjectId))
			{
				return false;
			}
			if (this.HasSize != header.HasSize || this.HasSize && !this.Size.Equals(header.Size))
			{
				return false;
			}
			if (this.HasStatus != header.HasStatus || this.HasStatus && !this.Status.Equals(header.Status))
			{
				return false;
			}
			if (this.Error.Count != header.Error.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Error.Count; i++)
			{
				if (!this.Error[i].Equals(header.Error[i]))
				{
					return false;
				}
			}
			if (this.HasTimeout == header.HasTimeout && (!this.HasTimeout || this.Timeout.Equals(header.Timeout)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.ServiceId.GetHashCode();
			if (this.HasMethodId)
			{
				hashCode ^= this.MethodId.GetHashCode();
			}
			hashCode ^= this.Token.GetHashCode();
			if (this.HasObjectId)
			{
				hashCode ^= this.ObjectId.GetHashCode();
			}
			if (this.HasSize)
			{
				hashCode ^= this.Size.GetHashCode();
			}
			if (this.HasStatus)
			{
				hashCode ^= this.Status.GetHashCode();
			}
			foreach (ErrorInfo error in this.Error)
			{
				hashCode ^= error.GetHashCode();
			}
			if (this.HasTimeout)
			{
				hashCode ^= this.Timeout.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num += ProtocolParser.SizeOfUInt32(this.ServiceId);
			if (this.HasMethodId)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.MethodId);
			}
			num += ProtocolParser.SizeOfUInt32(this.Token);
			if (this.HasObjectId)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.ObjectId);
			}
			if (this.HasSize)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Size);
			}
			if (this.HasStatus)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Status);
			}
			if (this.Error.Count > 0)
			{
				foreach (ErrorInfo error in this.Error)
				{
					num++;
					uint serializedSize = error.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.HasTimeout)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.Timeout);
			}
			num += 2;
			return num;
		}

		public static Header ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Header>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Header.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Header instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.ServiceId);
			if (instance.HasMethodId)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.MethodId);
			}
			stream.WriteByte(24);
			ProtocolParser.WriteUInt32(stream, instance.Token);
			if (instance.HasObjectId)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt64(stream, instance.ObjectId);
			}
			if (instance.HasSize)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt32(stream, instance.Size);
			}
			if (instance.HasStatus)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt32(stream, instance.Status);
			}
			if (instance.Error.Count > 0)
			{
				foreach (ErrorInfo error in instance.Error)
				{
					stream.WriteByte(58);
					ProtocolParser.WriteUInt32(stream, error.GetSerializedSize());
					ErrorInfo.Serialize(stream, error);
				}
			}
			if (instance.HasTimeout)
			{
				stream.WriteByte(64);
				ProtocolParser.WriteUInt64(stream, instance.Timeout);
			}
		}

		public void SetError(List<ErrorInfo> val)
		{
			this.Error = val;
		}

		public void SetMethodId(uint val)
		{
			this.MethodId = val;
		}

		public void SetObjectId(ulong val)
		{
			this.ObjectId = val;
		}

		public void SetServiceId(uint val)
		{
			this.ServiceId = val;
		}

		public void SetSize(uint val)
		{
			this.Size = val;
		}

		public void SetStatus(uint val)
		{
			this.Status = val;
		}

		public void SetTimeout(ulong val)
		{
			this.Timeout = val;
		}

		public void SetToken(uint val)
		{
			this.Token = val;
		}
	}
}