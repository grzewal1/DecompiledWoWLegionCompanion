using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.channel_invitation
{
	public class IncrementChannelCountResponse : IProtoBuf
	{
		private List<ulong> _ReservationTokens = new List<ulong>();

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<ulong> ReservationTokens
		{
			get
			{
				return this._ReservationTokens;
			}
			set
			{
				this._ReservationTokens = value;
			}
		}

		public int ReservationTokensCount
		{
			get
			{
				return this._ReservationTokens.Count;
			}
		}

		public List<ulong> ReservationTokensList
		{
			get
			{
				return this._ReservationTokens;
			}
		}

		public IncrementChannelCountResponse()
		{
		}

		public void AddReservationTokens(ulong val)
		{
			this._ReservationTokens.Add(val);
		}

		public void ClearReservationTokens()
		{
			this._ReservationTokens.Clear();
		}

		public void Deserialize(Stream stream)
		{
			IncrementChannelCountResponse.Deserialize(stream, this);
		}

		public static IncrementChannelCountResponse Deserialize(Stream stream, IncrementChannelCountResponse instance)
		{
			return IncrementChannelCountResponse.Deserialize(stream, instance, (long)-1);
		}

		public static IncrementChannelCountResponse Deserialize(Stream stream, IncrementChannelCountResponse instance, long limit)
		{
			if (instance.ReservationTokens == null)
			{
				instance.ReservationTokens = new List<ulong>();
			}
			while (true)
			{
				if (limit < (long)0 || stream.Position < limit)
				{
					int num = stream.ReadByte();
					if (num == -1)
					{
						if (limit >= (long)0)
						{
							throw new EndOfStreamException();
						}
						break;
					}
					else if (num == 8)
					{
						instance.ReservationTokens.Add(ProtocolParser.ReadUInt64(stream));
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
					if (stream.Position != limit)
					{
						throw new ProtocolBufferException("Read past max limit");
					}
					break;
				}
			}
			return instance;
		}

		public static IncrementChannelCountResponse DeserializeLengthDelimited(Stream stream)
		{
			IncrementChannelCountResponse incrementChannelCountResponse = new IncrementChannelCountResponse();
			IncrementChannelCountResponse.DeserializeLengthDelimited(stream, incrementChannelCountResponse);
			return incrementChannelCountResponse;
		}

		public static IncrementChannelCountResponse DeserializeLengthDelimited(Stream stream, IncrementChannelCountResponse instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return IncrementChannelCountResponse.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			IncrementChannelCountResponse incrementChannelCountResponse = obj as IncrementChannelCountResponse;
			if (incrementChannelCountResponse == null)
			{
				return false;
			}
			if (this.ReservationTokens.Count != incrementChannelCountResponse.ReservationTokens.Count)
			{
				return false;
			}
			for (int i = 0; i < this.ReservationTokens.Count; i++)
			{
				if (!this.ReservationTokens[i].Equals(incrementChannelCountResponse.ReservationTokens[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (ulong reservationToken in this.ReservationTokens)
			{
				hashCode ^= reservationToken.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.ReservationTokens.Count > 0)
			{
				foreach (ulong reservationToken in this.ReservationTokens)
				{
					num++;
					num += ProtocolParser.SizeOfUInt64(reservationToken);
				}
			}
			return num;
		}

		public static IncrementChannelCountResponse ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<IncrementChannelCountResponse>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			IncrementChannelCountResponse.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, IncrementChannelCountResponse instance)
		{
			if (instance.ReservationTokens.Count > 0)
			{
				foreach (ulong reservationToken in instance.ReservationTokens)
				{
					stream.WriteByte(8);
					ProtocolParser.WriteUInt64(stream, reservationToken);
				}
			}
		}

		public void SetReservationTokens(List<ulong> val)
		{
			this.ReservationTokens = val;
		}
	}
}