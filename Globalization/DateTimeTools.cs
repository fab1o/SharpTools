using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fabio.SharpTools.Globalization
{
    public sealed class DateTimeTools
    {
        private DateTimeTools() { }

        #region week
        public static DateTime? GetPastDate(short? dayOfWeek)
        {
            if (!dayOfWeek.HasValue)
                return null;

            if (dayOfWeek.Value < 0 || dayOfWeek.Value > 6)
                return null;

            DateTime weekDate = DateTime.Today;

            if (weekDate.DayOfWeek != (DayOfWeek)dayOfWeek.Value)
            {
                do
                {
                    DateTime temp = weekDate.AddDays(-1);
                    weekDate = new DateTime(temp.Year, temp.Month, temp.Day);
                } while (weekDate.DayOfWeek != (DayOfWeek)dayOfWeek.Value);
            }

            return weekDate;
        }

        /// <summary>
        /// Gets the past date before today
        /// </summary>
        /// <param name="dayOfWeek">Day of the week to get the date</param>
        /// <returns></returns>
        public static DateTime GetPastDate(DayOfWeek dayOfWeek)
        {
            DateTime weekDate = DateTime.Today;

            if (weekDate.DayOfWeek != dayOfWeek)
            {
                do
                {
                    DateTime temp = weekDate.AddDays(-1);
                    weekDate = new DateTime(temp.Year, temp.Month, temp.Day);
                } while (weekDate.DayOfWeek != dayOfWeek);
            }

            return weekDate;
        }

        /// <summary>
        /// Gets the past date before the specified date
        /// </summary>
        /// <param name="date">the specified date</param>
        /// <param name="dayOfWeek">Day of the week to get the date</param>
        /// <returns></returns>
        public static DateTime GetPastDate(DateTime date, DayOfWeek dayOfWeek)
        {
            DateTime weekDate = date;

            if (weekDate.DayOfWeek != dayOfWeek)
            {
                do
                {
                    DateTime temp = weekDate.AddDays(-1);
                    weekDate = new DateTime(temp.Year, temp.Month, temp.Day);
                } while (weekDate.DayOfWeek != dayOfWeek);
            }

            return weekDate;
        }

        public static DateTime? GetNextDate(short? dayOfWeek)
        {
            if (!dayOfWeek.HasValue)
                return null;

            if (dayOfWeek.Value < 0 || dayOfWeek.Value > 6)
                return null;

            DateTime weekDate = DateTime.Today;

            if (weekDate.DayOfWeek != (DayOfWeek)dayOfWeek.Value)
            {
                do
                {
                    DateTime temp = weekDate.AddDays(1);
                    weekDate = new DateTime(temp.Year, temp.Month, temp.Day);
                } while (weekDate.DayOfWeek != (DayOfWeek)dayOfWeek.Value);
            }

            return weekDate;
        }

        /// <summary>
        /// Gets the next date after today
        /// </summary>
        /// <param name="dayOfWeek">Day of the week to get the date</param>
        /// <returns></returns>
        public static DateTime GetNextDate(DayOfWeek dayOfWeek)
        {
            DateTime weekDate = DateTime.Today;

            if (weekDate.DayOfWeek != dayOfWeek)
            {
                do
                {
                    DateTime temp = weekDate.AddDays(1);
                    weekDate = new DateTime(temp.Year, temp.Month, temp.Day);
                } while (weekDate.DayOfWeek != dayOfWeek);
            }

            return weekDate;
        }

        /// <summary>
        /// Gets the next date after the specified date
        /// </summary>
        /// <param name="date">the specified date</param>
        /// <param name="dayOfWeek">Day of the week to get the date</param>
        /// <returns></returns>
        public static DateTime GetNextDate(DateTime date, DayOfWeek dayOfWeek)
        {
            DateTime weekDate = date;

            if (weekDate.DayOfWeek != dayOfWeek)
            {
                do
                {
                    DateTime temp = weekDate.AddDays(1);
                    weekDate = new DateTime(temp.Year, temp.Month, temp.Day);
                } while (weekDate.DayOfWeek != dayOfWeek);
            }

            return weekDate;
        }

        public static DateTime GetPastDateTime(DayOfWeek dayOfWeek)
        {
            DateTime weekDate = DateTime.Now;

            while (weekDate.DayOfWeek != dayOfWeek)
                weekDate = weekDate.AddDays(-1);

            return weekDate;
        }

        public static DateTime GetNextDateTime(DayOfWeek dayOfWeek)
        {
            DateTime weekDate = DateTime.Now;

            while (weekDate.DayOfWeek != dayOfWeek)
                weekDate = weekDate.AddDays(1);

            return weekDate;
        }
        #endregion

    }

}
