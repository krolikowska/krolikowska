using System;
using Priority_Queue;
using System.Collections.Generic;

namespace _15_Królikowska_Zborowska
{
    /// <summary>
    /// Node - klasa zawierająca informacje o jednym nowym węźle w grafie
    /// </summary>
    public class Node : FastPriorityQueueNode, IEquatable<Node> //IEqualityComparer<Node>
    {
        private byte[,] puzzle; //układanka dla danego węzła
        private Node parentNode = null; //referencja do rodzica
        private char? move = null; //określenie jaki "ruch" wygenerował ten układ
        private int cost = 0;
        private int rows = -1;
        private int columns = -1;
        // przechowuje informacje o układzie 
        public byte[,] Puzzle { get => puzzle; set => puzzle = value; }
      
        // jesli to nie root to zawiera odwolanie do rodzica
        public Node ParentNode { get => parentNode; set => parentNode = value; }
        public char? Move { get => move; set => move = value; }
        public int Cost { get => cost; set => cost = value; }
        public int Rows { get => Rows1; set => Rows1 = value; }
        public int Columns { get => Columns1; set => Columns1 = value; }
        public int Rows1 { get => rows; set => rows = value; }
        public int Columns1 { get => columns; set => columns = value; }

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="puzzle">układanka</param>
        /// <param name="parentNode">referencja do rodzica</param>
        public Node(byte[,] puzzle, Node parentNode)
        {
            this.puzzle = puzzle;
            this.parentNode = parentNode;
            Rows1 = puzzle.GetLength(0);
            Columns1 = puzzle.GetLength(1);
        }

        public Node()
        {

        }
        /// <summary>
        /// metoda pozwalająca przesunąć klocek w dół na miejsce pustego klocka
        /// </summary>
        /// <param name="source">układ wejściowy</param>
        /// <param name="zero">okreslenie "pustego miejsca" np 0</param>
        /// <param name="parent">wezel rodzica</param>
        /// <returns type="Node">zwraca nowy stan/węzeł</returns>
        public Node MoveDown(byte[,] source, byte zero, Node parent)
        {
            //w source znajduje sie stan poprzedni
            //w rezult bedzie ukladanka dla nowego węzła, ktory zostanie wygenerowany po operacji przesuniecia
            byte[,] result = new byte[Rows1, Columns1];

            Array.Copy(source, result, source.Length);

            tileIndex emptyTile = FindTile(source, zero); //indeks klocka pustego
            if (emptyTile.row == 0)
            {
                return null;
            }
            else
            {
                byte temp = result[emptyTile.row - 1, emptyTile.column]; //wartosc tego co przesuwamy, na to miejsce wchodzi 0
                result[emptyTile.row - 1, emptyTile.column] = 0;
                result[emptyTile.row, emptyTile.column] = temp; //a tu gdzie było zero wchodzi ten wezel
            }
            return new Node(result, parent);
        }
        /// <summary>
        /// metoda pozwalająca przesunąć klocek w górę na miejsce pustego klocka
        /// </summary>
        /// <param name="source">układ wejściowy</param>
        /// <param name="zero">okreslenie "pustego miejsca" np 0</param>
        /// <param name="parent">wezel rodzica</param>
        /// <returns type="Node">zwraca nowy stan/węzeł</returns>
        public Node MoveUp(byte[,] source, byte zero, Node parent)
        {
            byte[,] result = new byte[Rows1, Columns1];

            Array.Copy(source, result, source.Length);

            tileIndex emptyTile = FindTile(source, zero); //indeks klocka pustego
            if (emptyTile.row == source.GetLength(0) - 1)
            {
                return null;
            }
            else
            {

                byte temp = result[emptyTile.row + 1, emptyTile.column]; //wartosc tego co przesuwamy, na to miejsce wchodzi 0
                result[emptyTile.row + 1, emptyTile.column] = 0;
                result[emptyTile.row, emptyTile.column] = temp; //a tu gdzie było zero wchodzi ten wezel
            }
            return new Node(result, parent);
        }
        /// <summary>
        /// metoda pozwalająca przesunąć klocek w prawo na miejsce pustego klocka
        /// </summary>
        /// <param name="source">układ wejściowy</param>
        /// <param name="zero">okreslenie "pustego miejsca" np 0</param>
        /// <param name="parent">wezel rodzica</param>
        /// <returns type="Node">zwraca nowy stan/węzeł</returns>
        public Node MoveRight(byte[,] source, byte zero, Node parent)
        {
            byte[,] result = new byte[Rows1, Columns1];

            Array.Copy(source, result, source.Length);

            tileIndex emptyTile = FindTile(source, zero); //indeks klocka pustego
            if (emptyTile.column == 0)
            {
                return null;
            }
            else
            {

                byte temp = result[emptyTile.row, emptyTile.column - 1]; //wartosc tego co przesuwamy, na to miejsce wchodzi 0
                result[emptyTile.row, emptyTile.column - 1] = 0;
                result[emptyTile.row, emptyTile.column] = temp; //a tu gdzie było zero wchodzi ten wezel
            }

            return new Node(result, parent);

        }
        /// <summary>
        /// metoda pozwalająca przesunąć klocek w lewo na miejsce pustego klocka
        /// </summary>
        /// <param name="source">układ wejściowy</param>
        /// <param name="zero">okreslenie "pustego miejsca" np 0</param>
        /// <param name="parent">wezel rodzica</param>
        /// <returns type="Node">zwraca nowy stan/węzeł</returns>
        public Node MoveLeft(byte[,] source, byte zero, Node parent)
        {
            byte[,] result = new byte[Rows1, Columns1];

            Array.Copy(source, result, source.Length);

            tileIndex emptyTile = FindTile(source, zero); //indeks klocka pustego
            if (emptyTile.column == source.GetLength(1) - 1)
            {
                return null;
            }
            else
            {

                byte temp = result[emptyTile.row, emptyTile.column + 1]; //wartosc tego co przesuwamy, na to miejsce wchodzi 0
                result[emptyTile.row, emptyTile.column + 1] = 0;
                result[emptyTile.row, emptyTile.column] = temp; //a tu gdzie było zero wchodzi ten wezel
            }
            return new Node(result, parent);
        }


        /// <summary>
        /// metoda pomocnicza do lokalizowania danego klocka np. klocka pustego
        /// </summary>
        /// <param name="source">ukladanka którą przeszukujemy</param>
        /// <param name="zero">poszukiwany klocek</param>
        /// <returns type="tileIndex">zwraca rzad i kolumne gdzie sie znajduje</returns>
        public tileIndex FindTile(byte[,] source, byte zero)
        {
            tileIndex tile = new tileIndex(-1, -1);

            for (int i = 0; i < Rows1; i++)
            {
                for (int j = 0; j < Columns1; j++)
                {
                    if (source[i, j] == zero)
                    {
                        tile = new tileIndex(i, j);

                    }
                }
            }
            return tile;
        }
        /// <summary>
        /// Heurystyka do astara - odległość taksówkowa
        /// </summary>
        /// <param name="node"></param>
        /// <param name="solvedPuzzle"></param>
        /// <returns></returns>
        public int ComputeManhattanDistance(byte[,] solvedPuzzle)
        {
            int distance = 0;
            for (int i = 0; i < Rows1; i++)
            {
                for (int j = 0; j < Columns1; j++)
                {
                    byte tofind = this.puzzle[i, j];
                    tileIndex currentPos = FindTile(this.puzzle, tofind);
                    tileIndex targetPos = FindTile(solvedPuzzle, tofind);
                    distance += (Math.Abs(currentPos.column - targetPos.column) + Math.Abs(currentPos.row - targetPos.row));

                }
            }

            return distance;
        }

        //public int ComputeDiagonalShoortcut(byte[,] solvedPuzzle)
        //{
        //    int xDistance = 0;
        //    int yDistance = 0;
        //    int distance = 0;

        //    for (int i = 0; i < rows; i++)
        //    {
        //        for (int j = 0; j < columns; j++)
        //        {
        //            byte tofind = this.puzzle[i, j];
        //            tileIndex currentPos = FindTile(this.puzzle, tofind);
        //            tileIndex targetPos = FindTile(solvedPuzzle, tofind);
        //            xDistance = Math.Abs(currentPos.column - targetPos.column);
        //            yDistance = Math.Abs(currentPos.row - targetPos.row);

        //            if (xDistance > yDistance)
        //            {
        //                distance += 14 * yDistance + 10 * (xDistance - yDistance);
        //            }
        //            else
        //            {
        //                distance += 14 * xDistance + 10 * (yDistance - xDistance);
        //            }
        //        }
        //    }

        //    return distance;
        //}

        public int ComputeMisplacedTiles(byte[,] solvedPuzzle)
        {
            int distance = 0;
            for (int i = 0; i < Rows1; i++)
            {
                for (int j = 0; j < Columns1; j++)
                {
                    if (this.puzzle[i, j] != solvedPuzzle[i, j])
                        distance++;
                }
            }
            return distance;
        }

        public int countInversion(byte[,] puzzle)
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

        public bool Equals(Node node2)
        {
            bool thesame = true;


            for (int i = 0; i < Rows1; i++)
            {
                for (int j = 0; j < Columns1; j++)
                {
                    if (this.Puzzle[i, j] != node2.Puzzle[i, j])
                        return thesame = false;

                }
            }
            return thesame;
        }

      
        public int GetHashCode(Node obj)
        {
            return Puzzle.GetHashCode();
        }
    }

}
