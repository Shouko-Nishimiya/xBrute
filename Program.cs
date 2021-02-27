using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xBrute
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "xBrute v0.02";
	    Brute();
            
        }
        public static void Brute()
        {
            Console.Clear();
            Colorful.Console.Write("  [-] ", Color.Aqua);
            Colorful.Console.Write("How many threads: ", Color.White);
            try
            {
                Request.threads = int.Parse(Console.ReadLine());
            }
            catch
            {
                Request.threads = 80;
            }

            Console.Clear();
            for (; ; )
            {
                Colorful.Console.Write("  [-] ", Color.Aqua);
                Colorful.Console.Write("What type of proxies - HTTP | SOCKS4 | SOCKS5 -: ", Color.White);
                Request.proxyType = Console.ReadLine();
                Request.proxyType = Request.proxyType.ToUpper();
                if (Request.proxyType.Contains("HTTP") || Request.proxyType.Contains("SOCKS4") || Request.proxyType.Contains("SOCKS5"))
                {
                    break;
                }
                Colorful.Console.Write("  > Please select a valid proxyType", Color.Red);
            }

            Thread.Sleep(2000);
            Console.Clear();
            Task.Factory.StartNew(delegate ()
            {
                Request.updateTitle();
            });


            string fileName;
            OpenFileDialog openFileDialog = new OpenFileDialog();

            do
            {
                Colorful.Console.Write("  [-] ", Color.Aqua);
                Colorful.Console.Write("Load your combos..", Color.White);
                Thread.Sleep(500);
                openFileDialog.Title = "Select Combo List";
                openFileDialog.DefaultExt = "txt";
                openFileDialog.Filter = "Text files|*.txt";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.ShowDialog();
                fileName = openFileDialog.FileName;
            }
            while (!File.Exists(fileName));

            Request.comboList = new List<string>(File.ReadAllLines(fileName));
            Request.LoadCombos(fileName);

            Colorful.Console.Write("\n  > ", Color.Aqua);
            Colorful.Console.Write("Succesfully Loaded " + Request.totalCounter + " Combo lines", Color.White);

            Thread.Sleep(1000);
            Console.Clear();
            do
            {
                Colorful.Console.Write("  [-] ", Color.Aqua);
                Colorful.Console.Write("Load your proxies..", Color.White);
                Thread.Sleep(500);
                openFileDialog.Title = "Select Proxy List";
                openFileDialog.DefaultExt = "txt";
                openFileDialog.Filter = "Text files|*.txt";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.ShowDialog();
                fileName = openFileDialog.FileName;
            }
            while (!File.Exists(fileName));

            Request.proxyList = new List<string>(File.ReadAllLines(fileName));
            Request.Loadproxies(fileName);
            Colorful.Console.Clear();

            Colorful.Console.Write("  > ", Color.Aqua);
            Colorful.Console.Write("Succesfully Loaded " + Request.proxyTotalCounter + " Proxy lines", Color.White);

            Thread.Sleep(1000);
            Console.Clear();

            for (int i = 0; i <= Request.threads; i++)
            {
                new Thread(new ThreadStart(Request.Check)).Start();
            }

            Colorful.Console.ReadLine();
            Environment.Exit(0);
        }
    }
    
}
