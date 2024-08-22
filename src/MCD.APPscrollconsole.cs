using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using InControl;
using InputSystem;
using UnityEngine;
using UnityEngine.Rendering;
using Utilities.CameraEffects;

using MCD.ShGUIblinkview;
using MCD.ShGUIchatpositioner;
using MCD.ShGUIClock;
using MCD.ShGUIguruchatwindow;
using MCD.ShGUIFrame;
using MCD.ShGUIPrompter;
using MCD.ShGUIText;
using MCD.ShGUIView;

namespace MCD.Appscrollconsole
{
	// Token: 0x02000579 RID: 1401
	public class MCDAPPscrollconsole : MCDSHGUIview
	{
		// Token: 0x06001F27 RID: 7975 RVA: 0x000D6F4C File Offset: 0x000D534C
		public MCDAPPscrollconsole()
		{
			this.width = SHGUI.current.resolutionX;
			this.messages = new List<MCDSHGUIview>();
			this.queue = new List<mcdscrollmessage>();
			this.maxlines = SHGUI.current.resolutionY - 2;
			this.instructionsAnchorX = SHGUI.current.resolutionX - 2;
			this.instructionsAnchorY = SHGUI.current.resolutionY - 1;
			this.instructions = new MCDSHGUItext(string.Empty, this.instructionsAnchorX, this.instructionsAnchorY, 'z', false);
			base.AddSubView(this.instructions);
			this.allowCursorDraw = false;
		}

		// Token: 0x06001F28 RID: 7976 RVA: 0x000D704A File Offset: 0x000D544A
		public void ShowOnlyQuitInstructions()
		{
			this.chatQuitInstructions = (base.AddSubView(new MCDSHGUItext(string.Empty, SHGUI.current.resolutionX - 22, SHGUI.current.resolutionY - 1, 'z', false)) as MCDSHGUItext);
		}

		// Token: 0x06001F29 RID: 7977 RVA: 0x000D7084 File Offset: 0x000D5484
		public virtual void ShowChatFrames()
		{
			this.frame = base.AddSubView(new MCDSHGUIframe(0, 0, SHGUI.current.resolutionX - 1, SHGUI.current.resolutionY - 1, 'z', null, string.Empty, 'w'));
			this.appname = base.AddSubView(new MCDSHGUItext("guruCHAT", 3, 0, 'w', false));
			this.chatQuitInstructions = (base.AddSubView(new MCDSHGUItext(string.Empty, SHGUI.current.resolutionX - 22, SHGUI.current.resolutionY - 1, 'z', false)) as MCDSHGUItext);
			this.clock = base.AddSubView(new MCDSHGUIclock(77, 0, 'w'));
			this.frameOffset = 1;
			this.lines = 1;
		}

		// Token: 0x06001F2A RID: 7978 RVA: 0x000D713C File Offset: 0x000D553C
		public void ShowChat(int width)
		{
			this.frame = base.AddSubView(new MCDSHGUIframe(1, 0, 36, SHGUI.current.resolutionY - 1, 'z', null, string.Empty, 'w'));
			this.appname = base.AddSubView(new MCDSHGUItext("guruCHAT", 3, 0, 'w', false));
			this.frameOffset = 0;
			this.lines = 1;
		}

		// Token: 0x06001F2B RID: 7979 RVA: 0x000D71A0 File Offset: 0x000D55A0
		public void HideChatLabels()
		{
			if (this.appname != null)
			{
				this.appname.Kill();
			}
			if (this.instructions != null)
			{
				this.instructions.Kill();
			}
			if (this.chatQuitInstructions != null)
			{
				this.chatQuitInstructions.Kill();
			}
			if (this.clock != null)
			{
				this.clock.Kill();
			}
		}

		// Token: 0x06001F2C RID: 7980 RVA: 0x000D7205 File Offset: 0x000D5605
		public void HideChatFrames()
		{
			if (this.frame != null)
			{
				this.frame.Kill();
			}
			this.HideChatLabels();
			this.frameOffset = 0;
		}

		// Token: 0x06001F2D RID: 7981 RVA: 0x000D722C File Offset: 0x000D562C
		private void UpdateInstructions(string newtext)
		{
			if (!this.showInstruction)
			{
				return;
			}
			if (this.instructions.text != newtext)
			{
				this.instructions.PunchIn(0f);
			}
			this.instructions.text = newtext;
			this.instructions.x = this.instructionsAnchorX;
			this.instructions.y = this.lines + 1;
			this.instructions.GoFromRight();
		}

		// Token: 0x06001F2E RID: 7982 RVA: 0x000D72A8 File Offset: 0x000D56A8
		private void SetChatQuitInstructions(string newtext)
		{
			if (this.chatQuitInstructions == null)
			{
				return;
			}
			this.chatQuitInstructions.color = 'z';
			if (this.chatQuitInstructions.text != newtext)
			{
				this.chatQuitInstructions.PunchIn(0f);
			}
			this.chatQuitInstructions.text = newtext;
			this.chatQuitInstructions.x = SHGUI.current.resolutionX - 5;
			this.chatQuitInstructions.GoFromRight();
		}

		// Token: 0x06001F2F RID: 7983 RVA: 0x000D7324 File Offset: 0x000D5724
		public void MakeLastMessageRed()
		{
			MCDSHGUIview lastNotEmptyMessage = this.GetLastNotEmptyMessage();
			if (lastNotEmptyMessage != null)
			{
				lastNotEmptyMessage.SetColorRecursive('r');
			}
		}

		// Token: 0x06001F30 RID: 7984 RVA: 0x000D7348 File Offset: 0x000D5748
		private MCDSHGUIview GetLastNotEmptyMessage()
		{
			MCDSHGUIview result = this.messages[this.messages.Count - 1];
			for (int i = this.messages.Count - 1; i >= 0; i--)
			{
				if (this.messages[i].children.Count != 0)
				{
					result = this.messages[i];
					break;
				}
			}
			return result;
		}

		// Token: 0x06001F31 RID: 7985 RVA: 0x000D73BC File Offset: 0x000D57BC
		private void DisplayScrollMessage(mcdscrollmessage m)
		{
			if (!string.IsNullOrEmpty(m.controlCommand))
			{
				this.ParseCommand(m.controlCommand);
				return;
			}
			if (m.method != null)
			{
				m.method();
				return;
			}
			if (this.skipping)
			{
				return;
			}
			if (m.overrideLast)
			{
				SHGUI.current.PlaySound(SHGUIsound.messageswitch);
				MCDSHGUIview lastNotEmptyMessage = this.GetLastNotEmptyMessage();
				lastNotEmptyMessage.KillInstant();
				if (lastNotEmptyMessage is MCDSHGUItext)
				{
					this.lines -= (lastNotEmptyMessage as MCDSHGUItext).CountLines();
				}
				else if (lastNotEmptyMessage is MCDSHGUIguruchatwindow)
				{
					this.lines -= (lastNotEmptyMessage as MCDSHGUIguruchatwindow).height;
				}
			}
			this.messages.Add(m.view);
			base.AddSubViewBottom(m.view);
			m.view.overrideFadeInSpeed = 0.75f;
			if (this.showFadeForChatMessages)
			{
				m.view.ForceFadeRecursive(0f);
				m.view.overrideFadeInSpeed = 0.45f;
			}
			m.view.y = this.lines;
			m.view.fade = m.baseFade;
			this.delay = m.delay;
			this.lines += m.height;
			this.noPropmterInteractionTime = 0f;
			if (m.view is MCDSHGUIguruchatwindow && m.overrideLast)
			{
				(m.view as MCDSHGUIguruchatwindow).ShowInstantPunchIn();
			}
			if (m.view is MCDSHGUIprompter || !(m.view is MCDSHGUIguruchatwindow))
			{
			}
		}

		// Token: 0x06001F32 RID: 7986 RVA: 0x000D7568 File Offset: 0x000D5968
		private void ParseCommand(string command)
		{
			if (command == "skiphere")
			{
				this.skippable = false;
				this.skipping = false;
			}
			if (command.StartsWith("inter:"))
			{
				string interpolatorName = command.Substring("inter:".Length);
				//CameraEffectsManager.Instance[interpolatorName].Play();
			}
			if (command.StartsWith("sound:"))
			{
				string text = command.Substring("sound:".Length);
			}
			if (command == "kill")
			{
				this.Kill();
			}
			if (command == "onlylast")
			{
				bool flag = false;
				for (int i = this.messages.Count - 1; i >= 0; i--)
				{
					if (flag)
					{
						this.messages[i].Kill();
					}
					if (this.messages[i].children.Count > 0)
					{
						flag = true;
					}
				}
				this.HideChatLabels();
				this.HideChatFrames();
			}
			if (command == "nolabels")
			{
				this.HideChatLabels();
			}
			if (command == "noframes")
			{
				this.HideChatFrames();
			}
		}

		// Token: 0x06001F33 RID: 7987 RVA: 0x000D7698 File Offset: 0x000D5A98
		protected void DisplayEmptyMessage()
		{
			mcdscrollmessage m = new mcdscrollmessage(new MCDSHGUIview(), 1, 0.2f, false, 0f, string.Empty);
			this.DisplayScrollMessage(m);
		}

		// Token: 0x06001F34 RID: 7988 RVA: 0x000D76C8 File Offset: 0x000D5AC8
		protected void KillMessages()
		{
			for (int i = 0; i < this.messages.Count; i++)
			{
				this.messages[i].Kill();
			}
		}

		// Token: 0x06001F35 RID: 7989 RVA: 0x000D7704 File Offset: 0x000D5B04
		public void AddTextToQueue(string Text, float Delay, char color = 'z', int offset = 0)
		{
			MCDSHGUItext shguitext = new MCDSHGUItext(Text, this.frameOffset + offset, 0, color, false);
			this.queue.Add(new mcdscrollmessage(shguitext, shguitext.CountLines(), Delay, false, 0.5f, string.Empty));
		}

		// Token: 0x06001F36 RID: 7990 RVA: 0x000D7748 File Offset: 0x000D5B48
		public void AddTextToQueueBreakLines(string Text, float Delay, char color = 'z', int offset = 0)
		{
			MCDSHGUItext shguitext = new MCDSHGUItext(Text, this.frameOffset + offset, 0, color, false);
			shguitext.BreakCut(SHGUI.current.resolutionX - this.frameOffset, 100);
			this.queue.Add(new mcdscrollmessage(shguitext, shguitext.CountLines(), Delay, false, 0.5f, string.Empty));
		}

		// Token: 0x06001F37 RID: 7991 RVA: 0x000D77A8 File Offset: 0x000D5BA8
		public void AddTextToQueueCentered(string Text, float Delay, char color = 'z')
		{
			MCDSHGUItext shguitext = new MCDSHGUItext(Text, 0, 0, color, false);
			shguitext.x = this.frameOffset + SHGUI.current.resolutionX / 2 - shguitext.GetLineLength() / 2;
			this.queue.Add(new mcdscrollmessage(shguitext, shguitext.CountLines(), Delay, false, 0.5f, string.Empty));
		}

		// Token: 0x06001F38 RID: 7992 RVA: 0x000D7808 File Offset: 0x000D5C08
		public void AddTextToQueueCenteredBlinking(string Text, float Delay, char color = 'z')
		{
			MCDSHGUIview shguiview = new MCDSHGUIblinkview(0.3f);
			MCDSHGUItext shguitext = new MCDSHGUItext(Text, 0, 0, color, false);
			shguiview.AddSubView(shguitext);
			shguitext.x = this.frameOffset + SHGUI.current.resolutionX / 2 - shguitext.GetLineLength() / 2;
			this.queue.Add(new mcdscrollmessage(shguiview, shguitext.CountLines(), Delay, false, 0.5f, string.Empty));
		}

		// Token: 0x06001F39 RID: 7993 RVA: 0x000D7878 File Offset: 0x000D5C78
		public void AddEmptyLine(float Delay)
		{
			this.AddTextToQueue(string.Empty, Delay, 'z', 0);
		}

		// Token: 0x06001F3A RID: 7994 RVA: 0x000D7889 File Offset: 0x000D5C89
		public void AddWait(float Delay)
		{
			this.queue.Add(new mcdscrollmessage(new MCDSHGUIview(), 0, Delay, false, 0f, string.Empty));
		}

		// Token: 0x06001F3B RID: 7995 RVA: 0x000D78B0 File Offset: 0x000D5CB0
		public MCDSHGUIprompter AddPrompterToQueue(string Text, float Delay, bool centered = false)
		{
			MCDSHGUItext shguitext = new MCDSHGUItext(Text, this.frameOffset, 0, 'z', false);
			shguitext.BreakCut(SHGUI.current.resolutionX - 2 - this.frameOffset, 100);
			int longestLineLength = shguitext.GetLongestLineLength();
			MCDSHGUIprompter shguiprompter = new MCDSHGUIprompter(this.frameOffset, 0, 'w');
			if (this.defaultConsoleCallback != null)
			{
				shguiprompter.thisConsoleCallback = this.defaultConsoleCallback;
			}
			shguiprompter.SetInput(Text, true);
			if (centered)
			{
				shguiprompter.x = SHGUI.current.resolutionX / 2 - shguiprompter.GetFirstLineLengthWithoutSpecialSigns() / 2;
			}
			shguiprompter.maxLineLength = SHGUI.current.resolutionX - 2;
			shguiprompter.maxSmartBreakOffset = 0;
			this.queue.Add(new mcdscrollmessage(shguiprompter, shguitext.CountLines(), Delay, false, 0f, string.Empty));
			return shguiprompter;
		}

		// Token: 0x06001F3C RID: 7996 RVA: 0x000D7980 File Offset: 0x000D5D80
		public void AddPrompterToQueueFaster(string Text, float Delay, bool centered = false)
		{
			MCDSHGUItext shguitext = new MCDSHGUItext(Text, this.frameOffset, 0, 'z', false);
			shguitext.BreakCut(SHGUI.current.resolutionX - 2 - this.frameOffset, 100);
			int longestLineLength = shguitext.GetLongestLineLength();
			MCDSHGUIprompter shguiprompter = new MCDSHGUIprompter(this.frameOffset, 0, 'w');
			if (this.defaultConsoleCallback != null)
			{
				shguiprompter.thisConsoleCallback = this.defaultConsoleCallback;
			}
			shguiprompter.SetInput(Text, true);
			if (centered)
			{
				shguiprompter.x = SHGUI.current.resolutionX / 2 - shguiprompter.GetFirstLineLengthWithoutSpecialSigns() / 2;
			}
			shguiprompter.maxLineLength = SHGUI.current.resolutionX - 2;
			shguiprompter.maxSmartBreakOffset = 0;
			shguiprompter.baseCharDelay /= 2f;
			this.queue.Add(new mcdscrollmessage(shguiprompter, shguitext.CountLines(), Delay, false, 0f, string.Empty));
		}

		// Token: 0x06001F3D RID: 7997 RVA: 0x000D7A60 File Offset: 0x000D5E60
		public void AddInteractivePrompterToQueue(string Text, string prefix = "")
		{
			MCDSHGUItext shguitext = new MCDSHGUItext(Text, this.frameOffset, 0, 'w', false);
			shguitext.BreakCut(SHGUI.current.resolutionX - 2, 100);
			MCDSHGUIprompter shguiprompter = new MCDSHGUIprompter(this.frameOffset, 0, 'w');
			shguiprompter.SetInput(Text, true);
			shguiprompter.SwitchToManualInputMode();
			shguiprompter.maxLineLength = SHGUI.current.resolutionX - 2 - this.frameOffset;
			shguiprompter.maxSmartBreakOffset = 0;
			shguiprompter.AddPrefix(prefix);
			this.queue.Add(new mcdscrollmessage(shguiprompter, shguitext.CountLines(), 0f, false, 0f, string.Empty));
		}

		// Token: 0x06001F3E RID: 7998 RVA: 0x000D7B00 File Offset: 0x000D5F00
		protected mcdscrollmessage AddChatMessage(string sender, string message, bool leftright, bool interactive, bool poor, bool overrideLast = false, bool showInstant = false)
		{
			if (this.dontDisplaySender)
			{
				sender = string.Empty;
			}
			message = message.Replace("\r", string.Empty);
			MCDSHGUIguruchatwindow shguiguruchatwindow = new MCDSHGUIguruchatwindow(null);
			shguiguruchatwindow.SetAlign((!leftright) ? SHAlign.Right : SHAlign.Left);
			if (interactive)
			{
				shguiguruchatwindow.SetInteractive();
			}
			if (poor)
			{
				shguiguruchatwindow.poorMode = true;
			}
			shguiguruchatwindow.SetWidth(this.desiredFrameWidth);
			shguiguruchatwindow.SetContent(message);
			shguiguruchatwindow.SetLabel(sender);
			shguiguruchatwindow.x = this.chatMargin;
			if (!leftright)
			{
				shguiguruchatwindow.x = SHGUI.current.resolutionX - this.chatMargin - shguiguruchatwindow.width;
			}
			int heightOfCompleteTextWithFrameVERYSLOWandMOODY = shguiguruchatwindow.GetHeightOfCompleteTextWithFrameVERYSLOWandMOODY();
			mcdscrollmessage scrollmessage = new mcdscrollmessage(shguiguruchatwindow, heightOfCompleteTextWithFrameVERYSLOWandMOODY, 0f, false, 0f, string.Empty);
			scrollmessage.overrideLast = overrideLast;
			this.queue.Add(scrollmessage);
			if (overrideLast)
			{
				shguiguruchatwindow.showInstructions = false;
			}
			if (this.defaultConsoleCallback != null)
			{
				shguiguruchatwindow.SetCallback(this.defaultConsoleCallback);
			}
			if (showInstant)
			{
				shguiguruchatwindow.ShowInstantPunchIn();
			}
			if (showInstant && !interactive && leftright)
			{
				shguiguruchatwindow.showInstructions = false;
			}
			return scrollmessage;
		}

		// Token: 0x06001F3F RID: 7999 RVA: 0x000D7C37 File Offset: 0x000D6037
		public void AddControlCommand(string command)
		{
			this.queue.Add(new mcdscrollmessage(new MCDSHGUIview(), 0, 0f, false, 1f, command));
		}

		// Token: 0x06001F40 RID: 8000 RVA: 0x000D7C5C File Offset: 0x000D605C
		public void AddAction(Action method)
		{
			mcdscrollmessage scrollmessage = new mcdscrollmessage(new MCDSHGUIview(), 0, 0f, false, 1f, string.Empty);
			scrollmessage.method = method;
			this.queue.Add(scrollmessage);
		}

		// Token: 0x06001F41 RID: 8001 RVA: 0x000D7C98 File Offset: 0x000D6098
		public void AddMyMessage(string sender, string message)
		{
			this.AddChatMessage(sender, message, true, true, false, false, false);
		}

		// Token: 0x06001F42 RID: 8002 RVA: 0x000D7CA8 File Offset: 0x000D60A8
		public void AddMyMessageShowInstant(string sender, string message)
		{
			this.AddChatMessage(sender, message, true, false, false, false, true);
		}

		// Token: 0x06001F43 RID: 8003 RVA: 0x000D7CB8 File Offset: 0x000D60B8
		public void AddMyMessageChatOverride(string sender, string message)
		{
			this.AddWait(0.3f);
			this.AddAction(delegate
			{
				//AudioManager.Instance.PlayClip(AudioResources.Terminal.ChatOverrideRed, null);
				this.MakeLastMessageRed();
			});
			this.AddWait(0.2f);
			this.AddChatMessage(sender, message, true, false, false, true, false);
			this.AddWait(0.75f);
		}

		// Token: 0x06001F44 RID: 8004 RVA: 0x000D7D06 File Offset: 0x000D6106
		public void AddOtherMessage(string sender, string message)
		{
			this.AddChatMessage(sender, message, false, false, false, false, false);
		}

		// Token: 0x06001F45 RID: 8005 RVA: 0x000D7D16 File Offset: 0x000D6116
		public void AddMySystemMessage(string sender, string message)
		{
			this.AddChatMessage(sender, message, true, false, true, false, false);
		}

		// Token: 0x06001F46 RID: 8006 RVA: 0x000D7D26 File Offset: 0x000D6126
		public void AddOtherSystemMessage(string sender, string message)
		{
			this.AddChatMessage(sender, message, false, false, true, false, false);
		}

		// Token: 0x06001F47 RID: 8007 RVA: 0x000D7D36 File Offset: 0x000D6136
		public void AddMyQuit()
		{
			this.AddOtherQuit();
		}

		private MCDSHGUIchatpositioner GetCenteredChatWindow(string chatContent, int x, int y, bool interactive, int width = 22)
		{
			MCDSHGUIchatpositioner obj = new MCDSHGUIchatpositioner
			{
				x = x,
				y = y
			};
			MCDSHGUIguruchatwindow sHGUIguruchatwindow = new MCDSHGUIguruchatwindow();
			sHGUIguruchatwindow.SetAlign(SHAlign.Center);
			if (interactive)
			{
				sHGUIguruchatwindow.SetInteractive();
			}

			sHGUIguruchatwindow.SetWidth(width);
			sHGUIguruchatwindow.SetContent(chatContent);
			sHGUIguruchatwindow.SetLabel("");
			sHGUIguruchatwindow.showInstructions = interactive;
			obj.AddChat(sHGUIguruchatwindow);
			return obj;
		}

		// Token: 0x06001F48 RID: 8008 RVA: 0x000D7D40 File Offset: 0x000D6140
		public void AddOtherQuit()
		{
			this.AddControlCommand("skiphere");
			this.AddControlCommand("nolabels");
			string text = "APPscrollconsole.01.CHAT_ENDED".T();
			MCDSHGUIchatpositioner centeredChatWindow = GetCenteredChatWindow(text, 32, 0, false, 22);
			centeredChatWindow.chat.SetColorRecursive('z');
			MCDSHGUItext shguitext = new MCDSHGUItext(text + "^W1", 0, 0, 'w', false);
			shguitext.x = this.frameOffset + SHGUI.current.resolutionX / 2 - shguitext.GetLineLength() / 2 - 1;
			this.AddMessageToQueue(new MCDSHGUIview(), 1, 0f, false, 0f);
			this.AddMessageToQueue(centeredChatWindow, 1, 0f, false, 0f);
			this.AddMessageToQueue(new MCDSHGUIview(), 1, 1.75f, false, 0f);
		}

		// Token: 0x06001F49 RID: 8009 RVA: 0x000D7E0C File Offset: 0x000D620C
		public void AddRedQuit()
		{
			this.AddControlCommand("skiphere");
			this.AddControlCommand("nolabels");
			string text = "^Fr^Cr--YOUR-AUDIENCE-HAS-ENDED--";
			MCDSHGUIchatpositioner centeredChatWindow = GetCenteredChatWindow(text, 32, 0, false, 22);
			MCDSHGUItext shguitext = new MCDSHGUItext(text + "^W1", 0, 0, 'w', false);
			shguitext.x = this.frameOffset + SHGUI.current.resolutionX / 2 - shguitext.GetLineLength() / 2 - 1;
			this.AddMessageToQueue(new MCDSHGUIview(), 1, 0f, false, 0f);
			this.AddMessageToQueue(centeredChatWindow, 1, 0f, false, 0f);
			this.AddMessageToQueue(new MCDSHGUIview(), 1, 1.75f, false, 0f);
		}

		// Token: 0x06001F4A RID: 8010 RVA: 0x000D7EC2 File Offset: 0x000D62C2
		public void AddMessageToQueue(MCDSHGUIview View, int Height, float Delay, bool overrideLast = false, float baseFade = 0f)
		{
			this.queue.Add(new mcdscrollmessage(View, Height, Delay, overrideLast, baseFade, string.Empty));
		}

		// Token: 0x06001F4B RID: 8011 RVA: 0x000D7EE0 File Offset: 0x000D62E0
		public virtual void ShiftAllMessages(int offy)
		{
			for (int i = 0; i < this.messages.Count; i++)
			{
				if (this.messages[i].y + offy < this.killBelowThisY)
				{
					this.messages[i].KillInstant();
					this.messages.RemoveAt(i);
					i--;
				}
				else
				{
					this.messages[i].y += offy;
				}
			}
			int num = 0;
			for (int j = 0; j < this.messages.Count; j++)
			{
				if (j == 0)
				{
					num = this.messages[j].y - (this.killBelowThisY + 1);
					this.messages[j].y = this.killBelowThisY + 1;
				}
				else
				{
					this.messages[j].y -= num;
				}
			}
			this.lines += offy;
			this.lines -= num;
		}

		// Token: 0x06001F4C RID: 8012 RVA: 0x000D7FF8 File Offset: 0x000D63F8
		public override void Update()
		{
			base.Update();
			if (this.startQueueCount < 0)
			{
				this.startQueueCount = this.queue.Count;
			}
			this.delay -= Time.unscaledDeltaTime;
			if (this.lines > this.maxlines)
			{
				this.ShiftAllMessages(-(this.lines - this.maxlines));
			}
			if (this.customSkipTimeout < 0f)
			{
				if (this.skippable)
				{
					if (this.leaveChatOverrideString != null)
					{
						this.SetChatQuitInstructions(this.leaveChatOverrideString);
					}
					else
					{
						this.SetChatQuitInstructions(LocalizationManager.Instance.GetLocalized(this.leaveChatLocalizedString));
					}
				}
				else
				{
					this.SetChatQuitInstructions(string.Empty);
				}
			}
			else
			{
				this.customSkipTimeout -= Time.unscaledDeltaTime;
			}
			bool flag = false;
			if (this.messages.Count > 0)
			{
				if (this.messages[this.messages.Count - 1] is MCDSHGUIprompter)
				{
					flag = true;
					MCDSHGUIprompter shguiprompter = this.messages[this.messages.Count - 1] as MCDSHGUIprompter;
					if ((this.messages[this.messages.Count - 1] as MCDSHGUIprompter).manualUpdate)
					{
						if (shguiprompter.noInteractionTimer > 1.5f)
						{
							if (shguiprompter.IsAlmostFinished())
							{
								this.UpdateInstructions("ENTER TO EXECUTE");
							}
							else
							{
								this.UpdateInstructions("TYPE TO HACK");
							}
						}
					}
					else
					{
						this.UpdateInstructions(string.Empty);
					}
				}
				if (this.messages[this.messages.Count - 1] is MCDSHGUIguruchatwindow)
				{
					flag = true;
				}
			}
			if (flag)
			{
				if (this.messages[this.messages.Count - 1] is MCDSHGUIprompter && (this.messages[this.messages.Count - 1] as MCDSHGUIprompter).IsFinished())
				{
					this.DisplayNextMessage();
				}
				else if (this.messages[this.messages.Count - 1] is MCDSHGUIguruchatwindow)
				{
					MCDSHGUIguruchatwindow shguiguruchatwindow = this.messages[this.messages.Count - 1] as MCDSHGUIguruchatwindow;
					if (shguiguruchatwindow.finished)
					{
						this.DisplayNextMessage();
					}
				}
			}
			else
			{
				this.UpdateInstructions(string.Empty);
				if (this.delay < 0f)
				{
					this.DisplayNextMessage();
				}
			}
			for (int i = 0; i < this.messages.Count; i++)
			{
				MCDSHGUIguruchatwindow shguiguruchatwindow2 = this.messages[i] as MCDSHGUIguruchatwindow;
				if (shguiguruchatwindow2 != null)
				{
					shguiguruchatwindow2.x = this.chatMargin;
					if (shguiguruchatwindow2.Align == SHAlign.Right)
					{
						shguiguruchatwindow2.x = this.width - this.chatMargin - shguiguruchatwindow2.width - 26;
					}
				}
			}
		}

		// Token: 0x06001F4D RID: 8013 RVA: 0x000D82FC File Offset: 0x000D66FC
		public override void ReactToInputKeyboard(SHGUIinput key)
		{
			if (this.fadingOut)
			{
				return;
			}
			base.ReactToInputKeyboard(key);
			if (key == SHGUIinput.any && this.instructions.text == "ENTER TO EXECUTE")
			{
				this.instructions.PunchIn(0.5f);
			}
			/*if (key == SHGUIinput.esc && InputManager.CurrentTick - this.lastKey > 25UL)
			{
				this.SkipConsole();
			}
			if (key != SHGUIinput.none && key != SHGUIinput.esc)
			{
				this.lastKey = InputManager.CurrentTick;
			}
			if (this.resetInputAxes)
			{
				SHInput.ClearInputState();
			}*/
		}

		// Token: 0x06001F4E RID: 8014 RVA: 0x000D8396 File Offset: 0x000D6796
		public void AddCustomSkipMessage(string msg)
		{
			this.customSkip = true;
			if (this.customSkipMessages == null)
			{
				this.customSkipMessages = new List<string>();
			}
			this.customSkipMessages.Add(msg);
		}

		// Token: 0x06001F4F RID: 8015 RVA: 0x000D83C4 File Offset: 0x000D67C4
		private void SkipConsole()
		{
			if (!this.skipping && this.skippable)
			{
				if (this.customSkip)
				{
					if (this.customSkipMessages != null && this.customSkipMessages.Count > 0)
					{
						this.SetChatQuitInstructions(this.customSkipMessages[UnityEngine.Random.Range(0, this.customSkipMessages.Count)]);
					}
					if (this.customSkipTimeout > 0f)
					{
						//CameraEffectsManager.Instance["RedTextPunch"].Play();
						this.chatQuitInstructions.color = 'r';
					}
					else
					{
						this.chatQuitInstructions.color = 'z';
					}
					this.customSkipTimeout = 2f;
					SHGUI.current.PlaySound(SHGUIsound.noescape);
					return;
				}
				if (this.messages.Count > 0 && this.messages[this.messages.Count - 1] is MCDSHGUIguruchatwindow)
				{
					(this.messages[this.messages.Count - 1] as MCDSHGUIguruchatwindow).Stop();
				}
				this.skipping = true;
			}
		}

		// Token: 0x06001F50 RID: 8016 RVA: 0x000D84E8 File Offset: 0x000D68E8
		public void DisplayNextMessage()
		{
			if (this.queue.Count > 0)
			{
				this.DisplayScrollMessage(this.queue[0]);
				this.queue.RemoveAt(0);
				this.isEmptyAndFinished = false;
			}
			else
			{
				if (this.killOnEmptyQueue)
				{
					this.Kill();
				}
				this.isEmptyAndFinished = true;
			}
		}

		// Token: 0x06001F51 RID: 8017 RVA: 0x000D8548 File Offset: 0x000D6948
		public float GetProgress()
		{
			return (float)this.queue.Count / (float)this.startQueueCount;
		}

		// Token: 0x06001F52 RID: 8018 RVA: 0x000D855E File Offset: 0x000D695E
		public int GetPendingMessagesCount()
		{
			return this.queue.Count;
		}

		// Token: 0x06001F53 RID: 8019 RVA: 0x000D856C File Offset: 0x000D696C
		public void ClearMessagesAbove(int y)
		{
			for (int i = 0; i < this.messages.Count; i++)
			{
				if (this.messages[i].y < y)
				{
					this.messages[i].Kill();
				}
			}
		}

		// Token: 0x06001F54 RID: 8020 RVA: 0x000D85C0 File Offset: 0x000D69C0
		public void ClearInstantMessagesAbove(int y)
		{
			for (int i = 0; i < this.messages.Count; i++)
			{
				if (this.messages[i].y < y)
				{
					this.messages[i].KillInstant();
				}
			}
		}

		// Token: 0x06001F55 RID: 8021 RVA: 0x000D8614 File Offset: 0x000D6A14
		public void FastForward()
		{
			MCDSHGUIview shguiview = this.messages.Last<MCDSHGUIview>();
			if (shguiview != null && shguiview is MCDSHGUIprompter)
			{
				MCDSHGUIprompter shguiprompter = shguiview as MCDSHGUIprompter;
				shguiprompter.PunchIn(1f);
				if (!shguiprompter.IsAlmostFinished())
				{
					shguiprompter.UpdateConsole(true);
				}
				shguiprompter.Update();
			}
			if (this.queue.Count > 0)
			{
				this.delay = -0.01f;
			}
		}

		// Token: 0x06001F56 RID: 8022 RVA: 0x000D8684 File Offset: 0x000D6A84
		public List<mcdscrollmessage> GetQueue()
		{
			return this.queue;
		}

		// Token: 0x06001F57 RID: 8023 RVA: 0x000D868C File Offset: 0x000D6A8C
		public List<MCDSHGUIview> GetMessage()
		{
			return this.messages;
		}

		// Token: 0x06001F58 RID: 8024 RVA: 0x000D8694 File Offset: 0x000D6A94
		public void SpeedUpConsole()
		{
			if (this.delay > 0.01f)
			{
				this.delay = 0.01f;
			}
			foreach (mcdscrollmessage scrollmessage in this.queue)
			{
				if (scrollmessage.view.GetType() == typeof(MCDSHGUIprompter))
				{
					(scrollmessage.view as MCDSHGUIprompter).currentCharDelay = -1f;
				}
			}
		}

		// Token: 0x04001F5C RID: 8028
		public int lines;

		// Token: 0x04001F5D RID: 8029
		public int maxlines = 22;

		// Token: 0x04001F5E RID: 8030
		public int desiredFrameWidth = 35;

		// Token: 0x04001F5F RID: 8031
		public int frameOffset;

		// Token: 0x04001F60 RID: 8032
		private int chatMargin = 3;

		// Token: 0x04001F61 RID: 8033
		protected List<MCDSHGUIview> messages;

		// Token: 0x04001F62 RID: 8034
		protected List<mcdscrollmessage> queue;

		// Token: 0x04001F63 RID: 8035
		protected float delay;

		// Token: 0x04001F64 RID: 8036
		private int startQueueCount = -100000;

		// Token: 0x04001F65 RID: 8037
		private MCDSHGUItext instructions;

		// Token: 0x04001F66 RID: 8038
		private MCDSHGUItext chatQuitInstructions;

		// Token: 0x04001F67 RID: 8039
		public MCDSHGUIview appname;

		// Token: 0x04001F68 RID: 8040
		public MCDSHGUIview clock;

		// Token: 0x04001F69 RID: 8041
		public MCDSHGUIview frame;

		// Token: 0x04001F6A RID: 8042
		public bool showInstruction = true;

		// Token: 0x04001F6B RID: 8043
		private float noPropmterInteractionTime;

		// Token: 0x04001F6C RID: 8044
		private int instructionsAnchorX;

		// Token: 0x04001F6D RID: 8045
		private int instructionsAnchorY;

		// Token: 0x04001F6E RID: 8046
		private const string typeInstruction = "TYPE TO HACK";

		// Token: 0x04001F6F RID: 8047
		private const string enterInstruction = "ENTER TO EXECUTE";

		// Token: 0x04001F70 RID: 8048
		public bool skippable = true;

		// Token: 0x04001F71 RID: 8049
		public bool customSkip;

		// Token: 0x04001F72 RID: 8050
		public List<string> customSkipMessages;

		// Token: 0x04001F73 RID: 8051
		public float customSkipTimeout = 1f;

		// Token: 0x04001F74 RID: 8052
		private bool skipping;

		// Token: 0x04001F75 RID: 8053
		public bool killOnEmptyQueue = true;

		// Token: 0x04001F76 RID: 8054
		public bool showFadeForChatMessages;

		// Token: 0x04001F77 RID: 8055
		public string leaveChatLocalizedString = "LEAVE_CHAT_INPUT";

		// Token: 0x04001F78 RID: 8056
		public string leaveChatOverrideString;

		// Token: 0x04001F79 RID: 8057
		public Action defaultConsoleCallback;

		// Token: 0x04001F7A RID: 8058
		public bool dontDisplaySender = true;

		// Token: 0x04001F7B RID: 8059
		public int width;

		// Token: 0x04001F7C RID: 8060
		public int killBelowThisY = -10;

		// Token: 0x04001F7D RID: 8061
		private ulong lastKey;

		// Token: 0x04001F7E RID: 8062
		public bool resetInputAxes;

		// Token: 0x04001F7F RID: 8063
		public bool isEmptyAndFinished;
	}

	public class mcdscrollmessage
	{
		// Token: 0x06001F26 RID: 7974 RVA: 0x000D6F0C File Offset: 0x000D530C
		public mcdscrollmessage(MCDSHGUIview View, int Height, float Delay, bool overrideLast = false, float baseFade = 0f, string controlCommand = "")
		{
			this.view = View;
			this.height = Height;
			this.delay = Delay;
			this.overrideLast = overrideLast;
			this.baseFade = baseFade;
			this.controlCommand = controlCommand;
		}

		// Token: 0x04001F55 RID: 8021
		public MCDSHGUIview view;

		// Token: 0x04001F56 RID: 8022
		public int height;

		// Token: 0x04001F57 RID: 8023
		public float delay;

		// Token: 0x04001F58 RID: 8024
		public bool overrideLast;

		// Token: 0x04001F59 RID: 8025
		public float baseFade;

		// Token: 0x04001F5A RID: 8026
		public string controlCommand = string.Empty;

		// Token: 0x04001F5B RID: 8027
		public Action method;
	}
}