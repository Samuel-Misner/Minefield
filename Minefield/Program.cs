using System;
using System.Collections.Generic;

namespace Minefield
{
    class Field
    {
        public Random rand = new Random();
        public int FieldX;
        public int FieldY;
        public int Mines;
        public int CurrentMines;
        public List<List<Cell>> Cells;
        public Field(int x, int y, List<List<Cell>> cells, int mines)
        {
            FieldX = x;
            FieldY = y;
            Cells = cells;
            Mines = mines;
            CurrentMines = mines;
            GenerateField();
        }

        public void Flag(Cell cell)
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

        public void UncoverCells()
        {
            for (int y = 0; y < FieldY; y++)
            {
                for (int x = 0; x < FieldX; x++)
                {
                    Cells[y][x].Covered = false;
                }
            }
        }
        public void PrintField()
        {
            Console.WriteLine($"Mines Left: {CurrentMines}\n");

            List<List<Cell>> cells = Cells;

            int fieldY = cells.Count;
            int fieldX = cells[0].Count;

            for (int y = 0; y < fieldY; y++)
            {
                string line = "";
                for (int x = 0; x < fieldX; x++)
                {
                    if (cells[y][x].Covered)
                    {
                        if (cells[y][x].Flagged)
                        {
                            line += "! ";
                        }
                        else
                        {
                            line += "x ";
                        }
                    }
                    else
                    {
                        if (cells[y][x].Contains > 0)
                        {
                            line += $"{cells[y][x].Contains} ";
                        }
                        else if (cells[y][x].Contains == 0)
                        {
                            line += "- ";
                        }
                        else if (cells[y][x].Contains == -1)
                        {
                            line += "* ";
                        }
                    }
                }
                Console.WriteLine(line);
            }
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
                                Cells[y][x].Covered = false;
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

            for (int i = 0; i < Mines; i++)
            {
                int num = rand.Next(0, coordinateList.Count);
                Cells[coordinateList[num][1]][coordinateList[num][0]].Contains = -1;
                coordinateList.RemoveAt(num);
            }

            for (int y = 0; y < FieldY; y++)
            {
                for (int x = 0; x < FieldX; x++)
                {
                    if (Cells[y][x].Contains == -1)
                    {
                        AdjustSurroundingCells(x, y);
                    }
                }
            }
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
        public void FlagMines()
        {
            for (int y = 0; y < FieldY; y++)
            {
                for (int x = 0; x < FieldX; x++)
                {
                    if (Cells[y][x].Contains == -1 && Cells[y][x].Flagged == false && Cells[y][x].Covered)
                    {
                        Flag(Cells[y][x]);
                    }
                }
            }
        }
        public void Win()
        {
            Console.Clear();
            FlagMines();
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
        static void PrintOptions(List<string> options)
        {
            for (int i = 0; i < options.Count; i++)
            {
                Console.WriteLine($"{(i + 1) + ".", 2} {options[i]}");
            }
        }
        static void Main(string[] args)
        {
            bool run = true;

            int x = 0;
            int y = 0;
            int mines = 0;

            Random random = new Random();
            Console.WriteLine("What size would you like the grid to be?");

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
                    Console.Write("Please enter a number greater than 1 for x: ");
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

            List<List<Cell>> cells = new List<List<Cell>>();

            Console.WriteLine("");

            Field field = new Field(x, y, cells, mines);

            while (run)
            {
                Console.Clear();
                field.PrintField();

                Console.WriteLine("\nWould you like to uncover or flag a cell?\n");

                PrintOptions(new List<string>() { "Uncover", "Flag" });

                int userInt = 0;

                validInput = false;
                do
                {
                    string input = Console.ReadLine();
                    if (int.TryParse(input, out userInt) && userInt > 0 && userInt <= 2)
                    {
                        validInput = true;
                    }
                    else
                    {
                        Console.Write("Please enter a valid integer between 1 and 2: ");
                    }
                } while (!validInput);

                int userX = 0;
                int userY = 0;

                Console.Write("Please enter coordinates on the grid (x y): ");
                validInput = false;
                do
                {
                    string input = Console.ReadLine();
                    string[] coordinates = input.Split(" ");
                    if (coordinates.Length == 2 && int.TryParse(coordinates[0], out userX) && userX > 0 && userX <= field.FieldX && int.TryParse(coordinates[1], out userY) && userY > 0 && userY <= field.FieldY)
                    {
                        validInput = true;
                    }
                    else
                    {
                        Console.Write("Please enter coordinates on the grid (x y): ");
                    }
                } while (!validInput);

                userX--;
                userY--;
                userY = (y - 1) - userY;

                if (userInt == 1)
                {
                    if (!field.Cells[userY][userX].Flagged)
                    {
                        if (field.Cells[userY][userX].Contains == -1)
                        {
                            Console.Clear();
                            Console.WriteLine("You lost!\n");
                            field.UncoverCells();
                            field.PrintField();
                            run = false;
                        }
                        else if (field.Cells[userY][userX].Contains > 0)
                        {
                            field.Cells[userY][userX].Covered = false;
                            if (field.CheckWin())
                            {
                                field.Win();
                                run = false;
                            }
                        }
                        else if (field.Cells[userY][userX].Contains == 0)
                        {
                            field.UncoverSurroundingCells(userX, userY);
                            if (field.CheckWin())
                            {
                                field.Win();
                                run = false;
                            }
                        }
                    }
                }
                else if (userInt == 2)
                {
                    field.Flag(field.Cells[userY][userX]);
                }
            }
        }
    }
}
