﻿using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UnityEngine;
using Rocket.Core.Extensions;
using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Core.Logging;

namespace Rocket.Unturned.Events
{
    public sealed class UnturnedEvents : MonoBehaviour, IRocketImplementationEvents
    {
        private static UnturnedEvents Instance;
        private void Awake()
        {
            Instance = this;
            Steam.OnServerDisconnected += (CSteamID r) => { OnPlayerDisconnected.TryInvoke(UnturnedPlayer.FromCSteamID(r)); };
            Steam.OnServerShutdown += () => { onShutdown.TryInvoke(); };
            Steam.OnServerConnected += (CSteamID r) => {
                UnturnedPlayer p = UnturnedPlayer.FromCSteamID(r);
                p.Player.gameObject.TryAddComponent<UnturnedPlayerFeatures>();
                p.Player.gameObject.TryAddComponent<UnturnedPlayerEvents>();
                OnBeforePlayerConnected.TryInvoke(p);
            };
        }

        public delegate void PlayerDisconnected(UnturnedPlayer player);
        public event PlayerDisconnected OnPlayerDisconnected;

        private event ImplementationShutdown onShutdown;
        public event ImplementationShutdown OnShutdown
        {
            add
            {
                onShutdown += value;
            }

            remove
            {
                onShutdown -= value;
            }
        }

        internal static void triggerOnPlayerConnected(UnturnedPlayer player)
        {
            Instance.OnPlayerConnected.TryInvoke(player);
        }

        public delegate void PlayerConnected(UnturnedPlayer player);
        public event PlayerConnected OnPlayerConnected;
        public event PlayerConnected OnBeforePlayerConnected;
    }
}