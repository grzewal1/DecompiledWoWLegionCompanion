using System;

namespace bgs
{
	public class EventListener<Delegate>
	{
		protected Delegate m_callback;

		protected object m_userData;

		public EventListener()
		{
		}

		public EventListener(Delegate callback, object userData)
		{
			this.m_callback = callback;
			this.m_userData = userData;
		}

		public override bool Equals(object obj)
		{
			EventListener<Delegate> eventListener = obj as EventListener<Delegate>;
			if (eventListener == null)
			{
				return this.Equals(obj);
			}
			return (!this.m_callback.Equals(eventListener.m_callback) ? false : this.m_userData == eventListener.m_userData);
		}

		public Delegate GetCallback()
		{
			return this.m_callback;
		}

		public override int GetHashCode()
		{
			int hashCode = 23;
			if (this.m_callback != null)
			{
				hashCode = hashCode * 17 + this.m_callback.GetHashCode();
			}
			if (this.m_userData != null)
			{
				hashCode = hashCode * 17 + this.m_userData.GetHashCode();
			}
			return hashCode;
		}

		public object GetUserData()
		{
			return this.m_userData;
		}

		public void SetCallback(Delegate callback)
		{
			this.m_callback = callback;
		}

		public void SetUserData(object userData)
		{
			this.m_userData = userData;
		}
	}
}