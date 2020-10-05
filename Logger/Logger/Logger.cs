using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Logger
{
    public enum LEVEL
    {
        ALL, DEBUG, INFO, WARN, ERROR, FATAL, OFF, TRACE
    }
    public class Logger
    {
        public string name;
        public string type;
        public string path;
        public string fileName;
        public int maxRollBackup;
        public int maxSize;
        public string pattern;
        public Logger(string name, string type, string path, string fileName, int maxRollBackup, int maxSize, string pattern)
        {
            this.name = name;
            this.type = type;
            this.path = path;
            this.fileName = fileName;
            this.maxRollBackup = maxRollBackup;
            this.maxSize = maxSize;
            this.pattern = pattern;
        }
        public void rollFiles()
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            string fullPath = Path.Combine(path, fileName);
            if (File.Exists(fullPath) && new FileInfo(fullPath).Length>=maxSize)
            {
                var array = Directory.GetFiles(path, fileName + "*");
                Array.Sort(array);
                for(int i = array.Length; i >= 0; i--)
                {
                    if (i + 1 == maxSize) new FileInfo(array[i]).Delete();
                    else
                    {
                        if (i == 0) File.Move(array[i], array[i] + ".1");
                        else File.Move(array[i], Path.ChangeExtension(array[i], (i + 1).ToString()));
                    }
                }
            }
        }
        public string convertPatternToString()
        {
            var tmp = pattern;
            tmp = tmp.Replace("%date", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            tmp = tmp.Replace("%name", name);
            return tmp;
        }
        public void info(string message)
        {
            string tmp = convertPatternToString();
            tmp = tmp.Replace("%message", message);
            tmp = tmp.Replace("%level", "INFO");
            rollFiles();
            string fullPath = Path.Combine(path, fileName);
            using (StreamWriter writer = File.AppendText(fullPath))
            {
                writer.WriteLine(tmp);
            }
        }
        public void debug(string message)
        {
            string tmp = convertPatternToString();
            tmp = tmp.Replace("%message", message);
            tmp = tmp.Replace("%level", "DEBUG");
            rollFiles();
            string fullPath = Path.Combine(path, fileName);
            using (StreamWriter writer = File.AppendText(fullPath))
            {
                writer.WriteLine(tmp);
            }
        }
        public void error(string message)
        {
            string tmp = convertPatternToString();
            tmp = tmp.Replace("%message", message);
            tmp = tmp.Replace("%level", "ERROR");
            rollFiles();
            string fullPath = Path.Combine(path, fileName);
            using (StreamWriter writer = File.AppendText(fullPath))
            {
                writer.WriteLine(tmp);
            }
        }
    }
}
