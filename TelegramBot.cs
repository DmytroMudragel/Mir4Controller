using System;
using AutoIt;
using System.Drawing;
using Telegram.Bot;
using System.IO;
using Telegram.Bot.Types.InputFiles;
using System.Windows;
using System.Threading;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Mir4Controller
{
    public class TelegramBot
    {
        //Settings
        private static readonly string token = Settings.token;
        private static readonly string userId = Settings.userId;
        private static readonly string pcId = Settings.pcId;

        private const string getScreenshotBtnText = Settings.getScreenshotBtnText;
        private const string showAllmir4ProcessesBtnText = Settings.showAllmir4ProcessesBtnText;
        private const string startBtnText = Settings.startBtnText;
        private const string stopBtnText = Settings.stopBtnText;
        private const string checkCharacterState = Settings.checkCharacterState;

        //private static int numberOfMIr4ProcessesT = 0;
        //private static List<string> mir4ProcessesT = new List<string> { };
        //private static int gameClientscountT = 0;

        public static bool BotСondition = false;

        public static ITelegramBotClient botClient;


        //For submenus
        //internal enum State { None, InWhisper, Turn }
        //internal class UserState { public object State { get; set; } }
        //private static Dictionary<long, UserState> ClientStates = new Dictionary<long, UserState>();


        /// <summary>
        /// Telegram bot initialize from settings of TelegramBot class
        /// </summary>
        public static void Init()
        {
            try
            {
                botClient = new TelegramBotClient(token) { Timeout = TimeSpan.FromSeconds(10) };
                var Me = botClient.GetMeAsync().Result;
                if (Me != null && !string.IsNullOrEmpty(Me.FirstName))
                {
                    SendTelegramMessage("Mir controller is ready");
                }
                GetUpdates();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        /// <summary>
        /// Sends text
        /// </summary>
        /// <param name="textToSend"></param>
        public static void SendTelegramMessage(string textToSend)
        {
            try
            {
                botClient.SendTextMessageAsync(chatId: userId, text: textToSend, replyMarkup: ControllerDobuttons());
            }
            catch (Exception e)
            {
                Debug.AddDebugRecord(e.Message.ToString(), false);
            }

        }

        /// <summary>
        /// Sends text and image of active window 
        /// </summary>
        /// <param name="textToSend"></param>
        /// <param name="photoToSend"></param>
        public static void SendTelegramMessage(string textToSend, Bitmap photoToSend)
        {
            try
            {
                photoToSend.Save("tg.png");
                FileStream fs = File.OpenRead("tg.png");
                InputOnlineFile inputOnlineFile = new InputOnlineFile(fs, pcId + textToSend + ".png");
                botClient.SendDocumentAsync(userId, inputOnlineFile).Wait();
            }
            catch (Exception e)
            {
                Debug.AddDebugRecord(e.Message.ToString(), false);
            }

        }

        /// <summary>
        /// Sends message to telegram when application closed 
        /// </summary>
        public static void MirControllerWasClosed()
        {
            if (botClient != null) botClient.SendTextMessageAsync(chatId: userId, text: "MirController was closed", replyMarkup: new ReplyKeyboardRemove());
        }

        /// <summary>
        /// Checks whether the user sent a new task 
        /// </summary>
        public static void GetUpdates()
        {
            int offset = 0;
            while (true)
            {
                try
                {
                    var Updates = botClient.GetUpdatesAsync(offset).Result;
                    if (Updates != null && Updates.Length > 0)
                    {
                        foreach (var update in Updates)
                        {
                            ProcessUpdate(update);
                            offset = update.Id + 1;
                        }
                    }
                }
                catch (Exception ex) { Debug.AddDebugRecord(ex.Message.ToString(), false); }
                Thread.Sleep(1000);
            }
        }

        private static void ProcessUpdate(global::Telegram.Bot.Types.Update update)
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    {
                        string text = update.Message.Text;
                        switch (text)
                        {
                            case startBtnText:
                                {
                                    BotСondition = true;
                                    Mir4WindowsController.controllerState = true;
                                }
                                break;
                            case stopBtnText:
                                {
                                    BotСondition = false;
                                    Mir4WindowsController.controllerState = false;
                                }
                                break;
                            case checkCharacterState:
                                {
                                    try
                                    {
                                        bool IsBlack0 = Mir4WindowsController.IsPixelBlack(Settings.Mir4Window0, Settings.WindowToFindClass, Settings.mir0CoordsX, Settings.mir0CoordsY);
                                        bool IsBlack1 = Mir4WindowsController.IsPixelBlack(Settings.Mir4Window1, Settings.WindowToFindClass, Settings.mir1CoordsX, Settings.mir1CoordsY);
                                        bool IsBlack2 = Mir4WindowsController.IsPixelBlack(Settings.Mir4Window2, Settings.WindowToFindClass, Settings.mir2CoordsX, Settings.mir2CoordsY);
                                        if (IsBlack0 && IsBlack1 && IsBlack2)
                                        {
                                            Mir4WindowsController.controllerState = false;
                                            BotСondition = false;
                                            ScreenshotHandler.Delay(5000);
                                            Mir4WindowsController.CheckCharracterState(Settings.Mir4Window0, Settings.WindowToFindClass, Settings.mir0CoordsX, Settings.mir0CoordsY);
                                            ScreenshotHandler.Delay(500);
                                            Mir4WindowsController.CheckCharracterState(Settings.Mir4Window1, Settings.WindowToFindClass, Settings.mir1CoordsX, Settings.mir1CoordsY);
                                            ScreenshotHandler.Delay(500);
                                            Mir4WindowsController.CheckCharracterState(Settings.Mir4Window2, Settings.WindowToFindClass, Settings.mir2CoordsX, Settings.mir2CoordsY);
                                            ScreenshotHandler.Delay(5000);
                                            BotСondition = true;
                                            Mir4WindowsController.controllerState = true;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.AddDebugRecord(ex.Message.ToString(), true);
                                        botClient.SendTextMessageAsync(chatId: userId, text: pcId + " | Something happend:" + ex.Message.ToString() + " cannot take screen.", replyMarkup: ControllerDobuttons());
                                    }
                                }
                                break;
                            case getScreenshotBtnText:
                                {
                                    try
                                    {
                                        Rectangle bounds = new Rectangle(0, 0, (int)SystemParameters.VirtualScreenWidth, (int)SystemParameters.VirtualScreenHeight);
                                        Bitmap Screenshot = new Bitmap(bounds.Width, bounds.Height);
                                        using (Graphics graphics = Graphics.FromImage(Screenshot))
                                        {
                                            graphics.CopyFromScreen(new System.Drawing.Point(bounds.Left, bounds.Top), System.Drawing.Point.Empty, bounds.Size);
                                        }
                                        Screenshot.Save("tg.png");
                                        InputOnlineFile inputOnlineFile = new InputOnlineFile(File.OpenRead("tg.png"), pcId + " screenshot.png");
                                        botClient.SendDocumentAsync(userId, inputOnlineFile).Wait();
                                        Screenshot.Dispose();
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.AddDebugRecord(ex.Message.ToString(), true);
                                        botClient.SendTextMessageAsync(chatId: userId, text: pcId + " | Something happend:" + ex.Message.ToString() + " cannot take screen.", replyMarkup: ControllerDobuttons());
                                    }
                                }
                                break;
                            case showAllmir4ProcessesBtnText:
                                {
                                    try
                                    {
                                        Mir4WindowsController.GetMir4Window(Settings.Mir4Window0);
                                        Mir4WindowsController.GetMir4Window(Settings.Mir4Window1);
                                        Mir4WindowsController.GetMir4Window(Settings.Mir4Window2);
                                    }
                                    catch (Exception ex)
                                    {
                                        Debug.AddDebugRecord(ex.Message.ToString(), true);
                                        botClient.SendTextMessageAsync(chatId: userId, text: pcId + " | Something happend:" + ex.Message.ToString() + " cannot take screen.", replyMarkup: ControllerDobuttons());
                                    }
                                }
                                break;
                            default:
                                SendTelegramMessage("Choose another action");
                            break;
                        }
                        break;
                    }
                    break;
                default:
                    SendTelegramMessage("This type of message is not supported");
                    break;
            }
        }

        private static IReplyMarkup ControllerDobuttons()
        {
            var Keyboard = new List<List<KeyboardButton>>
                {
                    new List<KeyboardButton> { new KeyboardButton(startBtnText), new KeyboardButton(getScreenshotBtnText), new KeyboardButton(showAllmir4ProcessesBtnText) },
                    new List<KeyboardButton> { new KeyboardButton(stopBtnText), new KeyboardButton(checkCharacterState) }
                };
            return new ReplyKeyboardMarkup(Keyboard);
        }

    }
}


