using System;
using System.Runtime.CompilerServices;

public class Key
{
	public uint Field
	{
		get;
		set;
	}

	public Wire WireType
	{
		get;
		set;
	}

	public Key(uint field, Wire wireType)
	{
		this.Field = field;
		this.WireType = wireType;
	}

	public override string ToString()
	{
		return string.Format("[Key: {0}, {1}]", this.Field, this.WireType);
	}
}