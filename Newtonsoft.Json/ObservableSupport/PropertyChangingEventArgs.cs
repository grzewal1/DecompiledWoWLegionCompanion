using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.ObservableSupport
{
	public class PropertyChangingEventArgs : EventArgs
	{
		public virtual string PropertyName
		{
			get;
			set;
		}

		public PropertyChangingEventArgs(string propertyName)
		{
			this.PropertyName = propertyName;
		}
	}
}