using System.Collections.Generic;
using Exiled.Events.EventArgs;

namespace IRCPlugin
{
    public class ConsoleCommands
    {
        private readonly ircPlugin _pluginInstance;
        public ConsoleCommands(ircPlugin pluginInstance) => this._pluginInstance = pluginInstance;

        public void OnConsoleCommand(SendingConsoleCommandEventArgs ev)
        {
            switch (ev.Name)
            {
                case "irc_help":
                {
                    ev.Allow = false;
                    ev.Color = "green";
                    ev.ReturnMessage = "사용 가능한 명령어: \n" + ".irc_help, .irc_stats, .irc_discord, .irc_displaylevel, .irc_scp";
                    break;
                }
                case "irc_stats":
                {
                    ev.Allow = false;
                    ev.Color = "green";
                    string name = ev.Player.GetDatabasePlayer().Name;
                    int exp = ev.Player.GetDatabasePlayer().Exp;
                    int level = ev.Player.GetDatabasePlayer().Level;
                    int totalKilled = ev.Player.GetDatabasePlayer().TotalKilled;
                    int totalScpKilled = ev.Player.GetDatabasePlayer().TotalScpKilled;
                    int totalEscaped = ev.Player.GetDatabasePlayer().TotalEscaped;
                    int totalDeath = ev.Player.GetDatabasePlayer().TotalDeath;
                    int totalGamesPlayed = ev.Player.GetDatabasePlayer().TotalGamesPlayed;
                    int totalScpPlayed = ev.Player.GetDatabasePlayer().TotalScpGamesPlayed;
                    ev.ReturnMessage =
                        $"{name}님의 현재 통계 \n 레벨: {level} | 경험치: {exp} \n 처치한 적: {totalKilled} | 격리한 SCP: {totalScpKilled} | 탈출한 횟수: {totalEscaped} | 죽은 횟수: {totalDeath} \n 총 플레이한 게임: {totalGamesPlayed} | SCP로 플레이한 게임: {totalScpPlayed}";
                    break;
                }
                case "irc_discord":
                {
                    ev.Allow = false;
                    ev.Color = "green";
                    ev.ReturnMessage = "https://discord.gg/sQZgbKx";
                    break;
                }
                case "irc_displaylevel":
                {
                    if (ev.Player.GetDatabasePlayer().DisplayBadge == true)
                    {
                        ev.Player.GetDatabasePlayer().DisplayBadge = false;
                    }
                    else
                    {
                        ev.Player.GetDatabasePlayer().DisplayBadge = true;
                    }
                    break;
                }
                case "irc_scp":
                {
                    if (ev.Player.Team == Team.SCP)
                    {
                        List<Exiled.API.Features.Player> scp_list = new List<Exiled.API.Features.Player>();
                        foreach (var player in Exiled.API.Features.Player.List)
                        {
                            if (player.Team == Team.SCP) scp_list.Add(player);
                        }

                        ev.IsAllowed = false;
                        ev.Color = "green";
                        foreach (var player in scp_list)
                        {
                            ev.ReturnMessage = $"{ev.Player.Role.ToString()} - {ev.Player.DisplayNickname}({ev.Player.Nickname})";
                        }
                    }
                    else
                    {
                        ev.IsAllowed = false;
                        ev.Color = "red";
                        ev.ReturnMessage = "SCP진영만 사용 가능한 명령어입니다!";
                    }
                    break;
                }
            }
        }
    }
}