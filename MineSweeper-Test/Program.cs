using System;

namespace MineSweeper
{
    internal class Program
    {
        // Define Variables
        static int width = 8;
        static int height = 8;
        static int numMines = 10;
        static char[,] board = new char[height, width];   // Represents the visible board
        static bool[,] mines = new bool[height, width];   // Tracks where the mines are
        static bool[,] isRevealed = new bool[height, width];  // Tracks revealed cells
        static bool[,] isFlagged = new bool[height, width];   // Tracks flagged cells
        static int[,] adjacentMines = new int[height, width]; // Tracks number of adjacent mines for each cell
        static bool gameOver = false;

        static void Main(string[] args)
        {
            // Initialize game
            InitializeBoard();
            PlaceMines();
            CalculateMinesAround();

            while (!gameOver)
            {
                Console.Clear();
                DisplayBoard();
                HandlePlayerInput();
            }

            Console.Clear();
            DisplayBoard();
            Console.WriteLine("Game Over!");
            Console.ReadKey();
        }

        // Method to initialize the board
        static void InitializeBoard()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    board[i, j] = '#';  // All cells start hidden
                    isRevealed[i, j] = false;
                    isFlagged[i, j] = false;
                    mines[i, j] = false;
                }
            }
        }

        // Method for placing mines randomly
        static void PlaceMines()
        {
            Random rand = new Random();
            int minesPlaced = 0;
            while (minesPlaced < numMines)
            {
                int randRow = rand.Next(0, height);
                int randCol = rand.Next(0, width);

                if (!mines[randRow, randCol])
                {
                    mines[randRow, randCol] = true;
                    minesPlaced++;
                }
            }
        }

        // Calculate number of adjacent mines for each cell
        static void CalculateMinesAround()
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (!mines[row, col])
                    {
                        adjacentMines[row, col] = CountAdjacentMines(row, col);
                    }
                }
            }
        }

        // Count mines around a given cell
        static int CountAdjacentMines(int row, int col)
        {
            int mineCount = 0;
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    if (i >= 0 && i < height && j >= 0 && j < width && mines[i, j])
                    {
                        mineCount++;
                    }
                }
            }
            return mineCount;
        }

        // Display the board to the player
        static void DisplayBoard()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (isFlagged[i, j])
                    {
                        Console.Write("F ");
                    }
                    else if (!isRevealed[i, j])
                    {
                        Console.Write("# ");
                    }
                    else if (mines[i, j])
                    {
                        Console.Write("* ");
                    }
                    else
                    {
                        Console.Write(adjacentMines[i, j] + " ");
                    }
                }
                Console.WriteLine();
            }
        }

        // Handle player input for revealing or flagging cells
        static void HandlePlayerInput()
        {
            while (true)
            {
                Console.WriteLine("Enter 'r' to reveal or 'f' to flag followed by row and column (e.g., 'r 3 4'): ");
                string input = Console.ReadLine();
                string[] parts = input.Split();

                if (parts.Length < 3) return;

                char action = parts[0][0];
                int row = int.Parse(parts[1]);
                int col = int.Parse(parts[2]);

                if (action == 'r')
                {
                    RevealCell(row, col);
                    break;
                }
                else if (action == 'f')
                {
                    FlagCell(row, col);
                    break;
                }
            }
        }

        // Reveal the cell and check for game over
        static void RevealCell(int row, int col)
        {
            if (row < 0 || row >= height || col < 0 || col >= width || isRevealed[row, col])
            {
                return;
            }

            isRevealed[row, col] = true;

            // If the cell contains a mine, game over
            if (mines[row, col])
            {
                gameOver = true;
                return;
            }

           
            // If the cell has no adjacent mines, reveal adjacent cells
            if (adjacentMines[row, col] == 0)
            {
                RevealAdjacentCells(row, col);
            }
            

            // Check if the player has won
            if (CheckWinCondition())
            {
                Console.WriteLine("Congratulations! You've won!");
                gameOver = true;
            }
        }

        // Flag or unflag a cell
        static void FlagCell(int row, int col)
        {
            if (row < 0 || row >= height || col < 0 || col >= width || isRevealed[row, col])
            {
                return; 
            }

            isFlagged[row, col] = !isFlagged[row, col];
        }

        // Reveal adjacent cells recursively
        static void RevealAdjacentCells(int row, int col)
        {
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    if (i >= 0 && i < height && j >= 0 && j < width && !isRevealed[i, j])
                    {
                        RevealCell(i, j);
                    }
                }
            }
        }

        // Check if the player has won (all non-mine cells are revealed)
        static bool CheckWinCondition()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (!mines[i, j] && !isRevealed[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}


