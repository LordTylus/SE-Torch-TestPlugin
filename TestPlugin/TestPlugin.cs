using NLog;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.Entities.Cube;
using Sandbox.Game.World;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.Managers.PatchManager;
using Torch.Mod;
using Torch.Session;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.ObjectBuilders;

namespace TestPlugin {

    public class TestPlugin : TorchPluginBase, IWpfPlugin {

        public static readonly Logger Log = LogManager.GetCurrentClassLogger();

        private TestControl _control;
        public UserControl GetControl() => _control ?? (_control = new TestControl(this));

        private Persistent<TestConfig> _config;
        public TestConfig Config => _config?.Data;

        public override void Init(ITorchBase torch) {
            base.Init(torch);

            SetupConfig();

            var sessionManager = Torch.Managers.GetManager<TorchSessionManager>();
            if (sessionManager != null)
                sessionManager.SessionStateChanged += SessionChanged;
            else
                Log.Warn("No session manager loaded!");

            Save();
        }

        private void SessionChanged(ITorchSession session, TorchSessionState state) {

            switch (state) {

                case TorchSessionState.Loaded:

                    var multiplayerManagerBase = Torch.CurrentSession.Managers.GetManager<IMultiplayerManagerBase>();

                    if (multiplayerManagerBase != null) {

                        multiplayerManagerBase.PlayerJoined += PlayerJoined;
                        multiplayerManagerBase.PlayerLeft += PlayerLeft;

                    } else {
                        Log.Warn("No multiplayer manager loaded!");
                    }

                      break;

                case TorchSessionState.Unloading:

                    if (multiplayerManagerBase != null) {

                        multiplayerManagerBase.PlayerJoined -= PlayerJoined;
                        multiplayerManagerBase.PlayerLeft -= PlayerLeft;
                    }

                    break;
            }
        }

        public void DoStuff(MySlimBlock slimBlock, ReadOnlyDictionary<MyDefinitionId, int> amountDict) {

            MyCubeGrid grid = null;
            if(grid.PlayerPresenceTier == MyUpdateTiersPlayerPresence.Normal) {
                //DO YOUR STUFF
            }


            MyCubeBlock block = slimBlock.FatBlock;

            /* Copy the dictiornary as I want to modify it */
            var typeIdDict = new Dictionary<MyDefinitionId, int>(amountDict);

            /* all Inventories */
            for (int i = 0; i < block.InventoryCount; i++) {
                
                var inventory = block.GetInventory(i);

                var itemsList = inventory.GetItems();

                /* We loop through the items in reverse otherwise the we run out of bounds. */
                for(int j = itemsList.Count; j >= 0; j--) {

                    var item = itemsList[j];
                    var definitionId = item.Content.GetObjectId();

                    /* If that definition is not in dictionary ignore. */
                    if(typeIdDict.TryGetValue(definitionId, out int value)) {

                        value--;

                        /* checking if we are below defined limit */
                        if (value < 0)
                            inventory.RemoveItemsAt(j);

                        /* writing reduced value back in dictionary */
                        typeIdDict[definitionId] = value;
                    }
                }

                /* We probably (most likely) only need to refresh it once. after we are done fiddling around with it. */
                inventory.Refresh();
            }
        }

        private void PlayerLeft(IPlayer player) {

            long idendity = MySession.Static.Players.TryGetIdentityId(player.SteamId);

            Log.Debug("Player " + player.Name + " with ID " + player.SteamId + " and Identity " + idendity + " left.");

            if (idendity == 0)
                return;

            if (Config.RemoveGpsOnJoin) {

                Log.Debug("Removing Biggest Grid GPS for Player #" + idendity);
                RemoveGpsFromPlayer(idendity);
            }
        }

        private void PlayerJoined(IPlayer player) {

            long idendity = MySession.Static.Players.TryGetIdentityId(player.SteamId);

            Log.Debug("Player " + player.Name + " with ID " + player.SteamId + " and Identity " + idendity + " joined.");

            if (idendity == 0)
                return;

            if (Config.RemoveGpsOnJoin) {

                Log.Debug("Removing Biggest Grid GPS for Player #" + idendity);
                RemoveGpsFromPlayer(idendity);
            }
        }

        private void SetupConfig() {

            var configFile = Path.Combine(StoragePath, "TestConfig.cfg");

            try {

                _config = Persistent<TestConfig>.Load(configFile);

            } catch (Exception e) {
                Log.Warn(e);
            }

            if (_config?.Data == null) {

                Log.Info("Create Default Config, because none was found!");

                _config = new Persistent<TestConfig>(configFile, new TestConfig());
                _config.Save();
            }
        }

        public void Save() {
            try {
                _config.Save();
                Log.Info("Configuration Saved.");
            } catch (IOException e) {
                Log.Warn(e, "Configuration failed to save");
            }
        }
    }
}
[Command("whatsmyip")]
[Permission(MyPromoteLevel.None)]
public void GetIP(ulong steamId = 0) {
    if (steamId == 0)
        steamId = Context.Player.SteamUserId;

    VRage.GameServices.MyP2PSessionState statehack = new VRage.GameServices.MyP2PSessionState();
    MySteamServiceWrapper.Static.Peer2Peer.GetSessionState(steamId, ref statehack);
    var ip = new IPAddress(BitConverter.GetBytes(statehack.RemoteIP).Reverse().ToArray());
    Context.Respond($"Your IP is {ip}");
}
