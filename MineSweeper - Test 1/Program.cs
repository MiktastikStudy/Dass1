using System;

namespace MineSweeper
{
    internal class Program
    {
        // Define the board size and number of mines
        static int width = 8;   // Width of the board
        static int height = 8;  // Height of the board
        static int numMines = 10;  // Number of mines to be placed on the board

        // Game board data
        static char[,] board = new char[height, width];   // Represents the visible state of the board (e.g., '#' for hidden)
        static bool[,] mines = new bool[height, width];   // Tracks whether a mine is placed in a cell
        static bool[,] revealed = new bool[height, width]; // Tracks whether a cell has been revealed
        static bool[,] flagged = new bool[height, width];  // Tracks whether a cell has been flagged by the player

        // Game state
        static bool gameOver = false; // Boolean to track if the game is over

        static void Main(string[] args)
        {
            // Step 1: Initialize the board (set all cells to hidden)
            InitializeBoard();

            // Step 2: Place mines randomly on the board
            PlaceMines();

            // Step 3: Calculate adjacent mines for non-mine cells
            CalculateAdjacentMines();

            // Step 4: Main game loop - repeat until the game is over
            while (!gameOver)
            {
                Console.Clear();  // Clear the console for a fresh view of the board
                DisplayBoard();   // Show the current board state to the player
                HandlePlayerInput();  // Get input from the player for the next move
            }

            // After the game ends, show the final board and announce the result
            Console.Clear();
            DisplayBoard();
            if (CheckWinCondition())
                Console.WriteLine("Congratulations! You won!");  // Player wins if all non-mine cells are revealed
            else
                Console.WriteLine("You hit a mine! Game Over.");  // Game over if the player reveals a mine

            Console.ReadKey();  // Wait for the player to press a key before exiting
        }

        // Initialize the board by setting all cells to hidden ('#') and no mines, revealed, or flagged
        static void InitializeBoard()
        {
            for (int i = 0; i < height; i++)  // Loop through each row
            {
                for (int j = 0; j < width; j++)  // Loop through each column
                {
                    board[i, j] = '#';  // All cells are hidden initially
                    mines[i, j] = false;  // No mines placed initially
                    revealed[i, j] = false;  // No cells revealed initially
                    flagged[i, j] = false;  // No cells flagged initially
                }
            }
        }

        // Summary:
        // This function initializes the game board and resets all the arrays. 
        // Every cell starts hidden, with no mines, no flags, and no revealed cells.

        // Randomly place a specific number of mines on the board, ensuring no duplicates
        static void PlaceMines()
        {
            Random rand = new Random();  // Create a random number generator
            int minesPlaced = 0;  // Keep track of how many mines have been placed

            // Loop until we've placed the required number of mines
            while (minesPlaced < numMines)
            {
                int row = rand.Next(0, height);  // Generate a random row index
                int col = rand.Next(0, width);   // Generate a random column index

                // If there's no mine in this cell, place one
                if (!mines[row, col])
                {
                    mines[row, col] = true;  // Place the mine
                    minesPlaced++;  // Increment the count of placed mines
                }
            }
        }

        // Summary:
        // This function randomly places the specified number of mines on the board.
        // It ensures that no two mines are placed in the same cell by checking before placing.

        // Calculate how many adjacent mines surround each non-mine cell
        static void CalculateAdjacentMines()
        {
            // Loop through every cell on the board
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    // If the current cell is not a mine, calculate adjacent mines
                    if (!mines[row, col])
                    {
                        int adjacentMines = CountAdjacentMines(row, col);  // Get the count of adjacent mines
                        // If there are no adjacent mines, display an empty space; otherwise, show the count
                        board[row, col] = adjacentMines == 0 ? ' ' : (char)(adjacentMines + '0');
                    }
                }
            }
        }

        // Summary:
        // This function calculates how many mines surround each non-mine cell.
        // If there are no surrounding mines, the cell will be blank (' '); otherwise, it will show the number of adjacent mines.

        // Helper function to count the number of mines adjacent to a specific cell
        static int CountAdjacentMines(int row, int col)
        {
            int mineCount = 0;  // Initialize the mine counter
                                // Loop through the 3x3 grid surrounding the cell
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    // Check if the neighboring cell is within bounds and contains a mine
                    if (i >= 0 && i < height && j >= 0 && j < width && mines[i, j])
                    {
                        mineCount++;  // Increment the mine counter
                    }
                }
            }
            return mineCount;  // Return the total number of mines found around the cell
        }

        // Summary:
        // This function counts how many mines are adjacent to a given cell.
        // It checks all the surrounding cells in a 3x3 area and returns the count of mines found.

        // Display the current state of the board to the player
        static void DisplayBoard()
        {
            for (int i = 0; i < height; i++)  // Loop through each row
            {
                for (int j = 0; j < width; j++)  // Loop through each column
                {
                    // If the cell is flagged by the player, show an 'F'
                    if (flagged[i, j])
                    {
                        Console.Write("F ");
                    }
                    // If the cell is still hidden, show a '#'
                    else if (!revealed[i, j])
                    {
                        Console.Write("# ");
                    }
                    // If the cell contains a mine, show a '*'
                    else if (mines[i, j])
                    {
                        Console.Write("* ");
                    }
                    // Otherwise, show the number of adjacent mines (or an empty space)
                    else
                    {
                        Console.Write(board[i, j] + " ");
                    }
                }
                Console.WriteLine();  // Move to the next line after each row
            }
        }

        // Summary:
        // This function displays the current state of the board, including hidden cells, revealed cells,
        // flagged cells, and mine positions (if revealed). It outputs the board to the console.

        // Handle the player's input to reveal or flag a cell
        static void HandlePlayerInput()
        {
            Console.WriteLine("Enter 'r' to reveal or 'f' to flag, followed by row and column (e.g., 'r 3 4'): ");
            string input = Console.ReadLine();  // Get input from the player
            string[] parts = input.Split();  // Split the input into components

            // Ensure that the input is valid (contains at least 3 parts)
            if (parts.Length < 3) return;

            // Get the action ('r' for reveal or 'f' for flag) and the row/col coordinates
            char action = parts[0][0];
            int row = int.Parse(parts[1]);
            int col = int.Parse(parts[2]);

            // Check if the coordinates are within bounds
            if (row < 0 || row >= height || col < 0 || col >= width) return;

            // Perform the requested action: reveal or flag the cell
            if (action == 'r')
            {
                RevealCell(row, col);  // Call the function to reveal the cell
            }
            else if (action == 'f')
            {
                FlagCell(row, col);  // Call the function to flag the cell
            }
        }

        // Summary:
        // This function handles user input. The player can choose to either reveal a cell ('r') or flag a cell ('f').
        // The input is split into parts to extract the action and the row/column of the cell.

        // Reveal a cell and check if it's a mine or safe
        static void RevealCell(int row, int col)
        {
            // If the cell is already revealed or flagged, do nothing
            if (revealed[row, col] || flagged[row, col]) return;

            revealed[row, col] = true;  // Mark the cell as revealed

            // If the revealed cell contains a mine, the game is over
            if (mines[row, col])
            {
                gameOver = true;  // Set the game over flag
                return;
            }

            // If the revealed cell has no adjacent mines, reveal surrounding cells
            if (board[row, col] == ' ')
            {
                RevealAdjacentCells(row, col);  // Recursively reveal adjacent cells
            }

            // Check if the player has won by revealing all non-mine cells
            if (CheckWinCondition())
            {
                gameOver = true;  // End the game if the player wins
            }
        }

        // Summary:
        // This function reveals the cell at the specified row and column. If the cell contains a mine, the game ends.
        // If the cell is empty and has no adjacent mines, the adjacent cells are also revealed.

        // Flag or unflag a cell
        static void FlagCell(int row, int col)
        {
            // If the cell is already revealed, it can't be flagged or unflagged
            if (revealed[row, col]) return;

            // Toggle the flagged state of the cell (flag or unflag it)
            flagged[row, col] = !flagged[row, col];
        }

        // Summary:
        // This function allows the player to flag a cell if they suspect it contains a mine.
        // It also allows the player to unflag a cell if they change their mind.

        // Reveal adjacent cells recursively if there are no adjacent mines
        static void RevealAdjacentCells(int row, int col)
        {
            // Loop through the 3x3 grid surrounding the current cell
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    // Check if the neighboring cell is within bounds and not yet revealed
                    if (i >= 0 && i < height && j >= 0 && j < width && !revealed[i, j])
                    {
                        RevealCell(i, j);  // Recursively reveal the neighboring cell
                    }
                }
            }
        }

        // Summary:
        // This function reveals adjacent cells recursively if the current cell has no adjacent mines.
        // It's used to reveal large empty areas of the board when there are no nearby mines.

        // Check if the player has won by revealing all non-mine cells
        static bool CheckWinCondition()
        {
            // Loop through the entire board
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    // If a non-mine cell is still hidden, the game is not won yet
                    if (!mines[i, j] && !revealed[i, j])
                    {
                        return false;  // Return false if there are still hidden cells
                    }
                }
            }
            return true;  // Return true if all non-mine cells are revealed (player wins)
        }

        // Summary:
        // This function checks if the player has won the game by revealing all non-mine cells.
        // If all non-mine cells are revealed, the player wins. Otherwise, the game continues.
    }
}
