using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSweeper___Test
{
    internal class Program
    {
        static int width = 8;
        static int height = 8;
        static bool isMine = false;
        static bool isRevealed = false;
        static bool isFlagged = false;
        static int adjacentMines = 0;

        static void Main(string[] args)
        {
        }

        static string String()
        {
            if (isFlagged)
                return "F";
            if (!isRevealed)
                return "?";
            if (isMine)
                return "*";
            return adjacentMines > 0 ? adjacentMines.ToString() : " ";
        }




    }
}

