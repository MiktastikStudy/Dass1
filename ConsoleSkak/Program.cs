

using System;

namespace SimpleChessGame
{
    class Program
    {
        static string gameChoice;
        static void Main(string[] args)
        {
            while (true)
            {
             Console.Clear(); // Ryd konsollen mellem valg
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
                        //PlayMineSweeper();
                        break;
                    case "2":
                        PlayChess();
                        break;
                    case "3":
                        //PlayBattleShips();
                        break;
                    case "4":
                        //PlayJeoperdy();
                        break;
                    case "5":
                        // Luk spillet ned
                        Console.WriteLine("Exiting game...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }

        }



        static void PlayChess()
        {
            // Opret et 8x8 skakbræt med tegn, hvor '.' repræsenterer et tomt felt
            char[,] board = InitializeBoard();
            char[,] previousBoard = new char[8, 8]; // Holder styr på forrige bræt

            // Simpel spil loop, som printer brættet og lader brugeren spille
            while (true)
            {
                PrintBoard(board);
                Console.WriteLine(" ");
                Console.WriteLine("Indtast dit træk (f.eks. e2 e4)");
                Console.WriteLine("'Reset' for at starte forfra");
                Console.WriteLine("'Undo' for at gå et træk tilbage");
                Console.WriteLine("Eller 'quit' for at afslutte:");
                string move = Console.ReadLine().ToLower();

                if (move == "quit")
                {
                    // Afslut programmet
                    Console.WriteLine("Afslutter spillet. Tak for spillet!");
                    return; //return til main menu
                }
                else if (move == "reset")
                {
                    // Nulstil brættet
                    board = InitializeBoard();
                    Console.WriteLine("Brættet er nulstillet.");
                    continue;
                }
                else if (move == "undo")
                {
                    // Gå tilbage til forrige bræt-tilstand
                    board = UndoMove(previousBoard);
                    Console.WriteLine("Sidste træk er fortrudt.");
                    continue;
                }

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

                // Gem den nuværende tilstand af brættet før flytning
                SaveBoardState(board, previousBoard);

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

        // Funktion til at initialisere skakbrættet til startpositionen
        static char[,] InitializeBoard()
        {
            return new char[8, 8]
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

            return true;
        }

        // Gemmer den nuværende tilstand af brættet
        static void SaveBoardState(char[,] board, char[,] previousBoard)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    previousBoard[row, col] = board[row, col];
                }
            }
        }

        // Gendanner den forrige bræt-tilstand
        static char[,] UndoMove(char[,] previousBoard)
        {
            char[,] newBoard = new char[8, 8];
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    newBoard[row, col] = previousBoard[row, col];
                }
            }
            return newBoard;
        }

        // Print skakbrættet med farver for brikkerne
        static void PrintBoard(char[,] board)
        {
            Console.Clear();
            Console.WriteLine("  a b c d e f g h");

            for (int row = 0; row < 8; row++)
            {
                Console.Write(8 - row + " ");
                for (int col = 0; col < 8; col++)
                {
                    char piece = board[row, col];

                    // Ændr baggrundsfarven og tekstfarven afhængig af brikken
                    if (char.IsUpper(piece)) // Hvide brikker
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }
                    else if (char.IsLower(piece)) // Sorte brikker
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else // Tomme felter
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    Console.Write(piece + " ");
                    Console.ResetColor();  // Nulstil farver til standard
                }
                Console.WriteLine(8 - row);
            }
            Console.WriteLine("  a b c d e f g h");
        }
    }
}
