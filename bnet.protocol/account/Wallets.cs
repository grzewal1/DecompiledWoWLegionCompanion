using System;
using System.Collections.Generic;
using System.IO;

namespace bnet.protocol.account
{
	public class Wallets : IProtoBuf
	{
		private List<Wallet> _Wallets_ = new List<Wallet>();

		public bool IsInitialized
		{
			get
			{
				return true;
			}
		}

		public List<Wallet> Wallets_
		{
			get
			{
				return this._Wallets_;
			}
			set
			{
				this._Wallets_ = value;
			}
		}

		public int Wallets_Count
		{
			get
			{
				return this._Wallets_.Count;
			}
		}

		public List<Wallet> Wallets_List
		{
			get
			{
				return this._Wallets_;
			}
		}

		public Wallets()
		{
		}

		public void AddWallets_(Wallet val)
		{
			this._Wallets_.Add(val);
		}

		public void ClearWallets_()
		{
			this._Wallets_.Clear();
		}

		public void Deserialize(Stream stream)
		{
			Wallets.Deserialize(stream, this);
		}

		public static Wallets Deserialize(Stream stream, Wallets instance)
		{
			return Wallets.Deserialize(stream, instance, (long)-1);
		}

		public static Wallets Deserialize(Stream stream, Wallets instance, long limit)
		{
			if (instance.Wallets_ == null)
			{
				instance.Wallets_ = new List<Wallet>();
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
					else if (num == 10)
					{
						instance.Wallets_.Add(Wallet.DeserializeLengthDelimited(stream));
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

		public static Wallets DeserializeLengthDelimited(Stream stream)
		{
			Wallets wallet = new Wallets();
			Wallets.DeserializeLengthDelimited(stream, wallet);
			return wallet;
		}

		public static Wallets DeserializeLengthDelimited(Stream stream, Wallets instance)
		{
			long position = (long)ProtocolParser.ReadUInt32(stream);
			position = position + stream.Position;
			return Wallets.Deserialize(stream, instance, position);
		}

		public override bool Equals(object obj)
		{
			Wallets wallet = obj as Wallets;
			if (wallet == null)
			{
				return false;
			}
			if (this.Wallets_.Count != wallet.Wallets_.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Wallets_.Count; i++)
			{
				if (!this.Wallets_[i].Equals(wallet.Wallets_[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = this.GetType().GetHashCode();
			foreach (Wallet wallets_ in this.Wallets_)
			{
				hashCode = hashCode ^ wallets_.GetHashCode();
			}
			return hashCode;
		}

		public uint GetSerializedSize()
		{
			uint num = 0;
			if (this.Wallets_.Count > 0)
			{
				foreach (Wallet wallets_ in this.Wallets_)
				{
					num++;
					uint serializedSize = wallets_.GetSerializedSize();
					num = num + serializedSize + ProtocolParser.SizeOfUInt32(serializedSize);
				}
			}
			return num;
		}

		public static Wallets ParseFrom(byte[] bs)
		{
			return ProtobufUtil.ParseFrom<Wallets>(bs, 0, -1);
		}

		public void Serialize(Stream stream)
		{
			Wallets.Serialize(stream, this);
		}

		public static void Serialize(Stream stream, Wallets instance)
		{
			if (instance.Wallets_.Count > 0)
			{
				foreach (Wallet wallets_ in instance.Wallets_)
				{
					stream.WriteByte(10);
					ProtocolParser.WriteUInt32(stream, wallets_.GetSerializedSize());
					Wallet.Serialize(stream, wallets_);
				}
			}
		}

		public void SetWallets_(List<Wallet> val)
		{
			this.Wallets_ = val;
		}
	}
}