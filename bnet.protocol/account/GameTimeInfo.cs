using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GameTimeInfo : IProtoBuf
	{
		public bool HasIsUnlimitedPlayTime;

		private bool _IsUnlimitedPlayTime;

		public bool HasPlayTimeExpires;

		private ulong _PlayTimeExpires;

		public bool HasIsSubscription;

		private bool _IsSubscription;

		public bool HasIsRecurringSubscription;

		private bool _IsRecurringSubscription;

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool IsRecurringSubscription
		{
			get
			{
				return this._IsRecurringSubscription;
			}
			set
			{
				this._IsRecurringSubscription = value;
				this.HasIsRecurringSubscription = true;
			}
		}

		public bool IsSubscription
		{
			get
			{
				return this._IsSubscription;
			}
			set
			{
				this._IsSubscription = value;
				this.HasIsSubscription = true;
			}
		}

		public bool IsUnlimitedPlayTime
		{
			get
			{
				return this._IsUnlimitedPlayTime;
			}
			set
			{
				this._IsUnlimitedPlayTime = value;
				this.HasIsUnlimitedPlayTime = true;
			}
		}

		public ulong PlayTimeExpires
		{
			get
			{
				return this._PlayTimeExpires;
			}
			set
			{
				this._PlayTimeExpires = value;
				this.HasPlayTimeExpires = true;
			}
		}

		public GameTimeInfo()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameTimeInfo.Deserialize(stream, this);
		}

		public static GameTimeInfo Deserialize(Stream stream, GameTimeInfo instance)
		{
			return GameTimeInfo.Deserialize(stream, instance, (long)-1);
		}

		public static GameTimeInfo Deserialize(Stream stream, GameTimeInfo instance, long limit)
		{
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
							instance.IsUnlimitedPlayTime = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 40)
						{
							instance.PlayTimeExpires = ProtocolParser.ReadUInt64(stream);
						}
						else if (num1 == 48)
						{
							instance.IsSubscription = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 56)
						{
							instance.IsRecurringSubscription = ProtocolParser.ReadBool(stream);
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

		public static GameTimeInfo DeserializeLengthDelimited(Stream stream)
		{
			GameTimeInfo gameTimeInfo = new GameTimeInfo();
			GameTimeInfo.DeserializeLengthDelimited(stream, gameTimeInfo);
			return gameTimeInfo;
		}

		public static GameTimeInfo DeserializeLengthDelimited(Stream stream, GameTimeInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GameTimeInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameTimeInfo gameTimeInfo = obj as GameTimeInfo;
			if (gameTimeInfo == null)
			{
				return false;
			}
			if (this.HasIsUnlimitedPlayTime != gameTimeInfo.HasIsUnlimitedPlayTime || this.HasIsUnlimitedPlayTime && !this.IsUnlimitedPlayTime.Equals(gameTimeInfo.IsUnlimitedPlayTime))
			{
				return false;
			}
			if (this.HasPlayTimeExpires != gameTimeInfo.HasPlayTimeExpires || this.HasPlayTimeExpires && !this.PlayTimeExpires.Equals(gameTimeInfo.PlayTimeExpires))
			{
				return false;
			}
			if (this.HasIsSubscription != gameTimeInfo.HasIsSubscription || this.HasIsSubscription && !this.IsSubscription.Equals(gameTimeInfo.IsSubscription))
			{
				return false;
			}
			if (this.HasIsRecurringSubscription == gameTimeInfo.HasIsRecurringSubscription && (!this.HasIsRecurringSubscription || this.IsRecurringSubscription.Equals(gameTimeInfo.IsRecurringSubscription)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasIsUnlimitedPlayTime)
			{
				hashCode = hashCode ^ this.IsUnlimitedPlayTime.GetHashCode();
			}
			if (this.HasPlayTimeExpires)
			{
				hashCode = hashCode ^ this.PlayTimeExpires.GetHashCode();
			}
			if (this.HasIsSubscription)
			{
				hashCode = hashCode ^ this.IsSubscription.GetHashCode();
			}
			if (this.HasIsRecurringSubscription)
			{
				hashCode = hashCode ^ this.IsRecurringSubscription.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasIsUnlimitedPlayTime)
			{
				num++;
				num++;
			}
			if (this.HasPlayTimeExpires)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.PlayTimeExpires);
			}
			if (this.HasIsSubscription)
			{
				num++;
				num++;
			}
			if (this.HasIsRecurringSubscription)
			{
				num++;
				num++;
			}
			return num;
		}

		public static GameTimeInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameTimeInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameTimeInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameTimeInfo instance)
		{
			if (instance.HasIsUnlimitedPlayTime)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteBool(stream, instance.IsUnlimitedPlayTime);
			}
			if (instance.HasPlayTimeExpires)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt64(stream, instance.PlayTimeExpires);
			}
			if (instance.HasIsSubscription)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteBool(stream, instance.IsSubscription);
			}
			if (instance.HasIsRecurringSubscription)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteBool(stream, instance.IsRecurringSubscription);
			}
		}

		public void SetIsRecurringSubscription(bool val)
		{
			this.IsRecurringSubscription = val;
		}

		public void SetIsSubscription(bool val)
		{
			this.IsSubscription = val;
		}

		public void SetIsUnlimitedPlayTime(bool val)
		{
			this.IsUnlimitedPlayTime = val;
		}

		public void SetPlayTimeExpires(ulong val)
		{
			this.PlayTimeExpires = val;
		}
	}
}