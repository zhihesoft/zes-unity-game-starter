using System.Collections.Generic;
using UnityEngine;

namespace Zes
{
    public class Logger
    {
        private static Dictionary<string, Logger> loggers = new Dictionary<string, Logger>();

        public static Logger GetLogger<T>()
        {
            return GetLogger(typeof(T).FullName);
        }

        public static Logger GetLogger(string name)
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
            Debug.Log($"[INFO] {getMessage(message)}");
        }

        public void debug(object message)
        {
            Debug.Log($"[DEBUG] {getMessage(message)}");
        }

        public void warn(object message)
        {
            Debug.LogWarning($"[WARN] {getMessage(message)}");
        }

        public void error(object message)
        {
            Debug.LogError($"[ERROR] {getMessage(message)}");
        }
    }
}
