using System;
using System.Runtime.CompilerServices;

public class KeyValue
{
	public Key Key
	{
		get;
		set;
	}

	public byte[] Value
	{
		get;
		set;
	}

	public KeyValue(Key key, byte[] value)
	{
		this.Key = key;
		this.Value = value;
	}

	public override string ToString()
	{
		return string.Format("[KeyValue: {0}, {1}, {2} bytes]", this.Key.Field, this.Key.WireType, (int)this.Value.Length);
	}
}