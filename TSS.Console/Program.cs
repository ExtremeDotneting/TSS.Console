using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSS.UniverseLogic;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;


namespace TSS.ConsoleOutput
{
  
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WindowWidth = Console.LargestWindowWidth;
            WindowMaximaizer.Maximize();
            Console.Title = @"TSS";

            //Universe un = new Universe(30, 15, 100);
            ////Console.Write(SerializeTest.ReflectionSerializer(un, "unv"));
            //un.DoUniverseTick(); un.DoUniverseTick(); un.DoUniverseTick(); un.DoUniverseTick(); un.DoUniverseTick();
            //Universe un2= BinarySerializer.FromBase64String<Universe>(BinarySerializer.ToBase64String(un));
            //Console.WriteLine("success");


            //ConsoleOutputManager consoleOutputManager2 = new ConsoleOutputManager(un2);
            //consoleOutputManager2.StartSimulation();

            //return;

            string tssLabel = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n                     _______  __   __  _______    _______  _______  ______    _______  __    _  _______  _______  _______  _______   \r\n                    |       ||  | |  ||       |  |       ||       ||    _ |  |       ||  |  | ||       ||       ||       ||       |  \r\n                    |_     _||  |_|  ||    ___|  |  _____||_     _||   | ||  |   _   ||   |_| ||    ___||    ___||  _____||_     _|  \r\n                      |   |  |       ||   |___   | |_____   |   |  |   |_||_ |  | |  ||       ||   | __ |   |___ | |_____   |   |    \r\n                      |   |  |       ||    ___|  |_____  |  |   |  |    __  ||  |_|  ||  _    ||   ||  ||    ___||_____  |  |   |    \r\n                      |   |  |   _   ||   |___    _____| |  |   |  |   |  | ||       || | |   ||   |_| ||   |___  _____| |  |   |    \r\n                      |___|  |__| |__||_______|  |_______|  |___|  |___|  |_||_______||_|  |__||_______||_______||_______|  |___|    \r\n                                       _______  __   __  ______    __   __  ___   __   __  _______                                   \r\n                                      |       ||  | |  ||    _ |  |  | |  ||   | |  | |  ||       |                                  \r\n                                      |  _____||  | |  ||   | ||  |  |_|  ||   | |  |_|  ||    ___|                                  \r\n                                      | |_____ |  |_|  ||   |_||_ |       ||   | |       ||   |___                                   \r\n                                      |_____  ||       ||    __  ||       ||   | |       ||    ___|                                  \r\n                                       _____| ||       ||   |  | | |     | |   |  |     | |   |___                                   \r\n                                      |_______||_______||___|  |_|  |___|  |___|   |___|  |_______|                                  ";
            Console.Write(tssLabel);
            Console.Write("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nWelcome to my evolution simulator. Read more in manual.\nTo continue press enter...");
            Console.ReadLine();
            Console.Clear();

            Universe universe = null;
            Console.WriteLine("Did you want to load previous universe? (y/n)");
            char key = Console.ReadKey().KeyChar;
            
            if (key == 'y')
            {
                try
                {
                    string saveFile = Environment.CurrentDirectory + @"\saved_un.txt";
                    Console.Clear();
                    universe = BinarySerializer.FromBase64String<Universe>(File.ReadAllText(saveFile));
                }
                catch
                {
                    key = 'n';
                    Console.WriteLine("Load Universe exception!");
                }
            }
            if(key !='y')
            {
                int width = 1, height = 1, cellsCount = 1;
                bool haveEx = true;
                while (haveEx)
                {
                    try
                    {
                        Console.Clear();
                        Console.WriteLine("Now you can set size of field.\nDon`t set very big values, that will work slowly and take much place at screen.");
                        Console.Write(@"Insert width: ");
                        string widthStr = Console.ReadLine();
                        Console.Write(@"Insert height: ");
                        string heightStr = Console.ReadLine();
                        if (widthStr[0] == 'm')
                            width = Console.LargestWindowWidth - 8;
                        else
                            width = Convert.ToInt32(widthStr);
                        if (heightStr[0] == 'm')
                            height = Console.LargestWindowHeight - 19;
                        else
                            height = Convert.ToInt32(heightStr);

                        Console.Write("Ok.\nNow set how much cells you want generate at start: ");
                        cellsCount = Convert.ToInt32(Console.ReadLine());
                        haveEx = false;
                    }
                    catch
                    {
                        Console.WriteLine(@"Something went wrong! Try again.");
                    }
                }
                Console.WriteLine("Success! Now we are ready to start.\nIf you want to change configs press 'c' while game running, change opened file, save and close to load." +
                    "\nTo see the simulation just minimaize window.");
                Console.WriteLine(@"You can make save pressing 's' button.");
                Console.WriteLine(@"Press enter to start simulation...");
                Console.ReadLine();
                universe = new Universe(width, height, cellsCount);
            }

            ConsoleOutputManager consoleOutputManager = new ConsoleOutputManager(universe);
            consoleOutputManager.StartSimulation();
            
        }


    }

    public class WindowMaximaizer
    {
        [DllImport("user32.dll")]
        static extern bool ShowWindow(System.IntPtr hWnd, int cmdShow);

        public static void Maximize()
        {
            Process p = Process.GetCurrentProcess();
            ShowWindow(p.MainWindowHandle, 3); //SW_MAXIMIZE = 3
        }
    }



}
    