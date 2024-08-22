using System;

using MCD.ShGUIFrame;
using MCD.ShGUIPrompter;
using MCD.ShGUIRect;
using MCD.ShGUIText;
using MCD.ShGUIView;

namespace MCD.ShGUIguruchatwindow
{
	// Token: 0x020005B1 RID: 1457
	public class MCDSHGUIguruchatwindow : MCDSHGUIview
	{
		// Token: 0x06002098 RID: 8344 RVA: 0x000F3F60 File Offset: 0x000F2360
		public MCDSHGUIguruchatwindow(string frameStyle = null)
		{
			this.frameStyle = frameStyle;
			this.background = (base.AddSubView(new MCDSHGUIrect(0, 0, 0, 0, 'z', ' ', 2)) as MCDSHGUIrect);
			this.frameElement = (base.AddSubView(new MCDSHGUIframe(0, 0, 0, 0, 'z', frameStyle, string.Empty, 'w')) as MCDSHGUIframe);
			this.labelElement = (base.AddSubView(new MCDSHGUItext(string.Empty, 1, 0, 'z', false)) as MCDSHGUItext);
			this.subElement = (base.AddSubView(new MCDSHGUItext(string.Empty, 1, 0, 'z', false)) as MCDSHGUItext);
			this.permanentInstruction = (base.AddSubView(new MCDSHGUItext(string.Empty, 1, 0, 'z', false)) as MCDSHGUItext);
			this.textElement = (base.AddSubView(new MCDSHGUIprompter(1, 1, 'w')) as MCDSHGUIprompter);
			this.SetContent(string.Empty);
		}

		// Token: 0x06002099 RID: 8345 RVA: 0x000F406C File Offset: 0x000F246C
		public MCDSHGUIguruchatwindow SetWidth(int w)
		{
			this.desiredWidth = w;
			this.textElement.maxLineLength = w - 2;
			return this;
		}

		// Token: 0x0600209A RID: 8346 RVA: 0x000F4084 File Offset: 0x000F2484
		public MCDSHGUIguruchatwindow SetCallback(Action a)
		{
			this.textElement.thisConsoleCallback = a;
			return this;
		}

		// Token: 0x0600209B RID: 8347 RVA: 0x000F4093 File Offset: 0x000F2493
		public MCDSHGUIguruchatwindow SetContent(string text)
		{
			this.message = text;
			this.textElement.SetInput(text, true);
			return this;
		}

		// Token: 0x0600209C RID: 8348 RVA: 0x000F40AA File Offset: 0x000F24AA
		public void SetFrameColor(char color)
		{
			this.frameElement.color = color;
			this.subElement.color = color;
			this.permanentInstruction.color = color;
		}

		// Token: 0x0600209D RID: 8349 RVA: 0x000F40D0 File Offset: 0x000F24D0
		public void Fit(bool force = false)
		{
			if (this.textElement.IsFinished() && !force)
			{
				return;
			}
			int num = this.height;
			this.width = this.textElement.longestLineAfterSmartBreak + 2;
			if (this.width < this.labelElement.text.Length + 1)
			{
				this.width = this.labelElement.text.Length + 1;
			}
			if (this.width < this.subElement.text.Length + 1)
			{
				this.width = this.subElement.text.Length + 1;
			}
			if (this.width < this.permanentInstruction.text.Length + 1)
			{
				this.width = this.permanentInstruction.text.Length + 1;
			}
			this.height = this.textElement.CountLines() + 1;
			this.RefreshFrames();
			this.height++;
			if (this.height != num && this.height > 3)
			{
				this.frameElement.PunchIn(0.4f);
			}
		}

		// Token: 0x0600209E RID: 8350 RVA: 0x000F41FC File Offset: 0x000F25FC
		public int GetHeightOfCompleteTextWithFrameVERYSLOWandMOODY()
		{
			MCDSHGUIprompter shguiprompter = new MCDSHGUIprompter(0, 0, 'w');
			MCDSHGUIprompter shguiprompter2 = this.textElement;
			shguiprompter.muteSounds = true;
			shguiprompter.SetInput(shguiprompter2.input, true);
			shguiprompter.maxLineLength = shguiprompter2.maxLineLength;
			shguiprompter.ShowInstant();
			return shguiprompter.CountLines() + 2;
		}

		// Token: 0x0600209F RID: 8351 RVA: 0x000F4248 File Offset: 0x000F2648
		public MCDSHGUIprompter GetPrompter()
		{
			return this.textElement;
		}

		// Token: 0x060020A0 RID: 8352 RVA: 0x000F4250 File Offset: 0x000F2650
		private void RefreshFrames()
		{
			this.frameElement.remove = true;
			this.frameElement.hidden = true;
			float fade = this.frameElement.fade;
			this.frameElement = (base.AddSubViewBottom(new MCDSHGUIframe(-1, 0, this.width, this.height, this.frameElement.color, this.frameStyle, string.Empty, 'w')) as MCDSHGUIframe);
			this.frameElement.PunchIn(fade);
			this.background.remove = true;
			this.background.hidden = true;
			float fade2 = this.background.fade;
			this.background = (base.AddSubViewBottom(new MCDSHGUIrect(-1, 0, this.width, this.height, '0', ' ', 2)) as MCDSHGUIrect);
			this.background.PunchIn(fade2);
		}

		// Token: 0x060020A1 RID: 8353 RVA: 0x000F4324 File Offset: 0x000F2724
		public override void Update()
		{
			base.Update();
			if (!this.playedSound)
			{
				this.playedSound = true;
				if (this.Align == SHAlign.Left)
				{
					if (this.frameElement.color != 'r')
					{
						SHGUI.current.PlaySound(SHGUIsound.pong);
					}
					else
					{
						SHGUI.current.PlaySound(SHGUIsound.redpong);
					}
				}
				else if (this.frameElement.color != 'r')
				{
					SHGUI.current.PlaySound(SHGUIsound.ping);
				}
				else
				{
					SHGUI.current.PlaySound(SHGUIsound.redping);
				}
			}
			this.Fit(false);
			this.UpdateInstructions();
			this.UpdatePermanentInstruction();
			if (this.poorMode)
			{
			}
			this.finished = this.textElement.IsFinished();
		}

		// Token: 0x060020A2 RID: 8354 RVA: 0x000F43E2 File Offset: 0x000F27E2
		private void UpdatePermanentInstruction()
		{
			this.permanentInstruction.x = this.width - 2;
			this.permanentInstruction.y = this.height - 1;
			this.permanentInstruction.GoFromRight();
		}

		// Token: 0x060020A3 RID: 8355 RVA: 0x000F4416 File Offset: 0x000F2816
		public MCDSHGUIguruchatwindow SetAlign(SHAlign Align)
		{
			this.Align = Align;
			return this;
		}

		// Token: 0x060020A4 RID: 8356 RVA: 0x000F4420 File Offset: 0x000F2820
		public MCDSHGUIguruchatwindow SetInteractive()
		{
			this.textElement.drawCarriage = true;
			this.textElement.SwitchToManualInputMode();
			return this;
		}

		// Token: 0x060020A5 RID: 8357 RVA: 0x000F443A File Offset: 0x000F283A
		public void ShowInstantPunchIn()
		{
			this.textElement.ShowInstant();
			this.textElement.PunchIn(0.2f);
			this.textElement.SetConfirmed();
		}

		// Token: 0x060020A6 RID: 8358 RVA: 0x000F4462 File Offset: 0x000F2862
		public MCDSHGUIguruchatwindow SetLabel(string text)
		{
			this.labelElement.text = text;
			return this;
		}

		// Token: 0x060020A7 RID: 8359 RVA: 0x000F4474 File Offset: 0x000F2874
		public override void ReactToInputKeyboard(SHGUIinput key)
		{
			if (this.fadingOut)
			{
				return;
			}
			base.ReactToInputKeyboard(key);
			if (key == SHGUIinput.any && this.subElement != null && this.subElement.text != LocalizationManager.Instance.GetLocalized("TYPE_TO_HACK_INPUT"))
			{
				this.subElement.PunchIn(0.6f);
			}
		}

		// Token: 0x060020A8 RID: 8360 RVA: 0x000F44DC File Offset: 0x000F28DC
		private void UpdateInstructions()
		{
			if (this.subElement == null)
			{
				return;
			}
			string text = this.subElement.text;
			if (this.showInstructions && !this.textElement.IsFinished())
			{
				if (this.textElement.manualUpdate)
				{
					if (this.textElement.IsAlmostFinished())
					{
						this.subElement.text = LocalizationManager.Instance.GetLocalized("ENTER_TO_SEND_INPUT");
					}
					else if (!this.textElement.waitsForEnter)
					{
						this.subElement.text = LocalizationManager.Instance.GetLocalized("TYPE_TO_HACK_INPUT");
					}
					else
					{
						this.subElement.text = LocalizationManager.Instance.GetLocalized("ENTER_TO_SEND_INPUT");
					}
				}
				else
				{
					this.subElement.text = string.Empty;
				}
				this.subElement.x = this.width - 2;
				this.subElement.y = this.height - 1;
				this.subElement.GoFromRight();
				if (text != this.subElement.text)
				{
					this.subElement.PunchIn(0.6f);
				}
			}
			else if (this.subElement != null && !this.subElement.fadingOut)
			{
				this.subElement.Kill();
			}
			if (this.textElement.IsFinished())
			{
				this.subElement.Kill();
			}
		}

		// Token: 0x060020A9 RID: 8361 RVA: 0x000F4657 File Offset: 0x000F2A57
		public void Stop()
		{
			this.showInstructions = false;
			this.textElement.Stop();
		}

		// Token: 0x060020AA RID: 8362 RVA: 0x000F466B File Offset: 0x000F2A6B
		public void ShowPermanentInstruction(string instruction = "")
		{
			this.permanentInstruction.text = instruction;
		}

		// Token: 0x04002243 RID: 8771
		public string message = string.Empty;

		// Token: 0x04002244 RID: 8772
		public MCDSHGUIprompter textElement;

		// Token: 0x04002245 RID: 8773
		public MCDSHGUItext labelElement;

		// Token: 0x04002246 RID: 8774
		public MCDSHGUItext subElement;

		// Token: 0x04002247 RID: 8775
		public MCDSHGUItext permanentInstruction;

		// Token: 0x04002248 RID: 8776
		public MCDSHGUIframe frameElement;

		// Token: 0x04002249 RID: 8777
		public MCDSHGUIrect background;

		// Token: 0x0400224A RID: 8778
		private string frameStyle;

		// Token: 0x0400224B RID: 8779
		public int height = 1;

		// Token: 0x0400224C RID: 8780
		public int width = 30;

		// Token: 0x0400224D RID: 8781
		public int desiredWidth = 30;

		// Token: 0x0400224E RID: 8782
		public SHAlign Align;

		// Token: 0x0400224F RID: 8783
		private int currentIndex;

		// Token: 0x04002250 RID: 8784
		public bool finished;

		// Token: 0x04002251 RID: 8785
		private bool confirmed;

		// Token: 0x04002252 RID: 8786
		public bool showInstructions = true;

		// Token: 0x04002253 RID: 8787
		public bool poorMode;

		// Token: 0x04002254 RID: 8788
		public bool playedSound;
	}
}