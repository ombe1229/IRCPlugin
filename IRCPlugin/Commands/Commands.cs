using Exiled.Events.EventArgs;

namespace IRCPlugin
{
    public class Commands
    {
        public void OnRaCommand(SendingRemoteAdminCommandEventArgs ev)
        {
            switch (ev.Name)
            {
                case "irc_help":
                {
                    ev.IsAllowed = false;
                    ev.ReplyMessage = "IRC플러그인 관리자 명령어 목록: \n";
                    break;
                }
                case "user_stat":
                {
                    
                    break;
                }
            }
        }
    }
}