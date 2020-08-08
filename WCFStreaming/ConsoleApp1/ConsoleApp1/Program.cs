using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp1.Datatest;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var con = new wcfConnector("http://localhost:8733/Design_Time_Addresses/WcfServiceLibrary1/Service1");
            var proxy = con.connect();
            var formatter = new BinaryFormatter();
            Stream a = proxy.GetData();
            var res = new List<string>();
            try
            {
                while (a.CanRead)
                {
                    //string버전(BigDataStream)
                    //res.Add(formatter.Deserialize(a).ToString());

                    //List<string>버전(BigDatStream2)
                    res.AddRange(formatter.Deserialize(a) as List<string>);
                }
            }
            catch(Exception e) { }
        }
    }
    class wcfConnector
    {
        string uri;
        BasicHttpBinding binding;
        EndpointAddress address;
        public wcfConnector(string uri)
        {
            this.uri = uri;
            binding = new BasicHttpBinding();
            address = new EndpointAddress(uri);
            binding.TransferMode = TransferMode.Streamed;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.CloseTimeout = new TimeSpan(1, 10, 0);

        }
        public Service1Client connect()
        {
            Service1Client proxy = new Service1Client(binding, address);
            return proxy;
        }
    }
}