using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace MCD.Assets.Scripts
{
	public class SHGame
	{
		// Token: 0x06001E89 RID: 7817 RVA: 0x000D30B2 File Offset: 0x000D14B2
		public SHGame()
		{
			//Assert.raiseExceptions = true;
			Debug.unityLogger.logEnabled = Debug.isDebugBuild;
			this.Reset(true);
			if (!Application.isPlaying)
			{
				return;
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06001E8A RID: 7818 RVA: 0x000D30E1 File Offset: 0x000D14E1
		public static SHGame Instance
		{
			get
			{
				if (SHGame.instance == null)
				{
					SHGame.instance = new SHGame();
				}
				return SHGame.instance;
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06001E8B RID: 7819 RVA: 0x000D30FC File Offset: 0x000D14FC
		/*public SHQualitySettings.QualityLevel QualityLevel
		{
			get
			{
				return this.QualitySettings.Current;
			}
		}*/

		// Token: 0x06001E8C RID: 7820 RVA: 0x000D310C File Offset: 0x000D150C
		public void Reset(bool newParser = true)
		{
			//this.QualitySettings = new SHQualitySettings();
			//this.QualitySettings.Init();
			//this.AudioSettings = new SHAudioSettings();
			//this.PhysicSettings = new SHPhysicSettings();
			this.ControlsSettings = new SHControlsSettings();
			//this.AccessibilitySettings = new SHAccessibilitySettings();
			//this.SHStreamSettings = new SHStreamSettings();
			if (newParser)
			{
				this.SettingsParser = new SettingsParser();
			}
			else
			{
				//this.SettingsParser.Parser.Clear();
			}
			this.AllSettings = new ISHSettings[]
			{
				//this.QualitySettings,
				//this.AudioSettings,
				//this.PhysicSettings,
				this.ControlsSettings,
				//this.AccessibilitySettings,
				//this.SHStreamSettings
			};
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x06001E8D RID: 7821 RVA: 0x000D31CE File Offset: 0x000D15CE
		// (set) Token: 0x06001E8E RID: 7822 RVA: 0x000D31D6 File Offset: 0x000D15D6
		//public SHQualitySettings QualitySettings { get; private set; }

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x06001E8F RID: 7823 RVA: 0x000D31DF File Offset: 0x000D15DF
		// (set) Token: 0x06001E90 RID: 7824 RVA: 0x000D31E7 File Offset: 0x000D15E7
		//public SHConsoleCommands ConsoleCommands { get; private set; }

		// Token: 0x1700014E RID: 334
		// (get) Token: 0x06001E91 RID: 7825 RVA: 0x000D31F0 File Offset: 0x000D15F0
		// (set) Token: 0x06001E92 RID: 7826 RVA: 0x000D31F8 File Offset: 0x000D15F8
		//public SHAudioSettings AudioSettings { get; private set; }

		// Token: 0x1700014F RID: 335
		// (get) Token: 0x06001E93 RID: 7827 RVA: 0x000D3201 File Offset: 0x000D1601
		// (set) Token: 0x06001E94 RID: 7828 RVA: 0x000D3209 File Offset: 0x000D1609
		//public SHPhysicSettings PhysicSettings { get; private set; }

		// Token: 0x17000150 RID: 336
		// (get) Token: 0x06001E95 RID: 7829 RVA: 0x000D3212 File Offset: 0x000D1612
		// (set) Token: 0x06001E96 RID: 7830 RVA: 0x000D321A File Offset: 0x000D161A
		public SHControlsSettings ControlsSettings { get; private set; }

		// Token: 0x17000151 RID: 337
		// (get) Token: 0x06001E97 RID: 7831 RVA: 0x000D3223 File Offset: 0x000D1623
		// (set) Token: 0x06001E98 RID: 7832 RVA: 0x000D322B File Offset: 0x000D162B
		//public SHAccessibilitySettings AccessibilitySettings { get; private set; }

		// Token: 0x17000152 RID: 338
		// (get) Token: 0x06001E99 RID: 7833 RVA: 0x000D3234 File Offset: 0x000D1634
		// (set) Token: 0x06001E9A RID: 7834 RVA: 0x000D323C File Offset: 0x000D163C
		//public SHStreamSettings SHStreamSettings { get; private set; }

		// Token: 0x06001E9B RID: 7835 RVA: 0x000D3248 File Offset: 0x000D1648
		/*public void LoadSettingsParserAsync(Action onComplete)
		{
			this.SettingsParser.LoadFromFileAsync(delegate (bool r)
			{
				foreach (ISHSettings ishsettings in this.AllSettings)
				{
					try
					{
						ishsettings.Load(this.SettingsParser.Parser);
					}
					catch (Exception ex)
					{
						Debug.LogWarningFormat("Error during parsing: {0}, error message: {1}", new object[]
						{
							ishsettings.GetType().Name,
							ex.Message
						});
					}
				}
				this.ControlsSettings.SetProfile(InControlManager.customProfileInstance);
				onComplete.SafeCall();
			});
		}*/

		// Token: 0x06001E9C RID: 7836 RVA: 0x000D3280 File Offset: 0x000D1680
		/*public INIParser GetParser()
		{
			if (!this.SettingsParser.IsLoaded)
			{
				Debug.LogError("Settings are not loaded yet!");
				return null;
			}
			return this.SettingsParser.Parser;
		}*/

		// Token: 0x06001E9D RID: 7837 RVA: 0x000D32AC File Offset: 0x000D16AC
		/*public void SaveSettings(Action<StorageResult> OnSaveResult)
		{
			if (!this.SettingsParser.IsLoaded)
			{
				Debug.LogError("Settings are not loaded yet!");
				return;
			}
			if (!this.SettingsSaved)
			{
				this.QualitySettings.SaveSettings(this.SettingsParser.Parser);
				this.AudioSettings.Save(this.SettingsParser.Parser);
				this.PhysicSettings.Save(this.SettingsParser.Parser);
				this.ControlsSettings.Save(this.SettingsParser.Parser);
				this.AccessibilitySettings.Save(this.SettingsParser.Parser);
				this.SHStreamSettings.Save(this.SettingsParser.Parser);
				this.SettingsParser.Save();
				this.SettingsSaved = true;
				Singleton<PlayerManager>.Instance.Storage.CommitAsync(OnSaveResult, CommitParams.None);
			}
		}*/

		// Token: 0x04001EBA RID: 7866
		private static SHGame instance;

		// Token: 0x04001EC2 RID: 7874
		public SettingsParser SettingsParser;

		// Token: 0x04001EC3 RID: 7875
		private bool settingsInitialized;

		// Token: 0x04001EC4 RID: 7876
		private ISHSettings[] AllSettings;

		// Token: 0x04001EC5 RID: 7877
		public bool SettingsSaved;
	}
}