using System;
using UnityEngine;

// Token: 0x0200076F RID: 1903

namespace MCD.ShDebug
{
	public enum LogCategory
	{
		// Token: 0x04002E3F RID: 11839
		Audio = 1,
		// Token: 0x04002E40 RID: 11840
		AI,
		// Token: 0x04002E41 RID: 11841
		Animations = 4,
		// Token: 0x04002E42 RID: 11842
		GameFlow = 8,
		// Token: 0x04002E43 RID: 11843
		Difficulty = 16,
		// Token: 0x04002E44 RID: 11844
		UI = 32,
		// Token: 0x04002E45 RID: 11845
		Other = 64,
		// Token: 0x04002E46 RID: 11846
		Replays = 128,
		// Token: 0x04002E47 RID: 11847
		Postprocess = 256,
		// Token: 0x04002E48 RID: 11848
		Storage = 512
	}

	public enum LogPriority
	{
		// Token: 0x04002E39 RID: 11833
		Zero = 1,
		// Token: 0x04002E3A RID: 11834
		Low,
		// Token: 0x04002E3B RID: 11835
		Medium = 4,
		// Token: 0x04002E3C RID: 11836
		High = 8,
		// Token: 0x04002E3D RID: 11837
		Exception = 16
	}
	public class SHDebug
	{
		// Token: 0x1700028E RID: 654
		// (get) Token: 0x06002C1C RID: 11292 RVA: 0x0013183F File Offset: 0x0012FC3F
		public static SHLogger _Logger
		{
			get
			{
				if (SHDebug.logger == null)
				{
					SHDebug.logger = new SHLogger(Debug.unityLogger.logHandler);
				}
				return SHDebug.logger;
			}
		}

		// Token: 0x06002C1D RID: 11293 RVA: 0x00131864 File Offset: 0x0012FC64
		public static void SetFilePath(string filePath)
		{
			SHDebug.filePath = filePath;
		}

		// Token: 0x06002C1E RID: 11294 RVA: 0x0013186C File Offset: 0x0012FC6C
		public static void LogBreak(LogPriority priority = LogPriority.Zero)
		{
			SHDebug.Log("_________________________________", priority, LogCategory.Other);
		}

		// Token: 0x06002C1F RID: 11295 RVA: 0x0013187B File Offset: 0x0012FC7B
		public static void Log(object message, LogPriority priority = LogPriority.Zero, LogCategory category = LogCategory.Other)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Log, priority, category))
			{
				SHDebug._Logger.Log(LogType.Log, message);
			}
		}

		// Token: 0x06002C20 RID: 11296 RVA: 0x0013189B File Offset: 0x0012FC9B
		public static void Log(object message, UnityEngine.Object context, LogPriority priority = LogPriority.Zero, LogCategory category = LogCategory.Other)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Log, priority, category))
			{
				SHDebug._Logger.Log(LogType.Log, message, context);
			}
		}

		// Token: 0x06002C21 RID: 11297 RVA: 0x001318BC File Offset: 0x0012FCBC
		public static void LogFormat(string format, LogPriority priority = LogPriority.Zero, LogCategory category = LogCategory.Other, params object[] args)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Log, priority, category))
			{
				SHDebug._Logger.LogFormat(LogType.Log, format, args);
			}
		}

		// Token: 0x06002C22 RID: 11298 RVA: 0x001318DD File Offset: 0x0012FCDD
		public static void LogFormat(UnityEngine.Object context, string format, LogPriority priority = LogPriority.Zero, LogCategory category = LogCategory.Other, params object[] args)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Log, priority, category))
			{
				SHDebug._Logger.LogFormat(LogType.Log, context, format, args);
			}
		}

		// Token: 0x06002C23 RID: 11299 RVA: 0x00131900 File Offset: 0x0012FD00
		public static void LogError(object message, LogPriority priority = LogPriority.Low, LogCategory category = LogCategory.Other)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Error, priority, category))
			{
				SHDebug._Logger.Log(LogType.Error, message);
			}
		}

		// Token: 0x06002C24 RID: 11300 RVA: 0x00131920 File Offset: 0x0012FD20
		public static void LogError(object message, UnityEngine.Object context, LogPriority priority = LogPriority.Low, LogCategory category = LogCategory.Other)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Error, priority, category))
			{
				SHDebug._Logger.Log(LogType.Error, message, context);
			}
		}

		// Token: 0x06002C25 RID: 11301 RVA: 0x00131941 File Offset: 0x0012FD41
		public static void LogErrorFormat(string format, LogPriority priority = LogPriority.Low, LogCategory category = LogCategory.Other, params object[] args)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Error, priority, category))
			{
				SHDebug._Logger.LogFormat(LogType.Error, format, args);
			}
		}

		// Token: 0x06002C26 RID: 11302 RVA: 0x00131962 File Offset: 0x0012FD62
		public static void LogErrorFormat(UnityEngine.Object context, string format, LogPriority priority = LogPriority.Low, LogCategory category = LogCategory.Other, params object[] args)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Error, priority, category))
			{
				SHDebug._Logger.LogFormat(LogType.Error, context, format, args);
			}
		}

		// Token: 0x06002C27 RID: 11303 RVA: 0x00131985 File Offset: 0x0012FD85
		public static void LogException(Exception exception, LogCategory category = LogCategory.Other)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Exception, LogPriority.Exception, category))
			{
				SHDebug._Logger.LogException(exception);
			}
		}

		// Token: 0x06002C28 RID: 11304 RVA: 0x001319A5 File Offset: 0x0012FDA5
		public static void LogException(Exception exception, UnityEngine.Object context, LogCategory category = LogCategory.Other)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Exception, LogPriority.Exception, category))
			{
				SHDebug._Logger.LogException(exception, context);
			}
		}

		// Token: 0x06002C29 RID: 11305 RVA: 0x001319C6 File Offset: 0x0012FDC6
		public static void LogWarning(object message, LogPriority priority = LogPriority.Zero, LogCategory category = LogCategory.Other)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Warning, priority, category))
			{
				SHDebug._Logger.Log(LogType.Warning, message);
			}
		}

		// Token: 0x06002C2A RID: 11306 RVA: 0x001319E6 File Offset: 0x0012FDE6
		public static void LogWarning(object message, UnityEngine.Object context, LogPriority priority = LogPriority.Zero, LogCategory category = LogCategory.Other)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Warning, priority, category))
			{
				SHDebug._Logger.Log(LogType.Warning, message, context);
			}
		}

		// Token: 0x06002C2B RID: 11307 RVA: 0x00131A07 File Offset: 0x0012FE07
		public static void LogFormatWarning(string format, LogPriority priority = LogPriority.Zero, LogCategory category = LogCategory.Other, params object[] args)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Warning, priority, category))
			{
				SHDebug._Logger.LogFormat(LogType.Warning, format, args);
			}
		}

		// Token: 0x06002C2C RID: 11308 RVA: 0x00131A28 File Offset: 0x0012FE28
		public static void LogFormatWarning(UnityEngine.Object context, string format, LogPriority priority = LogPriority.Zero, LogCategory category = LogCategory.Other, params object[] args)
		{
			if (SHDebug._Logger.IsLogTypeAllowed(LogType.Warning, priority, category))
			{
				SHDebug._Logger.LogFormat(LogType.Warning, context, format, args);
			}
		}

		// Token: 0x04002E33 RID: 11827
		private static string fileName = "\\SuperHotLog.txt";

		// Token: 0x04002E34 RID: 11828
		private static string filePath = "C://";

		// Token: 0x04002E35 RID: 11829
		private static SHLogger logger;
	}

	public class SHLogger : Logger
	{
		// Token: 0x06002C2E RID: 11310 RVA: 0x00131A61 File Offset: 0x0012FE61
		public SHLogger(ILogHandler logHandler) : base(logHandler)
		{
			this.priorityMask = 0;
			this.categoryMask = 0;
		}

		// Token: 0x06002C2F RID: 11311 RVA: 0x00131A78 File Offset: 0x0012FE78
		public bool IsLogTypeAllowed(LogType logType, LogPriority logPriority, LogCategory logCategory)
		{
			bool flag = base.IsLogTypeAllowed(logType);
			return (this.priorityMask & (int)logPriority) == (int)logPriority && (this.categoryMask & (int)logCategory) == (int)logCategory && flag;
		}

		// Token: 0x04002E36 RID: 11830
		internal int priorityMask;

		// Token: 0x04002E37 RID: 11831
		internal int categoryMask;
	}
}