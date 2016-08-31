using ENet;
using LeagueSandbox.GameServer.Logic.Enet;
using LeagueSandbox.GameServer.Logic.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSandbox.GameServer.Core.Logic.PacketHandlers.Packets
{
    class HandleKeyCheck : IPacketHandler
    {
        public bool HandlePacket(Peer peer, byte[] data, Game game)
        {
            var keyCheck = new KeyCheck(data);
            var userId = game.GetBlowfish().Decrypt(keyCheck.checkId);

            if (userId != keyCheck.userId)
                return false;

            var playerNo = 0;

            for (int i = 0; i < game.GetPlayers().Count; i++)
            {
                var p = game.GetPlayers()[i];
                if (p.Item2.UserId == userId)
                {
                    if (p.Item2.GetPeer() != null)
                    {
                        Logger.LogCoreWarning("Ignoring new player " + userId + ", already connected!");
                        return false;
                    }

                    //TODO: add at least port or smth
                    p.Item1 = peer.Address.port;
                    p.Item2.SetPeer(peer);
                    var response = new KeyCheck(keyCheck.userId, playerNo);
                    bool bRet = game.PacketHandlerManager.sendPacket(peer, response, Channel.CHL_HANDSHAKE);
                    handleGameNumber(p.Item2, peer, game);//Send 0x91 Packet?
                    return true;
                }
                ++playerNo;
            }
            return false;
        }

        bool handleGameNumber(ClientInfo client, Peer peer, Game game)
        {
            var world = new WorldSendGameNumber(1, client.GetName());
            return game.PacketHandlerManager.sendPacket(peer, world, Channel.CHL_S2C);
        }
    }
}
