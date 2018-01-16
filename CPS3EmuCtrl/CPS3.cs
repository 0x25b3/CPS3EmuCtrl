/* CPS3EmuCtrl - CPS3.cs
 * 0x25b3, 180116
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CPS3EmuCtrl
{
    /// <summary>
    /// List of available games in ElSemi's CPS3 Emulator, Version 1.0A
    /// </summary>
    public enum CPS3Game
    {
        JOJOBA,
        JOJO,
        JOJOALT,
        SFIII2,
        SFIII3,
        SFIII3A,
        SFIII,
        WARZARD,
        JOJOBADID,
        JOJOALTCHD,
        JOJOCBD,
        SFIII2CHD,
        SFIII3ACHD,
        SFIII3CHD,
        SFIIIDID,
        WARZARDCHD
    }

    /// <summary>
    /// Control-Class for ElSemi's CPS3 Emulator
    /// </summary>
    public class CPS3
    {
        public static string CPS3Path = @"emulator.exe";
        public static int CPS3StepDelay = 50; // in Milliseconds; might need to be changed according to the client's performance

        /// <summary>
        /// Launches the given game using the configured CPS3Path
        /// </summary>
        /// <param name="Game">Game to launch</param>
        /// <returns>true, if (probably) launched successfully</returns>
        public static bool LaunchGame(CPS3Game Game)
        {
            bool Result = false;

            /* The following code represents the dirty way: programmatically clicking through the menus. The obvious
             * way of getting a hold of the actual objects did not work. So this is just a lame workaround, that only seems 
             * to work well if the mouse doesn't moved. Enough for my purposes though.
             */

            try
            {
                var WorkingDirectory = System.IO.Path.GetFullPath(CPS3Path);
                WorkingDirectory = WorkingDirectory.Substring(0, WorkingDirectory.LastIndexOf('\\'));

                var StartInfo = new ProcessStartInfo(System.IO.Path.GetFileName(CPS3Path))
                {
                    WorkingDirectory = WorkingDirectory,
                };

                // Launch emulator process
                var EmulatorProcess = Process.Start(StartInfo);

                // Delay for some milliseconds to give the window some time to open up
                Thread.Sleep(CPS3StepDelay);

                // Get window position
                (int EmulatorX, int EmulatorY) = Win32.GetPosition(EmulatorProcess);

                Thread.Sleep(CPS3StepDelay);

                // Strange positions, probably due to the window's border. Calibrated using trial and error for Windows 10
                var EmulatorMenuPosition = new System.Drawing.Point(30, -20);
                var LoadRomPosition = new System.Drawing.Point(25, 10);
                var GamePosition = new System.Drawing.Point(25, 65 + ((int)Game * 14));

                // Raise click on "Emulator" menu
                Win32.ClickOnPoint(EmulatorProcess, EmulatorMenuPosition);

                Thread.Sleep(CPS3StepDelay);

                // Raise click on "Load Rom..."
                Win32.ClickOnPoint(EmulatorProcess, LoadRomPosition);

                Thread.Sleep(CPS3StepDelay);

                // Raise click on target rom - lame double click
                Win32.ClickOnPoint(EmulatorProcess, GamePosition, true);

                Result = true;
            }
            catch (Exception ex)
            {
                //TODO: Do something with the exception? No? Okay.
                Result = false;
            }

            return Result;
        }
    }
}
