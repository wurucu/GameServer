using LeagueSandbox.GameServer.Core.Logic;
using LeagueSandbox.GameServer.Logic.GameObjects;
using LeagueSandbox.GameServer.PluginSystem.Faces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSandbox.GameServer.PluginSystem
{
    public class PluginManager
    {
        public static string PluginDLL = AppDomain.CurrentDomain.BaseDirectory + @"\Plugin\GameServerPlugin.dll";
        public static List<Type> PluginTypes;
        public static bool AutoReloadPlugin = true; // Watch is file changed to reload

        public delegate void DReloadPlugin();
        public static event DReloadPlugin ReloadPlugin;
        public static FileSystemWatcher PluginFileWatch;
        public static void LoadPlugin()
        {
            if (PluginTypes == null)
            { //First load
                if (!File.Exists(PluginDLL))
                {
                    Logger.LogFullColor("Plugin file not found ! (" + PluginDLL + ")", "EXCEPTION", ConsoleColor.Red);
                    return;
                } 
                Assembly asm = Assembly.Load(File.ReadAllBytes(PluginDLL));
                PluginTypes = asm.GetTypes().Where(x =>
                            x.GetInterfaces().Contains(typeof(IUnit)) ||
                            x.GetInterfaces().Contains(typeof(IBuff))).ToList(); 

                Logger.LogFullColor($"PLUGIN : {PluginTypes.Count} class loaded.", "INFO", ConsoleColor.Cyan);

                if (AutoReloadPlugin && PluginFileWatch == null)
                {
                    FileInfo fi = new FileInfo(PluginDLL);
                    PluginFileWatch = new FileSystemWatcher();
                    PluginFileWatch.Path = fi.Directory.FullName;
                    PluginFileWatch.Filter = fi.Name;
                    PluginFileWatch.NotifyFilter = NotifyFilters.LastWrite;
                    PluginFileWatch.Changed += Watch_Changed; 
                    PluginFileWatch.EnableRaisingEvents = true;
                    Logger.LogFullColor($"PLUGIN : Watch Mode ON !", "INFO", ConsoleColor.Cyan);
                }
            }
        }

        private static void Watch_Changed(object sender, FileSystemEventArgs e)
        {
            if (ReloadPlugin != null)
            {
                PluginTypes = null;
                ReloadPlugin();
            } 
        }

        public static PluginUnit LoadChampion(Game game, Champion champion)
        {
            PluginUnit plugin = new PluginUnit();
            LoadPlugin();
            
            if (PluginTypes == null || PluginTypes.Count == 0) 
                return plugin; //Plugin empty
             
            Type pluginType = PluginTypes.Where(x => x.Name == champion.getType() && x.GetInterfaces().Contains(typeof(IUnit))).FirstOrDefault();
            if (pluginType == null)
            {
                Logger.LogFullColor($"PluginDLL in champion class ({champion.getType()}) not found.", "EXCEPTION", ConsoleColor.Red);
                return plugin;
            }
            plugin.Content = (IUnit)Activator.CreateInstance(pluginType); // new ?Chmapion
            plugin.getContent<IUnit>().Initialize(game, champion);
            plugin.Loaded = true;
            Logger.LogFullColor(champion.getType() + " plugin is loaded !", "LOAD", ConsoleColor.Green);

            return plugin;            
        }

        public static void Exception(Exception exp, PluginBase plugin)
        {
            
        }
    }
}
