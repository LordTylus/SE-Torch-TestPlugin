using System.Windows;
using System.Windows.Controls;

namespace TestPlugin {

    public partial class TestControl : UserControl {

        private TestPlugin Plugin { get; }

        private TestControl() {
            InitializeComponent();
        }

        public TestControl(TestPlugin plugin) : this() {
            Plugin = plugin;
            DataContext = plugin.Config;
        }

        private void SaveButton_OnClick(object sender, RoutedEventArgs e) {
            Plugin.Save();
        }
    }
}
