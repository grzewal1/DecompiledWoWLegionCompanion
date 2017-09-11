using System;
using System.IO;

namespace bnet.protocol.authentication
{
	public class VersionInfoNotification : IProtoBuf
	{
		public bool HasVersionInfo;

		private bnet.protocol.authentication.VersionInfo _VersionInfo;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bnet.protocol.authentication.VersionInfo VersionInfo
		{
			get
			{
				return this._VersionInfo;
			}
			set
			{
				this._VersionInfo = value;
				this.HasVersionInfo = value != null;
			}
		}

		public VersionInfoNotification()
		{
		}

		public void Deserialize(Stream stream)
		{
			VersionInfoNotification.Deserialize(stream, this);
		}

		public static VersionInfoNotification Deserialize(Stream stream, VersionInfoNotification instance)
		{
			return VersionInfoNotification.Deserialize(stream, instance, (long)-1);
		}

		public static VersionInfoNotification Deserialize(Stream stream, VersionInfoNotification instance, long limit)
		{
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
					else if (num != 10)
					{
						Key key = ProtocolParser.ReadKey((byte)num, stream);
						if (key.Field == 0)
						{
							throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
						}
						ProtocolParser.SkipKey(stream, key);
					}
					else if (instance.VersionInfo != null)
					{
						bnet.protocol.authentication.VersionInfo.DeserializeLengthDelimited(stream, instance.VersionInfo);
					}
					else
					{
						instance.VersionInfo = bnet.protocol.authentication.VersionInfo.DeserializeLengthDelimited(stream);
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

		public static VersionInfoNotification DeserializeLengthDelimited(Stream stream)
		{
			VersionInfoNotification versionInfoNotification = new VersionInfoNotification();
			VersionInfoNotification.DeserializeLengthDelimited(stream, versionInfoNotification);
			return versionInfoNotification;
		}

		public static VersionInfoNotification DeserializeLengthDelimited(Stream stream, VersionInfoNotification instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return VersionInfoNotification.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			VersionInfoNotification versionInfoNotification = obj as VersionInfoNotification;
			if (versionInfoNotification == null)
			{
				return false;
			}
			if (this.HasVersionInfo == versionInfoNotification.HasVersionInfo && (!this.HasVersionInfo || this.VersionInfo.Equals(versionInfoNotification.VersionInfo)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasVersionInfo)
			{
				hashCode ^= this.VersionInfo.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasVersionInfo)
			{
				num++;
				uint serializedSize = this.VersionInfo.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			return num;
		}

		public static VersionInfoNotification ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<VersionInfoNotification>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			VersionInfoNotification.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, VersionInfoNotification instance)
		{
			if (instance.HasVersionInfo)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.VersionInfo.GetSerializedSize());
				bnet.protocol.authentication.VersionInfo.Serialize(stream, instance.VersionInfo);
			}
		}

		public void SetVersionInfo(bnet.protocol.authentication.VersionInfo val)
		{
			this.VersionInfo = val;
		}
	}
}