﻿// FFXIVAPP.Client
// AppContextHelper.cs
// 
// © 2013 Ryan Wilson

using System;
using System.Collections.Generic;
using System.Linq;
using FFXIVAPP.Client.Delegates;
using FFXIVAPP.Client.Utilities;
using FFXIVAPP.Client.ViewModels;
using FFXIVAPP.Common.Core.Constant;
using FFXIVAPP.Common.Core.Memory;
using FFXIVAPP.Common.Core.Parse;
using Newtonsoft.Json;
using SmartAssembly.Attributes;
using PlayerEntity = FFXIVAPP.Common.Core.Memory.PlayerEntity;

namespace FFXIVAPP.Client.Helpers
{
    [DoNotObfuscate]
    public class AppContextHelper
    {
        #region Property Backings

        private static AppContextHelper _instance;
        private List<uint> _pets;

        public static AppContextHelper Instance
        {
            get { return _instance ?? (_instance = new AppContextHelper()); }
            set { _instance = value; }
        }

        public List<uint> Pets
        {
            get
            {
                return _pets ?? (_pets = new List<uint>
                {
                    1398,
                    1399,
                    1400,
                    1401,
                    1402,
                    1403,
                    1404,
                    2095
                });
            }
        }

        public ActorEntity CurrentUser { get; set; }
        public PlayerEntity CurrentUserStats { get; set; }

        #endregion

        public void RaiseNewConstants(ConstantsEntity constantsEntity)
        {
            PluginHost.Instance.RaiseNewConstantsEntity(constantsEntity);
        }

        public void RaiseNewChatLogEntry(ChatLogEntry chatLogEntry)
        {
            if (ChatLogWorkerDelegate.IsPaused)
            {
                return;
            }
            AppViewModel.Instance.ChatHistory.Add(chatLogEntry);
            // process official plugins
            if (chatLogEntry.Line.ToLower()
                            .StartsWith("com:"))
            {
                LogPublisher.HandleCommands(chatLogEntry);
            }
            LogPublisher.Parse.Process(chatLogEntry);
            // THIRD PARTY
            PluginHost.Instance.RaiseNewChatLogEntry(chatLogEntry);
        }

        public void RaiseNewMonsterEntries(List<ActorEntity> actorEntities)
        {
            if (!actorEntities.Any())
            {
                return;
            }
            MonsterWorkerDelegate.NPCEntries = actorEntities;
            Func<bool> saveToDictionary = delegate
            {
                try
                {
                    var enumerable = MonsterWorkerDelegate.UniqueNPCEntries.ToList();
                    foreach (var actor in actorEntities)
                    {
                        var exists = enumerable.FirstOrDefault(n => n.ID == actor.ID);
                        if (exists != null)
                        {
                            continue;
                        }
                        if (HttpPostHelper.IsValidJson(JsonConvert.SerializeObject(actor)))
                        {
                            MonsterWorkerDelegate.UniqueNPCEntries.Add(actor);
                        }
                        XIVDBViewModel.Instance.MonsterSeen++;
                    }
                }
                catch (Exception ex)
                {
                }
                return true;
            };
            saveToDictionary.BeginInvoke(delegate
            {
                var chunkSize = MonsterWorkerDelegate.UploadHelper.ChunkSize;
                var chunksProcessed = MonsterWorkerDelegate.UploadHelper.ChunksProcessed;
                if (MonsterWorkerDelegate.UniqueNPCEntries.Count <= (chunkSize * (chunksProcessed + 1)))
                {
                    return;
                }
                MonsterWorkerDelegate.ProcessUploads();
            }, saveToDictionary);
            // THIRD PARTY
            PluginHost.Instance.RaiseNewMonsterEntries(actorEntities);
        }

        public void RaiseNewNPCEntries(List<ActorEntity> actorEntities)
        {
            if (!actorEntities.Any())
            {
                return;
            }
            NPCWorkerDelegate.NPCEntries = actorEntities;
            Func<bool> saveToDictionary = delegate
            {
                try
                {
                    var enumerable = NPCWorkerDelegate.UniqueNPCEntries.ToList();
                    foreach (var actor in actorEntities)
                    {
                        var exists = enumerable.FirstOrDefault(n => n.NPCID2 == actor.NPCID2);
                        if (exists != null)
                        {
                            continue;
                        }
                        if (HttpPostHelper.IsValidJson(JsonConvert.SerializeObject(actor)))
                        {
                            NPCWorkerDelegate.UniqueNPCEntries.Add(actor);
                        }
                        XIVDBViewModel.Instance.NPCSeen++;
                    }
                }
                catch (Exception ex)
                {
                }
                return true;
            };
            saveToDictionary.BeginInvoke(delegate
            {
                var chunkSize = NPCWorkerDelegate.UploadHelper.ChunkSize;
                var chunksProcessed = NPCWorkerDelegate.UploadHelper.ChunksProcessed;
                if (NPCWorkerDelegate.UniqueNPCEntries.Count <= (chunkSize * (chunksProcessed + 1)))
                {
                    return;
                }
                NPCWorkerDelegate.ProcessUploads();
            }, saveToDictionary);
            // THIRD PARTY
            PluginHost.Instance.RaiseNewNPCEntries(actorEntities);
        }

        public void RaiseNewPCEntries(List<ActorEntity> actorEntities)
        {
            if (!actorEntities.Any())
            {
                return;
            }
            PCWorkerDelegate.NPCEntries = actorEntities;
            CurrentUser = actorEntities.First();
            Func<bool> saveToDictionary = delegate
            {
                try
                {
                    var enumerable = PCWorkerDelegate.UniqueNPCEntries.ToList();
                    foreach (var actor in actorEntities)
                    {
                        var exists = enumerable.FirstOrDefault(n => String.Equals(n.Name, actor.Name, StringComparison.CurrentCultureIgnoreCase));
                        if (exists != null)
                        {
                            continue;
                        }
                        PCWorkerDelegate.UniqueNPCEntries.Add(actor);
                        XIVDBViewModel.Instance.PCSeen++;
                    }
                }
                catch (Exception ex)
                {
                }
                return true;
            };
            saveToDictionary.BeginInvoke(delegate
            {
                var chunkSize = PCWorkerDelegate.UploadHelper.ChunkSize;
                var chunksProcessed = PCWorkerDelegate.UploadHelper.ChunksProcessed;
                if (PCWorkerDelegate.UniqueNPCEntries.Count <= (chunkSize * (chunksProcessed + 1)))
                {
                    return;
                }
                PCWorkerDelegate.ProcessUploads();
            }, saveToDictionary);
            // THIRD PARTY
            PluginHost.Instance.RaiseNewPCEntries(actorEntities);
        }

        public void RaiseNewPlayerEntity(PlayerEntity playerEntity)
        {
            CurrentUserStats = playerEntity;
            // THIRD PARTY
            PluginHost.Instance.RaiseNewPlayerEntity(playerEntity);
        }

        public void RaiseNewTargetEntity(TargetEntity targetEntity)
        {
            // THIRD PARTY
            PluginHost.Instance.RaiseNewTargetEntity(targetEntity);
        }

        public void RaiseNewParseEntity(ParseEntity parseEntity)
        {
            // THIRD PARTY
            PluginHost.Instance.RaiseNewParseEntity(parseEntity);
        }
    }
}