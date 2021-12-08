using System;
using System.Collections.Generic;

namespace Minefield
{
    class Field
    {
        public Random rand = new Random();
        public int FieldX;
        public int FieldY;
        public int PlayerX;
        public int PlayerY;
        public int Mines;
        public int CurrentMines;
        public List<List<Cell>> Cells;
        public Field(int x, int y, int mines)
        {
            FieldX = x;
            FieldY = y;
            PlayerX = 0;
            PlayerY = FieldY - 1;
            Mines = mines;
            CurrentMines = mines;
            Cells = new List<List<Cell>>();
            GenerateField();
        }

        public void GenerateField()
        {
            for (int y = 0; y < FieldY; y++)
            {
                Cells.Add(new List<Cell>());
                for (int x = 0; x < FieldX; x++)
                {
                    Cells[y].Add(new Cell(0, true, false, x, y));
                }
            }

            List<int[]> coordinateList = new List<int[]>();

            for (int y = 0; y < FieldY; y++)
            {
                for (int x = 0; x < FieldX; x++)
                {
                    coordinateList.Add(new int[] { x, y });
                }
            }

            List<int[]> minesList = new List<int[]>();

            for (int i = 0; i < Mines; i++)
            {
                int num = rand.Next(0, coordinateList.Count);
                Cells[coordinateList[num][1]][coordinateList[num][0]].Contains = -1;
                minesList.Add(coordinateList[num]);
                coordinateList.RemoveAt(num);
            }

            for (int i = 0; i < minesList.Count; i++)
            {
                Cells[minesList[i][1]][minesList[i][0]].Contains = -1;
                AdjustSurroundingCells(minesList[i][0], minesList[i][1]);
            }
        }
        public void AdjustSurroundingCells(int cellX, int cellY)
        {
            for (int y = cellY - 1; y <= cellY + 1; y++)
            {
                if (y >= 0 && y < FieldY)
                {
                    for (int x = cellX - 1; x <= cellX + 1; x++)
                    {
                        if (x >= 0 && x < FieldX)
                        {
                            if (Cells[y][x].Contains != -1)
                            {
                                Cells[y][x].Contains++;
                            }
                        }
                    }
                }
            }
        }
        public Cell GetPlayerCell()
        {
            return Cells[PlayerY][PlayerX];
        }
        public bool IsCovered(Cell cell)
        {
            return cell.Covered;
        }
        public void UncoverCell(Cell cell)
        {
            cell.Covered = false;
        }
        public void UncoverSurroundingCells(int cellX, int cellY)
        {
            for (int y = cellY - 1; y <= cellY + 1; y++)
            {
                if (y >= 0 && y < FieldY)
                {
                    for (int x = cellX - 1; x <= cellX + 1; x++)
                    {
                        if (x >= 0 && x < FieldX)
                        {
                            if (Cells[y][x].Covered)
                            {
                                UncoverCell(Cells[y][x]);
                                if (Cells[y][x].Contains == 0)
                                {
                                    UncoverSurroundingCells(x, y);
                                }
                            }
                        }
                    }
                }
            }
        }
        public void FlagCell(Cell cell)
        {
            if (cell.Covered)
            {
                if (cell.Flagged)
                {
                    CurrentMines++;
                }
                else
                {
                    CurrentMines--;
                }
                cell.Flagged = !cell.Flagged;
            }
        }
        public void PrintField()
        {
            Console.WriteLine($"Mines Left: {CurrentMines}\n");

            for (int y = 0; y < FieldY; y++)
            {
                string line = "";
                for (int x = 0; x < FieldX; x++)
                {
                    if (!(PlayerX == x && PlayerY == y))
                    {
                        line += GetCellString(Cells[y][x]);
                    }
                    else
                    {
                        Console.Write(line);
                        line = "";
                        line += GetCellString(Cells[y][x]);
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.Write(line[0].ToString());
                        Console.ResetColor();
                        Console.Write(line[1]);
                        line = "";
                    }
                }
                Console.WriteLine(line);
            }
            Console.WriteLine("\n\nUncover: X\nFlag: F");
        }
        public string GetCellString(Cell cell)
        {
            if (cell.Covered)
            {
                if (cell.Flagged)
                {
                    return "! ";
                }
                else
                {
                    return "x ";
                }
            }
            else
            {
                if (cell.Contains > 0)
                {
                    return $"{cell.Contains} ";
                }
                else if (cell.Contains == 0)
                {
                    return "- ";
                }
                else
                {
                    return "* ";
                }
            }
        }
        public void UncoverCells()
        {
            for (int y = 0; y < FieldY; y++)
            {
                for (int x = 0; x < FieldX; x++)
                {
                    UncoverCell(Cells[y][x]);
                }
            }
        }
        public void FlagMines()
        {
            for (int y = 0; y < FieldY; y++)
            {
                for (int x = 0; x < FieldX; x++)
                {
                    if (Cells[y][x].Contains == -1 && Cells[y][x].Flagged == false && Cells[y][x].Covered)
                    {
                        FlagCell(Cells[y][x]);
                    }
                }
            }
        }
        public void RemovePlayer()
        {
            PlayerX = -1;
            PlayerY = -1;
        }
        public bool CheckWin()
        {
            int checkedCells = 0;
            for (int y = 0; y < FieldY; y++)
            {
                for (int x = 0; x < FieldX; x++)
                {
                    if (!Cells[y][x].Covered)
                    {
                        checkedCells++;
                        if (checkedCells + Mines == (FieldX * FieldY))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public void Win()
        {
            Console.Clear();
            UncoverCells();
            FlagMines();
            PlayerX = -1;
            PlayerY = -1;
            CurrentMines = 0;
            Console.WriteLine("You win!\n");
            PrintField();
        }
    }
    class Cell
    {
        public int Contains;
        public bool Covered;
        public bool Flagged;
        public int CellX;
        public int CellY;

        public Cell(int contains, bool covered, bool flagged, int cellX, int cellY)
        {
            Contains = contains;
            Covered = covered;
            Flagged = flagged;
            CellX = cellX;
            CellY = cellY;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            bool run = true;

            int x = 0;
            int y = 0;
            int mines = 0;

            Random random = new Random();
            Console.WriteLine("What size would you like the field to be?");

            Console.Write("x: ");

            bool validInput = false;
            do
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out x) && x > 1)
                {
                    validInput = true;
                }
                else
                {
                    Console.Write("Please enter a number greater than 1 for x: ");
                }
            } while (!validInput);

            Console.Write("y: ");

            validInput = false;
            do
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out y) && y > 1)
                {
                    validInput = true;
                }
                else
                {
                    Console.Write("Please enter a number greater than 1 for y: ");
                }
            } while (!validInput);

            Console.Write("\nHow many mines would you like in your field? ");

            validInput = false;
            do
            {
                string input = Console.ReadLine();
                if (int.TryParse(input, out mines) && mines > 0 && mines < (x * y))
                {
                    validInput = true;
                }
                else
                {
                    Console.Write($"Please enter a number between 1 and {x * y}: ");
                }
            } while (!validInput);

            Console.WriteLine("");

            Field field = new Field(x, y, mines);

            while (run)
            {
                Console.Clear();
                field.PrintField();
                ConsoleKeyInfo input = Console.ReadKey(false);
                if (input.Key == ConsoleKey.X)
                {
                    if (field.IsCovered(field.GetPlayerCell()))
                    {
                        if (field.GetPlayerCell().Contains > 0)
                        {
                            field.UncoverCell(field.GetPlayerCell());
                            if (field.CheckWin())
                            {
                                field.Win();
                                run = false;
                            }
                        }
                        else if (field.GetPlayerCell().Contains == 0)
                        {
                            field.UncoverSurroundingCells(field.PlayerX, field.PlayerY);
                            if (field.CheckWin())
                            {
                                field.Win();
                                run = false;
                            }
                        }
                        else if (field.GetPlayerCell().Contains == -1)
                        {
                            Console.Clear();
                            Console.WriteLine("You lost!\n");
                            field.RemovePlayer();
                            field.UncoverCells();
                            field.PrintField();
                            run = false;
                        }
                    }
                }
                else if (input.Key == ConsoleKey.F)
                {
                    if (field.GetPlayerCell().Covered)
                    {
                        field.FlagCell(field.GetPlayerCell());
                    }
                }
                else if (input.Key == ConsoleKey.UpArrow && field.PlayerY > 0)
                {
                    field.PlayerY--;
                }
                else if (input.Key == ConsoleKey.DownArrow && field.PlayerY < field.FieldY - 1)
                {
                    field.PlayerY++;
                }
                else if (input.Key == ConsoleKey.LeftArrow && field.PlayerX > 0)
                {
                    field.PlayerX--;
                }
                else if (input.Key == ConsoleKey.RightArrow && field.PlayerX < field.FieldX - 1)
                {
                    field.PlayerX++;
                }
            }
        }
    }
}
