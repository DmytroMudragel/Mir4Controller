using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mir4Controller
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {     

        public MainWindow() 
        {
            InitializeComponent();
            Thread myThread = new Thread(new ThreadStart(TelegramBot.Init));
            myThread.Start();
            Thread myThread2 = new Thread(new ThreadStart(StartButtonHandler));
            myThread2.Start();
            Closed += new EventHandler(MainWindow_Closed);
        }

        private static void MainWindow_Closed(object sender, EventArgs e)
        {
            TelegramBot.MirControllerWasClosed();
            Mir4WindowsController.controllerState = false;
            TelegramBot.BotСondition = false;
            Environment.Exit(0);
            Application.Current.Shutdown();
        }

        private void Main_Click(object sender, RoutedEventArgs e)
        {
            MainTab.Visibility = Visibility.Visible;
            SettingsTab.Visibility = Visibility.Hidden;
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MainTab.Visibility = Visibility.Hidden;
            SettingsTab.Visibility = Visibility.Visible;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (startButton.Content is "Start")
            {
                startButton.Content = "Pause";
                textNotifierBox.Text = "Controller is running";
                TelegramBot.BotСondition = true;
                Mir4WindowsController.controllerState = true;
                //Thread myThread = new Thread(new ThreadStart(Mir4WindowsController.Run));
                //myThread.SetApartmentState(ApartmentState.STA);
                //myThread.Start();
                //SaveProfileSettings();
            }
            else
            {
                startButton.Content = "Start";
                textNotifierBox.Text = "Controller stops";
                Mir4WindowsController.controllerState = false;
                TelegramBot.BotСondition = false;
            }
        }


        private void StartButtonHandler()
        {
            bool prev = TelegramBot.BotСondition;
            while (true)
            {
                if (ProcessesListBox.Items.Count != 0)
                {
                    Task.Run(() => Application.Current.Dispatcher.Invoke(new Action(() => { ProcessesListBox.Items.Clear(); })));
                }
                //string res = ProcessesListBox.Items.ToString();
                //for (int i = 0; i < Mir4WindowsController.mir4Processes.Count; i++)
                //{
                //        string temp = Mir4WindowsController.mir4Processes[i];
                //    //    //ListBoxItem tmp = new ListBoxItem { Content = temp, Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#757575") };
                //    //    //bool a = ProcessesListBox.Items.Cast<string>().Any(x => x == temp);
                //    //    //if (ProcessesListBox != ProcessesListBox.FindStringExact("StringToFind"))
                //    //    //{
                //    //    //    ProcessesListBox.Items.Add("StringToAdd");
                //    //    //}
                //    //    //var item = ProcessesListBox.Items.findby("hello"); // or FindByText
                //    //   // if (ProcessesListBox.Items.Cast<ListBoxItem>().Any(x => x.Content != temp))
                //    //   ///{
                //    //        if (i < Mir4WindowsController.mir4Processes.Count)
                //    if (Mir4WindowsController.mir4Processes.Contains(temp) && (ProcessesListBox.Items[i].ToString() != temp))
                //    {
                //        Task.Run(() => Application.Current.Dispatcher.Invoke(new Action(() => { ProcessesListBox.Items.Add(new ListBoxItem { Content = Mir4WindowsController.mir4Processes[i], Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#757575") }); })));
                //    }
                //}
                if (Mir4WindowsController.mir4Processes.Contains("Mir4G[0]"))
                {
                    Task.Run(() => Application.Current.Dispatcher.Invoke(new Action(() => { ProcessesListBox.Items.Add(new ListBoxItem { Content = "Mir4G[0]", Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#757575") }); })));
                }
                if (Mir4WindowsController.mir4Processes.Contains("Mir4G[1]"))
                {
                    Task.Run(() => Application.Current.Dispatcher.Invoke(new Action(() => { ProcessesListBox.Items.Add(new ListBoxItem { Content = "Mir4G[1]", Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#757575") }); })));
                }
                if (Mir4WindowsController.mir4Processes.Contains("Mir4G[2]"))
                {
                    Task.Run(() => Application.Current.Dispatcher.Invoke(new Action(() => { ProcessesListBox.Items.Add(new ListBoxItem { Content = "Mir4G[2]", Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#757575") }); })));
                }



                if (TelegramBot.BotСondition && TelegramBot.BotСondition != prev)
                {
                    Task.Run(() => Application.Current.Dispatcher.Invoke(new Action(() => { startButton.Content = "Pause"; })));
                    Task.Run(() => Application.Current.Dispatcher.Invoke(new Action(() => { textNotifierBox.Text = "Controller is running"; })));
                    Thread myThread = new Thread(new ThreadStart(Mir4WindowsController.Run));
                    myThread.SetApartmentState(ApartmentState.STA);
                    myThread.Start();
                    //SaveProfileSettings();
                    prev = TelegramBot.BotСondition;
                    //TelegramBot.SendMessage("Controller is running", false);
                }

                if (!TelegramBot.BotСondition && TelegramBot.BotСondition != prev)
                {
                    Task.Run(() => Application.Current.Dispatcher.Invoke(new Action(() => { startButton.Content = "Start"; })));
                    Task.Run(() => Application.Current.Dispatcher.Invoke(new Action(() => { textNotifierBox.Text = "Controller stops"; })));
                    prev = TelegramBot.BotСondition;
                    //TelegramBot.SendMessage("Controller stops", false);
                }
                ScreenshotHandler.Delay(500);
            }
        }

        //private void RunbuttonClick(object sender, RoutedEventArgs e)
        //{
        //    if (Runbutton.Content is "Run")
        //    {
        //        //if (!File.Exists(GlobalBotHandler.CurrentProfilePath) || !File.Exists(GlobalBotHandler.CurrentRoutePath))
        //        //{
        //        //    MessageBox.Show("Route or profile file not exist!");
        //        //    return;
        //        //}
        //        //ProfileSettings.ProfileLocation = GlobalBotHandler.CurrentProfilePath;
        //        //MeshHandler.MeshPath = GlobalBotHandler.CurrentRoutePath;
        //        //if (!ProfileSettings.ConfigReader() & !MeshHandler.ReadMesh())
        //        //{
        //        //    MessageBox.Show("Error while reading files!");
        //        //    return;
        //        //}
        //        //Runbutton.Content = "Pause";
        //        //textNotifierbox.Text = "Bot is running";
        //        TelegramBot.BotСondition = true;/////
        //        //Thread myThread = new Thread(new ThreadStart(GlobalBotHandler.RunBot));
        //        //myThread.SetApartmentState(ApartmentState.STA);
        //        //myThread.Start();
        //        //Console.WriteLine("Profiles was successfully loaded");
        //        //SaveProfileSettings();
        //    }
        //    else
        //    {
        //        //GlobalBotHandler.BotStop = true;
        //        TelegramBot.BotСondition = false;/////
        //        //Runbutton.Content = "Run";
        //        //textNotifierbox.Text = "Bot stopped or will stop soon..";
        //    }

        //}
    }
}

