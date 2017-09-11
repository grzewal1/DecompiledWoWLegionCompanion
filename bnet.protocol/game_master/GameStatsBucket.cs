using System;
using System.IO;

namespace bnet.protocol.game_master
{
	public class GameStatsBucket : IProtoBuf
	{
		public bool HasBucketMin;

		private float _BucketMin;

		public bool HasBucketMax;

		private float _BucketMax;

		public bool HasWaitMilliseconds;

		private uint _WaitMilliseconds;

		public bool HasGamesPerHour;

		private uint _GamesPerHour;

		public bool HasActiveGames;

		private uint _ActiveGames;

		public bool HasActivePlayers;

		private uint _ActivePlayers;

		public bool HasFormingGames;

		private uint _FormingGames;

		public bool HasWaitingPlayers;

		private uint _WaitingPlayers;

		public bool HasOpenJoinableGames;

		private uint _OpenJoinableGames;

		public bool HasPlayersInOpenJoinableGames;

		private uint _PlayersInOpenJoinableGames;

		public bool HasOpenGamesTotal;

		private uint _OpenGamesTotal;

		public bool HasPlayersInOpenGamesTotal;

		private uint _PlayersInOpenGamesTotal;

		public uint ActiveGames
		{
			get
			{
				return this._ActiveGames;
			}
			set
			{
				this._ActiveGames = value;
				this.HasActiveGames = true;
			}
		}

		public uint ActivePlayers
		{
			get
			{
				return this._ActivePlayers;
			}
			set
			{
				this._ActivePlayers = value;
				this.HasActivePlayers = true;
			}
		}

		public float BucketMax
		{
			get
			{
				return this._BucketMax;
			}
			set
			{
				this._BucketMax = value;
				this.HasBucketMax = true;
			}
		}

		public float BucketMin
		{
			get
			{
				return this._BucketMin;
			}
			set
			{
				this._BucketMin = value;
				this.HasBucketMin = true;
			}
		}

		public uint FormingGames
		{
			get
			{
				return this._FormingGames;
			}
			set
			{
				this._FormingGames = value;
				this.HasFormingGames = true;
			}
		}

		public uint GamesPerHour
		{
			get
			{
				return this._GamesPerHour;
			}
			set
			{
				this._GamesPerHour = value;
				this.HasGamesPerHour = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint OpenGamesTotal
		{
			get
			{
				return this._OpenGamesTotal;
			}
			set
			{
				this._OpenGamesTotal = value;
				this.HasOpenGamesTotal = true;
			}
		}

		public uint OpenJoinableGames
		{
			get
			{
				return this._OpenJoinableGames;
			}
			set
			{
				this._OpenJoinableGames = value;
				this.HasOpenJoinableGames = true;
			}
		}

		public uint PlayersInOpenGamesTotal
		{
			get
			{
				return this._PlayersInOpenGamesTotal;
			}
			set
			{
				this._PlayersInOpenGamesTotal = value;
				this.HasPlayersInOpenGamesTotal = true;
			}
		}

		public uint PlayersInOpenJoinableGames
		{
			get
			{
				return this._PlayersInOpenJoinableGames;
			}
			set
			{
				this._PlayersInOpenJoinableGames = value;
				this.HasPlayersInOpenJoinableGames = true;
			}
		}

		public uint WaitingPlayers
		{
			get
			{
				return this._WaitingPlayers;
			}
			set
			{
				this._WaitingPlayers = value;
				this.HasWaitingPlayers = true;
			}
		}

		public uint WaitMilliseconds
		{
			get
			{
				return this._WaitMilliseconds;
			}
			set
			{
				this._WaitMilliseconds = value;
				this.HasWaitMilliseconds = true;
			}
		}

		public GameStatsBucket()
		{
		}

		public void Deserialize(Stream stream)
		{
			GameStatsBucket.Deserialize(stream, this);
		}

		public static GameStatsBucket Deserialize(Stream stream, GameStatsBucket instance)
		{
			return GameStatsBucket.Deserialize(stream, instance, (long)-1);
		}

		public static GameStatsBucket Deserialize(Stream stream, GameStatsBucket instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			instance.BucketMin = 0f;
			instance.BucketMax = 4.2949673E+09f;
			instance.WaitMilliseconds = 0;
			instance.GamesPerHour = 0;
			instance.ActiveGames = 0;
			instance.ActivePlayers = 0;
			instance.FormingGames = 0;
			instance.WaitingPlayers = 0;
			instance.OpenJoinableGames = 0;
			instance.PlayersInOpenJoinableGames = 0;
			instance.OpenGamesTotal = 0;
			instance.PlayersInOpenGamesTotal = 0;
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
							case 21:
							{
								instance.BucketMax = binaryReader.ReadSingle();
								continue;
							}
							case 24:
							{
								instance.WaitMilliseconds = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							default:
							{
								if (num1 == 13)
								{
									instance.BucketMin = binaryReader.ReadSingle();
									continue;
								}
								else if (num1 == 32)
								{
									instance.GamesPerHour = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num1 == 40)
								{
									instance.ActiveGames = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num1 == 48)
								{
									instance.ActivePlayers = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num1 == 56)
								{
									instance.FormingGames = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num1 == 64)
								{
									instance.WaitingPlayers = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num1 == 72)
								{
									instance.OpenJoinableGames = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num1 == 80)
								{
									instance.PlayersInOpenJoinableGames = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num1 == 88)
								{
									instance.OpenGamesTotal = ProtocolParser.ReadUInt32(stream);
									continue;
								}
								else if (num1 == 96)
								{
									instance.PlayersInOpenGamesTotal = ProtocolParser.ReadUInt32(stream);
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

		public static GameStatsBucket DeserializeLengthDelimited(Stream stream)
		{
			GameStatsBucket gameStatsBucket = new GameStatsBucket();
			GameStatsBucket.DeserializeLengthDelimited(stream, gameStatsBucket);
			return gameStatsBucket;
		}

		public static GameStatsBucket DeserializeLengthDelimited(Stream stream, GameStatsBucket instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return GameStatsBucket.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameStatsBucket gameStatsBucket = obj as GameStatsBucket;
			if (gameStatsBucket == null)
			{
				return false;
			}
			if (this.HasBucketMin != gameStatsBucket.HasBucketMin || this.HasBucketMin && !this.BucketMin.Equals(gameStatsBucket.BucketMin))
			{
				return false;
			}
			if (this.HasBucketMax != gameStatsBucket.HasBucketMax || this.HasBucketMax && !this.BucketMax.Equals(gameStatsBucket.BucketMax))
			{
				return false;
			}
			if (this.HasWaitMilliseconds != gameStatsBucket.HasWaitMilliseconds || this.HasWaitMilliseconds && !this.WaitMilliseconds.Equals(gameStatsBucket.WaitMilliseconds))
			{
				return false;
			}
			if (this.HasGamesPerHour != gameStatsBucket.HasGamesPerHour || this.HasGamesPerHour && !this.GamesPerHour.Equals(gameStatsBucket.GamesPerHour))
			{
				return false;
			}
			if (this.HasActiveGames != gameStatsBucket.HasActiveGames || this.HasActiveGames && !this.ActiveGames.Equals(gameStatsBucket.ActiveGames))
			{
				return false;
			}
			if (this.HasActivePlayers != gameStatsBucket.HasActivePlayers || this.HasActivePlayers && !this.ActivePlayers.Equals(gameStatsBucket.ActivePlayers))
			{
				return false;
			}
			if (this.HasFormingGames != gameStatsBucket.HasFormingGames || this.HasFormingGames && !this.FormingGames.Equals(gameStatsBucket.FormingGames))
			{
				return false;
			}
			if (this.HasWaitingPlayers != gameStatsBucket.HasWaitingPlayers || this.HasWaitingPlayers && !this.WaitingPlayers.Equals(gameStatsBucket.WaitingPlayers))
			{
				return false;
			}
			if (this.HasOpenJoinableGames != gameStatsBucket.HasOpenJoinableGames || this.HasOpenJoinableGames && !this.OpenJoinableGames.Equals(gameStatsBucket.OpenJoinableGames))
			{
				return false;
			}
			if (this.HasPlayersInOpenJoinableGames != gameStatsBucket.HasPlayersInOpenJoinableGames || this.HasPlayersInOpenJoinableGames && !this.PlayersInOpenJoinableGames.Equals(gameStatsBucket.PlayersInOpenJoinableGames))
			{
				return false;
			}
			if (this.HasOpenGamesTotal != gameStatsBucket.HasOpenGamesTotal || this.HasOpenGamesTotal && !this.OpenGamesTotal.Equals(gameStatsBucket.OpenGamesTotal))
			{
				return false;
			}
			if (this.HasPlayersInOpenGamesTotal == gameStatsBucket.HasPlayersInOpenGamesTotal && (!this.HasPlayersInOpenGamesTotal || this.PlayersInOpenGamesTotal.Equals(gameStatsBucket.PlayersInOpenGamesTotal)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasBucketMin)
			{
				hashCode ^= this.BucketMin.GetHashCode();
			}
			if (this.HasBucketMax)
			{
				hashCode ^= this.BucketMax.GetHashCode();
			}
			if (this.HasWaitMilliseconds)
			{
				hashCode ^= this.WaitMilliseconds.GetHashCode();
			}
			if (this.HasGamesPerHour)
			{
				hashCode ^= this.GamesPerHour.GetHashCode();
			}
			if (this.HasActiveGames)
			{
				hashCode ^= this.ActiveGames.GetHashCode();
			}
			if (this.HasActivePlayers)
			{
				hashCode ^= this.ActivePlayers.GetHashCode();
			}
			if (this.HasFormingGames)
			{
				hashCode ^= this.FormingGames.GetHashCode();
			}
			if (this.HasWaitingPlayers)
			{
				hashCode ^= this.WaitingPlayers.GetHashCode();
			}
			if (this.HasOpenJoinableGames)
			{
				hashCode ^= this.OpenJoinableGames.GetHashCode();
			}
			if (this.HasPlayersInOpenJoinableGames)
			{
				hashCode ^= this.PlayersInOpenJoinableGames.GetHashCode();
			}
			if (this.HasOpenGamesTotal)
			{
				hashCode ^= this.OpenGamesTotal.GetHashCode();
			}
			if (this.HasPlayersInOpenGamesTotal)
			{
				hashCode ^= this.PlayersInOpenGamesTotal.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasBucketMin)
			{
				num++;
				num += 4;
			}
			if (this.HasBucketMax)
			{
				num++;
				num += 4;
			}
			if (this.HasWaitMilliseconds)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.WaitMilliseconds);
			}
			if (this.HasGamesPerHour)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.GamesPerHour);
			}
			if (this.HasActiveGames)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.ActiveGames);
			}
			if (this.HasActivePlayers)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.ActivePlayers);
			}
			if (this.HasFormingGames)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.FormingGames);
			}
			if (this.HasWaitingPlayers)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.WaitingPlayers);
			}
			if (this.HasOpenJoinableGames)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.OpenJoinableGames);
			}
			if (this.HasPlayersInOpenJoinableGames)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.PlayersInOpenJoinableGames);
			}
			if (this.HasOpenGamesTotal)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.OpenGamesTotal);
			}
			if (this.HasPlayersInOpenGamesTotal)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.PlayersInOpenGamesTotal);
			}
			return num;
		}

		public static GameStatsBucket ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameStatsBucket>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameStatsBucket.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameStatsBucket instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasBucketMin)
			{
				stream.WriteByte(13);
				binaryWriter.Write(instance.BucketMin);
			}
			if (instance.HasBucketMax)
			{
				stream.WriteByte(21);
				binaryWriter.Write(instance.BucketMax);
			}
			if (instance.HasWaitMilliseconds)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.WaitMilliseconds);
			}
			if (instance.HasGamesPerHour)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt32(stream, instance.GamesPerHour);
			}
			if (instance.HasActiveGames)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt32(stream, instance.ActiveGames);
			}
			if (instance.HasActivePlayers)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteUInt32(stream, instance.ActivePlayers);
			}
			if (instance.HasFormingGames)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteUInt32(stream, instance.FormingGames);
			}
			if (instance.HasWaitingPlayers)
			{
				stream.WriteByte(64);
				ProtocolParser.WriteUInt32(stream, instance.WaitingPlayers);
			}
			if (instance.HasOpenJoinableGames)
			{
				stream.WriteByte(72);
				ProtocolParser.WriteUInt32(stream, instance.OpenJoinableGames);
			}
			if (instance.HasPlayersInOpenJoinableGames)
			{
				stream.WriteByte(80);
				ProtocolParser.WriteUInt32(stream, instance.PlayersInOpenJoinableGames);
			}
			if (instance.HasOpenGamesTotal)
			{
				stream.WriteByte(88);
				ProtocolParser.WriteUInt32(stream, instance.OpenGamesTotal);
			}
			if (instance.HasPlayersInOpenGamesTotal)
			{
				stream.WriteByte(96);
				ProtocolParser.WriteUInt32(stream, instance.PlayersInOpenGamesTotal);
			}
		}

		public void SetActiveGames(uint val)
		{
			this.ActiveGames = val;
		}

		public void SetActivePlayers(uint val)
		{
			this.ActivePlayers = val;
		}

		public void SetBucketMax(float val)
		{
			this.BucketMax = val;
		}

		public void SetBucketMin(float val)
		{
			this.BucketMin = val;
		}

		public void SetFormingGames(uint val)
		{
			this.FormingGames = val;
		}

		public void SetGamesPerHour(uint val)
		{
			this.GamesPerHour = val;
		}

		public void SetOpenGamesTotal(uint val)
		{
			this.OpenGamesTotal = val;
		}

		public void SetOpenJoinableGames(uint val)
		{
			this.OpenJoinableGames = val;
		}

		public void SetPlayersInOpenGamesTotal(uint val)
		{
			this.PlayersInOpenGamesTotal = val;
		}

		public void SetPlayersInOpenJoinableGames(uint val)
		{
			this.PlayersInOpenJoinableGames = val;
		}

		public void SetWaitingPlayers(uint val)
		{
			this.WaitingPlayers = val;
		}

		public void SetWaitMilliseconds(uint val)
		{
			this.WaitMilliseconds = val;
		}
	}
}