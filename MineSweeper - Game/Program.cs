using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MineSweeper
{
    internal class Program
    {
        static int height = 8;
        static int width = 8;
        static char[,] board = new char[height, width];      // Store the current view of the board
        static bool[,] isRevealed = new bool[height, width]; // Track revealed cells
        static int numMines = 8;                            // Number of mines
        static bool[,] isMine = new bool[height, width];     // Track mine positions
        static int[,] adjacentMines = new int[height, width];// Store number of mines around each cell
        static bool[,] isFlagged = new bool[height, width];  // Track flagged cells
        static bool gameOver = false;                        // Game over
        static int flagsLeft;                                // Tracks how many flags are left
        static string gameChoice;

        static void Main(string[] args)
        {
            Console.WriteLine("Choose one of 4 games;" +
                "\n 1. Minesweeper" +
                "\n 2. Chess" +
                "\n 3. Battleships" +
                "\n 4. Jeoperdy" +
                "\n 5. Exit");
            Console.WriteLine("Write the number of your choice");
            gameChoice = Console.ReadLine();

            switch (gameChoice)
            {
                case "1":
                    PlayMineSweeper();
                    break;
                case "2":
                    //PlayChess();
                    break;
                case "3":
                    //PlayBattleShips();
                    break;
                case "4":
                    //PlayJeoperdy();
                    break;
                case "5":
                    //Exit();
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }

        static void PlayMineSweeper()
        {
            while (true)
            {
                // Initialize board and place mines
                StartGame();

                // Game loop - continues until gameOver becomes true
                while (!gameOver)
                {
                    Console.Clear();
                    DisplayBoard();  // Display the current board state
                    PlayerInput();   // Handle player's input for revealing or flagging cells
                }
                // If the game is over, display the board one last time and end the game
                Console.Clear();
                DisplayBoard();

                // Display message based on the game outcome
                if (CheckWinCondition())
                {
                    Console.WriteLine("You won!");  // Win message
                }
                else
                {
                    Console.WriteLine("Game over!"); // Game over (loss)
                }

                // Prompt to try again or quit
                if (!TryAgain())
                {
                    break; // Quit if player chooses not to try again
                }
            }
        }

        // Method to start the game (reset variables)
        static void StartGame()
        {
            InitializeBoard();
            PlaceMines();
            CalculateMinesAround();
            flagsLeft = numMines;
            gameOver = false;
        }

        // Method to initialize the board with unrevealed cells, and no mines or flags
        static void InitializeBoard()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    board[i, j] = '#';              // '#' represents an unrevealed cell
                    isRevealed[i, j] = false;       // No cells are revealed initially
                    isFlagged[i, j] = false;        // No cells are flagged initially
                    isMine[i, j] = false;           // No mines are placed yet
                }
            }
        }

        // Method to display the board to the player
        static void DisplayBoard()
        {
            Console.WriteLine($"Flags left: {flagsLeft}");

            Console.WriteLine("\n  01234567");
            for (int i = 0; i < height; i++)
            {
                Console.Write($"{i} ");
                for (int j = 0; j < width; j++)
                {
                    if (isFlagged[i, j])
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write('F');  // Show 'F' for flagged cells
                        Console.ResetColor();
                    }
                    else if (!isRevealed[i, j])
                    {
                        Console.Write('#');   // Show '#' for unrevealed cells
                    }
                    else if (isMine[i, j])
                    {
                        Console.Write('*');   // Show '*' for mines
                    }
                    else
                    {
                        Console.Write(adjacentMines[i, j]); // Show the number of adjacent mines
                    }
                }
                Console.WriteLine();  // Move to the next line after each row
            }
        }

        // Method to randomly place mines on the board
        static void PlaceMines()
        {
            Random rand = new Random();   // Random generator for mine placement
            int minesPlaced = 0;          // Counter to keep track of how many mines are placed

            while (minesPlaced < numMines)
            {
                // Generate random coordinates for mine placement
                int randRow = rand.Next(0, height);
                int randCol = rand.Next(0, width);

                // Place a mine if there isn't one already at the generated location
                if (!isMine[randRow, randCol])
                {
                    isMine[randRow, randCol] = true;
                    minesPlaced++;  // Increment the counter for placed mines
                }
            }
        }

        // Method to calculate the number of mines surrounding each cell
        static void CalculateMinesAround()
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (!isMine[row, col])
                    {
                        adjacentMines[row, col] = CountAdjacentMines(row, col); // Calculate adjacent mines for non-mine cells
                    }
                }
            }
        }

        // Helper method to count mines around a specific cell
        static int CountAdjacentMines(int row, int col)
        {
            int minesAround = 0;
            // Check all adjacent cells (including diagonals)
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    // Ensure the cell is within the board bounds and is a mine
                    if (i >= 0 && i < height && j >= 0 && j < width && isMine[i, j])
                    {
                        minesAround++;  // Increment count if adjacent cell is a mine
                    }
                }
            }
            return minesAround;  // Return the total count of surrounding mines
        }

        // Method to toggle flagging of a cell
        static void Flag(int row, int col)
        {
            // If the cell is out of bounds or already revealed, ignore the flagging attempt
            if (row < 0 || row >= height || col < 0 || col >= width || isRevealed[row, col])
            {
                return;
            }

            if (!isFlagged[row, col])
            {
                if (flagsLeft > 0) // Only place a flag if we have flags left
                {
                    isFlagged[row, col] = true;
                    flagsLeft--; // Decrease the flag count
                }
                else
                {
                    Console.WriteLine("No flags left!");
                }
            }
            else
            {
                isFlagged[row, col] = false;
                flagsLeft++; // Increase the flag count when a flag is removed
            }
        }

        // Method to handle player input for revealing or flagging cells
        static void PlayerInput()
        {
            while (true)
            {
                string input;
                char output;
                string rowInput;
                int rowOutput;
                string colInput;
                int colOutput;

                // Prompt for player input (reveal or flag)
                Console.WriteLine("Enter 'r' to reveal, or 'f' to flag");
                input = Console.ReadLine().ToLower();

                if (char.TryParse(input, out output))
                {
                    if (output == 'r')
                    {
                        // Reveal a cell
                        Console.WriteLine("Enter row");
                        rowInput = Console.ReadLine();   // Get row input from player
                        if (int.TryParse(rowInput, out rowOutput) && rowOutput >= 0 && rowOutput < height)
                        {
                            Console.WriteLine("Enter col");
                            colInput = Console.ReadLine();
                            if (int.TryParse(colInput, out colOutput) && colOutput >= 0 && colOutput < width)
                            {
                                RevealCell(rowOutput, colOutput);

                                // Check if the player has won after revealing a cell
                                if (CheckWinCondition())
                                {
                                    gameOver = true;
                                    Console.Clear();
                                    DisplayBoard();
                                    Console.WriteLine("You won!"); // Display the win message
                                }
                                break;
                            }
                        }
                    }
                    else if (output == 'f')
                    {
                        // Flag a cell
                        Console.WriteLine("Enter row");
                        rowInput = Console.ReadLine();   // Get row input from player
                        if (int.TryParse(rowInput, out rowOutput) && rowOutput >= 0 && rowOutput < height)
                        {
                            Console.WriteLine("Enter col");
                            colInput = Console.ReadLine();
                            if (int.TryParse(colInput, out colOutput) && colOutput >= 0 && colOutput < width)
                            {
                                Flag(rowOutput, colOutput);

                                // Check if the player has won after flagging a cell
                                if (CheckWinCondition())
                                {
                                    gameOver = true;
                                    Console.Clear();
                                    DisplayBoard();
                                    Console.WriteLine("You won!"); // Display the win message
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, try again");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input, try again");
                }
            }
        }

        // Method to reveal a cell on the board
        static void RevealCell(int row, int col)
        {
            // Ignore if the cell is out of bounds or already revealed
            if (row < 0 || row >= height || col < 0 || col >= width || isRevealed[row, col] || isFlagged[row, col])
            {
                return;
            }

            isRevealed[row, col] = true;  // Reveal the cell

            // If the revealed cell is a mine, game over
            if (isMine[row, col])
            {
                gameOver = true;
            }
            // If the revealed cell has no adjacent mines, reveal adjacent cells recursively
            else if (adjacentMines[row, col] == 0)
            {
                for (int i = row - 1; i <= row + 1; i++)
                {
                    for (int j = col - 1; j <= col + 1; j++)
                    {
                        if (i >= 0 && i < height && j >= 0 && j < width)
                        {
                            RevealCell(i, j);  // Recursively reveal adjacent cells
                        }
                    }
                }
            }
        }

        // Method to check if the player has won the game
        static bool CheckWinCondition()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    // If there is a non-mine cell that is not revealed, the player hasn't won yet
                    if (!isMine[i, j] && !isRevealed[i, j])
                    {
                        return false; // Continue the game
                    }

                    // If there is a mine cell that is not flagged, the player hasn't won yet
                    if (isMine[i, j] && !isFlagged[i, j])
                    {
                        return false; // Continue the game
                    }
                }
            }
            return true; // All non-mine cells are revealed, player wins
        }

        // Method to ask the player if they want to play again
        static bool TryAgain()
        {
            Console.WriteLine("Play again? (y/n)");
            string input = Console.ReadLine().ToLower();
            return input == "y";
        }
    }
}


