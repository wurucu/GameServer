using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ENet;
using static LeagueSandbox.GameServer.Logic.Chatbox.ChatboxManager;

namespace LeagueSandbox.GameServer.Logic.Chatbox.Commands
{
    class GiveCommand : ChatCommand
    {
        public GiveCommand(string command, string syntax, ChatboxManager owner) : base(command, syntax, owner){}

        public override void Execute(Peer peer, bool hasReceivedArguments, string arguments = "")
        {
            var split = arguments.ToLower().Split(' ');
            int idItem;
            if (split.Length < 2)
            {
                _owner.SendDebugMsgFormatted(DebugMsgType.SYNTAXERROR);
                ShowSyntax();
            }
            else if (int.TryParse(split[1], out idItem))
            {
                var itemTemplate = _owner.GetGame().ItemManager.SafeGetItemType(idItem);
                var i = _owner.GetGame().GetPeerInfo(peer).GetChampion().Inventory.AddItem(itemTemplate);
                _owner.GetGame().PacketNotifier.notifyItemBought(_owner.GetGame().GetPeerInfo(peer).GetChampion(), i);
            }
        }
    }
}
