using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AntColonyWeb.Parser
{
    public class TimeConverter
    {
        public static string[][] ConvertToStringUkr(string[][] time, int n)
        {
            string[][] time_res = time;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (i == j) continue;
                    else
                        time_res[i][j] = time_res[i][j].Replace("ч.", "год.").Replace("мин.", "хв.");
            return time_res;
        }

        public static int[][] ConvertToInt(string[][] time, int n)
        {
            string[][] time_res = time;
            int[][] times_result = new int[n][];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    times_result[i] = new int[n];
                    times_result[i][j] = 0;
                }

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (i == j) continue;
                    else 
                    {
                        int firstIndex = 0;
                        int secondIndex = time_res[i][j].IndexOf(" год.", firstIndex);
                        int hours = Convert.ToInt32(time_res[i][j].Substring(firstIndex, secondIndex - firstIndex));
                        times_result[i][j] = hours * 60; 
                        var searchFor = "год. "; 
                        firstIndex = time_res[i][j].IndexOf(searchFor);
                        secondIndex = time_res[i][j].IndexOf(" хв.", firstIndex);
                        int minutes = Convert.ToInt32(time_res[i][j].Substring(firstIndex + searchFor.Length, secondIndex - firstIndex - searchFor.Length));
                        times_result[i][j] += minutes;
                    }
            
            return times_result;
        }
    }
}