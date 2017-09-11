using bnet.protocol;
using bnet.protocol.attribute;
using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.game_master
{
	public class Player : IProtoBuf
	{
		public bool HasIdentity;

		private bnet.protocol.Identity _Identity;

		private List<bnet.protocol.attribute.Attribute> _Attribute = new List<bnet.protocol.attribute.Attribute>();

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

		public bnet.protocol.Identity Identity
		{
			get
			{
				return this._Identity;
			}
			set
			{
				this._Identity = value;
				this.HasIdentity = value != null;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public Player()
		{
		}

		public void AddAttribute(bnet.protocol.attribute.Attribute val)
		{
			this._Attribute.Add(val);
		}

		public void ClearAttribute()
		{
			this._Attribute.Clear();
		}

		public void Deserialize(Stream stream)
		{
			Player.Deserialize(stream, this);
		}

		public static Player Deserialize(Stream stream, Player instance)
		{
			return Player.Deserialize(stream, instance, (long)-1);
		}

		public static Player Deserialize(Stream stream, Player instance, long limit)
		{
			if (instance.Attribute == null)
			{
				instance.Attribute = new List<bnet.protocol.attribute.Attribute>();
			}
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num != -1)
					{
						int num1 = num;
						if (num1 != 10)
						{
							if (num1 == 18)
							{
								instance.Attribute.Add(bnet.protocol.attribute.Attribute.DeserializeLengthDelimited(stream));
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
						else if (instance.Identity != null)
						{
							bnet.protocol.Identity.DeserializeLengthDelimited(stream, instance.Identity);
						}
						else
						{
							instance.Identity = bnet.protocol.Identity.DeserializeLengthDelimited(stream);
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

		public static Player DeserializeLengthDelimited(Stream stream)
		{
			Player player = new Player();
			Player.DeserializeLengthDelimited(stream, player);
			return player;
		}

		public static Player DeserializeLengthDelimited(Stream stream, Player instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return Player.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Player player = obj as Player;
			if (player == null)
			{
				return false;
			}
			if (this.HasIdentity != player.HasIdentity || this.HasIdentity && !this.Identity.Equals(player.Identity))
			{
				return false;
			}
			if (this.Attribute.Count != player.Attribute.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Attribute.Count; i++)
			{
				if (!this.Attribute[i].Equals(player.Attribute[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasIdentity)
			{
				hashCode ^= this.Identity.GetHashCode();
			}
			foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
			{
				hashCode ^= attribute.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasIdentity)
			{
				num++;
				uint serializedSize = this.Identity.GetSerializedSize();
				num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			}
			if (this.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in this.Attribute)
				{
					num++;
					uint serializedSize1 = attribute.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			return num;
		}

		public static Player ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Player>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Player.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Player instance)
		{
			if (instance.HasIdentity)
			{
				stream.WriteByte(10);
				ProtocolParser.WriteUInt32(stream, instance.Identity.GetSerializedSize());
				bnet.protocol.Identity.Serialize(stream, instance.Identity);
			}
			if (instance.Attribute.Count > 0)
			{
				foreach (bnet.protocol.attribute.Attribute attribute in instance.Attribute)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, attribute.GetSerializedSize());
					bnet.protocol.attribute.Attribute.Serialize(stream, attribute);
				}
			}
		}

		public void SetAttribute(List<bnet.protocol.attribute.Attribute> val)
		{
			this.Attribute = val;
		}

		public void SetIdentity(bnet.protocol.Identity val)
		{
			this.Identity = val;
		}
	}
}