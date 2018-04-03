using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class ThreadSafeStack : MemoryStreamStack, IDisposable
{
	private Stack<MemoryStream> stack = new Stack<MemoryStream>();

	public ThreadSafeStack()
	{
	}

	public void Dispose()
	{
		object obj = this.stack;
		Monitor.Enter(obj);
		try
		{
			this.stack.Clear();
		}
		finally
		{
			Monitor.Exit(obj);
		}
	}

	public MemoryStream Pop()
	{
		MemoryStream memoryStream;
		object obj = this.stack;
		Monitor.Enter(obj);
		try
		{
			memoryStream = (this.stack.Count != 0 ? this.stack.Pop() : new MemoryStream());
		}
		finally
		{
			Monitor.Exit(obj);
		}
		return memoryStream;
	}

	public void Push(MemoryStream stream)
	{
		object obj = this.stack;
		Monitor.Enter(obj);
		try
		{
			this.stack.Push(stream);
		}
		finally
		{
			Monitor.Exit(obj);
		}
	}
}