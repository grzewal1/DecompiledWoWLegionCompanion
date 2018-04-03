using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.account
{
	public class AccountFieldTags : IProtoBuf
	{
		public bool HasAccountLevelInfoTag;

		private uint _AccountLevelInfoTag;

		public bool HasPrivacyInfoTag;

		private uint _PrivacyInfoTag;

		public bool HasParentalControlInfoTag;

		private uint _ParentalControlInfoTag;

		private List<ProgramTag> _GameLevelInfoTags = new List<ProgramTag>();

		private List<ProgramTag> _GameStatusTags = new List<ProgramTag>();

		private List<RegionTag> _GameAccountTags = new List<RegionTag>();

		public uint AccountLevelInfoTag
		{
			get
			{
				return this._AccountLevelInfoTag;
			}
			set
			{
				this._AccountLevelInfoTag = value;
				this.HasAccountLevelInfoTag = true;
			}
		}

		public List<RegionTag> GameAccountTags
		{
			get
			{
				return this._GameAccountTags;
			}
			set
			{
				this._GameAccountTags = value;
			}
		}

		public int GameAccountTagsCount
		{
			get
			{
				return this._GameAccountTags.Count;
			}
		}

		public List<RegionTag> GameAccountTagsList
		{
			get
			{
				return this._GameAccountTags;
			}
		}

		public List<ProgramTag> GameLevelInfoTags
		{
			get
			{
				return this._GameLevelInfoTags;
			}
			set
			{
				this._GameLevelInfoTags = value;
			}
		}

		public int GameLevelInfoTagsCount
		{
			get
			{
				return this._GameLevelInfoTags.Count;
			}
		}

		public List<ProgramTag> GameLevelInfoTagsList
		{
			get
			{
				return this._GameLevelInfoTags;
			}
		}

		public List<ProgramTag> GameStatusTags
		{
			get
			{
				return this._GameStatusTags;
			}
			set
			{
				this._GameStatusTags = value;
			}
		}

		public int GameStatusTagsCount
		{
			get
			{
				return this._GameStatusTags.Count;
			}
		}

		public List<ProgramTag> GameStatusTagsList
		{
			get
			{
				return this._GameStatusTags;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint ParentalControlInfoTag
		{
			get
			{
				return this._ParentalControlInfoTag;
			}
			set
			{
				this._ParentalControlInfoTag = value;
				this.HasParentalControlInfoTag = true;
			}
		}

		public uint PrivacyInfoTag
		{
			get
			{
				return this._PrivacyInfoTag;
			}
			set
			{
				this._PrivacyInfoTag = value;
				this.HasPrivacyInfoTag = true;
			}
		}

		public AccountFieldTags()
		{
		}

		public void AddGameAccountTags(RegionTag val)
		{
			this._GameAccountTags.Add(val);
		}

		public void AddGameLevelInfoTags(ProgramTag val)
		{
			this._GameLevelInfoTags.Add(val);
		}

		public void AddGameStatusTags(ProgramTag val)
		{
			this._GameStatusTags.Add(val);
		}

		public void ClearGameAccountTags()
		{
			this._GameAccountTags.Clear();
		}

		public void ClearGameLevelInfoTags()
		{
			this._GameLevelInfoTags.Clear();
		}

		public void ClearGameStatusTags()
		{
			this._GameStatusTags.Clear();
		}

		public void Deserialize(Stream stream)
		{
			AccountFieldTags.Deserialize(stream, this);
		}

		public static AccountFieldTags Deserialize(Stream stream, AccountFieldTags instance)
		{
			return AccountFieldTags.Deserialize(stream, instance, (long)-1);
		}

		public static AccountFieldTags Deserialize(Stream stream, AccountFieldTags instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.GameLevelInfoTags == null)
			{
				instance.GameLevelInfoTags = new List<ProgramTag>();
			}
			if (instance.GameStatusTags == null)
			{
				instance.GameStatusTags = new List<ProgramTag>();
			}
			if (instance.GameAccountTags == null)
			{
				instance.GameAccountTags = new List<RegionTag>();
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
					else if (num == 21)
					{
						instance.AccountLevelInfoTag = binaryReader.ReadUInt32();
					}
					else if (num == 29)
					{
						instance.PrivacyInfoTag = binaryReader.ReadUInt32();
					}
					else if (num == 37)
					{
						instance.ParentalControlInfoTag = binaryReader.ReadUInt32();
					}
					else if (num == 58)
					{
						instance.GameLevelInfoTags.Add(ProgramTag.DeserializeLengthDelimited(stream));
					}
					else if (num == 74)
					{
						instance.GameStatusTags.Add(ProgramTag.DeserializeLengthDelimited(stream));
					}
					else if (num == 90)
					{
						instance.GameAccountTags.Add(RegionTag.DeserializeLengthDelimited(stream));
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

		public static AccountFieldTags DeserializeLengthDelimited(Stream stream)
		{
			AccountFieldTags accountFieldTag = new AccountFieldTags();
			AccountFieldTags.DeserializeLengthDelimited(stream, accountFieldTag);
			return accountFieldTag;
		}

		public static AccountFieldTags DeserializeLengthDelimited(Stream stream, AccountFieldTags instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return AccountFieldTags.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			AccountFieldTags accountFieldTag = obj as AccountFieldTags;
			if (accountFieldTag == null)
			{
				return false;
			}
			if (this.HasAccountLevelInfoTag != accountFieldTag.HasAccountLevelInfoTag || this.HasAccountLevelInfoTag && !this.AccountLevelInfoTag.Equals(accountFieldTag.AccountLevelInfoTag))
			{
				return false;
			}
			if (this.HasPrivacyInfoTag != accountFieldTag.HasPrivacyInfoTag || this.HasPrivacyInfoTag && !this.PrivacyInfoTag.Equals(accountFieldTag.PrivacyInfoTag))
			{
				return false;
			}
			if (this.HasParentalControlInfoTag != accountFieldTag.HasParentalControlInfoTag || this.HasParentalControlInfoTag && !this.ParentalControlInfoTag.Equals(accountFieldTag.ParentalControlInfoTag))
			{
				return false;
			}
			if (this.GameLevelInfoTags.Count != accountFieldTag.GameLevelInfoTags.Count)
			{
				return false;
			}
			for (int i = 0; i < this.GameLevelInfoTags.Count; i++)
			{
				if (!this.GameLevelInfoTags[i].Equals(accountFieldTag.GameLevelInfoTags[i]))
				{
					return false;
				}
			}
			if (this.GameStatusTags.Count != accountFieldTag.GameStatusTags.Count)
			{
				return false;
			}
			for (int j = 0; j < this.GameStatusTags.Count; j++)
			{
				if (!this.GameStatusTags[j].Equals(accountFieldTag.GameStatusTags[j]))
				{
					return false;
				}
			}
			if (this.GameAccountTags.Count != accountFieldTag.GameAccountTags.Count)
			{
				return false;
			}
			for (int k = 0; k < this.GameAccountTags.Count; k++)
			{
				if (!this.GameAccountTags[k].Equals(accountFieldTag.GameAccountTags[k]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasAccountLevelInfoTag)
			{
				hashCode ^= this.AccountLevelInfoTag.GetHashCode();
			}
			if (this.HasPrivacyInfoTag)
			{
				hashCode ^= this.PrivacyInfoTag.GetHashCode();
			}
			if (this.HasParentalControlInfoTag)
			{
				hashCode ^= this.ParentalControlInfoTag.GetHashCode();
			}
			foreach (ProgramTag gameLevelInfoTag in this.GameLevelInfoTags)
			{
				hashCode ^= gameLevelInfoTag.GetHashCode();
			}
			foreach (ProgramTag gameStatusTag in this.GameStatusTags)
			{
				hashCode ^= gameStatusTag.GetHashCode();
			}
			foreach (RegionTag gameAccountTag in this.GameAccountTags)
			{
				hashCode ^= gameAccountTag.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasAccountLevelInfoTag)
			{
				num++;
				num += 4;
			}
			if (this.HasPrivacyInfoTag)
			{
				num++;
				num += 4;
			}
			if (this.HasParentalControlInfoTag)
			{
				num++;
				num += 4;
			}
			if (this.GameLevelInfoTags.Count > 0)
			{
				foreach (ProgramTag gameLevelInfoTag in this.GameLevelInfoTags)
				{
					num++;
					uint serializedSize = gameLevelInfoTag.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.GameStatusTags.Count > 0)
			{
				foreach (ProgramTag gameStatusTag in this.GameStatusTags)
				{
					num++;
					uint serializedSize1 = gameStatusTag.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			if (this.GameAccountTags.Count > 0)
			{
				foreach (RegionTag gameAccountTag in this.GameAccountTags)
				{
					num++;
					uint num1 = gameAccountTag.GetSerializedSize();
					num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
				}
			}
			return num;
		}

		public static AccountFieldTags ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<AccountFieldTags>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			AccountFieldTags.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, AccountFieldTags instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasAccountLevelInfoTag)
			{
				stream.WriteByte(21);
				binaryWriter.Write(instance.AccountLevelInfoTag);
			}
			if (instance.HasPrivacyInfoTag)
			{
				stream.WriteByte(29);
				binaryWriter.Write(instance.PrivacyInfoTag);
			}
			if (instance.HasParentalControlInfoTag)
			{
				stream.WriteByte(37);
				binaryWriter.Write(instance.ParentalControlInfoTag);
			}
			if (instance.GameLevelInfoTags.Count > 0)
			{
				foreach (ProgramTag gameLevelInfoTag in instance.GameLevelInfoTags)
				{
					stream.WriteByte(58);
					ProtocolParser.WriteUInt32(stream, gameLevelInfoTag.GetSerializedSize());
					ProgramTag.Serialize(stream, gameLevelInfoTag);
				}
			}
			if (instance.GameStatusTags.Count > 0)
			{
				foreach (ProgramTag gameStatusTag in instance.GameStatusTags)
				{
					stream.WriteByte(74);
					ProtocolParser.WriteUInt32(stream, gameStatusTag.GetSerializedSize());
					ProgramTag.Serialize(stream, gameStatusTag);
				}
			}
			if (instance.GameAccountTags.Count > 0)
			{
				foreach (RegionTag gameAccountTag in instance.GameAccountTags)
				{
					stream.WriteByte(90);
					ProtocolParser.WriteUInt32(stream, gameAccountTag.GetSerializedSize());
					RegionTag.Serialize(stream, gameAccountTag);
				}
			}
		}

		public void SetAccountLevelInfoTag(uint val)
		{
			this.AccountLevelInfoTag = val;
		}

		public void SetGameAccountTags(List<RegionTag> val)
		{
			this.GameAccountTags = val;
		}

		public void SetGameLevelInfoTags(List<ProgramTag> val)
		{
			this.GameLevelInfoTags = val;
		}

		public void SetGameStatusTags(List<ProgramTag> val)
		{
			this.GameStatusTags = val;
		}

		public void SetParentalControlInfoTag(uint val)
		{
			this.ParentalControlInfoTag = val;
		}

		public void SetPrivacyInfoTag(uint val)
		{
			this.PrivacyInfoTag = val;
		}
	}
}