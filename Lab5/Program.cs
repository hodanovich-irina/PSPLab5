using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Lab5
{
    class Program
    {
        public static string SuccessHeaders(int contentLength)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("HTTP/1.1 200 OK").Append("\r\n");
            builder.Append("Date: ").Append(DateTime.Now).Append("\r\n");
            builder.Append("Content-Type: text/html; charset=UTF-8").Append("\r\n");
            builder.Append("Content-Length: ").Append(contentLength).Append("\r\n");
            builder.Append("Connection: close").Append("\r\n");
            builder.Append("\r\n");
            return builder.ToString();
        }

        private static string AnswerPage(string val)
        {
            StringBuilder bodyBuilder = new StringBuilder();
            bodyBuilder.Append("Answer:");
            bodyBuilder.Append("<br><br>");
            bodyBuilder.Append(val);
            bodyBuilder.Append("<br>");
            bodyBuilder.Append("<br>");
            bodyBuilder.Append("<a href='/'> Home page </a> ");
            string body = bodyBuilder.ToString();
            return string.Concat(SuccessHeaders(body.Length), body);
        }

        private static string BadAnswerPage()
        {
            StringBuilder bodyBuilder = new StringBuilder();
            bodyBuilder.Append("Something was bad");
            bodyBuilder.Append("<br>");
            bodyBuilder.Append("<a href='/'> Save result </a> ");
            string body = bodyBuilder.ToString();
            return string.Concat(SuccessHeaders(body.Length), body);
        }

        private static string IndexPage()
        {
            StringBuilder bodyBuilder = new StringBuilder();
            bodyBuilder.Append("<form method='post'>");
            bodyBuilder.Append("Enter matrix and vector:");
            bodyBuilder.Append("<br>");
            bodyBuilder.Append("<textarea type='text' name='val' style='width:500px;height:500px;'></textarea>");
            bodyBuilder.Append("<br>");
            bodyBuilder.Append("<input type='submit' >");
            bodyBuilder.Append("</form>");
            string body = bodyBuilder.ToString();
            return string.Concat(SuccessHeaders(body.Length), body);
        }

        static void ProcessClient(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream clientStream = client.GetStream();
            try
            {
                clientStream.ReadTimeout = 200;
                clientStream.WriteTimeout = 200;

                Console.WriteLine("Waiting for client message...");
                string messageData = ReadMessage(clientStream);

                Console.WriteLine("request: ");
                Console.WriteLine(messageData);
                string page = "";
                if (messageData.StartsWith("POST"))
                {
                    Regex r = new Regex("\r\n\r\n");
                    string[] request = r.Split(messageData, 2);
                    if (request.Length < 1)
                    {
                        page = BadAnswerPage();
                    }
                    else
                    {
                        Regex reg = new Regex("%0D%0A");
                        string val = request[1].Split('=')[1];
                        var newVal = reg.Split(val, 5);
                        string matrix = "";
                        string vector = "";
                        for (int i = 0; i < newVal.Length; i++) 
                        {
                            if (i == newVal.Length - 1)
                            {
                                var vecRes = newVal[i].Split('+');
                                for (int h = 0; h < vecRes.Length; h++)
                                {
                                    if (h != vecRes.Length - 1)
                                        vector += vecRes[h] + " ";
                                    else
                                        vector += vecRes[h];
                                }
                            }
                            else
                            {
                                var matrRes = newVal[i].Split('+');
                                for (int h = 0; h < matrRes.Length; h++)
                                {
                                    if (h == matrRes.Length - 1 && i == newVal.Length - 2)
                                        matrix += matrRes[h];
                                    else
                                        matrix += matrRes[h] + " ";
                                }
                            }

                        }
                        string result1 = Resolver.Result(Resolver.ReadMatrix(matrix), Resolver.ReadVector(vector));
                        page = AnswerPage(result1);
                    }
                }
                else
                {
                    page = IndexPage();
                }
                Console.WriteLine("response:");
                Console.WriteLine(page);

                byte[] message = Encoding.UTF8.GetBytes(page);
                clientStream.Write(message, 0, message.Length);
                clientStream.Flush();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                clientStream.Close();
                client.Close();
            }
        }

        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8080);

            listener.Start();

            Console.WriteLine(listener.LocalEndpoint);

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(ProcessClient, client);
            }
        }

        static string ReadMessage(NetworkStream clientStream)
        {
            StringBuilder messageData = new StringBuilder();
            try
            {
                byte[] buffer = new byte[2048];
                int bytes = -1;

                do
                {
                    bytes = clientStream.Read(buffer, 0, buffer.Length);
                    Decoder decoder = Encoding.UTF8.GetDecoder();
                    char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                    decoder.GetChars(buffer, 0, bytes, chars, 0);
                    messageData.Append(chars);
                }
                while (bytes != 0);
            }
            catch (Exception)
            {

            }

            return messageData.ToString();
        }
    }
}