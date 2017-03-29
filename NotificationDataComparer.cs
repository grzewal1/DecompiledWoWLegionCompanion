using System;
using System.Collections.Generic;

public class NotificationDataComparer : IComparer<NotificationData>
{
	public NotificationDataComparer()
	{
	}

	public int Compare(NotificationData n1, NotificationData n2)
	{
		return (int)(n1.secondsRemaining - n2.secondsRemaining);
	}
}