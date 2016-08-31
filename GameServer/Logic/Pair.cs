﻿using LeagueSandbox.GameServer.Logic.Enet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSandbox.GameServer.Logic
{
    public class Pair<T1, T2>
    {
        public T1 Item1 { get; set; }
        public T2 Item2 { get; set; }

        public Pair()
        {

        }

        public Pair(T1 t1, T2 t2)
        {
            Item1 = t1;
            Item2 = t2;
        }
    }

    public struct SClientInfoR
    {
        public int Port { get; set; }
        public ClientInfo Player { get; set; }
    }
}
