using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiteScanner
{

    public static class Network
    {
        static string serverURL;
        static readonly object locker = new object();


        public static void UpdateServerURL()
        {
            var serverURLtemp = FileSystem.ReadFromFile("serverURL.txt");
            serverURL = serverURLtemp[0];
        }

        public static string ReadHttpResponse(HttpWebResponse response)
        {
            var encoding = ASCIIEncoding.ASCII;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                return reader.ReadToEnd();
            }
        }


        public static string GetServerRequestResult(string url)
        {
            lock (locker)
            {
                while (true)
                {
                    try
                    {
                        HttpWebResponse response = ServerRequest(url);
                        string result = ReadHttpResponse(response);
                        return result;
                    }
                    catch (Exception e)
                    {
                        ConsoleEffects.ConsolePrintColor($"Error occured while connecting server: {e}");
                        Thread.Sleep(5000);
                    }
                }
            }
        }

        public static HttpWebResponse ServerRequest(string url)
        {
            lock (locker)
            {
                while (true)
                {
                    try
                    {
                        HttpWebRequest request = GetHttpWebRequest(serverURL + url, 20);
                        return (HttpWebResponse)request.GetResponse();
                    }
                    catch (System.Net.WebException e)
                    {
                        ConsoleEffects.ConsolePrintColor($"Error occured connecting to the Server: {e}");
                        Thread.Sleep(5000);
                    }
                }
            }

        }

        public static HttpWebRequest GetHttpWebRequest(string url, int timeout)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = timeout * 1000;
            return request;
        }

        public static async Task<bool> ProxyRequestAsync(string url, string proxy_ip_and_port, int timeout = 5)
        {

            try
            {
                WebProxy myproxy = new WebProxy(proxy_ip_and_port);
                HttpWebRequest request = GetHttpWebRequest(url, timeout);
                request.Proxy = myproxy;


                using (var response = (HttpWebResponse)await Task.Factory
        .FromAsync<WebResponse>(request.BeginGetResponse,
                                request.EndGetResponse,
                                null))
                {
                    if (response.StatusCode == HttpStatusCode.OK) return true;
                }
            }
            catch (System.Net.WebException)
            {
                // No response
            }
            catch (System.UriFormatException)
            {
                // Incorrect format
            }

            return false;
        }


    }
}
