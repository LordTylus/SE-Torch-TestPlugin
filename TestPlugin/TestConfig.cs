using System;
using System.Collections.Generic;
using Torch;

namespace TestPlugin {
    public class TestConfig : ViewModel {

        private string _Username = "root";
        private string _Password = "";
        private int _AuthToken = 0;
        private bool _PreferBulkChanges = true;

        private List<TestEntry> _entries = new List<TestEntry>();

        public string Username { get => _Username; set => SetValue(ref _Username, value); }
        public string Password { get => _Password; set => SetValue(ref _Password, value); }
        public int AuthToken { get => _AuthToken; set => SetValue(ref _AuthToken, value); }
        public bool PreferBulkChanges { get => _PreferBulkChanges; set => SetValue(ref _PreferBulkChanges, value); }

        public List<TestEntry> Entries { get => _entries; set => SetValue(ref _entries, value);    }
    }

public class TestEntry {

    /// <summary>
    /// A unique identifier for the plugin that identifies the dependency.
    /// </summary>
    public int Plugin { get; set; }

    /// <summary>
    /// The plugin minimum version. This must include a string in the format of #[.#[.#]] for update checking purposes.
    /// </summary>
    public string MinVersion { get; set; }
}
}
