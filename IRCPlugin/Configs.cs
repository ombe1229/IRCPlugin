using Exiled.API.Interfaces;
using System.ComponentModel;
using Log = Exiled.API.Features.Log;

namespace IRCPlugin
{
    public class Configs : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        

        [Description("Use mini939 spawn?")]
        public bool Mini939Enable { get; private set; } = false;

        [Description("Should SCP Leave message be shown?")]
        public bool ScpLeaveMessageEnable { get; private set; } = true;

        [Description("Should Round start message be shown?")]
        public bool RoundStartMessageEnable { get; private set; } = true;

        [Description("Should Round end message be shown?")]
        public bool RoundEndMessageEnable { get; private set; } = true;

        [Description("Should decontamination message be shown?")]
        public bool DecontaminationMessageEnable { get; private set; } = true;

        [Description("Sould NTF message be shown?")]
        public bool NtfMassageEnable { get; private set; } = true;

        [Description("Should Chaos message be shown?")]
        public bool ChaosMessageEnable { get; private set; } = true;

        [Description("Should SCP terminated message be shown?")]
        public bool ScpTerminatedMessageEnable { get; private set; } = true;

        [Description("Should nuke message be shown?")]
        public bool NukeMessageEnable { get; private set; } = true;

        [Description("Should death message be shown?")]
        public bool DeathMessageEnable { get; private set; } = true;

        [Description("Should 106 Femur breaker sacrifice message be shown?")]
        public bool FemurBreakerMessageEnable { get; private set; } = true;

        [Description("Should filtering the nickname?")]
        public bool NickNameFilteringEnable { get; private set; } = true;

        [Description("Use SCP healing?")]
        public bool ScpHealingEnable { get; private set; } = true;

        [Description("173 healing ammount")]
        public int Healing173 { get; private set; } = 150;

        [Description("096 healing ammount")]
        public int Healing096 { get; private set; } = 0;
        
        [Description("049 healing ammount")]
        public int Healing049 { get; private set; } = 25;
        
        [Description("049-2 healing ammount")]
        public int Healing0492 { get; private set; } = 25;
        
        [Description("106 healing ammount")]
        public int Healing106 { get; private set; } = 75;
        
        [Description("939 healing ammount")]
        public int Healing939 { get; private set; } = 125;

        [Description("Database name, change it only if you are running multiple servers")]
        public string DatabaseName { get; private set; } = "IrcPlugin";

        [Description("In which folder database should be stored?")]
        public string DatabaseFolder { get; private set; } = "EXILED";




        

        public void ConfigValidator()
        {
            if (!IsEnabled)
            {
                Log.Warn("You disabled the IRCPlugin in server configs!");
            }
        }
    }
}