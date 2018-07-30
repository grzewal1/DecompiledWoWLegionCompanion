using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WoWCompanionApp
{
	public class NewsArticle
	{
		public string Body
		{
			get;
			private set;
		}

		public DateTime Date
		{
			get;
			private set;
		}

		public Texture2D Image
		{
			get;
			private set;
		}

		public string Title
		{
			get;
			private set;
		}

		public NewsArticle(string title, string text, Texture2D image, DateTime date)
		{
			this.Title = title;
			this.Body = text;
			this.Image = image;
			this.Date = date;
		}
	}
}