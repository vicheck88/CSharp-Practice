using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Microsoft.SqlServer.Server;
using Npgsql;

namespace WcfServiceLibrary1
{
    // 참고: "리팩터링" 메뉴에서 "이름 바꾸기" 명령을 사용하여 코드 및 config 파일에서 클래스 이름 "Service1"을 변경할 수 있습니다.
    public class Service1 : IService1
    {
        public Stream GetData()
        {
            //return new BigDataStream();
            return new BigDataStream3();
        }
    }
#if true
    public class BigDataStream3 : Stream
    {
        NpgsqlConnection conn;
        NpgsqlDataReader _reader;
        IFormatter formatter;
        ConcurrentBag<MemoryStream> streamBag;
        bool readFinished;
        List<string> queryResult;
        List<byte> _encodeBuffer = new List<byte>();
        public BigDataStream3()
        {
            // open DB connection
            // run your query
            // get a DataReader
            string connectionString = "Host=;Port=5432;Database=;Username=;Password=";
            conn = new NpgsqlConnection(connectionString);
            conn.Open();
            string SQL = "";
            NpgsqlCommand comm = new NpgsqlCommand(SQL, conn);
            _reader = comm.ExecuteReader();

            queryResult = new List<string>();
            formatter = new BinaryFormatter();
            streamBag = new ConcurrentBag<MemoryStream>();
            Thread producer = new Thread(produceStream);
            producer.Start();
        }

        private void produceStream()
        {
            readFinished = false;
            while (_reader.Read())
            {
                queryResult.Add(_reader[2].ToString());
                if(queryResult.Count > 3)
                {
                    var stream = new MemoryStream();
                    formatter.Serialize(stream, queryResult);
                    streamBag.Add(stream);
                    queryResult.Clear();
                }
            }
            if (queryResult.Count > 0)
            {
                var stream = new MemoryStream();
                formatter.Serialize(stream, queryResult);
                streamBag.Add(stream);
                queryResult.Clear();
            }
            readFinished = true;
        }
        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_encodeBuffer.Count == 0)
            {
                MemoryStream stream = null;
                while(!readFinished || streamBag.Count > 0)
                {
                    if (!streamBag.TryTake(out stream)) Thread.Sleep(100);
                    else break;
                }
                if(stream != null) _encodeBuffer.AddRange(stream.ToArray());                
            }
            int cnt = _encodeBuffer.Count > count ? count : _encodeBuffer.Count;
            _encodeBuffer.CopyTo(0, buffer, 0, cnt);
            _encodeBuffer.RemoveRange(0, cnt);
            return cnt;
        }

        public override void Close()
        {
            _reader.Close();
            conn.Close();
            // close the reader + db connection
            base.Close();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
#endif
#if false
    public class BigDataStream2 : Stream
    {
        NpgsqlConnection conn;
        NpgsqlDataReader _reader;
        IFormatter formatter;
        public BigDataStream2()
        {
            // open DB connection
            // run your query
            // get a DataReader
            string connectionString = "Host=;Port=5432;Database=;Username=;Password=";
            conn = new NpgsqlConnection(connectionString);
            conn.Open();
            string SQL = "";
            NpgsqlCommand comm = new NpgsqlCommand(SQL, conn);
            _reader = comm.ExecuteReader();

            memstream = new MemoryStream();
            queryResult = new List<string>();
            formatter = new BinaryFormatter();
        }

        MemoryStream memstream;
        List<string> queryResult;
        List<byte> _encodeBuffer = new List<byte>();

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (_encodeBuffer.Count == 0)
            {
                while (queryResult.Count < 3 && _reader.Read())
                {
                    queryResult.Add(_reader[2].ToString());
                }
                if (queryResult.Count > 0)
                {
                    memstream = new MemoryStream();
                    formatter.Serialize(memstream, queryResult);
                    _encodeBuffer.AddRange(memstream.ToArray());
                    queryResult.Clear();
                }
            }
            int cnt = _encodeBuffer.Count > count ? count : _encodeBuffer.Count;
            _encodeBuffer.CopyTo(0, buffer, 0, cnt);
            _encodeBuffer.RemoveRange(0, cnt);
            return cnt;
        }

        public override void Close()
        {
            _reader.Close();
            conn.Close();
            // close the reader + db connection
            base.Close();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    } 
#endif
#if false
    public class BigDataStream : Stream
    {
        NpgsqlConnection conn;
        NpgsqlDataReader _reader;
        IFormatter formatter;
        MemoryStream str;
        public BigDataStream()
        {
            // open DB connection
            // run your query
            // get a DataReader
            string connectionString = "Host=;Port=5432;Database=;Username=;Password=";
            conn = new NpgsqlConnection(connectionString);
            conn.Open();
            string SQL = "";
            NpgsqlCommand comm = new NpgsqlCommand(SQL, conn);
            _reader = comm.ExecuteReader();

            formatter = new BinaryFormatter();
            str = new MemoryStream();
        }

        // you need a buffer to encode your data between calls to Read
        List<byte> _encodeBuffer = new List<byte>();

        public override bool CanRead => throw new NotImplementedException();

        public override bool CanSeek => throw new NotImplementedException();

        public override bool CanWrite => throw new NotImplementedException();

        public override long Length => throw new NotImplementedException();

        public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override int Read(byte[] buffer, int offset, int count)
        {
            // read from the DataReader and populate the _encodeBuffer
            // until the _encodeBuffer contains at least count bytes
            // (or until there are no more records)
            // for example:

            while (_encodeBuffer.Count < count && _reader.Read())
            {
                // (1)
                // encode the record into a byte array. How to do this?
                // you can read into a class and then use the data 
                // contract serialization for example. If you do this, you
                // will probably find it easier to prepend an integer which
                // specifies the length of the following encoded message. 
                // This will make it easier for the client to deserialize it.

                // (2)
                // append the encoded record bytes (plus any length prefix 
                // etc) to _encodeBuffer
                str = new MemoryStream();
                formatter.Serialize(str, _reader[2].ToString());
                _encodeBuffer.AddRange(str.ToArray());
            }
            int cnt = _encodeBuffer.Count > count ? count : _encodeBuffer.Count;
            if (_encodeBuffer.Count > 0)
            {
                _encodeBuffer.CopyTo(0, buffer, 0, cnt);
                _encodeBuffer.RemoveRange(0, cnt);

            }
            // remove up to the first count bytes from _encodeBuffer
            // and copy them into buffer at the offset requested

            // return the number of bytes added
            return cnt;
        }

        public override void Close()
        {
            _reader.Close();
            conn.Close();
            // close the reader + db connection
            base.Close();
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    } 
#endif
}
