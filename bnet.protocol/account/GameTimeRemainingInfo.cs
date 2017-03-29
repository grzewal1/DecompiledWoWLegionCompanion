using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GameTimeRemainingInfo : IProtoBuf
	{
		public bool HasMinutesRemaining;

		private uint _MinutesRemaining;

		public bool HasParentalDailyMinutesRemaining;

		private uint _ParentalDailyMinutesRemaining;

		public bool HasParentalWeeklyMinutesRemaining;

		private uint _ParentalWeeklyMinutesRemaining;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint MinutesRemaining
		{
			get
			{
				return this._MinutesRemaining;
			}
			set
			{
				this._MinutesRemaining = value;
				this.HasMinutesRemaining = true;
			}
		}

		public uint ParentalDailyMinutesRemaining
		{
			get
			{
				return this._ParentalDailyMinutesRemaining;
			}
			set
			{
				this._ParentalDailyMinutesRemaining = value;
				this.HasParentalDailyMinutesRemaining = true;
			}
		}

		public uint ParentalWeeklyMinutesRemaining
		{
			get
			{
				return this._ParentalWeeklyMinutesRemaining;
			}
			set
			{
				this._ParentalWeeklyMinutesRemaining = value;
				this.HasParentalWeeklyMinutesRemaining = true;
			}
		}

		public GameTimeRemainingInfo()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameTimeRemainingInfo.Deserialize(stream, this);
		}

		public static GameTimeRemainingInfo Deserialize(Stream stream, GameTimeRemainingInfo instance)
		{
			return GameTimeRemainingInfo.Deserialize(stream, instance, (long)-1);
		}

		public static GameTimeRemainingInfo Deserialize(Stream stream, GameTimeRemainingInfo instance, long limit)
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
							instance.MinutesRemaining = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 16)
						{
							instance.ParentalDailyMinutesRemaining = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 24)
						{
							instance.ParentalWeeklyMinutesRemaining = ProtocolParser.ReadUInt32(stream);
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

		public static GameTimeRemainingInfo DeserializeLengthDelimited(Stream stream)
		{
			GameTimeRemainingInfo gameTimeRemainingInfo = new GameTimeRemainingInfo();
			GameTimeRemainingInfo.DeserializeLengthDelimited(stream, gameTimeRemainingInfo);
			return gameTimeRemainingInfo;
		}

		public static GameTimeRemainingInfo DeserializeLengthDelimited(Stream stream, GameTimeRemainingInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GameTimeRemainingInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameTimeRemainingInfo gameTimeRemainingInfo = obj as GameTimeRemainingInfo;
			if (gameTimeRemainingInfo == null)
			{
				return false;
			}
			if (this.HasMinutesRemaining != gameTimeRemainingInfo.HasMinutesRemaining || this.HasMinutesRemaining && !this.MinutesRemaining.Equals(gameTimeRemainingInfo.MinutesRemaining))
			{
				return false;
			}
			if (this.HasParentalDailyMinutesRemaining != gameTimeRemainingInfo.HasParentalDailyMinutesRemaining || this.HasParentalDailyMinutesRemaining && !this.ParentalDailyMinutesRemaining.Equals(gameTimeRemainingInfo.ParentalDailyMinutesRemaining))
			{
				return false;
			}
			if (this.HasParentalWeeklyMinutesRemaining == gameTimeRemainingInfo.HasParentalWeeklyMinutesRemaining && (!this.HasParentalWeeklyMinutesRemaining || this.ParentalWeeklyMinutesRemaining.Equals(gameTimeRemainingInfo.ParentalWeeklyMinutesRemaining)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasMinutesRemaining)
			{
				hashCode = hashCode ^ this.MinutesRemaining.GetHashCode();
			}
			if (this.HasParentalDailyMinutesRemaining)
			{
				hashCode = hashCode ^ this.ParentalDailyMinutesRemaining.GetHashCode();
			}
			if (this.HasParentalWeeklyMinutesRemaining)
			{
				hashCode = hashCode ^ this.ParentalWeeklyMinutesRemaining.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasMinutesRemaining)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.MinutesRemaining);
			}
			if (this.HasParentalDailyMinutesRemaining)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.ParentalDailyMinutesRemaining);
			}
			if (this.HasParentalWeeklyMinutesRemaining)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.ParentalWeeklyMinutesRemaining);
			}
			return num;
		}

		public static GameTimeRemainingInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameTimeRemainingInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameTimeRemainingInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameTimeRemainingInfo instance)
		{
			if (instance.HasMinutesRemaining)
			{
				stream.WriteByte(8);
				ProtocolParser.WriteUInt32(stream, instance.MinutesRemaining);
			}
			if (instance.HasParentalDailyMinutesRemaining)
			{
				stream.WriteByte(16);
				ProtocolParser.WriteUInt32(stream, instance.ParentalDailyMinutesRemaining);
			}
			if (instance.HasParentalWeeklyMinutesRemaining)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.ParentalWeeklyMinutesRemaining);
			}
		}

		public void SetMinutesRemaining(uint val)
		{
			this.MinutesRemaining = val;
		}

		public void SetParentalDailyMinutesRemaining(uint val)
		{
			this.ParentalDailyMinutesRemaining = val;
		}

		public void SetParentalWeeklyMinutesRemaining(uint val)
		{
			this.ParentalWeeklyMinutesRemaining = val;
		}
	}
}