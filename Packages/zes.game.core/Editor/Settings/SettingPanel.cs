using UnityEditor;

namespace Zes.Settings
{
    public abstract class SettingPanel
    {
        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }

        public bool dirty { get; set; }

        public SettingsManager manager { get; set; }

        /// <summary>
        /// render gui
        /// if return true, means config is dirty, should save it.
        /// </summary>
        /// <returns></returns>
        abstract public void OnGUI();
        public virtual void OnShow() { }
        public virtual void OnHide() { }

        protected string TextField(string label, string value)
        {
            var ret = EditorGUILayout.TextField(label, value);
            if (!dirty)
            {
                dirty = ret != value;
            }
            return ret;
        }

        protected int IntField(string label, int value)
        {
            int ret = EditorGUILayout.IntField(label, value);
            if (!dirty)
            {
                dirty = ret != value;
            }
            return ret;
        }

        protected bool BoolField(string label, bool value)
        {
            var ret = EditorGUILayout.Toggle(label, value);
            if (!dirty)
            {
                dirty = ret != value;
            }
            return ret;
        }
    }
}
