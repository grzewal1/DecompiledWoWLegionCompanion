using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class ThreadSafeStack : IDisposable, MemoryStreamStack
{
	private Stack<MemoryStream> stack = new Stack<MemoryStream>();

	public ThreadSafeStack()
	{
	}

	public void Dispose()
	{
		Stack<MemoryStream> memoryStreams = this.stack;
		Monitor.Enter(memoryStreams);
		try
		{
			this.stack.Clear();
		}
		finally
		{
			Monitor.Exit(memoryStreams);
		}
	}

	public MemoryStream Pop()
	{
		MemoryStream memoryStream;
		Stack<MemoryStream> memoryStreams = this.stack;
		Monitor.Enter(memoryStreams);
		try
		{
			memoryStream = (this.stack.Count != 0 ? this.stack.Pop() : new MemoryStream());
		}
		finally
		{
			Monitor.Exit(memoryStreams);
		}
		return memoryStream;
	}

	public void Push(MemoryStream stream)
	{
		Stack<MemoryStream> memoryStreams = this.stack;
		Monitor.Enter(memoryStreams);
		try
		{
			this.stack.Push(stream);
		}
		finally
		{
			Monitor.Exit(memoryStreams);
		}
	}
}