using System;
using Exiled.API.Features;
using Exiled.Events.EventArgs;
using Respawning;


namespace IRCPlugin
{
    public class EventHandlers
    {
        private readonly ircPlugin _pluginInstance;
        public EventHandlers(ircPlugin pluginInstance) => this._pluginInstance = pluginInstance;
        
        
        internal void OnJoin(JoinedEventArgs ev)
        {
            if (!Database.LiteDatabase.GetCollection<Player>().Exists(player => player.Id == ev.Player.GetRawUserId()))
            {
                Log.Info(ev.Player.Nickname + " is not present on DB!");
                _pluginInstance.DatabasePlayerData.AddPlayer(ev.Player);
            }

            var databasePlayer = ev.Player.GetDatabasePlayer();
            if (Database.PlayerData.ContainsKey(ev.Player))
            {
                Database.PlayerData.Add(ev.Player, databasePlayer);
                databasePlayer.LastSeen = DateTime.Now;
                databasePlayer.Name = ev.Player.Nickname;
                if (databasePlayer.FirstJoin == DateTime.MinValue) databasePlayer.FirstJoin = DateTime.Now;
            }

            if (!_pluginInstance.Config.NickNameFilteringEnable)
            {
                var nickname = FilteringNickname(ev.Player);
                if (ev.Player.Nickname != nickname) ev.Player.DisplayNickname = nickname;
                Log.Info($"{ev.Player} is joined.");
            }
        }

        internal void OnLeave(LeftEventArgs ev)
        {
            if (ev.Player.Nickname != "Dedicated Server" && ev.Player != null && Database.PlayerData.ContainsKey(ev.Player))
            {
                ev.Player.GetDatabasePlayer().SetCurrentDayPlayTime();
                Database.LiteDatabase.GetCollection<Player>().Update(Database.PlayerData[ev.Player]);
                Database.PlayerData.Remove(ev.Player);
            }

            if (_pluginInstance.Config.ScpLeaveMessageEnable && ev.Player.Team == Team.SCP)
            {
                AddExp(ev.Player,-10);
                Map.Broadcast(10,$"<color=red>{ev.Player.Nickname}</color>(이)가 <color=red>SCP</color>진영에서 게임을 중도 퇴장하였습니다.");
            }
        }
        
        internal void OnRoundStart()
        {
            ircPlugin.IsStarted = true;
            foreach (var player in Exiled.API.Features.Player.List)
            {
                player.GetDatabasePlayer().TotalGamesPlayed++;
                AddExp(player,3);
            }
            
            if (!_pluginInstance.Config.RoundStartMessageEnable)
            {
                Map.Broadcast(10,"<color=green>라운드가 시작되었습니다.</color>");
            }
            Log.Info("New round has been started.");
        }

        internal void OnRoundEnd(RoundEndedEventArgs ev)
        {   
            ircPlugin.IsStarted = false;
            if (!_pluginInstance.Config.RoundEndMessageEnable) return;
            Map.Broadcast(10,"<color=green>라운드가 종료되었습니다.</color>");
            Log.Info("Round has been ended.");
        }
        
        internal void OnRoundRestart()
        {
            ircPlugin.IsStarted = false;
        }

        internal void OnSpawned(SpawningEventArgs ev)
        {
            if (ev.Player.Team == Team.SCP) ev.Player.GetDatabasePlayer().TotalScpGamesPlayed++;
            AddExp(ev.Player,3);
        }

        internal void OnDecontaminate(DecontaminatingEventArgs ev)
        {
            if (!_pluginInstance.Config.DecontaminationMessageEnable) return;
            Map.Broadcast(10,"<color=red>저위험군</color>이 폐쇠되었고 오염 제거 준비가 완료되었습니다.\n<color=red>유기체</color>의 제거가 시작되었습니다.");
            Log.Info("Decontaminate has been started.");
        }

        internal void OnNTFSpawned(AnnouncingNtfEntranceEventArgs ev)
        {
            if (!_pluginInstance.Config.NtfMassageEnable)
            {
                Map.Broadcast(10,$"<color=blue>기동특무부대 엡실론-11, {ev.UnitName}</color>분대가 시설 내에 진입했습니다.\n<color=red>{ev.ScpsLeft}</color>개체의 <color=red>SCP</color>가 재격리 대기 중입니다.");
                Log.Info("NTF has been spawned.");
            }
        }

        internal void OnEscaped(EscapingEventArgs ev)
        {
            if (ev.Player.Team == Team.CDP)
            {
                switch (ev.Player.IsCuffed)
                {
                    case true:
                        AddExp(ev.Player,10);
                        ev.Player.Broadcast(10,"당신은 <color=orange>D계급 인원</color>으로 탈출하여 <color=green>혼돈의 반란</color>에 합류했습니다.");
                        break;
                    case false:
                        AddExp(ev.Player, 8);
                        ev.Player.Broadcast(10,"당신은 <color=orange>D계급 인원</color>으로 탈출하여 <color=blue>기동특무부대</color>에 합류했습니다.");
                        break;
                }
            } else if (ev.Player.Team == Team.RSC)
            {
                switch (ev.Player.IsCuffed)
                {
                    case true:
                        AddExp(ev.Player,10);
                        ev.Player.Broadcast(10,"당신은 <color=yellow>과학자</color>로 탈출하여 <color=blue>기동특무부대</color>에 합류했습니다.");
                        break;
                    case false:
                        AddExp(ev.Player, 8);
                        ev.Player.Broadcast(10,"당신은 <color=yellow>과학자</color>로 탈출하여 <color=green>혼돈의 반란</color>에 합류했습니다.");
                        break;
                }
            }
        }

        internal void OnChaosSpawned(RespawningTeamEventArgs ev)
        {
            if (ev.NextKnownTeam != SpawnableTeamType.ChaosInsurgency ||
                !_pluginInstance.Config.ChaosMessageEnable) return;
            foreach (var p in Exiled.API.Features.Player.List)
            {
                if (p.Team == Team.CDP || p.Team == Team.CHI) p.Broadcast(10,"<color=green>혼돈의 반란</color>이 시설 내에 진입했습니다.");
            }
        }

        internal void OnScpTerminated(AnnouncingScpTerminationEventArgs ev)
        {
            ev.Killer.GetDatabasePlayer().TotalScpKilled++;
            AddExp(ev.Killer,20);
            
            if (!_pluginInstance.Config.ScpTerminatedMessageEnable) return;
            Map.Broadcast(10,$"<color=red>{ev.Role.fullName}</color>가 <color=red>{ev.Killer.Nickname}</color>에 의해 격리되었습니다.");
        }

        internal void OnWarheadEvents(StartingEventArgs ev)
        {
            if (!_pluginInstance.Config.NukeMessageEnable) return;
            Map.Broadcast(10,
                ev.IsAllowed ? "<color=red>알파 탄두</color> 폭파 절차가 발동되었습니다.\n남아있는 인원은 신속히 대피하시길 바랍니다." : "<color=red>폭파 절차</color>가 취소되었습니다. 시스템을 재가동합니다.");
        }

        internal void OnPlayerDeath(DiedEventArgs ev)
        {
            AddExp(ev.Target, 5);
            ev.Target.GetDatabasePlayer().TotalDeath++;
            ev.Killer.GetDatabasePlayer().TotalKilled++;
            
            
            if (_pluginInstance.Config.DeathMessageEnable)
            {
                if (ev.Target.Team != Team.SCP)
                {
                    AddExp(ev.Killer,5);
                    
                    ev.Target.Broadcast(10,$"당신은 <color=red>{ev.Killer.Nickname}</color> ({ev.Killer.Role.ToString()})에 의해 사망했습니다.");
                }
            }

            if (_pluginInstance.Config.ScpHealingEnable && ev.Killer.Team == Team.SCP)
            {
                if (ev.Killer == ev.Target) return;
                
                AddExp(ev.Killer,10);
                
                int heal = 0;
                switch (ev.Killer.Role)
                {
                    case RoleType.Scp173 :
                        heal = _pluginInstance.Config.Healing173;
                        break;
                    case RoleType.Scp096 :
                        heal = _pluginInstance.Config.Healing096;
                        break;
                    case RoleType.Scp049 :
                        heal = _pluginInstance.Config.Healing049;
                        break;
                    case RoleType.Scp0492 :
                        heal = _pluginInstance.Config.Healing0492;
                        break;
                    case RoleType.Scp106 :
                        heal = _pluginInstance.Config.Healing106;
                        break;
                    case RoleType.Scp93953 :
                        heal = _pluginInstance.Config.Healing939;
                        break;
                    case RoleType.Scp93989 :
                        heal = _pluginInstance.Config.Healing939;
                        break;
                }

                if (ev.Killer.MaxHealth <= ev.Killer.Health + heal) ev.Killer.Health = ev.Killer.MaxHealth;
                else ev.Killer.Health += heal;
                ev.Killer.Broadcast(10,$"당신은 인간을 처치하여 체력을 회복했습니다.\n현재 HP:<color=red>{ev.Killer.Health}</color>");
            }
        }
        
        internal void OnFemurBreakerSacrifice(EnteringFemurBreakerEventArgs ev)
        {
            AddExp(ev.Player,10);
            if (!_pluginInstance.Config.FemurBreakerMessageEnable) return;
            Map.Broadcast(10,$"<color=red>{ev.Player.Nickname}</color>(이)가 106희생대에 진입했습니다.");
        }





        private string FilteringNickname(Exiled.API.Features.Player player)
        {
            string value =  player.Nickname.Replace("Twitch", "").Replace("트위치", "").Replace("Youtube", "").Replace("유튜브", "");
            return value;
        }

        private static void AddExp(Exiled.API.Features.Player player, int exp)
        {
            int nowExp = player.GetDatabasePlayer().Exp;
            int nowLevel = player.GetDatabasePlayer().Level;
            if (nowExp + exp >= (nowLevel + 2) * 2)
            {
                player.GetDatabasePlayer().Level++;
                player.GetDatabasePlayer().Exp = 0;
                player.Broadcast(5,$"레벨업!\n당신의 레벨이 <color=green>{player.GetDatabasePlayer().Level}</color>레벨으로 올랐습니다.\n`를 눌러 콘솔창을 연 뒤 <color=green>.irc_stats</color> 명령어로 확인이 가능합니다!");
            }
            else
            {
                player.GetDatabasePlayer().Exp += exp;
            }
            return;
        }
    }
}