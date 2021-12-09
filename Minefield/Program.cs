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
        public List<int[]> MineCoordinates;
        public Field(int x, int y, int mines)
        {
            FieldX = x;
            FieldY = y;
            PlayerX = 0;
            PlayerY = FieldY - 1;
            Mines = mines;
            CurrentMines = mines;
            Cells = new List<List<Cell>>();
            MineCoordinates = new List<int[]>();
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

            for (int i = 0; i < Mines; i++)
            {
                int num = rand.Next(0, coordinateList.Count);
                Cells[coordinateList[num][1]][coordinateList[num][0]].Contains = -1;
                MineCoordinates.Add(coordinateList[num]);
                coordinateList.RemoveAt(num);
            }

            for (int i = 0; i < MineCoordinates.Count; i++)
            {
                Cells[MineCoordinates[i][1]][MineCoordinates[i][0]].Contains = -1;
                FillCellNumbers(MineCoordinates[i][0], MineCoordinates[i][1]);
            }
        } // Generates the positions of the mines in the field and fills in the cells surrounding the mines with numbers
        public void FillCellNumbers(int cellX, int cellY)
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
        } // Takes locations of mines and fills the surrounding cells with numbers
        public Cell GetPlayerCell()
        {
            return Cells[PlayerY][PlayerX];
        } // Returns the current position of the cell the player is on
        public bool IsCovered(Cell cell)
        {
            return cell.Covered;
        } // Returns whether a cell is covered or not
        public bool IsFlagged(Cell cell)
        {
            return cell.Flagged;
        } // Returns whether a cell is flagged or not
        public void UncoverCell(Cell cell)
        {
            cell.Covered = false;
        } // Uncovers a cell
        public void UncoverSurroundingCells(Cell cell)
        {
            for (int y = cell.CellY - 1; y <= cell.CellY + 1; y++)
            {
                if (y >= 0 && y < FieldY)
                {
                    for (int x = cell.CellX - 1; x <= cell.CellX + 1; x++)
                    {
                        if (x >= 0 && x < FieldX)
                        {
                            Cell checkCell = Cells[y][x];
                            if (IsCovered(checkCell))
                            {
                                if (IsFlagged(checkCell))
                                {
                                    FlagCell(checkCell);
                                }
                                UncoverCell(checkCell);
                                if (checkCell.Contains == 0)
                                {
                                    UncoverSurroundingCells(checkCell);
                                }
                            }
                        }
                    }
                }
            }
        } // Recursive method, used to uncover all adjacent empty cells
        public void FlagCell(Cell cell)
        {
            if (IsFlagged(cell))
            {
                CurrentMines++;
            }
            else
            {
                CurrentMines--;
            }
            cell.Flagged = !cell.Flagged;
        } // Flags a cell
        public void PrintField()
        {
            Console.WriteLine($"Mines Left: {CurrentMines}\n");

            for (int y = -1; y < FieldY + 1; y++)
            {
                for (int x = -1; x < FieldX + 1; x++)
                {
                    if (y != -1 && y != FieldY)
                    {
                        if (x != -1 && x != FieldX)
                        {
                            Cell cell = Cells[y][x];
                            if (!(PlayerX == x && PlayerY == y))
                            {
                                string str = GetCellString(cell);
                                if (!IsCovered(cell))
                                {
                                    Console.ForegroundColor = cell.Colors[cell.Contains];
                                    Console.Write(str[0]);
                                    Console.ResetColor();
                                    Console.Write(str[1]);
                                }
                                else
                                {
                                    Console.Write(str);
                                }
                            }
                            else
                            {
                                string str = GetCellString(cell);
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.Write(str[0]);
                                Console.ResetColor();
                                Console.Write(str[1]);
                            }
                        }
                        else
                        {
                            Console.Write(GetBorderString(x, y));
                        }
                    }
                    else
                    {
                        Console.Write(GetBorderString(x, y));
                    }
                }
                Console.WriteLine("");
            }
            Console.WriteLine("\n\nUncover: X\nFlag: F");
        } // Prints the current state of the field, including mines left and user controls
        public string GetCellString(Cell cell)
        {
            if (IsFlagged(cell))
            {
                return "! ";
            }
            if (IsCovered(cell))
            {
                return "x ";
            }
            else
            {
                if (cell.Contains > 0)
                {
                    return $"{cell.Contains} ";
                }
                else if (cell.Contains == 0)
                {
                    return "  ";
                }
                else
                {
                    return "* ";
                }
            }
        } // Returns the string that will display for a cell
        public string GetBorderString(int x, int y)
        {
            if ((x == -1 || x == FieldX) && (y != -1 && y != FieldY))
            {
                return "| ";
            }
            else
            {
                if (x == -1)
                {
                    return "  ";
                }
                else if (x >= 0 && x < FieldX - 1)
                {
                    return "--";
                }
                else if (x == FieldX - 1)
                {
                    return "- ";
                }
                else
                {
                    return "  ";
                }
            }
        } // Returns strings that build the border for the field
        public void UncoverCells(bool won)
        {
            for (int y = 0; y < FieldY; y++)
            {
                for (int x = 0; x < FieldX; x++)
                {
                    Cell cell = Cells[y][x];
                    if (won && IsFlagged(cell))
                    {
                        FlagCell(cell);
                    }
                    else if (IsFlagged(cell) && cell.Contains >= 0)
                    {
                        FlagCell(cell);
                    }
                    UncoverCell(cell);
                }
            }
        } // Uncovers all cells, used only for displaying the entire board after the game is over
        public void FlagMines()
        {
            foreach (int[] coordinates in MineCoordinates)
            {
                int x = coordinates[0];
                int y = coordinates[1];
                FlagCell(Cells[y][x]);
            }
        } // Flags all mines, used only for displaying all mines flagged when the player wins
        public void RemovePlayer()
        {
            PlayerX = -1;
            PlayerY = -1;
        } // Removes the player from the board so no cells are highlighted
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
                    }
                }
            }
            return checkedCells + Mines == (FieldX * FieldY);
        } // Returns whether the win condition has been met
        public void Win()
        {
            Console.Clear();
            UncoverCells(true);
            FlagMines();
            RemovePlayer();
            Console.WriteLine("You win!\n");
            PrintField();
        } // Ends the game with a win screen
        public void Lose()
        {
            Console.Clear();
            Console.WriteLine("You lost!\n");
            RemovePlayer();
            UncoverCells(false);
            PrintField();
        } // Ends the game with a lose screen
    }
    class Cell
    {
        public int Contains;
        public bool Covered;
        public bool Flagged;
        public int CellX;
        public int CellY;
        public Dictionary<int, ConsoleColor> Colors = new Dictionary<int, ConsoleColor>();

        public Cell(int contains, bool covered, bool flagged, int cellX, int cellY)
        {
            Contains = contains;
            Covered = covered;
            Flagged = flagged;
            CellX = cellX;
            CellY = cellY;
            Colors.Add(-1, ConsoleColor.Red);
            Colors.Add(0, ConsoleColor.Black);
            Colors.Add(1, ConsoleColor.Blue);
            Colors.Add(2, ConsoleColor.Green);
            Colors.Add(3, ConsoleColor.DarkRed);
            Colors.Add(4, ConsoleColor.DarkMagenta);
            Colors.Add(5, ConsoleColor.Magenta);
            Colors.Add(6, ConsoleColor.Cyan);
            Colors.Add(7, ConsoleColor.DarkGray);
            Colors.Add(8, ConsoleColor.Gray);
        }

        
    }
    class Program
    {
        static Field CreateFieldFromUserInput()
        {
            int x = 0;
            int y = 0;
            int mines = 0;

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

            return new Field(x, y, mines);
        } // Asks the user for input and creates a Field based off of input
        static bool ControlHandler(Field field)
        {
            ConsoleKeyInfo input = Console.ReadKey(false);

            if (input.Key == ConsoleKey.UpArrow && field.PlayerY > 0)
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
            else if (input.Key == ConsoleKey.X)
            {
                if (!field.IsFlagged(field.GetPlayerCell()) && field.IsCovered(field.GetPlayerCell()))
                {
                    return HandleCellContains(field);
                }
            }
            else if (input.Key == ConsoleKey.F)
            {
                if (field.GetPlayerCell().Covered)
                {
                    field.FlagCell(field.GetPlayerCell());
                }
            }
            return true;
        } // Handles control inputs and returns the result of HandleCellContains
        static bool HandleCellContains(Field field)
        {
            if (field.GetPlayerCell().Contains > 0)
            {
                field.UncoverCell(field.GetPlayerCell());
                if (field.CheckWin())
                {
                    field.Win();
                    return false;
                }
            }
            else if (field.GetPlayerCell().Contains == 0)
            {
                field.UncoverSurroundingCells(field.GetPlayerCell());
                if (field.CheckWin())
                {
                    field.Win();
                    return false;
                }
            }
            else if (field.GetPlayerCell().Contains == -1)
            {
                field.Lose();
                return false;
            }
            return true;
        } // Handles uncover cell action and returns whether the game should continue or not
        static void Main(string[] args)
        {
            Field field = CreateFieldFromUserInput();

            bool run = true;

            while (run)
            {
                Console.Clear();
                field.PrintField();

                run = ControlHandler(field);
            }
        }
    }
}
