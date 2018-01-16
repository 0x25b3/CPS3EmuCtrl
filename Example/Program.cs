using CPS3EmuCtrl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            CPS3.CPS3Path = @"cps3emulator\emulator.exe";
            CPS3.CPS3StepDelay = 50;
            CPS3.LaunchGame(CPS3Game.SFIII3);
        }
    }
}
