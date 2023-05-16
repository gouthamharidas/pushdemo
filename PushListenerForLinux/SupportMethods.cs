using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushListenerForLinux
{
    public class SupportMethods
    {
        public void WriteLog1(string MessagetobeLogged)
        {
            try
            {
                DateTime now = DateTime.Now;
                //using (StreamWriter writer = new StreamWriter(Application.StartupPath + @"\" + now.ToString("ddMMMyy") + ".txt", true))
                using (StreamWriter writer = new StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + "\\" + now.ToString("ddMMMyy") + ".txt", true))
                {
                    writer.WriteLine(DateTime.Now.ToString("dd-MMM-yy HH:mm:ss.fff") + " --> " + MessagetobeLogged);
                    writer.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        public void WriteConnectionLog(string MessagetobeLogged)
        {
            try
            {
                DateTime now = DateTime.Now;
                //using (StreamWriter writer = new StreamWriter(Application.StartupPath + @"\" + now.ToString("ddMMMyy") + "_Connection.txt", true))
                using (StreamWriter writer = new StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + "\\" + now.ToString("ddMMMyy") + ".txt", true))
                {
                    writer.WriteLine(DateTime.Now.ToString("dd-MMM-yy HH:mm:ss.fff") + " --> " + MessagetobeLogged);
                    writer.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        public void WriteExceptionLog(string ExceptionMessagetobeLogged)
        {
            try
            {
                DateTime now = DateTime.Now;
                //using (StreamWriter writer = new StreamWriter(Application.StartupPath + @"\" + now.ToString("ddMMMyy") + "_Exceptions.txt", true))
                using (StreamWriter writer = new StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase).Substring(6) + "\\" + now.ToString("ddMMMyy") + ".txt", true))
                {
                    writer.WriteLine(DateTime.Now.ToString("dd-MMM-yy HH:mm:ss.fff") + " --> " + ExceptionMessagetobeLogged);
                    writer.Close();
                }
            }
            catch (Exception)
            {
            }
        }

        public void WriteLog(string Message)
        {
            try
            {
                string m_baseDir = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.RelativeSearchPath;
                string FileLocation = m_baseDir + DateTime.Now.ToString("yyyy-MM-dd") + " TCPIPV6Log.txt";
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
            catch (Exception)
            {

            }
            finally
            {
                //tmrPing.Enabled = true;
            }
        }

        public void WriteLogWithIP(string ip, string message)
        {
            // 2401:4900:4022:9d4d::2
            string iP = ip.Replace(":", ".");
            string m_baseDir = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.RelativeSearchPath;
            var blockLoadLog = Directory.CreateDirectory(Path.Combine(m_baseDir, "TCPIPV6Log/"));
            var innerFolder = DateTime.Today.ToString("yyyy-MM-dd");
            var subFolder = blockLoadLog.CreateSubdirectory(innerFolder);
            // var json = JsonSerializer.Serialize();
            string filename = iP + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
            StreamWriter sw = new StreamWriter(Path.Combine(subFolder.FullName, filename), true);
            sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + message);
            sw.Close();
        }

        public void ErrorWriteLog(string Message)
        {
            try
            {
                string m_baseDir = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.RelativeSearchPath;
                string FileLocation = m_baseDir + DateTime.Now.ToString("yyyy-MM-dd") + " TCPIPV6ErrorLog.txt";
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
            catch (Exception)
            {

            }
            finally
            {
                //tmrPing.Enabled = true;
            }
        }
    }
}
