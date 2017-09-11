using System;
using System.IO;
using System.Text;

namespace bnet.protocol.account
{
	public class GameSessionLocation : IProtoBuf
	{
		public bool HasIpAddress;

		private string _IpAddress;

		public bool HasCountry;

		private uint _Country;

		public bool HasCity;

		private string _City;

		public string City
		{
			get
			{
				return this._City;
			}
			set
			{
				this._City = value;
				this.HasCity = value != null;
			}
		}

		public uint Country
		{
			get
			{
				return this._Country;
			}
			set
			{
				this._Country = value;
				this.HasCountry = true;
			}
		}

		public string IpAddress
		{
			get
			{
				return this._IpAddress;
			}
			set
			{
				this._IpAddress = value;
				this.HasIpAddress = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public GameSessionLocation()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameSessionLocation.Deserialize(stream, this);
		}

		public static GameSessionLocation Deserialize(Stream stream, GameSessionLocation instance)
		{
			return GameSessionLocation.Deserialize(stream, instance, (long)-1);
		}

		public static GameSessionLocation Deserialize(Stream stream, GameSessionLocation instance, long limit)
		{
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
							instance.IpAddress = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 16)
						{
							instance.Country = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 26)
						{
							instance.City = ProtocolParser.ReadString(stream);
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

		public static GameSessionLocation DeserializeLengthDelimited(Stream stream)
		{
			GameSessionLocation gameSessionLocation = new GameSessionLocation();
			GameSessionLocation.DeserializeLengthDelimited(stream, gameSessionLocation);
			return gameSessionLocation;
		}

		public static GameSessionLocation DeserializeLengthDelimited(Stream stream, GameSessionLocation instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameSessionLocation.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameSessionLocation gameSessionLocation = obj as GameSessionLocation;
			if (gameSessionLocation == null)
			{
				return false;
			}
			if (this.HasIpAddress != gameSessionLocation.HasIpAddress || this.HasIpAddress && !this.IpAddress.Equals(gameSessionLocation.IpAddress))
			{
				return false;
			}
			if (this.HasCountry != gameSessionLocation.HasCountry || this.HasCountry && !this.Country.Equals(gameSessionLocation.Country))
			{
				return false;
			}
			if (this.HasCity == gameSessionLocation.HasCity && (!this.HasCity || this.City.Equals(gameSessionLocation.City)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasIpAddress)
			{
				hashCode ^= this.IpAddress.GetHashCode();
			}
			if (this.HasCountry)
			{
				hashCode ^= this.Country.GetHashCode();
			}
			if (this.HasCity)
			{
				hashCode ^= this.City.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasIpAddress)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.IpAddress);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasCountry)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.Country);
			}
			if (this.HasCity)
			{
				num++;
				uint byteCount1 = (uint)Encoding.UTF8.GetByteCount(this.City);
				num = num + ProtocolParser.SizeOfUInt32(byteCount1) + byteCount1;
			}
			return num;
		}

		public static GameSessionLocation ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameSessionLocation>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameSessionLocation.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameSessionLocation instance)
		{
			if (instance.HasIpAddress)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.IpAddress));
			}
			if (instance.HasCountry)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.Country);
			}
			if (instance.HasCity)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.City));
			}
		}

		public void SetCity(string val)
		{
			this.City = val;
		}

		public void SetCountry(uint val)
		{
			this.Country = val;
		}

		public void SetIpAddress(string val)
		{
			this.IpAddress = val;
		}
	}
}