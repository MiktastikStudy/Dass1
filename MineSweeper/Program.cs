using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{
    /*
     * Create a MineSweeper game
     * Elements that need to be implemented:
     * - Creation of a board
     * - Random placement of mines - Method
     * - Ability to set flags - Method
     * - Numbers to indicate surrounding mines
     * 
     * Plan: 
     * - Create an array for the board, and a method to initialize it, call the method in `Main`.
     * - Allow the player to navigate the board using arrow keys and reveal cells.
     * - Create a quit function using 'q' or 'Esc'.
     * - Allow the player to flag suspected mines with 'f'.
     * - Randomly place mines on the board.
     * - Show numbers that indicate how many mines are adjacent to each cell.
     * - Test for bugs regarding flagging, removal of flags, and flood-fill for adjacent cells.
     */
    internal class Program
    {
        // Define board size and related arrays for storing the board state
        static int height = 8;
        static int width = 8;
        static char[,] board = new char[height, width];      // Store the current view of the board
        static bool[,] isRevealed = new bool[height, width]; // Track revealed cells
        static int numMines = 10;                            // Number of mines
        static bool[,] isMine = new bool[height, width];     // Track mine positions
        static int[,] adjacentMines = new int[height, width];// Store number of mines around each cell
        static bool[,] isFlagged = new bool[height, width];  // Track flagged cells
        static bool gameOver = false;                        // Game over flag

        // Main method, initiates the game
        static void Main(string[] args)
        {
            // Initialize board and place mines
            InitializeBoard();
            PlaceMines();
            CalculateMinesAround();

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
            Console.WriteLine("Game over!");
            Console.ReadKey(); // Wait for player input before closing
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
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (isFlagged[i, j])
                    {
                        Console.Write('F');  // Show 'F' for flagged cells
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
            // Toggle the flag state for the cell
            isFlagged[row, col] = !isFlagged[row, col];
        }

        // Method to handle player input for revealing or flagging cells
        static void PlayerInput()
        {
            while (true)
            {
                char input;
                int rowInput;
                int colInput;

                // Prompt for player input (reveal or flag)
                Console.WriteLine("Enter 'r' or 'f' to reveal or flag a cell");
                input = char.Parse(Console.ReadLine().ToLower());  // Convert input to lowercase for easier handling
                if (input == 'r')
                {
                    // Reveal a cell
                    Console.WriteLine("Enter row");
                    rowInput = int.Parse(Console.ReadLine());   // Get row input from player
                    Console.WriteLine("Enter col");
                    colInput = int.Parse(Console.ReadLine());   // Get column input from player
                    RevealCell(rowInput, colInput);             // Call method to reveal the cell
                    break;
                }
                else if (input == 'f')
                {
                    // Flag a cell
                    Console.WriteLine("Enter row");
                    rowInput = int.Parse(Console.ReadLine());   // Get row input from player
                    Console.WriteLine("Enter col");
                    colInput = int.Parse(Console.ReadLine());   // Get column input from player
                    Flag(rowInput, colInput);                   // Call method to flag the cell
                    break;
                }
            }
        }

        // Method to reveal a cell on the board
        static void RevealCell(int row, int col)
        {
            // Ignore if the cell is out of bounds or already revealed
            if (row < 0 || row >= height || col < 0 || col >= width || isRevealed[row, col])
            {
                return;
            }
            // Mark the cell as revealed
            isRevealed[row, col] = true;

            // If the revealed cell is a mine, the game is over
            if (isMine[row, col])
            {
                gameOver = true;
                return;
            }

            // If there are no adjacent mines, recursively reveal surrounding cells
            if (adjacentMines[row, col] == 0)
            {
                RevealAdjacentCells(row, col);  // Reveal adjacent cells using flood-fill technique
            }
        }

        // Method to recursively reveal adjacent cells when there are no surrounding mines
        static void RevealAdjacentCells(int row, int col)
        {
            // Loop through adjacent cells (including diagonals)
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    // Ensure the cell is within bounds and not yet revealed
                    if (i >= 0 && i < height && j >= 0 && j < width && !isRevealed[i, j])
                    {
                        RevealCell(i, j);  // Recursively reveal the adjacent cell
                    }
                }
            }
        }
    }
}
