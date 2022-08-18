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

        private string GetMessage(object message)
        {
            return $"[{name}] {message}";
        }

        public void Info(object message)
        {
            UnityEngine.Debug.Log($"[INFO] {GetMessage(message)}");
        }

        public void Debug(object message)
        {
            UnityEngine.Debug.Log($"[DEBUG] {GetMessage(message)}");
        }

        public void Warn(object message)
        {
            UnityEngine.Debug.LogWarning($"[WARN] {GetMessage(message)}");
        }

        public void Error(object message)
        {
            UnityEngine.Debug.LogError($"[ERROR] {GetMessage(message)}");
        }
    }
}
