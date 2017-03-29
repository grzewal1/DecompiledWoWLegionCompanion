using bnet.protocol;
using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.challenge
{
	public class ChallengeUserRequest : IProtoBuf
	{
		private List<Challenge> _Challenges = new List<Challenge>();

		public bool HasId;

		private uint _Id;

		public bool HasDeadline;

		private ulong _Deadline;

		private List<bnet.protocol.attribute.Attribute> _Attributes = new List<bnet.protocol.attribute.Attribute>();

		public bool HasGameAccountId;

		private EntityId _GameAccountId;

		public List<bnet.protocol.attribute.Attribute> Attributes
		{
			get
			{
				return this._Attributes;
			}
			set
			{
				this._Attributes = value;
			}
		}

		public int AttributesCount
		{
			get
			{
				return this._Attributes.Count;
			}
		}

		public List<bnet.protocol.attribute.Attribute> AttributesList
		{
			get
			{
				return this._Attributes;
			}
		}

		public List<Challenge> Challenges
		{
			get
			{
				return this._Challenges;
			}
			set
			{
				this._Challenges = value;
			}
		}

		public int ChallengesCount
		{
			get
			{
				return this._Challenges.Count;
			}
		}

		public List<Challenge> ChallengesList
		{
			get
			{
				return this._Challenges;
			}
		}

		public uint Context
		{
			get;
			set;
		}

		public ulong Deadline
		{
			get
			{
				return this._Deadline;
			}
			set
			{
				this._Deadline = value;
				this.HasDeadline = true;
			}
		}

		public EntityId GameAccountId
		{
			get
			{
				return this._GameAccountId;
			}
			set
			{
				this._GameAccountId = value;
				this.HasGameAccountId = value != null;
			}
		}

		public uint Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				this._Id = value;
				this.HasId = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ChallengeUserRequest()
		{
		}

		public void AddAttributes(bnet.protocol.attribute.Attribute val)
		{
			this._Attributes.Add(val);
		}

		public void AddChallenges(Challenge val)
		{
			this._Challenges.Add(val);
		}

		public void ClearAttributes()
		{
			this._Attributes.Clear();
		}

		public void ClearChallenges()
		{
			this._Challenges.Clear();
		}

		public void Deserialize(Stream stream)
		{
			ChallengeUserRequest.Deserialize(stream, this);
		}

		public static ChallengeUserRequest Deserialize(Stream stream, ChallengeUserRequest instance)
		{
			return ChallengeUserRequest.Deserialize(stream, instance, (long)-1);
		}

		public static ChallengeUserRequest Deserialize(Stream stream, ChallengeUserRequest instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.Challenges == null)
			{
				instance.Challenges = new List<Challenge>();
			}
			if (instance.Attributes == null)
			{
				instance.Attributes = new List<bnet.protocol.attribute.Attribute>();
			}
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
								instance.Context = binaryReader.ReadUInt32();
								continue;
							}
							case 24:
							{
								instance.Id = ProtocolParser.ReadUInt32(stream);
								continue;
							}
							default:
							{
								if (num1 == 10)
								{
									instance.Challenges.Add(Challenge.DeserializeLengthDelimited(stream));
									continue;
								}
								else if (num1 == 32)
								{
									instance.Deadline = ProtocolParser.ReadUInt64(stream);
									continue;
								}
								else if (num1 == 42)
								{
									instance.Attributes.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
									continue;
								}
								else if (num1 == 50)
								{
									if (instance.GameAccountId != null)
									{
										EntityId.DeserializeLengthDelimited(stream, instance.GameAccountId);
									}
									else
									{
										instance.GameAccountId = EntityId.DeserializeLengthDelimited(stream);
									}
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

		public static ChallengeUserRequest DeserializeLengthDelimited(Stream stream)
		{
			ChallengeUserRequest challengeUserRequest = new ChallengeUserRequest();
			ChallengeUserRequest.DeserializeLengthDelimited(stream, challengeUserRequest);
			return challengeUserRequest;
		}

		public static ChallengeUserRequest DeserializeLengthDelimited(Stream stream, ChallengeUserRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return ChallengeUserRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChallengeUserRequest challengeUserRequest = obj as ChallengeUserRequest;
			if (challengeUserRequest == null)
			{
				return false;
			}
			if (this.Challenges.Count != challengeUserRequest.Challenges.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Challenges.Count; i++)
			{
				if (!this.Challenges[i].Equals(challengeUserRequest.Challenges[i]))
				{
					return false;
				}
			}
			if (!this.Context.Equals(challengeUserRequest.Context))
			{
				return false;
			}
			if (this.HasId != challengeUserRequest.HasId || this.HasId && !this.Id.Equals(challengeUserRequest.Id))
			{
				return false;
			}
			if (this.HasDeadline != challengeUserRequest.HasDeadline || this.HasDeadline && !this.Deadline.Equals(challengeUserRequest.Deadline))
			{
				return false;
			}
			if (this.Attributes.Count != challengeUserRequest.Attributes.Count)
			{
				return false;
			}
			for (int j = 0; j < this.Attributes.Count; j++)
			{
				if (!this.Attributes[j].Equals(challengeUserRequest.Attributes[j]))
				{
					return false;
				}
			}
			if (this.HasGameAccountId == challengeUserRequest.HasGameAccountId && (!this.HasGameAccountId || this.GameAccountId.Equals(challengeUserRequest.GameAccountId)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (Challenge challenge in this.Challenges)
			{
				hashCode = hashCode ^ challenge.GetHashCode();
			}
			hashCode = hashCode ^ this.Context.GetHashCode();
			if (this.HasId)
			{
				hashCode = hashCode ^ this.Id.GetHashCode();
			}
			if (this.HasDeadline)
			{
				hashCode = hashCode ^ this.Deadline.GetHashCode();
			}
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attributes)
			{
				hashCode = hashCode ^ attribute.GetHashCode();
			}
			if (this.HasGameAccountId)
			{
				hashCode = hashCode ^ this.GameAccountId.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Challenges.Count > 0)
			{
				foreach (Challenge challenge in this.Challenges)
				{
					num++;
					uint serializedSize = challenge.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			num = num + 4;
			if (this.HasId)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt32(this.Id);
			}
			if (this.HasDeadline)
			{
				num++;
				num = num + ProtocolParser.SizeOfUInt64(this.Deadline);
			}
			if (this.Attributes.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attributes)
				{
					num++;
					uint serializedSize1 = attribute.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			if (this.HasGameAccountId)
			{
				num++;
				uint num1 = this.GameAccountId.GetSerializedSize();
				num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
			}
			num++;
			return num;
		}

		public static ChallengeUserRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChallengeUserRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChallengeUserRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChallengeUserRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.Challenges.Count > 0)
			{
				foreach (Challenge challenge in instance.Challenges)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, challenge.GetSerializedSize());
					Challenge.Serialize(stream, challenge);
				}
			}
			stream.WriteByte(21);
			binaryWriter.Write(instance.Context);
			if (instance.HasId)
			{
				stream.WriteByte(24);
				ProtocolParser.WriteUInt32(stream, instance.Id);
			}
			if (instance.HasDeadline)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt64(stream, instance.Deadline);
			}
			if (instance.Attributes.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attributes)
				{
					stream.WriteByte(42);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
			if (instance.HasGameAccountId)
			{
				stream.WriteByte(50);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.GameAccountId);
			}
		}

		public void SetAttributes(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attributes = val;
		}

		public void SetChallenges(List<Challenge> val)
		{
			this.Challenges = val;
		}

		public void SetContext(uint val)
		{
			this.Context = val;
		}

		public void SetDeadline(ulong val)
		{
			this.Deadline = val;
		}

		public void SetGameAccountId(EntityId val)
		{
			this.GameAccountId = val;
		}

		public void SetId(uint val)
		{
			this.Id = val;
		}
	}
}