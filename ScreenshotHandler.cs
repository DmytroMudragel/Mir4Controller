using AutoIt;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Windows;
using System.Threading;
using System.Runtime.InteropServices;

namespace Mir4Controller
{
    public static class ScreenshotHandler
    {
        #region Packed

        public static readonly object screenshotLock = new object();
        public static readonly Random random = new Random();

        /// <summary>
        /// Provides random delay: between delay time and delay time +7%
        /// </summary>
        /// <param name="delayTime"></param>
        public static void Delay(int delayTime)
        {
            Thread.Sleep(random.Next(delayTime, delayTime + (7 * (delayTime / 100))));
        }

        /// <summary>
        /// Returns window rectangle or empty rectangle if there no window
        /// </summary>
        /// <param name="winToFindName"></param>
        /// <returns></returns>
        public static Rectangle GetWindowRect(string winToFindName, string windowToFindClass, bool needed)
        {
            lock (screenshotLock)
            {
                if (!IsTheWindowsExist(windowToFindClass))
                {
                    return Rectangle.Empty;
                }
                if (needed && (AutoItX.WinGetTitle("[ACTIVE]") != winToFindName))
                {
                    AutoItX.WinActivate(windowToFindClass);
                    AutoItX.WinGetTitle();
                }
                return AutoItX.WinGetPos(windowToFindClass);
            }
        }

        /// <summary>
        /// Check is there windows exists 
        /// </summary>
        /// <returns></returns>
        public static bool IsTheWindowsExist(string windowToFindClass)
        {
            lock (screenshotLock)
            {
                return AutoItX.WinExists(windowToFindClass) == 1;
            }
        }

        /// <summary>
        /// Return certain window screenshot as bitmap or null 
        /// </summary>
        /// <param name="windowToActivateName"></param>
        /// <param name="windowToFindClass"></param>
        /// <param name="screenshotLock"></param>
        /// <returns></returns>
        public static Bitmap GetWinScreenshot(string windowToActivateName, string windowToFindClass)
        {
            lock (screenshotLock)
            {
                Rectangle currWinRect = GetWindowRect(windowToActivateName, windowToFindClass, true);
                if (currWinRect == Rectangle.Empty)
                {
                    return null;
                }
                Bitmap bitmap = new Bitmap(currWinRect.Width, currWinRect.Height);
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(new System.Drawing.Point(currWinRect.Left, currWinRect.Top), System.Drawing.Point.Empty, currWinRect.Size);
                }
                return bitmap;
            }
        }


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

        /// <summary>
        /// Return certain window screenshot even if it is not active as bitmap or null
        /// </summary>
        /// <param name="windowToActivateName"></param>
        /// <param name="windowToFindClass"></param>
        /// <param name="screenshotLock"></param>
        /// <returns></returns>
        /// 
        public static Bitmap GetWinScreenshotNotActive(string windowToActivateName, string windowToFindClass)
        {
            lock (screenshotLock)
            {
                Rectangle currWinRect = GetWindowRect(windowToActivateName, windowToFindClass, false);
                if (currWinRect == Rectangle.Empty)
                {
                    return null;
                }
                Bitmap B = new Bitmap(currWinRect.Width, currWinRect.Height);
                using (Graphics graphics = Graphics.FromImage(B))
                {
                    Bitmap bmp = new Bitmap(currWinRect.Size.Width, currWinRect.Size.Height, graphics);
                    Graphics memoryGraphics = Graphics.FromImage(bmp);
                    IntPtr dc = memoryGraphics.GetHdc();
                    var handle = AutoItX.WinGetHandle(windowToFindClass);
                    bool success = PrintWindow(handle, dc, 0);
                    memoryGraphics.ReleaseHdc(dc);
                    return bmp;
                }
            }
        }

        #endregion


        #region Unpacked
        //public static bool ClickInWow(int x, int y, string button = "RIGHT", int speed = 1) // uses for click in wow window // check 4 param
        //{
        //    EmulateMouseMove.EmulateMoveMouse(wowRect.Left + x, wowRect.Top + y, speed);
        //    System.Threading.Thread.Sleep(new Random().Next(10, 40));
        //    Debug.AddDebugRecord(wowRect.Left + x + " " + (wowRect.Top + y), true);
        //    AutoItX.MouseDown(button);
        //    System.Threading.Thread.Sleep(new Random().Next(50, 70));
        //    AutoItX.MouseUp(button);
        //    return true;
        //}

        //public static void EnterText(string text)
        //{
        //    PressButton("ENTER");
        //    Clipboard.SetText(text);
        //    System.Threading.Thread.Sleep(new Random().Next(50, 100));
        //    AutoItX.Send("^V");
        //    System.Threading.Thread.Sleep(new Random().Next(50, 100));
        //    PressButton("ENTER");
        //}

        //public static void MouseMoveInMow(int x, int y, int speed = 2)
        //{
        //    EmulateMouseMove.EmulateMoveMouse(wowRect.Left + x, wowRect.Top + y, speed);
        //}

        //public static bool PressButton(string button, int time = -1) // uses for pressbutton
        //{
        //    if (time == -1)
        //    {
        //        time = new Random().Next(50, 110);
        //    }
        //    lock (balanceLock)
        //    {
        //        StackTrace stackTrace = new StackTrace();
        //        Console.WriteLine(stackTrace.GetFrame(1).GetMethod().Name);
        //        //Debug.AddDebugRecord(DateTime.Now + " " + DateTime.Now.Millisecond + " | Press Button : " + button, true);
        //        DownButton(button);
        //        System.Threading.Thread.Sleep(time);
        //        //Debug.AddDebugRecord(DateTime.Now + " " + DateTime.Now.Millisecond + " | UnPress Button : " + button, false);
        //        UnButton(button);
        //    }
        //    return true;

        //}

        public static bool DownButton(string button) // uses for press button while not unpress it
        {
            AutoItX.Send("{" + button + " down}");
            return true;
        }

        public static bool UnButton(string button) // unpress method upper
        {
            AutoItX.Send("{" + button + " up}");
            return true;
        }


        public static string GenerateName(int len)
        {
            Random r = new Random();
            string[] consonants = { "b", "c", "d", "f", "g", "h", "j", "k", "l", "m", "l", "n", "p", "q", "r", "s", "sh", "zh", "t", "v", "w", "x" };
            string[] vowels = { "a", "e", "i", "o", "u", "ae", "y" };
            string Name = "";
            Name += consonants[r.Next(consonants.Length)].ToUpper();
            Name += vowels[r.Next(vowels.Length)];
            int b = 2;
            while (b < len)
            {
                Name += consonants[r.Next(consonants.Length)];
                b++;
                Name += vowels[r.Next(vowels.Length)];
                b++;
            }

            return Name;
        }

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CURSORINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        public static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DrawIconEx(IntPtr hdc, int xLeft, int yTop, IntPtr hIcon, int cxWidth, int cyHeight, int istepIfAniCur, IntPtr hbrFlickerFreeDraw, int diFlags);

        public const Int32 CURSOR_SHOWING = 0x0001;
        public const Int32 DI_NORMAL = 0x0003;

      
    }

    public class EmulateMouseMove
    {

        static readonly Random random = new Random();

        public static void EmulateMoveMouse(int x, int y, int Speed = 4)
        {
            GetCursorPos(out System.Drawing.Point c);
            WindMouse(c.X, c.Y, x, y, 9.0, 3.0, 5.0 / Speed, 7.5 / Speed, 5.0 * Speed, 10f * Speed);
        }

        private static void WindMouse(double xs, double ys, double xe, double ye, double gravity, double wind, double minWait, double maxWait, double maxStep, double targetArea)
        {

            double dist, windX = 0, windY = 0, veloX = 0, veloY = 0, randomDist, veloMag, step;
            int oldX, oldY, newX = (int)Math.Round(xs), newY = (int)Math.Round(ys);

            double sqrt2 = Math.Sqrt(2.0);
            double sqrt3 = Math.Sqrt(3.0);
            double sqrt5 = Math.Sqrt(5.0);

            dist = Hypot(xe - xs, ye - ys);

            while (dist > 1.0)
            {

                wind = Math.Min(wind, dist);

                if (dist >= targetArea)
                {
                    int w = random.Next((int)Math.Round(wind) * 2 + 1);
                    windX = windX / sqrt3 + (w - wind) / sqrt5;
                    windY = windY / sqrt3 + (w - wind) / sqrt5;
                }
                else
                {
                    windX /= sqrt2;
                    windY /= sqrt2;
                    if (maxStep < 3)
                        maxStep = random.Next(3) + 3.0;
                    else
                        maxStep /= sqrt5;
                }

                veloX += windX;
                veloY += windY;
                veloX += gravity * (xe - xs) / dist;
                veloY += gravity * (ye - ys) / dist;

                if (Hypot(veloX, veloY) > maxStep)
                {
                    randomDist = maxStep / 2.0 + random.Next((int)Math.Round(maxStep) / 2);
                    veloMag = Hypot(veloX, veloY);
                    veloX = veloX / veloMag * randomDist;
                    veloY = veloY / veloMag * randomDist;
                }

                oldX = (int)Math.Round(xs);
                oldY = (int)Math.Round(ys);
                xs += veloX;
                ys += veloY;
                dist = Hypot(xe - xs, ye - ys);
                newX = (int)Math.Round(xs);
                newY = (int)Math.Round(ys);

                if (oldX != newX || oldY != newY)
                    SetCursorPos(newX, newY);

                int wait = (int)Math.Round(/*waitDiff  (step / maxStep)*/ +minWait);
                Thread.Sleep(wait);
            }

            int endX = (int)Math.Round(xe);
            int endY = (int)Math.Round(ye);
            if (endX != newX || endY != newY)
                SetCursorPos(endX, endY);
        }

        static double Hypot(double dx, double dy)
        {
            return Math.Sqrt(dx * dx + dy * dy);
        }

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out System.Drawing.Point p);
    
    #endregion
    }       
}
