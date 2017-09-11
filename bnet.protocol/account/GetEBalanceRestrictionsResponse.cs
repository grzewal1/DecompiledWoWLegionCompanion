using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.account
{
	public class GetEBalanceRestrictionsResponse : IProtoBuf
	{
		private List<CurrencyRestriction> _CurrencyRestrictions = new List<CurrencyRestriction>();

		public List<CurrencyRestriction> CurrencyRestrictions
		{
			get
			{
				return this._CurrencyRestrictions;
			}
			set
			{
				this._CurrencyRestrictions = value;
			}
		}

		public int CurrencyRestrictionsCount
		{
			get
			{
				return this._CurrencyRestrictions.Count;
			}
		}

		public List<CurrencyRestriction> CurrencyRestrictionsList
		{
			get
			{
				return this._CurrencyRestrictions;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GetEBalanceRestrictionsResponse()
		{
		}

		public void AddCurrencyRestrictions(CurrencyRestriction val)
		{
			this._CurrencyRestrictions.Add(val);
		}

		public void ClearCurrencyRestrictions()
		{
			this._CurrencyRestrictions.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GetEBalanceRestrictionsResponse.Deserialize(stream, this);
		}

		public static GetEBalanceRestrictionsResponse Deserialize(Stream stream, GetEBalanceRestrictionsResponse instance)
		{
			return GetEBalanceRestrictionsResponse.Deserialize(stream, instance, (long)-1);
		}

		public static GetEBalanceRestrictionsResponse Deserialize(Stream stream, GetEBalanceRestrictionsResponse instance, long limit)
		{
			if (instance.CurrencyRestrictions == null)
			{
				instance.CurrencyRestrictions = new List<CurrencyRestriction>();
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
					else if (num == 10)
					{
						instance.CurrencyRestrictions.Add(CurrencyRestriction.DeserializeLengthDelimited(stream));
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

		public static GetEBalanceRestrictionsResponse DeserializeLengthDelimited(Stream stream)
		{
			GetEBalanceRestrictionsResponse getEBalanceRestrictionsResponse = new GetEBalanceRestrictionsResponse();
			GetEBalanceRestrictionsResponse.DeserializeLengthDelimited(stream, getEBalanceRestrictionsResponse);
			return getEBalanceRestrictionsResponse;
		}

		public static GetEBalanceRestrictionsResponse DeserializeLengthDelimited(Stream stream, GetEBalanceRestrictionsResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GetEBalanceRestrictionsResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GetEBalanceRestrictionsResponse getEBalanceRestrictionsResponse = obj as GetEBalanceRestrictionsResponse;
			if (getEBalanceRestrictionsResponse == null)
			{
				return false;
			}
			if (this.CurrencyRestrictions.Count != getEBalanceRestrictionsResponse.CurrencyRestrictions.Count)
			{
				return false;
			}
			for (int i = 0; i < this.CurrencyRestrictions.Count; i++)
			{
				if (!this.CurrencyRestrictions[i].Equals(getEBalanceRestrictionsResponse.CurrencyRestrictions[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (CurrencyRestriction currencyRestriction in this.CurrencyRestrictions)
			{
				hashCode ^= currencyRestriction.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.CurrencyRestrictions.Count > 0)
			{
				foreach (CurrencyRestriction currencyRestriction in this.CurrencyRestrictions)
				{
					num++;
					uint serializedSize = currencyRestriction.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static GetEBalanceRestrictionsResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GetEBalanceRestrictionsResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GetEBalanceRestrictionsResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GetEBalanceRestrictionsResponse instance)
		{
			if (instance.CurrencyRestrictions.Count > 0)
			{
				foreach (CurrencyRestriction currencyRestriction in instance.CurrencyRestrictions)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, currencyRestriction.GetSerializedSize());
					CurrencyRestriction.Serialize(stream, currencyRestriction);
				}
			}
		}

		public void SetCurrencyRestrictions(List<CurrencyRestriction> val)
		{
			this.CurrencyRestrictions = val;
		}
	}
}