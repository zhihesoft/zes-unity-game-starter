using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Zes
{
    public class Logger
    {
        private static Dictionary<string, Logger> loggers = new Dictionary<string, Logger>();

        public static Logger getLogger<T>()
        {
            return getLogger(typeof(T).FullName);
        }

        public static Logger getLogger(string name)
        {
            if (loggers.ContainsKey(name))
            {
                return loggers[name];
            }
            Logger logger = new Logger(name);
            loggers.Add(name, logger);
            return logger;
        }

        private Logger(string name)
        {
            this.name = name;
        }

        private string name;

        private string getMessage(object message)
        {
            return $"[{name}] {message}";
        }

        public void info(object message)
        {
            Debug.Log($"[INF] {getMessage(message)}");
        }

        public void warn(object message)
        {
            Debug.LogWarning($"[WRN] {getMessage(message)}");
        }

        public void error(object message)
        {
            Debug.LogError($"[ERR] {getMessage(message)}");
        }
    }
}
