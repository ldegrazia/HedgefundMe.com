using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HedgefundMe.com
{
    public static class DateHelper
    {
        public static bool IsWeekend()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }

            return false;

        }
        public static bool IsWeekend(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }

            return false;

        }
        public static bool IsHoliday()
        {
            var presidentsDay = DateTime.Parse("2/17/2014");
            if (DateTime.Now.Date == presidentsDay.Date)
            {
                return true;
            }

            return false;

        }
        /// <summary>
        /// Returns friday or the latest day of the week.
        /// </summary>
        /// <returns></returns>
        public static DateTime GetLatestWeekdayDate()
        {
            DateTime now = DateTime.Now;

            if (now.DayOfWeek == DayOfWeek.Saturday)
            {
                return now = now.AddDays(-1).Date;
            }
            if (now.DayOfWeek == DayOfWeek.Sunday)
            {
                return now.AddDays(-2).Date;
            }

            DateTime dt = GetNextUpdateDate();
            TimeSpan date_difference = dt.Subtract(now);


            if (date_difference.Minutes > 0) // && now.Minute < dt.Minute)// 10 && now.Minute < 38)
            {
                if (now.DayOfWeek == DayOfWeek.Monday)
                {
                    return now.AddDays(-3).Date;
                }
                now = now.AddDays(-1).Date;
            }
            return now;
        }
        /// <summary>
        /// Returns the next update data load date.
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNextUpdateDate()
        {
            DateTime now = DateTime.Now;

            if (now.DayOfWeek == DayOfWeek.Saturday)
            {
                now = now.AddDays(+2);
                DateTime Updatedate = new DateTime(now.Year, now.Month, now.Day, 10, 38, 00);
                return Updatedate;
            }
            if (now.DayOfWeek == DayOfWeek.Sunday)
            {
                now = now.AddDays(+1);
                DateTime Updatedate = new DateTime(now.Year, now.Month, now.Day, 10, 38, 00);
                return Updatedate;
            }
            if (now.DayOfWeek == DayOfWeek.Friday)
            {
                if (now.Hour > 10 && now.Minute > 38)
                {
                    now = now.AddDays(3);
                }
                DateTime Updatedate = new DateTime(now.Year, now.Month, now.Day, 10, 38, 00);
                return Updatedate;
            }
            DateTime weekdayupdatedate = new DateTime(now.Year, now.Month, now.Day, 10, 38, 00);
            if (now.Hour > 22)
            {

                weekdayupdatedate = weekdayupdatedate.AddDays(1);

            }
            return weekdayupdatedate;
        }
        public static List<int> GetMonthsOut()
        {
            List<int> mylist = new List<int>();
            for (int i = 1; i < 200; i++)
            {
                mylist.Add(i);
            }
            return mylist;
        }
        public static DateTime GetLastFriday()
        {
            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                return DateTime.Now.AddDays(-1).Date;
            }
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                return DateTime.Now.AddDays(-2).Date;
            }
            return DateTime.Now.Date;
        }
        /// <summary>
        /// Does not return a weekend
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime SanitizeDate(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                return date.AddDays(-1).Date;
            }
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                return date.AddDays(-2).Date;
            }
            return date.Date;
        }
        /// <summary>
        /// Returns the next weekday date, if date is sat or sunday returns monday
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static DateTime GetNextWeekdayDate(DateTime date)
        {
            if (date.DayOfWeek == DayOfWeek.Saturday)
            {
                return date.AddDays(+2).Date;
            }
            if (date.DayOfWeek == DayOfWeek.Sunday)
            {
                return date.AddDays(+1).Date;
            }
            return date.Date;
        }
        public static string GetTimeInWords(string dateString)
        {
            DateTime thedate;
            if (DateTime.TryParse(dateString, out thedate))
            {
                return GetTimeInWords(thedate);
            }
            return dateString;

        }
        /// <summary>
        /// Returns hours ago
        /// </summary>
        /// <param name="theDate"></param>
        /// <returns></returns>
        public static string GetTimeInWords(DateTime theDate)
        {
            //get the time difference in days
            DateTime dt = DateTime.Now;
            TimeSpan date_difference = dt.Subtract(theDate);
            switch (date_difference.Days)
            {
                case 0:
                    {
                        switch (date_difference.Hours)
                        {
                            case 0:
                                {
                                    return " " + date_difference.Minutes + " minutes ago";
                                }
                            case 1:
                                {
                                    return " an hour ago";
                                }
                            default:
                                {
                                    return date_difference.Hours + " hours ago";
                                }
                        }


                    }
                case 1:
                    {

                        return " Yesterday";
                    }
                default:
                    {
                        return " " + date_difference.Days + " days ago";
                    }
            }

        }
        /// <summary>
        /// Finds the days that have past from the earlier data
        /// </summary>
        /// <param name="laterDate"></param>
        /// <param name="earlierDate"></param>
        /// <returns></returns>
        public static int GetDaysPast(DateTime laterDate, DateTime earlierDate)
        {
            TimeSpan span = laterDate.Subtract(earlierDate);
            return span.Days;
        }
        /// <summary>
        /// For example, to check if it's between 6 AM and 2:30 PM PST
        /// IsTimeOfDayBetween(someTime, new TimeSpan(6, 0, 0), new TimeSpan(14, 30, 0))
        /// </summary>
        /// <param name="time"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
         public static bool IsTimeOfDayBetween(DateTime time,
                                      TimeSpan startTime, TimeSpan endTime)
        {
            if (endTime == startTime)
            {
                return true;
            }
            else if (endTime < startTime)
            {
                return time.TimeOfDay <= endTime ||
                    time.TimeOfDay >= startTime;
            }
            else
            {
                return time.TimeOfDay >= startTime &&
                    time.TimeOfDay <= endTime;
            }

        }
        /// <summary>
        /// Returns true if the time is between 6:30 and 13:00 (9:30am and 4:00pm)
        /// </summary>
        /// <returns></returns>
        public static bool IsMarketHours()
         {
            if (DateTime.Now.Hour == 6 && DateTime.Now.Minute >= 30)
            {
                return true;
            }
            if (DateTime.Now.Hour <= 6)
            {
                 return false;
            }
            if(DateTime.Now.Hour >= 13)
            {
                return false;
            } 
            return true;
         }
        /// <summary>
        /// Returns true if the time is 4 and before 4:02pm
        /// </summary>
        /// <returns></returns>
        public static bool IsFourPm()
        {
            return (DateTime.Now.Hour == 13 && DateTime.Now.Minute < 02);
        }
        /// <summary>
        /// Returns true if the time is 3:45 and before 4:0pm
        /// </summary>
        /// <returns></returns>
        public static bool IsThreeFortyFive()
        {
            return (DateTime.Now.Hour == 12 && DateTime.Now.Minute >= 44);
        }
        /// <summary>
        /// Returns true if the time is 4 and before 4:30pm
        /// </summary>
        /// <returns></returns>
        public static bool IsFourToFourThirtyPm()
        {
            return (DateTime.Now.Hour == 13 && DateTime.Now.Minute <29);
        }
        /// <summary>
        /// Returns true if the time is 4:30
        /// </summary>
        /// <returns></returns>
        public static bool IsFourThirty()
        {
            return (DateTime.Now.Hour == 13 && (DateTime.Now.Minute >= 29 && DateTime.Now.Minute <= 35)); 
        }
        /// <summary>
        /// returns true if the time is after 3:30pm but before 4  
        /// </summary>
        /// <returns></returns>
        public static bool IsLastHalfHour()
        { 
            return DateTime.Now.Hour == 12 && DateTime.Now.Minute >= 30;
        }
        /// <summary>
        /// returns true if the time is 9:30 am or before 9:40 am  
        /// </summary>
        /// <returns></returns>
        public static bool IsFirstRun()
        {
            return (DateTime.Now.Hour == 6 && (DateTime.Now.Minute >29 && DateTime.Now.Minute < 40));
        }
        /// <summary>
        /// returns true if the time is 9:40 am or before 9:59 am  
        /// </summary>
        /// <returns></returns>
        public static bool IsBeforeTenAm()
        {
            return (DateTime.Now.Hour == 6 && (DateTime.Now.Minute > 30 && DateTime.Now.Minute < 59));
        }
        /// <summary>
        /// Returns the volume change ratio for the current time of day
        /// </summary>
        /// <returns></returns>
        public static double GetVolumeChangeTrigger(bool? isLocal)
        {
            /*  11 2 ratio 8/9  0.0075
                10 1 ratio 7/9  0.00667
                9  12 ratio 1/2 0.005
                8  11 ratio 1/3 0.00334
                7  10 ratio 1/4 0.0025
             *  everything else 0.011
             * 
             */
            var hour = DateTime.Now.Hour;
            if(isLocal.HasValue && isLocal.Value)
            {
                switch (hour)
                {
                    case 14:
                        {
                            return 0.0085;
                        }
                    case 13:
                        {
                            return 0.0065;
                        }
                    case 12:
                        {
                            return 0.0045;
                        }
                    case 11:
                        {
                            return 0.0045;
                        }
                    case 10:
                        {
                            return 0.0035;
                        }
                    default:
                        {
                            return 0.011;
                        }
                }
            }
           
            switch(hour)
            {
                case 11: //2PM
                    {
                        return 0.0085;
                    }
                case 10: //1PM
                    {
                        return 0.0065;
                    }
                case 9: //12PM
                    {
                        return 0.0055;
                    }
                case 8: //11am
                    {
                        return 0.0045;
                    }
                case 7: //10 am
                    {
                        return 0.0035;
                    }
                default:
                    {
                        return 0.011;
                    }
            }
        }
        public static string LastRankFetch()
        {
            if (DateTime.Now.Hour < 6 || DateTime.Now.Hour > 14 || DateHelper.IsWeekend())
            {
                return "5:00 PM EST";                    
            }
            var eastern = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            var local = TimeZoneInfo.Local; // PDT  

            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime now = DateTime.Now.ToUniversalTime();

            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(now, easternZone);

            return easternTime.ToString("h tt"); 
                 
             
        }
        /// <summary>
        /// Returns the time as Easter Standard time
        /// </summary>
        /// <param name="theTime"></param>
        /// <returns></returns>
        public static DateTime ToEasternStandardTime(DateTime theTime)
        { 

            TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime now = theTime.ToUniversalTime();

            DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(now, easternZone);

            return easternTime; 
        }
        /// <summary>
        /// Throws an exception if the current time and date are not valid for fetching data
        /// </summary>
        public static void DateGuard()
        {
            if (DateHelper.IsWeekend())
            {
                //Logger.WriteLine(MessageType.Information, "Fetch not run, it is a weekend.");
                throw new Exception(" It is a weekend. ");
            }
            if (DateHelper.IsHoliday())
            {
                //Logger.WriteLine(MessageType.Information, "Fetch not run, it is a holiday.");
                throw new Exception(" It is a holiday. ");
            }
            if (!DateHelper.IsMarketHours())
            {
                //Logger.WriteLine(MessageType.Information, "Fetch not run, it is not between 6:30 AM and 2 PM PST.");
                throw new Exception( "It is not between 6:30 AM and 2 PM PST.");
            }
        }
    }
}