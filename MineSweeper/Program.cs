using System;
using System.Collections.Generic;
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
     */
    internal class Program
    {

        //Define Variables
        static string playerChoice;
        static int width = 8;
        static int height = 8;
        static int numMines = 10;
        static int minesPlaced = 0;
        static bool isMine = false;
        static bool isRevealed = false;
        static bool isFlagged = false;
        static int adjacentMines = 0;
        static char[,] board = new char[height, width];
        static bool[,] mines = new bool[height, width];

        //Main - execute game
        static void Main(string[] args)
        {
            Board();
            PlaceMines();
            Quit();
        }

        //Method for the "board" / grid
        static void Board()
        {
            Console.Clear();
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Console.Write(board[i, j] = '#');
                }
                Console.WriteLine();
            }
        }

        //Method for placing mines on the board
        static void PlaceMines()
        {
            Random rand = new Random();
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

        //Calculate and Count adjancent mines methods needs to be done here


        //Method for quiting the game
        static void Quit()
        {
            //Until the player writes "Leave" the game keeps running
            while (true)
            {
                Console.WriteLine("\nType quit to leave");
                playerChoice = Console.ReadLine();
                if (playerChoice == "quit")
                {
                    break;
                }
            }
        }
    }
}
