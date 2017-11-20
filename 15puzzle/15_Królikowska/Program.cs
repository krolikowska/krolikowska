using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Priority_Queue;

namespace _15_Królikowska_Zborowska
{
    class Program
    {
        static void Main(string[] args)
        {
          

            int wybor;
          
            
            string path = @"../../../puzzle.txt";
            byte[,] puzzle = ReadPuzzleFromFile(path);

            do { Console.WriteLine("Choose algorithm" +
                  "\n1.Depth First Search DFS\n" +
                  "\n2.Breadth-first search BFS\n" +
                  "\n3.A* with chosen heuristic \n");

                wybor = int.Parse(Console.ReadLine());

                switch (wybor)
                {
                    case 1:
                        string pathDFS = @"../../../puzzle.txt";
                        byte[,] puzzleDFS = ReadPuzzleFromFile(pathDFS);
                        char[] priorityDFS = SetPriority();
                        SolveDFS(puzzleDFS, priorityDFS);
                        break;
                    case 2:
                        string pathBFS = @"../../../puzzle.txt";
                        byte[,] puzzleBFS = ReadPuzzleFromFile(pathBFS);
                        char[] priorityBFS = SetPriority();
                        SolveBFS(puzzleBFS, priorityBFS);
                        break;
                    case 3:
                        string pathA = @"../../../puzzle.txt";
                        Console.WriteLine("Choose heuristic \n1.manhattan distance heuristic (default) \n2.inversion \n3.misplaced tiles heuristic");
                        int heuristic = int.Parse(Console.ReadLine());
                        byte[,] puzzleA = ReadPuzzleFromFile(pathA);
                        if (heuristic == 2)
                            SolveAstar(puzzleA, 2);
                        else if (heuristic == 3)
                            SolveAstar(puzzleA, 3);
                        else SolveAstar(puzzleA, 1);
                        break;

                } }while(Convert.ToChar(Console.ReadLine()) == 't');





                Console.ReadKey();

        }


        private static char[] SetPriority()
        {
            Console.WriteLine("Please write priority for searching nodes e.g. DGLP and press enter, for random priority type R");
            string prio = Console.ReadLine();
            char[] priority = new char[4] { 'R', 'R', 'R', 'R' };
            prio.ToUpper();
            if (prio.Length == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    priority[i] = prio[i];
                }
                Console.WriteLine("Priority has been set to:{0}", prio);
            }
            else
            {
                Console.WriteLine("Priority has been set to random");
            }

            return priority;
        }

        private static byte[,] ReadPuzzleFromFile(string path)
        {
            string[] file = File.ReadAllLines(path);

            byte[,] puzzle = new byte[file.Length, file.Length];
            for (int i = 0; i < file.Length; i++)
            {

                string[] tmp = file[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < tmp.Length; j++)
                {

                    puzzle[i, j] = byte.Parse(tmp[j]);
                }
            }

            return puzzle;
        }

        /// <summary>
        /// Wywoalnie metod do ulozenia ukladanki - agorytm DFS, wydrukowanie rozwiązania na konsoli
        /// </summary>
        /// <param name="puzzleToSolve">ukladanka do ułożenia</param>
        /// <param name="priority">priorytet przeszukiwań</param>
        private static int SolveDFS(byte[,] puzzleToSolve, char[] priority)
        {
            byte[,] solvedpuzzle = new byte[puzzleToSolve.GetLength(0), puzzleToSolve.GetLength(1)];
            InitPuzzle(solvedpuzzle);
            Nodes allNodes = new Nodes(solvedpuzzle, puzzleToSolve, priority);

            Console.WriteLine("Puzzle to solve: ");
            allNodes.PrintPuzzle(puzzleToSolve);

            if(!allNodes.IsSolvable(puzzleToSolve))
              return -1;

            Console.WriteLine("\nRunning DFS*");

            Thread.Sleep(2000);

            List<char?> solutionDFS = new List<char?>();
            solutionDFS = allNodes.DFS();
            PrintSolution(allNodes, solutionDFS);
            return 1;
        }
        /// <summary>
        /// Wywoalnie metod do ulozenia ukladanki - agorytm BFS, wydrukowanie rozwiązania na konsoli
        /// </summary>
        /// <param name="puzzleToSolve">ukladanka do ułożenia</param>
        /// <param name="priority">priorytet przeszukiwań</param>
        private static int SolveBFS(byte[,] puzzleToSolve, char[] priority)
        {
            byte[,] solvedpuzzle = new byte[puzzleToSolve.GetLength(0), puzzleToSolve.GetLength(1)];
            InitPuzzle(solvedpuzzle);
            Nodes allNodes = new Nodes(solvedpuzzle, puzzleToSolve, priority);

            Console.WriteLine("Puzzle to solve: ");
            allNodes.PrintPuzzle(puzzleToSolve);
            Console.WriteLine("\nRunning BFS*");
            Thread.Sleep(2000);

            if (!allNodes.IsSolvable(puzzleToSolve))
                return -1;


            List<char?> solutionBFS = new List<char?>();
            solutionBFS = allNodes.BFS();
            PrintSolution(allNodes, solutionBFS);

            return 1;
        }

        private static int SolveAstar(byte[,] puzzleToSolve, int heuristicID)
        {
            byte[,] solvedpuzzle = new byte[puzzleToSolve.GetLength(0), puzzleToSolve.GetLength(1)];
            InitPuzzle(solvedpuzzle);
            Nodes allNodes = new Nodes(solvedpuzzle, puzzleToSolve);
            Console.WriteLine("Puzzle to solve: ");
            allNodes.PrintPuzzle(puzzleToSolve);


            if (!allNodes.IsSolvable(puzzleToSolve))
                return -1;

            Console.WriteLine("\nRunning A*");
            Thread.Sleep(2000);

            List<char?> solutionA = new List<char?>();
            solutionA = allNodes.Astar(heuristicID);
            PrintSolution(allNodes, solutionA);
            return 1;
        }
        /// <summary>
        /// metoda pomocnicza do wyświetlenia poszczególnych etapów ułożenia układanki
        /// </summary>
        /// <param name="allNodes">na rzecz tego obiektu wywolujemy metode wyswietlajaca etapy ukladania</param>
        /// <param name="solutionPath">lista przechowujaca poszczegolne kroki rozwiazania</param>
        private static void PrintSolution(Nodes allNodes, List<char?> solutionPath)
        {
            List<string> solution = new List<string>();
            if (solutionPath == null)
            {
                Console.WriteLine("algorytm nie znalazł rozwiązania");
                Console.WriteLine(allNodes.VisitedNodesNumber);
                Console.ReadLine();
            }

            Console.WriteLine("Ilosc kroków rozwiązania algorytmu: {0}", solutionPath.Count);
            foreach (var i in solutionPath)
            {
                Console.Write(i);
                solution.Add(i.ToString());
            }

            File.WriteAllLines(@"../../solutionpath.txt", solution);
            Console.WriteLine();
            Console.WriteLine($"Odwiedzono: {allNodes.VisitedNodesNumber} węzłów");
            Console.ReadLine();
            allNodes.PrintSolution(allNodes.StartPuzzle, solutionPath.ToArray<char?>());
        }
        /// <summary>
        /// metoda pomocnicza, dla danego rozmiaru układanki generuje pożadane układ - rozwiązanie postaci np {1,2,3,4,5,6,7,8,0}
        /// </summary>
        /// <param name="puzzle"></param>
        static void InitPuzzle(byte[,] puzzle)
        {
            byte k = 1;

            for (int i = 0; i < puzzle.GetLength(0); i++)
            {
                for (int j = 0; j < puzzle.GetLength(1); j++)
                {
                    puzzle[i, j] = k;
                    k++;

                    if (i == puzzle.GetLength(0) - 1 && j == puzzle.GetLength(1) - 1)
                        puzzle[i, j] = 0;
                }

            }


        }
    }
}
