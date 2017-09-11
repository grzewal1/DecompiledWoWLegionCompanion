using bnet.protocol;
using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.challenge
{
	public class SendChallengeToUserRequest : IProtoBuf
	{
		public bool HasPeerId;

		private ProcessId _PeerId;

		public bool HasGameAccountId;

		private EntityId _GameAccountId;

		private List<Challenge> _Challenges = new List<Challenge>();

		public bool HasTimeout;

		private ulong _Timeout;

		private List<bnet.protocol.attribute.Attribute> _Attributes = new List<bnet.protocol.attribute.Attribute>();

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

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public ProcessId PeerId
		{
			get
			{
				return this._PeerId;
			}
			set
			{
				this._PeerId = value;
				this.HasPeerId = value != null;
			}
		}

		public ulong Timeout
		{
			get
			{
				return this._Timeout;
			}
			set
			{
				this._Timeout = value;
				this.HasTimeout = true;
			}
		}

		public SendChallengeToUserRequest()
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
			SendChallengeToUserRequest.Deserialize(stream, this);
		}

		public static SendChallengeToUserRequest Deserialize(Stream stream, SendChallengeToUserRequest instance)
		{
			return SendChallengeToUserRequest.Deserialize(stream, instance, (long)-1);
		}

		public static SendChallengeToUserRequest Deserialize(Stream stream, SendChallengeToUserRequest instance, long limit)
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
							case 37:
							{
								instance.Context = binaryReader.ReadUInt32();
								continue;
							}
							case 40:
							{
								instance.Timeout = ProtocolParser.ReadUInt64(stream);
								continue;
							}
							default:
							{
								if (num1 == 10)
								{
									if (instance.PeerId != null)
									{
										ProcessId.DeserializeLengthDelimited(stream, instance.PeerId);
									}
									else
									{
										instance.PeerId = ProcessId.DeserializeLengthDelimited(stream);
									}
									continue;
								}
								else if (num1 == 18)
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
								else if (num1 == 26)
								{
									instance.Challenges.Add(Challenge.DeserializeLengthDelimited(stream));
									continue;
								}
								else if (num1 == 50)
								{
									instance.Attributes.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
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

		public static SendChallengeToUserRequest DeserializeLengthDelimited(Stream stream)
		{
			SendChallengeToUserRequest sendChallengeToUserRequest = new SendChallengeToUserRequest();
			SendChallengeToUserRequest.DeserializeLengthDelimited(stream, sendChallengeToUserRequest);
			return sendChallengeToUserRequest;
		}

		public static SendChallengeToUserRequest DeserializeLengthDelimited(Stream stream, SendChallengeToUserRequest instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return SendChallengeToUserRequest.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			SendChallengeToUserRequest sendChallengeToUserRequest = obj as SendChallengeToUserRequest;
			if (sendChallengeToUserRequest == null)
			{
				return false;
			}
			if (this.HasPeerId != sendChallengeToUserRequest.HasPeerId || this.HasPeerId && !this.PeerId.Equals(sendChallengeToUserRequest.PeerId))
			{
				return false;
			}
			if (this.HasGameAccountId != sendChallengeToUserRequest.HasGameAccountId || this.HasGameAccountId && !this.GameAccountId.Equals(sendChallengeToUserRequest.GameAccountId))
			{
				return false;
			}
			if (this.Challenges.Count != sendChallengeToUserRequest.Challenges.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Challenges.Count; i++)
			{
				if (!this.Challenges[i].Equals(sendChallengeToUserRequest.Challenges[i]))
				{
					return false;
				}
			}
			if (!this.Context.Equals(sendChallengeToUserRequest.Context))
			{
				return false;
			}
			if (this.HasTimeout != sendChallengeToUserRequest.HasTimeout || this.HasTimeout && !this.Timeout.Equals(sendChallengeToUserRequest.Timeout))
			{
				return false;
			}
			if (this.Attributes.Count != sendChallengeToUserRequest.Attributes.Count)
			{
				return false;
			}
			for (int j = 0; j < this.Attributes.Count; j++)
			{
				if (!this.Attributes[j].Equals(sendChallengeToUserRequest.Attributes[j]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasPeerId)
			{
				hashCode ^= this.PeerId.GetHashCode();
			}
			if (this.HasGameAccountId)
			{
				hashCode ^= this.GameAccountId.GetHashCode();
			}
			foreach (Challenge challenge in this.Challenges)
			{
				hashCode ^= challenge.GetHashCode();
			}
			hashCode ^= this.Context.GetHashCode();
			if (this.HasTimeout)
			{
				hashCode ^= this.Timeout.GetHashCode();
			}
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attributes)
			{
				hashCode ^= attribute.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasPeerId)
			{
				num++;
				uint serializedSize = this.PeerId.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.HasGameAccountId)
			{
				num++;
				uint serializedSize1 = this.GameAccountId.GetSerializedSize();
				num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
			}
			if (this.Challenges.Count > 0)
			{
				foreach (Challenge challenge in this.Challenges)
				{
					num++;
					uint num1 = challenge.GetSerializedSize();
					num = num + num1 + ProtocolParser.SizeOfUInt32(num1);
				}
			}
			num += 4;
			if (this.HasTimeout)
			{
				num++;
				num += ProtocolParser.SizeOfUInt64(this.Timeout);
			}
			if (this.Attributes.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attributes)
				{
					num++;
					uint serializedSize2 = attribute.GetSerializedSize();
					num = num + serializedSize2 + ProtocolParser.SizeOfUInt32(serializedSize2);
				}
			}
			num++;
			return num;
		}

		public static SendChallengeToUserRequest ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<SendChallengeToUserRequest>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			SendChallengeToUserRequest.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, SendChallengeToUserRequest instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			if (instance.HasPeerId)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.PeerId.GetSerializedSize());
				ProcessId.Serialize(stream, instance.PeerId);
			}
			if (instance.HasGameAccountId)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteUInt32(stream, instance.GameAccountId.GetSerializedSize());
				EntityId.Serialize(stream, instance.GameAccountId);
			}
			if (instance.Challenges.Count > 0)
			{
				foreach (Challenge challenge in instance.Challenges)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteUInt32(stream, challenge.GetSerializedSize());
					Challenge.Serialize(stream, challenge);
				}
			}
			stream.WriteByte(37);
			binaryWriter.Write(instance.Context);
			if (instance.HasTimeout)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt64(stream, instance.Timeout);
			}
			if (instance.Attributes.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attributes)
				{
					stream.WriteByte(50);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
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

		public void SetGameAccountId(EntityId val)
		{
			this.GameAccountId = val;
		}

		public void SetPeerId(ProcessId val)
		{
			this.PeerId = val;
		}

		public void SetTimeout(ulong val)
		{
			this.Timeout = val;
		}
	}
}