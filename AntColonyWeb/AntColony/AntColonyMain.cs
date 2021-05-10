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
        private static int alpha;
        private static int beta;
        private static double rho;
        private static double Q;
        private static int numCities;
        private static int numAnts;
        private static int maxTime;
        private static int Money;
        private static int Time;
        private static int[][] dists_cost;
        private static int[][] dists_time;
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

                Console.WriteLine("Number cities in problem = " + numCities);

                Console.WriteLine("\nNumber ants = " + numAnts);
                Console.WriteLine("Maximum time = " + maxTime);
                Console.WriteLine("Amount of money = " + Money);
                Console.WriteLine("Amount of time = " + Time);

                Console.WriteLine("\nAlpha (pheromone influence) = " + alpha);
                Console.WriteLine("Beta (local node influence) = " + beta);
                Console.WriteLine("Rho (pheromone evaporation coefficient) = " + rho.ToString("F2"));
                Console.WriteLine("Q (pheromone deposit factor) = " + Q.ToString("F2"));

                Console.WriteLine("Final graph of costs:");
                for (int i = 0; i < numCities; i++)
                {
                    Console.Write("{0}: ", i + 1);
                    for (int j = 0; j < numCities; j++)
                    {
                        Console.Write("| " + dists_cost[i][j] + " |");
                    }
                    Console.Write("\n");
                }

                Console.WriteLine("\nFinal graph of time:");
                for (int i = 0; i < numCities; i++)
                {
                    Console.Write("{0}: ", i + 1);
                    for (int j = 0; j < numCities; j++)
                    {
                        Console.Write("| " + dists_time[i][j] + " |");
                    }
                    Console.Write("\n");
                }

                Console.WriteLine("\nFinal graph of values:");
                for (int i = 0; i < numCities; i++)
                {
                    Console.Write("{0}: ", i + 1);
                    Console.Write("| " + values[i] + " |\n");
                }

                Console.WriteLine("\nInitialing ants\n");
                int[][] ants = InitAnts(numAnts, numCities, dists_cost);

                double[] bestLength = new double[2];
                int[] bestTrail = new int[numCities];

                double bestValue = 0;

                Console.WriteLine("\nInitializing pheromones on trails");
                double[][] pheromones = InitPheromones(numCities, dists_cost);

                int time = 0;
                Console.WriteLine("\nEntering UpdateAnts - UpdatePheromones loop\n");
                while (time < maxTime)
                {
                    UpdateAnts(ants, pheromones, dists_cost, dists_time);
                    UpdatePheromones(pheromones, ants, dists_cost, dists_time);

                    int[] currBestTrail = AntColonyMain.BestTrail(ants, dists_cost, dists_time, values);
                    double[] currBestLength = Length(currBestTrail, dists_cost, dists_time);
                    double currBestValue = Values(currBestTrail, values);
                    if ((bestValue < currBestValue && (currBestLength[0] <= Money && currBestLength[1] <= Time)))
                    {
                        bestLength = currBestLength;
                        bestValue = currBestValue;
                        bestTrail = currBestTrail;
                        Console.WriteLine("New best length of: COST: {0}  TIME: {1}  VALUE: {2}    found at time {3}", bestLength[0].ToString("F1"), bestLength[1].ToString("F1"), bestValue, time);
                        Display(bestTrail);
                    }
                    time++;
                }

                Console.WriteLine("\nTime complete");

                Console.WriteLine("\nBest trail found:");
                Console.WriteLine("\nLength of best trail found: COST: {0}  TIME: {1}  VALUE: {2} ", bestLength[0].ToString("F1"), bestLength[1].ToString("F1"), bestValue);
                List<int> trail = Display(bestTrail);
                AntColonyResult result = new AntColonyResult(trail, bestLength[0], bestLength[1], bestValue);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("No solution for this graph...");
                Console.ReadLine();
                return null; 
            }
        }

        // Main

        // --------------------------------------------------------------------------------------------

        private static void FilesInput(AntColonyAlgorithmSetup setup)
        {
            try
            {
                alpha = setup.alpha;
                beta = setup.beta;
                rho = setup.rho;
                Q = setup.Q;
                numCities = setup.n;
                numAnts = setup.m;
                maxTime = setup.iterations;
                Money = setup.max_money;
                Time = setup.max_time;
                if (alpha < 0 || beta < 0 || rho < 0 || Q < 0 || numCities < 2 || numAnts < 1 || maxTime < 1 || Money < 0 || Time < 0)
                    throw new Exception("Один або більше параметрів є невірними");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
                Environment.Exit(0);
            }

            dists_time = MakeGraphDistancesTime(numCities, setup.CT, setup.DT);
            dists_cost = MakeGraphDistancesCost(numCities, setup.CC, setup.DC);
            values = setup.values;
        }

        private static int[][] MakeGraphDistancesCost(int numCities, int[] CC, int[][] DC)
        {
            int[][] costs = new int[numCities][];
            for (int i = 0; i <= costs.Length - 1; i++)
            {
                costs[i] = new int[numCities];
            }

            for (int i = 0; i <= numCities - 1; i++)
            {
                for (int j = 0; j <= numCities - 1; j++)
                {
                    if (DC[i][j] == 0)
                        continue;
                    else
                        costs[i][j] = CC[i] + DC[i][j];
                }
            }
            return costs;
        }

        private static int[][] MakeGraphDistancesTime(int numCities, int[] CT, int[][] DT)
        {
            int[][] times = new int[numCities][];
            for (int i = 0; i <= times.Length - 1; i++)
            {
                times[i] = new int[numCities];
            }

            for (int i = 0; i <= numCities - 1; i++)
            {
                for (int j = 0; j <= numCities - 1; j++)
                {
                    if (DT[i][j] == 0)
                        continue;
                    else
                        times[i][j] = CT[i] + DT[i][j];
                }
            }
            return times;
        }

       /* private static int[] MakeGraphValues(int numCities, string f)
        {
            int[] v = new int[numCities];

            int counter = 0;
            string line;
            string con = folder + "Graph/" + f;
            try
            {
                StreamReader file = new StreamReader(con);
                while ((line = file.ReadLine()) != null)
                {
                    v[counter] = Int32.Parse(line);
                    if (v[counter] < 0)
                    {
                        Console.WriteLine("Один або більше значень у файлах є невірними (< 0)");
                        Console.ReadLine();
                        Environment.Exit(0);
                    }
                    counter++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
                Environment.Exit(0);
            }
            return v;
        }*/

        private static int[][] InitAnts(int numAnts, int numCities, int[][] dists_cost)
        {
            int[][] ants = new int[numAnts][];
            for (int i = 0; i <= ants.Length - 1; i++)
            {
                ants[i] = new int[numAnts];
            }
            return ants;
        }


        private static int IndexOfTarget(int[] trail, int target)
        {
            for (int i = 0; i <= trail.Length - 1; i++)
            {
                if (trail[i] == target)
                {
                    return i;
                }
            }
            return -1;
        }

        private static double[] Length(int[] trail, int[][] dists_cost, int[][] dists_time)
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
                    double[] res = Distance(trail[i], trail[0], dists_cost, dists_time);
                    result[0] += res[0];
                    result[1] += res[1];
                }
                else
                {
                    if (i != trail.Length - 1)
                        if (trail[i + 1] != -1)
                        {
                            double[] res = Distance(trail[i], trail[i + 1], dists_cost, dists_time);
                            result[0] += res[0];
                            result[1] += res[1];
                        }
                }
            }
            return result;
        }

        // -------------------------------------------------------------------------------------------- 

        private static int[] BestTrail(int[][] ants, int[][] dists_cost, int[][] dists_time, int[] values)
        {
            double[] bestLength = Length(ants[0], dists_cost, dists_time);
            double bestVal = Values(ants[0], values);
            int idxBestLength = 0;
            for (int k = 1; k <= ants.Length - 1; k++)
            {
                double[] len = Length(ants[k], dists_cost, dists_time);
                double val = Values(ants[k], values);
                if ((bestVal < val && (len[0] + len[1] < bestLength[0] + bestLength[1])) || (bestVal == val && (len[0] + len[1] < bestLength[0] + bestLength[1])))
                {
                    bestLength = len;
                    bestVal = val;
                    idxBestLength = k;
                }
            }
            int numCities = ants[0].Length;
            int[] bestTrail_Renamed = new int[numCities];
            ants[idxBestLength].CopyTo(bestTrail_Renamed, 0);
            return bestTrail_Renamed;
        }

        // -------------------------------------------------------------------------------------------- 

        private static double Values(int[] trail, int[] values)
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

        // --------------------------------------------------------------------------------------------

        private static double[][] InitPheromones(int numCities, int[][] dists_cost)
        {
            double[][] pheromones = new double[numCities][];
            for (int i = 0; i <= numCities - 1; i++)
            {
                pheromones[i] = new double[numCities];
            }
            for (int i = 0; i <= pheromones.Length - 1; i++)
            {
                for (int j = 0; j <= pheromones[i].Length - 1; j++)
                {
                    if (dists_cost[i][j] == 0)
                        pheromones[i][j] = 0;
                    else
                        pheromones[i][j] = 0.01;
                }
            }
            return pheromones;
        }

        // --------------------------------------------------------------------------------------------

        private static void UpdateAnts(int[][] ants, double[][] pheromones, int[][] dists_cost, int[][] dists_time)
        {
            int numCities = pheromones.Length;
            for (int k = 0; k <= ants.Length - 1; k++)
            {
                Random rnd = new Random();
                int start = rnd.Next(0, numCities);
                //int start = 1; ТУТ МОЖНА ВКАЗАТИ ПОЧАТКОВУ ТОЧКУ!
                int[] newTrail = BuildTrail(k, start, pheromones, dists_cost, dists_time);
                ants[k] = newTrail;
            }
        }

        private static int[] BuildTrail(int k, int start, double[][] pheromones, int[][] dists_cost, int[][] dists_time)
        {
            int numCities = pheromones.Length;
            int[] trail = new int[numCities];
            int ind = 0;
            while (ind == 0)
            {
                ind = 1;
                bool[] visited = new bool[numCities];
                trail[0] = start;
                visited[start] = false;
                for (int i = 0; i <= numCities - 2; i++)
                {
                    int cityX = trail[i];
                    int next = NextCity(k, cityX, visited, pheromones, dists_cost, dists_time);

                    if (i == numCities - 2 && dists_cost[next][trail[0]] == 0)
                        ind = 0;

                    trail[i + 1] = next;
                    visited[next] = true;
                    if (next == start)
                    {
                        for (int j = i + 2; j <= numCities - 1; j++)
                            trail[j] = -1;
                        return trail;
                    }
                }
            }
            return trail;
        }

        private static int NextCity(int k, int cityX, bool[] visited, double[][] pheromones, int[][] dists_cost, int[][] dists_time)
        {
            double[] probs = MoveProbs(k, cityX, visited, pheromones, dists_cost, dists_time);

            double[] cumul = new double[probs.Length + 1];
            for (int i = 0; i <= probs.Length - 1; i++)
            {
                cumul[i + 1] = cumul[i] + probs[i];
            }

            Random rnd = new Random();
            double p = rnd.NextDouble();

            for (int i = 0; i <= cumul.Length - 2; i++)
            {
                if (p >= cumul[i] && p < cumul[i + 1])
                {
                    return i;
                }
            }
            throw new Exception("Failure to return valid city in NextCity");
        }

        private static double[] MoveProbs(int k, int cityX, bool[] visited, double[][] pheromones, int[][] dists_cost, int[][] dists_time)
        {
            int numCities = pheromones.Length;
            double[] taueta = new double[numCities];
            double sum = 0.0;
            // sum of all tauetas
            // i is the adjacent city
            for (int i = 0; i <= taueta.Length - 1; i++)
            {
                if (i == cityX)
                {
                    taueta[i] = 0.0;
                    // prob of moving to self is 0
                }
                else if (visited[i] == true)
                {
                    taueta[i] = 0.0;
                    // prob of moving to a visited city is 0
                }
                else if (dists_cost[cityX][i] == 0)
                {
                    taueta[i] = 0.0;
                    // prob of moving to city, withiut direct pass is 0
                }
                else
                {
                    double[] dists = Distance(cityX, i, dists_cost, dists_time);
                    taueta[i] = Math.Pow(pheromones[cityX][i], alpha) * Math.Pow((1.0 / (dists[0] * dists[1])), beta);
                    // could be huge when pheromone[][] is big
                    if (taueta[i] < 0.0001)
                    {
                        taueta[i] = 0.0001;
                    }
                    else if (taueta[i] > (double.MaxValue / (numCities * 100)))
                    {
                        taueta[i] = double.MaxValue / (numCities * 100);
                    }
                }
                sum += taueta[i];
            }

            double[] probs = new double[numCities];
            for (int i = 0; i <= probs.Length - 1; i++)
            {
                probs[i] = taueta[i] / sum;
                // big trouble if sum = 0.0
            }
            return probs;
        }

        // --------------------------------------------------------------------------------------------

        private static void UpdatePheromones(double[][] pheromones, int[][] ants, int[][] dists_cost, int[][] dists_time)
        {
            for (int i = 0; i <= pheromones.Length - 1; i++)
            {
                for (int j = 0; j <= pheromones[i].Length - 1; j++)
                {
                    if (j == i || dists_cost[i][j] == 0)
                        continue;
                    for (int k = 0; k <= ants.Length - 1; k++)
                    {
                        double[] length = AntColonyMain.Length(ants[k], dists_cost, dists_time);
                        // length of ant k trail
                        double decrease = (1.0 - rho) * pheromones[i][j];
                        double increase = 0.0;
                        if (EdgeInTrail(i, j, ants[k]) == true)
                        {
                            increase = (Q / (length[0] * length[1]));
                        }

                        pheromones[i][j] = decrease + increase;

                        if (pheromones[i][j] < 0.0001)
                        {
                            pheromones[i][j] = 0.0001;
                        }
                        else if (pheromones[i][j] > 100000.0)
                        {
                            pheromones[i][j] = 100000.0;
                        }

                        //pheromones[j][i] = pheromones[i][j];
                    }
                }
            }
        }

        private static bool EdgeInTrail(int cityX, int cityY, int[] trail)
        {
            // are cityX and cityY adjacent to each other in trail[]?
            int lastIndex = trail.Length - 1;
            int idx = IndexOfTarget(trail, cityX);

            if (idx != -1)
            {
                if (idx == 0 && trail[1] == cityY)
                {
                    return true;
                }
                else if (idx == 0 && trail[lastIndex] == cityY)
                {
                    return true;
                }
                else if (idx == 0)
                {
                    return false;
                }
                else if (idx == lastIndex && trail[0] == cityY)
                {
                    return true;
                }
                else if (idx == lastIndex)
                {
                    return false;
                }
                else if (idx != lastIndex && trail[idx + 1] == cityY)
                {
                    return true;
                }
                else if (idx != lastIndex && trail[idx + 1] == -1 && trail[idx] == trail[0])
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


        // --------------------------------------------------------------------------------------------


        private static double[] Distance(int cityX, int cityY, int[][] dists_cost, int[][] dists_time)
        {
            double[] res = new double[2] { dists_cost[cityX][cityY], dists_time[cityX][cityY] };
            return res;
        }

        // --------------------------------------------------------------------------------------------

        private static List<int> Display(int[] trail)
        {
            string result = "";
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
                        //Console.Write(trail[0] + 1);
                        result += trail[0] + 1;
                        res.Add(trail[0]);
                    }
                    else
                    {
                        if (i == trail.Length - 1)
                        {
                            if (trail[i] == trail[0])
                            {
                                //Console.Write(trail[0] + 1);
                                result += trail[0] + 1;
                                res.Add(trail[0]);
                            }
                            else
                            {
                                //Console.Write(trail[i] + 1 + " ---> ");
                                result += trail[i] + 1 + " ---> ";
                                res.Add(trail[i]);
                            }
                        }

                        if (i < trail.Length - 1)
                        {
                            //Console.Write(trail[i] + 1 + " ---> ");
                            result += trail[i] + 1 + " ---> ";
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
                        //Console.Write("");
                        result += "";
                    }
                    else
                    {
                        if (i != trail.Length - 1)
                        {
                            if (trail[i + 1] == -1)
                            {
                                //Console.Write(trail[i] + 1);
                                result += trail[i] + 1;
                                res.Add(trail[i]);
                            }
                            else
                            {
                                //Console.Write(trail[i] + 1 + " ---> ");
                                result += trail[i] + 1 + " ---> ";
                                res.Add(trail[i]);
                            }
                        }
                        else
                        {
                            //Console.Write(trail[i] + 1);
                            result += trail[i] + 1;
                            res.Add(trail[i]);
                        }
                    }
                }
            }
            return res;
            //Console.WriteLine();
        }


        private static void ShowAnts(int[][] ants, int[][] dists_cost, int[][] dists_time)
        {
            for (int i = 0; i <= ants.Length - 1; i++)
            {
                Console.Write(i + ": [ ");

                for (int j = 0; j <= ants[i].Length; j++)
                {
                    if (j == ants[i].Length)
                        Console.Write(ants[i][0] + 1);
                    else
                        Console.Write(ants[i][j] + 1 + " ---> ");
                }

                Console.Write("] len = ");
                double[] len = Length(ants[i], dists_cost, dists_time);
                Console.Write("Cost: {0}  Time: {1} ", len[0].ToString("F1"), len[1].ToString("F1"));
                Console.WriteLine("");
            }
        }

    }
}