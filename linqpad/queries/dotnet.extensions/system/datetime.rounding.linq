<Query Kind="Program" />

void Main()
{
	
	
	
	var constant = 60.00; // Time Constant (always in interval of 60)
	
	(DateTime.Now.Ticks % constant).Dump();
	
	var now = DateTime.Now;
	
	now.AddTicks(-now.TimeOfDay.Ticks).Dump();
	var o = 13;
	var x = (double)now.Ticks;
	
	var t = Math.Round(x, o, MidpointRounding.ToEven);
	
	DateTime.FromBinary((long)t).Dump();
	
	
	
	
	DateTime.Now.RoundToMinute(18, DateTimeMidpointRounding.Nearest).Dump();
	
	var interval = 11;
	var remainder = 60 % interval;
	var fixedInterval = 60 / (60 / interval);
	var divisionCount = 60 / fixedInterval;
	
	
	var minute =  TimeSpan.FromMinutes(fixedInterval).Ticks;
	var current = DateTime.Now.Ticks;

	
	var flooring = (current / minute) * minute;
	var nearest  = ((current + (minute >> 1) + 1)/ minute) * minute;
	var ceiling =  ((current + minute - 1)/ minute) * minute;
	
	fixedInterval.Dump();
	remainder.Dump();
	
	flooring.Dump();
	nearest.Dump();
	ceiling.Dump();
	
	
	
	var newDateTime = DateTime.FromBinary(flooring).Dump();

	
	
	new DateTime(flooring).Dump();
	new DateTime(nearest).Dump();
	new DateTime(ceiling).Dump();
	
	
	/*
		
		currentMinute / interval 
	
	*/
	
	//var current = DateTime.Now;
	//var minute = TimeSpan.FromMinutes(current.Minute).Ticks;
	//var interval = (long)13;
	//var ticks = current.Ticks;
	//
	//while(interval < minute)
	//{
	//	if(interval++ > minute)
	//		
	//	if(interval > 60)
	//	
	//	interval++;
	//}
	//
	//var interval = 23;
	//var upperBound = 60;
	//var lowerBound = 0;
	//
	//if(
	
	
	
	
	
//	var interval = 22;
//	var currentTime = DateTime.Now;
//	var elapsedTime = new TimeSpan(currentTime.Hour, currentTime.Minute, currentTime.Second).Ticks;
//	var minute = TimeSpan.FromMinutes(interval).Ticks;
//
//	//DateTime.Now.TimeOfDay.Ticks
//
//	var flooring = (elapsedTime / minute) * minute;
//	var nearest  = ((elapsedTime + (minute >> 1) + 1)/ minute) * minute;
//	var ceiling =  ((elapsedTime + minute - 1)/ minute) * minute;
//	
//	TimeSpan.FromTicks(flooring).Dump();
//	TimeSpan.FromTicks(nearest).Dump();
//	TimeSpan.FromTicks(ceiling).Dump();
//	
//	currentTime.AddTicks(-(currentTime.TimeOfDay.Ticks)).AddTicks(flooring).Dump();
//	currentTime.AddTicks(-(currentTime.TimeOfDay.Ticks)).AddTicks(nearest).Dump();
//	currentTime.AddTicks(-(currentTime.TimeOfDay.Ticks)).AddTicks(ceiling).Dump();
//
//	//DateTime.Now..AddTicks(flooring).Dump();
//	//DateTime(nearest).Dump();
//	//DateTime(ceiling).Dump();

}


namespace System
{
	
	#region datetime.rounding
	public enum DateTimeMidpointRounding
	{
		Up,
		Down,
		Nearest
	}


	public static class DateTimeRound
	{
		/// <summary>
		/// </summary>
		/// <param name="second">
		public static DateTime RoundToSecond(this DateTime dateTime, double second, DateTimeMidpointRounding midpointRounding)
		{
			
			return new DateTime(RoundTimeSpanTicks(dateTime.Ticks, TimeSpan.FromSeconds(second).Ticks, midpointRounding));
		}
			

		public static DateTime RoundToMinute(this DateTime dateTime, int minute, DateTimeMidpointRounding midpointRounding = DateTimeMidpointRounding.Nearest)
		{
			var interval = TimeSpan.FromMinutes(minute);
			var divisible = IsDivible(60, (int)minute);

			if(minute < 0 || minute > 30)
				throw new ArgumentOutOfRangeException("minute", minute, "The value needs to be either between 0 or 30 in order to be divisble by 60 minutes.");

			if (divisible && midpointRounding == DateTimeMidpointRounding.Nearest)
				return dateTime.AddTicks(((interval.Ticks + 1) >> 1) - ((dateTime.Ticks + ((interval.Ticks + 1) >> 1)) % interval.Ticks));

			if (divisible && midpointRounding == DateTimeMidpointRounding.Up)
				return (dateTime.Ticks % interval.Ticks) == 0 ? dateTime : dateTime.AddTicks(interval.Ticks - (dateTime.Ticks % interval.Ticks));

			if(divisible && midpointRounding == DateTimeMidpointRounding.Down)
				return dateTime.AddTicks(-(dateTime.Ticks % TimeSpan.FromMinutes(minute).Ticks));

			return dateTime.AddTicks(((interval.Ticks + 1) >> 1) - ((dateTime.Ticks + ((interval.Ticks + 1) >> 1)) % interval.Ticks));

		}
			

		public static DateTime RoundToHour(this DateTime dateTime, double hour, DateTimeMidpointRounding midpointRounding) 
		{
			if(hour < -24 || hour > 24)
				throw new ArgumentOutOfRangeException("hour", hour, "The value needs to be either between -24 or 24");
				
			return new DateTime(RoundTimeSpanTicks(dateTime.Ticks, TimeSpan.FromHours(hour).Ticks, midpointRounding));	
		}

		private static bool IsDivible(int a, int b)
		{
			while (b % 2 == 0) { b /= 2; }
			while (b % 5 == 0) { b /= 5; }
			return a % b == 0;
		}



		private static long RoundTimeSpanTicks(long currentTicks, long intervalTicks, DateTimeMidpointRounding midpointRounding)
		{
			switch (midpointRounding)
			{
				// Flooring (DateTime Ticks)
				case DateTimeMidpointRounding.Down:
					//dateTime.AddTicks(-(dateTime.Ticks % interval.Ticks));
					return (currentTicks / intervalTicks) * intervalTicks;

				// Nearest (DateTime Ticks)
				case DateTimeMidpointRounding.Nearest:
					return ((currentTicks + (intervalTicks / 2) + 1) / intervalTicks) * intervalTicks;

				// Ceiling (DateTime Ticks)
				case DateTimeMidpointRounding.Up:
					return ((currentTicks + intervalTicks - 1) / intervalTicks) * intervalTicks;

				default:
					return 0;
			}
		}
	}
	#endregion
}

