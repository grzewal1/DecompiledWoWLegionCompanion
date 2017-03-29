using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.connection
{
	public class DisconnectNotification : IProtoBuf
	{
		public bool HasReason;

		private string _Reason;

		public uint ErrorCode
		{
			get;
			set;
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public string Reason
		{
			get
			{
				return this._Reason;
			}
			set
			{
				this._Reason = value;
				this.HasReason = value != null;
			}
		}

		public DisconnectNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			DisconnectNotification.Deserialize(stream, this);
		}

		public static DisconnectNotification Deserialize(Stream stream, DisconnectNotification instance)
		{
			return DisconnectNotification.Deserialize(stream, instance, (long)-1);
		}

		public static DisconnectNotification Deserialize(Stream stream, DisconnectNotification instance, long limit)
		{
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 8)
						{
							instance.ErrorCode = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 18)
						{
							instance.Reason = ProtocolParser.ReadString(stream);
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
						if (limit >= (long)0)
						{
							throw new EndOfStreamException();
						}
						break;
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

		public static DisconnectNotification DeserializeLengthDelimited(Stream stream)
		{
			DisconnectNotification disconnectNotification = new DisconnectNotification();
			DisconnectNotification.DeserializeLengthDelimited(stream, disconnectNotification);
			return disconnectNotification;
		}

		public static DisconnectNotification DeserializeLengthDelimited(Stream stream, DisconnectNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return DisconnectNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			DisconnectNotification disconnectNotification = obj as DisconnectNotification;
			if (disconnectNotification == null)
			{
				return false;
			}
			if (!this.ErrorCode.Equals(disconnectNotification.ErrorCode))
			{
				return false;
			}
			if (this.HasReason == disconnectNotification.HasReason && (!this.HasReason || this.Reason.Equals(disconnectNotification.Reason)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.ErrorCode.GetHashCode();
			if (this.HasReason)
			{
				hashCode = hashCode ^ this.Reason.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num = num + ProtocolParser.SizeOfUInt32(this.ErrorCode);
			if (this.HasReason)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Reason);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			num++;
			return num;
		}

		public static DisconnectNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<DisconnectNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			DisconnectNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, DisconnectNotification instance)
		{
			stream.WriteByte(8);
			ProtocolParser.WriteUInt32(stream, instance.ErrorCode);
			if (instance.HasReason)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Reason));
			}
		}

		public void SetErrorCode(uint val)
		{
			this.ErrorCode = val;
		}

		public void SetReason(string val)
		{
			this.Reason = val;
		}
	}
}