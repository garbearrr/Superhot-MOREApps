extern alias SHSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

using UnityEngine;
using MCD.Audio;

namespace MCD.StoryApps.Slasher
{
	public class APPSlasherGame : SHSharp::SHGUIappbase
	{
		// Token: 0x06002710 RID: 10000 RVA: 0x001187C4 File Offset: 0x00116BC4
		public APPSlasherGame() : base("slash'em-all", true)
		{
			this.APPINSTRUCTION.Kill();
			this.APPLABEL.Kill();
			this.enemies = new List<Monster>();
			this.PreparePlayer();
			this.PrepareStats();
			this.ui = new SlasherUI();
			base.AddSubView(this.ui);
			this.currentGameState = APPSlasherGame.GameState.WaveNotStarted;

			// TODO: Add the bg music resource
			this.backgroundMusic = MCDAudio.PlayMusic(null, this.musicClip, 1f, true);
			this.backgroundMusicInstance = "";
		}

		// Token: 0x06002711 RID: 10001 RVA: 0x00118894 File Offset: 0x00116C94
		public override void Update()
		{
			switch (this.currentGameState)
			{
				case APPSlasherGame.GameState.WaveNotStarted:
					this.waveNumber++;
					if (this.waveNumber != 1)
					{
						this.GenerateNumberOfEnemies();
					}
					this.SpawnEnemies();
					this.wavePrepareTimer = 0f;
					this.currentGameState = APPSlasherGame.GameState.WavePreparing;
					this.ui.SetEnemiesNumber(this.standardEnemiesInWave, this.fastEnemiesInWave, this.heavyEnemiesInWave);
					this.ui.SetWaveText(this.waveNumber);
					this.ui.ShowWavePrepareText();
					break;
				case APPSlasherGame.GameState.WavePreparing:
					if (this.wavePrepareTimer > this.wavePrepareTime)
					{
						this.ui.HideWavePrepareText();
						this.currentGameState = APPSlasherGame.GameState.WaveStarted;
						SHSharp::AudioManager.Instance.PlayClip(this.waveStartClip, null, SlasherCommon.VOL);
						this.wavePrepareTimer = 0f;
					}
					else
					{
						this.wavePrepareTimer += Time.unscaledDeltaTime;
					}
					break;
				case APPSlasherGame.GameState.WaveStarted:
					{
						int count = SHSharp::SHGUI.current.views.Count;
						this.ProcessPlayerActions();
						this.ProcessDeadEnemies();
						foreach (Monster monster in this.enemies)
						{
							monster.GoToTarget(this.player.x);
						}
						if (this.enemies.Count == 0)
						{
							SHSharp::AudioManager.Instance.PlayClip(this.winClip, null, SlasherCommon.VOL);
							this.currentGameState = APPSlasherGame.GameState.WaveFinished;
							this.ui.ShowWaveClearedText();
						}
						if (this.player.CurrentHealth == 0)
						{
							this.player.Kill();
							SHSharp::AudioManager.Instance.PlayClip(this.lostClip, null, SlasherCommon.VOL);
							this.ui.ShowLostText();
							this.currentGameState = APPSlasherGame.GameState.Lost;
						}
						this.CheckLeftEnemies();
						break;
					}
				case APPSlasherGame.GameState.WaveFinished:
					if (this.waveFinishedTimer > this.waveFinishedTime)
					{
						this.ui.HideWaveClearedText();
						this.currentGameState = APPSlasherGame.GameState.WaveNotStarted;
						this.waveFinishedTimer = 0f;
					}
					else
					{
						this.waveFinishedTimer += Time.unscaledDeltaTime;
					}
					break;
				case APPSlasherGame.GameState.Lost:
					this.ui.SetHealthBarProgress(this.player.CurrentHealth);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			base.Update();
		}

		// Token: 0x06002712 RID: 10002 RVA: 0x00118B24 File Offset: 0x00116F24
		public override void Redraw(int offx, int offy)
		{
			base.Redraw(offx, offy);
			this.player.Redraw(offx, offy);
			foreach (SHSharp::SHGUIsprite shguisprite in this.enemies)
			{
				shguisprite.Redraw(offx, offy);
			}
			this.ui.Redraw(offx, offy);
		}

		// Token: 0x06002713 RID: 10003 RVA: 0x00118BA4 File Offset: 0x00116FA4
		public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
		{
			switch (this.currentGameState)
			{
				case APPSlasherGame.GameState.WaveNotStarted:
					break;
				case APPSlasherGame.GameState.WavePreparing:
					break;
				case APPSlasherGame.GameState.WaveStarted:
					this.player.ReactToInputKeyboard(key);
					break;
				case APPSlasherGame.GameState.WaveFinished:
					break;
				case APPSlasherGame.GameState.Lost:
					if (key == SHSharp::SHGUIinput.enter)
					{
						this.ui.HideLostText();
						this.currentGameState = APPSlasherGame.GameState.WaveNotStarted;
						foreach (Monster monster in this.enemies)
						{
							monster.Kill();
						}
						this.enemies.Clear();
						this.player.Kill();
						this.PreparePlayer();
						this.PrepareStats();
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			if (key == SHSharp::SHGUIinput.esc)
			{
				SHSharp::SHGUI.current.PopView();
				this.OnExit();
			}
		}

		// Token: 0x06002714 RID: 10004 RVA: 0x00118CA4 File Offset: 0x001170A4
		public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
		{
			if (clicked && this.currentGameState == APPSlasherGame.GameState.WaveStarted)
			{
				this.player.ReactToInputMouse(x, y, clicked, scroll);
			}
		}

		// Token: 0x06002715 RID: 10005 RVA: 0x00118CC8 File Offset: 0x001170C8
		public override void OnExit()
		{
			SHSharp::AudioManager.Instance.FadeoutAndDestroy(this.backgroundMusic.gameObject, 1f);
			//Singleton<PlayerManager>.Instance.Account.SetRichPresence(SystemAccount.RichPresence.RICH_PRESENCE_MENU);
		}

		// Token: 0x06002716 RID: 10006 RVA: 0x00118CF8 File Offset: 0x001170F8
		private void PreparePlayer()
		{
			this.player = new Player();
			SlasherCommon.SetCenterPosition(34, out this.player.x, out this.player.y);
			base.AddSubView(this.player);
			this.previousHealth = this.player.CurrentHealth;
		}

		// Token: 0x06002717 RID: 10007 RVA: 0x00118D4B File Offset: 0x0011714B
		private void PrepareStats()
		{
			this.leftEnemiesRangeBottom = -20;
			this.leftEnemiesRangeTop = -10;
			this.rightEnemiesRangeBottom = 65;
			this.rightEnemiesRangeTop = 75;
			this.waveNumber = 0;
			this.standardEnemiesInWave = 2;
			this.fastEnemiesInWave = 0;
			this.heavyEnemiesInWave = 0;
		}

		// Token: 0x06002718 RID: 10008 RVA: 0x00118D8C File Offset: 0x0011718C
		private void ProcessPlayerActions()
		{
			if (this.player.CurrentState == Player.PlayerState.Attacking && !this.damageDealt)
			{
				bool flag = false;
				foreach (Monster monster in this.enemies)
				{
					if (this.player.CurrentDirection == Player.Direction.Left)
					{
						if (SlasherCommon.IsInRange(monster.x + monster.FrameLength, this.player.x + Player.FrameLength - Player.AttackRange, this.player.x + Player.FrameLength))
						{
							monster.ReceiveDamage(Player.Damage);
							flag = true;
						}
					}
					else if (SlasherCommon.IsInRange(monster.x, this.player.x + Player.FrameLength, this.player.x + Player.FrameLength + Player.AttackRange))
					{
						monster.ReceiveDamage(Player.Damage);
						flag = true;
					}
				}
				if (flag)
				{
					SHSharp::AudioManager.Instance.PlayClip(this.enemyKillClip, null, SlasherCommon.VOL);
				}
				this.damageDealt = true;
			}
			else if (this.player.CurrentState == Player.PlayerState.Idle)
			{
				this.damageDealt = false;
			}
			if (this.player.CurrentHealth < this.previousHealth)
			{
				SHSharp::AudioManager.Instance.PlayClip(this.playerHitClip, null, SlasherCommon.VOL);
				this.previousHealth = this.player.CurrentHealth;
			}
			this.ui.SetHealthBarProgress(this.player.CurrentHealth);
		}

		// Token: 0x06002719 RID: 10009 RVA: 0x00118F48 File Offset: 0x00117348
		private void ProcessDeadEnemies()
		{
			for (int i = 0; i < this.enemies.Count; i++)
			{
				Monster monster = this.enemies[i];
				if (monster.CurrentHealth == 0)
				{
					monster.Die();
					this.enemies.Remove(monster);
				}
			}
		}

		// Token: 0x0600271A RID: 10010 RVA: 0x00118F9C File Offset: 0x0011739C
		private void SpawnEnemies()
		{
			for (int i = 0; i < this.standardEnemiesInWave; i++)
			{
				MeleeZombie meleeZombie = new MeleeZombie
				{
					Target = this.player
				};
				this.enemies.Add(meleeZombie);
				meleeZombie.x = this.GenerateEnemyPosition(false);
				meleeZombie.y = 11;
				meleeZombie.Speed = UnityEngine.Random.Range(meleeZombie.Speed, meleeZombie.SpeedMax);
				base.AddSubView(meleeZombie);
			}
			for (int j = 0; j < this.fastEnemiesInWave; j++)
			{
				FastZombie fastZombie = new FastZombie
				{
					Target = this.player
				};
				this.enemies.Add(fastZombie);
				fastZombie.x = this.GenerateEnemyPosition(false);
				fastZombie.y = 17;
				fastZombie.Speed = UnityEngine.Random.Range(fastZombie.Speed, fastZombie.SpeedMax);
				base.AddSubView(fastZombie);
			}
			for (int k = 0; k < this.heavyEnemiesInWave; k++)
			{
				HeavyZombie heavyZombie = new HeavyZombie
				{
					Target = this.player
				};
				this.enemies.Add(heavyZombie);
				heavyZombie.x = this.GenerateEnemyPosition(true);
				heavyZombie.y = 7;
				heavyZombie.Speed = UnityEngine.Random.Range(heavyZombie.Speed, heavyZombie.SpeedMax);
				base.AddSubView(heavyZombie);
			}
		}

		// Token: 0x0600271B RID: 10011 RVA: 0x00119104 File Offset: 0x00117504
		private int GenerateEnemyPosition(bool isHeavy)
		{
			int result = 0;
			int num = UnityEngine.Random.Range(0, 2);
			int num2 = 0;
			if (num != 0)
			{
				if (num == 1)
				{
					int generatedPosition;
					bool flag;
					do
					{
						flag = false;
						if (num2 >= 50)
						{
							this.rightEnemiesRangeTop++;
							num2 = 0;
						}
						if (isHeavy)
						{
							generatedPosition = UnityEngine.Random.Range(80, 150);
						}
						else
						{
							generatedPosition = UnityEngine.Random.Range(this.rightEnemiesRangeBottom, this.rightEnemiesRangeTop);
							if (this.enemies.Any((Monster enemy) => SlasherCommon.IsInRange(generatedPosition, enemy.x - 4, enemy.x + 4)))
							{
								flag = true;
								num2++;
							}
						}
					}
					while (flag);
					result = generatedPosition;
				}
			}
			else
			{
				int generatedPosition;
				bool flag;
				do
				{
					flag = false;
					if (num2 >= 50)
					{
						this.leftEnemiesRangeBottom--;
						num2 = 0;
					}
					if (isHeavy)
					{
						generatedPosition = UnityEngine.Random.Range(-100, -30);
					}
					else
					{
						generatedPosition = UnityEngine.Random.Range(this.leftEnemiesRangeBottom, this.leftEnemiesRangeTop);
						if (this.enemies.Any((Monster enemy) => SlasherCommon.IsInRange(generatedPosition, enemy.x - 4, enemy.x + 4)))
						{
							flag = true;
							num2++;
						}
					}
				}
				while (flag);
				result = generatedPosition;
			}
			return result;
		}

		// Token: 0x0600271C RID: 10012 RVA: 0x00119248 File Offset: 0x00117648
		private void GenerateNumberOfEnemies()
		{
			int num = this.standardEnemiesInWave;
			this.standardEnemiesInWave = num + UnityEngine.Random.Range(4, 12);
			if (this.waveNumber == 3)
			{
				this.fastEnemiesInWave = 1;
			}
			else if (this.waveNumber > 3)
			{
				num = this.fastEnemiesInWave;
				this.fastEnemiesInWave = num + UnityEngine.Random.Range(2, 8);
			}
			if (this.waveNumber == 5)
			{
				this.heavyEnemiesInWave = 1;
			}
			else if (this.waveNumber > 5)
			{
				num = this.heavyEnemiesInWave;
				this.heavyEnemiesInWave = num + UnityEngine.Random.Range(1, 5);
			}
		}

		// Token: 0x0600271D RID: 10013 RVA: 0x001192E0 File Offset: 0x001176E0
		private void CheckLeftEnemies()
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (Monster monster in this.enemies)
			{
				if (monster.GetType() == typeof(MeleeZombie))
				{
					num++;
				}
				else if (monster.GetType() == typeof(FastZombie))
				{
					num2++;
				}
				else if (monster.GetType() == typeof(HeavyZombie))
				{
					num3++;
				}
			}
			this.ui.SetEnemiesNumber(num, num2, num3);
		}

		// Token: 0x04002949 RID: 10569
		private int previousHealth;

		// Token: 0x0400294A RID: 10570
		private Player player;

		// Token: 0x0400294B RID: 10571
		private List<Monster> enemies;

		// Token: 0x0400294C RID: 10572
		private SlasherUI ui;

		// Token: 0x0400294D RID: 10573
		private bool damageDealt;

		// Token: 0x0400294E RID: 10574
		private int waveNumber;

		// Token: 0x0400294F RID: 10575
		private int standardEnemiesInWave = 2;

		// Token: 0x04002950 RID: 10576
		private int fastEnemiesInWave;

		// Token: 0x04002951 RID: 10577
		private int heavyEnemiesInWave;

		// Token: 0x04002952 RID: 10578
		private int firstWaveEnemiesNumber = 2;

		// Token: 0x04002953 RID: 10579
		private int leftEnemiesRangeBottom = -20;

		// Token: 0x04002954 RID: 10580
		private int leftEnemiesRangeTop = -10;

		// Token: 0x04002955 RID: 10581
		private int rightEnemiesRangeBottom = 60;

		// Token: 0x04002956 RID: 10582
		private int rightEnemiesRangeTop = 70;

		// Token: 0x04002957 RID: 10583
		private APPSlasherGame.GameState currentGameState;

		// Token: 0x04002958 RID: 10584
		private float wavePrepareTimer;

		// Token: 0x04002959 RID: 10585
		private float wavePrepareTime = 1.5f;

		// Token: 0x0400295A RID: 10586
		private float waveFinishedTimer;

		// Token: 0x0400295B RID: 10587
		private float waveFinishedTime = 1f;

		// Token: 0x0400295C RID: 10588
		private AudioSource backgroundMusic;

		private string backgroundMusicInstance;

		// Token: 0x02000697 RID: 1687
		public enum GameState
		{
			// Token: 0x0400295E RID: 10590
			WaveNotStarted,
			// Token: 0x0400295F RID: 10591
			WavePreparing,
			// Token: 0x04002960 RID: 10592
			WaveStarted,
			// Token: 0x04002961 RID: 10593
			WaveFinished,
			// Token: 0x04002962 RID: 10594
			Lost
		}

		private AudioClip enemyKillClip = MCDAudio.LoadWavFromEmbeddedResource("AC_ZombieSlasher_EnemyKill.wav");

		private AudioClip lostClip = MCDAudio.LoadWavFromEmbeddedResource("AC_ZombieSlasher_Lose.wav");

		private AudioClip musicClip = MCDAudio.LoadWavFromEmbeddedResource("AC_ZombieSlasher_Music.wav");

		private AudioClip playerHitClip = MCDAudio.LoadWavFromEmbeddedResource("AC_ZombieSlasher_PlayerHit.wav");

		private AudioClip waveStartClip = MCDAudio.LoadWavFromEmbeddedResource("AC_ZombieSlasher_WaveStart.wav");

		private AudioClip winClip = MCDAudio.LoadWavFromEmbeddedResource("AC_ZombieSlasher_Win.wav");
	}

	public class APPSlasherIntro : SHSharp::SHGUIappbase
	{
		// Token: 0x0600271E RID: 10014 RVA: 0x001193E4 File Offset: 0x001177E4
		public APPSlasherIntro() : base("slash'em-all", true)
		{
			this.APPFRAME.Kill();
			this.APPINSTRUCTION.Kill();
			this.APPLABEL.Kill();
			this.InitializeLogo();
			this.InitializeInstructions();
			this.GenerateZombie();
			//Singleton<PlayerManager>.Instance.Account.SetRichPresence(SystemAccount.RichPresence.RICH_PRESENCE_MINIGAME);
		}

		// Token: 0x0600271F RID: 10015 RVA: 0x00119484 File Offset: 0x00117884
		public override void Update()
		{
			if (this.launchGameStarted || this.closeGameStarted)
			{
				return;
			}
			this.HandleLogoBlink();
			this.HandleZombieBehaviour();
			this.HandleInstructionRewrite();
			base.Update();
		}

		// Token: 0x06002720 RID: 10016 RVA: 0x001194B5 File Offset: 0x001178B5
		public override void Redraw(int offx, int offy)
		{
			base.Redraw(offx, offy);
			this.logo.Redraw(offx, offy);
			this.zombie.Redraw(offx, offy);
			this.instructions.Redraw(offx, offy);
		}

		// Token: 0x06002721 RID: 10017 RVA: 0x001194E8 File Offset: 0x001178E8
		public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
		{
			switch (key)
			{
				case SHSharp::SHGUIinput.enter:
					this.LaunchGame();
					this.launchGameStarted = true;
					break;
				case SHSharp::SHGUIinput.esc:
					SHSharp::SHGUI.current.PopView();
					this.OnExit();
					this.closeGameStarted = true;
					break;
			}
		}

		// Token: 0x06002722 RID: 10018 RVA: 0x0011957E File Offset: 0x0011797E
		public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
		{
			if (clicked && !this.launchGameStarted)
			{
				this.LaunchGame();
				this.launchGameStarted = true;
			}
		}

		// Token: 0x06002723 RID: 10019 RVA: 0x001195A0 File Offset: 0x001179A0
		private void InitializeInstructions()
		{
			this.instructions = new SHSharp::SHGUIview();
			string key = "enter"; //SHGame.Instance.ControlsSettings.GetKey(SHControlsSettings.SHGUIKeys.Confirm);
			this.startInstruction = string.Format("Ported by Garbear from MCD | Press Enter to Begin", key);
			this.instruction = new SHSharp::SHGUItext(this.startInstruction, 0, 0, 'r', false);
			SlasherCommon.SetCenterPosition(this.instruction.GetLineLength(), out this.instruction.x, out this.instruction.y);
			this.instruction.y = 20;
			this.instructions.AddSubView(this.instruction);
			base.AddSubView(this.instructions);
			this.instructionTextTimer = this.instructionTextTime;
		}

		// Token: 0x06002724 RID: 10020 RVA: 0x00119658 File Offset: 0x00117A58
		private void InitializeLogo()
		{
			this.logo = SlasherCommon.AddFrameFromStr(new SHSharp::SHGUIsprite(), SlasherCommon.AssetToText(SlasherCommon.GetAssetName("Logo.txt")), 17);
			this.logo.x = 10;
			this.logo.y = 1;
			this.logo.color = 'F';
			base.AddSubView(this.logo);
		}

		// Token: 0x06002725 RID: 10021 RVA: 0x001196B0 File Offset: 0x00117AB0
		private void GenerateZombie()
		{
			this.zombie = new FastZombie();
			int num = UnityEngine.Random.Range(0, 2);
			if (num != 0)
			{
				if (num == 1)
				{
					this.zombie.x = UnityEngine.Random.Range(70, 80);
				}
			}
			else
			{
				this.zombie.x = UnityEngine.Random.Range(-30, -20);
			}
			if (this.zombie.x < 0)
			{
				this.zombieTarget = 100;
			}
			else
			{
				this.zombieTarget = -60;
			}
			this.zombie.y = 18;
			this.zombie.StartDelayTime = 0f;
			base.AddSubView(this.zombie);
		}

		// Token: 0x06002726 RID: 10022 RVA: 0x00119769 File Offset: 0x00117B69
		private void LaunchGame()
		{
			SHSharp::AudioManager.Instance.FadeoutAndLeave(this.noiseInstance.gameObject, 1f, 0f);
			this.Kill();
			SHSharp::SHGUI.current.AddViewOnTop(new APPSlasherGame());
			//SHSharp::SHGUI.current.LaunchAppByName("StoryApps.Games.Slasher.APPSlasherGame");
		}

		// Token: 0x06002727 RID: 10023 RVA: 0x0011979C File Offset: 0x00117B9C
		private int FindElement(char[] array, char element)
		{
			for (int i = 0; i < array.Length; i++)
			{
				char c = array[i];
				if (c == element)
				{
					return i;
				}
			}
			return -1;
		}

		private AudioClip CreateAudioClipFromData(float[] audioData, int sampleRate, int channels)
		{
			AudioClip clip = AudioClip.Create("CustomClip", audioData.Length / channels, channels, sampleRate, false);
			clip.SetData(audioData, 0);
			return clip;
		}

		// Token: 0x06002728 RID: 10024 RVA: 0x001197CC File Offset: 0x00117BCC
		private void HandleLogoBlink()
		{
			if (this.logoTimer >= this.logoBlinkTime)
			{
				if (!this.blinkStarted)
				{

					this.noiseInstance = SHSharp::AudioManager.Instance.PlayClip(this.logoNoise, null, SlasherCommon.VOL);
					this.blinkStarted = true;
				}
				char color = this.logo.color;
				int num = UnityEngine.Random.Range(1, 4);
				this.blinkTimer++;
				if (this.blinkTimer < num)
				{
					return;
				}
				if (!this.blinkReturn)
				{
					if (color != this.colors.First<char>())
					{
						char color2 = this.colors[this.FindElement(this.colors, color) - 1];
						this.logo.color = color2;
					}
					else if (color == this.colors[0])
					{
						this.blinkReturn = true;
					}
					this.blinkTimer = 0;
				}
				else
				{
					if (color != this.colors[this.colors.Length - 1])
					{
						char color3 = this.colors[this.FindElement(this.colors, color) + 1];
						this.logo.color = color3;
					}
					else
					{
						this.blinkReturn = false;
						this.logoTimer = 0f;
						this.logoBlinkTime = UnityEngine.Random.Range(0.5f, 3f);
						this.blinkStarted = false;
						SHSharp::AudioManager.Instance.FadeoutAndLeave(this.noiseInstance.gameObject, 1f, 0f);
					}
					this.blinkTimer = 0;
				}
			}
			this.logoTimer += Time.unscaledDeltaTime;
		}

		// Token: 0x06002729 RID: 10025 RVA: 0x0011995C File Offset: 0x00117D5C
		private void HandleZombieBehaviour()
		{
			if (!this.zombie.Attacking)
			{
				this.zombie.GoToTarget(this.zombieTarget);
			}
			else
			{
				this.zombie.Kill();
				base.RemoveView(this.zombie);
				this.GenerateZombie();
			}
		}

		// Token: 0x0600272A RID: 10026 RVA: 0x001199AC File Offset: 0x00117DAC
		private void HandleInstructionRewrite()
		{
			if (this.instructionTextTimer >= this.instructionTextTime)
			{
				if (!this.rewriteStarted)
				{
					this.instruction.text = this.writeSymbol.ToString();
					this.rewriteStarted = true;
				}
				if (this.instructionRewriteTimer >= this.instructionRewriteTime)
				{
					if (this.startInstruction.Length + 1 == this.instruction.text.Length)
					{
						this.rewriteStarted = false;
						this.instructionRewriteTimer = 0f;
						this.instructionTextTimer = 0f;
						this.instruction.text = this.instruction.text.TrimEnd(new char[]
						{
							this.writeSymbol
						});
					}
					else
					{
						this.instruction.text = this.instruction.text.TrimEnd(new char[]
						{
							this.writeSymbol
						});
						SHSharp::SHGUItext shguitext = this.instruction;
						shguitext.text += this.startInstruction.ElementAt(this.instruction.text.Length);
						SHSharp::AudioManager.Instance.PlayClip(this.typeTextClip, null, SlasherCommon.VOL);//.RandomizeAudioParameters(0.2f, 0f, true, false);
						SHSharp::SHGUItext shguitext2 = this.instruction;
						shguitext2.text += this.writeSymbol;
					}
					this.instructionRewriteTimer = 0f;
				}
			}
			this.instructionRewriteTimer += Time.unscaledDeltaTime;
			this.instructionTextTimer += Time.unscaledDeltaTime;
		}

		// Token: 0x04002963 RID: 10595
		private SHSharp::SHGUIview instructions;

		// Token: 0x04002964 RID: 10596
		private SHSharp::SHGUItext instruction;

		// Token: 0x04002965 RID: 10597
		private AudioSource noiseInstance;

		// Token: 0x04002966 RID: 10598
		private SHSharp::SHGUIsprite logo;

		// Token: 0x04002967 RID: 10599
		private Monster zombie;

		// Token: 0x04002968 RID: 10600
		private int zombieTarget;

		// Token: 0x04002969 RID: 10601
		private float logoTimer;

		// Token: 0x0400296A RID: 10602
		private float logoBlinkTime = 2f;

		// Token: 0x0400296B RID: 10603
		private bool blinkReturn;

		// Token: 0x0400296C RID: 10604
		private bool blinkStarted;

		// Token: 0x0400296D RID: 10605
		private int blinkTimer;

		// Token: 0x0400296E RID: 10606
		private float instructionTextTimer;

		// Token: 0x0400296F RID: 10607
		private float instructionTextTime = 3f;

		// Token: 0x04002970 RID: 10608
		private float instructionRewriteTimer;

		// Token: 0x04002971 RID: 10609
		private float instructionRewriteTime = 0.04f;

		// Token: 0x04002972 RID: 10610
		private bool rewriteStarted;

		// Token: 0x04002973 RID: 10611
		private bool launchGameStarted;

		// Token: 0x04002974 RID: 10612
		private bool closeGameStarted;

		// Token: 0x04002975 RID: 10613
		private char[] colors = new char[]
		{
			'0',
			'1',
			'2',
			'3',
			'4',
			'5',
			'6',
			'7',
			'8',
			'9',
			'A',
			'B',
			'C',
			'D',
			'E',
			'F'
		};

		// Token: 0x04002976 RID: 10614
		private string startInstruction;

		// Token: 0x04002977 RID: 10615
		private char writeSymbol = '▓';

		private AudioClip logoNoise = MCDAudio.LoadWavFromEmbeddedResource("AC_ZombieSlasher_LogoNoise.wav");

		private AudioClip typeTextClip = MCDAudio.LoadWavFromEmbeddedResource("AC_ZombieSlasher_TextType.wav");
	}

	public class FastZombie : Monster
	{
		// Token: 0x0600272B RID: 10027 RVA: 0x00119ED4 File Offset: 0x001182D4
		public FastZombie()
		{
			base.Health = 20;
			base.Speed = 2f;
			base.SpeedMax = 5f;
			base.AttackRange = 0;
			base.Damage = 10;
			base.AttackCooldown = 1f;
			base.CurrentHealth = base.Health;
			base.FrameLength = 25;
			base.AddFrameFromStr(SlasherCommon.AssetToText(SlasherCommon.GetAssetName("FastZombie.txt")), 5);
			this.color = 'C';
		}
	}

	public class HeavyZombie : Monster
	{
		// Token: 0x0600272C RID: 10028 RVA: 0x00119F4C File Offset: 0x0011834C
		public HeavyZombie()
		{
			base.Health = 70;
			base.Speed = 0.5f;
			base.SpeedMax = 0.8f;
			base.AttackRange = 2;
			base.Damage = 25;
			base.AttackCooldown = 3f;
			base.CurrentHealth = base.Health;
			base.FrameLength = 21;
			base.AddFrameFromStr(SlasherCommon.AssetToText(SlasherCommon.GetAssetName("HeavyZombie.txt")), 14);
			this.color = 'E';
		}

		// Token: 0x0600272D RID: 10029 RVA: 0x00119FC4 File Offset: 0x001183C4
		protected override void SetCurrentFrames()
		{
			float num = (float)base.Health / 3f;
			Monster.Direction currentDirection = base.CurrentDirection;
			if (currentDirection != Monster.Direction.Left)
			{
				if (currentDirection != Monster.Direction.Right)
				{
					throw new ArgumentOutOfRangeException();
				}
				if ((float)base.CurrentHealth < num)
				{
					this.WalkFrame1 = 15;
					this.WalkFrame2 = 16;
					this.AttackFrame = 17;
				}
				else if ((float)base.CurrentHealth < num * 2f)
				{
					this.WalkFrame1 = 12;
					this.WalkFrame2 = 13;
					this.AttackFrame = 14;
				}
				else
				{
					this.WalkFrame1 = 9;
					this.WalkFrame2 = 10;
					this.AttackFrame = 11;
				}
			}
			else if ((float)base.CurrentHealth < num)
			{
				this.WalkFrame1 = 6;
				this.WalkFrame2 = 7;
				this.AttackFrame = 8;
			}
			else if ((float)base.CurrentHealth < num * 2f)
			{
				this.WalkFrame1 = 3;
				this.WalkFrame2 = 4;
				this.AttackFrame = 5;
			}
			else
			{
				this.WalkFrame1 = 0;
				this.WalkFrame2 = 1;
				this.AttackFrame = 2;
			}
		}

		// Token: 0x0200069B RID: 1691
		private new enum MonsterAnimations
		{
			// Token: 0x04002979 RID: 10617
			WalkLeft01,
			// Token: 0x0400297A RID: 10618
			WalkLeft02,
			// Token: 0x0400297B RID: 10619
			AttackLeft,
			// Token: 0x0400297C RID: 10620
			WalkLeft1Hit01,
			// Token: 0x0400297D RID: 10621
			WalkLeft1Hit02,
			// Token: 0x0400297E RID: 10622
			AttackLeft1Hit,
			// Token: 0x0400297F RID: 10623
			WalkLeft2Hit01,
			// Token: 0x04002980 RID: 10624
			WalkLeft2Hit02,
			// Token: 0x04002981 RID: 10625
			AttackLeft2Hit,
			// Token: 0x04002982 RID: 10626
			WalkRight01,
			// Token: 0x04002983 RID: 10627
			WalkRight02,
			// Token: 0x04002984 RID: 10628
			AttackRight,
			// Token: 0x04002985 RID: 10629
			WalkRight1Hit01,
			// Token: 0x04002986 RID: 10630
			WalkRight1Hit02,
			// Token: 0x04002987 RID: 10631
			AttackRight1Hit,
			// Token: 0x04002988 RID: 10632
			WalkRight2Hit01,
			// Token: 0x04002989 RID: 10633
			WalkRight2Hit02,
			// Token: 0x0400298A RID: 10634
			AttackRight2Hit
		}
	}

	public class MeleeZombie : Monster
	{
		// Token: 0x0600272E RID: 10030 RVA: 0x0011A0E4 File Offset: 0x001184E4
		public MeleeZombie()
		{
			base.Health = 30;
			base.Speed = 0.7f;
			base.SpeedMax = 1.1f;
			base.AttackRange = 0;
			base.Damage = 30;
			base.AttackCooldown = 2f;
			base.CurrentHealth = base.Health;
			base.FrameLength = 14;
			base.AddFrameFromStr(SlasherCommon.AssetToText(SlasherCommon.GetAssetName("Monster.txt")), 10);
			this.color = 'A';
		}
	}

	public abstract class Monster : SHSharp::SHGUIsprite
	{
		// Token: 0x0600272F RID: 10031 RVA: 0x00119B56 File Offset: 0x00117F56
		protected Monster()
		{
			this.movementTimer = 0f;
		}

		// Token: 0x1700020B RID: 523
		// (get) Token: 0x06002730 RID: 10032 RVA: 0x00119B69 File Offset: 0x00117F69
		// (set) Token: 0x06002731 RID: 10033 RVA: 0x00119B71 File Offset: 0x00117F71
		public Player Target { get; set; }

		// Token: 0x1700020C RID: 524
		// (get) Token: 0x06002732 RID: 10034 RVA: 0x00119B7A File Offset: 0x00117F7A
		// (set) Token: 0x06002733 RID: 10035 RVA: 0x00119B82 File Offset: 0x00117F82
		public int FrameLength { get; set; }

		// Token: 0x1700020D RID: 525
		// (get) Token: 0x06002734 RID: 10036 RVA: 0x00119B8B File Offset: 0x00117F8B
		// (set) Token: 0x06002735 RID: 10037 RVA: 0x00119B93 File Offset: 0x00117F93
		public int Health { get; set; }

		// Token: 0x1700020E RID: 526
		// (get) Token: 0x06002736 RID: 10038 RVA: 0x00119B9C File Offset: 0x00117F9C
		// (set) Token: 0x06002737 RID: 10039 RVA: 0x00119BA4 File Offset: 0x00117FA4
		public int CurrentHealth { get; set; }

		// Token: 0x1700020F RID: 527
		// (get) Token: 0x06002738 RID: 10040 RVA: 0x00119BAD File Offset: 0x00117FAD
		// (set) Token: 0x06002739 RID: 10041 RVA: 0x00119BB5 File Offset: 0x00117FB5
		public float Speed { get; set; }

		// Token: 0x17000210 RID: 528
		// (get) Token: 0x0600273A RID: 10042 RVA: 0x00119BBE File Offset: 0x00117FBE
		// (set) Token: 0x0600273B RID: 10043 RVA: 0x00119BC6 File Offset: 0x00117FC6
		public float SpeedMax { get; set; }

		// Token: 0x17000211 RID: 529
		// (get) Token: 0x0600273C RID: 10044 RVA: 0x00119BCF File Offset: 0x00117FCF
		// (set) Token: 0x0600273D RID: 10045 RVA: 0x00119BD7 File Offset: 0x00117FD7
		public int AttackRange { get; set; }

		// Token: 0x17000212 RID: 530
		// (get) Token: 0x0600273E RID: 10046 RVA: 0x00119BE0 File Offset: 0x00117FE0
		// (set) Token: 0x0600273F RID: 10047 RVA: 0x00119BE8 File Offset: 0x00117FE8
		public float AttackCooldown { get; set; }

		// Token: 0x17000213 RID: 531
		// (get) Token: 0x06002740 RID: 10048 RVA: 0x00119BF1 File Offset: 0x00117FF1
		// (set) Token: 0x06002741 RID: 10049 RVA: 0x00119BF9 File Offset: 0x00117FF9
		public int Damage { get; set; }

		// Token: 0x17000214 RID: 532
		// (get) Token: 0x06002742 RID: 10050 RVA: 0x00119C02 File Offset: 0x00118002
		// (set) Token: 0x06002743 RID: 10051 RVA: 0x00119C0A File Offset: 0x0011800A
		public Monster.Direction CurrentDirection { get; private set; }

		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06002744 RID: 10052 RVA: 0x00119C13 File Offset: 0x00118013
		// (set) Token: 0x06002745 RID: 10053 RVA: 0x00119C1B File Offset: 0x0011801B
		public bool Attacking { get; private set; }

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06002746 RID: 10054 RVA: 0x00119C24 File Offset: 0x00118024
		// (set) Token: 0x06002747 RID: 10055 RVA: 0x00119C2C File Offset: 0x0011802C
		public float StartDelayTime { get; set; }

		// Token: 0x06002748 RID: 10056 RVA: 0x00119C38 File Offset: 0x00118038
		public override void Update()
		{
			this.movementTimer += Time.unscaledDeltaTime * this.Speed;
			this.attackResetTimer += Time.unscaledDeltaTime;
			this.startDelayTimer += Time.unscaledDeltaTime;
			base.Update();
		}

		// Token: 0x06002749 RID: 10057 RVA: 0x00119C88 File Offset: 0x00118088
		public virtual void Attack()
		{
			this.SetCurrentFrames();
			this.currentFrame = this.AttackFrame;
			if (this.Target != null)
			{
				this.Target.ReceiveDamage(this.Damage);
			}
		}

		// Token: 0x0600274A RID: 10058 RVA: 0x00119CB8 File Offset: 0x001180B8
		public virtual void ReceiveDamage(int amount)
		{
			this.CurrentHealth -= amount;
			if (this.CurrentHealth <= 0)
			{
				this.CurrentHealth = 0;
			}
		}

		// Token: 0x0600274B RID: 10059 RVA: 0x00119CDB File Offset: 0x001180DB
		public virtual void Die()
		{
			this.Kill();
		}

		public virtual void AddFrameFromStr(string content, int rowsPerFrame)
		{
			string[] array = content.Split('\n');
			int num = 0;
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text = text + array[i] + "\n";
				num++;
				if (num > rowsPerFrame - 1)
				{
					num = 0;
					this.AddFrame(text);
					text = "";
				}
			}
		}

		// Token: 0x0600274C RID: 10060 RVA: 0x00119CE4 File Offset: 0x001180E4
		public virtual void GoToTarget(int targetPositionX)
		{
			if (this.startDelayTimer < this.StartDelayTime)
			{
				return;
			}
			if (this.movementTimer >= 0.2f)
			{
				if (this.x + this.FrameLength + this.AttackRange < targetPositionX + Player.FrameLength)
				{
					this.x++;
					this.CurrentDirection = Monster.Direction.Left;
					this.attackResetTimer = this.AttackCooldown;
					this.PlayWalkSequence();
					this.Attacking = false;
				}
				else if (this.x - this.AttackRange > targetPositionX + Player.FrameLength)
				{
					this.x--;
					this.CurrentDirection = Monster.Direction.Right;
					this.attackResetTimer = this.AttackCooldown;
					this.PlayWalkSequence();
					this.Attacking = false;
				}
				else if (this.attackResetTimer > this.AttackCooldown)
				{
					this.Attack();
					this.Attacking = true;
					this.attackResetTimer = 0f;
				}
				else if (this.attackResetTimer > 0.4f)
				{
					this.currentFrame = this.WalkFrame1;
				}
				this.movementTimer = 0f;
			}
		}

		// Token: 0x0600274D RID: 10061 RVA: 0x00119E08 File Offset: 0x00118208
		protected virtual void SetCurrentFrames()
		{
			Monster.Direction currentDirection = this.CurrentDirection;
			if (currentDirection != Monster.Direction.Left)
			{
				if (currentDirection != Monster.Direction.Right)
				{
					throw new ArgumentOutOfRangeException();
				}
				this.WalkFrame1 = 3;
				this.WalkFrame2 = 4;
				this.AttackFrame = 5;
			}
			else
			{
				this.WalkFrame1 = 0;
				this.WalkFrame2 = 1;
				this.AttackFrame = 2;
			}
		}

		// Token: 0x0600274E RID: 10062 RVA: 0x00119E68 File Offset: 0x00118268
		private void PlayWalkSequence()
		{
			this.SetCurrentFrames();
			if (this.currentFrame != this.WalkFrame1 && this.currentFrame != this.WalkFrame2)
			{
				this.currentFrame = this.WalkFrame1;
				return;
			}
			this.currentFrame = ((this.currentFrame != this.WalkFrame1) ? this.WalkFrame1 : this.WalkFrame2);
		}

		// Token: 0x04002997 RID: 10647
		protected int WalkFrame1;

		// Token: 0x04002998 RID: 10648
		protected int WalkFrame2;

		// Token: 0x04002999 RID: 10649
		protected int AttackFrame;

		// Token: 0x0400299A RID: 10650
		private float movementTimer;

		// Token: 0x0400299B RID: 10651
		private const float MovementTime = 0.2f;

		// Token: 0x0400299C RID: 10652
		private float attackResetTimer;

		// Token: 0x0400299D RID: 10653
		private const float AttackAnimationTime = 0.4f;

		// Token: 0x0400299E RID: 10654
		private float startDelayTimer;

		// Token: 0x0200069E RID: 1694
		public enum Direction
		{
			// Token: 0x040029A0 RID: 10656
			Left,
			// Token: 0x040029A1 RID: 10657
			Right
		}

		// Token: 0x0200069F RID: 1695
		protected enum MonsterAnimations
		{
			// Token: 0x040029A3 RID: 10659
			WalkLeft01,
			// Token: 0x040029A4 RID: 10660
			WalkLeft02,
			// Token: 0x040029A5 RID: 10661
			AttackLeft,
			// Token: 0x040029A6 RID: 10662
			WalkRight01,
			// Token: 0x040029A7 RID: 10663
			WalkRight02,
			// Token: 0x040029A8 RID: 10664
			AttackRight
		}
	}

	public class Player : SHSharp::SHGUIsprite
	{
		// Token: 0x0600274F RID: 10063 RVA: 0x0011A15A File Offset: 0x0011855A
		public Player()
		{
			SlasherCommon.AddFrameFromStr(this, SlasherCommon.AssetToText(SlasherCommon.GetAssetName("Knight.txt")), 10);
			this.CurrentState = Player.PlayerState.Idle;
			this.attackResetTimer = 0.2f;
			this.CurrentHealth = Player.Health;
		}

		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06002750 RID: 10064 RVA: 0x0011A195 File Offset: 0x00118595
		// (set) Token: 0x06002751 RID: 10065 RVA: 0x0011A19D File Offset: 0x0011859D
		public Player.Direction CurrentDirection { get; set; }

		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06002752 RID: 10066 RVA: 0x0011A1A6 File Offset: 0x001185A6
		// (set) Token: 0x06002753 RID: 10067 RVA: 0x0011A1AE File Offset: 0x001185AE
		public Player.PlayerState CurrentState { get; set; }

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06002754 RID: 10068 RVA: 0x0011A1B7 File Offset: 0x001185B7
		// (set) Token: 0x06002755 RID: 10069 RVA: 0x0011A1BF File Offset: 0x001185BF
		public int CurrentHealth { get; set; }

		// Token: 0x06002756 RID: 10070 RVA: 0x0011A1C8 File Offset: 0x001185C8
		public override void Update()
		{
			this.animationTimer += Time.unscaledDeltaTime;
			this.attackResetTimer += Time.unscaledDeltaTime;
			this.HackNoneInput();
			this.ResetAttackToIdle();
			base.Update();
		}

		// Token: 0x06002757 RID: 10071 RVA: 0x0011A200 File Offset: 0x00118600
		public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
		{
			switch (key)
			{
				case SHSharp::SHGUIinput.none:
					if (this.CurrentState == Player.PlayerState.Idle)
					{
						return;
					}
					this.idleRequested = true;
					return;
				default:
					// TODO: Come back to this
					if (key != SHSharp::SHGUIinput.up)
					{
						return;
					}
					break;
				case SHSharp::SHGUIinput.left:
					if (this.x > 0 && this.CurrentState != Player.PlayerState.Attacking)
					{
						this.PlayWalkSequence(Player.Direction.Left);
						this.idleRequested = false;
					}
					return;
				case SHSharp::SHGUIinput.right:
					if (this.x < 30 && this.CurrentState != Player.PlayerState.Attacking)
					{
						this.PlayWalkSequence(Player.Direction.Right);
						this.idleRequested = false;
					}
					return;
				case SHSharp::SHGUIinput.enter:
					break;
			}
			this.PlayAttackSequence();
		}

		// Token: 0x06002758 RID: 10072 RVA: 0x0011A2B2 File Offset: 0x001186B2
		public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
		{
			if (clicked)
			{
				this.PlayAttackSequence();
			}
		}

		// Token: 0x06002759 RID: 10073 RVA: 0x0011A2C0 File Offset: 0x001186C0
		public void ReceiveDamage(int damage)
		{
			this.CurrentHealth -= damage;
			if (this.CurrentHealth <= 0)
			{
				this.CurrentHealth = 0;
			}
		}

		// Token: 0x0600275A RID: 10074 RVA: 0x0011A2E4 File Offset: 0x001186E4
		private void PlayWalkSequence(Player.Direction walkDirection)
		{
			int num;
			if (walkDirection != Player.Direction.Left)
			{
				if (walkDirection != Player.Direction.Right)
				{
					throw new ArgumentOutOfRangeException("walkDirection", walkDirection, null);
				}
				this.idleFrame = 0;
				this.walkFrame = 1;
				this.CurrentDirection = Player.Direction.Right;
				num = 1;
			}
			else
			{
				this.idleFrame = 3;
				this.walkFrame = 4;
				this.CurrentDirection = Player.Direction.Left;
				num = -1;
			}
			if (this.currentFrame != this.idleFrame && this.currentFrame != this.walkFrame)
			{
				this.currentFrame = this.idleFrame;
				return;
			}
			if (this.animationTimer < 0.08f)
			{
				return;
			}
			this.CurrentState = Player.PlayerState.Walking;
			this.x += num;
			this.currentFrame = ((this.currentFrame != this.idleFrame) ? this.idleFrame : this.walkFrame);
			this.animationTimer = 0f;
		}

		// Token: 0x0600275B RID: 10075 RVA: 0x0011A3D8 File Offset: 0x001187D8
		private void PlayAttackSequence()
		{
			if (this.CurrentState == Player.PlayerState.Attacking)
			{
				return;
			}
			if (this.attackResetTimer < 0.2f)
			{
				return;
			}
			int currentFrame;
			if (this.currentFrame == 3 || this.currentFrame == 4)
			{
				currentFrame = 5;
				this.CurrentDirection = Player.Direction.Left;
			}
			else
			{
				currentFrame = 2;
				this.CurrentDirection = Player.Direction.Right;
			}
			this.CurrentState = Player.PlayerState.Attacking;
			this.currentFrame = currentFrame;
			this.animationTimer = 0f;
		}

		// Token: 0x0600275C RID: 10076 RVA: 0x0011A44C File Offset: 0x0011884C
		private void SwitchToIdle()
		{
			if (this.currentFrame == 5 || this.currentFrame == 4 || this.currentFrame == 3)
			{
				this.currentFrame = 3;
				this.CurrentDirection = Player.Direction.Left;
			}
			else
			{
				this.currentFrame = 0;
				this.CurrentDirection = Player.Direction.Right;
			}
			this.CurrentState = Player.PlayerState.Idle;
		}

		// Token: 0x0600275D RID: 10077 RVA: 0x0011A4A5 File Offset: 0x001188A5
		private void ResetAttackToIdle()
		{
			if (this.CurrentState != Player.PlayerState.Attacking)
			{
				return;
			}
			if (this.animationTimer < 0.08f)
			{
				return;
			}
			this.SwitchToIdle();
			this.animationTimer = 0f;
			this.attackResetTimer = 0f;
		}

		// Token: 0x0600275E RID: 10078 RVA: 0x0011A4E4 File Offset: 0x001188E4
		private void HackNoneInput()
		{
			if (this.idleRequested)
			{
				if (this.idleResetTimer >= 0.3f)
				{
					this.SwitchToIdle();
					this.idleResetTimer = 0f;
					this.idleRequested = false;
				}
				else
				{
					this.idleResetTimer += Time.unscaledDeltaTime;
				}
			}
			if (this.CurrentState == Player.PlayerState.Walking && !this.idleRequested)
			{
				this.idleResetTimer = 0f;
			}
			if (this.CurrentState == Player.PlayerState.Attacking)
			{
				this.idleResetTimer = 0f;
				this.idleRequested = false;
			}
		}

		// Token: 0x040029A9 RID: 10665
		public static int AttackRange = 13;

		// Token: 0x040029AA RID: 10666
		public static int Health = 100;

		// Token: 0x040029AB RID: 10667
		public static int Damage = 30;

		// Token: 0x040029AC RID: 10668
		public static int FrameLength = 17;

		// Token: 0x040029B0 RID: 10672
		private float animationTimer;

		// Token: 0x040029B1 RID: 10673
		private const float AnimationTime = 0.08f;

		// Token: 0x040029B2 RID: 10674
		private float idleResetTimer;

		// Token: 0x040029B3 RID: 10675
		private const float IdleResetTime = 0.3f;

		// Token: 0x040029B4 RID: 10676
		private bool idleRequested;

		// Token: 0x040029B5 RID: 10677
		private float attackResetTimer;

		// Token: 0x040029B6 RID: 10678
		private const float AttackResetTime = 0.2f;

		// Token: 0x040029B7 RID: 10679
		private int idleFrame;

		// Token: 0x040029B8 RID: 10680
		private int walkFrame = 1;

		// Token: 0x020006A1 RID: 1697
		public enum PlayerState
		{
			// Token: 0x040029BA RID: 10682
			Idle,
			// Token: 0x040029BB RID: 10683
			Walking,
			// Token: 0x040029BC RID: 10684
			Attacking
		}

		// Token: 0x020006A2 RID: 1698
		public enum Direction
		{
			// Token: 0x040029BE RID: 10686
			Left,
			// Token: 0x040029BF RID: 10687
			Right
		}

		// Token: 0x020006A3 RID: 1699
		private enum KnightAnimations
		{
			// Token: 0x040029C1 RID: 10689
			IdleRight,
			// Token: 0x040029C2 RID: 10690
			WalkRight,
			// Token: 0x040029C3 RID: 10691
			AttackRight,
			// Token: 0x040029C4 RID: 10692
			IdleLeft,
			// Token: 0x040029C5 RID: 10693
			WalkLeft,
			// Token: 0x040029C6 RID: 10694
			AttackLeft
		}
	}

	public static class SlasherCommon
	{
		// Token: 0x06002760 RID: 10080 RVA: 0x0011A599 File Offset: 0x00118999
		public static void SetCenterPosition(int componentLength, out int positionX, out int positionY)
		{
			positionX = (SHSharp::SHGUI.current.resolutionX - componentLength - 1) / 2;
			positionY = (SHSharp::SHGUI.current.resolutionY - 1) / 2;
		}

		// Token: 0x06002761 RID: 10081 RVA: 0x0011A5BD File Offset: 0x001189BD
		public static bool IsInRange(int numberToCheck, int bottom, int top)
		{
			return numberToCheck >= bottom && numberToCheck <= top;
		}

		public static string GetAssetName(string name)
		{
			var assembly = Assembly.GetExecutingAssembly();
			string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(name));

			return resourceName;
		}

		public static string AssetToText(string name)
        {
			var assembly = Assembly.GetExecutingAssembly();

			using (Stream stream = assembly.GetManifestResourceStream(name))
			using (StreamReader reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}

		public static SHSharp::SHGUIsprite AddFrameFromStr(SHSharp::SHGUIsprite s, string content, int rowsPerFrame)
		{
			string[] array = content.Split('\n');
			int num = 0;
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text = text + array[i] + "\n";
				num++;
				if (num > rowsPerFrame - 1)
				{
					num = 0;
					s.AddFrame(text);
					text = "";
				}
			}

			return s;
		}

		public static SHSharp::SHGUIsprite AddSpecyficFrameFromStr(SHSharp::SHGUIsprite s, string content, int rowsPerFrame, int addFrame, string sound = null)
		{
			string[] array = content.Split('\n');
			int num = 0;
			string text = "";
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				text = text + array[i] + "\n";
				num++;
				if (num > rowsPerFrame - 1)
				{
					num = 0;
					if (addFrame == num2)
					{
						s.AddFrame(text, sound);
					}

					text = "";
					num2++;
				}
			}

			return s;
		}

		public static float VOL = 0.7f;
	}

	public class SlasherUI : SHSharp::SHGUIview
	{
		// Token: 0x06002762 RID: 10082 RVA: 0x0011A5D0 File Offset: 0x001189D0
		public SlasherUI()
		{
			this.playerHealthBar = new SHSharp::SHGUIprogressbar(40, 2, 12, "Health", string.Empty)
			{
				currentProgress = 1f,
				style = "r█w░r█",
				color = 'r',
				subtitleView =
				{
					x = 6
				}
			};
			base.AddSubView(this.playerHealthBar);
			this.wavePrepare = new SHSharp::SHGUItext("WAVE STARTED!", 0, 0, 'r', false);
			SlasherCommon.SetCenterPosition(this.wavePrepare.GetLineLength(), out this.wavePrepare.x, out this.wavePrepare.y);
			this.waveStartIndicator = new SHSharp::SHGUILoadingIndicator(31, 12, 0.1f, 'r');
			string key = "r"; //SHGame.Instance.ControlsSettings.GetKey(SHControlsSettings.SHGUIKeys.Confirm);
			this.restartInstruction = string.Format("Press Enter to Restart", key);
			this.restart = new SHSharp::SHGUItext(this.restartInstruction, 0, 0, 'r', false);
			SlasherCommon.SetCenterPosition(this.restart.GetLineLength(), out this.restart.x, out this.restart.y);
			this.restart.y++;
			this.standardZombieIcon = SlasherCommon.AddSpecyficFrameFromStr(new SHSharp::SHGUIsprite(), SlasherCommon.AssetToText(SlasherCommon.GetAssetName("Icons.txt")), 4, 0);
			this.fastZombieIcon = SlasherCommon.AddSpecyficFrameFromStr(new SHSharp::SHGUIsprite(), SlasherCommon.AssetToText(SlasherCommon.GetAssetName("Icons.txt")), 4, 1);
			this.heavyZombieIcon = SlasherCommon.AddSpecyficFrameFromStr(new SHSharp::SHGUIsprite(), SlasherCommon.AssetToText(SlasherCommon.GetAssetName("Icons.txt")), 4, 2);
			this.standardZombieIcon.x = 5;
			this.fastZombieIcon.x = 15;
			this.heavyZombieIcon.x = 25;
			this.standardZombieIcon.y = 1;
			this.fastZombieIcon.y = 1;
			this.heavyZombieIcon.y = 1;
			this.standardZombieIcon.color = 'D';
			this.fastZombieIcon.color = 'D';
			this.heavyZombieIcon.color = 'D';
			base.AddSubView(this.standardZombieIcon);
			base.AddSubView(this.fastZombieIcon);
			base.AddSubView(this.heavyZombieIcon);
			this.standardZombiesLeft = new SHSharp::SHGUItext("2000", 6, 5, 'r', false);
			this.fastZombiesLeft = new SHSharp::SHGUItext("2000", 16, 5, 'r', false);
			this.heavyZombiesLeft = new SHSharp::SHGUItext("2000", 26, 5, 'r', false);
			base.AddSubView(this.standardZombiesLeft);
			base.AddSubView(this.fastZombiesLeft);
			base.AddSubView(this.heavyZombiesLeft);
		}

		// Token: 0x06002763 RID: 10083 RVA: 0x0011A885 File Offset: 0x00118C85
		public override void Redraw(int offx, int offy)
		{
			base.Redraw(offx, offy);
			if (this.restartTextEnabled)
			{
				this.HandleInstructionRewrite();
			}
		}

		// Token: 0x06002764 RID: 10084 RVA: 0x0011A8A0 File Offset: 0x00118CA0
		public void SetHealthBarProgress(float progress)
		{
			this.playerHealthBar.currentProgress = progress;
		}

		// Token: 0x06002765 RID: 10085 RVA: 0x0011A8B0 File Offset: 0x00118CB0
		public void SetHealthBarProgress(int health)
		{
			this.playerHealthBar.currentProgress = (float)health / 100f;
			SHSharp::SHGUItext shguitext = this.playerHealthBar.subtitleView as SHSharp::SHGUItext;
			if (shguitext != null)
			{
				shguitext.text = health.ToString();
			}
		}

		// Token: 0x06002766 RID: 10086 RVA: 0x0011A8FC File Offset: 0x00118CFC
		public void ShowWavePrepareText()
		{
			SlasherCommon.SetCenterPosition(this.wavePrepare.GetLineLength(), out this.wavePrepare.x, out this.wavePrepare.y);
			base.AddSubView(this.wavePrepare);
			base.AddSubView(this.waveStartIndicator);
		}

		// Token: 0x06002767 RID: 10087 RVA: 0x0011A949 File Offset: 0x00118D49
		public void HideWavePrepareText()
		{
			base.RemoveView(this.wavePrepare);
			base.RemoveView(this.waveStartIndicator);
		}

		// Token: 0x06002768 RID: 10088 RVA: 0x0011A964 File Offset: 0x00118D64
		public void ShowWaveClearedText()
		{
			this.wavePrepare.text = "WAVE CLEARED!";
			SlasherCommon.SetCenterPosition(this.wavePrepare.GetLineLength(), out this.wavePrepare.x, out this.wavePrepare.y);
			base.AddSubView(this.wavePrepare);
		}

		// Token: 0x06002769 RID: 10089 RVA: 0x0011A9B4 File Offset: 0x00118DB4
		public void HideWaveClearedText()
		{
			base.RemoveView(this.wavePrepare);
		}

		// Token: 0x0600276A RID: 10090 RVA: 0x0011A9C4 File Offset: 0x00118DC4
		public void ShowLostText()
		{
			this.wavePrepare.text = "YOU LOST!";
			SlasherCommon.SetCenterPosition(this.wavePrepare.GetLineLength(), out this.wavePrepare.x, out this.wavePrepare.y);
			base.AddSubView(this.wavePrepare);
			base.AddSubView(this.restart);
			this.restartTextEnabled = true;
		}

		// Token: 0x0600276B RID: 10091 RVA: 0x0011AA28 File Offset: 0x00118E28
		public void HideLostText()
		{
			base.RemoveView(this.wavePrepare);
			base.RemoveView(this.restart);
			this.restartTextEnabled = false;
		}

		// Token: 0x0600276C RID: 10092 RVA: 0x0011AA4C File Offset: 0x00118E4C
		public void SetEnemiesNumber(int standardEnemies, int fastEnemies, int heavyEnemies)
		{
			this.standardZombiesLeft.text = standardEnemies.ToString();
			this.fastZombiesLeft.text = fastEnemies.ToString();
			this.heavyZombiesLeft.text = heavyEnemies.ToString();
		}

		// Token: 0x0600276D RID: 10093 RVA: 0x0011AAA1 File Offset: 0x00118EA1
		public void SetWaveText(int waveNumber)
		{
			this.wavePrepare.text = string.Format("WAVE {0}!", waveNumber);
		}

		// Token: 0x0600276E RID: 10094 RVA: 0x0011AAC0 File Offset: 0x00118EC0
		private void HandleInstructionRewrite()
		{
			if (this.textRewriteTimer >= this.textRewriteTime)
			{
				if (!this.rewriteStarted)
				{
					this.restart.text = this.writeSymbol.ToString();
					this.rewriteStarted = true;
				}
				if (this.singleRewriteTimer >= this.singleRewriteTime)
				{
					if (this.restartInstruction.Length + 1 == this.restart.text.Length)
					{
						this.rewriteStarted = false;
						this.singleRewriteTimer = 0f;
						this.textRewriteTimer = 0f;
						this.restart.text = this.restart.text.TrimEnd(new char[]
						{
							this.writeSymbol
						});
					}
					else
					{
						this.restart.text = this.restart.text.TrimEnd(new char[]
						{
							this.writeSymbol
						});
						SHSharp::SHGUItext shguitext = this.restart;
						shguitext.text += this.restartInstruction.ElementAt(this.restart.text.Length);
						SHSharp::SHGUItext shguitext2 = this.restart;
						shguitext2.text += this.writeSymbol;
					}
					this.singleRewriteTimer = 0f;
				}
			}
			this.singleRewriteTimer += Time.unscaledDeltaTime;
			this.textRewriteTimer += Time.unscaledDeltaTime;
		}

		// Token: 0x040029C7 RID: 10695
		private SHSharp::SHGUIprogressbar playerHealthBar;

		// Token: 0x040029C8 RID: 10696
		private SHSharp::SHGUIsprite standardZombieIcon;

		// Token: 0x040029C9 RID: 10697
		private SHSharp::SHGUItext standardZombiesLeft;

		// Token: 0x040029CA RID: 10698
		private SHSharp::SHGUIsprite fastZombieIcon;

		// Token: 0x040029CB RID: 10699
		private SHSharp::SHGUItext fastZombiesLeft;

		// Token: 0x040029CC RID: 10700
		private SHSharp::SHGUIsprite heavyZombieIcon;

		// Token: 0x040029CD RID: 10701
		private SHSharp::SHGUItext heavyZombiesLeft;

		// Token: 0x040029CE RID: 10702
		private SHSharp::SHGUItext wavePrepare;

		// Token: 0x040029CF RID: 10703
		private SHSharp::SHGUILoadingIndicator waveStartIndicator;

		// Token: 0x040029D0 RID: 10704
		private SHSharp::SHGUItext restart;

		// Token: 0x040029D1 RID: 10705
		private float textRewriteTimer;

		// Token: 0x040029D2 RID: 10706
		private float textRewriteTime = 3f;

		// Token: 0x040029D3 RID: 10707
		private float singleRewriteTimer;

		// Token: 0x040029D4 RID: 10708
		private float singleRewriteTime = 0.04f;

		// Token: 0x040029D5 RID: 10709
		private bool rewriteStarted;

		// Token: 0x040029D6 RID: 10710
		private string restartInstruction;

		// Token: 0x040029D7 RID: 10711
		private char writeSymbol = '▓';

		// Token: 0x040029D8 RID: 10712
		private bool restartTextEnabled;
	}
}