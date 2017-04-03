using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSS.UniverseLogic;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace TSS.ConsoleOutput
{
    public class ConsoleOutputManager
    {
        IUniverse universe;
        int width, height;
        string borderStr;
        bool disposed;

        public IUniverse GetUniverse()
        {
            return universe; 
        }
        public ConsoleOutputManager(IUniverse universe)
        {
            this.universe = universe;
            width = universe.GetWidth();
            height = universe.GetHeight();
            borderStr += '+';
            for (int i = 0; i < width; i++)
            {
                borderStr += '-';
            }
            borderStr += '+';
            disposed = false;
           
        }
        public void StartSimulation()
        {
            new Thread(() =>
            {
                AppDomain.CurrentDomain.ProcessExit += 
                delegate {
                    Thread.CurrentThread.Abort();
                };
                Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
                while (!disposed)
                {
                    GetUniverse().DoUniverseTick();
                    PrintFrame();
                    Thread.Sleep(ConstsUniverse.Special_PrintingPause);
                }
            }).Start();
            new Thread(() =>
            {
                AppDomain.CurrentDomain.ProcessExit +=
                delegate
                {
                    Thread.CurrentThread.Abort();
                };
                while (!disposed)
                {
                    UpdateConsts();
                    Thread.Sleep(3000);
                }
            }).Start();

        }
        public void UpdateConsts()
        {
            string configsPath = Environment.CurrentDirectory + @"\program_configs.txt";
            string saveFile = Environment.CurrentDirectory + @"\saved_un.txt";
            Process proc = new Process();
            proc.StartInfo.FileName = configsPath;

            try
            {
                ConstsUniverse.FromXmlString(
                    File.ReadAllText(configsPath)
                    );
                char key = Console.ReadKey(true).KeyChar;
                if(key == 's')
                {
                    File.WriteAllText(
                    saveFile, BinarySerializer.ToBase64String( GetUniverse())
                    );
                }
                if (key != 'c')
                    return;
                proc.Start();
                while (!proc.HasExited)
                    Thread.Sleep(500);
                
            }
            catch
            {
                File.WriteAllLines(
                    configsPath, ConstsUniverse.ToXmlString().Split('\n')
                    );
            }

        }
        void PrintFrame()
        {
            string outputStr = @"";
            outputStr+= GetFieldString(GetUniverse().GetAllDescriptors());
            if(ConstsUniverse.Special_EnableStatistics)
                outputStr += "\n\n" + GetInfoString();
            Console.Clear();
            Console.WriteLine(outputStr);
        }
        string GetFieldString(int[,] desc)
        {
            char cell = 'O';
            char deadCell = '#', food = '*', empty = ' ';

            string outputStr = borderStr + '\n';
            for (int j = 0; j < height; j++)
            {
                outputStr += '|';
                for (int i = 0; i < width; i++)
                {
                    int descriptor = desc[i,j];
                    if (descriptor == 0)
                    {
                        outputStr += empty;
                    }
                    else if (descriptor == -1)
                    {
                        outputStr += food;
                    }
                    else if (descriptor == -2)
                    {
                        outputStr += deadCell;
                    }
                    else
                    {
                        outputStr += cell;
                    }
                }
                outputStr += "|\n";
            }
            outputStr += borderStr;
            return outputStr;
        }
        string GetInfoString()
        {
            string info = @"Statistics";
            info += string.Format("\nWidth: {0}, ", width);
            info += string.Format("Height: {0};", height);
            info += string.Format("\nTick number: {0};", universe.GetTicksCount());

            Tuple<ICell, int> mostFit = GetUniverse().GetMostFit();
            if (mostFit.Item1 == null)
                info += "\nNone cell;";
            else
            {
                ICell cell = mostFit.Item1;
                int cellsCount = mostFit.Item2;
                info += string.Format("\nCells count: {0};\nMost fit genome({1} cells):", universe.GetCellsCount(), cellsCount);

                info += string.Format("\n\tDescriptor: {0};\n\tHunger = {1};\n\tAggression: {2};\n\tFriendly: {3};\n\tReproduction: {4};",
                    cell.GetDescriptor(),
                    cell.GetGenome().GetHunger(),
                    cell.GetGenome().GetAggression(),
                    cell.GetGenome().GetFriendly(),
                    cell.GetGenome().GetReproduction()
                    );  
            }
            return info;
        }

    }

    
}
