using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace bnet.protocol.account
{
	public class ParentalControlInfo : IProtoBuf
	{
		public bool HasTimezone;

		private string _Timezone;

		public bool HasMinutesPerDay;

		private uint _MinutesPerDay;

		public bool HasMinutesPerWeek;

		private uint _MinutesPerWeek;

		public bool HasCanReceiveVoice;

		private bool _CanReceiveVoice;

		public bool HasCanSendVoice;

		private bool _CanSendVoice;

		private List<bool> _PlaySchedule = new List<bool>();

		public bool CanReceiveVoice
		{
			get
			{
				return this._CanReceiveVoice;
			}
			set
			{
				this._CanReceiveVoice = value;
				this.HasCanReceiveVoice = true;
			}
		}

		public bool CanSendVoice
		{
			get
			{
				return this._CanSendVoice;
			}
			set
			{
				this._CanSendVoice = value;
				this.HasCanSendVoice = true;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public uint MinutesPerDay
		{
			get
			{
				return this._MinutesPerDay;
			}
			set
			{
				this._MinutesPerDay = value;
				this.HasMinutesPerDay = true;
			}
		}

		public uint MinutesPerWeek
		{
			get
			{
				return this._MinutesPerWeek;
			}
			set
			{
				this._MinutesPerWeek = value;
				this.HasMinutesPerWeek = true;
			}
		}

		public List<bool> PlaySchedule
		{
			get
			{
				return this._PlaySchedule;
			}
			set
			{
				this._PlaySchedule = value;
			}
		}

		public int PlayScheduleCount
		{
			get
			{
				return this._PlaySchedule.Count;
			}
		}

		public List<bool> PlayScheduleList
		{
			get
			{
				return this._PlaySchedule;
			}
		}

		public string Timezone
		{
			get
			{
				return this._Timezone;
			}
			set
			{
				this._Timezone = value;
				this.HasTimezone = value != null;
			}
		}

		public ParentalControlInfo()
		{
		}

		public void AddPlaySchedule(bool val)
		{
			this._PlaySchedule.Add(val);
		}

		public void ClearPlaySchedule()
		{
			this._PlaySchedule.Clear();
		}

		public void Deserialize(Stream stream)
		{
			ParentalControlInfo.Deserialize(stream, this);
		}

		public static ParentalControlInfo Deserialize(Stream stream, ParentalControlInfo instance)
		{
			return ParentalControlInfo.Deserialize(stream, instance, (long)-1);
		}

		public static ParentalControlInfo Deserialize(Stream stream, ParentalControlInfo instance, long limit)
		{
			if (instance.PlaySchedule == null)
			{
				instance.PlaySchedule = new List<bool>();
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
					else if (num == 26)
					{
						instance.Timezone = ProtocolParser.ReadString(stream);
					}
					else if (num == 32)
					{
						instance.MinutesPerDay = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 40)
					{
						instance.MinutesPerWeek = ProtocolParser.ReadUInt32(stream);
					}
					else if (num == 48)
					{
						instance.CanReceiveVoice = ProtocolParser.ReadBool(stream);
					}
					else if (num == 56)
					{
						instance.CanSendVoice = ProtocolParser.ReadBool(stream);
					}
					else if (num == 64)
					{
						instance.PlaySchedule.Add(ProtocolParser.ReadBool(stream));
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

		public static ParentalControlInfo DeserializeLengthDelimited(Stream stream)
		{
			ParentalControlInfo parentalControlInfo = new ParentalControlInfo();
			ParentalControlInfo.DeserializeLengthDelimited(stream, parentalControlInfo);
			return parentalControlInfo;
		}

		public static ParentalControlInfo DeserializeLengthDelimited(Stream stream, ParentalControlInfo instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position += stream.Position;
			return ParentalControlInfo.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			ParentalControlInfo parentalControlInfo = obj as ParentalControlInfo;
			if (parentalControlInfo == null)
			{
				return false;
			}
			if (this.HasTimezone != parentalControlInfo.HasTimezone || this.HasTimezone && !this.Timezone.Equals(parentalControlInfo.Timezone))
			{
				return false;
			}
			if (this.HasMinutesPerDay != parentalControlInfo.HasMinutesPerDay || this.HasMinutesPerDay && !this.MinutesPerDay.Equals(parentalControlInfo.MinutesPerDay))
			{
				return false;
			}
			if (this.HasMinutesPerWeek != parentalControlInfo.HasMinutesPerWeek || this.HasMinutesPerWeek && !this.MinutesPerWeek.Equals(parentalControlInfo.MinutesPerWeek))
			{
				return false;
			}
			if (this.HasCanReceiveVoice != parentalControlInfo.HasCanReceiveVoice || this.HasCanReceiveVoice && !this.CanReceiveVoice.Equals(parentalControlInfo.CanReceiveVoice))
			{
				return false;
			}
			if (this.HasCanSendVoice != parentalControlInfo.HasCanSendVoice || this.HasCanSendVoice && !this.CanSendVoice.Equals(parentalControlInfo.CanSendVoice))
			{
				return false;
			}
			if (this.PlaySchedule.Count != parentalControlInfo.PlaySchedule.Count)
			{
				return false;
			}
			for (int i = 0; i < this.PlaySchedule.Count; i++)
			{
				if (!this.PlaySchedule[i].Equals(parentalControlInfo.PlaySchedule[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			if (this.HasTimezone)
			{
				hashCode ^= this.Timezone.GetHashCode();
			}
			if (this.HasMinutesPerDay)
			{
				hashCode ^= this.MinutesPerDay.GetHashCode();
			}
			if (this.HasMinutesPerWeek)
			{
				hashCode ^= this.MinutesPerWeek.GetHashCode();
			}
			if (this.HasCanReceiveVoice)
			{
				hashCode ^= this.CanReceiveVoice.GetHashCode();
			}
			if (this.HasCanSendVoice)
			{
				hashCode ^= this.CanSendVoice.GetHashCode();
			}
			foreach (bool playSchedule in this.PlaySchedule)
			{
				hashCode ^= playSchedule.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.HasTimezone)
			{
				num++;
				uint byteCount = (uint)Encoding.UTF8.GetByteCount(this.Timezone);
				num = num + ProtocolParser.SizeOfUInt32(byteCount) + byteCount;
			}
			if (this.HasMinutesPerDay)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.MinutesPerDay);
			}
			if (this.HasMinutesPerWeek)
			{
				num++;
				num += ProtocolParser.SizeOfUInt32(this.MinutesPerWeek);
			}
			if (this.HasCanReceiveVoice)
			{
				num++;
				num++;
			}
			if (this.HasCanSendVoice)
			{
				num++;
				num++;
			}
			if (this.PlaySchedule.Count > 0)
			{
				foreach (bool playSchedule in this.PlaySchedule)
				{
					num++;
					num++;
				}
			}
			return num;
		}

		public static ParentalControlInfo ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<ParentalControlInfo>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			ParentalControlInfo.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, ParentalControlInfo instance)
		{
			if (instance.HasTimezone)
			{
				stream.WriteByte(26);
				ProtocolParser.WriteBytes(stream, Encoding.UTF8.GetBytes(instance.Timezone));
			}
			if (instance.HasMinutesPerDay)
			{
				stream.WriteByte(32);
				ProtocolParser.WriteUInt32(stream, instance.MinutesPerDay);
			}
			if (instance.HasMinutesPerWeek)
			{
				stream.WriteByte(40);
				ProtocolParser.WriteUInt32(stream, instance.MinutesPerWeek);
			}
			if (instance.HasCanReceiveVoice)
			{
				stream.WriteByte(48);
				ProtocolParser.WriteBool(stream, instance.CanReceiveVoice);
			}
			if (instance.HasCanSendVoice)
			{
				stream.WriteByte(56);
				ProtocolParser.WriteBool(stream, instance.CanSendVoice);
			}
			if (instance.PlaySchedule.Count > 0)
			{
				foreach (bool playSchedule in instance.PlaySchedule)
				{
					stream.WriteByte(64);
					ProtocolParser.WriteBool(stream, playSchedule);
				}
			}
		}

		public void SetCanReceiveVoice(bool val)
		{
			this.CanReceiveVoice = val;
		}

		public void SetCanSendVoice(bool val)
		{
			this.CanSendVoice = val;
		}

		public void SetMinutesPerDay(uint val)
		{
			this.MinutesPerDay = val;
		}

		public void SetMinutesPerWeek(uint val)
		{
			this.MinutesPerWeek = val;
		}

		public void SetPlaySchedule(List<bool> val)
		{
			this.PlaySchedule = val;
		}

		public void SetTimezone(string val)
		{
			this.Timezone = val;
		}
	}
}