using System;
using System.IO;
using System.Text;

namespace bnet.protocol.authentication
{
	public class VersionInfo : IProtoBuf
	{
		public bool HasNumber;

		private uint _Number;

		public bool HasPatch;

		private string _Patch;

		public bool HasIsOptional;

		private bool _IsOptional;

		public bool HasKickTime;

		private ulong _KickTime;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool IsOptional
		{
			get
			{
				return this._IsOptional;
			}
			set
			{
				this._IsOptional = value;
				this.HasIsOptional = true;
			}
		}

		public ulong KickTime
		{
			get
			{
				return this._KickTime;
			}
			set
			{
				this._KickTime = value;
				this.HasKickTime = true;
			}
		}

		public uint Number
		{
			get
			{
				return this._Number;
			}
			set
			{
				this._Number = value;
				this.HasNumber = true;
			}
		}

		public string Patch
		{
			get
			{
				return this._Patch;
			}
			set
			{
				this._Patch = value;
				this.HasPatch = value != null;
			}
		}

		public VersionInfo()
		{
		}

		public void Deserialize(Stream stream)
		{
			VersionInfo.Deserialize(stream, this);
		}

		public static VersionInfo Deserialize(Stream stream, VersionInfo instance)
		{
			return VersionInfo.Deserialize(stream, instance, (long)-1);
		}

		public static VersionInfo Deserialize(Stream stream, VersionInfo instance, long limit)
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
							instance.Number = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 18)
						{
							instance.Patch = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 24)
						{
							instance.IsOptional = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 32)
						{
							instance.KickTime = ProtocolParser.ReadUInt64(stream);
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

		public static VersionInfo DeserializeLengthDelimited(Stream stream)
		{
			VersionInfo versionInfo = new VersionInfo();
			VersionInfo.DeserializeLengthDelimited(stream, versionInfo);
			return versionInfo;
		}

		public static VersionInfo DeserializeLengthDelimited(Stream stream, VersionInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return VersionInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			VersionInfo versionInfo = obj as VersionInfo;
			if (versionInfo == null)
			{
				return false;
			}
			if (this.HasNumber != versionInfo.HasNumber || this.HasNumber && !this.Number.Equals(versionInfo.Number))
			{
				return false;
			}
			if (this.HasPatch != versionInfo.HasPatch || this.HasPatch && !this.Patch.Equals(versionInfo.Patch))
			{
				return false;
			}
			if (this.HasIsOptional != versionInfo.HasIsOptional || this.HasIsOptional && !this.IsOptional.Equals(versionInfo.IsOptional))
			{
				return false;
			}
			if (this.HasKickTime == versionInfo.HasKickTime && (!this.HasKickTime || this.KickTime.Equals(versionInfo.KickTime)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasNumber)
			{
				hashCode = hashCode ^ this.Number.GetHashCode();
			}
			if (this.HasPatch)
			{
				hashCode = hashCode ^ this.Patch.GetHashCode();
			}
			if (this.HasIsOptional)
			{
				hashCode = hashCode ^ this.IsOptional.GetHashCode();
			}
			if (this.HasKickTime)
			{
				hashCode = hashCode ^ this.KickTime.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasNumber)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.Number);
			}
			if (this.HasPatch)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Patch);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasIsOptional)
			{
				num++;
				num++;
			}
			if (this.HasKickTime)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.KickTime);
			}
			return num;
		}

		public static VersionInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<VersionInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			VersionInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, VersionInfo instance)
		{
			if (instance.HasNumber)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.Number);
			}
			if (instance.HasPatch)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Patch));
			}
			if (instance.HasIsOptional)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.IsOptional);
			}
			if (instance.HasKickTime)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt64(stream, instance.KickTime);
			}
		}

		public void SetIsOptional(bool val)
		{
			this.IsOptional = val;
		}

		public void SetKickTime(ulong val)
		{
			this.KickTime = val;
		}

		public void SetNumber(uint val)
		{
			this.Number = val;
		}

		public void SetPatch(string val)
		{
			this.Patch = val;
		}
	}
}