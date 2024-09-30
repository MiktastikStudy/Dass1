using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper
{

    /*
     * Create MineSweeper game
     * Elements that needs to be impleneted:
     * Oprettelse af et board
     * Random placering af Mines - Method
     * Man skal kunne sætte Flags - Method
     * Tal der indikere miner omkring dem
     * 
     * Plan - 
     * Lav array for board, og lav metode til board og call den i main - Boardet skal være fyldt af # som indikere ikke vendte felter
     * Lav derefter at spilleren kan flytte sig rundt via pil-taster og vende felterne, efter et felt er blevet vendt skal den være tom til at starte med
     * Quit funktion - Ny metode hvor man skal kunne trykke på q eller Esc for at quite spillet
     * Man skal kunne sætte flag der hvor man tror bomberne er ved at trykke på f
     * Bomber skal placeres random på boardet
     * Der skal printes tal ud til spilleren som angiver bomber omkring felterne
     * 
     * Arbejde videre med udseende af board
     * Test for fejl angående F på ikke bombe, at F skal kunne fjernes igen - Muligvis noget med Floodfill calculation
     */
    internal class Program
    {
        //Define variables for board size and 2D array for board
        static int height = 8;
        static int width = 8;
        static char[,] board = new char [height, width];
        static bool[,] isRevealed = new bool[height, width];
        static int numMines = 10;
        static bool[,] isMine = new bool[height, width];
        static int[,] adjacentMines = new int[height, width];
        static bool[,] isFlagged = new bool[height, width];
        static bool gameOver = false;

        //Main - execute game
        static void Main(string[] args)
        {

            //Initalize the game
            InitializeBoard();
            PlaceMines();
            CalculateMinesAround();

            //While gameOver is false, then you can play the game
            while (gameOver == false)
            {
                Console.Clear();
                DisplayBoard();
                PlayerInput();
            }

            //If gameOver is true then clear board and it prints out "game over" to the plyaer
            Console.Clear();
            DisplayBoard();
            Console.WriteLine("Game over!");
            Console.ReadKey();
        }

        static void InitializeBoard()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    board[i, j] = '#';
                    isRevealed[i,j] = false;
                    isFlagged[i, j] = false;
                    isMine[i, j] = false;
                }
            }
        }

        static void DisplayBoard()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (isFlagged[i, j])
                    {
                        Console.Write('F');  // Show flag if flagged
                    }
                    else if (!isRevealed[i, j])
                    {
                        Console.Write('#');   // Show unrevealed cell
                    }
                    else if (isMine[i, j])
                    {
                        Console.Write('*');   // Show mine
                    }
                    else
                    {
                        Console.Write(adjacentMines[i, j]); // Show number of adjacent mines
                    }
                }
                Console.WriteLine();
            }
        }

        static void PlaceMines()
        {
            Random rand = new Random();
            int minesPlaced = 0;

            while (minesPlaced < numMines)
            {
                int randRow = rand.Next(0, height);
                int randCol = rand.Next(0, width);

                if (!isMine[randRow, randCol])
                {
                    isMine[randRow, randCol] = true;
                    minesPlaced++;
                }
            }
        }

        static void CalculateMinesAround()
        {
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    if (!isMine[row, col])
                    {
                        adjacentMines[row, col] = CountAdjacentMines(row, col);
                    }
                }
            }
        }

        // Count mines around a given cell
        static int CountAdjacentMines(int row, int col)
        {
            int  minesAround= 0;
            for (int i = row - 1; i <= row + 1; i++)
            {
                for (int j = col - 1; j <= col + 1; j++)
                {
                    if (i >= 0 && i < height && j >= 0 && j < width && isMine[i, j]) //Checks if the cell is a mine
                    {
                        minesAround++; //Increase int minesAround
                    }
                }
            }
            return minesAround; //Return the value of minesAround
        }

        static void Flag(int row, int col)
        {
            //If out of bounds or the cell is revealed then it returns
            if (row < 0 || row >= height || col < 0 || col >= width || isRevealed[row, col])
            {
                return;
            }
            //else isFlagged becomes true if it was false or it becomes false if it was true
            else
            {
                isFlagged[row, col] = !isFlagged[row, col];
            }
        }

        static void PlayerInput()
        {
            while (true)
            {
                char input;
                int rowInput;
                int colInput;

                Console.WriteLine("Enter 'r' or 'f' to reveal or flag a cell");
                input = char.Parse(Console.ReadLine().ToLower());
                if (input == 'r')
                {
                    Console.WriteLine("Enter row");
                    rowInput = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter col");
                    colInput = int.Parse(Console.ReadLine());
                    RevealCell(rowInput, colInput);
                    break;
                } else if(input == 'f')
                {
                    Console.WriteLine("Enter row");
                    rowInput = int.Parse(Console.ReadLine());
                    Console.WriteLine("Enter col");
                    colInput = int.Parse(Console.ReadLine());
                    Flag(rowInput, colInput);
                    break;
                }
            }
        }

        static void RevealCell(int row, int col)
        {
            //If out of bounds or the cell is revealed then it returns
            if (row < 0 || row >= height || col < 0 || col >= width || isRevealed[row, col])
            {
                return;
            }
            else
            {
                isRevealed[row, col] = true;
            }

            if (isMine[row, col])
            {
                gameOver = true;
                return;
            }

            if (adjacentMines[row,col] == 0)
            {
                RevealAdjacentCells(row, col);
            }

        }

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
    }
}
