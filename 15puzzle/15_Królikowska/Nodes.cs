using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Priority_Queue;

namespace _15_Królikowska_Zborowska
{
    /// <summary>
    /// pomocnicza struktra okreslajaca polzenie pustego klocka
    /// </summary>
    public struct tileIndex
    {

        public int row;
        public int column;

        public tileIndex(int row, int column)
        {
            this.row = row;
            this.column = column;
        }
    }
    public class Nodes
    {
        /// <summary>
        /// Klasa przechowująca informacje o danym węźle grafu 
        /// </summary>

        private char[] priority = new char[4] { 'L', 'G', 'P', 'D' }; ///domyślny priorytet, przekazujmy wlasciwy w konstr.
        private byte[,] targetPuzzle; ///ulozona układanka
        private byte[,] startPuzzle;
        private LinkedList<Node> children = new LinkedList<Node>(); ///lista do przechowywania potomnych stanow układanki
        private long visitedNodesNumber;

        public long VisitedNodesNumber { get => visitedNodesNumber; set => visitedNodesNumber = value; }
        public byte[,] StartPuzzle { get => startPuzzle; set => startPuzzle = value; }

        ///liczba odwiedzonych stanów
        public Nodes(byte[,] solvedPuzzle, byte[,] startPuzzle, char[] priority)
        {
            this.StartPuzzle = startPuzzle;
            this.priority = priority;
            this.targetPuzzle = solvedPuzzle;
        }

        public Nodes(byte[,] solvedPuzzle, byte[,] startPuzzle)
        {
            this.StartPuzzle = startPuzzle;
            this.priority = new char[4] { 'L', 'G', 'P', 'D' };
            this.targetPuzzle = solvedPuzzle;
        }

        /// <summary>
        /// Metoda pozwalajaca wygenerowac pochodne stany, powstale po przesunieciu klocka
        /// </summary>
        /// <param name="parent"> wezel rodzic dla ktorego generowane sa pochodne stany</param>
        /// <returns type="List<Node>">zwraca listę wygenerowanych stanów</returns>
        public LinkedList<Node> GenerateChildren(Node parent)
        {
            LinkedList<Node> children = new LinkedList<Node>();
            char? current = null; ///aktualny "ruch" do wykonania
            children.Clear();

            byte[,] parentPuzzle = new byte[parent.Rows, parent.Columns]; ///ukladanka z wezla Parent
            Array.Copy(parent.Puzzle, parentPuzzle, parentPuzzle.Length); //kopiuemy zeby nie zmieniac oryginalnej w wezle

            if (priority[0] == 'R')
            {
                priority = new char[4] { 'L', 'G', 'P', 'D' };
                Shuffle(priority);
            }


            for (int i = 0; i < priority.Length; i++)
            {
                current = priority[i]; //aktualny ruch z priorytetu
                Node node;

                switch (current)
                {
                    case 'L':

                        node = parent.MoveLeft(parentPuzzle, 0, parent); //generujemy ruch w lewo, jeśli niemożliwy, to zwróci null
                        if (node != null && parent.Move != 'P') //sprawdzamy czy nie wraca na miejsce z ktorego go przesnęliśmy
                        {
                            node.Move = ('L'); //zaznaczamy jaki ruch wygenerował powstanie tego układu
                            children.AddLast(node); //dodajemy do listy dzieci

                        }
                        break;
                    case 'P':

                        node = parent.MoveRight(parentPuzzle, 0, parent);
                        if (node != null && parent.Move != 'L')
                        {
                            node.Move = ('P');
                            children.AddLast(node);
                        }
                        break;
                    case 'G':
                        node = parent.MoveUp(parentPuzzle, 0, parent);
                        if (node != null && parent.Move != 'D')
                        {
                            node.Move = ('G');
                            children.AddLast(node);
                        }
                        break;
                    case 'D':
                        node = parent.MoveDown(parentPuzzle, 0, parent);
                        if (node != null && parent.Move != 'G')
                        {
                            node.Move = 'D';
                            children.AddLast(node);
                        }
                        break;
                    default:
                        break;
                }
            }

            return children;
        }

        ///// <summary>
        ///// Metoda sprawdzajaca czy układanka jest taka sama jak rozwiązanie które chcemy mieć
        ///// </summary>
        ///// <param name="current">węzeł dla którego sprawdzamy układankę</param>
        ///// <returns type="bool">zwraca true jeśli znaleźliśmy rozwiązanie</returns>
        //public bool IsSolved(Node current)
        //{
        //    Node final = new Node(targetPuzzle, null);
        //    if (current.Equals(final))
        //        return true;
        //    else return false;

        //}


        /// <summary>
        /// algorytm przeszukiwania wszerz BFS
        /// </summary>
        /// <returns type=" List<char?> ">sciezka ruchow jakie nalezy wykonac zbey ulozyc ukladanke</returns>
        public List<char?> BFS()
        {
            Stopwatch sw = Stopwatch.StartNew(); //do monitorowana czasu trwania algorytmu


            Queue<Node> queue = new Queue<Node>(); ///umieszczamy wierzcholki do odwiedzenia w kolejce
            byte[,] rootPuzzle = new byte[StartPuzzle.GetLength(0), StartPuzzle.GetLength(1)];

            Array.Copy(StartPuzzle, rootPuzzle, StartPuzzle.Length); //kopiujemy nieulozona ukladanke
                                                                     //  List<Node> visitedNodes = new List<Node>(); ///list odwiedzonych wezlow
            HashSet<Node> visitedNodes = new HashSet<Node>();

            List<char?> moves = new List<char?>(); ///lista ruchow jakie prowadza do rozwiazana
            List<Node> parentlist = new List<Node>(); ///lista rodzicow, "skad przyszlismy do tego stanu"

            children.Clear();
            VisitedNodesNumber = 0;


            Node final = new Node(targetPuzzle, null); //z tym bedziemy wezlem bedziemy porownywac
            Node rootNode = new Node(rootPuzzle, null); //tworzymy korzen
            rootNode.Move = 'X'; //ustawiamy jego ruch na X 

            queue.Enqueue(rootNode); //wrzucamy do kolejki 

            while (queue.Count != 0) //dopóki kolejka nie jest pusta
            {
                Node nodeFromQueue = queue.Dequeue(); //pobieramy pierwszy element z kolejki
                visitedNodes.Add(nodeFromQueue); //dodajemy do listy odwiedzonych wezlow
                VisitedNodesNumber = visitedNodes.Count;

                this.children = GenerateChildren(nodeFromQueue); //dla pobranego z kolejki tworzymy pochodne stany, przesuwamy klocki

                for (int i = 0; i < children.Count; i++) //sprawdzamy w petli czy ktores z dzieci jest odwiedzone, jesli tak to je usuwamy
                {
                    Node temp = children.ElementAt(i);
                    if (visitedNodes.Contains(temp))
                    {
                        children.Remove(temp);

                    }
                }


                if (children.Count != 0)
                {
                    foreach (Node childNode in children)
                    {
                        queue.Enqueue(childNode); //wrzucamy na koniec kolejki do odwiedzenia

                        if (childNode.Equals(final)) //sprawdzamy czy znalezlismy rozwiazanie
                        {
                            visitedNodes.Add(childNode);
                            MakeSolutionPath(moves, parentlist, childNode); //dla danego wezla sprawdzamy jego wszystkich rodzicow i zapisujemy ich "ruch" node.Move do listy ruchow moves
                            sw.Stop();
                            Console.WriteLine("BFS Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
                            return moves;

                        }
                    }
                }
            }
            sw.Stop();

            Console.WriteLine("BFS Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
            Console.ReadLine();
            return null;
        }
        /// <summary>
        /// Metoda pozwalająca na pobranie zapisanych ruchów ze wszystkich węzłów prowadzących do rozwiązania
        /// </summary>
        /// <param name="moves">lista ruchów do której dodajemy rozwiązanie</param>
        /// <param name="parentlist">lista przechowująca wszystkich rodziców dla węzła będącego rozwiązaniem</param>
        /// <param name="childNode">węzeł dla którego poszukujemy ścieżki</param>
        private void MakeSolutionPath(List<char?> moves, List<Node> parentlist, Node childNode)
        {
            FetchAllParents(childNode, parentlist); //rekurencyjnie pobieramy rodziców
            foreach (var n in parentlist)
            {
                if (n.Move != 'X')
                {
                    moves.Add(n.Move);
                }
            }
            moves.Reverse(); //odwracamy bo dodawane są od końca ruchy
        }

        /// <summary>
        /// Metoda do rekurencyjnego odwołana się do wszystkich rodziców dla danego węzła
        /// </summary>
        /// <param name="node">węzeł dla którego chcemy znalezc sciezka</param>
        /// <param name="parentlist">lista rodzicow</param>
        public void FetchAllParents(Node node, List<Node> parentlist)
        {
            while (node.ParentNode != null)
            {
                parentlist.Add(node);
                node = node.ParentNode;
            }
        }

        /// <summary>
        /// algorytm DFS przeszukiwania w głąb
        /// priorytet jest zapisany we właściwościach Nodes
        /// </summary>
        /// <returns> zwraca ciag ruchow potrzebnych do ulozenia</returns>
        public List<char?> DFS()
        {
            Stopwatch sw = Stopwatch.StartNew(); //do monitorowana czasu trwania algorytmu

            Stack<Node> stack = new Stack<Node>(); //umieszczamy wierzcholki do odwiedzenia
            byte[,] rootPuzzle = new byte[StartPuzzle.GetLength(0), StartPuzzle.GetLength(1)]; //od tego zaczynami 
            Array.Copy(StartPuzzle, rootPuzzle, StartPuzzle.Length); //kopiujemy tablice, wszystkie tab to ref parametr
            //List<Node> visitedNodes = new List<Node>(); //lista z odwiedzonymi wezlami ----hash set 
            HashSet<Node> visitedNodes = new HashSet<Node>();

            List<char?> moves = new List<char?>(); // zapisujemy jakie ruchy wykonalismy zeby dojsc do rozwiazania
            List<Node> parentlist = new List<Node>(); ///lista rodzicow, "skad przyszlismy do tego stanu"


            children.Clear(); //korzystamy z wlasciwosci zdefiniowanej w nodes wiec na starcie trzeba "wyczyscic"
            VisitedNodesNumber = 0;

            Node final = new Node(targetPuzzle, null); //reprezentuje ulozona ukladanke
            Node rootNode = new Node(rootPuzzle, null); //tworzymy korzen
            rootNode.Move = 'X';
            stack.Push(rootNode); //wrzucamy element glowny na stos

            while (stack.Count != 0) //dopóki coś jest na stosie
            {

                Node nodeFromStack = stack.Pop(); //pobieramy węzeł ze stosu
                                                  //  if (nodeFromStack.Move != 'X') // root nie zapisanej informacji jakim jest ruchem, wiec dodalismy X, nei chcemy tego na liscie

                visitedNodes.Add(nodeFromStack); //dodajemy do odwiedzonych węzłów 
                this.children = GenerateChildren(nodeFromStack); //tworzymy dzieci - kolejne ruchy wg priorytetu

                for (int i = 0; i < children.Count; i++) //sprawdzamy w petli czy ktores z dzieci jest odwiedzone, jesli tak to je usuwamy
                {
                    Node temp = children.ElementAt(i);
                    if (IsVisited(temp,visitedNodes))
                    {
                        children.Remove(temp);
                    }
                }

                VisitedNodesNumber = visitedNodes.LongCount();

                if (children.Count != 0)
                {
                    children.Reverse(); //odwracamy kolejność na liście, tak żeby odkładać na stosie zgodnie z priorytetem 
                    foreach (Node n in children) //jesli sa nieodwiedzone to wszystkie je umieszczamy na stosie
                    {
                        
                            stack.Push(n);  //to wrzuc go na stos do odwiedzenia
                      
                        if (n.Equals(final)) //sprawdzamy czy znaleźliśmy rozwiązanie
                        {
                            MakeSolutionPath(moves, parentlist, n); //dla danego wezla sprawdzamy jego wszystkich rodzicow i zapisujemy ich "ruch" node.Move do listy ruchow moves
                            sw.Stop();
                            Console.WriteLine("DFS Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
                            return moves;
                        }

                    }
                }

            }
            Console.ReadLine();
            return null; //jesli nie znajdziemy rozwiazania to null
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="idHeuristic"> 1. manhattan, cos innego position</param>
        /// <returns></returns>
        public List<char?> Astar(int idHeuristic)
        {
            Stopwatch sw = Stopwatch.StartNew(); //do monitorowana czasu trwania algorytmu

            //  FastPriorityQueue<Node> queue = new FastPriorityQueue<Node>(100000000);
            SimplePriorityQueue<Node> queue = new SimplePriorityQueue<Node>();

            byte[,] rootPuzzle = new byte[StartPuzzle.GetLength(0), StartPuzzle.GetLength(1)]; //od tego zaczynami 
            Array.Copy(StartPuzzle, rootPuzzle, StartPuzzle.Length); //kopiujemy tablice, wszystkie tab to ref parametr

            HashSet<Node> visitedNodes = new HashSet<Node>();
            List<char?> moves = new List<char?>(); // zapisujemy jakie ruchy wykonalismy zeby dojsc do rozwiazania
            List<Node> parentlist = new List<Node>(); ///lista rodzicow, "skad przyszlismy do tego stanu"
           

            children.Clear(); //korzystamy z wlasciwosci zdefiniowanej w nodes wiec na starcie trzeba "wyczyscic"
            VisitedNodesNumber = 0;


            Node final = new Node(targetPuzzle, null); //tworzymy wezel z ulozona 
            Node rootNode = new Node(rootPuzzle, null); //tworzymy korzen
            rootNode.Move = 'X'; //ustawiamy jego ruch na X 

            queue.Enqueue(rootNode, rootNode.ComputeManhattanDistance(targetPuzzle)); //wrzucamy do kolejki 

            while (queue.Count != 0) //dopóki kolejka nie jest pusta
            {
                Node nodeCurrent = queue.Dequeue(); //pobieramy pierwszy element z kolejki

                if (nodeCurrent.Equals(final))
                {
                    parentlist.Clear();
                    MakeSolutionPath(moves, parentlist, nodeCurrent); //dla danego wezla sprawdzamy jego wszystkich rodzicow i zapisujemy ich "ruch" node.Move do listy ruchow moves
                    sw.Stop();
                    Console.WriteLine("A* Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
                    return moves;

                }
                visitedNodes.Add(nodeCurrent); //dodajemy do listy odwiedzonych wezlow -->to bedzie ClosedList
                VisitedNodesNumber = visitedNodes.Count;

                this.children = GenerateChildren(nodeCurrent); //dla pobranego z kolejki tworzymy pochodne stany, przesuwamy klocki

                if (children.Count != 0)
                {
                    foreach (Node child in children)
                    {
                        if (!visitedNodes.Contains(child))
                        {
                            FetchAllParents(child, parentlist);

                            if (idHeuristic == 1 || idHeuristic > 3)
                                child.Cost = parentlist.Count + child.ComputeManhattanDistance(targetPuzzle); //koszt dotarcia do tego miejsca + odleglosc tego punktu od ulozonego stanu
                            else if (idHeuristic == 2)
                                child.Cost = parentlist.Count + child.countInversion(targetPuzzle); //koszt dotarcia do tego miejsca + odleglosc tego punktu od ulozonego stanu
                            else if (idHeuristic == 3)
                                child.Cost = parentlist.Count + child.ComputeMisplacedTiles(targetPuzzle); //koszt dotarcia do tego miejsca + odleglosc tego punktu od ulozonego stanu

                            //jesli jeszcze nie byl odwiedzony to wrzucamy do kolejki priorytetowej
                            parentlist.Clear();
                            queue.Enqueue(child, child.Cost);
                        }
                    }

                }

            }
            sw.Stop();

            Console.WriteLine("A* Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
            Console.ReadLine();
            return null;
        }

        public bool IsVisited(Node node, HashSet<Node> vsitedNodes)
        {
            bool isVisited = false;

            foreach (var n in vsitedNodes)
            {
                if (n.Equals(node))
                    return isVisited = true;
            }
            
            return isVisited;
        }

        /// <summary>
        /// metoda do wyświetlania sposobu rozwiązania układanki na konsoli 
        /// </summary>
        /// <param name="source"> układ startowy, nieułożony </param>
        /// <param name="solution"> sciezka ruchow jakie nalezy wykonać żeby ułożyć układankę</param>
        public void PrintSolution(byte[,] source, char?[] solution)
        {
            byte[,] result = new byte[source.GetLength(0), source.GetLength(1)];
            Array.Copy(source, result, source.Length); //kopiujemy tablice, pracujemy na kopii
            char? current; //aktualny ruch do wykonania
            tileIndex emptyTile = new tileIndex(-1, -1); //nieustawione położenie klocka "0", pustego miejsca


            //odszukujemy na jakiej pozycji jest "pusty klocek"
            for (int i = 0; i < source.GetLength(0); i++)
            {
                for (int j = 0; j < source.GetLength(1); j++)
                {
                    if (source[i, j] == 0)
                    {
                        emptyTile = new tileIndex(i, j);
                    }
                }
            }
            Thread.Sleep(2000); //zatrzymujemy program 

            for (int i = 0; i < solution.Length; i++)
            {

                int milliseconds = 300;

                Console.Clear();
                current = solution[i];  //aktualny ruch do wykonania pobieramy z rozwiazania
                byte temp;

                switch (current)
                {

                    case 'L':

                        temp = result[emptyTile.row, emptyTile.column + 1]; //wartosc klocka który przesuwamy 
                        result[emptyTile.row, emptyTile.column + 1] = 0;    //w miejsce klocka ktory przuneliśmy wchodzi "pusty klocek"
                        result[emptyTile.row, emptyTile.column] = temp; //a tu gdie był pusty klocek, wchodzi klocek przesunięty
                        emptyTile.column += 1; //aktualizujemy położenie pustego klocka

                        PrintPuzzle(result); //wyswietlamy układankę przez czas milliseconds, czyscimy ekran
                        Thread.Sleep(milliseconds);

                        if (i != solution.Length - 1)
                            Console.Clear();
                        break;
                    case 'P':
                        temp = result[emptyTile.row, emptyTile.column - 1];
                        result[emptyTile.row, emptyTile.column - 1] = 0;
                        result[emptyTile.row, emptyTile.column] = temp;
                        emptyTile.column -= 1;
                        PrintPuzzle(result);

                        Thread.Sleep(milliseconds);
                        if (i != solution.Length - 1)
                            Console.Clear();
                        break;
                    case 'G':
                        temp = result[emptyTile.row + 1, emptyTile.column];
                        result[emptyTile.row + 1, emptyTile.column] = 0;
                        result[emptyTile.row, emptyTile.column] = temp;
                        emptyTile.row += 1;
                        PrintPuzzle(result);

                        Thread.Sleep(milliseconds);
                        if (i != solution.Length - 1)
                            Console.Clear();
                        break;
                    case 'D':
                        temp = result[emptyTile.row - 1, emptyTile.column];
                        result[emptyTile.row - 1, emptyTile.column] = 0;
                        result[emptyTile.row, emptyTile.column] = temp;
                        emptyTile.row -= 1;
                        PrintPuzzle(result);
                        Thread.Sleep(milliseconds);
                        if (i != solution.Length - 1)
                            Console.Clear();
                        break;
                    default:
                        break;
                }
            }


        }

        /// <summary>
        /// Moteda do wyswietlenia ukladanki na konsoli
        /// </summary>
        /// <param name="puzzle"> ukladanka do wyswietlenia </param>
        public void PrintPuzzle(byte[,] puzzle)
        {

            for (int i = 0; i < puzzle.GetLength(0); i++)
            {
                for (int j = 0; j < puzzle.GetLength(1); j++)
                {
                    if (puzzle[i, j] == 0) //zamiast zera wyswielamy "puste miejsc"
                    { Console.Write($"   "); }
                    else if (puzzle[i, j] < 10)
                    { Console.Write($" {puzzle[i, j]} "); }
                    else
                        Console.Write($"{puzzle[i, j]} ");

                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        /// <summary>
        /// metoda pomocnicza do mieszania sekwencji priorytetu
        /// </summary>
        /// <param name="sequence"></param>
        private void Shuffle(char[] sequence)
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            for (int i = 0; i < sequence.Length; ++i)
            {

                int r = rnd.Next(i, sequence.Length);
                char tmp = sequence[r];
                sequence[r] = sequence[i];
                sequence[i] = tmp;
            }
        }

        public bool isOdd(int num)
        {
            if (num % 2 == 1)
                return true;
            else return false;
        }
        public bool isEven(int num)
        {
            if (num % 2 == 0)
                return true;
            else return false;
        }
        public bool IsSolvable(byte[,] puzzle)
        {

            if (puzzle.GetLength(0) == puzzle.GetLength(1)) //tylko dla kwadratowych
            {
                int inversionCount = countInversion(puzzle);
                int n = puzzle.GetLength(0);

                if (isOdd(n) && isEven(inversionCount))
                {
                    Console.WriteLine("Is solvable");
                    return true;
                }

                else if (isEven(n) && isOdd(positionOfromBottom(puzzle)) && isEven(inversionCount))
                {
                    Console.WriteLine("Is solvable");
                    return true;
                }
                else if (isEven(n) && isEven(positionOfromBottom(puzzle)) && isEven(inversionCount))
                {
                    Console.WriteLine("Is unsolvable");
                    return false;
                }
                else
                {
                    Console.WriteLine("Is solvable");
                    return true;
                }

            }

            Console.WriteLine("Can check solvability only for square puzzles");
            return true; //bo mimo wszystko chcemy sprawzac nawet niekwadratowe
        }
        private int positionOfromBottom(byte[,] puzzle)
        {
            int n = puzzle.GetLength(0); //ile ma wierszy, kolumn 
            Node node = new Node(puzzle, null);
            tileIndex tile = node.FindTile(puzzle, 0);

            return n - tile.row;
        }
        private int countInversion(byte[,] puzzle)
        {
            List<byte> check = new List<byte>();
            foreach (var p in puzzle)
            {

                check.Add(p);
            }
            int inwers = 0;
            for (int j = 0; j < check.Count - 1; j++)
            {

                for (int i = j; i < check.Count - 1; i++)
                {
                    if (check[j] != 0 && check[i + 1] != 0)
                        if (check[j] > check[i + 1])
                            inwers++;
                }
            }

            return inwers;
        }

    }

}
