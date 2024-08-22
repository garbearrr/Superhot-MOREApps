using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using MelonLoader;
using UnityEngine;

namespace MCD.SHGUI__
{
	/*public class MCDSHGUI : MonoBehaviour
	{
		// Token: 0x06001FF2 RID: 8178 RVA: 0x000EF298 File Offset: 0x000ED698
		private void Start()
		{
			this.Colors = default(MCDSHGUI.SHGUIColors);
			this.Colors.SetupPalette();
			this.input = new SHInputGUI();
			int num = this.resolutionX * this.resolutionY;
			MCDSHGUI.current = this;
			this.display = new char[num];
			this.color = new char[num];
			this.background = new char[num];
			this.backgroundColor = new char[num];
			this.Clear();
			if (!MCDSHGUI.shadersLoaded)
			{
				Shader.WarmupAllShaders();
				MCDSHGUI.shadersLoaded = true;
			}
			this.str = new StringBuilder(this.resolutionX * this.resolutionY * 2);
			AudioManager.Instance.AudioFadeIntBGFader();
			MCDSHGUI.figletDictionary[FigletFontType.Alligator] = new Figlet("Fonts/alligator");
			MCDSHGUI.figletDictionary[FigletFontType.Alphabet] = new Figlet("Fonts/Alphabet");
			MCDSHGUI.figletDictionary[FigletFontType.Banner3] = new Figlet("Fonts/banner3");
		}

		// Token: 0x06001FF3 RID: 8179 RVA: 0x000EF38C File Offset: 0x000ED78C
		private void Update()
		{
			this.internalUpdate();
		}

		// Token: 0x06001FF4 RID: 8180 RVA: 0x000EF394 File Offset: 0x000ED794
		private void internalUpdate()
		{
			if (Time.unscaledDeltaTime < 0.3f)
			{
				this.scriptsLoaded--;
			}
			if (PlayerController.Instance != null && PlayerController.Instance.velocity == Vector3.zero)
			{
				this.actualDelay += Time.unscaledDeltaTime;
			}
			else if (PlayerController.Instance != null && PlayerController.Instance.velocity != Vector3.zero)
			{
				this.actualDelay = 0f;
			}
			if (this.scriptsLoaded > 0)
			{
				return;
			}
			this.PopViewFromQueue();
			this.VisibleViewCount = 0;
			for (int i = 0; i < this.views.Count; i++)
			{
				if (!this.views[i].hidden && !(this.views[i] is AppHotswitch) && !(this.views[i] is SHGUIHotswitchview))
				{
					this.VisibleViewCount++;
				}
			}
			if (this.viewCountOld > 0 || this.VisibleViewCount <= 0 || this.OnScreen)
			{
			}
			if (this.viewCountOld > 0 && this.VisibleViewCount <= 0)
			{
				if (this.OnScreen)
				{
					MenuEffectManager.Instance.ForceMenuBackgroundToTransparentAndUpdateBloom(false);
				}
				if (Crosshair2D.Instance != null && !CyberspaceFlowControl.IsInAsciiOverlay)
				{
					Crosshair2D.Instance.SetEnabled(true);
				}
			}
			this.viewCountOld = this.VisibleViewCount;
			this.waiter--;
			if (this.waiter > 0)
			{
				return;
			}
			this.waiter = 0;
			this.Clear();
			int num = 0;
			for (int j = this.views.Count - 1; j >= 0; j--)
			{
				if (this.views[j].dontDrawViewsBelow)
				{
					num = j;
					break;
				}
			}
			this.anyVisiblePreviousFrame = this.anyVisible;
			this.anyVisible = false;
			for (int k = num; k < this.views.Count; k++)
			{
				this.views[k].Update();
				this.views[k].FadeUpdate(1f, 1f);
				this.views[k].Redraw(0, 0);
				if (!this.views[k].hidden)
				{
					this.anyVisible = true;
				}
			}
			this.ReactToInputKeyboard(Time.unscaledDeltaTime);
			this.ReactToInputMouse();
			for (int l = this.views.Count - 1; l >= 0; l--)
			{
				if (this.views[l].remove)
				{
					this.views.RemoveAt(l);
					break;
				}
			}
			MCDSHGUIview interactableView = this.GetInteractableView();
			int num2 = this.cursorY;
			int num3 = this.cursorX;
			int num4 = (int)(Input.mousePosition.x / (float)Screen.width * 64f);
			int num5 = (int)(((float)Screen.height - Input.mousePosition.y) / (float)Screen.height * 24f);
			this.cursorX = num4;
			this.cursorY = num5;
			this.mouseX += Input.GetAxis("mouse x");
			this.mouseY -= Input.GetAxis("mouse y");
			if (this.alterCursor)
			{
				Cursor.visible = false;
			}
			this.cursorX = Mathf.Clamp(this.cursorX, 0, 79);
			this.cursorY = Mathf.Clamp(this.cursorY, 0, 23);
			if (num2 != this.cursorY || num3 != this.cursorX)
			{
				this.cursorActive = true;
				this.cursorTimer = 5f;
				if (!this.cursorVirgin && interactableView != null)
				{
					interactableView.SpeedUpFadeIn();
				}
				this.cursorVirgin = false;
			}
			this.cursorTimer -= Time.unscaledDeltaTime;
			if (this.cursorTimer < 0f)
			{
				this.cursorActive = false;
			}
			if (this.cursorActive && interactableView != null && interactableView.allowCursorDraw)
			{
				this.cursorAnimatorTimer -= Time.unscaledDeltaTime;
				if (this.cursorAnimatorTimer < 0f)
				{
					this.cursorAnimatorTimer = 0.02f;
					this.cursorAnimator++;
					if (this.cursorAnimator > this.cursorAnimation.Length - 1)
					{
						this.cursorAnimator = 0;
					}
				}
				if (this.stringContainsChar(this.GetPixelFront(this.cursorX, this.cursorY), "▀▄█▌░▒▓■▪"))
				{
					MCDSHGUI.current.SetPixelFront(this.cursorAnimation[this.cursorAnimator], this.cursorX, this.cursorY, 'r');
				}
				else
				{
					MCDSHGUI.current.SetPixelBack(this.cursorAnimation[this.cursorAnimator], this.cursorX, this.cursorY, 'r');
				}
			}
			if (this.anyVisiblePreviousFrame && !this.anyVisible)
			{
				this.Clear();
				this.Print();
			}
			bool flag = this.anyVisible && this.anyVisiblePreviousFrame && this.DontDrawCounter == 0;
			AsciiText.Instance.FrameBuffer.shouldBeDrawn = flag;
			if (flag)
			{
				this.Print();
			}
			this.DontDrawCounter--;
			if (this.DontDrawCounter < 0)
			{
				this.DontDrawCounter = 0;
			}
			for (int m = num; m < this.views.Count; m++)
			{
				this.views[m].LateUpdate();
			}
			if (this.delayedQueuePop)
			{
				this.delayedQueuePopTime -= Time.unscaledDeltaTime;
				if (this.delayedQueuePopTime < 0f)
				{
					MCDSHGUI.current.PopViewFromQueue();
					this.delayedQueuePop = false;
				}
			}
		}

		// Token: 0x06001FF5 RID: 8181 RVA: 0x000EF9B5 File Offset: 0x000EDDB5
		public float GetScaleFactor()
		{
			return base.gameObject.GetComponent<CanvasScaler>().scaleFactor;
		}

		// Token: 0x06001FF6 RID: 8182 RVA: 0x000EF9C8 File Offset: 0x000EDDC8
		public void PlaySound(SHGUIsound sound)
		{
			switch (sound)
			{
				case SHGUIsound.tick:
					AudioManager.Instance.PlayCursorSound();
					break;
				case SHGUIsound.redtick:
					AudioManager.Instance.PlayClip(AudioResources.Terminal.TickRed, null);
					break;
				case SHGUIsound.ping:
					AudioManager.Instance.PlayClip(AudioResources.NonTerminal.PingNT, null).RandomizeAudioParameters(0.02f, 0.05f, true, true);
					break;
				case SHGUIsound.pong:
					AudioManager.Instance.PlayClip(AudioResources.NonTerminal.PongNT, null).RandomizeAudioParameters(0.03f, 0.05f, true, true);
					break;
				case SHGUIsound.redping:
					AudioManager.Instance.PlayClip(AudioResources.Terminal.PingRed, null);
					break;
				case SHGUIsound.redpong:
					AudioManager.Instance.PlayClip(AudioResources.Terminal.PongRed, null);
					break;
				case SHGUIsound.download:
					{
						float newPitch = UnityEngine.Random.Range(0.99f, 1.01f);
						AudioManager.Instance.PlayClip(AudioResources.Terminal.Downloading, null).SetPitch(newPitch);
						break;
					}
				case SHGUIsound.downloaded:
					AudioManager.Instance.PlayClip(AudioResources.Terminal.Downloaded, null);
					break;
				case SHGUIsound.confirm:
					AudioManager.Instance.PlayClip(AudioResources.NonTerminal.EnterNT, null).SetCooldown(0.2f);
					break;
				case SHGUIsound.wrong:
					AudioManager.Instance.PlayClip(AudioResources.Terminal.Error, null);
					break;
				case SHGUIsound.driveloading:
					AudioManager.Instance.PlayClip(AudioResources.Terminal.Ping, null);
					break;
				case SHGUIsound.incomingmessage:
					AudioManager.Instance.PlayClip(AudioResources.Terminal.MessageIncoming, null);
					break;
				case SHGUIsound.finalscramble:
					AudioManager.Instance.PlayClip(AudioResources.Terminal.ScrambleFinal, null);
					break;
				case SHGUIsound.restrictedpopup:
					AudioManager.Instance.PlayClip(AudioResources.Terminal.Error, null);
					break;
				case SHGUIsound.messageswitch:
					AudioManager.Instance.PlayClip(AudioResources.Terminal.MessageSwap, null);
					break;
				case SHGUIsound.noescape:
					AudioManager.Instance.PlayClip(AudioResources.Terminal.PingRed, null);
					break;
				case SHGUIsound.piOSLaunch:
					AudioManager.Instance.PlayClip(AudioResources.Terminal.PiOsLaunch, null);
					break;
				case SHGUIsound.mapCellChange:
					AudioManager.Instance.PlayCursorSound();
					break;
				case SHGUIsound.mapCellWrong:
					AudioManager.Instance.PlayClip(AudioResources.Terminal.Error, null);
					break;
				default:
					SHDebug.Log("No sound defined. See SHGUI", LogPriority.Zero, LogCategory.Other);
					break;
			}
		}

		// Token: 0x06001FF7 RID: 8183 RVA: 0x000EFCF1 File Offset: 0x000EE0F1
		public void ForceNextInQueue()
		{
		}

		// Token: 0x06001FF8 RID: 8184 RVA: 0x000EFCF3 File Offset: 0x000EE0F3
		public void FadeNoiseLoop(float fadeTime = 0.1f, float desVolume = 0f)
		{
			AudioManager.Instance.FadeOutClip(ref this.BackgroundNoiseId, fadeTime, desVolume, true);
		}

		// Token: 0x06001FF9 RID: 8185 RVA: 0x000EFD08 File Offset: 0x000EE108
		public static Vector2Int LocalPosToGlobal(SHGUIview localView, int localX, int localY)
		{
			return MCDSHGUI.LocalPosToGlobal(localView, new Vector2Int(localX, localY));
		}

		// Token: 0x06001FFA RID: 8186 RVA: 0x000EFD18 File Offset: 0x000EE118
		public static Vector2Int LocalPosToGlobal(SHGUIview localView, Vector2Int localPos)
		{
			SHGUIview shguiview = localView;
			while (shguiview.parent != null)
			{
				localPos += new Vector2Int(shguiview.x, shguiview.y);
				shguiview = shguiview.parent;
			}
			return localPos;
		}

		// Token: 0x06001FFB RID: 8187 RVA: 0x000EFD58 File Offset: 0x000EE158
		public static int GetId()
		{
			return MCDSHGUI.id++;
		}

		// Token: 0x06001FFC RID: 8188 RVA: 0x000EFD68 File Offset: 0x000EE168
		public void DarkenViews()
		{
			foreach (MCDSHGUIview shguiview in this.views)
			{
				shguiview.Darken();
			}
		}

		// Token: 0x06001FFD RID: 8189 RVA: 0x000EFDC4 File Offset: 0x000EE1C4
		public void BrightenViews()
		{
			foreach (MCDSHGUIview shguiview in this.views)
			{
				shguiview.Brighten();
			}
		}

		// Token: 0x06001FFE RID: 8190 RVA: 0x000EFE20 File Offset: 0x000EE220
		public void TurnBackgroundOn(bool doReset = true)
		{
			if (PlayerActions.CURRENT != null)
			{
				PlayerActions.CURRENT.enabled = false;
				PlayerActions.CURRENT.GetComponent<PlayerController>().enabled = false;
			}
			if (doReset)
			{
				CameraEffectsManager.Instance.ResetAll();
			}
			this.OnScreen = true;
		}

		// Token: 0x06001FFF RID: 8191 RVA: 0x000EFE70 File Offset: 0x000EE270
		public void AddViewOnTop(SHGUIview view)
		{
			if (this.views.Count > 0)
			{
				SHGUIview shguiview = this.views[this.views.Count - 1];
				if (shguiview != null)
				{
					shguiview.OnExit();
				}
			}
			if (this.GetInteractableView() == null)
			{
				this.views.Add(view);
			}
			else
			{
				this.views.Insert(this.GetInteractableViewIndex() + 1, view);
			}
			view.OnEnter();
		}

		// Token: 0x06002000 RID: 8192 RVA: 0x000EFEEB File Offset: 0x000EE2EB
		public void AddViewAtBottom(SHGUIview view)
		{
			this.views.Insert(0, view);
			this.views[0].OnEnter();
		}

		// Token: 0x06002001 RID: 8193 RVA: 0x000EFF0B File Offset: 0x000EE30B
		public void AddViewToQueue(SHGUIview view)
		{
			this.viewQueue.Add(view);
		}

		// Token: 0x06002002 RID: 8194 RVA: 0x000EFF19 File Offset: 0x000EE319
		public void AddViewToQueueFront(SHGUIview view)
		{
			this.viewQueue.Insert(0, view);
		}

		// Token: 0x06002003 RID: 8195 RVA: 0x000EFF28 File Offset: 0x000EE328
		public void PopView()
		{
			if (this.views.Count > 0 && this.views.Last<SHGUIview>() != null)
			{
				this.views.Last<SHGUIview>().OnExit();
			}
			bool flag = false;
			SHGUIview interactableView = this.GetInteractableView();
			if (interactableView != null)
			{
				flag = !interactableView.dontDrawViewsBelow;
				interactableView.Kill();
			}
			if (this.views.Count == 0)
			{
				this.PopViewFromQueue();
			}
			interactableView = this.GetInteractableView();
			if (interactableView != null)
			{
				if (!flag)
				{
					this.GetInteractableView().PunchIn(0f);
				}
				else
				{
					this.GetInteractableView().PunchIn(0.9f);
				}
				this.GetInteractableView().OnPop();
			}
			SHInput.ClearInputState();
		}

		// Token: 0x06002004 RID: 8196 RVA: 0x000EFFE4 File Offset: 0x000EE3E4
		public SHGUIview PopViewFromQueue()
		{
			if (this.viewQueue.Count > 0)
			{
				SHGUIview shguiview = this.viewQueue[0];
				this.AddViewOnTop(shguiview);
				this.viewQueue.RemoveAt(0);
				return shguiview;
			}
			this.finished = true;
			return null;
		}

		// Token: 0x06002005 RID: 8197 RVA: 0x000F002C File Offset: 0x000EE42C
		public void KillAll()
		{
			for (int i = 0; i < this.views.Count; i++)
			{
				this.views[i].Kill();
			}
		}

		// Token: 0x06002006 RID: 8198 RVA: 0x000F0068 File Offset: 0x000EE468
		public void KillAllInstant()
		{
			for (int i = 0; i < this.views.Count; i++)
			{
				this.views[i].KillInstant();
			}
		}

		// Token: 0x06002007 RID: 8199 RVA: 0x000F00A4 File Offset: 0x000EE4A4
		public SHGUIview LaunchAppByName(string name)
		{
			Type type = Type.GetType(name);
			if (type != null)
			{
				SHGUIview shguiview = Activator.CreateInstance(type) as SHGUIview;
				this.AddViewOnTop(shguiview);
				return shguiview;
			}
			//SHDebug.Log("app: " + name + " not found", LogPriority.Zero, LogCategory.Other);
			return null;
		}

		// Token: 0x06002008 RID: 8200 RVA: 0x000F00EC File Offset: 0x000EE4EC
		public void ShowVideo(string videoname)
		{
			this.AddViewOnTop(new APPvideo(videoname, true, 1f, 0f));
		}

		// Token: 0x06002009 RID: 8201 RVA: 0x000F0105 File Offset: 0x000EE505
		public string GetASCIIartByName(string artname, bool defaultPath = true)
		{
			return (!defaultPath) ? Resources.Load(artname).ToString() : Resources.Load("ASCII/" + artname).ToString();
		}

		// Token: 0x0600200A RID: 8202 RVA: 0x000F0134 File Offset: 0x000EE534
		public string GetPaddedAsciiArtByName(string asciiArtName)
		{
			string asciiartByName = this.GetASCIIartByName(asciiArtName, true);
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < asciiartByName.Length; i++)
			{
				if (asciiartByName[i] == '\n')
				{
					for (int j = num; j < MCDSHGUI.current.resolutionX; j++)
					{
						stringBuilder.Append(' ');
					}
					num = 0;
					num2++;
				}
				else
				{
					stringBuilder.Append(asciiartByName[i]);
					num++;
				}
			}
			for (int k = num2; k < MCDSHGUI.current.resolutionY; k++)
			{
				for (int l = num; l < MCDSHGUI.current.resolutionX; l++)
				{
					stringBuilder.Append(' ');
				}
				num = 0;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600200B RID: 8203 RVA: 0x000F0214 File Offset: 0x000EE614
		public string GetPaddedAndCenteredAsciiArtByName(string asciiArtName)
		{
			string asciiartByName = this.GetASCIIartByName(asciiArtName, true);
			SHGUItext shguitext = new SHGUItext(asciiartByName, 1, 1, 'w', false);
			int num = shguitext.CountLines();
			int longestLineLength = shguitext.GetLongestLineLength();
			StringBuilder stringBuilder = new StringBuilder();
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < (MCDSHGUI.current.resolutionY - num) / 2; i++)
			{
				for (int j = num2; j < MCDSHGUI.current.resolutionX; j++)
				{
					stringBuilder.Append(' ');
				}
				num3++;
				num2 = 0;
			}
			for (int k = num3; k < MCDSHGUI.current.resolutionY; k++)
			{
				for (int l = 0; l < (MCDSHGUI.current.resolutionX - longestLineLength) / 2; l++)
				{
					stringBuilder.Append(' ');
					num2++;
				}
				for (int m = (MCDSHGUI.current.resolutionX - longestLineLength) / 2; m < MCDSHGUI.current.resolutionX; m++)
				{
					if (num4 >= asciiartByName.Length)
					{
						break;
					}
					if (asciiartByName[num4] == '\n' || asciiartByName[num4] == '\r')
					{
						for (int n = num2; n < MCDSHGUI.current.resolutionX; n++)
						{
							stringBuilder.Append(' ');
							m++;
						}
						num2 = 0;
						num3++;
						m = MCDSHGUI.current.resolutionX;
						num4++;
						num4++;
					}
					else
					{
						stringBuilder.Append(asciiartByName[num4]);
						num2++;
						num4++;
					}
				}
				num2 = 0;
			}
			for (int num5 = num3; num5 < (MCDSHGUI.current.resolutionY - num) / 2; num5++)
			{
				for (int num6 = num2; num6 < MCDSHGUI.current.resolutionX; num6++)
				{
					stringBuilder.Append(' ');
				}
				num3++;
				num2 = 0;
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600200C RID: 8204 RVA: 0x000F042C File Offset: 0x000EE82C
		public string GetASCIIartFromFont(string text, FigletFontType font = FigletFontType.Banner3)
		{
			Figlet figlet = MCDSHGUI.figletDictionary[font];
			string text2 = figlet.ToAsciiArt(text.Replace('|', '\n'));
			text2 = text2.Replace('A', '█');
			text2 = text2.Replace('B', '█');
			text2 = text2.Replace('C', '█');
			text2 = text2.Replace('D', '█');
			text2 = text2.Replace('E', '█');
			text2 = text2.Replace('F', '█');
			text2 = text2.Replace('G', '█');
			text2 = text2.Replace('H', '█');
			text2 = text2.Replace('I', '█');
			text2 = text2.Replace('J', '█');
			text2 = text2.Replace('K', '█');
			text2 = text2.Replace('L', '█');
			text2 = text2.Replace('M', '█');
			text2 = text2.Replace('N', '█');
			text2 = text2.Replace('O', '█');
			text2 = text2.Replace('P', '█');
			text2 = text2.Replace('Q', '█');
			text2 = text2.Replace('R', '█');
			text2 = text2.Replace('S', '█');
			text2 = text2.Replace('T', '█');
			text2 = text2.Replace('U', '█');
			text2 = text2.Replace('V', '█');
			text2 = text2.Replace('W', '█');
			text2 = text2.Replace('Z', '█');
			text2 = text2.Replace('X', '█');
			text2 = text2.Replace('Y', '█');
			text2 = text2.Replace('a', '█');
			text2 = text2.Replace('b', '█');
			text2 = text2.Replace('c', '█');
			text2 = text2.Replace('d', '█');
			text2 = text2.Replace('e', '█');
			text2 = text2.Replace('f', '█');
			text2 = text2.Replace('g', '█');
			text2 = text2.Replace('h', '█');
			text2 = text2.Replace('i', '█');
			text2 = text2.Replace('j', '█');
			text2 = text2.Replace('k', '█');
			text2 = text2.Replace('l', '█');
			text2 = text2.Replace('m', '█');
			text2 = text2.Replace('n', '█');
			text2 = text2.Replace('o', '█');
			text2 = text2.Replace('p', '█');
			text2 = text2.Replace('q', '█');
			text2 = text2.Replace('r', '█');
			text2 = text2.Replace('s', '█');
			text2 = text2.Replace('t', '█');
			text2 = text2.Replace('u', '█');
			text2 = text2.Replace('v', '█');
			text2 = text2.Replace('w', '█');
			text2 = text2.Replace('z', '█');
			text2 = text2.Replace('x', '█');
			text2 = text2.Replace('y', '█');
			text2 = text2.Replace('0', '█');
			text2 = text2.Replace('1', '█');
			text2 = text2.Replace('2', '█');
			text2 = text2.Replace('3', '█');
			text2 = text2.Replace('4', '█');
			text2 = text2.Replace('5', '█');
			text2 = text2.Replace('6', '█');
			text2 = text2.Replace('7', '█');
			text2 = text2.Replace('8', '█');
			text2 = text2.Replace('9', '█');
			text2 = text2.Replace('.', '█');
			text2 = text2.Replace(',', '█');
			text2 = text2.Replace(':', '█');
			text2 = text2.Replace(';', '█');
			text2 = text2.Replace('+', '█');
			text2 = text2.Replace('$', '█');
			text2 = text2.Replace('#', '█');
			return text2.Replace('\'', '█');
		}

		// Token: 0x0600200D RID: 8205 RVA: 0x000F082C File Offset: 0x000EEC2C
		public SHGUIview DisplayFiglet(string text, float delay, FigletFontType font = FigletFontType.Banner3, bool withOutline = false, SHGUIview parent = null)
		{
			SHGUIview shguiview = new SHGUItempview(delay);
			shguiview.overrideFadeInSpeed = 10.5f;
			shguiview.overrideFadeOutSpeed = 1f;
			shguiview.DisregardParentsFadeSpeeds = true;
			shguiview.dontDrawViewsBelow = false;
			shguiview.allowCursorDraw = false;
			if (parent == null)
			{
				this.AddViewOnTop(shguiview);
			}
			else
			{
				parent.AddSubView(shguiview);
			}
			string asciiartFromFont = MCDSHGUI.current.GetASCIIartFromFont(text, font);
			SHGUItext shguitext = new SHGUItext(asciiartFromFont, 0, 1, 'w', false);
			shguitext.dontDrawViewsBelow = false;
			if (withOutline)
			{
				for (int i = -1; i <= 1; i++)
				{
					for (int j = -1; j <= 1; j++)
					{
						SHGUItext view = new SHGUItext(asciiartFromFont, i, 1 + j, '0', false);
						shguiview.AddSubView(view);
					}
				}
			}
			shguiview.x = MCDSHGUI.current.resolutionX / 2 - shguitext.GetLongestLineLength() / 2 + 1;
			shguiview.y = 8;
			shguiview.AddSubView(shguitext);
			CameraEffectsManager.Instance["TextPunch"].Play();
			CameraEffectsManager.Instance["RedTextPunch"].Play();
			AudioManager.Instance.PlayClip(AudioResources.Terminal.Snapshot, null);
			return shguiview;
		}

		// Token: 0x0600200E RID: 8206 RVA: 0x000F0964 File Offset: 0x000EED64
		public SHGUItext GetCenteredAsciiArt(string artname, int screenPosX = 32, int screenPosY = 12)
		{
			SHGUItext shguitext = new SHGUItext(this.GetASCIIartByName(artname, true), 1, 1, 'w', false);
			shguitext.x = screenPosX - shguitext.GetLongestLineLength() / 2;
			shguitext.y = screenPosY - shguitext.CountLines() / 2;
			return shguitext;
		}

		// Token: 0x0600200F RID: 8207 RVA: 0x000F09A8 File Offset: 0x000EEDA8
		public void Clear()
		{
			int length = this.resolutionX * this.resolutionY;
			Array.Clear(this.display, 0, length);
			Array.Clear(this.background, 0, length);
		}

		// Token: 0x06002010 RID: 8208 RVA: 0x000F09E0 File Offset: 0x000EEDE0
		public void SetPixelFront(char C, int x, int y, char col)
		{
			if (x < 0 || x >= this.resolutionX)
			{
				return;
			}
			if (y < 0 || y >= this.resolutionY)
			{
				return;
			}
			if (C == '\r' || C == '\t')
			{
				C = ' ';
			}
			int num = x + this.resolutionX * y;
			this.display[num] = C;
			this.color[num] = col;
		}

		// Token: 0x06002011 RID: 8209 RVA: 0x000F0A47 File Offset: 0x000EEE47
		public void SetPixelFrontRisky(char fgChar, int pos, char fgColor)
		{
			this.display[pos] = fgChar;
			this.color[pos] = fgColor;
		}

		// Token: 0x06002012 RID: 8210 RVA: 0x000F0A5C File Offset: 0x000EEE5C
		public void SetColorFront(char c, int x, int y)
		{
			if (x < 0 || x >= this.resolutionX)
			{
				return;
			}
			if (y < 0 || y >= this.resolutionY)
			{
				return;
			}
			int num = x + this.resolutionX * y;
			this.color[num] = c;
		}

		// Token: 0x06002013 RID: 8211 RVA: 0x000F0AA8 File Offset: 0x000EEEA8
		public void SetHiFiColorFront(Color c, int x, int y)
		{
			if (x < 0 || x >= this.resolutionX)
			{
				return;
			}
			if (y < 0 || y >= this.resolutionY)
			{
				return;
			}
			Debug.LogError("HIFI COLORS NOT SUPPORTED ANYMORE, TODO CONVERSION FROM HICOLOR TO CHAR");
			int num = x + this.resolutionX * y;
		}

		// Token: 0x06002014 RID: 8212 RVA: 0x000F0AF4 File Offset: 0x000EEEF4
		public char GetColorFront(int x, int y)
		{
			if (x < 0 || x >= this.resolutionX)
			{
				return ' ';
			}
			if (y < 0 || y >= this.resolutionY)
			{
				return ' ';
			}
			int num = x + this.resolutionX * y;
			return this.color[num];
		}

		// Token: 0x06002015 RID: 8213 RVA: 0x000F0B40 File Offset: 0x000EEF40
		public char GetPixelFront(int x, int y)
		{
			if (x < 0 || x >= this.resolutionX)
			{
				return ' ';
			}
			if (y < 0 || y >= this.resolutionY)
			{
				return ' ';
			}
			int num = x + this.resolutionX * y;
			return this.display[num];
		}

		// Token: 0x06002016 RID: 8214 RVA: 0x000F0B8C File Offset: 0x000EEF8C
		public char GetPixelBack(int x, int y)
		{
			if (x < 0 || x >= this.resolutionX)
			{
				return ' ';
			}
			if (y < 0 || y >= this.resolutionY)
			{
				return ' ';
			}
			int num = x + this.resolutionX * y;
			return this.background[num];
		}

		// Token: 0x06002017 RID: 8215 RVA: 0x000F0BD8 File Offset: 0x000EEFD8
		public void SetPixelBack(char fgColor, int x, int y, char bgColor)
		{
			if (x < 0 || x >= this.resolutionX)
			{
				return;
			}
			if (y < 0 || y >= this.resolutionY)
			{
				return;
			}
			if (fgColor == '\r' || fgColor == '\t')
			{
				fgColor = ' ';
			}
			if (bgColor == '\r' || bgColor == '\t')
			{
				bgColor = '0';
			}
			int num = x + this.resolutionX * y;
			this.background[num] = fgColor;
			this.backgroundColor[num] = bgColor;
		}

		// Token: 0x06002018 RID: 8216 RVA: 0x000F0C58 File Offset: 0x000EF058
		public void DrawLine(string style, int startpos, int endpos, int colpos, bool horizontal, char col, float fade = 1f)
		{
			fade += 0.5f;
			fade = Mathf.Clamp01(fade);
			int num = (int)((float)startpos + (float)(endpos - startpos) * (1f - fade));
			int num2 = (int)((float)endpos + (float)(startpos - endpos) * (1f - fade));
			if (num == num2)
			{
				num2--;
				style = "+++";
			}
			if (horizontal)
			{
				for (int i = num; i <= num2; i++)
				{
					char c;
					if (i == num)
					{
						c = style[0];
					}
					else if (i == num2)
					{
						c = style[2];
					}
					else
					{
						c = style[1];
					}
					if (UnityEngine.Random.value < fade)
					{
						this.SetPixelFront(c, i, colpos, col);
					}
				}
			}
			else
			{
				for (int j = num; j <= num2; j++)
				{
					char c;
					if (j == num)
					{
						c = style[0];
					}
					else if (j == num2)
					{
						c = style[2];
					}
					else
					{
						c = style[1];
					}
					if (UnityEngine.Random.value < fade)
					{
						this.SetPixelFront(c, colpos, j, col);
					}
				}
			}
		}

		// Token: 0x06002019 RID: 8217 RVA: 0x000F0D78 File Offset: 0x000EF178
		private void ClearRect(int startx, int starty, int endx, int endy, float fade = 1f)
		{
			for (int i = (int)((float)startx * fade); i < (int)((float)endx * fade); i++)
			{
				for (int j = (int)((float)starty * fade); j < (int)((float)endy * fade); j++)
				{
					if (UnityEngine.Random.value < fade)
					{
						this.SetPixelFront(' ', i, j, ' ');
					}
					else
					{
						this.SetPixelFront(StringScrambler.GetGlitchChar(), i, j, ' ');
					}
				}
			}
		}

		// Token: 0x0600201A RID: 8218 RVA: 0x000F0DEC File Offset: 0x000EF1EC
		public void DrawText(string text, int x, int y, char col, float fade = 1f, char backColor = ' ', bool drawSpaces = false, bool fillUnfadedWithBlack = false)
		{
			int num = x;
			int num2 = 0;
			if (text.Length > 0 && 1f - fade > 0f)
			{
				text = StringScrambler.GetScrambledString(text, 1f - fade, "▀ ▄ █ ▌ ▐░ ▒ ▓ ■▪");
			}
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '\n' || text[i] == '\n')
				{
					num = x;
					num2++;
				}
				else
				{
					if (UnityEngine.Random.value < fade)
					{
						if ((drawSpaces || text[i] != ' ') && text[i] != '\r')
						{
							this.SetPixelFront(text[i], num, y + num2, col);
							if (backColor != ' ')
							{
								this.SetPixelBack('█', num, y + num2, backColor);
							}
						}
					}
					else if (fillUnfadedWithBlack)
					{
						this.SetPixelFront('█', num, y + num2, '0');
						this.SetPixelBack('█', num, y + num2, '0');
					}
					num++;
				}
			}
		}

		// Token: 0x0600201B RID: 8219 RVA: 0x000F0EFC File Offset: 0x000EF2FC
		public void FillSpaceWithColors(string colors, int x, int y, float fade = 1f, bool drawSpaces = false, bool fillUnfadedWithBlack = false)
		{
			int num = x;
			int num2 = 0;
			for (int i = 0; i < colors.Length; i++)
			{
				if (colors[i] == '\n' || colors[i] == '\n')
				{
					num = x;
					num2++;
				}
				else
				{
					if (UnityEngine.Random.value < fade)
					{
						if ((drawSpaces || colors[i] != ' ') && colors[i] != '\r')
						{
							this.SetColorFront(colors[i], num, y + num2);
						}
					}
					else if (fillUnfadedWithBlack)
					{
						this.SetColorFront('0', num, y + num2);
					}
					num++;
				}
			}
		}

		// Token: 0x0600201C RID: 8220 RVA: 0x000F0FA8 File Offset: 0x000EF3A8
		public void DrawTextSkipSpaces(string text, int x, int y, char col, float fade = 1f, char backColor = ' ')
		{
			int num = x;
			int num2 = 0;
			text = StringScrambler.GetScrambledString(text, 1f - fade, "▀ ▄ █ ▌ ▐░ ▒ ▓ ■▪");
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '\n' || text[i] == '\n')
				{
					num = x;
					num2++;
				}
				else
				{
					if (UnityEngine.Random.value < fade && text[i] != ' ')
					{
						this.SetPixelFront(text[i], num, y + num2, col);
						if (backColor != ' ')
						{
							this.SetPixelBack('█', num, y + num2, backColor);
						}
					}
					num++;
				}
			}
		}

		// Token: 0x0600201D RID: 8221 RVA: 0x000F1058 File Offset: 0x000EF458
		public void DrawBlack(string text, int x, int y)
		{
			int num = x;
			int num2 = 0;
			text = StringScrambler.GetScrambledString(text, 0f, "▀ ▄ █ ▌ ▐░ ▒ ▓ ■▪");
			for (int i = 0; i < text.Length; i++)
			{
				if (text[i] == '\n' || text[i] == '\n')
				{
					num = x;
					num2++;
				}
				else
				{
					if (UnityEngine.Random.value < 1f && text[i] != ' ')
					{
						this.SetPixelFront(' ', num, y + num2, 'x');
					}
					num++;
				}
			}
		}

		// Token: 0x0600201E RID: 8222 RVA: 0x000F10E8 File Offset: 0x000EF4E8
		public void DrawRectBack(int startx, int starty, int endx, int endy, char col, float fade = 1f)
		{
			for (int i = startx; i < endx; i++)
			{
				for (int j = starty; j < endy; j++)
				{
					if (UnityEngine.Random.value < fade)
					{
						this.SetPixelBack('█', i, j, col);
					}
					else
					{
						this.SetPixelBack(' ', i, j, col);
					}
				}
			}
		}

		// Token: 0x0600201F RID: 8223 RVA: 0x000F1148 File Offset: 0x000EF548
		public void DrawRect(int startx, int starty, int endx, int endy, char col, float fade = 1f)
		{
			for (int i = startx; i < endx; i++)
			{
				for (int j = starty; j < endy; j++)
				{
					if (UnityEngine.Random.value < fade)
					{
						this.SetPixelFront('█', i, j, col);
					}
					else
					{
						this.SetPixelFront(' ', i, j, col);
					}
				}
			}
		}

		// Token: 0x06002020 RID: 8224 RVA: 0x000F11A8 File Offset: 0x000EF5A8
		private bool stringContainsChar(char c, string str)
		{
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] == c)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06002021 RID: 8225 RVA: 0x000F11DC File Offset: 0x000EF5DC
		private void ReactToInputKeyboard(float deltaTime)
		{
			if (this.GetInteractableView() == null)
			{
				return;
			}
			SHGUIinput shguiinput = this.input.GetInput().key;
			SHGUIinput shguiinput2 = shguiinput;
			if (this.lastKey == shguiinput)
			{
				this.keyTimer += Mathf.Min(this.DeltaTimeFrac, deltaTime);
				if (this.keyTimer > 0.3f)
				{
					this.keyTimer = 0.275f;
				}
				else
				{
					shguiinput = SHGUIinput.none;
				}
			}
			else
			{
				this.keyTimer = 0f;
			}
			this.lastKey = shguiinput2;
			this.GetInteractableView().ReactToInputKeyboard(shguiinput);
		}

		// Token: 0x06002022 RID: 8226 RVA: 0x000F1275 File Offset: 0x000EF675
		private void ReactToInputGamepad()
		{
			if (this.GetInteractableView() == null)
			{
				return;
			}
		}

		// Token: 0x06002023 RID: 8227 RVA: 0x000F1284 File Offset: 0x000EF684
		private void ReactToInputMouse()
		{
			if (!this.cursorActive)
			{
				return;
			}
			if (this.GetInteractableView() == null)
			{
				return;
			}
			SHGUIinput scroll = SHGUIinput.none;
			if (Input.mouseScrollDelta.y > 0f)
			{
				scroll = SHGUIinput.scrollUp;
			}
			else if (Input.mouseScrollDelta.y < 0f)
			{
				scroll = SHGUIinput.scrollDown;
			}
			bool clicked = false;
			if (Input.GetKeyUp(KeyCode.Mouse0))
			{
				clicked = true;
			}
			this.GetInteractableView().ReactToInputMouse(this.cursorX, this.cursorY, clicked, scroll);
		}

		// Token: 0x06002024 RID: 8228 RVA: 0x000F1310 File Offset: 0x000EF710
		public SHGUIview GetLatestRedVoiceView()
		{
			for (int i = this.views.Count - 1; i >= 0; i--)
			{
				if (!RedVoice.GetShouldExcludeView(this.views[i].GetType()))
				{
					if (this.views[i].RedVoiceViewKey != null && this.views[i].RedVoiceViewKey.Length > 0)
					{
						return this.views[i];
					}
				}
			}
			return null;
		}

		// Token: 0x06002025 RID: 8229 RVA: 0x000F139C File Offset: 0x000EF79C
		public SHGUIview GetInteractableView()
		{
			if (this.views.Count == 0)
			{
				return null;
			}
			for (int i = this.views.Count - 1; i >= 0; i--)
			{
				if (!this.views[i].fadingOut && !this.views[i].remove && this.views[i].interactable)
				{
					return this.views[i];
				}
			}
			return this.views[0];
		}

		// Token: 0x06002026 RID: 8230 RVA: 0x000F1434 File Offset: 0x000EF834
		public int GetInteractableViewIndex()
		{
			if (this.views.Count == 0)
			{
				return 0;
			}
			for (int i = this.views.Count - 1; i >= 0; i--)
			{
				if (!this.views[i].fadingOut && !this.views[i].remove && this.views[i].interactable)
				{
					return i;
				}
			}
			return 0;
		}

		// Token: 0x06002027 RID: 8231 RVA: 0x000F14B8 File Offset: 0x000EF8B8
		public int GetProperViewCount()
		{
			int num = 0;
			for (int i = 0; i < this.views.Count; i++)
			{
				if (!this.views[i].fadingOut && !this.views[i].remove)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06002028 RID: 8232 RVA: 0x000F1514 File Offset: 0x000EF914
		public void Print()
		{
			if (this.SymbolToColorArray == null)
			{
				this.SymbolToColorArray = new Color32[256];
				this.SymbolToColorArray[32] = this.Colors.Default;
				this.SymbolToColorArray[114] = this.Colors.r;
				this.SymbolToColorArray[103] = this.Colors.g;
				this.SymbolToColorArray[119] = this.Colors.w;
				this.SymbolToColorArray[98] = this.Colors.b;
				this.SymbolToColorArray[122] = this.Colors.z;
				this.SymbolToColorArray[48] = this.Colors.Black;
				this.SymbolToColorArray[49] = this.Colors.Black1;
				this.SymbolToColorArray[50] = this.Colors.Black2;
				this.SymbolToColorArray[51] = this.Colors.Black3;
				this.SymbolToColorArray[52] = this.Colors.Black4;
				this.SymbolToColorArray[53] = this.Colors.Black5;
				this.SymbolToColorArray[54] = this.Colors.Black6;
				this.SymbolToColorArray[55] = this.Colors.Black7;
				this.SymbolToColorArray[56] = this.Colors.Black8;
				this.SymbolToColorArray[57] = this.Colors.Black9;
				this.SymbolToColorArray[65] = this.Colors.GrayA;
				this.SymbolToColorArray[66] = this.Colors.GrayB;
				this.SymbolToColorArray[67] = this.Colors.GrayC;
				this.SymbolToColorArray[68] = this.Colors.GrayD;
				this.SymbolToColorArray[69] = this.Colors.GrayE;
				this.SymbolToColorArray[70] = this.Colors.WhiteF;
				this.SymbolToColorArray[116] = this.Colors.BlackWithoutAlpha;
			}
			if (AsciiText.Instance.FrameBuffer != null)
			{
				AsciiText.Framebuffer frameBuffer = AsciiText.Instance.FrameBuffer;
				Vector2 vector = AsciiText.CharsetArray[32];
				Color color = default(Color);
				color.r = vector.x;
				color.g = vector.y;
				Color32 color2 = Color.black;
				Color32 color3 = Color.black;
				Vector2 vector2 = Vector2.zero;
				for (int i = 0; i < this.display.Length; i++)
				{
					if (this.display[i] == '\0' && this.background[i] == '\0')
					{
						frameBuffer.CharCoords[i] = color;
						frameBuffer.BGCharCoords[i] = color;
						frameBuffer.CharColor[i] = color2;
						frameBuffer.BGCharColor[i] = color3;
					}
					else
					{
						if (this.display[i] == '\u007f')
						{
							vector2 = AsciiText.CharsetArray[(int)this.display[i - 1]];
							vector2.x += AsciiText.CharsetArray[(int)this.display[i - 1]].z;
							frameBuffer.Char[i] = this.display[i - 1];
						}
						else
						{
							vector2 = AsciiText.CharsetArray[(int)this.display[i]];
							frameBuffer.Char[i] = this.display[i];
						}
						Color32 color4 = this.SymbolToColorArray[(int)this.backgroundColor[i]];
						Color32 color5 = this.SymbolToColorArray[(int)this.color[i]];
						frameBuffer.CharCoords[i].r = vector2.x;
						frameBuffer.CharCoords[i].g = vector2.y;
						frameBuffer.CharColor[i] = color5;
						frameBuffer.BGChar[i] = this.background[i];
						vector2 = AsciiText.CharsetArray[(int)this.background[i]];
						frameBuffer.BGCharCoords[i].r = vector2.x;
						frameBuffer.BGCharCoords[i].g = vector2.y;
						frameBuffer.BGCharColor[i] = color4;
					}
				}
			}
		}

		// Token: 0x06002029 RID: 8233 RVA: 0x000F1A6C File Offset: 0x000EFE6C
		private bool CompareColor32(Color32 a, Color32 b)
		{
			return a.r == b.r && a.g == b.g && a.b == b.b && a.a == b.a;
		}

		// Token: 0x0600202A RID: 8234 RVA: 0x000F1AD0 File Offset: 0x000EFED0
		private char ColorToSymbol(Color32 c)
		{
			if (this.CompareColor32(c, this.Colors.Default))
			{
				return ' ';
			}
			if (this.CompareColor32(c, this.Colors.r))
			{
				return 'r';
			}
			if (this.CompareColor32(c, this.Colors.g))
			{
				return 'g';
			}
			if (this.CompareColor32(c, this.Colors.w))
			{
				return 'w';
			}
			if (this.CompareColor32(c, this.Colors.b))
			{
				return 'b';
			}
			if (this.CompareColor32(c, this.Colors.z))
			{
				return 'z';
			}
			if (this.CompareColor32(c, this.Colors.Black))
			{
				return '0';
			}
			if (this.CompareColor32(c, this.Colors.Black1))
			{
				return '1';
			}
			if (this.CompareColor32(c, this.Colors.Black2))
			{
				return '2';
			}
			if (this.CompareColor32(c, this.Colors.Black3))
			{
				return '3';
			}
			if (this.CompareColor32(c, this.Colors.Black4))
			{
				return '4';
			}
			if (this.CompareColor32(c, this.Colors.Black5))
			{
				return '5';
			}
			if (this.CompareColor32(c, this.Colors.Black6))
			{
				return '6';
			}
			if (this.CompareColor32(c, this.Colors.Black7))
			{
				return '7';
			}
			if (this.CompareColor32(c, this.Colors.Black8))
			{
				return '8';
			}
			if (this.CompareColor32(c, this.Colors.Black9))
			{
				return '9';
			}
			if (this.CompareColor32(c, this.Colors.GrayA))
			{
				return 'A';
			}
			if (this.CompareColor32(c, this.Colors.GrayB))
			{
				return 'B';
			}
			if (this.CompareColor32(c, this.Colors.GrayC))
			{
				return 'C';
			}
			if (this.CompareColor32(c, this.Colors.GrayD))
			{
				return 'D';
			}
			if (this.CompareColor32(c, this.Colors.GrayE))
			{
				return 'E';
			}
			if (this.CompareColor32(c, this.Colors.WhiteF))
			{
				return 'F';
			}
			if (this.CompareColor32(c, this.Colors.BlackWithoutAlpha))
			{
				return 't';
			}
			return ' ';
		}

		// Token: 0x0600202B RID: 8235 RVA: 0x000F1D35 File Offset: 0x000F0135
		private Color32 SymbolToColor(char c)
		{
			if (this.SymbolToColorArray != null)
			{
				return this.SymbolToColorArray[(int)c];
			}
			return this.Colors.Default;
		}

		// Token: 0x0600202C RID: 8236 RVA: 0x000F1D60 File Offset: 0x000F0160
		private string GetColorFromChar(char c)
		{
			string result = "#000000ff";
			if (c == 'r')
			{
				result = "#ff0000ff";
			}
			else if (c == 'g')
			{
				result = "#00ff00ff";
			}
			else if (c == 'w')
			{
				result = "#ffffffff";
			}
			else if (c == 'b')
			{
				result = "#0000ffff";
			}
			else if (c == 'z')
			{
				result = "#888888ff";
			}
			else if (c == '0')
			{
				result = "#000000ff";
			}
			else if (c == '1')
			{
				result = "#101010ff";
			}
			else if (c == '2')
			{
				result = "#202020ff";
			}
			else if (c == '3')
			{
				result = "#303030ff";
			}
			else if (c == '4')
			{
				result = "#404040ff";
			}
			else if (c == '5')
			{
				result = "#505050ff";
			}
			else if (c == '6')
			{
				result = "#606060ff";
			}
			else if (c == '7')
			{
				result = "#707070ff";
			}
			else if (c == '8')
			{
				result = "#808080ff";
			}
			else if (c == '9')
			{
				result = "#909090ff";
			}
			else if (c == 'A')
			{
				result = "#a0a0a0ff";
			}
			else if (c == 'B')
			{
				result = "#b0b0b0ff";
			}
			else if (c == 'C')
			{
				result = "#c0c0c0ff";
			}
			else if (c == 'D')
			{
				result = "#d0d0d0ff";
			}
			else if (c == 'E')
			{
				result = "#e0e0e0ff";
			}
			else if (c == 'F')
			{
				result = "#ffffffff";
			}
			else if (c == 't')
			{
				result = "#ffffff00";
			}
			return result;
		}

		// Token: 0x0600202D RID: 8237 RVA: 0x000F1F14 File Offset: 0x000F0314
		public Color ColorStringToColor(char c)
		{
			Color result;
			if (c == 'r')
			{
				result = new Color(1f, 0f, 0f, 1f);
			}
			else if (c == 'g')
			{
				result = new Color(0f, 1f, 0f, 1f);
			}
			else if (c == 'w')
			{
				result = new Color(1f, 1f, 1f, 1f);
			}
			else if (c == 'b')
			{
				result = new Color(0f, 0f, 1f, 1f);
			}
			else if (c == 'z')
			{
				result = new Color(0.53f, 0.53f, 0.53f, 1f);
			}
			else if (c == '0')
			{
				result = new Color(0f, 0f, 0f, 1f);
			}
			else if (c == '1')
			{
				result = new Color(0.062f, 0.062f, 0.062f, 1f);
			}
			else if (c == '2')
			{
				result = new Color(0.124f, 0.124f, 0.124f, 1f);
			}
			else if (c == '3')
			{
				result = new Color(0.18599999f, 0.18599999f, 0.18599999f, 1f);
			}
			else if (c == '4')
			{
				result = new Color(0.248f, 0.248f, 0.248f, 1f);
			}
			else if (c == '5')
			{
				result = new Color(0.31f, 0.31f, 0.31f, 1f);
			}
			else if (c == '6')
			{
				result = new Color(0.371999979f, 0.371999979f, 0.371999979f, 1f);
			}
			else if (c == '7')
			{
				result = new Color(0.434f, 0.434f, 0.434f, 1f);
			}
			else if (c == '8')
			{
				result = new Color(0.496f, 0.496f, 0.496f, 1f);
			}
			else if (c == '9')
			{
				result = new Color(0.557999969f, 0.557999969f, 0.557999969f, 1f);
			}
			else if (c == 'A')
			{
				result = new Color(0.62f, 0.62f, 0.62f, 1f);
			}
			else if (c == 'B')
			{
				result = new Color(0.682f, 0.682f, 0.682f, 1f);
			}
			else if (c == 'C')
			{
				result = new Color(0.743999958f, 0.743999958f, 0.743999958f, 1f);
			}
			else if (c == 'D')
			{
				result = new Color(0.806f, 0.806f, 0.806f, 1f);
			}
			else if (c == 'E')
			{
				result = new Color(0.868f, 0.868f, 0.868f, 1f);
			}
			else if (c == 'F')
			{
				result = new Color(1f, 1f, 1f, 1f);
			}
			else if (c == 't')
			{
				result = new Color(1f, 1f, 1f, 0f);
			}
			else
			{
				result = new Color(0f, 0f, 0f, 1f);
			}
			return result;
		}

		// Token: 0x0600202E RID: 8238 RVA: 0x000F22AD File Offset: 0x000F06AD
		public int GetCenterX(string messageForOffsetting = "")
		{
			return this.resolutionX / 2 - messageForOffsetting.Length / 2 + 1;
		}

		// Token: 0x0600202F RID: 8239 RVA: 0x000F22C2 File Offset: 0x000F06C2
		public int GetCenterX(int length)
		{
			return this.resolutionX / 2 - length / 2 + 1;
		}

		// Token: 0x06002030 RID: 8240 RVA: 0x000F22D2 File Offset: 0x000F06D2
		public int GetCenterY()
		{
			return this.resolutionY / 2 - 1;
		}

		// Token: 0x06002031 RID: 8241 RVA: 0x000F22E0 File Offset: 0x000F06E0
		public SHGUIview GetFramedText(string content, int x, int y, char textcolor, char framecolor, bool centered = false)
		{
			SHGUIview shguiview = new SHGUIview();
			shguiview.x = x;
			shguiview.y = y;
			SHGUIview shguiview2 = new SHGUIview();
			if (centered)
			{
				shguiview2.x = -(content.Length / 2);
				shguiview2.y = -1;
			}
			shguiview.AddSubView(shguiview2);
			shguiview2.AddSubView(new SHGUIrect(0, 0, content.Length + 3, 2, '0', ' ', 2));
			shguiview2.AddSubView(new SHGUIframe(0, 0, content.Length + 3, 2, framecolor, null, string.Empty, 'w'));
			shguiview2.AddSubView(new SHGUItext(content, 2, 1, textcolor, false));
			return shguiview;
		}

		// Token: 0x06002032 RID: 8242 RVA: 0x000F237C File Offset: 0x000F077C
		public SHGUIchatpositioner GetCenteredChatWindow(string chatContent, int x, int y, bool interactive, int width = 22)
		{
			SHGUIchatpositioner shguichatpositioner = new SHGUIchatpositioner(SHGUIchatposition.Center);
			shguichatpositioner.x = x;
			shguichatpositioner.y = y;
			SHGUIguruchatwindow shguiguruchatwindow = new SHGUIguruchatwindow(null);
			shguiguruchatwindow.SetAlign(SHAlign.Center);
			if (interactive)
			{
				shguiguruchatwindow.SetInteractive();
			}
			shguiguruchatwindow.SetWidth(width);
			shguiguruchatwindow.SetContent(chatContent);
			shguiguruchatwindow.SetLabel(string.Empty);
			shguiguruchatwindow.showInstructions = interactive;
			shguichatpositioner.AddChat(shguiguruchatwindow);
			return shguichatpositioner;
		}

		// Token: 0x06002033 RID: 8243 RVA: 0x000F23EC File Offset: 0x000F07EC
		public SHGUIchatpositioner GetLeftAlignedTransferMessage(string chatContent, int x, int y, bool interactive, int width = 25)
		{
			SHGUIchatpositioner shguichatpositioner = new SHGUIchatpositioner(SHGUIchatposition.Left);
			shguichatpositioner.x = x;
			shguichatpositioner.y = y;
			SHGUIguruchatwindow shguiguruchatwindow = new SHGUIguruchatwindow(SHGUIframe.GetSimpleStyle());
			shguiguruchatwindow.SetAlign(SHAlign.Center);
			if (interactive)
			{
				shguiguruchatwindow.SetInteractive();
			}
			shguiguruchatwindow.SetWidth(width);
			shguiguruchatwindow.SetContent(chatContent + "^W9^W9^W9^W9^W9^W9^W9^W9^W9^W9");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SHGUI.03.TRANSFER_MESSAGE".T());
			while (stringBuilder.Length < 29)
			{
				stringBuilder.Append('-');
			}
			shguiguruchatwindow.SetLabel(stringBuilder.ToString());
			shguiguruchatwindow.showInstructions = interactive;
			shguichatpositioner.AddChat(shguiguruchatwindow);
			return shguichatpositioner;
		}

		// Token: 0x06002034 RID: 8244 RVA: 0x000F249C File Offset: 0x000F089C
		public void DrawTextProgress(string content, int X, int Y, char color, float progress, int progressEdgeLength = 6)
		{
			int num = (int)(Mathf.Clamp01(progress) * (float)content.Length);
			bool flag = true;
			if (progress > 1f)
			{
				flag = false;
			}
			int num2 = 0;
			int num3 = 0;
			for (int i = 0; i < num; i++)
			{
				if (content[i] == '\n')
				{
					num2 = 0;
					num3++;
				}
				else
				{
					char c = content[i];
					char col = color;
					if (Mathf.Abs(i - num) < progressEdgeLength && flag && c != ' ' && c != '\r' && UnityEngine.Random.value < 0.5f)
					{
						c = StringScrambler.GetGlitchChar();
						col = 'w';
					}
					if (c != ' ' && c != '\r')
					{
						SHGUI.current.SetPixelFront(c, X + num2, Y + num3, col);
					}
					num2++;
				}
			}
		}

		// Token: 0x06002035 RID: 8245 RVA: 0x000F2578 File Offset: 0x000F0978
		public static int ProperMod(int x, int m)
		{
			return (x % m + m) % m;
		}

		// Token: 0x06002036 RID: 8246 RVA: 0x000F2581 File Offset: 0x000F0981
		public void SetDelayedQueuePop(float delay)
		{
			this.delayedQueuePop = true;
			this.delayedQueuePopTime = delay;
		}

		// Token: 0x06002037 RID: 8247 RVA: 0x000F2591 File Offset: 0x000F0991
		public float GetActualDelay()
		{
			return this.actualDelay;
		}

		// Token: 0x04002153 RID: 8531
		private SHGUI.SHGUIColors Colors;

		// Token: 0x04002154 RID: 8532
		private SHInputGUI input;

		// Token: 0x04002155 RID: 8533
		public static SHGUI current;

		// Token: 0x04002156 RID: 8534
		public static int id = 0;

		// Token: 0x04002157 RID: 8535
		public int resolutionX = 80;

		// Token: 0x04002158 RID: 8536
		public int resolutionY = 24;

		// Token: 0x04002159 RID: 8537
		public char[] display;

		// Token: 0x0400215A RID: 8538
		public char[] color;

		// Token: 0x0400215B RID: 8539
		private char[] background;

		// Token: 0x0400215C RID: 8540
		private char[] backgroundColor;

		// Token: 0x0400215D RID: 8541
		public bool alterCursor = true;

		// Token: 0x0400215E RID: 8542
		public List<SHGUIview> views = new List<SHGUIview>();

		// Token: 0x0400215F RID: 8543
		public List<SHGUIview> viewQueue = new List<SHGUIview>();

		// Token: 0x04002160 RID: 8544
		private bool cursorActive;

		// Token: 0x04002161 RID: 8545
		private int cursorX;

		// Token: 0x04002162 RID: 8546
		private int cursorY;

		// Token: 0x04002163 RID: 8547
		private float cursorTimer;

		// Token: 0x04002164 RID: 8548
		private float cursorAnimatorTimer;

		// Token: 0x04002165 RID: 8549
		private int cursorAnimator;

		// Token: 0x04002166 RID: 8550
		private string cursorAnimation = "████▓▓░░▓▓░░▓▓";

		// Token: 0x04002167 RID: 8551
		private bool cursorVirgin = true;

		// Token: 0x04002168 RID: 8552
		private SHGUIinput lastKey;

		// Token: 0x04002169 RID: 8553
		private float keyTimer;

		// Token: 0x0400216A RID: 8554
		public bool finished;

		// Token: 0x0400216B RID: 8555
		private float mouseX;

		// Token: 0x0400216C RID: 8556
		private float mouseY;

		// Token: 0x0400216D RID: 8557
		public Image LevelThumbnail;

		// Token: 0x0400216E RID: 8558
		public bool OnScreen;

		// Token: 0x0400216F RID: 8559
		private StringBuilder str;

		// Token: 0x04002170 RID: 8560
		public int VisibleViewCount;

		// Token: 0x04002171 RID: 8561
		private static Dictionary<FigletFontType, Figlet> figletDictionary = new Dictionary<FigletFontType, Figlet>();

		// Token: 0x04002172 RID: 8562
		public bool delayedQueuePop;

		// Token: 0x04002173 RID: 8563
		public float delayedQueuePopTime;

		// Token: 0x04002174 RID: 8564
		public int DontDrawCounter;

		// Token: 0x04002175 RID: 8565
		private float actualDelay;

		// Token: 0x04002176 RID: 8566
		public float targetDelay = 1f;

		// Token: 0x04002177 RID: 8567
		public HeartAnimInMenu heartsAnimForMenu;

		// Token: 0x04002178 RID: 8568
		public HeartDisplayForMenu heartsDisplayForMenu;

		// Token: 0x04002179 RID: 8569
		private static bool shadersLoaded = false;

		// Token: 0x0400217A RID: 8570
		public string BackgroundNoiseId;

		// Token: 0x0400217B RID: 8571
		private int waiter;

		// Token: 0x0400217C RID: 8572
		private int scriptsLoaded = 5;

		// Token: 0x0400217D RID: 8573
		private int viewCountOld;

		// Token: 0x0400217E RID: 8574
		private bool anyVisible;

		// Token: 0x0400217F RID: 8575
		private bool anyVisiblePreviousFrame;

		// Token: 0x04002180 RID: 8576
		private static float initialTime = 0f;

		// Token: 0x04002181 RID: 8577
		private static float currentTime = 0f;

		// Token: 0x04002182 RID: 8578
		private const string cursorFrontingChars = "▀▄█▌░▒▓■▪";

		// Token: 0x04002183 RID: 8579
		private float DeltaTimeFrac = 0.0166666675f;

		// Token: 0x04002184 RID: 8580
		private Color32[] SymbolToColorArray;

		// Token: 0x020005A5 RID: 1445
		protected struct SHGUIColors
		{
			// Token: 0x06002039 RID: 8249 RVA: 0x000F25C8 File Offset: 0x000F09C8
			public void SetupPalette()
			{
				this.r = new Color32(byte.MaxValue, 0, 0, byte.MaxValue);
				this.g = new Color32(0, byte.MaxValue, 0, byte.MaxValue);
				this.w = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				this.b = new Color32(0, 0, byte.MaxValue, byte.MaxValue);
				this.z = new Color(0.53f, 0.53f, 0.53f, 1f);
				this.Black = new Color(0f, 0f, 0f, 1f);
				this.Black1 = new Color(0.2f, 0f, 0f, 1f);
				this.Black2 = new Color(0.124f, 0.124f, 0.124f, 1f);
				this.Black3 = new Color(0.18599999f, 0.18599999f, 0.18599999f, 1f);
				this.Black4 = new Color(0.248f, 0.248f, 0.248f, 1f);
				this.Black5 = new Color(0.31f, 0.31f, 0.31f, 1f);
				this.Black6 = new Color(0.371999979f, 0.371999979f, 0.371999979f, 1f);
				this.Black7 = new Color(0.434f, 0.434f, 0.434f, 1f);
				this.Black8 = new Color(0.496f, 0.496f, 0.496f, 1f);
				this.Black9 = new Color(0.557999969f, 0.557999969f, 0.557999969f, 1f);
				this.GrayA = new Color(0.62f, 0.62f, 0.62f, 1f);
				this.GrayB = new Color(0.682f, 0.682f, 0.682f, 1f);
				this.GrayC = new Color(0.743999958f, 0.743999958f, 0.743999958f, 1f);
				this.GrayD = new Color(0.806f, 0.806f, 0.806f, 1f);
				this.GrayE = new Color(0.868f, 0.868f, 0.868f, 1f);
				this.WhiteF = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
				this.BlackWithoutAlpha = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
				this.Default = new Color32(0, 0, 0, byte.MaxValue);
			}

			// Token: 0x04002185 RID: 8581
			public Color32 r;

			// Token: 0x04002186 RID: 8582
			public Color32 g;

			// Token: 0x04002187 RID: 8583
			public Color32 w;

			// Token: 0x04002188 RID: 8584
			public Color32 b;

			// Token: 0x04002189 RID: 8585
			public Color32 z;

			// Token: 0x0400218A RID: 8586
			public Color32 Black;

			// Token: 0x0400218B RID: 8587
			public Color32 Black1;

			// Token: 0x0400218C RID: 8588
			public Color32 Black2;

			// Token: 0x0400218D RID: 8589
			public Color32 Black3;

			// Token: 0x0400218E RID: 8590
			public Color32 Black4;

			// Token: 0x0400218F RID: 8591
			public Color32 Black5;

			// Token: 0x04002190 RID: 8592
			public Color32 Black6;

			// Token: 0x04002191 RID: 8593
			public Color32 Black7;

			// Token: 0x04002192 RID: 8594
			public Color32 Black8;

			// Token: 0x04002193 RID: 8595
			public Color32 Black9;

			// Token: 0x04002194 RID: 8596
			public Color32 GrayA;

			// Token: 0x04002195 RID: 8597
			public Color32 GrayB;

			// Token: 0x04002196 RID: 8598
			public Color32 GrayC;

			// Token: 0x04002197 RID: 8599
			public Color32 GrayD;

			// Token: 0x04002198 RID: 8600
			public Color32 GrayE;

			// Token: 0x04002199 RID: 8601
			public Color32 WhiteF;

			// Token: 0x0400219A RID: 8602
			public Color32 BlackWithoutAlpha;

			// Token: 0x0400219B RID: 8603
			public Color32 Default;
		}
	}
	public class MCDSHGUIframe : SHGUIframe
	{
		// Token: 0x06002094 RID: 8340 RVA: 0x000F3CD4 File Offset: 0x000F20D4
		public MCDSHGUIframe(int Startx, int Starty, int Endx, int Endy, char Col, string style = null, string title = "", char titleColor = 'w')
		: base(Startx, Starty, Endx, Endy, Col, style)
		{

			this.x = Startx;
			this.y = Starty;
			this.Width = Endx - this.x;
			this.Height = Endy - this.y;

			if (style == null)
			{
				style = MCDSHGUIframe.GetStandardStyle();
			}

			this.SetColor(Col);

			// The base constructor will be called and this will be populated with the old line data.
			this.KillChildrenInstant();
			
			SHGUIview[] array = new SHGUIline[4];
			this.lines = array;
			
			this.lines[0] = base.AddSubView(new SHGUIline(0, this.Width, 0, true, Col).SetStyle(string.Concat(new object[]
			{
			string.Empty,
			style[0],
			style[5],
			style[1]
			})));
			this.lines[1] = base.AddSubView(new SHGUIline(0, this.Width, this.Height, true, Col).SetStyle(string.Concat(new object[]
			{
			string.Empty,
			style[2],
			style[5],
			style[3]
			})));
			this.lines[2] = base.AddSubView(new SHGUIline(0, this.Height, 0, false, Col).SetStyle(string.Concat(new object[]
			{
			string.Empty,
			style[0],
			style[4],
			style[2]
			})));
			this.lines[3] = base.AddSubView(new SHGUIline(0, this.Height, this.Width, false, Col).SetStyle(string.Concat(new object[]
			{
			string.Empty,
			style[1],
			style[4],
			style[3]
			})));

			if (title != null)
			{
				base.AddSubView(new SHGUItext(title, this.Width / 2 - title.Length / 2, Starty, titleColor, false));
			}
		}

		// Token: 0x06002097 RID: 8343 RVA: 0x000F3F18 File Offset: 0x000F2318
		public override MCDSHGUIview SetColor(char color)
		{
			this.color = color;
			if (this.lines != null)
			{
				for (int i = 0; i < this.lines.Length; i++)
				{
					this.lines[i].color = color;
				}
			}
			return this;
		}

		// Token: 0x04002240 RID: 8768
		public int Width;

		// Token: 0x04002241 RID: 8769
		public int Height;

		// Token: 0x04002242 RID: 8770
		private MCDSHGUIview[] lines;
	}

	public class MCDSHGUIview
	{
		// Token: 0x0600214C RID: 8524 RVA: 0x00024410 File Offset: 0x00022810
		public MCDSHGUIview()
		{
			this.Init();
		}

		// Token: 0x0600214D RID: 8525 RVA: 0x0002445C File Offset: 0x0002285C
		protected new void Init()
		{
			this.id = MCDSHGUI.GetId();
			this.children = new List<MCDSHGUIview>();
			this.fadingIn = true;
		}

		// Token: 0x0600214E RID: 8526 RVA: 0x0002447B File Offset: 0x0002287B
		public virtual void OnPop()
		{
		}

		// Token: 0x0600214F RID: 8527 RVA: 0x00024480 File Offset: 0x00022880
		public new virtual void OnEnter()
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].OnEnter();
			}
		}

		// Token: 0x06002150 RID: 8528 RVA: 0x000244BC File Offset: 0x000228BC
		public new virtual void OnExit()
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].OnExit();
			}
		}

		// Token: 0x06002151 RID: 8529 RVA: 0x000244F8 File Offset: 0x000228F8
		public new virtual void Update()
		{
			if (this.UpdateOnlyWhenVisible && this.hidden)
			{
				return;
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Update();
				if (this.children[i].fade < 0.6f && this.fadingIn)
				{
					break;
				}
				if (this.children[i].remove)
				{
					this.children.RemoveAt(i);
					i--;
				}
			}
			this.UpdateColorPunch();
		}

		// Token: 0x06002152 RID: 8530 RVA: 0x000245A0 File Offset: 0x000229A0
		public new virtual void LateUpdate()
		{
			if (this.UpdateOnlyWhenVisible && this.hidden)
			{
				return;
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].LateUpdate();
			}
		}

		// Token: 0x06002153 RID: 8531 RVA: 0x000245F4 File Offset: 0x000229F4
		public new virtual void FadeUpdate(float inSpeedMulti = 1f, float outSpeedMulti = 1f)
		{
			if (this.UpdateOnlyWhenVisible && this.hidden)
			{
				return;
			}
			if (this.DisregardParentsFadeSpeeds)
			{
				inSpeedMulti = 1f;
				outSpeedMulti = 1f;
			}
			if (!this.hack && this.forcedFadeSpeedRegardless != 0f)
			{
				this.hack = true;
				this.fade = 0.5f;
			}
			if (this.fadingIn)
			{
				if (this.forcedFadeSpeedRegardless == 0f)
				{
					this.fade += 0.2f * Time.unscaledDeltaTime * 25f * inSpeedMulti * this.overrideFadeInSpeed;
				}
				else
				{
					this.fade += this.forcedFadeSpeedRegardless;
				}
				if (this.fade > 1f)
				{
					this.fade = 1f;
					this.fadingIn = false;
				}
			}
			if (this.fadingOut)
			{
				if (this.forcedFadeSpeedRegardless == 0f)
				{
					this.fade -= 0.3f * Time.unscaledDeltaTime * 25f * outSpeedMulti * this.overrideFadeOutSpeed;
				}
				else
				{
					this.fade -= this.forcedFadeSpeedRegardless;
				}
				if (this.fade < 0f)
				{
					this.fade = 0f;
					this.fadingOut = false;
					if (!this.DontKillOnFadeOut)
					{
						this.remove = true;
					}
				}
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].FadeUpdate(inSpeedMulti * this.overrideFadeInSpeed, outSpeedMulti * this.overrideFadeOutSpeed);
			}
		}

		// Token: 0x06002154 RID: 8532 RVA: 0x000247A8 File Offset: 0x00022BA8
		public new void ForceFadeRecursive(float fade)
		{
			this.fade = fade;
			for (int i = 0; i < this.children.Count; i++)
			{
				MCDSHGUIview shguiview = this.children[i];
				shguiview.ForceFadeRecursive(fade);
			}
		}

		// Token: 0x06002155 RID: 8533 RVA: 0x000247EC File Offset: 0x00022BEC
		public new virtual void Redraw(int offx, int offy)
		{
			if (this.hidden)
			{
				return;
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Redraw(offx + this.x, offy + this.y);
			}
		}

		// Token: 0x06002156 RID: 8534 RVA: 0x00024842 File Offset: 0x00022C42
		public new MCDSHGUIview AddSubView(MCDSHGUIview view)
		{
			this.children.Add(view as MCDSHGUIview);
			view.parent = this;
			return view;
		}

		// Token: 0x06002157 RID: 8535 RVA: 0x00024858 File Offset: 0x00022C58
		public new MCDSHGUIview AddSubViewBottom(MCDSHGUIview view)
		{
			this.children.Insert(0, view as MCDSHGUIview);
			view.parent = this;
			return view;
		}

		// Token: 0x06002158 RID: 8536 RVA: 0x00024870 File Offset: 0x00022C70
		public new void RemoveView(MCDSHGUIview v)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].id == v.id)
				{
					this.children.RemoveAt(i);
					break;
				}
			}
		}

		// Token: 0x06002159 RID: 8537 RVA: 0x000248C6 File Offset: 0x00022CC6
		public void ColorPunchIn(char colorSet, char colorBack, float delay)
		{
			this.IsColorPunch = true;
			this.lastColor = new char?(colorBack);
			this.ColorPunchTimer = Time.unscaledTime + delay;
			this.color = colorSet;
		}

		// Token: 0x0600215A RID: 8538 RVA: 0x000248F0 File Offset: 0x00022CF0
		public new virtual void Kill()
		{
			this.fadingIn = false;
			this.fadingOut = true;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Kill();
			}
		}

		// Token: 0x0600215B RID: 8539 RVA: 0x00024938 File Offset: 0x00022D38
		public new virtual void KillWithoutChildren()
		{
			this.fadingIn = false;
			this.fadingOut = true;
		}

		// Token: 0x0600215C RID: 8540 RVA: 0x00024948 File Offset: 0x00022D48
		public new void KillChildren()
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Kill();
			}
		}

		// Token: 0x0600215D RID: 8541 RVA: 0x00024982 File Offset: 0x00022D82
		public new void KillInstant()
		{
			this.remove = true;
		}

		// Token: 0x0600215E RID: 8542 RVA: 0x0002498B File Offset: 0x00022D8B
		public new void KillChildrenInstant()
		{
			this.children = new List<MCDSHGUIview>();
		}

		// Token: 0x0600215F RID: 8543 RVA: 0x00024998 File Offset: 0x00022D98
		public new virtual void ReactToInputKeyboard(SHGUIinput key)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].ReactToInputKeyboard(key);
			}
		}

		// Token: 0x06002160 RID: 8544 RVA: 0x000249D3 File Offset: 0x00022DD3
		public new virtual void ReactToInputMouse(int x, int y, bool clicked, SHGUIinput scroll)
		{
		}

		// Token: 0x06002161 RID: 8545 RVA: 0x000249D8 File Offset: 0x00022DD8
		public new void PunchIn(float startFade = 0f)
		{
			if (this.fadingOut)
			{
				return;
			}
			this.fade = startFade;
			this.fadingIn = true;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].PunchIn(startFade);
			}
		}

		// Token: 0x06002162 RID: 8546 RVA: 0x00024A30 File Offset: 0x00022E30
		public new void ForcedSoftFadeIn()
		{
			this.fade = -1f;
			this.fadingIn = true;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].ForcedSoftFadeIn();
			}
		}

		// Token: 0x06002163 RID: 8547 RVA: 0x00024A7C File Offset: 0x00022E7C
		public new void SpeedUpFadeIn()
		{
			if (!this.fadingIn)
			{
				return;
			}
			this.fade += 0.4f;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].SpeedUpFadeIn();
			}
		}

		// Token: 0x06002164 RID: 8548 RVA: 0x00024AD4 File Offset: 0x00022ED4
		public new bool FixedUpdater(float delay = 0.05f)
		{
			this.timer -= Time.unscaledDeltaTime;
			if (this.timer < 0f)
			{
				this.timer = delay;
				return true;
			}
			return false;
		}

		// Token: 0x06002165 RID: 8549 RVA: 0x00024B02 File Offset: 0x00022F02
		public new virtual MCDSHGUIview SetColor(char c)
		{
			this.color = c;
			return this;
		}

		// Token: 0x06002166 RID: 8550 RVA: 0x00024B0C File Offset: 0x00022F0C
		public new MCDSHGUIview SetColorRecursive(char c)
		{
			this.color = c;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].SetColor(this.color);
			}
			return this;
		}

		// Token: 0x06002167 RID: 8551 RVA: 0x00024B58 File Offset: 0x00022F58
		public MCDSHGUIview SetColorRecursiveWithChildren(char c)
		{
			this.color = c;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].SetColorRecursiveWithChildren(this.color);
			}
			return this;
		}

		// Token: 0x06002168 RID: 8552 RVA: 0x00024BA1 File Offset: 0x00022FA1
		public new MCDSHGUIview SetCursorDraw(bool v)
		{
			this.allowCursorDraw = v;
			return this;
		}

		// Token: 0x06002169 RID: 8553 RVA: 0x00024BAC File Offset: 0x00022FAC
		public virtual void Darken()
		{
			char? c = this.lastColor;
			if (c != null)
			{
				char? c2 = this.lastColor;
				if (((c2 == null) ? null : new int?((int)c2.Value)) != (int)this.color)
				{
					return;
				}
			}
			this.lastColor = new char?(this.color);
			this.color = '3';
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Darken();
			}
		}

		// Token: 0x0600216A RID: 8554 RVA: 0x00024C64 File Offset: 0x00023064
		public virtual void Brighten()
		{
			char? c = this.lastColor;
			if (c == null)
			{
				return;
			}
			char? c2 = this.lastColor;
			this.color = c2.Value;
			this.lastColor = null;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Brighten();
			}
		}

		// Token: 0x0600216B RID: 8555 RVA: 0x00024CD8 File Offset: 0x000230D8
		private void UpdateColorPunch()
		{
			if (!this.IsColorPunch)
			{
				return;
			}
			if (Time.unscaledTime > this.ColorPunchTimer)
			{
				char? c = this.lastColor;
				this.SetColor(c.Value);
				this.IsColorPunch = false;
			}
		}

		// Token: 0x04002305 RID: 8965
		public int x;

		// Token: 0x04002306 RID: 8966
		public int y;

		// Token: 0x04002307 RID: 8967
		public char color = 'w';

		// Token: 0x04002308 RID: 8968
		public char? lastColor;

		// Token: 0x04002309 RID: 8969
		public List<MCDSHGUIview> children;

		// Token: 0x0400230A RID: 8970
		public MCDSHGUIview parent;

		// Token: 0x0400230B RID: 8971
		public float fade;

		// Token: 0x0400230C RID: 8972
		public bool fadingIn;

		// Token: 0x0400230D RID: 8973
		public bool fadingOut;

		// Token: 0x0400230E RID: 8974
		public bool hidden;

		// Token: 0x0400230F RID: 8975
		public bool interactable = true;

		// Token: 0x04002310 RID: 8976
		public bool dontDrawViewsBelow = true;

		// Token: 0x04002311 RID: 8977
		public bool allowCursorDraw;

		// Token: 0x04002312 RID: 8978
		public bool remove;

		// Token: 0x04002313 RID: 8979
		public int id;

		// Token: 0x04002314 RID: 8980
		private float timer;

		// Token: 0x04002315 RID: 8981
		public float overrideFadeInSpeed = 1f;

		// Token: 0x04002316 RID: 8982
		public float overrideFadeOutSpeed = 1f;

		// Token: 0x04002317 RID: 8983
		public float forcedFadeSpeedRegardless;

		// Token: 0x04002318 RID: 8984
		public bool DisregardParentsFadeSpeeds;

		// Token: 0x04002319 RID: 8985
		public bool DontKillOnFadeOut;

		// Token: 0x0400231A RID: 8986
		public bool shouldCloseOnClick = true;

		// Token: 0x0400231B RID: 8987
		private bool IsColorPunch;

		// Token: 0x0400231C RID: 8988
		private char ColorPunch;

		// Token: 0x0400231D RID: 8989
		private float ColorPunchTimer;

		// Token: 0x0400231E RID: 8990
		public string RedVoiceViewKey;

		// Token: 0x0400231F RID: 8991
		public bool UpdateOnlyWhenVisible;

		// Token: 0x04002320 RID: 8992
		private bool hack;
	}*/
}