using System;
using System.IO;

namespace bnet.protocol.account
{
	public class GameSessionInfo : IProtoBuf
	{
		public bool HasStartTime;

		private uint _StartTime;

		public bool HasLocation;

		private GameSessionLocation _Location;

		public bool HasHasBenefactor;

		private bool _HasBenefactor;

		public bool HasIsUsingIgr;

		private bool _IsUsingIgr;

		public bool HasParentalControlsActive;

		private bool _ParentalControlsActive;

		public bool HasBenefactor
		{
			get
			{
				return this._HasBenefactor;
			}
			set
			{
				this._HasBenefactor = value;
				this.HasHasBenefactor = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public bool IsUsingIgr
		{
			get
			{
				return this._IsUsingIgr;
			}
			set
			{
				this._IsUsingIgr = value;
				this.HasIsUsingIgr = true;
			}
		}

		public GameSessionLocation Location
		{
			get
			{
				return this._Location;
			}
			set
			{
				this._Location = value;
				this.HasLocation = value != null;
			}
		}

		public bool ParentalControlsActive
		{
			get
			{
				return this._ParentalControlsActive;
			}
			set
			{
				this._ParentalControlsActive = value;
				this.HasParentalControlsActive = true;
			}
		}

		public uint StartTime
		{
			get
			{
				return this._StartTime;
			}
			set
			{
				this._StartTime = value;
				this.HasStartTime = true;
			}
		}

		public GameSessionInfo()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameSessionInfo.Deserialize(stream, this);
		}

		public static GameSessionInfo Deserialize(Stream stream, GameSessionInfo instance)
		{
			return GameSessionInfo.Deserialize(stream, instance, (long)-1);
		}

		public static GameSessionInfo Deserialize(Stream stream, GameSessionInfo instance, long limit)
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
							instance.StartTime = ProtocolParser.ReadUInt32(stream);
						}
						else if (num1 == 34)
						{
							if (instance.Location != null)
							{
								GameSessionLocation.DeserializeLengthDelimited(stream, instance.Location);
							}
							else
							{
								instance.Location = GameSessionLocation.DeserializeLengthDelimited(stream);
							}
						}
						else if (num1 == 40)
						{
							instance.HasBenefactor = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 48)
						{
							instance.IsUsingIgr = ProtocolParser.ReadBool(stream);
						}
						else if (num1 == 56)
						{
							instance.ParentalControlsActive = ProtocolParser.ReadBool(stream);
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

		public static GameSessionInfo DeserializeLengthDelimited(Stream stream)
		{
			GameSessionInfo gameSessionInfo = new GameSessionInfo();
			GameSessionInfo.DeserializeLengthDelimited(stream, gameSessionInfo);
			return gameSessionInfo;
		}

		public static GameSessionInfo DeserializeLengthDelimited(Stream stream, GameSessionInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GameSessionInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameSessionInfo gameSessionInfo = obj as GameSessionInfo;
			if (gameSessionInfo == null)
			{
				return false;
			}
			if (this.HasStartTime != gameSessionInfo.HasStartTime || this.HasStartTime && !this.StartTime.Equals(gameSessionInfo.StartTime))
			{
				return false;
			}
			if (this.HasLocation != gameSessionInfo.HasLocation || this.HasLocation && !this.Location.Equals(gameSessionInfo.Location))
			{
				return false;
			}
			if (this.HasHasBenefactor != gameSessionInfo.HasHasBenefactor || this.HasHasBenefactor && !this.HasBenefactor.Equals(gameSessionInfo.HasBenefactor))
			{
				return false;
			}
			if (this.HasIsUsingIgr != gameSessionInfo.HasIsUsingIgr || this.HasIsUsingIgr && !this.IsUsingIgr.Equals(gameSessionInfo.IsUsingIgr))
			{
				return false;
			}
			if (this.HasParentalControlsActive == gameSessionInfo.HasParentalControlsActive && (!this.HasParentalControlsActive || this.ParentalControlsActive.Equals(gameSessionInfo.ParentalControlsActive)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasStartTime)
			{
				hashCode = hashCode ^ this.StartTime.GetHashCode();
			}
			if (this.HasLocation)
			{
				hashCode = hashCode ^ this.Location.GetHashCode();
			}
			if (this.HasHasBenefactor)
			{
				hashCode = hashCode ^ this.HasBenefactor.GetHashCode();
			}
			if (this.HasIsUsingIgr)
			{
				hashCode = hashCode ^ this.IsUsingIgr.GetHashCode();
			}
			if (this.HasParentalControlsActive)
			{
				hashCode = hashCode ^ this.ParentalControlsActive.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasStartTime)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.StartTime);
			}
			if (this.HasLocation)
			{
				num++;
				uint serializedSize = this.Location.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasHasBenefactor)
			{
				num++;
				num++;
			}
			if (this.HasIsUsingIgr)
			{
				num++;
				num++;
			}
			if (this.HasParentalControlsActive)
			{
				num++;
				num++;
			}
			return num;
		}

		public static GameSessionInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameSessionInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameSessionInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameSessionInfo instance)
		{
			if (instance.HasStartTime)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.StartTime);
			}
			if (instance.HasLocation)
			{
				stream.WriteByte(34);
				ProtocolParser.WriteUInt32(stream, instance.Location.GetSerializedSize());
				GameSessionLocation.Serialize(stream, instance.Location);
			}
			if (instance.HasHasBenefactor)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteBool(stream, instance.HasBenefactor);
			}
			if (instance.HasIsUsingIgr)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteBool(stream, instance.IsUsingIgr);
			}
			if (instance.HasParentalControlsActive)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteBool(stream, instance.ParentalControlsActive);
			}
		}

		public void SetHasBenefactor(bool val)
		{
			this.HasBenefactor = val;
		}

		public void SetIsUsingIgr(bool val)
		{
			this.IsUsingIgr = val;
		}

		public void SetLocation(GameSessionLocation val)
		{
			this.Location = val;
		}

		public void SetParentalControlsActive(bool val)
		{
			this.ParentalControlsActive = val;
		}

		public void SetStartTime(uint val)
		{
			this.StartTime = val;
		}
	}
}