using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using AutoIt;
using System.Threading.Tasks;
using System.Windows;
using Telegram.Bot.Types.InputFiles;

namespace Mir4Controller
{
    class Mir4WindowsController
    {
        public static bool controllerState = false;
        public static int numberOfMIr4Processes = 0;
        public static List<string> mir4Processes = new List<string> { };

        public static void Run()
        {
            mir4Processes = new List<string> { };
            while (controllerState)
            {
                PrerareWindow(Settings.Mir4Window0, mir4Processes, Settings.mir0CoordsX, Settings.mir0CoordsY, Settings.winSizeX, Settings.winSizeY);
                PrerareWindow(Settings.Mir4Window1, mir4Processes, Settings.mir1CoordsX, Settings.mir1CoordsY, Settings.winSizeX, Settings.winSizeY);
                PrerareWindow(Settings.Mir4Window2, mir4Processes, Settings.mir2CoordsX, Settings.mir2CoordsY, Settings.winSizeX, Settings.winSizeY);
                CheckAndRecovery(Settings.Mir4Window0,Settings.WindowToFindClass, Settings.mir0CoordsX, Settings.mir0CoordsY);
                CheckAndRecovery(Settings.Mir4Window1, Settings.WindowToFindClass, Settings.mir1CoordsX, Settings.mir1CoordsY);
                CheckAndRecovery(Settings.Mir4Window2, Settings.WindowToFindClass, Settings.mir2CoordsX, Settings.mir2CoordsY);
                ScreenshotHandler.Delay(2400);
            }
        }


        public static void CheckCharracterState(string window, string windowClass, int winRectX, int winRectY)
        {
            AutoItX.MouseClickDrag("LEFT", winRectX + 100, winRectY + 100, winRectX + 300, winRectY + 120);
            ScreenshotHandler.Delay(5000);
            TelegramBot.SendTelegramMessage($"{window}", ScreenshotHandler.GetWinScreenshot(window, windowClass));
            ScreenshotHandler.Delay(1000);
            AutoItX.MouseMove(winRectX + 325, winRectY + 315);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(2400);
            AutoItX.MouseMove(winRectX + 615, winRectY + 50);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(2400);
            AutoItX.MouseMove(winRectX + 80, winRectY + 280);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(2400);
        }

        public static bool IsPixelBlack(string window, string windowClass, int winRectX, int winRectY)
        {
            Bitmap controlBmp = ScreenshotHandler.GetWinScreenshot(window, windowClass);
            Color temp = controlBmp.GetPixel(130, 260);
            if ((temp.R <= 20) && (temp.G <= 20) && (temp.B <= 20))
            {
                return true;
            }
            else return false;
        }

        public static void CheckAndRecovery(string window, string windowClass, int winRectX, int winRectY)
        {
            Bitmap controlBmp = ScreenshotHandler.GetWinScreenshot(windowClass,window);
            Color temp = controlBmp.GetPixel(130, 260);
            if ((temp.R >= 20) && (temp.G >= 20) && (temp.B >= 20))
            {
                RecoverCharacter(window, winRectX, winRectY);
            }
        }

        public static bool RecoverCharacter(string window, int winRectX, int winRectY)
        {
            AutoItX.WinActivate(window);
            ScreenshotHandler.Delay(500);

            AutoItX.MouseMove(winRectX + 542, winRectY + 315);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(5000);

            AutoItX.Send("{f10}");
            ScreenshotHandler.Delay(5000);
            AutoItX.MouseMove(winRectX+130, winRectY+50);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(5000);
            AutoItX.MouseMove(winRectX + 528, winRectY + 320);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(5000);
            AutoItX.MouseMove(winRectX + 286, winRectY + 290);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(5000);
            AutoItX.MouseMove(winRectX + 286, winRectY + 280);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(2000);
            AutoItX.MouseMove(winRectX + 452, winRectY + 224);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(1000);
            AutoItX.MouseMove(winRectX + 350, winRectY + 280);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(1000);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(1000);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(1000);

            ScreenshotHandler.Delay(200000);

            AutoItX.MouseMove(winRectX + 180, winRectY + 338);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(1000);
            AutoItX.MouseMove(winRectX + 615, winRectY + 50);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(2400);
            AutoItX.MouseMove(winRectX + 80, winRectY + 280);
            AutoItX.MouseClick("LEFT");
            ScreenshotHandler.Delay(2400);
            return false;
        }


        private static void PrerareWindow(string currWin, List<string> mir4CurrProcesses, int xDestinationCoord, int yDestinationCoord,int winSizeX, int winSizeY)
        {
            Bitmap picture = ScreenshotHandler.GetWinScreenshot(Settings.WindowToFindClass, currWin);
            if (picture != null)
            {
                if ((AutoItX.WinGetPos(currWin).X != xDestinationCoord) && (AutoItX.WinGetPos(currWin).Y != yDestinationCoord))
                {
                    AutoItX.WinMove(currWin, "", xDestinationCoord, yDestinationCoord, winSizeX, winSizeY); 
                }
                if (!mir4CurrProcesses.Contains(currWin))
                {
                    mir4CurrProcesses.Add(currWin);
                }
                picture.Dispose();
            }
            ScreenshotHandler.Delay(100);
        }

        public static void GetMir4Window(string currWin)
        {
            Bitmap picture = ScreenshotHandler.GetWinScreenshot(Settings.WindowToFindClass, currWin);
            if (picture != null)
            {
                TelegramBot.SendTelegramMessage($" {currWin} ", picture);
                picture.Dispose();
            }
            ScreenshotHandler.Delay(100);
        }

    }
}
