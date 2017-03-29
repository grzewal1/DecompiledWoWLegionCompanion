using System;
using System.IO;

public class AllocationStack : IDisposable, MemoryStreamStack
{
	public AllocationStack()
	{
	}

	public void Dispose()
	{
	}

	public MemoryStream Pop()
	{
		return new MemoryStream();
	}

	public void Push(MemoryStream stream)
	{
	}
}