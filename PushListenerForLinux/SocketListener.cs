using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PushListenerForLinux
{

    public class StateObject
    {
        // Size of receive buffer.
        public const int BufferSize = 1024;

        // Receive buffer. 
        public byte[] buffer = new byte[BufferSize];

        // Received data string.
        public StringBuilder sb = new StringBuilder();

        // Client socket.
        public Socket workSocket = null;

    }

    public class SocketListener
    {
        static SupportMethods support = new SupportMethods();
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public void StartListening()
        {
            WriteLog("In StartListening");
            // Establish the local endpoint for the socket.  
            // The DNS name of the computer  
            // running the listener is "host.contoso.com".  
            IPHostEntry ipHostinfo = Dns.GetHostEntry(Dns.GetHostName());
            ipHostinfo.AddressList[0] = IPAddress.Parse("2406:da1a:9ae:f501:3d1e:f2bf:c42b:5e85"); //for 10.4.101.183
            //ipHostinfo.AddressList[0] = IPAddress.Parse("2404:a800:4101:1d::15"); //for 10.3.101.183  2404:a800:3101:1d::56
            // ipHostinfo.AddressList[0] = IPAddress.Parse("2404:a800:3101:1d::56");//My system ip 2404:a800:3101:1d::15
            IPAddress iPAddress = ipHostinfo.AddressList[0];
            //IPAddress iPAddress = IPAddress.Parse("2404:a800:3a00:2::25e");
            //IPEndPoint localEndPoint = new IPEndPoint(iPAddress, 4060);
            IPEndPoint localEndPoint = new IPEndPoint(iPAddress, 4060);
            //Socket listener;
            //if (isIPv6Address) // USED IPV6 Address to send data 
            //{
            //    //Creating object to socket based of type of the IP address used.
            //    listener = new Socket(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp);

            //}
            //else
            //{// USED IPV4 Address to send data
            // //Creating object to socket based of type of the IP address used.
            //    listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //}
            // Create a TCP/IP socket.
            Socket listener = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            // Bind the socket to the local endpoint and listen for incoming connections.
            try
            {
                listener.Bind(localEndPoint);
                //Console.WriteLine("Client IP is : " + localEndPoint.Address);
                WriteLog("Client IP is : " + localEndPoint.Address);
                listener.Listen(100);
                while (true)
                {
                    // Set the event to nonsignaled state.
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connections.  
                    //Console.WriteLine("Waiting for a Connection....");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // Wait until a connection is made before continuing. 
                    allDone.WaitOne();
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.ToString());
                support.ErrorWriteLog(e.ToString());
            }
            //Console.WriteLine("\nPress ENTER to continue...");
            //Console.Read();
        }
        public void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                //MeterDetails details = new MeterDetails();
                //DataTable dt = new DataTable();
                // Signal the main thread to continue.  
                allDone.Set();
                // Get the socket that handles the client request. 
                Socket listener = (Socket)ar.AsyncState;

                Socket handler = listener.EndAccept(ar);
                //Console.WriteLine("Coneection has Established" + handler.Connected);
                WriteLog("Coneection has Established" + handler.Connected);

                EndPoint socketAddress = handler.RemoteEndPoint;
                //dt = details.GetMeterData(socketAddress.ToString());
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = handler;
                handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
            }
            catch (Exception e)
            {
                support.ErrorWriteLog(e.ToString());
            }
        }
        public void ReadCallback(IAsyncResult ar)
        {
            try
            {
                String content = String.Empty;

                // Retrieve the state object and the handler socket  
                // from the asynchronous state object. 
                StateObject state = (StateObject)ar.AsyncState;
                Socket handler = state.workSocket;
                SocketError errorCode;

                // Read data from the client socket.
                int bytesRead = handler.EndReceive(ar, out errorCode);
                //int bytesRead = handler.EndReceive(ar);
                EndPoint socketAddress = handler.RemoteEndPoint;
                string toBeSearched1 = "]";
                string ReceivedClientIP = socketAddress.ToString();
                ReceivedClientIP = ReceivedClientIP.Substring(0, ReceivedClientIP.IndexOf(toBeSearched1) + toBeSearched1.Length);
                string[] IPport = socketAddress.ToString().Split(':');

                if (bytesRead > 0)
                {
                    // There  might be more data, so store the data received so far.
                    state.sb.Append(BitConverter.ToString(state.buffer, 0, bytesRead));
                    // Check for end-of-file tag. If it is not there, read
                    // more data.
                    content = state.sb.ToString();

                    //support.WriteLog(content);
                    //WriteLog(content);
                    if (content.Length > 0)
                    {
                        // All the data has been read from the
                        // client. Display it on the console.


                        state.sb.Clear();

                        // Echo the data back to the client.
                        //Send(handler, content);
                        string receivedtext = string.Empty;
                        receivedtext = content.Replace(@"-", string.Empty);

                        //Console.WriteLine("IP: " + ReceivedClientIP + "\n" + "Read Data from Socket: " + receivedtext + "\n" + "Data Pushed Time:" + DateTime.Now);
                        //WriteLog("IP: " + ReceivedClientIP + "\n" + "Read Data from Socket: " + receivedtext + "\n" + "Data Pushed Time:" + DateTime.Now);
                        receivedtext = ReceivedClientIP + "^" + receivedtext + "^" + DateTimeOffset.Now;
                        //support.WriteLog(receivedtext);
                        support.WriteLogWithIP(ReceivedClientIP, receivedtext);
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);

                        PublishTexttoQueue(receivedtext);
                    }
                    else
                    {
                        // Not all data received. Get more.  
                        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                    }
                }
                else if (errorCode != SocketError.Success)
                {
                    bytesRead = 0;
                    ///Console.WriteLine(IPport[0] + " Client Disconnected. . .");
                    WriteLog(IPport[0] + " Client Disconnected. . .");
                }
            }
            catch (Exception e)
            {
                support.ErrorWriteLog(e.ToString());
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.  
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.  
                int bytesSent = handler.EndSend(ar);
                WriteLog("Sent {0} bytes to client." + bytesSent);

                // handler.Shutdown(SocketShutdown.Both);
                // handler.Close();

            }
            catch (Exception e)
            {
                WriteLog(e.ToString());
            }
        }

        public void WriteLog(string Message)
        {
            try
            {
                var FileLocation = Directory.GetCurrentDirectory() + @"\" + DateTime.Now.ToString("yyyyMMdd") + "TCPLog.txt";

                if (!File.Exists(FileLocation))
                {
                    using (FileStream fs = File.Create(FileLocation))
                    {
                        //WriteLog("File Created");
                    }
                }

                ArrayList arrcontent = new ArrayList();

                if (!string.IsNullOrEmpty(FileLocation))
                {
                    if (File.Exists(FileLocation))
                    {
                        arrcontent.AddRange(File.ReadLines(FileLocation).ToArray());
                        arrcontent.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " ==> " + Message);
                        File.WriteAllText(FileLocation, String.Empty);
                        File.WriteAllLines(FileLocation, (string[])arrcontent.ToArray(typeof(string)));
                    }
                }
            }

            catch (Exception ex)
            {

            }
        }

        public string PublishTexttoQueue(string receivedText)
        {
            string status = string.Empty;
            string[] text = receivedText.Split('^');
            string ip = text[0];
            var client = new HttpClient();
            try
            {
                HttpRequestMessage httpRequestMessageMeterCMD = new HttpRequestMessage();
                httpRequestMessageMeterCMD.Method = HttpMethod.Get;

                //httpRequestMessageMeterCMD.RequestUri = new Uri("https://localhost:7181/api/Publisher?receivedText=" + receivedText);
                //httpRequestMessageMeterCMD.RequestUri = new Uri("http://172.16.15.21:5552/api/Publisher?receivedText=" + receivedText); //ForV6PushListener Queue
                httpRequestMessageMeterCMD.RequestUri = new Uri("http://172.16.15.21:5553/api/Publisher?receivedText=" + receivedText); //For UHES Queue

                var httpResponseMessageMTRCMD = client.Send(httpRequestMessageMeterCMD);

                if (httpResponseMessageMTRCMD.IsSuccessStatusCode)
                {
                    //var response = httpResponseMessageMTRCMD.Content.ReadAsStringAsync().Result;
                    status = "Message Published Successfully to Queue";
                    //support.WriteLog((status));
                    support.WriteLogWithIP(ip, status);
                }
                else
                {
                    status = "Message Not Published";
                }

            }
            catch (Exception ex)
            {
                support.ErrorWriteLog("Exception in PublishTexttoQueue: " + ex.Message);
            }
            return status;
        }

    }
}
