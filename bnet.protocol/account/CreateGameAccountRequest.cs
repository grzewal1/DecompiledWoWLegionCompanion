using System;
using System.IO;

namespace bnet.protocol.account
{
	public class CreateGameAccountRequest : IProtoBuf
	{
		public bool HasAccount;

		private AccountId _Account;

		public bool HasRegion;

		private uint _Region;

		public bool HasProgram;

		private uint _Program;

		public bool HasRealmPermissions;

		private uint _RealmPermissions;

		public AccountId Account
		{
			get
			{
				return this._Account;
			}
			set
			{
				this._Account = value;
				this.HasAccount = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint Program
		{
			get
			{
				return this._Program;
			}
			set
			{
				this._Program = value;
				this.HasProgram = true;
			}
		}

		public uint RealmPermissions
		{
			get
			{
				return this._RealmPermissions;
			}
			set
			{
				this._RealmPermissions = value;
				this.HasRealmPermissions = true;
			}
		}

		public uint Region
		{
			get
			{
				return this._Region;
			}
			set
			{
				this._Region = value;
				this.HasRegion = true;
			}
		}

		public CreateGameAccountRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			CreateGameAccountRequest.Deserialize(stream, this);
		}

		public static CreateGameAccountRequest Deserialize(Stream stream, CreateGameAccountRequest instance)
		{
			return CreateGameAccountRequest.Deserialize(stream, instance, (long)-1);
		}

		public static CreateGameAccountRequest Deserialize(Stream stream, CreateGameAccountRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.RealmPermissions = 1;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						switch (num1)
						{
							case 29:
							{
								instance.Program = binaryReader.ReadUInt32();
								continue;
							}
							case 32:
							{
								instance.RealmPermissions = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							default:
							{
								if (num1 == 10)
								{
									if (instance.Account != null)
									{
										AccountId.DeserializeLengthDelimited(stream, instance.Account);
									}
									else
									{
										instance.Account = AccountId.DeserializeLengthDelimited(stream);
									}
									continue;
								}
								else if (num1 == 16)
								{
									instance.Region = ProtocolParser.ReadUInt32(stream);
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

		public static CreateGameAccountRequest DeserializeLengthDelimited(Stream stream)
		{
			CreateGameAccountRequest createGameAccountRequest = new CreateGameAccountRequest();
			CreateGameAccountRequest.DeserializeLengthDelimited(stream, createGameAccountRequest);
			return createGameAccountRequest;
		}

		public static CreateGameAccountRequest DeserializeLengthDelimited(Stream stream, CreateGameAccountRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return CreateGameAccountRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			CreateGameAccountRequest createGameAccountRequest = obj as CreateGameAccountRequest;
			if (createGameAccountRequest == null)
			{
				return false;
			}
			if (this.HasAccount != createGameAccountRequest.HasAccount || this.HasAccount && !this.Account.Equals(createGameAccountRequest.Account))
			{
				return false;
			}
			if (this.HasRegion != createGameAccountRequest.HasRegion || this.HasRegion && !this.Region.Equals(createGameAccountRequest.Region))
			{
				return false;
			}
			if (this.HasProgram != createGameAccountRequest.HasProgram || this.HasProgram && !this.Program.Equals(createGameAccountRequest.Program))
			{
				return false;
			}
			if (this.HasRealmPermissions == createGameAccountRequest.HasRealmPermissions && (!this.HasRealmPermissions || this.RealmPermissions.Equals(createGameAccountRequest.RealmPermissions)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAccount)
			{
				hashCode = hashCode ^ this.Account.GetHashCode();
			}
			if (this.HasRegion)
			{
				hashCode = hashCode ^ this.Region.GetHashCode();
			}
			if (this.HasProgram)
			{
				hashCode = hashCode ^ this.Program.GetHashCode();
			}
			if (this.HasRealmPermissions)
			{
				hashCode = hashCode ^ this.RealmPermissions.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAccount)
			{
				num++;
				uint serializedSize = this.Account.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasRegion)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.Region);
			}
			if (this.HasProgram)
			{
				num++;
				num = num + 4;
			}
			if (this.HasRealmPermissions)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.RealmPermissions);
			}
			return num;
		}

		public static CreateGameAccountRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<CreateGameAccountRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			CreateGameAccountRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, CreateGameAccountRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasAccount)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.Account.GetSerializedSize());
				AccountId.Serialize(stream, instance.Account);
			}
			if (instance.HasRegion)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.Region);
			}
			if (instance.HasProgram)
			{
				stream.WriteByte(29);
				binaryWriter.Write(instance.Program);
			}
			if (instance.HasRealmPermissions)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt32(stream, instance.RealmPermissions);
			}
		}

		public void SetAccount(AccountId val)
		{
			this.Account = val;
		}

		public void SetProgram(uint val)
		{
			this.Program = val;
		}

		public void SetRealmPermissions(uint val)
		{
			this.RealmPermissions = val;
		}

		public void SetRegion(uint val)
		{
			this.Region = val;
		}
	}
}