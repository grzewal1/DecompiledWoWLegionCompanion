using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace bnet.protocol.account
{
	public class GameLevelInfo : IProtoBuf
	{
		public bool HasIsStarterEdition;

		private bool _IsStarterEdition;

		public bool HasIsTrial;

		private bool _IsTrial;

		public bool HasIsLifetime;

		private bool _IsLifetime;

		public bool HasIsRestricted;

		private bool _IsRestricted;

		public bool HasIsBeta;

		private bool _IsBeta;

		public bool HasName;

		private string _Name;

		public bool HasProgram;

		private uint _Program;

		private List<AccountLicense> _Licenses = new List<AccountLicense>();

		public bool HasRealmPermissions;

		private uint _RealmPermissions;

		public bool IsBeta
		{
			get
			{
				return this._IsBeta;
			}
			set
			{
				this._IsBeta = value;
				this.HasIsBeta = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool IsLifetime
		{
			get
			{
				return this._IsLifetime;
			}
			set
			{
				this._IsLifetime = value;
				this.HasIsLifetime = true;
			}
		}

		public bool IsRestricted
		{
			get
			{
				return this._IsRestricted;
			}
			set
			{
				this._IsRestricted = value;
				this.HasIsRestricted = true;
			}
		}

		public bool IsStarterEdition
		{
			get
			{
				return this._IsStarterEdition;
			}
			set
			{
				this._IsStarterEdition = value;
				this.HasIsStarterEdition = true;
			}
		}

		public bool IsTrial
		{
			get
			{
				return this._IsTrial;
			}
			set
			{
				this._IsTrial = value;
				this.HasIsTrial = true;
			}
		}

		public List<AccountLicense> Licenses
		{
			get
			{
				return this._Licenses;
			}
			set
			{
				this._Licenses = value;
			}
		}

		public int LicensesCount
		{
			get
			{
				return this._Licenses.Count;
			}
		}

		public List<AccountLicense> LicensesList
		{
			get
			{
				return this._Licenses;
			}
		}

		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				this._Name = value;
				this.HasName = value != null;
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

		public GameLevelInfo()
		{
		}

		public void AddLicenses(AccountLicense val)
		{
			this._Licenses.Add(val);
		}

		public void ClearLicenses()
		{
			this._Licenses.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GameLevelInfo.Deserialize(stream, this);
		}

		public static GameLevelInfo Deserialize(Stream stream, GameLevelInfo instance)
		{
			return GameLevelInfo.Deserialize(stream, instance, (long)-1);
		}

		public static GameLevelInfo Deserialize(Stream stream, GameLevelInfo instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.Licenses == null)
			{
				instance.Licenses = new List<AccountLicense>();
			}
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 24)
						{
							instance.IsStarterEdition = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 32)
						{
							instance.IsTrial = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 40)
						{
							instance.IsLifetime = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 48)
						{
							instance.IsRestricted = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 56)
						{
							instance.IsBeta = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 66)
						{
							instance.Name = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 77)
						{
							instance.Program = binaryReader.ReadUInt32();
						}
						else if (num1 == 82)
						{
							instance.Licenses.Add(AccountLicense.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 88)
						{
							instance.RealmPermissions = ProtocolParser.ReadUInt32(stream);
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

		public static GameLevelInfo DeserializeLengthDelimited(Stream stream)
		{
			GameLevelInfo gameLevelInfo = new GameLevelInfo();
			GameLevelInfo.DeserializeLengthDelimited(stream, gameLevelInfo);
			return gameLevelInfo;
		}

		public static GameLevelInfo DeserializeLengthDelimited(Stream stream, GameLevelInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GameLevelInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameLevelInfo gameLevelInfo = obj as GameLevelInfo;
			if (gameLevelInfo == null)
			{
				return false;
			}
			if (this.HasIsStarterEdition != gameLevelInfo.HasIsStarterEdition || this.HasIsStarterEdition && !this.IsStarterEdition.Equals(gameLevelInfo.IsStarterEdition))
			{
				return false;
			}
			if (this.HasIsTrial != gameLevelInfo.HasIsTrial || this.HasIsTrial && !this.IsTrial.Equals(gameLevelInfo.IsTrial))
			{
				return false;
			}
			if (this.HasIsLifetime != gameLevelInfo.HasIsLifetime || this.HasIsLifetime && !this.IsLifetime.Equals(gameLevelInfo.IsLifetime))
			{
				return false;
			}
			if (this.HasIsRestricted != gameLevelInfo.HasIsRestricted || this.HasIsRestricted && !this.IsRestricted.Equals(gameLevelInfo.IsRestricted))
			{
				return false;
			}
			if (this.HasIsBeta != gameLevelInfo.HasIsBeta || this.HasIsBeta && !this.IsBeta.Equals(gameLevelInfo.IsBeta))
			{
				return false;
			}
			if (this.HasName != gameLevelInfo.HasName || this.HasName && !this.Name.Equals(gameLevelInfo.Name))
			{
				return false;
			}
			if (this.HasProgram != gameLevelInfo.HasProgram || this.HasProgram && !this.Program.Equals(gameLevelInfo.Program))
			{
				return false;
			}
			if (this.Licenses.Count != gameLevelInfo.Licenses.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Licenses.Count; i++)
			{
				if (!this.Licenses[i].Equals(gameLevelInfo.Licenses[i]))
				{
					return false;
				}
			}
			if (this.HasRealmPermissions == gameLevelInfo.HasRealmPermissions && (!this.HasRealmPermissions || this.RealmPermissions.Equals(gameLevelInfo.RealmPermissions)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasIsStarterEdition)
			{
				hashCode = hashCode ^ this.IsStarterEdition.GetHashCode();
			}
			if (this.HasIsTrial)
			{
				hashCode = hashCode ^ this.IsTrial.GetHashCode();
			}
			if (this.HasIsLifetime)
			{
				hashCode = hashCode ^ this.IsLifetime.GetHashCode();
			}
			if (this.HasIsRestricted)
			{
				hashCode = hashCode ^ this.IsRestricted.GetHashCode();
			}
			if (this.HasIsBeta)
			{
				hashCode = hashCode ^ this.IsBeta.GetHashCode();
			}
			if (this.HasName)
			{
				hashCode = hashCode ^ this.Name.GetHashCode();
			}
			if (this.HasProgram)
			{
				hashCode = hashCode ^ this.Program.GetHashCode();
			}
			foreach (AccountLicense license in this.Licenses)
			{
				hashCode = hashCode ^ license.GetHashCode();
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
			if (this.HasIsStarterEdition)
			{
				num++;
				num++;
			}
			if (this.HasIsTrial)
			{
				num++;
				num++;
			}
			if (this.HasIsLifetime)
			{
				num++;
				num++;
			}
			if (this.HasIsRestricted)
			{
				num++;
				num++;
			}
			if (this.HasIsBeta)
			{
				num++;
				num++;
			}
			if (this.HasName)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Name);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasProgram)
			{
				num++;
				num = num + 4;
			}
			if (this.Licenses.Count > 0)
			{
				foreach (AccountLicense license in this.Licenses)
				{
					num++;
					uint serializedSize = license.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.HasRealmPermissions)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.RealmPermissions);
			}
			return num;
		}

		public static GameLevelInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameLevelInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameLevelInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameLevelInfo instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasIsStarterEdition)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.IsStarterEdition);
			}
			if (instance.HasIsTrial)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteBool(stream, instance.IsTrial);
			}
			if (instance.HasIsLifetime)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.IsLifetime);
			}
			if (instance.HasIsRestricted)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteBool(stream, instance.IsRestricted);
			}
			if (instance.HasIsBeta)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteBool(stream, instance.IsBeta);
			}
			if (instance.HasName)
			{
				stream.WriteByte(66);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Name));
			}
			if (instance.HasProgram)
			{
				stream.WriteByte(77);
				binaryWriter.Write(instance.Program);
			}
			if (instance.Licenses.Count > 0)
			{
				foreach (AccountLicense license in instance.Licenses)
				{
					stream.WriteByte(82);
					ProtocolParser.WriteUInt32(stream, license.GetSerializedSize());
					AccountLicense.Serialize(stream, license);
				}
			}
			if (instance.HasRealmPermissions)
			{
				stream.WriteByte(88);
				ProtocolParser.WriteUInt32(stream, instance.RealmPermissions);
			}
		}

		public void SetIsBeta(bool val)
		{
			this.IsBeta = val;
		}

		public void SetIsLifetime(bool val)
		{
			this.IsLifetime = val;
		}

		public void SetIsRestricted(bool val)
		{
			this.IsRestricted = val;
		}

		public void SetIsStarterEdition(bool val)
		{
			this.IsStarterEdition = val;
		}

		public void SetIsTrial(bool val)
		{
			this.IsTrial = val;
		}

		public void SetLicenses(List<AccountLicense> val)
		{
			this.Licenses = val;
		}

		public void SetName(string val)
		{
			this.Name = val;
		}

		public void SetProgram(uint val)
		{
			this.Program = val;
		}

		public void SetRealmPermissions(uint val)
		{
			this.RealmPermissions = val;
		}
	}
}