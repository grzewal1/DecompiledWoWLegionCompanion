using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace WoWCompanionApp
{
	public abstract class NewsService : MonoBehaviour
	{
		private List<NewsArticle> m_NewsArticles = new List<NewsArticle>();

		public DateTime FetchTime
		{
			get;
			protected set;
		}

		public List<NewsArticle> NewsArticles
		{
			get
			{
				return this.m_NewsArticles;
			}
		}

		protected NewsService()
		{
		}

		private void Start()
		{
		}

		private void Update()
		{
		}
	}
}