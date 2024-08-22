extern alias SHSharp;

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

using UnityEngine;

using MelonLoader;

using HarmonyLib;
using System.Text.RegularExpressions;

using MCD.StoryApps.Slasher;
using MCD.StoryApps.IdleMinigame;
using MCD.StoryApps.Westdude;
using MCD.APPPong;
using SH.APPspoilers;
using SH.BetaTalking1a;
using System.Text;



// !!!!!!!!!!!!!! USE THIS BAR: │  <----

namespace MOREApps
{

    public static class LocalizationAccessHelperExtensions
    {
        // Token: 0x06000B7D RID: 2941 RVA: 0x000431C3 File Offset: 0x000413C3
        public static string T(this string s, params string[] variables)
        {
            return SHSharp::LocalizationManager.Instance.GetLocalized(s, variables);
        }

        // Token: 0x06000B7F RID: 2943 RVA: 0x000431E4 File Offset: 0x000413E4
        public static List<string> ListT(this string s)
        {
            return s.T(Array.Empty<string>()).Split(new string[]
            {
            ":::"
            }, StringSplitOptions.None).ToList<string>();
        }
    }

    public class Init : MelonMod
    {
		public override void OnInitializeMelon()
        {
            base.OnInitializeMelon();
        }

		public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
			LoggerInstance.Msg($"Scene {sceneName} with build index {buildIndex} has been initialized!");

			if (buildIndex == 1)
            {
				CreateEmptyMenu();
            }
		}

		private void CreateEmptyMenu()
		{
			// Create a new empty menu
			SHSharp::SHGUIcommanderview emptyMenu = new SHSharp::SHGUIcommanderview
			{
				isRoot = true,
				path = "C:\\"
			};

			// Example of adding a button to the empty menu
			var simpleButton = new SHSharp::SHGUIcommanderbutton("012345678901│12345678", Color.White, (SHSharp::SHGUIcommanderbutton button) =>
			{
				MelonLogger.Msg("Simple Button clicked!");
			});
			//emptyMenu.AddButtonView(simpleButton);

			new Exploit(emptyMenu);

			new ReadME("spoilers.exe", "", emptyMenu, 'w', null, (SHSharp::SHGUIcommanderbutton button) => SHSharp::SHGUI.current.AddViewOnTop(new SHAPPspoilers()));

			new Last(emptyMenu);

			new Levels(emptyMenu);

			new Endless(emptyMenu);

			new Challenges(emptyMenu);

			new Mods(emptyMenu);

			new ReadME("readme.txt", "FolderStructure.Data.item.PROUDLY_PRESENTS___BETA_VERSION", emptyMenu);

			new App("replays.exe", "APPkillstagram", emptyMenu, Color.White, "FolderStructure.Data.item.KILLSTAGRAM_KILLSTAGRAM_KILLSTAGRAM");

			new App("recruit.exe", "APPrecruitredirect", emptyMenu, Color.White, "FolderStructure.Data.item.RECRUIT_DESCRIPTION");

			new App("credits.exe", "APPcreditsfinal", emptyMenu);

			new App("more.exe", "APPmore", emptyMenu, Color.White, "FolderStructure.Data.item.WANT_MORE");

			new App("quit.exe", "APPquit", emptyMenu);

			new ReadME("------------", "", emptyMenu, Color.White, "--------");

			new Setting(emptyMenu);

			new Apps(emptyMenu);

			new Demos(emptyMenu);

			new Cellular(emptyMenu);

			new Wires(emptyMenu);

			new Games(emptyMenu);

			new VR(emptyMenu);

			new Arts(emptyMenu);

			new Videos(emptyMenu);

			new Tests(emptyMenu);

			new Secrets(emptyMenu);

			new ReadME("mod.txt", "Made by Garbear", emptyMenu, Color.Green, " : - ) >");
			// Add the menu to the view stack
			SHSharp::SHGUI.current.AddViewOnTop(emptyMenu);

			MelonLogger.Msg("Empty menu with a simple button created.");

			//MelonLogger.Msg(Challenges.GetData());
		}
	}

	// TODO: Last level button

	public class Exploit : SHSharp::SHGUIcommanderbutton
    {
		public Exploit(SHSharp::SHGUIcommanderview parent, char color = 'w', string prefix = null)
			: base("superhot.exe".PadRight(12, ' ') + ReadME.SolvePrefix(prefix), color, PressCommand())
        {
			this.SetListLink(parent);
            this.SetData("DON'T launch this without backing up your save! This will launch the first level of the game.");
			this.SetRightPanelView(CreateExploitUpdatingView());
			this.RefreshText();
			this.AddDescriptionLikeUpdatedOrNew();

			parent.AddButtonView(this);
        }

        public static Action<SHSharp::SHGUIcommanderbutton> PressCommand()
        {
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				// First level
				SHSharp::LevelInfo levelInfoBySceneFileName = SHSharp::LevelSetup.GetLevelInfoByID(1);
				SHSharp::SHGUI.current.LaunchLevelAppTunnels(levelInfoBySceneFileName, true, false);
			};
		}

		public static SHSharp::SHGUIFlatView CreateExploitUpdatingView()
        {
			SHSharp::SHGUIFlatView updateView = new SHSharp::SHGUIFlatView(0, 1, 32, 19);
			updateView.UpdateOnlyWhenVisible = true;
			SHSharp::SHGUIanimationtimeline timeline = updateView.AddSubView(new SHSharp::SHGUIanimationtimeline()) as SHSharp::SHGUIanimationtimeline;
			timeline.UpdateOnlyWhenVisible = true;
			SHSharp::SHGUIprogressbar progressView = null;
			timeline.NewPhase().OnStart(delegate
			{
				progressView = (timeline.AddSubView(new SHSharp::SHGUIprogressbar(0, 7, 29, "DOWNLOADING".T(Array.Empty<string>()), "")) as SHSharp::SHGUIprogressbar);
				progressView.overrideFadeInSpeed = float.MaxValue;
			}).OnUpdate(delegate
			{
			}).EndAfter(1f);
			SHSharp::SHGUIanimationphase phase1_5 = timeline.NewPhase();
			phase1_5.OnStart(delegate
			{
			}).OnUpdate(delegate
			{
				progressView.currentProgress = phase1_5.GetProgress();
			}).EndAfter(10f);
			timeline.NewPhase().OnStart(delegate
			{
				progressView.currentProgress = 1f;
				progressView.labelView.hidden = true;
			}).OnUpdate(delegate
			{
			}).EndAfter(0.5f);
			SHSharp::SHGUItext updatedViewText = null;
			SHSharp::SHGUIframe updatedViewFrame = null;
			string crackUpdatedText = "SHSharp::piOsMenu.01.SUPERHOT_EXE_UPDATED".T(Array.Empty<string>());
			timeline.NewPhase().OnStart(delegate
			{
				progressView.Kill();
				int num = 16 - crackUpdatedText.Length / 2;
				updatedViewFrame = (timeline.AddSubView(new SHSharp::SHGUIframe(num - 1, 7, num + crackUpdatedText.Length + 1, 9, 'z', null)) as SHSharp::SHGUIframe);
				updatedViewText = (timeline.AddSubView(new SHSharp::SHGUItext(crackUpdatedText, 16 - crackUpdatedText.Length / 2, 8, 'z', false)) as SHSharp::SHGUItext);
				updateView.HasDrawingFinished = true;
			}).OnUpdate(delegate
			{
			}).EndAfter(2f);
			SHSharp::SHGUItext readmeView = null;
			timeline.NewPhase().OnStart(delegate
			{
				progressView.Kill();
				updatedViewFrame.Kill();
				updatedViewText.Kill();
				readmeView = (timeline.AddSubView(new SHSharp::SHGUItext("FolderStructure.Data.item.PROUDLY_PRESENTS___BETA_VERSION".T(Array.Empty<string>()), 0, 0, 'z', false)) as SHSharp::SHGUItext);
				readmeView.CenterTextForLineLength(32);
			}).OnUpdate(delegate
			{
			}).EndAfter(2f);
			return updateView;
		}
	}

	public class Last : SHSharp::SHGUIcommanderbutton
	{
		static string[] naggingFlavours = "SHSharp::piOsMenu.naggingFlavours.01.WHAT_ARE_YOU_WAITING_FOR___CAN".ListT().ToArray();
		public Last(SHSharp::SHGUIcommanderview parent, char color = 'w', string prefix = null)
			: base("last.exe".PadRight(12, ' ') + ReadME.SolvePrefix(prefix), color, (SHSharp::SHGUIcommanderbutton button) => { })
		{
			SHSharp::LevelInfo Lvl = SHSharp::LevelSetup.GetNewLevelInfo();

			PrepareLevelCommanderButtonForLevel(this, Lvl, parent, true);

			parent.AddButtonView(this);
		}

		public static string PrepareLevelDescription(List<string> dictionary, SHSharp::LevelInfo levelInfo)
		{
			bool flag = false;
			string text = SHSharp::LocalizationManager.Instance.GetLocalized(levelInfo.RawName + ".desc", ref flag, Array.Empty<string>());
			if (!flag || text == "" || SHSharp::GameplayModifiers.CurrentChallenge != null)
			{
				text = "";
			}
			levelInfo.LoadLevelTime();
			if (levelInfo.HaveLevelTime() || levelInfo.Secrets > 0 || text != string.Empty || SHSharp::GameplayModifiers.CurrentChallenge != null)
			{
				string text2 = "";
				if (SHSharp::GameplayModifiers.CurrentChallenge != null)
				{
					for (int i = 0; i < 32; i++)
					{
						text2 += "^";
					}
					text2 = SHSharp::StringScrambler.GetScrambledSimply(text2);
				}
				string text3 = "^";
				int num = UnityEngine.Random.Range(32, 120);
				for (int j = 0; j < num; j++)
				{
					text3 += "^";
				}
				for (int k = 0; k < num / 32; k++)
				{
					text3 = text3.Insert(UnityEngine.Random.Range(0, text3.Length - 1), dictionary[UnityEngine.Random.Range(0, dictionary.Count)]);
				}
				string text4 = "\n\n";
				if (SHSharp::GameplayModifiers.CurrentChallenge != null)
				{
					if (levelInfo.LevelTime.challengesTime.ContainsKey(SHSharp::GameplayModifiers.CurrentChallenge.ID))
					{
                        SHSharp::LocalizationLanguageSelector.AvailableLanguage currentLanguage = SHSharp::Settings.Instance.CurrentLanguage;
						if (currentLanguage == SHSharp::LocalizationLanguageSelector.AvailableLanguage.FRA || currentLanguage - SHSharp::LocalizationLanguageSelector.AvailableLanguage.RUS <= 1 || currentLanguage == SHSharp::LocalizationLanguageSelector.AvailableLanguage.SPA)
						{
							text4 = string.Concat(new string[]
							{
							text4,
							" ",
							"MENU_CHALLENGECOMPLETED".T(Array.Empty<string>()),
							" ",
							SHSharp::GameplayModifiers.CurrentChallenge.Name,
							"\n"
							});
						}
						else
						{
							text4 = string.Concat(new string[]
							{
							text4,
							" ",
							SHSharp::GameplayModifiers.CurrentChallenge.Name,
							" ",
							"MENU_CHALLENGECOMPLETED".T(Array.Empty<string>()),
							"\n"
							});
						}
						float num2 = levelInfo.LevelTime.challengesTime[SHSharp::GameplayModifiers.CurrentChallenge.ID];
						float num3 = 0f;
						if (levelInfo.ChallengesTimesPars.ContainsKey(SHSharp::GameplayModifiers.CurrentChallenge.ID))
						{
							num3 = levelInfo.ChallengesTimesPars[SHSharp::GameplayModifiers.CurrentChallenge.ID];
						}
						else if (levelInfo.gameTimePar > 0f)
						{
							num3 = levelInfo.gameTimePar;
						}
						text4 = string.Concat(new string[]
						{
						text4,
						" ",
						"MENU_PERSONALBEST".T(Array.Empty<string>()),
						": ",
						num2.ToString(".00"),
						"s\n"
						});
						if (num3 > 0f)
						{
							if (num2 < num3)
							{
								text4 = string.Concat(new string[]
								{
								text4,
								" ",
								"MENU_REDTIME".T(Array.Empty<string>()),
								": ",
								num3.ToString(".00"),
								"s\n"
								});
								text4 = text4 + " " + "MENU_REDTIMEACHIEVED".T(Array.Empty<string>()) + "\n\n";
							}
							else
							{
								text4 = string.Concat(new string[]
								{
								text4,
								" ",
								"MENU_REDTIME".T(Array.Empty<string>()),
								": ",
								num3.ToString(".00"),
								"s\n"
								});
								text4 = text4 + " " + "MENU_REDTIMENOTACHIEVED".T(Array.Empty<string>()) + "\n\n";
							}
						}
						else
						{
							text4 += "\n";
						}
					}
					if (text4 == "\n\n")
					{
						string name = SHSharp::GameplayModifiers.CurrentChallenge.Name;
						"MENU_CHALLENGENOTCOMPLETED".T(Array.Empty<string>());
						if (SHSharp::Settings.Instance.CurrentLanguage == SHSharp::LocalizationLanguageSelector.AvailableLanguage.FRA || SHSharp::Settings.Instance.CurrentLanguage == SHSharp::LocalizationLanguageSelector.AvailableLanguage.SPA || SHSharp::Settings.Instance.CurrentLanguage == SHSharp::LocalizationLanguageSelector.AvailableLanguage.PRT || SHSharp::Settings.Instance.CurrentLanguage == SHSharp::LocalizationLanguageSelector.AvailableLanguage.RUS || SHSharp::Settings.Instance.CurrentLanguage == SHSharp::LocalizationLanguageSelector.AvailableLanguage.POL)
						{
							text4 = string.Concat(new string[]
							{
							text4,
							" ",
							"MENU_CHALLENGENOTCOMPLETED".T(Array.Empty<string>()),
							" ",
							SHSharp::GameplayModifiers.CurrentChallenge.Name,
							"\n"
							});
						}
						else
						{
							text4 = string.Concat(new string[]
							{
							text4,
							" ",
							SHSharp::GameplayModifiers.CurrentChallenge.Name,
							" ",
							"MENU_CHALLENGENOTCOMPLETED".T(Array.Empty<string>()),
							"\n"
							});
						}
						text4 = text4 + " " + "MENU_CHALLENGENOTCOMPLETED2".T(Array.Empty<string>()) + "\n\n";
					}
				}
				string text5 = SHSharp::StringScrambler.GetScrambledSimply(SHSharp::StringScrambler.filler);
				for (int l = 0; l < 10; l++)
				{
					text5 = text5.Insert(UnityEngine.Random.Range(0, text5.Length - 1), dictionary[UnityEngine.Random.Range(0, dictionary.Count)]);
				}
				string text6 = "";
				if (levelInfo.Secrets > 0 && SHSharp::GameplayModifiers.CurrentChallenge == null)
				{
					if (text != string.Empty)
					{
						text6 = "\n";
					}
					text6 += "MENU_SECRETNOTCRACKED".T(Array.Empty<string>());
					int num4 = levelInfo.SecretsFound();
					if (num4 > 0 && num4 == levelInfo.Secrets)
					{
						text6 = "";
						if (text != string.Empty)
						{
							text6 = "\n";
						}
						text6 += "MENU_SECRETCRACKED".T(Array.Empty<string>());
					}
				}
				string text7 = SHSharp::StringScrambler.GetScrambledSimply(SHSharp::StringScrambler.longfiller + SHSharp::StringScrambler.longfiller + SHSharp::StringScrambler.longfiller);
				for (int m = 0; m < 15; m++)
				{
					text7 = text7.Insert(UnityEngine.Random.Range(0, text7.Length - 1), dictionary[UnityEngine.Random.Range(0, dictionary.Count)]);
				}
				if (text4 == "\n\n")
				{
					text4 = "";
				}
				string text8 = text + text6;
				if (text8.Length > 0)
				{
					text8 = "\n\n" + text8 + "\n\n";
				}
				return string.Concat(new string[]
				{
				text2,
				SHSharp::StringScrambler.GetScrambledSimply(text3),
				text4,
				text5,
				text8,
				text7
				});
			}
			string text9 = SHSharp::StringScrambler.GetGlitchString(576);
			int num5 = 25;
			for (int n = 0; n < num5; n++)
			{
				text9 = text9.Insert(UnityEngine.Random.Range(0, text9.Length - 1), dictionary[UnityEngine.Random.Range(0, dictionary.Count)]);
			}
			return text9;
		}

		public static string PrepareLevelDescription(string keywords)
		{
			string text = SHSharp::StringScrambler.GetGlitchString(576);
			List<string> list = keywords.Split(new char[]
			{
			','
			}).ToList<string>();
			int num = 25;
			for (int i = 0; i < num; i++)
			{
				text = text.Insert(UnityEngine.Random.Range(0, text.Length - 1), list[UnityEngine.Random.Range(0, list.Count)]);
			}
			return text;
		}

		public static void PrepareLevelCommanderButtonForLevel(SHSharp::SHGUIcommanderbutton b, SHSharp::LevelInfo Lvl, SHSharp::SHGUIcommanderview parent, bool isLast = false, string customName = "", bool isChallenge = false)
        {
			if (Lvl.SceneFileName == "foo")
			{
				b.ButtonText = "------------│--------";
				b.SetData("");
				b.RefreshText();
				b.SetOnActivate(delegate (SHSharp::SHGUIcommanderbutton x) { });
				b.SetListLink(parent);
				b.SetLocked(true);
				return;
			}

			string newTitle = Lvl.UniqueSHName.PadRight(12, ' ');
			if (newTitle.Length > 12)
			{
				newTitle = newTitle.Substring(0, 12);
			}

			newTitle += ("│" + "MENU_LEVEL8CHARS".T(Array.Empty<string>()));

			string descExtra = "This is the last level you played.";
			string desc = PrepareLevelDescription(Lvl.Tags.ToList<string>(), Lvl);
			
			if (isLast) 
			{
				desc = descExtra + desc.Substring(0, desc.Length - descExtra.Length);
			}

			if (Lvl.gameTimePar != -1f)
			{
				Lvl.LoadLevelTime();
			}

			b.SetListLink(parent);
			b.ButtonText = newTitle;
			b.SetData(desc);
			b.RefreshText();
			b.LevelToBeLoaded = Lvl.SceneFileName;
			b.LevelToBeLoadedName = Lvl.UniqueSHName;

			if (Lvl.AdditionalComponent != null)
			{
				b.AdditionalComponent = Lvl.AdditionalComponent;
			}

			bool ctp = Lvl.ChallengesTimesPars != null;
			bool cc = SHSharp::GameplayModifiers.CurrentChallenge != null;
			SHSharp::LevelInfo.TimeInfo LevelInfo = Lvl.LoadLevelTime();

			if (!isChallenge)
			{
				if (Lvl.Secrets > 0 && Lvl.SecretsFound() == Lvl.Secrets)
				{
					b.AddScrollingNotification("MENU_CRACKED8CHARS".T(Array.Empty<string>()).PadRight(8), false);
				}
			}
			else if (cc && LevelInfo.challengesTime.ContainsKey(SHSharp::GameplayModifiers.CurrentChallenge.ID))
			{
				float num = LevelInfo.challengesTime[SHSharp::GameplayModifiers.CurrentChallenge.ID];
				float num2 = 0f;
				if (ctp && Lvl.ChallengesTimesPars.ContainsKey(SHSharp::GameplayModifiers.CurrentChallenge.ID))
				{
					num2 = Lvl.ChallengesTimesPars[SHSharp::GameplayModifiers.CurrentChallenge.ID];
				}
				if (num2 > 0f)
				{
					if (num < num2)
					{
						b.AddScrollingNotification("MENU_REDTIME8CHARS".T(Array.Empty<string>()).PadRight(8), false);
					}
					else if (UnityEngine.Random.value > 0.95f)
					{
						b.AddScrollingNotification(naggingFlavours[UnityEngine.Random.Range(0, naggingFlavours.Length)], true);
					}
				}
			}

			b.SetOnActivate(delegate (SHSharp::SHGUIcommanderbutton x)
			{
				SHSharp::LevelInfo levelInfo = SHSharp::LevelSetup.GetLevelInfoByUniqueSHName(Lvl.UniqueSHName);
				if (levelInfo == null)
				{
					levelInfo = SHSharp::LevelSetup.GetLevelInfoBySceneFileName(Lvl.SceneFileName, -1);
				}
				SHSharp::LevelSetup.CurrentLevelInfo = levelInfo;
				SHSharp::SHGUI.current.LaunchLevelAppTunnels(levelInfo, true, false);
			});

			b.Refresh();
		}
	}

	public class Levels
    {
		public Levels(SHSharp::SHGUIcommanderview parent)
        {
			new Folder("LEVELS", parent, PressCommand(parent));
		}

		public static void AppendLevelData(SHSharp::SHGUIcommanderview l, List<SHSharp::LevelInfo> levels, bool isChallenge = false)
		{
			foreach (SHSharp::LevelInfo levelInfo in levels)
			{
				if (!levelInfo.IsIntermission && (SHSharp::GameplayModifiers.CurrentChallenge == null || (!levelInfo.IgnoreInChallenges && !levelInfo.ExcludeFromChallenges.Contains(SHSharp::GameplayModifiers.CurrentChallenge.ID))))
				{
					SHSharp::SHGUIcommanderbutton button = new SHSharp::SHGUIcommanderbutton("", 'w');
					button.SetListLink(l);
					Last.PrepareLevelCommanderButtonForLevel(button, levelInfo, l);
					l.AddButtonView(button);
				}
			}
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
        {
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "LEVELS");

				Folder.UpFolder(view);

				AppendLevelData(view, SHSharp::LevelSetup.Levels, false);
				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}
	}

	public class Endless
    {
		public Endless(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("ENDLESS", parent, PressCommand(parent));
        }

        public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
        {
            return (SHSharp::SHGUIcommanderbutton button) =>
            {
                FolderView view = new FolderView(folderParent, "ENDLESS");

				Folder.UpFolder(view);

				new ReadME("readme.txt", "FolderStructure.endless.item.A_HIGH_SCORE_BASED_ARCADE_VERSION", view);

                Levels.AppendLevelData(view, SHSharp::LevelSetup.EndlessLevels, false);
                SHSharp::SHGUI.current.AddViewOnTop(view);
            };
        }
    }

    public class Challenges
	{
		public Challenges(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("CHALLENGES", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "CHALLENGES");

				Folder.UpFolder(view);

				new ReadME("readme.txt", "CHALLENGES\n\n\n\nAll the story levels of SUPERHOT...WITH A TWIST.\n\n\n\nMANY.\n\nTWISTS.\n\n\n\nAn awesome, timed SPEEDRUN mode!\n\nLong awaited KATANA ONLY challenge.\n\nHarcore HARDMODE for the best.\n\n\n\nHave fun.", view);

				PrepareChallengesList(view, GetGameData().Descendants("Challenges").First<XElement>());
				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}

		public static XDocument GetGameData()
        {
			TextAsset textAsset = Resources.Load<TextAsset>("GameData");
			return XDocument.Parse(textAsset.text);
		}

		public static void PrepareChallengesList(SHSharp::SHGUIcommanderview l, XElement baseNode = null)
		{
			if (SHSharp::GameplayModifiers.CurrentChallenge != null)
			{
				SHSharp::GameplayModifiers.ResetChallenge();
			}
			foreach (XElement xelement in baseNode.Elements())
			{
				bool flag = true;
				string text = xelement.Attribute("name").Value.T(Array.Empty<string>());
				string originalName = text;
				if (SHSharp::SettingsInit.IsHalloween || !(originalName == "PUMPKIN"))
				{
					int requiredChallenge = -1;
					int num = -1;
					if (xelement.Attribute("unlockRequirementChallengeID") != null)
					{
						requiredChallenge = int.Parse(xelement.Attribute("unlockRequirementChallengeID").Value);
					}
					if (xelement.Attribute("unlockRequirementChallengeProgress") != null)
					{
						num = int.Parse(xelement.Attribute("unlockRequirementChallengeProgress").Value);
					}
					if (requiredChallenge != -1 && num != -1)
					{
						int num2 = -1;
						int num3 = -1;
						SHSharp::GameplayModifiers.AllChallenges.Find((SHSharp::ChallengeInfo c) => c.ID == requiredChallenge).GetChallengeProgress(out num3, out num2);
						flag = (num2 >= num);
					}

					flag = true;

					while (text.Length < 12)
					{
						text += " ";
					}
					if (!flag)
					{
						text = SHSharp::StringScrambler.GetScrambledString(text, 0.9f, "▀▄█▌▐░▒▓■▪01 ");
					}
					if (text.Length > 12)
					{
						text = text.Substring(0, 12);
					}
					int challengeID = int.Parse(xelement.Attribute("id").Value);
					SHSharp::ChallengeInfo challengeData = SHSharp::GameplayModifiers.AllChallenges.Find((SHSharp::ChallengeInfo c) => c.ID == challengeID);
					int num4 = 0;
					int num5 = 0;
					challengeData.GetChallengeProgress(out num5, out num4);
					int num6 = 0;
					int num7 = 0;
					challengeData.GetChallengeRedTimesProgress(out num7, out num6);
					text = text + "│" + "MENU_FOLDER8CHARS".T(Array.Empty<string>());
					SHSharp::SHGUIcommanderbutton shguicommanderbutton = new SHSharp::SHGUIcommanderbutton(text, 'w', null).SetListLink(l);
					string text2;
					
						text2 = xelement.Value.T(Array.Empty<string>());
						string str = "SHSharp::piOsMenu.finishedLevels.11.FINISHED_LEVELS__0__1".T(Array.Empty<string>());
						text2 += string.Format("\n\n" + str + "\n", num4, num5);
						if (num7 > 0 && challengeData.ID != 7 && challengeData.ID != 8)
						{
							string str2 = "SHSharp::piOsMenu.achievedRedTimes.12.ACHIEVED_RED_TIMES__0__1".T(Array.Empty<string>());
							text2 += string.Format(str2 + "\n", num6, num7);
						}
						string text3 = ((int)((float)(num4 + num6) / (float)(num5 + num7) * 100f)).ToString() + "%";
						if (text3.Length == 2)
						{
							shguicommanderbutton.AddScrollingNotification("----" + text3 + "->", false);
						}
						else if (text3.Length == 3)
						{
							shguicommanderbutton.AddScrollingNotification("---" + text3 + "->", false);
						}
						else if (text3.Length == 4)
						{
							shguicommanderbutton.AddScrollingNotification("--" + text3 + "->", false);
						}
						shguicommanderbutton.SetOnActivate(delegate (SHSharp::SHGUIcommanderbutton x)
						{
							SHSharp::GameplayModifiers.CurrentChallenge = challengeData;
							if (SHSharp::GameplayModifiers.CurrentChallenge.ID == 111)
							{
								SHSharp::SHGUI.current.LaunchLevelAppTunnels(SHSharp::LevelSetup.GetLevelInfoBySceneFileName("CezaryAddictIntro_P", -1), true, false);
								return;
							}
							XElement xelement2 = new XElement(GetStoryLevelsNode());
							xelement2.Name = originalName;

							FolderView view = new FolderView(l, originalName);

							Folder.UpFolder(view);

							Levels.AppendLevelData(view, SHSharp::LevelSetup.Levels, true);

							SHSharp::SHGUI.current.AddViewOnTop(view);
						});
					shguicommanderbutton.SetData(text2);
					l.AddButtonView(shguicommanderbutton);
				}
			}
		}

		public static XElement GetStoryLevelsNode()
        {
			string text = Resources.Load("FolderStructure").ToString();
			XDocument DATA = XDocument.Parse(text);
			return DATA.Root.Descendants("levels").ToList<XElement>()[0];
		}

		public static XDocument GetData()
        {
			string text = Resources.Load("FolderStructure").ToString();
			return XDocument.Parse(text);
		}
	}

	public class Mods
	{
		public Mods(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("Mods", parent, PressCommand(parent));
			//MelonLogger.Msg(Challenges.GetData().ToString());
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "Mods");

				Folder.UpFolder(view);

				new ReadME("readme.txt", "FolderStructure.mods.item.UNLOCK_MODIFIERS_BY_SCORING_KILLS", view);

				AppendGameDataListFromNode(view, Challenges.GetGameData().Descendants("Mods").First<XElement>());

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}

		public static void AppendGameDataListFromNode(SHSharp::SHGUIcommanderview l, XElement baseNode = null)
		{
			if (baseNode == null)
			{
				baseNode = Challenges.GetData().Root;
			}
			SHSharp::SHGUIcommanderbutton shguicommanderbutton = null;
			using (IEnumerator<XElement> enumerator = baseNode.Elements().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					XElement element = enumerator.Current;
					XElement element2 = element;
					if (element.Name.ToString() == "Level")
					{
						if (element.Attribute("intermission") == null && (SHSharp::GameplayModifiers.CurrentChallenge == null || element.Attribute("ignoreInChallenges") == null))
						{
							PrepareLevelCommanderButtonForXElement(ref shguicommanderbutton, element, l, "");
							l.AddButtonView(shguicommanderbutton);
						}
					}
					else if (element.Name.ToString() == "Mod")
					{
						string text = element.Attribute("name").Value.T(Array.Empty<string>()).ToString();
						while (text.Length < 12)
						{
							text += " ";
						}
						if (text.Length > 12)
						{
							text = text.Substring(0, 12);
						}
						FieldInfo[] fields = typeof(SHSharp::GameplayModifiers).GetFields();
						int num = (int)SHSharp::SaveManager.Instance.GetValue("totalKills", 0);
						bool locked = false;
						string data;
						//if (int.Parse(element.Attribute("killsRequired").Value) <= num && (bool)SHSharp::SaveManager.Instance.GetValue("storyFinished", false))
						if (true)
						{
							data = SHSharp::StringScrambler.GetScrambledSimply("^^^^^^^^^^^^^^^^^^^^^^^^^^\n^^^^^^^^ " + element.Attribute("name").Value.T(Array.Empty<string>()).ToUpper() + ".MOD\n^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\n\n") + SHSharp::StringScrambler.GetScrambledSimply(element2.Value.T(Array.Empty<string>())) + "\n\n" + SHSharp::StringScrambler.GetScrambledSimply(SHSharp::StringScrambler.longfiller + SHSharp::StringScrambler.longfiller + SHSharp::StringScrambler.longfiller + SHSharp::StringScrambler.longfiller);
						}
						else
						{
							locked = true;
							string text2 = "";
							int num2 = (int)(UnityEngine.Random.value * 10f);
							for (int i = 0; i < num2; i++)
							{
								text2 += "^";
							}
							string text7;
							if (int.Parse(element.Attribute("killsRequired").Value) <= 0)
							{
								string text3 = "SHSharp::piOsMenu.finishStoryModeToUnlock0.14.LOCKED".T(Array.Empty<string>());
								string text4 = "SHSharp::piOsMenu.finishStoryModeToUnlock1.15.FINISH".T(Array.Empty<string>());
								string text5 = "SHSharp::piOsMenu.finishStoryModeToUnlock2.16.STORY_MODE".T(Array.Empty<string>());
								string text6 = "SHSharp::piOsMenu.finishStoryModeToUnlock3.17.TO_UNLOCK".T(Array.Empty<string>());
								text7 = string.Concat(new string[]
								{
								"^^^^^^^^^^^^^^^^^^^^^",
								text2,
								SHSharp::StringScrambler.filler,
								"^^",
								text3,
								"^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^",
								text2,
								"^^^^^^^^^^^",
								text2,
								text2,
								"^^^^^^^",
								text2,
								text2,
								"^^^^^^^^^",
								text4,
								" ^^^^^^^",
								text2,
								text2,
								" ",
								text5,
								" ^^^^^^",
								text2,
								"^^^^^",
								SHSharp::StringScrambler.filler,
								"^^^^^^^^^^^^^^^^^^^^ ",
								text6,
								"^^^^^^^^^^^^^^^^^",
								text2,
								SHSharp::StringScrambler.filler,
								"^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^"
								});
							}
							else
							{
								string text8 = "SHSharp::piOsMenu.finishScoreMoreKillsToUnlock0.18.LOCKED".T(Array.Empty<string>());
								string text9 = "SHSharp::piOsMenu.finishScoreMoreKillsToUnlock1.19.SCORE".T(Array.Empty<string>());
								string text10 = "SHSharp::piOsMenu.finishScoreMoreKillsToUnlock2.20.MORE_KILLS".T(Array.Empty<string>());
								string text11 = "SHSharp::piOsMenu.finishScoreMoreKillsToUnlock3.21.TO_UNLOCK".T(Array.Empty<string>());
								text7 = string.Concat(new string[]
								{
								"^^^^^^^^^^^^^^^^^^^^^",
								text2,
								SHSharp::StringScrambler.filler,
								"^^",
								text8,
								"^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^",
								text2,
								"^^^^^^^^^^^",
								text2,
								text2,
								"^^^^^^^",
								text2,
								text2,
								"^^^^^^^^^^^^",
								text9,
								" ",
								(int.Parse(element.Attribute("killsRequired").Value) - num).ToString(),
								text2,
								text2,
								" ",
								text10,
								" ^^^^^^",
								text2,
								"^^^^^",
								SHSharp::StringScrambler.filler,
								"^^^^^^^^^^^^^^^^^^^^ ",
								text11,
								"^^^^^^^^^^^^^^^^^",
								text2,
								SHSharp::StringScrambler.filler,
								"^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^"
								});
							}
							data = SHSharp::StringScrambler.GetScrambledSimply(text7);
							string text12 = text;
							new Regex(".");
							text = SHSharp::StringScrambler.GetScrambledString(text12, 0.9f, "▀▄█▌▐░▒▓■▪01 ");
						}
						text += "│>------<";
						shguicommanderbutton = new SHSharp::SHGUIcommanderbutton(text, 'w', null).SetListLink(l).SetData(data);
						shguicommanderbutton.AssignedModifier = fields.ToList<FieldInfo>().Find((FieldInfo field) => field.FieldType == typeof(bool) && field.Name == element.Attribute("boolName").Value);
						shguicommanderbutton.SetLocked(locked);
						shguicommanderbutton.SetActive((bool)SHSharp::SaveManager.Instance.GetValue(shguicommanderbutton.AssignedModifier.Name, false));
						l.AddButtonView(shguicommanderbutton);
					}
				}
			}
			SHSharp::SHGUI.current.views.Add(l);
		}

		public static void PrepareLevelCommanderButtonForXElement(ref SHSharp::SHGUIcommanderbutton b, XElement element, SHSharp::SHGUIcommanderview l, string customName = "")
		{
			string text = (customName == "") ? element.Attribute("name").Value.T(Array.Empty<string>()) : customName;
			while (text.Length < 12)
			{
				text += " ";
			}
			if (text.Length > 12)
			{
				text = text.Substring(0, 12);
			}
			text = text + "│" + "MENU_ITEM8CHARS".T(Array.Empty<string>());
			string data = Last.PrepareLevelDescription(element.Value.T(Array.Empty<string>()));
			float gtime = -1f;
			float btime = -1f;
			if (element.Attribute("gt") != null && float.TryParse(element.Attribute("gt").Value, out gtime))
			{
				btime = SHSharp::LevelSetup.GetLevelInfoBySceneFileName(element.Attribute("scene").Value, -1).LoadLevelTime().gameTime;
			}
			XElement element1 = element;
			b = new SHSharp::SHGUIcommanderbutton(text, 'w', null).SetListLink(l).SetData(data);
			b.LevelToBeLoaded = element1.Attribute("scene").Value;
			XAttribute xattribute = element1.Attribute("addObject");
			if (xattribute != null)
			{
				b.AdditionalComponent = xattribute.Value;
			}
			b.LevelToBeLoadedName = element.Attribute("name").Value.T(Array.Empty<string>());
			SHSharp::SHGUIcommanderbutton shguicommanderbutton = b;
			shguicommanderbutton.GTime = gtime;
			shguicommanderbutton.BTime = btime;
			shguicommanderbutton.SetOnActivate(delegate (SHSharp::SHGUIcommanderbutton x)
			{
				SHSharp::LevelInfo levelInfo = SHSharp::LevelSetup.GetLevelInfoByUniqueSHName(element1.Attribute("name").Value.T(Array.Empty<string>()));
				if (levelInfo == null)
				{
					levelInfo = SHSharp::LevelSetup.GetLevelInfoBySceneFileName(element1.Attribute("scene").Value, -1);
				}
				SHSharp::LevelSetup.CurrentLevelInfo = levelInfo;
				SHSharp::SHGUI.current.LaunchLevelAppTunnels(levelInfo, true, false);
			});
		}
	}

	public class Setting
	{
		public Setting(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("SETTINGS", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "SETTINGS");

				Folder.UpFolder(view);

				new Folder("Controls", view, FolderAppLaunch("APPControlsSettings"), Color.White, "FolderStructure.settings.item.CHANGE_GAME_SETTINGS");
				new Folder("Graphics", view, FolderAppLaunch("APPGraphicsSettings"), Color.White, "FolderStructure.settings.item.CHANGE_GRAPHIC_SETTINGS");
				new Folder("Audio", view, FolderAppLaunch("AppAudioSettings"), Color.White, "FolderStructure.settings.item.CHANGE_AUDIO_SETTINGS");
				new Folder("Physics", view, FolderAppLaunch("APPPhysicSettings"), Color.White, "FolderStructure.settings.item.name.CHANGE_ADVANCED_GAMEPLAY_SETTINGS");

				new ReadME("restart.bat", "FolderStructure.settings.item.LAUNCH_NEW_GAME", view, Color.White, null, Softreset());
				new ReadME("reset.bat", "FolderStructure.settings.item.RESET_PROGRESS", view, Color.White, null, FolderAppLaunch("APPResetStory"));
				new ReadME("hideui.bat", "FolderStructure.settings.item.HIDE_UI", view, Color.White, null, HideUI()).SetData(SHSharp::StringScrambler.GetScrambledSimply("Hide UI:" + (SHSharp::MainDebug.hideUI ? " ON" : " OFF")));
				new ReadME("unlock.bat", "FolderStructure.settings.item.UNLOCK_EVERYTHING_DEBUG_ONLY_REMEMBER", view, Color.White, null, FolderAppLaunch("APPUnlockEverything"));
				new ReadME("godmode.bat", "FolderStructure.settings.item.GOD_MODE", view, Color.White, null, Godmode()).SetData(SHSharp::StringScrambler.GetScrambledSimply("GodMode:" + (SHSharp::MainDebug.godmodeActive ? " ON" : " OFF")));
				new App("speedrun.bat", "APPSetSpeedrunTimer", view);
				//new ReadME("userpick.bat", "FolderStructure.settings.item.USER_PICKER", view, Color.White, null, FolderAppLaunch("APPResetStory")).SetData(SHSharp::StringScrambler.GetScrambledSimply("Hide UI:" + (SHSharp::MainDebug.hideUI ? " ON" : " OFF")));

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}

		public static Action<SHSharp::SHGUIcommanderbutton> FolderAppLaunch(string app)
        {
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				SHSharp::SHGUI.current.LaunchAppByName(app);
			};
		}

		public static Action<SHSharp::SHGUIcommanderbutton> Softreset()
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				SHSharp::SHGUIview v = new SHSharp::SHGUIview();
				v.dontDrawViewsBelow = false;
				SHSharp::SHGUIchoice shguichoice = new SHSharp::SHGUIchoice("Confirmation_RESTART.BAT".T(Array.Empty<string>()), 32, 10);
				shguichoice.SetOnYes(delegate
				{
					SHSharp::SHGUI.current.LaunchAppByName("APPResetStory");
					SHSharp::SaveManager.Instance.SetControlsData(false);
					SHSharp::SaveManager.Instance.SetSettingsData();
				});
				shguichoice.SetOnNo(delegate
				{
					v.Kill();
				});
				v.AddSubView(shguichoice);
				SHSharp::SHGUI.current.AddViewOnTop(v);
			};
		}

		public static Action<SHSharp::SHGUIcommanderbutton> HideUI()
		{
			return (SHSharp::SHGUIcommanderbutton x) =>
			{
				SHSharp::MainDebug.hideUI = !SHSharp::MainDebug.hideUI;
				x.SetData("Hide UI: " + (SHSharp::MainDebug.hideUI ? "ON" : "OFF"));
				x.RefreshRightPanel();
			};
		}

		public static Action<SHSharp::SHGUIcommanderbutton> Godmode()
		{
			return (SHSharp::SHGUIcommanderbutton x) =>
			{
				SHSharp::MainDebug.godmodeActive = !SHSharp::MainDebug.godmodeActive;
				x.SetData("GodMode : " + (SHSharp::MainDebug.godmodeActive ? "ON" : "OFF"));
				x.RefreshRightPanel();
			};
		}
	}

	public class Apps
	{
		public Apps(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("APPS", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "APPS");

				Folder.UpFolder(view);

				new App("carpets.exe", "APPcarpets", view);
				new App("cartrip.exe", "APPautocarpets", view);
				new App("ggroups.exe", "APPstreamchat", view);

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}
	}

	public class Demos
	{
		public Demos(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("DEMOS", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "DEMOS");

				Folder.UpFolder(view);

				new App("sinus1.exe", "APPsinus", view);
				new App("sinus2.exe", "APPsinus2", view);
				new App("sinus3.exe", "APPsinus3", view);
				new App("starts.exe", "APPstars", view);
				new App("mirrors.exe", "APPmirrors", view);
				new App("letters.exe", "APPletters", view);
				new App("fire.exe", "APPfire", view);
				new App("rotozoom.exe", "APProtozoomer", view);
				new App("pipes.exe", "APPpipes", view);
				new App("rotoline.exe", "APProtolines", view);

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}
	}

	public class Cellular
	{
		public Cellular(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("CELLULAR", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "CELLULAR");

				Folder.UpFolder(view);

				new App("life.exe", "APPConway", view);
				new App("ant.exe", "APPAnt", view);
				new App("forest.exe", "APPForest", view);
				new App("sand.exe", "APPsandbox", view);
				new App("water.exe", "APPwaterbox", view);

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}
	}

	public class Wires
	{
		public Wires(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("WIRES", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "WIRES");

				Folder.UpFolder(view);

				new ReadME("wires.exe", "", view, Color.White, null, WirCommand("schemEmpty"));
				new ReadME("diodes.wir", "", view, Color.White, null, WirCommand("schemDiodes"));
				new ReadME("repeater.wir", "", view, Color.White, null, WirCommand("schemRepeater"));
				new ReadME("gateand1.wir", "", view, Color.White, null, WirCommand("schemANDbasic"));
				new ReadME("gateand2.wir", "", view, Color.White, null, WirCommand("schemANDbasic2"));
				new ReadME("gateand3.wir", "", view, Color.White, null, WirCommand("schemANDbasic3"));
				new ReadME("bigand.wir", "", view, Color.White, null, WirCommand("schemAND"));
				new ReadME("gateor1.wir", "", view, Color.White, null, WirCommand("schemORbasic"));
				new ReadME("gateor2.wir", "", view, Color.White, null, WirCommand("schemORbasic2"));
				new ReadME("gateor3.wir", "", view, Color.White, null, WirCommand("schemORbasic3"));
				new ReadME("bigor.wir", "", view, Color.White, null, WirCommand("schemOR"));
				new ReadME("gatexor1.wir", "", view, Color.White, null, WirCommand("schemXORbasic"));
				new ReadME("gatexor2.wir", "", view, Color.White, null, WirCommand("schemXORbasic2"));
				new ReadME("gatexor3.wir", "", view, Color.White, null, WirCommand("schemXORbasic3"));
				new ReadME("bigxor.wir", "", view, Color.White, null, WirCommand("schemXOR"));
				new ReadME("neon.wir", "", view, Color.White, null, WirCommand("schemNeon"));

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}

		public static Action<SHSharp::SHGUIcommanderbutton> WirCommand(string wirname)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				SHSharp::SHGUI.current.ShowWiresSchem(wirname);
			};
		}
	}

	public class Games
	{
		public Games(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("GAMES", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "GAMES");

				Folder.UpFolder(view);

				new App("treedude.exe", "APPtreedudeintro", view);
				new App("shrl.exe", "APPshrlgame", view);
				new App("2048.exe", "APP2048", view);
				new App("flappy.exe", "APPFlappy", view);
				new App("hotslot.exe", "APPHotSlot", view);
				new App("hottrain.exe", "APPFroger", view);
				new App("logo.exe", "APPOnionMilkLogo", view, Color.White, "This game straight up doesn't exist. 'APPOnionMilkLogo'");
				new App("maze.exe", "APPColorMaze", view);
				new App("onionman.exe", "APPOnionMan", view);
				new App("rogue.exe", "APPRogue", view);
				new App("popup.exe", "APPpopup", view);
				new Slash(view);
				new West(view);
				new Idle(view);
				new Pong(view);

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}
	}

	public class VR
	{
		public VR(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("VR", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "VR");

				Folder.UpFolder(view);

				new App("vr.exe", "APPvr", view);
				new App("cube.exe", "APPcube", view);
				new App("cylinder.exe", "APPcylinder", view);
				new App("capsule.exe", "APPcapsule", view);

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
        }
    }

    public class Arts
    {
        public Arts(SHSharp::SHGUIcommanderview parent)
        {
            new Folder("ART", parent, PressCommand(parent));
        }

        public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
        {
            return (SHSharp::SHGUIcommanderbutton button) =>
            {
                FolderView root = new FolderView(folderParent, "ART");

                Folder.UpFolder(root);

                new Folder("OTHER", root, (SHSharp::SHGUIcommanderbutton _) =>
                {
                    FolderView other = new FolderView(folderParent, "OTHER");
                    Folder.UpFolder(other);

                    new Folder("SOMETHING1", other, EmptyWithUp("SOMETHING1", other));

                    new Folder("SOMETHING2", other, (SHSharp::SHGUIcommanderbutton __) =>
                    {
                        FolderView smt2 = new FolderView(folderParent, "SOMETHING2");
                        Folder.UpFolder(smt2);

						string smrm = "Files sweet files. That's where I keep all my things.\n\nI like to place things in folders, it makes me feel clean.\n\nI also like the silliness of writing notes to myself. Like anyone would read that, lol.";

                        new ReadME("readme.txt", smrm, smt2);

                        SHSharp::SHGUI.current.AddViewOnTop(smt2);
                    });

					new ArtSomething3(other);

					new Folder("RANDOM", other, (SHSharp::SHGUIcommanderbutton __) =>
					{
						FolderView rand = new FolderView(folderParent, "RANDOM");
						Folder.UpFolder(rand);

						new Folder("HYPNO", rand, (SHSharp::SHGUIcommanderbutton ___) =>
						{
							FolderView hypno = new FolderView(folderParent, "HYPNO");
							Folder.UpFolder(hypno);

							new App("diy.exe", "APPshclicker", hypno);
							new App("clicker.exe", "APPShClicker2", hypno);

							SHSharp::SHGUI.current.AddViewOnTop(hypno);
						});

						SHSharp::SHGUI.current.AddViewOnTop(rand);
					});

                    SHSharp::SHGUI.current.AddViewOnTop(other);
                });

				string rm = "We would like to fill this\nfolder with LOTS of ASCIIart.\n\nMaybe you would like to\nplace your signature here,\nas a backer? It's an open\nspace for you guys!\nMaximim resolution for art\nis 62x22, so choose your\ncharacters wisely.\nSend your art to:\nascii@superhotgame.com";

				new ReadME("readme.txt", rm, root);
				new Art("super.art", "super", root);
				new Art("super2.art", "supersmall", root);
				new Art("super3.art", "supersmall-3", root);
				new Art("super4.art", "supersmall-4", root);
				new Art("super5.art", "supersmall-5", root);
				new Art("super6.art", "supersmall-7", root);
				new Art("super7.art", "supersmall-9", root);
				new Art("super8.art", "supersmall-11", root);
				new Art("super9.art", "supersmall-13", root);
				new Art("hot.art", "hot", root);
				new Art("hot2.art", "hotsmall", root);
				new Art("hot3.art", "hotsmall-3", root);
				new Art("hot4.art", "hotsmall-4", root);
				new Art("hot5.art", "hotsmall-5", root);
				new Art("hot6.art", "hotsmall-7", root);
				new Art("hot7.art", "hotsmall-9", root);
				new Art("hot8.art", "hotsmall-11", root);
				new Art("hot9.art", "hotsmall-13", root);
				new Art("tri.art", "tri", root);
				new Art("cubes.art", "cubes", root);
				new Art("skull.art", "skull", root);
				new Art("skull2.art", "skull2", root);
				new Art("dog.art", "Dog", root);
				new Art("kickstrt.art", "Addict", root);
				new Art("alley.art", "Dark alley", root);
				new Art("hacker.art", "Hackerroom", root);
				new Art("corridor.art", "MadCorridor", root);
				new Art("drop.art", "Surrender or die", root);
				new Art("subway.art", "Subway station", root);
				new Art("jump.art", "Bunny jump", root);
				new Art("bar.art", "Bar", root);
				new Art("fightclb.art", "Cage fight", root);
				new Art("xu.art", "XU", root);
				new Art("you.art", "YOU", root);
				new Art("pios.art", "piOS_logo", root);
				new Art("brain.art", "Brain", root);
				new App("bday.art", "APPbirthday", root);

				SHSharp::SHGUI.current.AddViewOnTop(root);
            };
        }

        public static Action<SHSharp::SHGUIcommanderbutton> EmptyWithUp(string name, SHSharp::SHGUIcommanderview folderParent)
        {
            return (SHSharp::SHGUIcommanderbutton button) =>
            {
                FolderView view = new FolderView(folderParent, name);

                Folder.UpFolder(view);

                SHSharp::SHGUI.current.AddViewOnTop(view);
            };
        }
    }

	public class Videos
	{
		public Videos(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("VIDEOS", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "VIDEOS");

				Folder.UpFolder(view);

				new Video("yes.avi", "ss10", view);
				new Video("trailer.avi", "ss5", view);
				new Video("rsm.avi", "ss8", view);
				
				new Video("ss.avi", "ss", view);
				new Video("ss2.avi", "ss2", view);
				new Video("ss3.avi", "ss3", view);
				new Video("ss5_vp8.avi", "ss5_vp8", view);
				new Video("ss8_vp8.avi", "ss8_vp8", view);
				new Video("ss10_vp8.avi", "ss10_vp8", view);
				new Video("ss11.avi", "ss11", view);

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}
	}

	public class Tests
	{
		public Tests(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("TESTS", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "TESTS");

				Folder.UpFolder(view);

				new App("mindcomm.exe", "APPdisplaytext", view);
                new ReadME("ingampop.exe", "", view, Color.White, null, (SHSharp::SHGUIcommanderbutton b) => SHSharp::SHGUI.current.AddViewOnTop(new SHSharp::APPingamepopup("this is a popup")));
				new App("prompter.exe", "APPprompter", view);
                //new App("guruchat.exe", "APPguruchat", view);
                new ReadME("betatalk.exe", "", view, Color.White, null, (SHSharp::SHGUIcommanderbutton b) => new SHBetaTalking1a());
                //new App("runqueue.exe", "APPkill", view);
				new App("restrict.exe", "APPrestrict", view);
				//new App("flood.exe", "APPpopupflood", view);
                new ReadME("flood.exe", "", view, Color.White, null, (SHSharp::SHGUIcommanderbutton b) => SHSharp::SHGUI.current.AddViewOnTop(new SHSharp::APPpopupflood(10, "enjoy the popups?")));
                new App("game.exe", "APPtextgame", view);
				new App("roar.exe", "APProar", view);
				new ReadME("luatest.exe", "Launch app and press enter", view, Color.White, null, Lua("luatest"));
				new App("outflow.exe", "APPmindcopyoutflow", view);
				new App("copy.exe", "APPmindcopy", view);
				new App("xcopy.exe", "APPxcopy", view);
				new App("recruit.exe", "APPrecruit", view);
				new App("recruitre.exe", "APPrecruitredirect", view);
				new App("console.exe", "AppSHConsole", view);
				new App("square.exe", "APPsquare", view);
				new App("church.exe", "APPClientChurch", view);
				new App("countdown.exe", "AppCountdown", view);
				//new App("credcons.exe", "APPcreditconsole", view);
				new App("credits1.exe", "APPcredits", view);
				new App("credits2.exe", "APPcredits2", view, 'w', "Wait for it to start");
                //new APPE3Ending("Thanks for playing the SUPERHOT E3 demo.^W3|It's just a first taste of the game.^W3|More is coming soon. In 2015.^W3||In the meantime - check out the endless mode^W3|and have fun with gameplay modifiers.^W3||Press [B] for menu.");
                //new App("e3ending.exe", "APPE3Ending", view);
                new ReadME("e3ending.exe", "", view, Color.White, null, (SHSharp::SHGUIcommanderbutton b) => SHSharp::SHGUI.current.AddViewOnTop(new SHSharp::APPE3Ending("Thanks for playing the SUPERHOT E3 demo.^W3|It's just a first taste of the game.^W3|More is coming soon. In 2015.^W3||In the meantime - check out the endless mode^W3|and have fun with gameplay modifiers.^W3||Press [B] for menu.")));
                //new App("music.exe", "AppMusic", view);
                new ReadME("music.exe", "Can be used to play any audio clip in the game", view, Color.White, null, (SHSharp::SHGUIcommanderbutton b) => SHSharp::SHGUI.current.AddViewOnTop(new SHSharp::AppMusic("halloween_music", true)));
                new App("randchat.exe", "APPrandomchat", view);
				new App("raycast.exe", "AppRaycast", view);
				new App("tunnel.exe", "APPtunnel", view);
				new App("tunnelgen.exe", "APPtunnelsgeneric", view);
				//new App("voices.exe", "APPvoices", view);
				new App("waves.exe", "APPwaves", view);
                //new App("wireworld.exe", "APPwireworld", view);
                //new ReadME("wireworld.exe", "", view, Color.White, null, (SHSharp::SHGUIcommanderbutton b) => SHSharp::SHGUI.current.AddViewOnTop(new SHSharp::APPwireworld("schemNeon")));

                new UserLevel("edittest_lvl", "myeditortest", "TestEditorLevel", view);
				
				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}

		public static Action<SHSharp::SHGUIcommanderbutton> Lua(string file)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				SHSharp::SHGUI.current.LaunchLuaAppByScriptName(file);
			};
		}
	}

	public class Secrets
	{
		public Secrets(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("SECRETS", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "SECRETS");

				Folder.UpFolder(view);

				for (int i = 1; i <= 37; i++)
                {
					new App($"secret{i}.exe", $"APPsecretchat{i}", view);
                }

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}
	}

	public class UserLevel : ReadME
    {
		public UserLevel(string name, string bundle, string scene, SHSharp::SHGUIcommanderview parent) : base(name, "launches user level: " + bundle, parent, Color.White, "-LEVEL->", Launch(bundle, scene))
		{
			this.SetData(this.data + "\n\nIt may be possible to make custom levels by placing bundles in the same directory as the game installation and launching them via bundle and scene name.");
        }

		public static Action<SHSharp::SHGUIcommanderbutton> Launch(string bundle, string scene)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				//SHSharp::SHGUI.current.LaunchUserLevel(bundle, scene);
			};
		}
	}

	public class ArtSomething3
	{
		public ArtSomething3(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("SOMETHING3", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "SOMETHING3");

				Folder.UpFolder(view);

				new ArtSecret(view);

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}
	}

	public class ArtSecret
	{
		public ArtSecret(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("SECRET", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "SECRET");

				Folder.UpFolder(view);

				new ArtPrivate(view);

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}
	}

	public class ArtPrivate
    {
		public ArtPrivate(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("PRIVATE", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "PRIVATE");

				Folder.UpFolder(view);

				new ArtReally(view);

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}
	}

    public class ArtReally
	{
		public ArtReally(SHSharp::SHGUIcommanderview parent)
		{
			new Folder("REALLY", parent, PressCommand(parent));
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(SHSharp::SHGUIcommanderview folderParent)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				FolderView view = new FolderView(folderParent, "REALLY");

				Folder.UpFolder(view);

				new ReadME("porn.txt", "FolderStructure.really.item.WHAT_ARE_YOU_LOOKING_FOR_HERE", view);
				new Art("pinup.art", "pinup", view);
				new Art("krogg.art", "krogg", view);
				new Art("pr0n39.art", "pr0n39", view);

				SHSharp::SHGUI.current.AddViewOnTop(view);
			};
		}
	}

	public class Video : ReadME
	{
		public Video(string name, string vidName, SHSharp::SHGUIcommanderview parent) : base(name, "", parent, Color.White, null, LaunchVid(vidName))
		{
		}

		public static Action<SHSharp::SHGUIcommanderbutton> LaunchVid(string vidName)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				SHSharp::SHGUI.current.ShowVideo(vidName);
			};
		}
	}
	public class Art : ReadME
	{
		public Art(string name, string artName, SHSharp::SHGUIcommanderview parent) : base(name, "", parent, Color.White, null, LaunchArt(name, artName))
		{
		}

		public static Action<SHSharp::SHGUIcommanderbutton> LaunchArt(string name, string artName)
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				SHSharp::SHGUI.current.ShowArtFile(name, artName, false);
			};
		}
	}

	public class FolderView : SHSharp::SHGUIcommanderview
    {
		public FolderView(SHSharp::SHGUIcommanderview parent, string dirname) : base()
        {
			this.isRoot = false;
			this.path = parent.path + dirname + "\\";
        }
    }

	public class Folder : SHSharp::SHGUIcommanderbutton
    {
		public Folder(string title, SHSharp::SHGUIcommanderview parent, Action<SHSharp::SHGUIcommanderbutton> func, char color = 'w', string content = "", string prefix = null)
			: base(title.PadRight(12, ' ').ToUpper() + SolvePrefix(prefix), color, func)
        {
			this.SetListLink(parent);

			content = SHSharp::StringScrambler.GetScrambledSimply(content.T(Array.Empty<string>()).ToString());
			this.SetData(content.Replace(" \n", "‾\n"));

			parent.AddButtonView(this);
		}

		public static string SolvePrefix(string prefix)
		{
			if (!string.IsNullOrEmpty(prefix))
			{
				return "│" + prefix.PadRight(8, ' ');
			}

			return "│" + "MENU_FOLDER8CHARS".T(Array.Empty<string>());
		}

		public static Action<SHSharp::SHGUIcommanderbutton> UpFolderAction()
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				SHSharp::SHGUI.current.PopView();
				SHSharp::GameplayModifiers.ResetChallenge();
			};
		}

		public static string GetUpFolderPrefix()
		{
			return "MENU_UPFOL8CHARS".T(Array.Empty<string>());
		}

		public static Folder UpFolder(SHSharp::SHGUIcommanderview view)
        {
			return new Folder("/..", view, Folder.UpFolderAction(), Color.White, "", Folder.GetUpFolderPrefix());
		}
	}


	public class App : SHSharp::SHGUIcommanderbutton
    {
		public App(string title, string app, SHSharp::SHGUIcommanderview parent, char color = 'w', string content = "", string prefix = null)
			: base(title.PadRight(12, ' ') + ReadME.SolvePrefix(prefix), color, PressCommand(app))
        {
			this.SetListLink(parent);

			content = SHSharp::StringScrambler.GetScrambledSimply(content.T(Array.Empty<string>()).ToString());
			this.SetData(content.Replace(" \n", "‾\n"));

			parent.AddButtonView(this);
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand(string app)
        {
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				SHSharp::SHGUIview view = SHSharp::SHGUI.current.LaunchAppByName(app);
				if (view == null)
				{
					MelonLogger.Error($"App: {app} not found!");
				}
			};
		}
	}

	public class Slash : App
    {
		public Slash(SHSharp::SHGUIcommanderview parent) : base("slasher.exe", "", parent)
        {
			this.SetOnActivate(PressCommand());
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand()
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				SHSharp::SHGUI.current.AddViewOnTop(new APPSlasherIntro());
			};
		}
	}

	public class West : App
	{
		public West(SHSharp::SHGUIcommanderview parent) : base("westdude.exe", "", parent)
		{
			this.SetOnActivate(PressCommand());
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand()
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
				SHSharp::SHGUI.current.AddViewOnTop(new APPWesternIntro());
			};
		}
	}

	public class Idle : App
	{
		public Idle(SHSharp::SHGUIcommanderview parent) : base("idle.exe", "", parent)
		{
			this.SetOnActivate(PressCommand());
		}

		public static Action<SHSharp::SHGUIcommanderbutton> PressCommand()
		{
			return (SHSharp::SHGUIcommanderbutton button) =>
			{
                SHSharp::SHGUI.current.AddViewOnTop(new AppTheSystem());
            };
		}
	}

    public class Pong : App
    {
        public Pong(SHSharp::SHGUIcommanderview parent) : base("pong.exe", "", parent)
        {
            this.SetOnActivate(PressCommand());
        }

        public static Action<SHSharp::SHGUIcommanderbutton> PressCommand()
        {
            return (SHSharp::SHGUIcommanderbutton button) =>
            {
                SHSharp::SHGUI.current.AddViewOnTop(new MCDAPPPong());
            };
        }
    }

    public class ReadME : SHSharp::SHGUIcommanderbutton
    {
		public ReadME(string title, string content, SHSharp::SHGUIcommanderview parent, char color = 'w', string prefix = null, Action<SHSharp::SHGUIcommanderbutton> func = null) : base(title.PadRight(12, ' ') + SolvePrefix(prefix), color, func)
		{
			this.SetListLink(parent);
           
			content = SHSharp::StringScrambler.GetScrambledSimply(content.T(Array.Empty<string>()).ToString());
			content = content.Replace("[RANDOM_TIME_PUN]", GetRandomTimepun());

			this.SetData(content.Replace(" \n", "‾\n"));

			parent.AddButtonView(this);
		}

		public static string SolvePrefix(string prefix)
		{
			if (!string.IsNullOrEmpty(prefix))
            {
				return "│" + prefix.PadRight(8, ' ');
            }

			return "│" + "MENU_ITEM8CHARS".T(Array.Empty<string>());
		}

		public static string GetRandomTimepun()
		{
			IEnumerable<XElement> enumerable = XDocument.Parse(Resources.Load<TextAsset>("GameData").text).Descendants("TimePuns").Descendants("pun");
			XElement[] array = (enumerable as XElement[]) ?? enumerable.ToArray<XElement>();
			return array[UnityEngine.Random.Range(0, array.Count<XElement>())].Value.T(Array.Empty<string>()).Replace("|", "\n") + "\n";
		}
	}

	public static class Color
    {
		public readonly static char Black = 'a';
		public readonly static char Blue = 'b';
		public readonly static char Green = 'g';
		public readonly static char Red = 'r';
		public readonly static char White = 'w';
		public readonly static char Grey = 'z';
	}

    [HarmonyPatch(typeof(SHSharp::piOsMenu), "CreateDirectoryStructure")]
    public static class Patch_CreateDirectoryStructure
    {

        [HarmonyPrefix]
        private static bool Prefix(ref SHSharp::piOsMenu __instance, List<int> allowedTags, List<int> downloadedTags, float downloadSpeed)
        {
			var assembly = Assembly.GetExecutingAssembly();
			string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("NewFileStructure.txt"));
			string result = null;

			MelonLogger.Msg(resourceName);

			using (Stream stream = assembly.GetManifestResourceStream(resourceName))
			using (StreamReader reader = new StreamReader(stream))
			{
				result = reader.ReadToEnd();
			}
			
            __instance.DATA = XDocument.Parse(result);


            // Assuming GameData loading needs to be patched similarly if it's external, or else left as is if it's internal
            TextAsset gameDataTextAsset = Resources.Load<TextAsset>("GameData");
            if (gameDataTextAsset != null)
            {
                __instance.GameData = XDocument.Parse(gameDataTextAsset.text);
            }
            else
            {
                MelonLogger.Error("Failed to load GameData TextAsset!");
                // Handle error appropriately, possibly return true to revert to original logic
            }

            // Continue with your patched logic...
            MethodInfo createViewFromNodeMethod = AccessTools.Method(__instance.GetType(), "CreateViewFromNode");
            if (createViewFromNodeMethod != null)
            {
                createViewFromNodeMethod.Invoke(__instance, new object[] { null, allowedTags, downloadedTags, downloadSpeed, false });
            }
            else
            {
                MelonLogger.Error("Failed to find the CreateViewFromNode method.");
            }

            return false; // Skip the original method after our custom logic
        }

	}
}
