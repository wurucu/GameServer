using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSandbox.GameServer.PluginSystem
{
    public enum EPluginType
    {
        Unit,
        Champion,
        Item,
        Buff
    }

    public class PluginBase
    {
        public EPluginType Type { get; set; }
        public string FileName { get; set; }
        public object Content { get; set; }
        public bool Loaded { get; set; }

        public T getContent<T>()
        {
            return (T)Content;
        }
    }

    public class PluginUnit : PluginBase
    {
        public string ModelName { get; set; }
        public PluginUnit()
        {
            this.Type = EPluginType.Unit;
        }
    }

    public class PluginItem : PluginBase
    {
        public string ItemName { get; set; }
        public PluginItem()
        {
            this.Type = EPluginType.Item;
        }
    }

    public class PluginBuff : PluginBase
    {
        public string BuffName { get; set; }
        public PluginBuff()
        {
            this.Type = EPluginType.Buff;
        }
    }
}
