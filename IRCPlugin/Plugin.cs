using Log = Exiled.API.Features.Log;
using ServerEvents = Exiled.Events.Handlers.Server;
using PlayerEvents = Exiled.Events.Handlers.Player;
using MapEvents = Exiled.Events.Handlers.Map;
using Features = Exiled.API.Features;


namespace IRCPlugin
{
    public class ircPlugin : Features.Plugin<Configs>
    {
        public static bool IsStarted { get; set; }
        public EventHandlers EventHandlers { get; private set; }
        public Database DatabasePlayerData { get; private set; }
        public Player Player { get; private set; }
        public new Commands Commands { get; private set; }
        public ConsoleCommands PlayerConsoleCommands { get; private set; }


        public void LoadEvents()
        {
            PlayerEvents.Joined += EventHandlers.OnJoin;
            PlayerEvents.Left += EventHandlers.OnLeave;
            PlayerEvents.Spawning += EventHandlers.OnSpawned;
            PlayerEvents.Died += EventHandlers.OnPlayerDeath;
            PlayerEvents.EnteringFemurBreaker += EventHandlers.OnFemurBreakerSacrifice;
            PlayerEvents.Escaping += EventHandlers.OnEscaped;
            ServerEvents.RoundStarted += EventHandlers.OnRoundStart;
            ServerEvents.RoundEnded += EventHandlers.OnRoundEnd;
            ServerEvents.RestartingRound += EventHandlers.OnRoundRestart;
            ServerEvents.RespawningTeam += EventHandlers.OnChaosSpawned;
            MapEvents.Decontaminating += EventHandlers.OnDecontaminate;
            MapEvents.AnnouncingNtfEntrance += EventHandlers.OnNTFSpawned;
            MapEvents.AnnouncingScpTermination += EventHandlers.OnScpTerminated;
        }
        
        public void LoadCommands()
        {
            ServerEvents.SendingRemoteAdminCommand += Commands.OnRaCommand;
            ServerEvents.SendingConsoleCommand += PlayerConsoleCommands.OnConsoleCommand;
        }

        
        
        
        public override void OnEnabled()
        {
            if (!Config.IsEnabled) return;
            EventHandlers = new EventHandlers(this);
            Commands = new Commands();
            PlayerConsoleCommands = new ConsoleCommands(this);
            EventHandlers = new EventHandlers(this);
            DatabasePlayerData = new Database(this);
            LoadEvents();
            LoadCommands();
            DatabasePlayerData.CreateDatabase();
            DatabasePlayerData.OpenDatabase();
            Log.Info("IrcPlugin enabled.");
        }

        public override void OnDisabled()
        {
            PlayerEvents.Joined -= EventHandlers.OnJoin;
            PlayerEvents.Left -= EventHandlers.OnLeave;
            ServerEvents.RoundStarted -= EventHandlers.OnRoundStart;
            ServerEvents.RoundEnded -= EventHandlers.OnRoundEnd;
            ServerEvents.RestartingRound -= EventHandlers.OnRoundRestart;
            PlayerEvents.Spawning -= EventHandlers.OnSpawned;
            MapEvents.Decontaminating -= EventHandlers.OnDecontaminate;
            MapEvents.AnnouncingNtfEntrance -= EventHandlers.OnNTFSpawned;
            ServerEvents.RespawningTeam -= EventHandlers.OnChaosSpawned;
            MapEvents.AnnouncingScpTermination -= EventHandlers.OnScpTerminated;
            PlayerEvents.Died -= EventHandlers.OnPlayerDeath;
            PlayerEvents.EnteringFemurBreaker -= EventHandlers.OnFemurBreakerSacrifice;
            PlayerEvents.Escaping -= EventHandlers.OnEscaped;
            EventHandlers = null;
            Commands = null;
            PlayerConsoleCommands = null;
            Database.LiteDatabase.Dispose();
        }

        public override void OnReloaded()
        {
            
        }
    }
}