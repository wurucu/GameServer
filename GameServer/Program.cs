using LeagueSandbox.GameServer.Core.Logic;
using LeagueSandbox.GameServer.Core.Logic.RAF;
using LeagueSandbox.GameServer.Logic;
using LeagueSandbox.GameServer.Logic.GameObjects;
using LeagueSandbox.GameServer.Logic.Items;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ENet;
using WTool;
using LeagueSandbox.GameServer.Core.Logic.PacketHandlers;

namespace LeagueSandbox
{
    class Program
    {
        private static uint SERVER_HOST = Address.IPv4HostAny;
        private static ushort SERVER_PORT = 5119;
        private static string SERVER_KEY = "17BLOhi6KZsTtldTsizvHg==";
        private static string SERVER_VERSION = "0.2.0";
        public static string ExecutingDirectory;

        static void Main(string[] args)
        {
            Console.WriteLine("Yorick " + SERVER_VERSION);
            WriteToLog.ExecutingDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            WriteToLog.LogfileName = "LeagueSandbox.txt";
            WriteToLog.CreateLogFile();
            ExecutingDirectory = WriteToLog.ExecutingDirectory;

            System.AppDomain.CurrentDomain.FirstChanceException += Logger.CurrentDomain_FirstChanceException;
            System.AppDomain.CurrentDomain.UnhandledException += Logger.CurrentDomain_UnhandledException;
            Logger.LogCoreInfo("Loading RAF files in filearchives/.");


            var settings = Settings.Load("Settings/Settings.json");
            if (!RAFManager.getInstance().init(System.IO.Path.Combine(settings.RadsPath, "filearchives")))
            {
                Logger.LogCoreError("Couldn't load RAF files. Make sure you have a 'filearchives' directory in the server's root directory. This directory is to be taken from RADS/projects/lol_game_client/");
                return;
            }

            Logger.LogCoreInfo("Game started");

            var game = new Game();
            var address = new Address(SERVER_HOST, SERVER_PORT);
            
            if (!game.Initialize(address, SERVER_KEY))
            {
                Logger.LogCoreError("Couldn't listen on port " + SERVER_PORT + ", or invalid key");
                return;
            }

            Game.Games.Add(game);
            WServer wtoolServer = new WServer(5999);
            wtoolServer.ReceiveData += WtoolServer_ReceiveData;
            wtoolServer.Start();
             
            game.NetLoop();

            PathNode.DestroyTable(); // Cleanup
        }

        private static void WtoolServer_ReceiveData(WClient Client, byte[] PData)
        {
            if (PData == null || PData.Length == 0 || Game.Games.FirstOrDefault() == null)
                return;
            byte proc = PData[0];
            byte[] Data = null;

            if (PData.Length > 1)
            {
                Data = new byte[PData.Length - 1];
                Array.Copy(PData, 1, Data, 0, PData.Length - 1);
            }

            if (proc == 0)
            { // Get first netid
                var user = Game.Games.FirstOrDefault().GetPlayers().FirstOrDefault();
                var ms = new MemoryStream();
                uint netID = 0;
                if (user != null)
                    netID = user.Item2.GetChampion().getNetId();

                BinaryWriter wrt = new BinaryWriter(ms);
                wrt.Write((byte)0);
                wrt.Write(netID);
                wrt.Close();
                Client.Send(ms.ToArray());
            }

            if (proc == 1) // Send data
            {
                LeagueSandbox.GameServer.Logic.Packets.Packet pckt = new GameServer.Logic.Packets.Packet((PacketCmdS2C)Data[0]);
                var wrt = pckt.getBuffer();
                byte[] tmp = new byte[Data.Length - 1];
                Array.Copy(Data, 1, tmp, 0, tmp.Length);
                wrt.Write(tmp);
                var firstUser = Game.Games.FirstOrDefault().GetPlayers().FirstOrDefault().Item2.GetChampion();
                Game.Games.FirstOrDefault().PacketHandlerManager.broadcastPacketVision(firstUser, pckt, Channel.CHL_S2C);
                Logger.Log("WTOOL-Send Packet > Head : " + ((PacketCmdS2C)Data[0]).ToString() + " - Count : " + Data.Length, "WTOOL-Send Packet > ", ConsoleColor.Green);
            }

        }
    }
}
