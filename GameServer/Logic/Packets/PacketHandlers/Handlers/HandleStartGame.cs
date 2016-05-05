﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENet;
using LeagueSandbox.GameServer.Logic.Packets;

namespace LeagueSandbox.GameServer.Core.Logic.PacketHandlers.Packets
{
    class HandleStartGame : IPacketHandler
    {
        public bool HandlePacket(Peer peer, byte[] data, Game game)
        {
            var peerInfo = game.getPeerInfo(peer);

            if (!peerInfo.Disconnected)
            {
                game.IncrementReadyPlayers();
            }

            if (game.getReadyPlayers() == game.getPlayers().Count)
            {
                var start = new StatePacket(PacketCmdS2C.PKT_S2C_StartGame);
                PacketHandlerManager.getInstace().broadcastPacket(start, Channel.CHL_S2C);

                foreach (var player in game.getPlayers())
                {
                    if (player.Item2.getPeer() == peer && !player.Item2.isVersionMatch())
                    {
                        var dm = new DebugMessage("Your client version does not match the server. Check the server log for more information.");
                        PacketHandlerManager.getInstace().sendPacket(peer, dm, Channel.CHL_S2C);
                    }
                }

                game.setStarted(true);
            }

            if (game.isStarted())
            {
                var map = game.getMap();
                if (peerInfo.Disconnected)
                {
                    foreach (var Player in game.getPlayers())
                    {
                        if (Player.Item2.getTeam() == peerInfo.getTeam())
                        {
                            var HeroSpawn2Packet = new HeroSpawn2(Player.Item2.getChampion());
                            PacketHandlerManager.getInstace().sendPacket(peer, HeroSpawn2Packet, Channel.CHL_S2C);
                        }
                    }
                    peerInfo.Disconnected = false;
                    PacketNotifier.notifyDebugMessage("Player " + peerInfo.userId + " reconnected.");

                    // Send the initial game time sync packets, then let the map send another
                    float gameTime = map.getGameTime() / 1000.0f;

                    var timer = new GameTimer(gameTime); // 0xC1
                    PacketHandlerManager.getInstace().sendPacket(peerInfo.getPeer(), timer, Channel.CHL_S2C);

                    var timer2 = new GameTimerUpdate(gameTime); // 0xC2
                    PacketHandlerManager.getInstace().sendPacket(peerInfo.getPeer(), timer2, Channel.CHL_S2C);
                    return true;
                }

                foreach (var p in game.getPlayers())
                {
                    map.AddObject(p.Item2.getChampion());

                    // Send the initial game time sync packets, then let the map send another
                    float gameTime = map.getGameTime() / 1000.0f;

                    var timer = new GameTimer(gameTime); // 0xC1
                    PacketHandlerManager.getInstace().sendPacket(p.Item2.getPeer(), timer, Channel.CHL_S2C);

                    var timer2 = new GameTimerUpdate(gameTime); // 0xC2
                    PacketHandlerManager.getInstace().sendPacket(p.Item2.getPeer(), timer2, Channel.CHL_S2C);
                }
            }

            return true;
        }
    }
}
