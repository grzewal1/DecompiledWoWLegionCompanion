using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace bnet.protocol.game_master
{
	public class GameFactoryDescription : IProtoBuf
	{
		public bool HasName;

		private string _Name;

		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

		private List<GameStatsBucket> _StatsBucket = new List<GameStatsBucket>();

		public bool HasUnseededId;

		private ulong _UnseededId;

		public bool HasAllowQueueing;

		private bool _AllowQueueing;

		public bool AllowQueueing
		{
			get
			{
				return this._AllowQueueing;
			}
			set
			{
				this._AllowQueueing = value;
				this.HasAllowQueueing = true;
			}
		}

		public List<bnet.protocol.attribute.Attribute> Attribute
		{
			get
			{
				return this._Attribute;
			}
			set
			{
				this._Attribute = value;
			}
		}

		public int AttributeCount
		{
			get
			{
				return this._Attribute.Count;
			}
		}

		public List<bnet.protocol.attribute.Attribute> AttributeList
		{
			get
			{
				return this._Attribute;
			}
		}

		public ulong Id
		{
			get;
			set;
		}

		public bool IsInitialized
		{
			get
			{
				return true;
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

		public List<GameStatsBucket> StatsBucket
		{
			get
			{
				return this._StatsBucket;
			}
			set
			{
				this._StatsBucket = value;
			}
		}

		public int StatsBucketCount
		{
			get
			{
				return this._StatsBucket.Count;
			}
		}

		public List<GameStatsBucket> StatsBucketList
		{
			get
			{
				return this._StatsBucket;
			}
		}

		public ulong UnseededId
		{
			get
			{
				return this._UnseededId;
			}
			set
			{
				this._UnseededId = value;
				this.HasUnseededId = true;
			}
		}

		public GameFactoryDescription()
		{
		}

		public void AddAttribute(bnet.protocol.attribute.Attribute val)
		{
			this._Attribute.Add(val);
		}

		public void AddStatsBucket(GameStatsBucket val)
		{
			this._StatsBucket.Add(val);
		}

		public void ClearAttribute()
		{
			this._Attribute.Clear();
		}

		public void ClearStatsBucket()
		{
			this._StatsBucket.Clear();
		}

		public void Deserialize(Stream stream)
		{
			GameFactoryDescription.Deserialize(stream, this);
		}

		public static GameFactoryDescription Deserialize(Stream stream, GameFactoryDescription instance)
		{
			return GameFactoryDescription.Deserialize(stream, instance, (long)-1);
		}

		public static GameFactoryDescription Deserialize(Stream stream, GameFactoryDescription instance, long limit)
		{
			BinaryReader binaryReader = new BinaryReader(stream);
			if (instance.Attribute == null)
			{
				instance.Attribute = new List<bnet.protocol.attribute.Attribute>();
			}
			if (instance.StatsBucket == null)
			{
				instance.StatsBucket = new List<GameStatsBucket>();
			}
			instance.UnseededId = (ulong)0;
			instance.AllowQueueing = true;
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 == 9)
						{
							instance.Id = binaryReader.ReadUInt64();
						}
						else if (num1 == 18)
						{
							instance.Name = ProtocolParser.ReadString(stream);
						}
						else if (num1 == 26)
						{
							instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 34)
						{
							instance.StatsBucket.Add(GameStatsBucket.DeserializeLengthDelimited(stream));
						}
						else if (num1 == 41)
						{
							instance.UnseededId = binaryReader.ReadUInt64();
						}
						else if (num1 == 48)
						{
							instance.AllowQueueing = ProtocolParser.ReadBool(stream);
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

		public static GameFactoryDescription DeserializeLengthDelimited(Stream stream)
		{
			GameFactoryDescription gameFactoryDescription = new GameFactoryDescription();
			GameFactoryDescription.DeserializeLengthDelimited(stream, gameFactoryDescription);
			return gameFactoryDescription;
		}

		public static GameFactoryDescription DeserializeLengthDelimited(Stream stream, GameFactoryDescription instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return GameFactoryDescription.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			GameFactoryDescription gameFactoryDescription = obj as GameFactoryDescription;
			if (gameFactoryDescription == null)
			{
				return false;
			}
			if (!this.Id.Equals(gameFactoryDescription.Id))
			{
				return false;
			}
			if (this.HasName != gameFactoryDescription.HasName || this.HasName && !this.Name.Equals(gameFactoryDescription.Name))
			{
				return false;
			}
			if (this.Attribute.Count != gameFactoryDescription.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(gameFactoryDescription.Attribute[i]))
				{
					return false;
				}
			}
			if (this.StatsBucket.Count != gameFactoryDescription.StatsBucket.Count)
			{
				return false;
			}
			for (int j = 0; j < this.StatsBucket.Count; j++)
			{
				if (!this.StatsBucket[j].Equals(gameFactoryDescription.StatsBucket[j]))
				{
					return false;
				}
			}
			if (this.HasUnseededId != gameFactoryDescription.HasUnseededId || this.HasUnseededId && !this.UnseededId.Equals(gameFactoryDescription.UnseededId))
			{
				return false;
			}
			if (this.HasAllowQueueing == gameFactoryDescription.HasAllowQueueing && (!this.HasAllowQueueing || this.AllowQueueing.Equals(gameFactoryDescription.AllowQueueing)))
			{
				return true;
			}
			return false;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode = hashCode ^ this.Id.GetHashCode();
			if (this.HasName)
			{
				hashCode = hashCode ^ this.Name.GetHashCode();
			}
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
			{
				hashCode = hashCode ^ attribute.GetHashCode();
			}
			foreach (GameStatsBucket statsBucket in this.StatsBucket)
			{
				hashCode = hashCode ^ statsBucket.GetHashCode();
			}
			if (this.HasUnseededId)
			{
				hashCode = hashCode ^ this.UnseededId.GetHashCode();
			}
			if (this.HasAllowQueueing)
			{
				hashCode = hashCode ^ this.AllowQueueing.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			num = num + 8;
			if (this.HasName)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Name);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
				{
					num++;
					uint serializedSize = attribute.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			if (this.StatsBucket.Count > 0)
			{
				foreach (GameStatsBucket statsBucket in this.StatsBucket)
				{
					num++;
					uint serializedSize1 = statsBucket.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			if (this.HasUnseededId)
			{
				num++;
				num = num + 8;
			}
			if (this.HasAllowQueueing)
			{
				num++;
				num++;
			}
			num++;
			return num;
		}

		public static GameFactoryDescription ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<GameFactoryDescription>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			GameFactoryDescription.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, GameFactoryDescription instance)
		{
			BinaryWriter binaryWriter = new BinaryWriter(stream);
			stream.WriteByte(9);
			binaryWriter.Write(instance.Id);
			if (instance.HasName)
			{
				stream.WriteByte(18);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Name));
			}
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(26);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
			if (instance.StatsBucket.Count > 0)
			{
				foreach (GameStatsBucket statsBucket in instance.StatsBucket)
				{
					stream.WriteByte(34);
					ProtocolParser.WriteUInt32(stream, statsBucket.GetSerializedSize());
					GameStatsBucket.Serialize(stream, statsBucket);
				}
			}
			if (instance.HasUnseededId)
			{
				stream.WriteByte(41);
				binaryWriter.Write(instance.UnseededId);
			}
			if (instance.HasAllowQueueing)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteBool(stream, instance.AllowQueueing);
			}
		}

		public void SetAllowQueueing(bool val)
		{
			this.AllowQueueing = val;
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetId(ulong val)
		{
			this.Id = val;
		}

		public void SetName(string val)
		{
			this.Name = val;
		}

		public void SetStatsBucket(List<GameStatsBucket> val)
		{
			this.StatsBucket = val;
		}

		public void SetUnseededId(ulong val)
		{
			this.UnseededId = val;
		}
	}
}