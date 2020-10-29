using Exiled.API.Features;
using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;

namespace IRCPlugin
{
    public class Database
    {
        private readonly ircPlugin _pluginInstance;
        public Database(ircPlugin pluginInstance) => this._pluginInstance = pluginInstance;

        public static LiteDatabase LiteDatabase { get; private set; }

        public string DatabaseDirectory =>
            Path.Combine(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    _pluginInstance.Config.DatabaseFolder), _pluginInstance.Config.DatabaseName);

        public string DatabaseFullPath => Path.Combine(DatabaseDirectory, $"{_pluginInstance.Config.DatabaseName}.db");
        public static Dictionary<Exiled.API.Features.Player, Player> PlayerData = new Dictionary<Exiled.API.Features.Player, Player>();

        public void CreateDatabase()
        {
            if (Directory.Exists(DatabaseDirectory)) return;

            try
            {
                Directory.CreateDirectory(DatabaseDirectory);
                Log.Warn("Database not found, Create new DB");
            }
            catch (Exception ex)
            {
                Log.Error($"Cannot create new DB.\n{ex.ToString()}");
            }
        }

        public void OpenDatabase()
        {
            try
            {
                LiteDatabase = new LiteDatabase(DatabaseFullPath);
                LiteDatabase.GetCollection<Player>().EnsureIndex(x => x.Id);
                LiteDatabase.GetCollection<Player>().EnsureIndex(x => x.Name);
                Log.Info("DB Loaded!");
            }
            catch (Exception ex)
            {
                Log.Error($"Failed to open DB!\n{ex.ToString()}");
            }
        }

        public void AddPlayer(Exiled.API.Features.Player player)
        {
            try
            {
                if (LiteDatabase.GetCollection<Player>().Exists(x => x.Id == DatabasePlayer.GetRawUserId(player))) return;

                LiteDatabase.GetCollection<Player>().Insert(new Player()
                {
                    Id = DatabasePlayer.GetRawUserId(player),
                    Name = player.Nickname,
                    TotalGamesPlayed = 0,
                    TotalScpGamesPlayed = 0,
                    TotalKilled = 0,
                    TotalScpKilled = 0,
                    TotalDeath = 0,
                    FirstJoin = DateTime.Now,
                    LastSeen = DateTime.Now,
                    PlayTimeRecords = null,
                    Exp = 0,
                    Level = 1,
                    TotalEscaped = 0,
                    TotalWin = 0,
                    DisplayBadge = false
                });
                Log.Info("Trying to add ID: " + player.UserId.Split('@')[0] + " Discriminator: " + player.UserId.Split('@')[1] + " to Database");
            }
            catch (Exception ex)
            {
                Log.Error($"Cannot add new user to Database: {player.Nickname} ({player.UserId.Split('@')[0]})!\n{ex.ToString()}");
            }
        }
    }
}