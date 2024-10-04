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
using static System.Net.Mime.MediaTypeNames;

namespace Main_
{
    internal class Program
    {
        static int height = 8;
        static int width = 8;
        static char[,] mineBoard = new char[height, width];      // Store the current view of the board
        static bool[,] isRevealed = new bool[height, width]; // Track revealed cells
        static int numMines = 8;                            // Number of mines
        static bool[,] isMine = new bool[height, width];     // Track mine positions
        static int[,] adjacentMines = new int[height, width];// Store number of mines around each cell
        static bool[,] isFlagged = new bool[height, width];  // Track flagged cells
        static bool gameOver = false;                        // Game over
        static int flagsLeft;                                // Tracks how many flags are left
        static string gameChoice;
        static char exitButton;

        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
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
                        PlayChess();
                        break;
                    case "3":
                        PlayBattleShips();
                        break;
                    case "4":
                        PlayJeoperdy();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        //Start Minesweeper
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

                    if (gameOver && exitButton == 'q')
                    {
                        return;
                    }
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
                    return; // Quit if player chooses not to try again
                }
            }
        }

        // Method to start the game (reset variables)
        static void StartGame()
        {
            InitializeMineBoard();
            PlaceMines();
            CalculateMinesAround();
            flagsLeft = numMines;
            gameOver = false;
        }

        // Method to initialize the board with unrevealed cells, and no mines or flags
        static void InitializeMineBoard()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    mineBoard[i, j] = '#';              // '#' represents an unrevealed cell
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
                Console.WriteLine("Enter 'r' to reveal,'f' to flag or 'q' to leave game");
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
                    else if (output == 'q')
                    {
                        gameOver = true;
                        exitButton = 'q';
                        return;
                    }
                    else
                    {
                        Console.WriteLine("Invalid input, try again");
                    }
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
            //Flood Fill:
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

        //Start on Chess
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

        //Start on Jeoperdy
        static void PlayJeoperdy()
        {
            int score = 0;
            int gameActive = 1; // Om det er aktivt eller ej
            int question1Answered = 0;
            int question2Answered = 0;
            int question3Answered = 0;
            int question4Answered = 0;
            int question5Answered = 0;
            int question6Answered = 0;

            Console.Clear();
            Console.WriteLine("Velkommen til Simons sindsyge Jeopardy!");

            while (gameActive == 1)
            {
                // Tjek om alt er besvaret
                if (question1Answered == 1 && question2Answered == 1 && question3Answered == 1 && question4Answered == 1 && question5Answered == 1 && question6Answered == 1)
                {
                    Console.WriteLine("Du færdig!!! Din score blev: " + score);
                    gameActive = 0;
                    break;
                }


                Console.WriteLine("\nVælg en kategori:");
                Console.WriteLine("1. Videnskab");
                Console.WriteLine("2. Historie");
                Console.WriteLine("3. Sport");
                Console.WriteLine("Eller skriv 'quit' for at afslutte.");

                string categoryChoice = Console.ReadLine().ToLower(); ;

                // Tjek for quit i kategori-valget
                if (categoryChoice == "quit")
                {
                    Console.WriteLine("Afslutter spillet... Tak for at spille!");
                    return; // Afslut Jeopardy og gå tilbage til hovedmenuen
                }

                if (categoryChoice == "1")
                {
                    // Videnskabs spørgsmål
                    if (question1Answered == 0)
                    {
                        Console.WriteLine("For 200 points: Hvilken gas består jordens atmosfære mest af?");
                        string answer = Console.ReadLine();

                        // Tjek for quit i svaret
                        if (answer.Equals("quit", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Afslutter spillet... Tak for at spille!");
                            return; // Afslut Jeopardy og gå tilbage til hovedmenuen
                        }

                        if (answer.Equals("Nitrogen", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Korrekt!");
                            score += 200;
                        }
                        else
                        {
                            Console.WriteLine("Forkert! Det korrekte svar var Nitrogen.");
                            score -= 200;
                        }
                        question1Answered = 1;
                    }
                    else if (question2Answered == 0)
                    {
                        Console.WriteLine("For 100 points: Nu bliver det svært\nHvad er det kemiske symbol for Vand?");
                        string answer = Console.ReadLine();

                        // Tjek for quit i svaret
                        if (answer.Equals("quit", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Afslutter spillet... Tak for at spille!");
                            return; // Afslut Jeopardy og gå tilbage til hovedmenuen
                        }

                        if (answer.Equals("H2O", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Korrekt!");
                            score += 100;
                        }
                        else
                        {
                            Console.WriteLine("Forkert! The rigtige svar er H2O.");
                            score -= 100;
                        }
                        question2Answered = 1;
                    }
                    else
                    {
                        Console.WriteLine("Der ikke flere videnskabs spørgsmål.");
                        Console.Clear();
                    }
                }
                else if (categoryChoice == "2")
                {
                    // Historie spørgsmål
                    if (question3Answered == 0)
                    {
                        Console.WriteLine("For 200 points:I hvilket år sluttede WW2?");
                        string answer = Console.ReadLine();

                        // Tjek for quit i svaret
                        if (answer.Equals("quit", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Afslutter spillet... Tak for at spille!");
                            return; // Afslut Jeopardy og gå tilbage til hovedmenuen
                        }

                        if (answer.Equals("1945", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("korrekt!");
                            score += 200;
                        }
                        else
                        {
                            Console.WriteLine("Forkert! Det korrekte svar var 1945. Kom igen ;)");
                            score -= 200;
                        }
                        question3Answered = 1;
                    }
                    else if (question4Answered == 0)
                    {
                        Console.WriteLine("For 300 points: Hvor lang tid varede 100 års krigen?");
                        string answer = Console.ReadLine();

                        // Tjek for quit i svaret
                        if (answer.Equals("quit", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Afslutter spillet... Tak for at spille!");
                            return; // Afslut Jeopardy og gå tilbage til hovedmenuen
                        }

                        if (answer.Equals("116", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Korrekt!");
                            score += 300;
                        }
                        else
                        {
                            Console.WriteLine("Forkert! Det korrekte svar var 116 år.");
                            score -= 300;
                        }
                        question4Answered = 1;
                    }
                    else
                    {
                        Console.WriteLine("Ikke flere spørgsmål i Historie.");
                        Console.Clear();
                    }
                }
                else if (categoryChoice == "3")
                {
                    // Sport spørgsmål
                    if (question5Answered == 0)
                    {
                        Console.WriteLine("For 200 points: Hvilket land vandt FIFA World Cup i 2022?");
                        string answer = Console.ReadLine();

                        // Tjek for quit i svaret
                        if (answer.Equals("quit", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Afslutter spillet... Tak for at spille!");
                            return; // Afslut Jeopardy og gå tilbage til hovedmenuen
                        }

                        if (answer.Equals("Argentina", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Korrekt!");
                            score += 200;
                        }
                        else
                        {
                            Console.WriteLine("Forkert! Det korrekte svar var Argentina.");
                            score -= 200;
                        }
                        question5Answered = 1;
                    }
                    else if (question6Answered == 0)
                    {
                        Console.WriteLine("For 100 points: Hvor mange spillere er der på et Pro fodboldhold?");
                        string answer = Console.ReadLine();

                        // Tjek for quit i svaret
                        if (answer.Equals("quit", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Afslutter spillet... Tak for at spille!");
                            return; // Afslut Jeopardy og gå tilbage til hovedmenuen
                        }

                        if (answer.Equals("11", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Korrekt!");
                            score += 100;
                        }
                        else
                        {
                            Console.WriteLine("Forkert! Det korrekte svar var 11.");
                            score -= 100;
                        }
                        question6Answered = 1;
                    }
                    else
                    {
                        Console.WriteLine("Ikke flere i Sport.");
                        Console.Clear();
                    }
                }
                else
                {
                    Console.WriteLine("Forkert. Vælg venligst 1, 2, eller 3.");
                }

                Console.WriteLine($"Din score er: {score}");
            }

            Console.WriteLine("Tak for at spille Simons sindsyge Jeopardy!");
            Console.ReadKey();
        }

        //Start on battleships
        static void PlayBattleShips()
        {
            //Størrelsen på spillebrættet - det er 6 x 6
            int boardSize = 6;

            //Spilleren får en bane og Computeren får en bane
            char[,] playerBoard = new char[boardSize, boardSize];
            char[,] computerBoard = new char[boardSize, boardSize];

            //Computerens skibslayout
            bool[,] computerShips = new bool[boardSize, boardSize];

            //Antallet af skibe der startes med
            int playerShips = 4;
            int computerShipsCount = 4;

            //Laver en random der skal bruges til computerens placering af skibe og dens skud
            Random random = new Random();

            //Viser brættet som '~' i consolen
            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    playerBoard[i, j] = '~';
                    computerBoard[i, j] = '~';
                }
                Console.WriteLine();


                //Placere spillerens skibe
                playerBoard[3, 5] = 'S';
                playerBoard[1, 2] = 'S';
                playerBoard[4, 2] = 'S';
                playerBoard[5, 3] = 'S';

                //Computeren placere sine skibe tilfældigt
                int placedShips = 0;
                while (placedShips < computerShipsCount)
                {
                    int row = random.Next(boardSize);
                    int col = random.Next(boardSize);

                    //Tjek om der allerede er et skib på denne position
                    if (!computerShips[row, col])
                    {
                        computerShips[row, col] = true;
                        placedShips++;
                    }
                }

                Console.WriteLine("Welcome to 'Battleship'" +
                    "\n- you against the computer!");
                Console.WriteLine();

                //Hoved løkken
                while (playerShips > 0 && computerShipsCount > 0)
                {
                    Console.Clear();

                    //Udskriver begge spilleplader
                    Console.WriteLine("\n--- Your board --- \t --- Opponents board ---");
                    Console.WriteLine("\n  0 1 2 3 4 5 \t\t   0 1 2 3 4 5");
                    for (int x = 0; x < boardSize; x++)
                    {
                        Console.Write($"{x} ");
                        //Udskriver spillerens plade
                        for (int y = 0; y < boardSize; y++)
                        {
                            PrintWithColor(playerBoard[x, y]);
                        }


                        Console.Write($"\t\t {x} ");
                        //Console.Write($"{x} ");
                        //Udskriv computerens plade
                        for (int y = 0; y < boardSize; y++)
                        {
                            if (computerBoard[x, y] == 'X' || computerBoard[x, y] == 'O')
                            {
                                PrintWithColor(computerBoard[x, y]);
                            }
                            else
                            {
                                Console.Write("~ ");
                            }
                        }
                        Console.WriteLine();
                    }


                    Console.WriteLine("\nYour turn! Shoot your opponents ships");
                    Console.WriteLine("Enter a row and a coloum (0-5) seperated by a space");
                    Console.WriteLine("Or write quit to return to main menu");

                    // Læs og tjek om spilleren vil afslutte
                    string playerInput = Console.ReadLine();
                    if (playerInput.ToLower() == "quit")
                    {
                        Console.WriteLine("Afslutter spillet... Tak for at spille!");
                        return; // Afslut spillet og gå tilbage til hovedmenuen
                    }

                    //læs og konverter spillerens input til rækker og kolonner
                    string[] input = playerInput.Split(' ');
                    int playerRow = int.Parse(input[0]);
                    int playerCol = int.Parse(input[1]);

                    //Tjek om spilleren rammer et skib
                    if (computerShips[playerRow, playerCol])
                    {
                        Console.WriteLine("Nice! You hit one of their ships");
                        computerBoard[playerRow, playerCol] = 'X';
                        computerShips[playerRow, playerCol] = false; //Fjerner skibet fra computerens skibslayout
                        computerShipsCount--;
                    }
                    else
                    {
                        Console.WriteLine("Missed! There was no ship there");
                        computerBoard[playerRow, playerCol] = 'O';
                    }

                    //Computeren skyder mod spillerens skibe
                    int compRow = random.Next(boardSize);
                    int compCol = random.Next(boardSize);

                    //Fortsæt med at vælge tilfældige koordinator, indtil computeren vælger et sted der ikke er skudt på endnu
                    while (playerBoard[compRow, compCol] == 'X' || playerBoard[compRow, compCol] == 'O')
                    {
                        compRow = random.Next(boardSize);
                        compCol = random.Next(boardSize);
                    }

                    //Vis computerens skud og resultat
                    Console.WriteLine($"\nOpponent shoots at ({compRow}, {compCol})");

                    if (playerBoard[compRow, compCol] == 'S')
                    {
                        Console.WriteLine("Oh no! Your opponent hit your ship!");
                        playerBoard[compRow, compCol] = 'X';
                        playerShips--;
                    }
                    else
                    {
                        Console.WriteLine("Phew! Your opponent missed");
                        playerBoard[compRow, compCol] = 'O';
                    }

                }

                //Spillet er slut - hvem har vundet
                if (playerShips == 0)
                {
                    Console.WriteLine("\nYou lost! Your opponent shot down all your ships");
                }
                else
                {
                    Console.WriteLine("\nCongratulations! You shot down all your opponents ships");
                }
                Console.ReadLine();
            }
        }

        static void PrintWithColor(char symbol)
        {
            switch (symbol)
            {
                case 'X':
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case 'O':
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case 'S':
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
            }
            Console.Write(symbol + " ");
            Console.ResetColor();
        }
    }
}



