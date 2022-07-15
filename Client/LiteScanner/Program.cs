using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace LiteScanner
{

    class Program
    {

        static string client_cpuInfo = "unknown";
        static string client_Name = "unknown";
        static string client_SystemName = "unknown";
        static string client_Description = "unknown";

        static string user_hash;

        static string externalIP = "unknown";

        static bool auto = true;
        static bool scanAllPorts = false;
        static bool scanRandomIPs = false;

        static string oneSelectedPort = "8080";
        static int tasksCount = 100;

        // Requests Timeout (Seconds)
        static int timeout = 5;

        static readonly object locker = new object();

        static int allScannedProxiesCounter = 0;
        static int goodProxiesCounter = 0;

        static List<string> octet1 = new List<string>();
        static List<string> octet2 = new List<string>();
        static List<string> octet3 = new List<string>();
        static List<string> octet4 = new List<string>();
        static List<string> ports = new List<string>();

        static List<string> websites = new List<string>();

        static int scanResultsCursorTop = 0;

        static readonly int updateTimerFrequency = 5; // Seconds
        static readonly System.Timers.Timer updateTimer = new System.Timers.Timer();


        static void Main(string[] args)
        {

            Console.Write("loading...");

            ParseArgs(args);

            UpdateServerURL();

            // Getting client hardware and software fingerprint using System.Management.ManagementClass
            GetHardwareFingerprint();

            GetClientIP();

            Console.Clear();

            DrawLogo();

            // Testing connection and also displaying a message from the server is there is some
            TestConnection();

            // Authorization by the hashcode generated from hardware fingerprint
            Authorize();

            // Client can see only the proxies discovered by himself
            DisplayAllProxiesFoundByClient();

            // Displaying other clients highscores
            DisplayHighScores();

            DisplaySettings();

            // If startup argument "random" is not given, we use proxies statistics provided in octet{n}.txt files to improve probability of discovering new proxies
            LoadStatistics();

            // User can set amount of tasks here in manual mode (if startup argument "manual" is given)
            GetTasksCount();

            StartScanning();

            // Working until the user presses the Enter key
            Console.ReadLine();

        }

        static void UpdateServerURL()
        {
            try
            {
                Network.UpdateServerURL();
            } catch(Exception e)
            {
                ShowErrorAndExit(e.Message, 1);
            }
        }

        static async Task<bool> CheckProxy(string ip_and_port, int tries = 1)
        {

            for (int i = 0; i < tries; i++)
            {
                Random rand = new Random();

                string address = websites[rand.Next(websites.Count)];

                bool success = await Network.ProxyRequestAsync(address, ip_and_port, timeout);

                if (success) return true;
            }

            return false;

        }

        static void ShowErrorAndExit(string msg, int exitCode, int waitTime = 5000)
        {
            ConsoleEffects.ConsolePrintColor(msg);
            Thread.Sleep(waitTime);
            Environment.Exit(exitCode);
        }

        private static void GetTasksCount()
        {
            if (!auto)
            {
                ConsoleEffects.ConsolePrintColor("Input number of tasks:");
                try
                {
                    tasksCount = Int32.Parse(Console.ReadLine());
                    ConsoleEffects.ConsolePrintColor("");
                }
                catch (Exception ex)
                {
                    ShowErrorAndExit(ex.Message, 1);
                }
            }
        }

        private static void DisplaySettings()
        {
            Thread.Sleep(100);
            if (auto) ConsoleEffects.ConsolePrintColor("Automode activated", ConsoleColor.DarkGreen); else ConsoleEffects.ConsolePrintColor("Manual mode activated", ConsoleColor.Blue);
            Thread.Sleep(100);
            if (scanAllPorts) ConsoleEffects.ConsolePrintColor("Scanning all ports"); else ConsoleEffects.ConsolePrintColor($"Scanning port {oneSelectedPort} only");
            Thread.Sleep(100);
            if (scanRandomIPs) ConsoleEffects.ConsolePrintColor("Scanning completely random IPs"); else ConsoleEffects.ConsolePrintColor("Scanning IPs according to statistics");
            Thread.Sleep(100);
        }

        private static void DisplayHighScores()
        {
            string scores_string = Network.GetServerRequestResult("getScores.php/");

            var scores = scores_string.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

            Thread.Sleep(100);

            ConsoleEffects.ConsolePrintColor("HIGH SCORES", ConsoleColor.Blue);

            Thread.Sleep(100);

            int place_n = 1;

            foreach (var line in scores)
            {
                ConsoleEffects.ConsolePrintColor(line);
                place_n++;
                Thread.Sleep(50);
            }

            ConsoleEffects.ConsolePrintColor("");
        }

        private static void DisplayAllProxiesFoundByClient()
        {
            string proxies_found = Network.GetServerRequestResult($"getAllProxiesFound.php/?user_hash={user_hash}");

            if (proxies_found != "")
            {

                var prxs = proxies_found.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

                Thread.Sleep(100);

                if (prxs.Length == 1)
                    ConsoleEffects.ConsolePrintColor($"{prxs.Length} proxy is discovered:", ConsoleColor.Blue);
                else
                    ConsoleEffects.ConsolePrintColor($"{prxs.Length} proxies are discovered:", ConsoleColor.Blue);

                Thread.Sleep(100);

                foreach (string p in prxs)
                {
                    Thread.Sleep(30);
                    ConsoleEffects.ConsolePrintColor(p);
                }

                ConsoleEffects.ConsolePrintColor("");
            }
        }

        private static void Authorize()
        {
            user_hash = Hasher.GetHashString(client_cpuInfo + client_Name + client_SystemName + client_Description);

            Network.ServerRequest($"newUser.php/?user_hash={user_hash}&ip={externalIP}&info={client_cpuInfo}\n{client_Name}\n{client_SystemName}\n{client_Description}");

            string name = Network.GetServerRequestResult($"getUserName.php/?user_hash={user_hash}");


            if (name != "")
            {
                ConsoleEffects.ConsolePrintColor($"Hi {name}!\n");
                Thread.Sleep(50);
            }
            else
            {
                ConsoleEffects.ConsolePrintColor("What is your name?", ConsoleColor.DarkGreen);
                name = Console.ReadLine();

                while (name.Length > 25)
                {
                    ConsoleEffects.ConsolePrintColor("The name is too long, please try again");
                    name = Console.ReadLine();
                }

                while (name.Length < 2)
                {
                    ConsoleEffects.ConsolePrintColor("The name is too short, please try again");
                    name = Console.ReadLine();
                }

                ConsoleEffects.ConsolePrintColor($"Nice to meet you {name}! Let's begin!\n");

                Network.ServerRequest($"setUserName.php/?name={name}&user_hash={user_hash}");

                Thread.Sleep(50);

            }
        }

        private static void TestConnection()
        {
            ConsoleEffects.ConsolePrintColor("Testing Connection", ConsoleColor.Blue);

            string textFromServer = Network.GetServerRequestResult("testConnection.php/");

            ConsoleEffects.ConsolePrintColor("Connected\n", ConsoleColor.DarkGreen);

            if (textFromServer.Length > 0)
            {
                Thread.Sleep(50);
                ConsoleEffects.ConsolePrintColor($"{textFromServer}\n");
                Thread.Sleep(1500);
            }
        }

        private static void DrawLogo()
        {
            for (int i = 0; i < 6; i++)
            {
                Console.SetCursorPosition(0, 0);
                Console.Write(ConsoleEffects.GetLogo(i));
                Thread.Sleep(150);
                continue;
            }
        }

        private static void GetClientIP()
        {
            try
            {
                externalIP = Network.GetServerRequestResult("getClientIP.php/");
            }
            catch (Exception e) { Console.Write($"Can't get client IP: {e.Message}"); }
        }

        private static void ParseArgs(string[] args)
        {
            // All available command-line arguments:

            // manual - user chooses number of scanning tasks in the app manually
            // allports - scanning ports according to the statistics
            // port:{port} - scanning only one particular port (8080 by default)
            // random - scanning IPs randomly not taking statistics into account
            // timeout:{number of seconds} - scanning requests timeout in seconds

            // Example:
            //          arguments:
            //                      150 port:3128 timeout:10 random
            //                                                      start 150 scanning tasks scanning random IPs connecting only to port 3128 with 10 seconds timeout 

            // Without any given arguments LiteScanner runs 100 tasks scanning IPs according to the statistics connecting only to port 8080 with 5 seconds timeout 

            bool tasksCoundParsed = false;

            foreach (string arg in args)
            {

                if (arg == "manual") auto = false;
                if (arg == "allports") scanAllPorts = true;
                if (arg == "random") scanRandomIPs = true;


                if (!tasksCoundParsed)
                {
                    // Let any int in arguments firt occured be the number of tasks
                    tasksCoundParsed = Int32.TryParse(arg, out int tasksCountTemp);
                    if (tasksCoundParsed) tasksCount = tasksCountTemp;

                }


                if (arg.Contains("port"))
                {
                    string[] arg_sep = arg.Split(':').ToArray();

                    if (arg_sep.Length > 1)
                    {
                        oneSelectedPort = arg_sep[1];
                    }
                }

                if (arg.Contains("timeout"))
                {
                    string[] arg_sep = arg.Split(':').ToArray();

                    try
                    {
                        timeout = Int32.Parse(arg_sep[1]);
                    }
                    catch (FormatException ex)
                    {
                        ConsoleEffects.ConsolePrintColor("arguments ERROR: incorrect request timeout");
                        ShowErrorAndExit(ex.Message, 1);
                    }
                }
            }
        }

        private static void GetHardwareFingerprint()
        {
            ManagementClass managClass = new ManagementClass("win32_processor");
            ManagementObjectCollection managCollec = managClass.GetInstances();

            // Collecting client hardware unique fingerprint
            foreach (ManagementObject managObj in managCollec)
            {
                client_cpuInfo = managObj.Properties["processorID"].Value.ToString();
                client_Name = managObj.Properties["Name"].Value.ToString();
                client_SystemName = managObj.Properties["SystemName"].Value.ToString();
                client_Description = managObj.Properties["Description"].Value.ToString();
            }
        }

        private static void LoadStatistics()
        {

            try
            {
                if (scanRandomIPs)
                {
                    octet1 = AddNumbersFrom0to255(octet1);
                    octet2 = AddNumbersFrom0to255(octet2);
                    octet3 = AddNumbersFrom0to255(octet3);
                    octet4 = AddNumbersFrom0to255(octet4);
                }
                else
                {
                    octet1 = FileSystem.ReadStatisticsFromFile("octet1.txt");
                    octet2 = FileSystem.ReadStatisticsFromFile("octet2.txt");
                    octet3 = FileSystem.ReadStatisticsFromFile("octet3.txt");
                    octet4 = FileSystem.ReadStatisticsFromFile("octet4.txt");
                }

                ports = FileSystem.ReadStatisticsFromFile("ports.txt");

                websites = FileSystem.ReadFromFile("websites.txt");
            }
            catch (FileSystemException e)
            {
                ShowErrorAndExit(e.Message, 1);
            }

            if (!scanRandomIPs) ConsoleEffects.ConsolePrintColor($"Statistics files are loaded, starting scanning... ({tasksCount} tasks)\n"); else ConsoleEffects.ConsolePrintColor($"Starting scanning... ({tasksCount} tasks)\n");
            Thread.Sleep(100);
        }

        static void StartScanning()
        {
            for (int i = 0; i < tasksCount; i++)
            {
                _ = Checker();
            }

            StartResultsDisplayTimer();

        }

        static void StartResultsDisplayTimer()
        {
            // Display results immediately
            DisplayResults(null, null);

            // And then display results every {updateTimerFrequency} seconds
            updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(DisplayResults);
            updateTimer.Interval = updateTimerFrequency * 1000;
            updateTimer.Start();
        }

        static void DisplayResults(object source, System.Timers.ElapsedEventArgs e)
        {
            lock (locker)
            {

                string msg = $"{allScannedProxiesCounter} proxies checked, {goodProxiesCounter} working proxies found";

                if (scanResultsCursorTop == 0)
                {
                    // Print on the last line and add an additional line for the first time and remember that line
                    scanResultsCursorTop = ConsoleEffects.ConsolePrintColor(msg, ConsoleColor.White, true, true);
                }
                else
                {
                    // Print on the same line every time
                    ConsoleEffects.ConsolePrintColor(msg, ConsoleColor.White, true, false, scanResultsCursorTop);
                }

            }
        }

        static string GetRandomIP_Port(Random rand)
        {
            string ip = $"{octet1[rand.Next(octet1.Count)]}.{octet2[rand.Next(octet2.Count)]}.{octet3[rand.Next(octet3.Count)]}.{octet4[rand.Next(octet4.Count)]}";
            string port = oneSelectedPort;
            if (scanAllPorts) port = ports[rand.Next(ports.Count)];
            return $"{ip}:{port}";
        }

        static string last_proxy = "";

        static string GuessIP_Port()
        {
            string ip_and_port = "";

            lock (locker)
            {
                Random rand = new Random();

                ip_and_port = GetRandomIP_Port(rand);

                while (last_proxy == ip_and_port)
                {
                    ip_and_port = GetRandomIP_Port(rand);
                    Thread.Sleep(10);
                }
                last_proxy = ip_and_port;

            }

            return ip_and_port;
        }

        static async Task Checker()
        {

            while (true)
            {

                string ip_and_port = GuessIP_Port();

                bool it_works = await CheckProxy(ip_and_port);
                IncrementAllScannedProxiesCounter();

                if (it_works)
                {
                    IncrementCountUpGoodProxiesCounter();
                    ConsoleEffects.ConsolePrintColor($"Proxy {ip_and_port} is working!", ConsoleColor.DarkCyan);
                    DisplayResults(null, null);

                    string serverResponse = Network.GetServerRequestResult($"saveProxy.php/?proxy={ip_and_port}&user_hash={user_hash}");

                    if (serverResponse == "OK")
                        ConsoleEffects.ConsolePrintColor($"Proxy {ip_and_port} is approved by Server", ConsoleColor.DarkGreen);
                    else
                        ConsoleEffects.ConsolePrintColor($"Proxy {ip_and_port} is not approved by Server", ConsoleColor.DarkRed);

                }

            }

        }

        private static void IncrementAllScannedProxiesCounter()
        {
            allScannedProxiesCounter++;
        }

        private static void IncrementCountUpGoodProxiesCounter()
        {
            goodProxiesCounter++;
        }

        static List<string> AddNumbersFrom0to255(List<string> list)
        {
            for (int i = 0; i <= 255; i++)
            {
                list.Add(i.ToString());
            }

            return list;
        }

    }
}
