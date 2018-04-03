using bnet.protocol;
using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GetLicensesRequest : IProtoBuf
	{
		public bool HasTargetId;

		private EntityId _TargetId;

		public bool HasGetAccountLicenses;

		private bool _GetAccountLicenses;

		public bool HasGetGameAccountLicenses;

		private bool _GetGameAccountLicenses;

		public bool HasGetDynamicAccountLicenses;

		private bool _GetDynamicAccountLicenses;

		public bool HasProgramId;

		private uint _ProgramId;

		public bool HasExcludeUnknownProgram;

		private bool _ExcludeUnknownProgram;

		public bool ExcludeUnknownProgram
		{
			get
			{
				return this._ExcludeUnknownProgram;
			}
			set
			{
				this._ExcludeUnknownProgram = value;
				this.HasExcludeUnknownProgram = true;
			}
		}

		public bool GetAccountLicenses
		{
			get
			{
				return this._GetAccountLicenses;
			}
			set
			{
				this._GetAccountLicenses = value;
				this.HasGetAccountLicenses = true;
			}
		}

		public bool GetDynamicAccountLicenses
		{
			get
			{
				return this._GetDynamicAccountLicenses;
			}
			set
			{
				this._GetDynamicAccountLicenses = value;
				this.HasGetDynamicAccountLicenses = true;
			}
		}

		public bool GetGameAccountLicenses
		{
			get
			{
				return this._GetGameAccountLicenses;
			}
			set
			{
				this._GetGameAccountLicenses = value;
				this.HasGetGameAccountLicenses = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint ProgramId
		{
			get
			{
				return this._ProgramId;
			}
			set
			{
				this._ProgramId = value;
				this.HasProgramId = true;
			}
		}

		public EntityId TargetId
		{
			get
			{
				return this._TargetId;
			}
			set
			{
				this._TargetId = value;
				this.HasTargetId = value != null;
			}
		}

		public GetLicensesRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetLicensesRequest.Deserialize(stream, this);
		}

		public static GetLicensesRequest Deserialize(Stream stream, GetLicensesRequest instance)
		{
			return GetLicensesRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetLicensesRequest Deserialize(Stream stream, GetLicensesRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.ExcludeUnknownProgram = false;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						switch (num)
						{
							case 45:
							{
								instance.ProgramId = binaryReader.ReadUInt32();
								continue;
							}
							case 48:
							{
								instance.ExcludeUnknownProgram = ProtocolParser.ReadBool(stream);
								continue;
							}
							default:
							{
								if (num == 10)
								{
									if (instance.TargetId != null)
									{
										EntityId.DeserializeLengthDelimited(stream, instance.TargetId);
									}
									else
									{
										instance.TargetId = EntityId.DeserializeLengthDelimited(stream);
									}
									continue;
								}
								else if (num == 16)
								{
									instance.GetAccountLicenses = ProtocolParser.ReadBool(stream);
									continue;
								}
								else if (num == 24)
								{
									instance.GetGameAccountLicenses = ProtocolParser.ReadBool(stream);
									continue;
								}
								else if (num == 32)
								{
									instance.GetDynamicAccountLicenses = ProtocolParser.ReadBool(stream);
									continue;
								}
								else
								{
									Key key = ProtocolParser.ReadKey((byte)num, stream);
									if (key.Field == 0)
									{
										throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
									}
									ProtocolParser.SkipKey(stream, key);
									continue;
								}
							}
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

		public static GetLicensesRequest DeserializeLengthDelimited(Stream stream)
		{
			GetLicensesRequest getLicensesRequest = new GetLicensesRequest();
			GetLicensesRequest.DeserializeLengthDelimited(stream, getLicensesRequest);
			return getLicensesRequest;
		}

		public static GetLicensesRequest DeserializeLengthDelimited(Stream stream, GetLicensesRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetLicensesRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetLicensesRequest getLicensesRequest = obj as GetLicensesRequest;
			if (getLicensesRequest == null)
			{
				return false;
			}
			if (this.HasTargetId != getLicensesRequest.HasTargetId || this.HasTargetId && !this.TargetId.Equals(getLicensesRequest.TargetId))
			{
				return false;
			}
			if (this.HasGetAccountLicenses != getLicensesRequest.HasGetAccountLicenses || this.HasGetAccountLicenses && !this.GetAccountLicenses.Equals(getLicensesRequest.GetAccountLicenses))
			{
				return false;
			}
			if (this.HasGetGameAccountLicenses != getLicensesRequest.HasGetGameAccountLicenses || this.HasGetGameAccountLicenses && !this.GetGameAccountLicenses.Equals(getLicensesRequest.GetGameAccountLicenses))
			{
				return false;
			}
			if (this.HasGetDynamicAccountLicenses != getLicensesRequest.HasGetDynamicAccountLicenses || this.HasGetDynamicAccountLicenses && !this.GetDynamicAccountLicenses.Equals(getLicensesRequest.GetDynamicAccountLicenses))
			{
				return false;
			}
			if (this.HasProgramId != getLicensesRequest.HasProgramId || this.HasProgramId && !this.ProgramId.Equals(getLicensesRequest.ProgramId))
			{
				return false;
			}
			if (this.HasExcludeUnknownProgram == getLicensesRequest.HasExcludeUnknownProgram && (!this.HasExcludeUnknownProgram || this.ExcludeUnknownProgram.Equals(getLicensesRequest.ExcludeUnknownProgram)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasTargetId)
			{
				hashCode ^= this.TargetId.GetHashCode();
			}
			if (this.HasGetAccountLicenses)
			{
				hashCode ^= this.GetAccountLicenses.GetHashCode();
			}
			if (this.HasGetGameAccountLicenses)
			{
				hashCode ^= this.GetGameAccountLicenses.GetHashCode();
			}
			if (this.HasGetDynamicAccountLicenses)
			{
				hashCode ^= this.GetDynamicAccountLicenses.GetHashCode();
			}
			if (this.HasProgramId)
			{
				hashCode ^= this.ProgramId.GetHashCode();
			}
			if (this.HasExcludeUnknownProgram)
			{
				hashCode ^= this.ExcludeUnknownProgram.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasTargetId)
			{
				num++;
				uint serializedSize = this.TargetId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasGetAccountLicenses)
			{
				num++;
				num++;
			}
			if (this.HasGetGameAccountLicenses)
			{
				num++;
				num++;
			}
			if (this.HasGetDynamicAccountLicenses)
			{
				num++;
				num++;
			}
			if (this.HasProgramId)
			{
				num++;
				num += 4;
			}
			if (this.HasExcludeUnknownProgram)
			{
				num++;
				num++;
			}
			return num;
		}

		public static GetLicensesRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetLicensesRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetLicensesRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetLicensesRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasTargetId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.TargetId.GetSerializedSize());
				EntityId.Serialize(stream, instance.TargetId);
			}
			if (instance.HasGetAccountLicenses)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteBool(stream, instance.GetAccountLicenses);
			}
			if (instance.HasGetGameAccountLicenses)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.GetGameAccountLicenses);
			}
			if (instance.HasGetDynamicAccountLicenses)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.GetDynamicAccountLicenses);
			}
			if (instance.HasProgramId)
			{
				stream.WriteByte(45);
				binaryWriter.Write(instance.ProgramId);
			}
			if (instance.HasExcludeUnknownProgram)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteBool(stream, instance.ExcludeUnknownProgram);
			}
		}

		public void SetExcludeUnknownProgram(bool val)
		{
			this.ExcludeUnknownProgram = val;
		}

		public void SetGetAccountLicenses(bool val)
		{
			this.GetAccountLicenses = val;
		}

		public void SetGetDynamicAccountLicenses(bool val)
		{
			this.GetDynamicAccountLicenses = val;
		}

		public void SetGetGameAccountLicenses(bool val)
		{
			this.GetGameAccountLicenses = val;
		}

		public void SetProgramId(uint val)
		{
			this.ProgramId = val;
		}

		public void SetTargetId(EntityId val)
		{
			this.TargetId = val;
		}
	}
}