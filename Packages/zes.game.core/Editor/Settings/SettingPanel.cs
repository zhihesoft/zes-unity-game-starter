using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

namespace Zes.Settings
{
    public abstract class SettingPanel
    {
        public SettingPanel(AppConfig config)
        {
            this.config = config;
        }

        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract string Description { get; }

        public bool dirty { get; set; }

        protected AppConfig config;

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
    }
}
