extern alias SHSharp;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using MCD.Audio;
using MCD.StoryApps.Common;


namespace MCD.StoryApps.Westdude
{
	public class APPWestern : SHSharp::SHGUIappbase
	{
		// Token: 0x0600276F RID: 10095 RVA: 0x0011AC3C File Offset: 0x0011903C
		public APPWestern() : base("westdude-by-mj-ported-from-mcd-by-garbear", true)
		{
			this.view = new WesternView();
			this.storyGenerator = new StoryGenerator();
			this.currentOpponentName = string.Empty;
			base.AddSubView(this.view);
			this.LaunchTutorial();
			this.APPINSTRUCTION.x -= 5;
			this.InitializeSounds();
		}

		// Token: 0x06002770 RID: 10096 RVA: 0x0011ACBF File Offset: 0x001190BF
		public override void Update()
		{
			base.Update();
			this.HandleCounting();
			this.HandleEnemyMove();
			this.HandleDeath();
		}

		// Token: 0x06002771 RID: 10097 RVA: 0x0011ACDC File Offset: 0x001190DC
		public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
		{
			if (key == SHSharp::SHGUIinput.enter)
			{
				this.HandlePhases();
			}
			else if (key == SHSharp::SHGUIinput.esc)
			{
				SHSharp::AudioManager.Instance.FadeoutAndLeave(this.musicInstance.gameObject, 1f, 0f);
				SHSharp::SHGUI.current.PopView();
				this.OnExit();
			}
		}

		// Token: 0x06002772 RID: 10098 RVA: 0x0011AD2D File Offset: 0x0011912D
		public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
		{
			if (clicked)
			{
				this.HandlePhases();
			}
		}

		// Token: 0x06002773 RID: 10099 RVA: 0x0011AD3B File Offset: 0x0011913B
		public override void OnExit()
		{
			//Singleton<PlayerManager>.Instance.Account.SetRichPresence(SystemAccount.RichPresence.RICH_PRESENCE_MENU);
		}

		// Token: 0x06002774 RID: 10100 RVA: 0x0011AD4D File Offset: 0x0011914D
		public override void Redraw(int offx, int offy)
		{
			base.Redraw(offx, offy);
			this.APPFRAME.Redraw(offx, offy);
			this.APPINSTRUCTION.Redraw(offx, offy);
			this.APPLABEL.Redraw(offx, offy);
		}

		// Token: 0x06002775 RID: 10101 RVA: 0x0011AD80 File Offset: 0x00119180
		private void HandlePhases()
		{
			switch (this.currentPhase)
			{
				case WesternPhase.Tutorial:
					this.HandleTutorialAction();
					break;
				case WesternPhase.Cinematic:
					this.LaunchMainGame();
					break;
				case WesternPhase.Counting:
					break;
				case WesternPhase.Duel:
					this.HandlePlayerMove();
					break;
				case WesternPhase.Win:
					if (this.view.DuelResultPrompter != null && this.view.DuelResultPrompter.IsFinished())
					{
						this.LaunchStoryScreen();
					}
					break;
				case WesternPhase.Lose:
					if (this.view.DuelResultPrompter != null && this.view.DuelResultPrompter.IsFinished())
					{
						this.LaunchGameOver();
					}
					break;
				case WesternPhase.StoryScreen:
					this.LaunchCinematic();
					break;
				case WesternPhase.GameOver:
					this.Restart();
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		// Token: 0x06002776 RID: 10102 RVA: 0x0011AE61 File Offset: 0x00119261
		private void CountEnemyTimeOffset()
		{
			this.enemyTimeOffset = this.baseEnemyTimeOffset / (float)this.level * 1.4f;
		}

		// Token: 0x06002777 RID: 10103 RVA: 0x0011AE80 File Offset: 0x00119280
		private void HandleCounting()
		{
			if (this.currentPhase == WesternPhase.Counting && this.view.CounterSprite.currentFrame >= this.view.CounterSprite.frames.Count - 1)
			{
				this.currentPhase = WesternPhase.Duel;
				this.enemyTimer = 0f;
			}
		}

		// Token: 0x06002778 RID: 10104 RVA: 0x0011AED8 File Offset: 0x001192D8
		private void HandleEnemyMove()
		{
			this.enemyTimer += Time.unscaledDeltaTime;
			//MelonLogger.Msg("Handling enemy move");
			if (this.enemyTimer >= this.enemyTimeOffset && this.currentPhase == WesternPhase.Duel && !this.enemyHasShot)
			{
				//MelonLogger.Msg("Enemy shot");
				//MelonLogger.Msg(this.playerHasShot);
				SHSharp::AudioManager.Instance.PlayClip(this.shot2Clip, null);
				this.view.ChangeSpriteAnimation(this.view.EnemySprite, MCDCom.AssetToText(MCDCom.GetAssetName("APPCowboyEnemyShooting.txt")), 12, 0.2f, false);
				this.enemyHasShot = true;
				if (!this.playerHasShot)
				{
					//MelonLogger.Msg("Player lost");
					this.currentPhase = WesternPhase.Lose;
					this.HandlePhases();
				}
			}
		}

		// Token: 0x06002779 RID: 10105 RVA: 0x0011AF7C File Offset: 0x0011937C
		private void HandlePlayerMove()
		{
			if (!this.playerHasShot && !this.isDuelOver)
			{
				this.playerHasShot = true;
				SHSharp::AudioManager.Instance.PlayClip(this.shot1Clip, null);
				this.view.ChangeSpriteAnimation(this.view.PlayerSprite, MCDCom.AssetToText(MCDCom.GetAssetName("APPCowboyShooting.txt")), 12, 0.2f, false);
				if (!this.enemyHasShot)
				{
					this.currentPhase = WesternPhase.Win;
				}
			}
		}

		// Token: 0x0600277A RID: 10106 RVA: 0x0011AFFC File Offset: 0x001193FC
		private void HandleDeath()
		{
			if (this.currentPhase > WesternPhase.Duel)
			{
				this.deathTimer += Time.unscaledDeltaTime;
				if (this.deathTimer < this.deathdelay || this.isDuelOver)
				{
					return;
				}
				this.view.CounterSprite.Kill();
				this.isDuelOver = true;
				if (this.currentPhase == WesternPhase.Win)
				{
					this.view.ChangeSpriteAnimation(this.view.EnemySprite, MCDCom.AssetToText(MCDCom.GetAssetName("APPCowboyEnemyDead.txt")), 12, 0.2f, false);
					this.view.InitializeDuelResultMessage(string.Format("^W4{0} died.", this.currentOpponentName));
					this.level++;
				}
				else if (this.currentPhase == WesternPhase.Lose)
				{
					this.view.ChangeSpriteAnimation(this.view.PlayerSprite, MCDCom.AssetToText(MCDCom.GetAssetName("APPCowboyDead.txt")), 12, 0.2f, false);
					this.view.InitializeDuelResultMessage((this.level != 1) ? string.Format("^W4{0} killed him.", this.currentOpponentName) : "^W4Apparently he didn't know how to kill.");
				}
			}
		}

		// Token: 0x0600277B RID: 10107 RVA: 0x0011B118 File Offset: 0x00119518
		private void HandleTutorialAction()
		{
			if (this.playerHasShot && this.view.TutorialComment != null && this.view.TutorialComment.IsFinished())
			{
				//this.playerHasShot = false; i did this
				this.LaunchCinematic();
			}
			else if (!this.playerHasShot)
			{
				this.playerHasShot = true;
				SHSharp::AudioManager.Instance.PlayClip(this.shot1Clip, null);
				this.view.ChangeSpriteAnimation(this.view.PlayerSprite, MCDCom.AssetToText(MCDCom.GetAssetName("APPCowboyShooting.txt")), 12, 0.2f, false);
				this.view.InitializeTutorialComment();
			}
		}

		// Token: 0x0600277C RID: 10108 RVA: 0x0011B1C0 File Offset: 0x001195C0
		private void LaunchTutorial()
		{
			this.view.InitializeTutorial();
			this.currentPhase = WesternPhase.Tutorial;
			string key = "enter"; // SHGame.Instance.ControlsSettings.GetKey(SHControlsSettings.SHGUIKeys.Confirm);
			string text = string.Format("Use Enter to Shoot", key);
			this.APPINSTRUCTION.text = text;
			this.APPINSTRUCTION.x = SHSharp::SHGUI.current.resolutionX - 1 - text.Length;
		}

		// Token: 0x0600277D RID: 10109 RVA: 0x0011B230 File Offset: 0x00119630
		private void LaunchCinematic()
		{
			this.currentOpponentName = this.storyGenerator.GenerateName();
			this.view.InitializeCinematic(this.level, this.currentOpponentName);
			this.currentPhase = WesternPhase.Cinematic;
			// TODO: There isn't an intro track?
			//this.tutPlaying = SHSharp::AudioManager.Instance.PlayClip(this.bgClip /*AudioResources.NonTerminal.WestDudeIntro*/, null);
			string key = "enter"; //SHGame.Instance.ControlsSettings.GetKey(SHControlsSettings.SHGUIKeys.Confirm);
			string text = string.Format("Press Enter to Skip Instructions", key);
			this.APPINSTRUCTION.text = text;
			this.APPINSTRUCTION.x = SHSharp::SHGUI.current.resolutionX - 6 - text.Length;
		}

		// Token: 0x0600277E RID: 10110 RVA: 0x0011B2DC File Offset: 0x001196DC
		private void LaunchMainGame()
		{
			this.view.InitializeMainGame();
			this.view.DuelResultPrompter = null;
			this.CountEnemyTimeOffset();
			this.currentPhase = WesternPhase.Counting;
			this.isDuelOver = false;
			this.enemyHasShot = false;
			this.playerHasShot = false;
			this.deathTimer = 0f;
			this.enemyTimer = 0f;
			string key = "enter"; //SHGame.Instance.ControlsSettings.GetKey(SHControlsSettings.SHGUIKeys.Confirm);
			string text = string.Format("Press Enter to Shoot", key);
			this.APPINSTRUCTION.text = text;
			this.APPINSTRUCTION.x = SHSharp::SHGUI.current.resolutionX - 6 - text.Length;
		}

		// Token: 0x0600277F RID: 10111 RVA: 0x0011B388 File Offset: 0x00119788
		private void LaunchGameOver()
		{
			this.view.InitializeStoryScreen(this.storyGenerator.GetGameOverStory(this.level));
			this.currentPhase = WesternPhase.GameOver;
			string key = "enter"; // SHGame.Instance.ControlsSettings.GetKey(SHControlsSettings.SHGUIKeys.Confirm);
			string text = string.Format("Press Enter to Restart", key);
			this.APPINSTRUCTION.text = text;
			this.APPINSTRUCTION.x = SHSharp::SHGUI.current.resolutionX - 6 - text.Length;
		}

		// Token: 0x06002780 RID: 10112 RVA: 0x0011B408 File Offset: 0x00119808
		private void LaunchStoryScreen()
		{
			this.view.InitializeStoryScreen(string.Format("{0} {1}", this.currentOpponentName, this.storyGenerator.GetRandomStory()));
			this.currentPhase = WesternPhase.StoryScreen;
			string key = "enter"; //SHGame.Instance.ControlsSettings.GetKey(SHControlsSettings.SHGUIKeys.Confirm);
			string text = string.Format("Press Enter to Skip Instructions.", key);
			this.APPINSTRUCTION.text = text;
			this.APPINSTRUCTION.x = SHSharp::SHGUI.current.resolutionX - 6 - text.Length;
		}

		// Token: 0x06002781 RID: 10113 RVA: 0x0011B492 File Offset: 0x00119892
		private void InitializeSounds()
		{
			this.musicInstance = MCDAudio.PlayMusic(null, this.bgClip, 1f, true, 1f, true);
		}

		// Token: 0x06002782 RID: 10114 RVA: 0x0011B4B9 File Offset: 0x001198B9
		private void Restart()
		{
			SHSharp::AudioManager.Instance.FadeoutAndLeave(this.musicInstance.gameObject, 1f, 0f);
			this.view.Kill();
			this.Kill();
			SHSharp::SHGUI.current.LaunchAppByName("StoryApps.Games.Western.APPWesternIntro");
		}

		// Token: 0x040029E2 RID: 10722
		private WesternPhase currentPhase;

		// Token: 0x040029E3 RID: 10723
		private WesternView view;

		// Token: 0x040029E4 RID: 10724
		private StoryGenerator storyGenerator;

		// Token: 0x040029E5 RID: 10725
		private AudioSource musicInstance;

		// Token: 0x040029E6 RID: 10726
		private string currentOpponentName;

		// Token: 0x040029E7 RID: 10727
		private float enemyTimer;

		// Token: 0x040029E8 RID: 10728
		private float deathTimer;

		// Token: 0x040029E9 RID: 10729
		private float deathdelay = 0.2f;

		// Token: 0x040029EA RID: 10730
		private float enemyTimeOffset;

		// Token: 0x040029EB RID: 10731
		private float baseEnemyTimeOffset = 0.3f;

		// Token: 0x040029EC RID: 10732
		private int level = 1;

		// Token: 0x040029ED RID: 10733
		private bool enemyHasShot;

		// Token: 0x040029EE RID: 10734
		private bool playerHasShot;

		// Token: 0x040029EF RID: 10735
		private bool isDuelOver;

		// Token: 0x040029F0 RID: 10736
		private string resultMessage;

		// Token: 0x040029F1 RID: 10737
		private const int instructionMargin = 6;

		private AudioClip shot1Clip = MCDAudio.LoadWavFromEmbeddedResource("AC_Westdude_Shot1.wav");

		private AudioClip shot2Clip = MCDAudio.LoadWavFromEmbeddedResource("AC_Westdude_Shot2.wav");

		private AudioClip bgClip = MCDAudio.LoadWavFromEmbeddedResource("AC_Westdude_Background.wav");

		private AudioSource tutPlaying;
	}

	public class APPWesternIntro : SHSharp::SHGUIappbase
	{
		// Token: 0x06002783 RID: 10115 RVA: 0x0011B4F8 File Offset: 0x001198F8
		public APPWesternIntro() : base("westdude by mj & garbear", true)
		{
			this.APPFRAME.Kill();
			this.APPLABEL.Kill();
			this.APPINSTRUCTION.Kill();
			this.InitializeLogo();
			this.InitializeStoryText();
			this.ShowAnimationStep(1);
			//Singleton<PlayerManager>.Instance.Account.SetRichPresence(SystemAccount.RichPresence.RICH_PRESENCE_MINIGAME);
		}

		// Token: 0x06002784 RID: 10116 RVA: 0x0011B55C File Offset: 0x0011995C
		public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
		{
			if (key == SHSharp::SHGUIinput.enter)
			{
				this.StartGame();
			}
			else if (key == SHSharp::SHGUIinput.esc)
			{
				this.soundSource.Stop();
				SHSharp::SHGUI.current.PopView();
				this.OnExit();
			}
		}

		// Token: 0x06002785 RID: 10117 RVA: 0x0011B592 File Offset: 0x00119992
		public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
		{
			if (clicked)
			{
				this.StartGame();
			}
		}

		// Token: 0x06002786 RID: 10118 RVA: 0x0011B5A0 File Offset: 0x001199A0
		public override void OnExit()
		{
			if (this.soundSource != null)
			{
				UnityEngine.Object.Destroy(this.soundSource);
			}
		}

		// Token: 0x06002787 RID: 10119 RVA: 0x0011B5BE File Offset: 0x001199BE
		public override void Redraw(int offx, int offy)
		{
			base.Redraw(offx, offy);
			this.APPFRAME.Redraw(offx, offy);
			this.APPINSTRUCTION.Redraw(offx, offy);
			this.APPLABEL.Redraw(offx, offy);
		}

		// Token: 0x06002788 RID: 10120 RVA: 0x0011B5EF File Offset: 0x001199EF
		private void StartGame()
		{
			this.Kill();
			this.soundSource.Stop();
			SHSharp::SHGUI.current.AddViewOnTop(new APPWestern());
		}

		// Token: 0x06002789 RID: 10121 RVA: 0x0011B614 File Offset: 0x00119A14
		private void InitializeLogo()
		{
			this.logoIview = new SHSharp::SHGUIview();
			this.logoIview.hidden = true;
			this.WesternLogoSprite = new SHSharp::SHGUIsprite();
			MCDCom.AddFrameFromStr(this.WesternLogoSprite, MCDCom.AssetToText(MCDCom.GetAssetName("APPWesternLogo.txt")), 17);
			this.WesternLogoSprite.x = 2;
			this.WesternLogoSprite.y = 1;
			this.logoSound = MCDAudio.LoadWavFromEmbeddedResource("AC_Westdude_Bum.wav");
			this.soundSource = SHSharp::SHGUI.current.gameObject.AddComponent<AudioSource>();
			this.soundSource.clip = this.logoSound;
			this.soundSource.loop = true;
			this.logoIview.AddSubView(this.WesternLogoSprite);
		}

		// Token: 0x0600278A RID: 10122 RVA: 0x0011B6C8 File Offset: 0x00119AC8
		private void InitializeStoryText()
		{
			this.prompters = new SHSharp::SHGUIview();
			this.prompters.hidden = true;
			this.prompter = new SHSharp::SHGUIprompter(20, 6, 'w');
			this.prompter.SetInput("^M2He came out of nowhere. ^M2\nHe had no family. ^M2\nHe had no dreams. ^M2\n\nHe had one goal.^M4^W1\nRevenge. ", true);
			this.prompters.AddSubView(this.prompter);
		}

		// Token: 0x0600278B RID: 10123 RVA: 0x0011B71F File Offset: 0x00119B1F
		public override void Update()
		{
			base.Update();
			if (this.prompter != null && this.prompter.IsFinished())
			{
				this.ShowAnimationStep(2);
			}
		}

		// Token: 0x0600278C RID: 10124 RVA: 0x0011B74C File Offset: 0x00119B4C
		private void ShowAnimationStep(int step)
		{
			if (step <= this.lastShownStep)
			{
				return;
			}
			if (step == 1)
			{
				this.prompters.hidden = false;
				base.AddSubView(this.prompters);
			}
			else
			{
				this.prompters.Kill();
				this.logoIview.hidden = false;
				this.soundSource.Play();
				base.AddSubView(this.logoIview);
			}
			this.lastShownStep = step;
		}

		// Token: 0x040029F2 RID: 10738
		private SHSharp::SHGUIsprite WesternLogoSprite;

		// Token: 0x040029F3 RID: 10739
		private SHSharp::SHGUIview prompters;

		// Token: 0x040029F4 RID: 10740
		private SHSharp::SHGUIprompter prompter;

		// Token: 0x040029F5 RID: 10741
		private SHSharp::SHGUIview logoIview;

		// Token: 0x040029F6 RID: 10742
		private AudioSource soundSource;

		// Token: 0x040029F7 RID: 10743
		private AudioClip logoSound;

		// Token: 0x040029F8 RID: 10744
		private float timer;

		// Token: 0x040029F9 RID: 10745
		private int lastShownStep = -1;
	}

	public class StoryGenerator
	{
		// Token: 0x0600278D RID: 10125 RVA: 0x0011B7C4 File Offset: 0x00119BC4
		public StoryGenerator()
		{
			this.baseNames = new List<string>
			{
				"Jack",
				"Pete",
				"Frank",
				"Clive",
				"Palmer",
				"Morgan",
				"Zak",
				"Warren",
				"Jesse",
				"Buster",
				"Bryce",
				"Blake",
				"Dude",
				"Randy",
				"Boone",
				"Emmett",
				"Maverick",
				"Nash",
				"Butch",
				"Gabe",
				"Luke",
				"Willy",
				"Dick",
				"Gus",
				"Billy",
				"Harry"
			};
			this.baseEpithets = new List<string>
			{
				"Handsome",
				"Ugly",
				"Twisted",
				"Huge",
				"Bazooka",
				"Calm",
				"Smart",
				"Unpredictable",
				"Caring",
				"Extraordinary",
				"Lazy",
				"Ambiguous",
				"Super",
				"Hot",
				"Red",
				"Lucky",
				"Dirty",
				"Cruel",
				"Vengeful",
				"Braggart",
				"Lasso",
				"Reckless",
				"Vicious",
				"Schwifty",
				"Little"
			};
			this.baseStories = new List<string>
			{
				"had a cancer.\nLast week of his life was taken from him.",
				"loved his dog.\nThey used to go on a long walk every sunday morning.",
				"was a master of pancakes. A fresh portion was waiting for him in house.",
				"was crafting wooden guns for homeless children.\nNow children are defenceless.",
				"was about to marry.\nHis wife would definitely miss him.",
				"was a Santa every Christmas.\nChildren were happy sitting on his knees and sharing their dreams with Santa.",
				"was just an ordinary guy.\nNobody would miss him.",
				"used to participate in Curling World Championship.\nYesterday he announced a big comeback.",
				"loved Donald Duck comics. He wanted to be like Scrooge McDuck.",
				"had two wives and three sons.\n They would seek revenge.",
				"- nobody cared about him.",
				"worked for charity. He was helping bad guys to become good guys.",
				"loved to hear birds tweeting in the middle of day."
			};
			this.currentNames = new List<string>(this.baseNames);
			this.currentEpithets = new List<string>(this.baseEpithets);
			this.currentStories = new List<string>(this.baseStories);
		}

		// Token: 0x0600278E RID: 10126 RVA: 0x0011BAF1 File Offset: 0x00119EF1
		public string GenerateName()
		{
			return string.Format("{0} {1}", this.GetRandomFromList(this.baseEpithets, this.currentEpithets), this.GetRandomFromList(this.baseNames, this.currentNames));
		}

		// Token: 0x0600278F RID: 10127 RVA: 0x0011BB24 File Offset: 0x00119F24
		public string GetGameOverStory(int level)
		{
			string arg = (level <= 1) ? "day" : "days";
			return string.Format("^M2He survived {0} {1}.\n\nDeath.\nHis destiny was fulfilled. ", level, arg);
		}

		// Token: 0x06002790 RID: 10128 RVA: 0x0011BB59 File Offset: 0x00119F59
		public string GetRandomStory()
		{
			return this.GetRandomFromList(this.baseStories, this.currentStories);
		}

		// Token: 0x06002791 RID: 10129 RVA: 0x0011BB70 File Offset: 0x00119F70
		private string GetRandomFromList(List<string> baseList, List<string> currentList)
		{
			if (!currentList.Any<string>())
			{
				currentList = new List<string>(baseList);
			}
			int index = UnityEngine.Random.Range(0, currentList.Count);
			string result = currentList.ElementAt(index);
			currentList.RemoveAt(index);
			return result;
		}

		// Token: 0x040029FA RID: 10746
		private readonly List<string> baseNames;

		// Token: 0x040029FB RID: 10747
		private readonly List<string> baseEpithets;

		// Token: 0x040029FC RID: 10748
		private readonly List<string> baseStories;

		// Token: 0x040029FD RID: 10749
		private List<string> currentStories;

		// Token: 0x040029FE RID: 10750
		private List<string> currentNames;

		// Token: 0x040029FF RID: 10751
		private List<string> currentEpithets;
	}

	public enum WesternPhase
	{
		// Token: 0x040029DA RID: 10714
		Tutorial,
		// Token: 0x040029DB RID: 10715
		Cinematic,
		// Token: 0x040029DC RID: 10716
		Counting,
		// Token: 0x040029DD RID: 10717
		Duel,
		// Token: 0x040029DE RID: 10718
		Win,
		// Token: 0x040029DF RID: 10719
		Lose,
		// Token: 0x040029E0 RID: 10720
		StoryScreen,
		// Token: 0x040029E1 RID: 10721
		GameOver
	}

	public class WesternView : SHSharp::SHGUIview
	{
		// Token: 0x06002793 RID: 10131 RVA: 0x0011BBB5 File Offset: 0x00119FB5
		public void InitializeStoryScreen(string story)
		{
			base.KillChildren();
			this.InitializeStoryPrompter(story);
		}

		// Token: 0x06002794 RID: 10132 RVA: 0x0011BBC4 File Offset: 0x00119FC4
		public void InitializeMainGame()
		{
			base.KillChildren();
			this.InitializeGround();
			this.InitializePlayer();
			this.InitializeEnemy();
			this.InitializeCounterText();
		}

		// Token: 0x06002795 RID: 10133 RVA: 0x0011BBE4 File Offset: 0x00119FE4
		public void InitializeCinematic(int level, string opponentName)
		{
			base.KillChildren();
			this.InitializeCinematicBackground();
			this.InitializeDuelTitle(level, opponentName);
		}

		// Token: 0x06002796 RID: 10134 RVA: 0x0011BBFA File Offset: 0x00119FFA
		public void InitializeTutorial()
		{
			this.InitializePlayer();
			this.PlayerSprite.x += 3;
			this.PlayerSprite.y -= 2;
			this.InitializeHintText();
		}

		// Token: 0x06002797 RID: 10135 RVA: 0x0011BC2E File Offset: 0x0011A02E
		public void InitializeTutorialComment()
		{
			this.TutorialComment = new SHSharp::SHGUIprompter(30, 16, 'w');
			this.TutorialComment.SetInput("He knew how to shoot.^M2\nBut did he know how to kill?\n", true);
			base.AddSubView(this.TutorialComment);
		}

		// Token: 0x06002798 RID: 10136 RVA: 0x0011BC5F File Offset: 0x0011A05F
		private void InitializeHintText()
		{
			this.TutorialTitle = new SHSharp::SHGUIprompter(5, 2, 'w');
			this.TutorialTitle.SetInput("Revenge.^M2\nThis word had been with him for a long time.", true);
			base.AddSubView(this.TutorialTitle);
		}

		// Token: 0x06002799 RID: 10137 RVA: 0x0011BC90 File Offset: 0x0011A090
		private void InitializeCinematicBackground()
		{
			this.CinematicSprite = new SHSharp::SHGUIsprite();
			MCDCom.AddFrameFromStr(this.CinematicSprite, MCDCom.AssetToText(MCDCom.GetAssetName("APPWesternCinematic.txt")), 23);
			this.CinematicSprite.color = 'w';
			this.CinematicSprite.x = 1;
			this.CinematicSprite.y = 0;
			this.CinematicSprite.loops = false;
			this.CinematicSprite.animationSpeed = 0.1f;
			base.AddSubView(this.CinematicSprite);
		}

		// Token: 0x0600279A RID: 10138 RVA: 0x0011BD0C File Offset: 0x0011A10C
		private void InitializeGround()
		{
			this.GroundSprite = new SHSharp::SHGUIsprite();
			this.GroundSprite.AddFrame(new string('/', 100), null /*default(AudioResource)*/);
			base.AddSubView(this.GroundSprite);
			this.GroundSprite.x = 1;
			this.GroundSprite.y = 22;
		}

		// Token: 0x0600279B RID: 10139 RVA: 0x0011BD68 File Offset: 0x0011A168
		private void InitializePlayer()
		{
			this.PlayerSprite = new SHSharp::SHGUIsprite();
			MCDCom.AddFrameFromStr(this.PlayerSprite, MCDCom.AssetToText(MCDCom.GetAssetName("APPCowboyIdle.txt")), 12);
			this.PlayerSprite.x = 3;
			this.PlayerSprite.y = 10;
			this.PlayerSprite.loops = true;
			this.PlayerSprite.animationSpeed = 0.3f;
			base.AddSubView(this.PlayerSprite);
		}

		// Token: 0x0600279C RID: 10140 RVA: 0x0011BDD8 File Offset: 0x0011A1D8
		private void InitializeEnemy()
		{
			this.EnemySprite = new SHSharp::SHGUIsprite();
			MCDCom.AddFrameFromStr(this.EnemySprite, MCDCom.AssetToText(MCDCom.GetAssetName("APPCowboyEnemyIdle.txt")), 12);
			this.EnemySprite.x = 43;
			this.EnemySprite.y = 10;
			this.EnemySprite.loops = true;
			this.EnemySprite.animationSpeed = 0.3f;
			base.AddSubView(this.EnemySprite);
		}

		// Token: 0x0600279D RID: 10141 RVA: 0x0011BE48 File Offset: 0x0011A248
		private void InitializeCounterText()
		{
			this.CounterSprite = new SHSharp::SHGUIsprite();
			MCDCom.AddFrameFromStr(this.CounterSprite, MCDCom.AssetToText(MCDCom.GetAssetName("APPWesternCounter.txt")), 7);
			this.CounterSprite.x = 16;
			this.CounterSprite.y = 1;
			this.CounterSprite.animationSpeed = 1f;
			this.CounterSprite.loops = false;
			base.AddSubView(this.CounterSprite);
		}

		// Token: 0x0600279E RID: 10142 RVA: 0x0011BEB8 File Offset: 0x0011A2B8
		private void InitializeDuelTitle(int level, string opponentName)
		{
			SHSharp::SHGUIguruchatwindow shguiguruchatwindow = new SHSharp::SHGUIguruchatwindow(null);
			shguiguruchatwindow.SetContent(string.Format("^M3Day {0}.\nHim vs. {1}", level, opponentName));
			shguiguruchatwindow.x = 3;
			shguiguruchatwindow.y = 2;
			base.AddSubView(shguiguruchatwindow);
		}

		// Token: 0x0600279F RID: 10143 RVA: 0x0011BEFA File Offset: 0x0011A2FA
		public void InitializeDuelResultMessage(string message)
		{
			this.DuelResultPrompter = new SHSharp::SHGUIprompter(20, 5, 'w');
			this.DuelResultPrompter.SetInput(message, true);
			base.AddSubView(this.DuelResultPrompter);
		}

		// Token: 0x060027A0 RID: 10144 RVA: 0x0011BF26 File Offset: 0x0011A326
		private void InitializeStoryPrompter(string story)
		{
			this.EndStoryPrompter = new SHSharp::SHGUIprompter(16, 10, 'w');
			this.EndStoryPrompter.SetInput(story, true);
			base.AddSubView(this.EndStoryPrompter);
		}

		// Token: 0x060027A1 RID: 10145 RVA: 0x0011BF53 File Offset: 0x0011A353
		public void ChangeSpriteAnimation(SHSharp::SHGUIsprite sprite, string text, int rowsPerFrame, float speed, bool loops = false)
		{
			sprite.frames.Clear();
			sprite.currentFrame = 0;
			sprite.currentAnimationTimer = 0f;
			sprite.animationSpeed = speed;
			sprite.loops = loops;
			MCDCom.AddFrameFromStr(sprite, text, rowsPerFrame);
			//sprite.AddFramesFromFile(file, rowsPerFrame, false);
		}

		// Token: 0x04002A00 RID: 10752
		public SHSharp::SHGUIsprite PlayerSprite;

		// Token: 0x04002A01 RID: 10753
		public SHSharp::SHGUIsprite EnemySprite;

		// Token: 0x04002A02 RID: 10754
		public SHSharp::SHGUIsprite GroundSprite;

		// Token: 0x04002A03 RID: 10755
		public SHSharp::SHGUIsprite CounterSprite;

		// Token: 0x04002A04 RID: 10756
		public SHSharp::SHGUIsprite CinematicSprite;

		// Token: 0x04002A05 RID: 10757
		public SHSharp::SHGUIprompter DuelResultPrompter;

		// Token: 0x04002A06 RID: 10758
		public SHSharp::SHGUIprompter EndStoryPrompter;

		// Token: 0x04002A07 RID: 10759
		public SHSharp::SHGUIprompter TutorialComment;

		// Token: 0x04002A08 RID: 10760
		public SHSharp::SHGUIprompter TutorialTitle;
	}
}
