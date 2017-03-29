using System;
using System.Runtime.CompilerServices;

public class Dustin
{
	public int Age
	{
		get;
		private set;
	}

	public string Name
	{
		get;
		private set;
	}

	public string Url
	{
		get;
		private set;
	}

	public Dustin()
	{
		this.Name = "Dustin Horne";
		this.Age = 34;
		this.Url = "http://www.dustinhorne.com";
	}
}