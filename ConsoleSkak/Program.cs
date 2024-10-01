

// Quit funktion
// Try again/reset funktion - o
// Farve på "brikkerne"
// Return to main menu funktion

//namespace ConsoleSkak
//{
//    internal class Program
//    {
//        static void Main(string[] args)
//        {
//       }
//    }
//}
using System;

namespace SimpleChessGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // Opret et 8x8 skakbræt med tegn, hvor '.' repræsenterer et tomt felt
            char[,] board = new char[8, 8]
            {
                { 'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R' }, // Hvide tårn, springer, løber, dronning, konge osv.
                { 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P' }, // Hvide bønder
                { '.', '.', '.', '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.', '.', '.', '.' },
                { '.', '.', '.', '.', '.', '.', '.', '.' },
                { 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p' }, // Sorte bønder
                { 'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r' }  // Sorte tårn, springer, løber, dronning, konge osv.
            };

            // Simpel spil loop, som printer brættet
            while (true)
            {
                PrintBoard(board);
                Console.WriteLine("Indtast dit træk (f.eks. e2 e4): ");
                string move = Console.ReadLine();

                if (move.Length != 5 || move[2] != ' ')
                {
                    Console.WriteLine("Ugyldigt træk. Prøv igen.");
                    continue;
                }

                // Fortolk inputtet
                int startCol = move[0] - 'a';
                int startRow = 8 - (move[1] - '0');
                int endCol = move[3] - 'a';
                int endRow = 8 - (move[4] - '0');

                // Flyt brikken
                if (IsMoveValid(board, startRow, startCol, endRow, endCol))
                {
                    board[endRow, endCol] = board[startRow, startCol];
                    board[startRow, startCol] = '.';
                }
                else
                {
                    Console.WriteLine("Ugyldigt træk. Prøv igen.");
                }
            }
        }

        // Simpel metode til at tjekke om et træk er gyldigt
        static bool IsMoveValid(char[,] board, int startRow, int startCol, int endRow, int endCol)
        {
            // Undgå at flytte tomme felter
            if (board[startRow, startCol] == '.')
            {
                return false;
            }

            // Tjek for gyldige koordinationer (indenfor brættet)
            if (startRow < 0 || startRow > 7 || startCol < 0 || startCol > 7 || endRow < 0 || endRow > 7 || endCol < 0 || endCol > 7)
            {
                return false;
            }

            // Simpelt tjek: Kan ikke slå egne brikker
            if (char.IsUpper(board[startRow, startCol]) && char.IsUpper(board[endRow, endCol]))
            {
                return false;
            }
            if (char.IsLower(board[startRow, startCol]) && char.IsLower(board[endRow, endCol]))
            {
                return false;
            }

            // I denne forenklede version antager vi bare, at ethvert træk er gyldigt, så længe det ikke bryder ovenstående regler
            return true;
        }

        // Print skakbrættet
        static void PrintBoard(char[,] board)
        {
            Console.Clear();
            Console.WriteLine("  a b c d e f g h");
            for (int row = 0; row < 8; row++)
            {
                Console.Write(8 - row + " ");
                for (int col = 0; col < 8; col++)
                {
                    Console.Write(board[row, col] + " ");
                }
                Console.WriteLine(8 - row);
            }
            Console.WriteLine("  a b c d e f g h");
        }
    }
}
