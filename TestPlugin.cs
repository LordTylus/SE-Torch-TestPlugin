using NLog;
using Sandbox.ModAPI;
using System;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using Torch;
using Torch.API;
using Torch.API.Managers;
using Torch.API.Plugins;
using Torch.API.Session;
using Torch.Managers.PatchManager;
using Torch.Session;

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

        private void SessionChanged(ITorchSession session, TorchSessionState newState) {

            Log.Info("Session-State is now " + newState);
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
