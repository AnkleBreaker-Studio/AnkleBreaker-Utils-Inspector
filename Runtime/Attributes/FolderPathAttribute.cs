using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    public class FolderPathAttribute : PropertyAttribute
    {
        public string DefaultPath { get; private set; }
        public bool AbsolutePath { get; set; }
        public bool RequireExistingPath { get; set; }

        public FolderPathAttribute(string defaultPath = "")
        {
            DefaultPath = defaultPath;
        }
    }
}

