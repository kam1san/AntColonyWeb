using AntColonyWeb.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace AntColonyWeb.AntColony
{
    internal class AntColonyMain
    {
        private static Random random = new Random(0);
        private static string folder = AppDomain.CurrentDomain.BaseDirectory;
        private static double a;
        private static double b;
        private static double r;
        private static double Q;
        private static int n;
        private static int ants_quantity;
        private static int maxTime;
        private static int Money;
        private static int Time;
        private static int[][] cash_distances;
        private static int[][] time_distances;
        private static int[] values;

        public static AntColonyResult Execute(AntColonyAlgorithmSetup setup)
        {
            return AntColony(setup);
        }
        private static AntColonyResult AntColony(AntColonyAlgorithmSetup setup)
        {
            Console.Clear();
            try
            {
                FilesInput(setup);

                int[][] ants = new int[ants_quantity][];
                for (int i = 0; i <= ants.Length - 1; i++)
                {
                    ants[i] = new int[ants_quantity];
                }

                double[] bestLength = new double[2];
                int[] best_trail = new int[n];

                double bestValue = 0;

                double[][] f = new double[n][];
                for (int i = 0; i <= n - 1; i++)
                {
                    f[i] = new double[n];
                }

                for (int i = 0; i <= f.Length - 1; i++)
                {
                    for (int j = 0; j <= f[i].Length - 1; j++)
                    {
                        if (cash_distances[i][j] == 0)
                            f[i][j] = 0;
                        else
                            f[i][j] = 0.01;
                    }
                }

                int time = 0;
                while (time < maxTime)
                {
                    AntsUpdate(ants, f, cash_distances, time_distances);
                    PheromonesUpdate(f, ants, cash_distances, time_distances);

                    int[] best_trail_current = AntColonyMain.FindBestTrail(ants, cash_distances, time_distances, values);
                    double[] best_length_current = FindLength(best_trail_current, cash_distances, time_distances);
                    double best_value_current = FindValues(best_trail_current, values);
                    if ((bestValue < best_value_current && (best_length_current[0] <= Money && best_length_current[1] <= Time)))
                    {
                        bestLength = best_length_current;
                        bestValue = best_value_current;
                        best_trail = best_trail_current;
                        TrailToString(best_trail);
                    }
                    time++;
                }
                List<int> trail = TrailToString(best_trail);
                AntColonyResult result = new AntColonyResult(trail, bestLength[0], bestLength[1], bestValue);
                return result;
            }
            catch (Exception e)
            {
                return null; 
            }
        }


        private static List<int> TrailToString(int[] trail)
        {
            List<int> res = new List<int>();
            int ind = 0;
            for (int i = 0; i < trail.Length; i++)
            {
                if (trail[i] == -1)
                    ind++;
            }

            if (ind == 0)
            {
                for (int i = 0; i <= trail.Length; i++)
                {
                    if (i == trail.Length && trail[i - 1] != trail[0])
                    {
                        res.Add(trail[0]);
                    }
                    else
                    {
                        if (i == trail.Length - 1)
                        {
                            if (trail[i] == trail[0])
                            {
                                res.Add(trail[0]);
                            }
                            else
                            {
                                res.Add(trail[i]);
                            }
                        }

                        if (i < trail.Length - 1)
                        {
                            res.Add(trail[i]);
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < trail.Length; i++)
                {
                    if (trail[i] == -1)
                    {
                    }
                    else
                    {
                        if (i != trail.Length - 1)
                        {
                            if (trail[i + 1] == -1)
                            {
                                res.Add(trail[i]);
                            }
                            else
                            {
                                res.Add(trail[i]);
                            }
                        }
                        else
                        {
                            res.Add(trail[i]);
                        }
                    }
                }
            }
            return res;
        }

        private static void FilesInput(AntColonyAlgorithmSetup setup)
        {
            try
            {
                a = setup.a;
                b = setup.b;
                r = setup.r;
                Q = setup.Q;
                n = setup.n;
                ants_quantity = setup.m;
                maxTime = setup.iterations;
                Money = setup.max_money;
                Time = setup.max_time;
                if (a < 0 || b < 0 || r < 0 || Q < 0 || n < 2 || ants_quantity < 1 || maxTime < 1 || Money < 0 || Time < 0)
                    throw new Exception("Один або більше параметрів є невірними");

            }
            catch (Exception ex)
            {
                Environment.Exit(0);
            }


            time_distances = new int[n][];
            for (int i = 0; i <= time_distances.Length - 1; i++)
            {
                time_distances[i] = new int[n];
            }

            for (int i = 0; i <= n - 1; i++)
            {
                for (int j = 0; j <= n - 1; j++)
                {
                    if (setup.DT[i][j] == 0)
                        continue;
                    else
                        time_distances[i][j] = setup.CT[i] + setup.DT[i][j];
                }
            }

            cash_distances = new int[n][];
            for (int i = 0; i <= cash_distances.Length - 1; i++)
            {
                cash_distances[i] = new int[n];
            }

            for (int i = 0; i <= n - 1; i++)
            {
                for (int j = 0; j <= n - 1; j++)
                {
                    if (setup.DC[i][j] == 0)
                        continue;
                    else
                        cash_distances[i][j] = setup.CC[i] + setup.DC[i][j];
                }
            }

            values = setup.values;
        }

        private static int FindCityindex(int[] trail, int city)
        {
            for (int i = 0; i <= trail.Length - 1; i++)
            {
                if (trail[i] == city)
                {
                    return i;
                }
            }
            return -1;
        }

        private static double[] FindLength(int[] trail, int[][] cash_distances, int[][] time_distances)
        {
            double[] result = new double[2];
            int counter = 0;
            for (int i = 1; i <= trail.Length - 1; i++)
                if (trail[i] == trail[0])
                    counter++;

            for (int i = 0; i <= trail.Length - 1; i++)
            {
                if (i == trail.Length - 1 && counter == 0)
                {
                    double[] res = new double[2] { cash_distances[trail[i]][trail[0]], time_distances[trail[i]][trail[0]] };
                    result[0] += res[0];
                    result[1] += res[1];
                }
                else
                {
                    if (i != trail.Length - 1)
                        if (trail[i + 1] != -1)
                        {
                            double[] res = new double[2] { cash_distances[trail[i]][trail[i + 1]], time_distances[trail[i]][trail[i + 1]] };
                            result[0] += res[0];
                            result[1] += res[1];
                        }
                }
            }
            return result;
        }

        private static int[] FindBestTrail(int[][] ants, int[][] cash_distances, int[][] time_distances, int[] values)
        {
            double[] bestLength = FindLength(ants[0], cash_distances, time_distances);
            double bestVal = FindValues(ants[0], values);
            int best_length_id = 0;
            for (int tmp = 1; tmp <= ants.Length - 1; tmp++)
            {
                double[] len = FindLength(ants[tmp], cash_distances, time_distances);
                double val = FindValues(ants[tmp], values);
                if ((bestVal < val && (len[0] + len[1] < bestLength[0] + bestLength[1])) || (bestVal == val && (len[0] + len[1] < bestLength[0] + bestLength[1])))
                {
                    bestLength = len;
                    bestVal = val;
                    best_length_id = tmp;
                }
            }
            int n = ants[0].Length;
            int[] best_trail_Renamed = new int[n];
            ants[best_length_id].CopyTo(best_trail_Renamed, 0);
            return best_trail_Renamed;
        }

        private static double FindValues(int[] trail, int[] values)
        {
            double result = 0;
            int counter = 0;
            for (int i = 1; i <= trail.Length - 1; i++)
                if (trail[i] == trail[0])
                    counter++;

            for (int i = 0; i <= trail.Length - 1; i++)
            {
                if (i == trail.Length - 1 && counter == 0)
                {
                    result += values[trail[i]];
                }
                else
                {
                    if (i != trail.Length - 1)
                        if (trail[i + 1] != -1)
                        {
                            result += values[trail[i]];
                        }
                }
            }
            return result;
        }

        private static void AntsUpdate(int[][] ants, double[][] f, int[][] cash_distances, int[][] time_distances)
        {
            int n = f.Length;
            for (int tmp = 0; tmp <= ants.Length - 1; tmp++)
            {
                Random rnd = new Random();
                int first = rnd.Next(0, n);
                //int first = 0; //ПОЧАТКОВА ТОЧКА
                int[] newTrail = TrailChoice(tmp, first, f, cash_distances, time_distances);
                ants[tmp] = newTrail;
            }
        }

        private static int[] TrailChoice(int tmp, int first, double[][] f, int[][] cash_distances, int[][] time_distances)
        {
            int n = f.Length;
            int[] trail = new int[n];
            int ind = 0;
            while (ind == 0)
            {
                ind = 1;
                bool[] visited = new bool[n];
                trail[0] = first;
                visited[first] = false;
                for (int i = 0; i <= n - 2; i++)
                {
                    int с1 = trail[i];
                    int next = LookForNext(tmp, с1, visited, f, cash_distances, time_distances);

                    if (i == n - 2 && cash_distances[next][trail[0]] == 0)
                        ind = 0;

                    trail[i + 1] = next;
                    visited[next] = true;
                    if (next == first)
                    {
                        for (int j = i + 2; j <= n - 1; j++)
                            trail[j] = -1;
                        return trail;
                    }
                }
            }
            return trail;
        }

        private static int LookForNext(int tmp, int с1, bool[] visited, double[][] f, int[][] cash_distances, int[][] time_distances)
        {
            double[] probabilities = FindMoveProbability(tmp, с1, visited, f, cash_distances, time_distances);

            double[] t = new double[probabilities.Length + 1];
            for (int i = 0; i <= probabilities.Length - 1; i++)
            {
                t[i + 1] = t[i] + probabilities[i];
            }

            Random rnd = new Random();
            double p = rnd.NextDouble();

            for (int i = 0; i <= t.Length - 2; i++)
            {
                if (p >= t[i] && p < t[i + 1])
                {
                    return i;
                }
            }
            throw new Exception("No valid city");
        }

        private static double[] FindMoveProbability(int tmp, int с1, bool[] visited, double[][] f, int[][] cash_distances, int[][] time_distances)
        {
            int n = f.Length;
            double[] t = new double[n];
            double sum = 0.0;
            for (int i = 0; i <= t.Length - 1; i++)
            {
                if (i == с1)
                {
                    t[i] = 0.0;
                }
                else if (visited[i] == true)
                {
                    t[i] = 0.0;
                }
                else if (cash_distances[с1][i] == 0)
                {
                    t[i] = 0.0;
                }
                else
                {
                    double[] dists = new double[2] { cash_distances[с1][i], time_distances[с1][i] };
                    t[i] = Math.Pow(f[с1][i], a) * Math.Pow((1.0 / (dists[0] * dists[1])), b);
                    if (t[i] < 0.0001)
                    {
                        t[i] = 0.0001;
                    }
                    else if (t[i] > (double.MaxValue / (n * 100)))
                    {
                        t[i] = double.MaxValue / (n * 100);
                    }
                }
                sum += t[i];
            }

            double[] probabilities = new double[n];
            for (int i = 0; i <= probabilities.Length - 1; i++)
            {
                probabilities[i] = t[i] / sum;
            }
            return probabilities;
        }

        private static void PheromonesUpdate(double[][] f, int[][] ants, int[][] cash_distances, int[][] time_distances)
        {
            for (int i = 0; i <= f.Length - 1; i++)
            {
                for (int j = 0; j <= f[i].Length - 1; j++)
                {
                    if (j == i || cash_distances[i][j] == 0)
                        continue;
                    for (int tmp = 0; tmp <= ants.Length - 1; tmp++)
                    {
                        double[] length = AntColonyMain.FindLength(ants[tmp], cash_distances, time_distances);
                        double minus = (1.0 - r) * f[i][j];
                        double plus = 0.0;
                        if (IsCityInTrail(i, j, ants[tmp]) == true)
                        {
                            plus = (Q / (length[0] * length[1]));
                        }

                        f[i][j] = minus + plus;

                        if (f[i][j] < 0.0001)
                        {
                            f[i][j] = 0.0001;
                        }
                        else if (f[i][j] > 1000)
                        {
                            f[i][j] = 1000;
                        }

                    }
                }
            }
        }

        private static bool IsCityInTrail(int с1, int с2, int[] trail)
        {
            int lastIndex = trail.Length - 1;
            int id = FindCityindex(trail, с1);

            if (id != -1)
            {
                if (id == 0 && trail[1] == с2)
                {
                    return true;
                }
                else if (id == 0 && trail[lastIndex] == с2)
                {
                    return true;
                }
                else if (id == 0)
                {
                    return false;
                }
                else if (id == lastIndex && trail[0] == с2)
                {
                    return true;
                }
                else if (id == lastIndex)
                {
                    return false;
                }
                else if (id != lastIndex && trail[id + 1] == с2)
                {
                    return true;
                }
                else if (id != lastIndex && trail[id + 1] == -1 && trail[id] == trail[0])
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }
    }
}