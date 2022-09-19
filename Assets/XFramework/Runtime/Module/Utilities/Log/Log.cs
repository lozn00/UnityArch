using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Text;

namespace XFramework
{
    public static class Log
    {
        public static ILog ILog { get; set; }

        /// <summary>
        /// Log����ͼ���С������Ĳ���ʾLog
        /// </summary>
        public static int LogLevel { get; set; } = DebugLevel;

        private const int DebugLevel = 1;
        private const int InfoLevel = 2;
        private const int WarnningLevel = 3;

        private static bool CheckLogLevel(int level)
        {
            return LogLevel <= level;
        }

        public static void Debug(string msg)
        {
            if (!CheckLogLevel(DebugLevel))
                return;

            ILog.Debug(msg);
        }

        public static void Debug(string msg, params object[] args)
        {
            if (!CheckLogLevel(DebugLevel))
                return;

            ILog.Debug(msg, args);
        }

        public static void Info(string msg)
        {
            if (!CheckLogLevel(InfoLevel))
                return;

            ILog.Info(msg);
        }

        public static void Info(string msg, params object[] args)
        {
            if (!CheckLogLevel(InfoLevel))
                return;

            ILog.Info(msg, args);
        }

        public static void Warnning(string msg)
        {
            if (!CheckLogLevel(WarnningLevel))
                return;

            ILog.Warnning(msg);
        }

        public static void Warnning(string msg, params object[] args)
        {
            if (!CheckLogLevel(WarnningLevel))
                return;

            ILog.Warnning(msg, args);
        }

        public static void Error(string msg)
        {
            StackTrace st = new StackTrace(1, true);
            string content = string.Format(msg);
            int count = st.FrameCount;
            var frames = st.GetFrames();
            StringBuilder sb = new StringBuilder();
             for (int i = 0; i < count; i++)
                {
                    String fileName = frames[i].GetFileName();
                    int assetsStart = fileName == null ? -1 : fileName.IndexOf("Assets");
                    if (assetsStart > 0)
                    {
                        fileName = fileName.Replace("\\", "/");
                        fileName = fileName.Substring(assetsStart);
                    }
                    else
                    {
                        fileName = "<Unknown File>";
                    }
                    sb.Append($"{frames[i].GetMethod().Name}({frames[i].GetMethod().GetParameters()})[{frames[i].GetILOffset()}](at {fileName}:{frames[i].GetFileLineNumber()})");
                    if (i != count - 1)
                    {
                        sb.Append("\n");

                    }
                }
                content = $"{content}\n{sb.ToString()}";
            ILog.Error(content);
            //ILog.Error(msg);
        }

        public static void Error(Exception ex)
        {
            if (ex.Data.Contains("StackTrace"))
            {
                ILog.Error($"{ex.Data["StackTrace"]}\n{ex}");
                return;
            }
            ILog.Error(ex);
        }

        public static void Error(string msg, params object[] args)
        {
            StackTrace st = new StackTrace(1, true);
            string content = string.Format(msg, args);
            int count = st.FrameCount;
            var frames = st.GetFrames();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < count; i++)
            {
                String fileName = frames[i].GetFileName();
                int assetsStart = fileName == null ? -1 : fileName.IndexOf("Assets");
                if (assetsStart > 0)
                {
                    fileName = fileName.Replace("\\", "/");
                    fileName = fileName.Substring(assetsStart);
                }
                else
                {
                    fileName = "<Unknown File>";
                }
                sb.Append($"{frames[i].GetMethod().Name}({frames[i].GetMethod().GetParameters()})[{frames[i].GetILOffset()}](at {fileName}:{frames[i].GetFileLineNumber()})");
                if (i != count - 1)
                {
                    sb.Append("\n");

                }
            }
            content = $"{content}\n{sb.ToString()}";
            ILog.Error(content);
            //content = $"{content}\n{st}";
            //content = System.Text.RegularExpressions.Regex.Replace(content, @"  at ", "");
        }
    }
}
