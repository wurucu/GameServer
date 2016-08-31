using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace WTool
{
    public class WClient : IWClient
    {
        public Socket Soket;

        private TcpClient tcpclnt;

        public WClient(string ServerIP, int ServerPort)
        {
            this.tcpclnt = new TcpClient();
            this.Soket = tcpclnt.Client;
            tcpclnt.Connect(ServerIP, ServerPort);
        }

        public WClient(Socket socket)
        { 
            this.Soket = socket;
        }

        public void Send(byte[] Data)
        {
            if (Soket.Connected)
                Soket.Send(Data);
        }
         

        public void Close()
        {
            if (Soket.Connected)
                Soket.Close();
        }
    }
}
