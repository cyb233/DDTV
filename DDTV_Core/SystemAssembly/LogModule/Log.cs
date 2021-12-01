﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace DDTV_Core.SystemAssembly.Log
{
    /// <summary>
    /// 核心日志信息处理
    /// </summary>
    public class Log
    {
        /// <summary>
        /// 日志等级(向下包含)
        /// </summary>
        private static LogClass.LogType LogLevel = LogClass.LogType.All;
        /// <summary>
        /// Log系统初始化
        /// </summary>
        /// <param name="log">日志等级</param>
        public static void LogInit(LogClass.LogType log = LogClass.LogType.All)
        {
            LogLevel= log;
            TimeModule.Time.Config.Init();
            LogDB.Config.SQLiteInit(false);


            AddLog(nameof(Log), LogClass.LogType.Info, "Log系统初始化完成");
        }
        /// <summary>
        /// 增加日志
        /// </summary>
        /// <param name="Source">日志来源(类名)</param>
        /// <param name="logType">日志类型</param>
        /// <param name="Message">日志内容</param>
        /// <param name="IsError">是否是错误(错误内容会出了数据库外另外写一份txt文本记录详细错误日志和堆栈)</param>
        /// <param name="exception">IsError为真时有效，错误日志的Exception信息</param>
        public static void AddLog(string Source, LogClass.LogType logType, string Message, bool IsError = false, Exception? exception = null)
        {
            Task.Run(() =>
            {
                LogClass logClass = new()
                {
                    RunningTime =TimeModule.Time.Operate.GetRunMilliseconds(),
                    Message = Message,
                    Source = Source,
                    Time = DateTime.Now,
                    Type = logType,
                    IsError= IsError,
                    exception= exception
                };
                if (IsError)
                {
                    ErrorLogFileWrite(logClass);// Source, $"{Message},错误堆栈:\n{exception.ToString()}");
                    Message = Message + " 详细信息已写入txt文本日志中";
                }
                if (logType <= LogLevel&& logType!= LogClass.LogType.Info_Transcod)
                {
                    Console.WriteLine($"{logClass.Time}:[{Enum.GetName(typeof(LogClass.LogType), (int)logType)}][{Source}]{Message}\n");
                }
                if (logType < LogClass.LogType.Trace)
                {
                    if (!LogDB.Operate.AddDb(logClass))
                    {
                        ErrorLogFileWrite(new LogClass()
                        {
                            exception = null,
                            IsError = true,
                            Message = "日志数据库写入失败",
                            RunningTime = TimeModule.Time.Operate.GetRunMilliseconds(),
                            Source = nameof(Log),
                            Time = DateTime.Now,
                            Type = LogClass.LogType.Error
                        });
                    }
                }
            });
        }

        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="Source"></param>
        /// <param name="Message"></param>
        public static void ErrorLogFileWrite(LogClass logClass)// string Source, string Message)
        {
            string ErrorText = $"{logClass.Time}:[Error][{logClass.Source}][{logClass.RunningTime}]{logClass.Message}！";
            File.AppendAllText(LogDB.ErrorFilePath, ErrorText, Encoding.UTF8);
        }
    }
}
