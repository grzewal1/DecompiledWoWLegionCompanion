using System;
using System.Runtime.CompilerServices;

namespace bgs
{
	internal class ResourcesAPIPendingState
	{
		public ResourcesAPI.ResourceLookupCallback Callback
		{
			get;
			set;
		}

		public object UserContext
		{
			get;
			set;
		}

		public ResourcesAPIPendingState()
		{
		}
	}
}