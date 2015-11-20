using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SH.Web.Utils
{
    public static class ValueRenderer
    {
        /// <summary>
        /// Convert seconds to string format: days HH:mm:ss
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static string SecondsToDaysTime(this long sec)
        {
            int secPerDay = 86400;

            int days = (int)(sec / secPerDay);
            sec -= days * secPerDay;

            int hours = (int)sec / 3600;
            sec -= hours * 3600;

            int minutes = (int)sec / 60;
            sec -= minutes * 60;

            string result = hours + ":" + minutes + ":" + sec;

            if (days > 0) result = days + " Days " + result;

            return result;
        }
        /// <summary>
        /// Conveert bytes value to Giga bytes value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToGb(this long value)
        {
            float result = value;
            result /= 1024;
            result /= 1024;
            result /= 1024;

            result = ((int)(result * 10)) / 10.0f;

            string ret = result.ToString();

            return ret;
        }
        /// <summary>
        /// Convert bytes size to KB,MB,GB, TB,PB
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToXBytes(this long value)
        {
            string[] formats = new[] { "B", "K", "M", "G", "T", "P" };
            int mult = 1024;
            int index = 0;

            float result = value;
            while (result > mult && index < formats.Length - 1)
            {
                result /= mult;
                index++;
            }

            result = ((int)(result * 10)) / 10.0f;

            string ret = result.ToString() + " " + formats[index];

            if (index > 0) ret += "B";

            return ret;
        }
    }
}