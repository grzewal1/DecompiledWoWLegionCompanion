using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace bnet.protocol.channel
{
	public class ChannelInfo : IProtoBuf
	{
		private List<bnet.protocol.channel.Member> _Member = new List<bnet.protocol.channel.Member>();

		public ChannelDescription Description
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

		public List<bnet.protocol.channel.Member> Member
		{
			get
			{
				return this._Member;
			}
			set
			{
				this._Member = value;
			}
		}

		public int MemberCount
		{
			get
			{
				return this._Member.Count;
			}
		}

		public List<bnet.protocol.channel.Member> MemberList
		{
			get
			{
				return this._Member;
			}
		}

		public ChannelInfo()
		{
		}

		public void AddMember(bnet.protocol.channel.Member val)
		{
			this._Member.Add(val);
		}

		public void ClearMember()
		{
			this._Member.Clear();
		}

		public void Deserialize(Stream stream)
		{
			ChannelInfo.Deserialize(stream, this);
		}

		public static ChannelInfo Deserialize(Stream stream, ChannelInfo instance)
		{
			return ChannelInfo.Deserialize(stream, instance, (long)-1);
		}

		public static ChannelInfo Deserialize(Stream stream, ChannelInfo instance, long limit)
		{
			if (instance.Member == null)
			{
				instance.Member = new List<bnet.protocol.channel.Member>();
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
								instance.Member.Add(bnet.protocol.channel.Member.DeserializeLengthDelimited(stream));
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
						else if (instance.Description != null)
						{
							ChannelDescription.DeserializeLengthDelimited(stream, instance.Description);
						}
						else
						{
							instance.Description = ChannelDescription.DeserializeLengthDelimited(stream);
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

		public static ChannelInfo DeserializeLengthDelimited(Stream stream)
		{
			ChannelInfo channelInfo = new ChannelInfo();
			ChannelInfo.DeserializeLengthDelimited(stream, channelInfo);
			return channelInfo;
		}

		public static ChannelInfo DeserializeLengthDelimited(Stream stream, ChannelInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ChannelInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ChannelInfo channelInfo = obj as ChannelInfo;
			if (channelInfo == null)
			{
				return false;
			}
			if (!this.Description.Equals(channelInfo.Description))
			{
				return false;
			}
			if (this.Member.Count != channelInfo.Member.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Member.Count; i++)
			{
				if (!this.Member[i].Equals(channelInfo.Member[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			hashCode ^= this.Description.GetHashCode();
			foreach (bnet.protocol.channel.Member member in this.Member)
			{
				hashCode ^= member.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			uint serializedSize = this.Description.GetSerializedSize();
			num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
			if (this.Member.Count > 0)
			{
				foreach (bnet.protocol.channel.Member member in this.Member)
				{
					num++;
					uint serializedSize1 = member.GetSerializedSize();
					num = num + serializedSize1 + ProtocolParser.SizeOfUInt32(serializedSize1);
				}
			}
			num++;
			return num;
		}

		public static ChannelInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ChannelInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ChannelInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ChannelInfo instance)
		{
			if (instance.Description == null)
			{
				throw new ArgumentNullException("Description", "Required by proto specification.");
			}
			stream.WriteByte(10);
			ProtocolParser.WriteUInt32(stream, instance.Description.GetSerializedSize());
			ChannelDescription.Serialize(stream, instance.Description);
			if (instance.Member.Count > 0)
			{
				foreach (bnet.protocol.channel.Member member in instance.Member)
				{
					stream.WriteByte(18);
					ProtocolParser.WriteUInt32(stream, member.GetSerializedSize());
					bnet.protocol.channel.Member.Serialize(stream, member);
				}
			}
		}

		public void SetDescription(ChannelDescription val)
		{
			this.Description = val;
		}

		public void SetMember(List<bnet.protocol.channel.Member> val)
		{
			this.Member = val;
		}
	}
}