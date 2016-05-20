using LeagueSandbox.GameServer.Logic.Content;
using Newtonsoft.Json.Linq;
using NLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueSandbox.GameServer.Logic
{
    public class Config
    {
        public Dictionary<string, PlayerConfig> Players { get; private set; }
        public GameConfig GameConfig { get; private set; }
        public MapSpawns MapSpawns { get; private set; }
        public MapConfig MapConfig { get; private set; }
        public ContentManager ContentManager { get; private set; }
        public const string VERSION = "Version 4.20.0.315 [PUBLIC]";

        public Config(string path)
        {
            LoadConfig(path);
        }

        private void LoadConfig(string path)
        {
            Players = new Dictionary<string, PlayerConfig>();

            var data = JObject.Parse(File.ReadAllText(path));

            // Read the player configuration
            var playerConfigurations = data.SelectToken("players");
            foreach (var player in playerConfigurations)
            {
                var playerConfig = new PlayerConfig(player);
                Players.Add(string.Format("player{0}", Players.Count + 1), playerConfig);
            }

            // Read the game configuration
            var game = data.SelectToken("game");
            GameConfig = new GameConfig(game);

            // Read spawns info
            ContentManager = ContentManager.LoadGameMode(GameConfig.GameMode);
            var mapPath = ContentManager.GetMapDataPath(GameConfig.Map);
            MapConfig = new MapConfig();
            var mapData = JObject.Parse(File.ReadAllText(mapPath));

            MapConfig.MapID = (int) mapData.Property("map").Value;
            MapConfig.GoldPerSecond = (float) mapData.Property("goldPerSecond").Value;
            MapConfig.ExperiencePerSecond = (float) mapData.Property("experiencePerSecond").Value;
            MapConfig.SpawnInterval = (float) mapData.Property("spawnInterval").Value;
            MapConfig.FirstSpawnTime = (float) mapData.Property("firstSpawnTime").Value;
            MapConfig.GoldTimer = (float) mapData.Property("goldTimer").Value;

            var experience = (JArray)mapData.Property("experience").Value;

            MapConfig.Experience = new List<int>();
            for (var i = 0; i < experience.Count; i++)
            {
                MapConfig.Experience.Add((int) experience[i]);
            }

            var spawns = mapData.SelectToken("spawns");

            MapSpawns = new MapSpawns();
            foreach (JProperty teamSpawn in spawns)
            {
                var team = teamSpawn.Name;
                var spawnsByPlayerCount = (JArray)teamSpawn.Value;
                for (var i = 0; i < spawnsByPlayerCount.Count; i++)
                {
                    var playerSpawns = new PlayerSpawns((JArray)spawnsByPlayerCount[i]);
                    MapSpawns.SetSpawns(team, playerSpawns, i);
                }
            }


        }
    }

    public class MapSpawns
    {
        public Dictionary<int, PlayerSpawns> Blue = new Dictionary<int, PlayerSpawns>();
        public Dictionary<int, PlayerSpawns> Purple = new Dictionary<int, PlayerSpawns>();

        public void SetSpawns(string team, PlayerSpawns spawns, int playerCount)
        {
            if (team.ToLower() == "blue")
            {
                Blue[playerCount] = spawns;
            }
            else if (team.ToLower() == "purple")
            {
                Purple[playerCount] = spawns;
            }
            else
            {
                throw new Exception("Invalid team");
            }
        }
    }

    public class MapConfig
    {
        public float GoldPerSecond { get; set; }
        public float ExperiencePerSecond { get; set; }
        public int MapID { get; set; }
        public float StartingGold { get; set; }
        public float SpawnInterval { get; set; }
        public float FirstSpawnTime { get; set; }
        public float GoldTimer { get; set; }
        public bool HasFountainHeal { get; set; }
        public List<int> Experience { get; set; } 
    }

    public class PlayerSpawns
    {
        private JArray _spawns;

        public PlayerSpawns(JArray spawns)
        {
            _spawns = spawns;
        }

        internal int GetXForPlayer(int playerId)
        {
            return (int)((JArray)_spawns[playerId])[0];
        }

        internal int GetYForPlayer(int playerId)
        {
            return (int)((JArray)_spawns[playerId])[0];
        }
    }

    public class GameConfig
    {
        public int Map { get { return (int)_gameData.SelectToken("map"); } }
        public string GameMode { get { return (string)_gameData.SelectToken("gameMode"); } }

        private JToken _gameData;

        public GameConfig(JToken gameData)
        {
            _gameData = gameData;
        }
    }


    public class PlayerConfig
    {
        public string Rank { get { return (string)_playerData.SelectToken("rank"); } }
        public string Name { get { return (string)_playerData.SelectToken("name"); } }
        public string Champion { get { return (string)_playerData.SelectToken("champion"); } }
        public string Team { get { return (string)_playerData.SelectToken("team"); } }
        public short Skin { get { return (short)_playerData.SelectToken("skin"); } }
        public string Summoner1 { get { return (string)_playerData.SelectToken("summoner1"); } }
        public string Summoner2 { get { return (string)_playerData.SelectToken("summoner2"); } }
        public short Ribbon { get { return (short)_playerData.SelectToken("ribbon"); } }
        public int Icon { get { return (int)_playerData.SelectToken("icon"); } }

        private JToken _playerData;

        public PlayerConfig(JToken playerData)
        {
            _playerData = playerData;
        }
    }
}
