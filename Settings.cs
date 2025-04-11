using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mir4Controller
{
    public static class Settings
    {
        public static readonly object screenshotLock = new object();

        // Mir4 windows
        public static string WindowToFindClass = "[CLASS:UnrealWindow]";
        public static string Mir4Window0 = "Mir4G[0]";
        public static string Mir4Window1 = "Mir4G[1]";
        public static string Mir4Window2 = "Mir4G[2]";

        public static readonly int winSizeX = 640;
        public static readonly int winSizeY = 375;
        public static readonly int mir1CoordsX = 100;
        public static readonly int mir1CoordsY = 100;
        public static readonly int mir2CoordsX = 100 + winSizeX + 40;
        public static readonly int mir2CoordsY = 100;
        public static readonly int mir0CoordsX = 100;
        public static readonly int mir0CoordsY = 100 + winSizeY + 40;

        // Telegram user data
        public static string token = "5011228823:AAFHEZrhdZ1Ijp7APSPsCBnQ2EuFzunMFxU";
        public static string userId = "464399966";
        public static string pcId = "Main pc";

        // Telegram buttons
        public const string getScreenshotBtnText = "Get Screenshot";
        public const string showAllmir4ProcessesBtnText = "Show Mir4 Precesses";
        public const string startBtnText = "Start Controller";
        public const string stopBtnText = "Stop Controller";
        public const string checkCharacterState = "Check State";
    }
}
