using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GetAccountRequest : IProtoBuf
	{
		public bool HasRef;

		private AccountReference _Ref;

		public bool HasFetchAll;

		private bool _FetchAll;

		public bool HasFetchBlob;

		private bool _FetchBlob;

		public bool HasFetchId;

		private bool _FetchId;

		public bool HasFetchEmail;

		private bool _FetchEmail;

		public bool HasFetchBattleTag;

		private bool _FetchBattleTag;

		public bool HasFetchFullName;

		private bool _FetchFullName;

		public bool HasFetchLinks;

		private bool _FetchLinks;

		public bool HasFetchParentalControls;

		private bool _FetchParentalControls;

		public bool FetchAll
		{
			get
			{
				return this._FetchAll;
			}
			set
			{
				this._FetchAll = value;
				this.HasFetchAll = true;
			}
		}

		public bool FetchBattleTag
		{
			get
			{
				return this._FetchBattleTag;
			}
			set
			{
				this._FetchBattleTag = value;
				this.HasFetchBattleTag = true;
			}
		}

		public bool FetchBlob
		{
			get
			{
				return this._FetchBlob;
			}
			set
			{
				this._FetchBlob = value;
				this.HasFetchBlob = true;
			}
		}

		public bool FetchEmail
		{
			get
			{
				return this._FetchEmail;
			}
			set
			{
				this._FetchEmail = value;
				this.HasFetchEmail = true;
			}
		}

		public bool FetchFullName
		{
			get
			{
				return this._FetchFullName;
			}
			set
			{
				this._FetchFullName = value;
				this.HasFetchFullName = true;
			}
		}

		public bool FetchId
		{
			get
			{
				return this._FetchId;
			}
			set
			{
				this._FetchId = value;
				this.HasFetchId = true;
			}
		}

		public bool FetchLinks
		{
			get
			{
				return this._FetchLinks;
			}
			set
			{
				this._FetchLinks = value;
				this.HasFetchLinks = true;
			}
		}

		public bool FetchParentalControls
		{
			get
			{
				return this._FetchParentalControls;
			}
			set
			{
				this._FetchParentalControls = value;
				this.HasFetchParentalControls = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public AccountReference Ref
		{
			get
			{
				return this._Ref;
			}
			set
			{
				this._Ref = value;
				this.HasRef = value != null;
			}
		}

		public GetAccountRequest()
		{
		}

		public void Deserialize(Stream stream)
		{
			GetAccountRequest.Deserialize(stream, this);
		}

		public static GetAccountRequest Deserialize(Stream stream, GetAccountRequest instance)
		{
			return GetAccountRequest.Deserialize(stream, instance, (long)-1);
		}

		public static GetAccountRequest Deserialize(Stream stream, GetAccountRequest instance, long limit)
		{
			instance.FetchAll = false;
			instance.FetchBlob = false;
			instance.FetchId = false;
			instance.FetchEmail = false;
			instance.FetchBattleTag = false;
			instance.FetchFullName = false;
			instance.FetchLinks = false;
			instance.FetchParentalControls = false;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 10)
						{
							if (instance.Ref != null)
							{
								AccountReference.DeserializeLengthDelimited(stream, instance.Ref);
							}
							else
							{
								instance.Ref = AccountReference.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 80)
						{
							instance.FetchAll = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 88)
						{
							instance.FetchBlob = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 96)
						{
							instance.FetchId = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 104)
						{
							instance.FetchEmail = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 112)
						{
							instance.FetchBattleTag = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 120)
						{
							instance.FetchFullName = ProtocolParser.ReadBool(stream);
						}
						else
						{
							Key key = ProtocolParser.ReadKey((byte)num, stream);
							uint field = key.Field;
							if (field == 16)
							{
								if (key.WireType == Wire.Varint)
								{
									instance.FetchLinks = ProtocolParser.ReadBool(stream);
									continue;
								}
							}
							else if (field != 17)
							{
								if (field == 0)
								{
									throw new ProtocolBufferException("Invalid field id: 0, something went wrong in the stream");
								}
								ProtocolParser.SkipKey(stream, key);
							}
							else if (key.WireType == Wire.Varint)
							{
								instance.FetchParentalControls = ProtocolParser.ReadBool(stream);
								continue;
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

		public static GetAccountRequest DeserializeLengthDelimited(Stream stream)
		{
			GetAccountRequest getAccountRequest = new GetAccountRequest();
			GetAccountRequest.DeserializeLengthDelimited(stream, getAccountRequest);
			return getAccountRequest;
		}

		public static GetAccountRequest DeserializeLengthDelimited(Stream stream, GetAccountRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GetAccountRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetAccountRequest getAccountRequest = obj as GetAccountRequest;
			if (getAccountRequest == null)
			{
				return false;
			}
			if (this.HasRef != getAccountRequest.HasRef || this.HasRef && !this.Ref.Equals(getAccountRequest.Ref))
			{
				return false;
			}
			if (this.HasFetchAll != getAccountRequest.HasFetchAll || this.HasFetchAll && !this.FetchAll.Equals(getAccountRequest.FetchAll))
			{
				return false;
			}
			if (this.HasFetchBlob != getAccountRequest.HasFetchBlob || this.HasFetchBlob && !this.FetchBlob.Equals(getAccountRequest.FetchBlob))
			{
				return false;
			}
			if (this.HasFetchId != getAccountRequest.HasFetchId || this.HasFetchId && !this.FetchId.Equals(getAccountRequest.FetchId))
			{
				return false;
			}
			if (this.HasFetchEmail != getAccountRequest.HasFetchEmail || this.HasFetchEmail && !this.FetchEmail.Equals(getAccountRequest.FetchEmail))
			{
				return false;
			}
			if (this.HasFetchBattleTag != getAccountRequest.HasFetchBattleTag || this.HasFetchBattleTag && !this.FetchBattleTag.Equals(getAccountRequest.FetchBattleTag))
			{
				return false;
			}
			if (this.HasFetchFullName != getAccountRequest.HasFetchFullName || this.HasFetchFullName && !this.FetchFullName.Equals(getAccountRequest.FetchFullName))
			{
				return false;
			}
			if (this.HasFetchLinks != getAccountRequest.HasFetchLinks || this.HasFetchLinks && !this.FetchLinks.Equals(getAccountRequest.FetchLinks))
			{
				return false;
			}
			if (this.HasFetchParentalControls == getAccountRequest.HasFetchParentalControls && (!this.HasFetchParentalControls || this.FetchParentalControls.Equals(getAccountRequest.FetchParentalControls)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasRef)
			{
				hashCode = hashCode ^ this.Ref.GetHashCode();
			}
			if (this.HasFetchAll)
			{
				hashCode = hashCode ^ this.FetchAll.GetHashCode();
			}
			if (this.HasFetchBlob)
			{
				hashCode = hashCode ^ this.FetchBlob.GetHashCode();
			}
			if (this.HasFetchId)
			{
				hashCode = hashCode ^ this.FetchId.GetHashCode();
			}
			if (this.HasFetchEmail)
			{
				hashCode = hashCode ^ this.FetchEmail.GetHashCode();
			}
			if (this.HasFetchBattleTag)
			{
				hashCode = hashCode ^ this.FetchBattleTag.GetHashCode();
			}
			if (this.HasFetchFullName)
			{
				hashCode = hashCode ^ this.FetchFullName.GetHashCode();
			}
			if (this.HasFetchLinks)
			{
				hashCode = hashCode ^ this.FetchLinks.GetHashCode();
			}
			if (this.HasFetchParentalControls)
			{
				hashCode = hashCode ^ this.FetchParentalControls.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasRef)
			{
				num++;
				uint serializedSize = this.Ref.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasFetchAll)
			{
				num++;
				num++;
			}
			if (this.HasFetchBlob)
			{
				num++;
				num++;
			}
			if (this.HasFetchId)
			{
				num++;
				num++;
			}
			if (this.HasFetchEmail)
			{
				num++;
				num++;
			}
			if (this.HasFetchBattleTag)
			{
				num++;
				num++;
			}
			if (this.HasFetchFullName)
			{
				num++;
				num++;
			}
			if (this.HasFetchLinks)
			{
				num = num + 2;
				num++;
			}
			if (this.HasFetchParentalControls)
			{
				num = num + 2;
				num++;
			}
			return num;
		}

		public static GetAccountRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetAccountRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetAccountRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetAccountRequest instance)
		{
			if (instance.HasRef)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.Ref.GetSerializedSize());
				AccountReference.Serialize(stream, instance.Ref);
			}
			if (instance.HasFetchAll)
			{
				stream.WriteByte(80);
				ProtocolParser.WriteBool(stream, instance.FetchAll);
			}
			if (instance.HasFetchBlob)
			{
				stream.WriteByte(88);
				ProtocolParser.WriteBool(stream, instance.FetchBlob);
			}
			if (instance.HasFetchId)
			{
				stream.WriteByte(96);
				ProtocolParser.WriteBool(stream, instance.FetchId);
			}
			if (instance.HasFetchEmail)
			{
				stream.WriteByte(104);
				ProtocolParser.WriteBool(stream, instance.FetchEmail);
			}
			if (instance.HasFetchBattleTag)
			{
				stream.WriteByte(112);
				ProtocolParser.WriteBool(stream, instance.FetchBattleTag);
			}
			if (instance.HasFetchFullName)
			{
				stream.WriteByte(120);
				ProtocolParser.WriteBool(stream, instance.FetchFullName);
			}
			if (instance.HasFetchLinks)
			{
				stream.WriteByte(128);
				stream.WriteByte(1);
				ProtocolParser.WriteBool(stream, instance.FetchLinks);
			}
			if (instance.HasFetchParentalControls)
			{
				stream.WriteByte(136);
				stream.WriteByte(1);
				ProtocolParser.WriteBool(stream, instance.FetchParentalControls);
			}
		}

		public void SetFetchAll(bool val)
		{
			this.FetchAll = val;
		}

		public void SetFetchBattleTag(bool val)
		{
			this.FetchBattleTag = val;
		}

		public void SetFetchBlob(bool val)
		{
			this.FetchBlob = val;
		}

		public void SetFetchEmail(bool val)
		{
			this.FetchEmail = val;
		}

		public void SetFetchFullName(bool val)
		{
			this.FetchFullName = val;
		}

		public void SetFetchId(bool val)
		{
			this.FetchId = val;
		}

		public void SetFetchLinks(bool val)
		{
			this.FetchLinks = val;
		}

		public void SetFetchParentalControls(bool val)
		{
			this.FetchParentalControls = val;
		}

		public void SetRef(AccountReference val)
		{
			this.Ref = val;
		}
	}
}