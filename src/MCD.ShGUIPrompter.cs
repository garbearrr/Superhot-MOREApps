using System;
using System.Text;
using InControl;
using InputSystem;
using UnityEngine;

using MCD.ShGUIguruchatwindow;
using MCD.ShGUIText;

namespace MCD.ShGUIPrompter
{
	// Token: 0x020005BC RID: 1468
	public class MCDSHGUIprompter : MCDSHGUItext
	{
		// Token: 0x060020D8 RID: 8408 RVA: 0x000F5130 File Offset: 0x000F3530
		public MCDSHGUIprompter(int X, int Y, char Col) : base(string.Empty, X, Y, Col, false)
		{
			this.output = new StringBuilder();
		}

		// Token: 0x060020D9 RID: 8409 RVA: 0x000F51C0 File Offset: 0x000F35C0
		public void SetInput(string str, bool clear = true)
		{
			if (clear)
			{
				this.Clear();
			}
			this.input += str;
			if (str.Length > 2 && str[0] == '^' && str[1] == 'F' && this.parent != null && this.parent is MCDSHGUIguruchatwindow)
			{
				(this.parent as MCDSHGUIguruchatwindow).SetFrameColor(str[2]);
			}
			this.PrecomposeOutputText();
		}

		// Token: 0x060020DA RID: 8410 RVA: 0x000F524B File Offset: 0x000F364B
		public void Stop()
		{
			this.baseSpeed = 0;
			this.input = string.Empty;
			this.manualUpdate = false;
			this.drawCarriage = false;
			this.currentCharDelay = -1f;
			this.UpdateConsole(true);
			this.currentCharDelay = -1f;
		}

		// Token: 0x060020DB RID: 8411 RVA: 0x000F528A File Offset: 0x000F368A
		public void ResetCurrentCharDelay()
		{
			this.currentCharDelay = 0f;
		}

		// Token: 0x060020DC RID: 8412 RVA: 0x000F5297 File Offset: 0x000F3697
		public void AddPrefix(string prefix)
		{
			this.output.Append(prefix);
		}

		// Token: 0x060020DD RID: 8413 RVA: 0x000F52A8 File Offset: 0x000F36A8
		public override void Update()
		{
			base.Update();
			this.noInteractionTimer += Time.unscaledDeltaTime;
			this.initDelay -= Time.unscaledDeltaTime;
			if (this.initDelay > 0f)
			{
				return;
			}
			if (this.fade < 0.99f)
			{
				return;
			}
			for (int i = 0; i < this.baseSpeed; i++)
			{
				this.UpdateConsole(false);
			}
			if (this.fadingIn)
			{
				this.fade = 1f;
			}
		}

		// Token: 0x060020DE RID: 8414 RVA: 0x000F5338 File Offset: 0x000F3738
		public void ShowInstant()
		{
			float num = this.currentCharDelay;
			bool flag = this.muteSounds;
			this.muteSounds = true;
			while (this.currentChar < this.input.Length)
			{
				this.currentCharDelay = -1f;
				this.UpdateConsole(true);
			}
			this.muteSounds = flag;
			this.currentCharDelay = num;
		}

		// Token: 0x060020DF RID: 8415 RVA: 0x000F5395 File Offset: 0x000F3795
		public void Clear()
		{
			this.currentChar = 0;
			this.currentPrintChar = 0;
			this.input = string.Empty;
			this.output.Length = 0;
			this.output.Capacity = 0;
		}

		// Token: 0x060020E0 RID: 8416 RVA: 0x000F53C8 File Offset: 0x000F37C8
		private void PrecomposeOutputText()
		{
			int i = 0;
			this.output.Length = 0;
			while (i < this.input.Length)
			{
				char c = this.input[i++];
				if (c == '^')
				{
					char c2 = this.input[i++];
					switch (c2)
					{
						case 'C':
							goto IL_7B;
						default:
							if (c2 == 'M' || c2 == 'W' || c2 == 'm' || c2 == 'w')
							{
								goto IL_7B;
							}
							break;
						case 'F':
							if (this.parent != null && this.parent is SHGUIguruchatwindow)
							{
								i++;
							}
							break;
					}
					goto IL_D8;
				IL_7B:
					i++;
				}
				else if (c != '~')
				{
					this.output.Append(c);
				}
				else
				{
					this.output.Append(StringScrambler.GetGlitchChar());
				}
			IL_D8:
				this.text = this.output.ToString();
				base.SmartBreakTextForLineLength(this.maxLineLength);
				this.output.Insert(0, this.text);
				this.output.Length = this.text.Length;
				this.text = string.Empty;
			}
		}

		// Token: 0x060020E1 RID: 8417 RVA: 0x000F5510 File Offset: 0x000F3910
		public void UpdateConsole(bool force = false)
		{
			this.currentCharDelay -= Time.unscaledDeltaTime;
			if ((((!this.manualUpdate && this.currentCharDelay < 0f) || (this.manualUpdate && this.charBuffer > 0 && this.currentCharDelay < 0f)) && !this.IsFinished() && !this.waitsForEnter) || force)
			{
				this.waitMulti = 1f;
				if (this.currentChar < this.input.Length)
				{
					char nextChar = this.GetNextChar();
					if (nextChar == '^')
					{
						char nextChar2 = this.GetNextChar();
						if (nextChar2 == 'O')
						{
							if (this.thisConsoleCallback != null)
							{
								this.thisConsoleCallback();
							}
						}
						else if (nextChar2 == 'C')
						{
							this.color = this.GetNextChar();
						}
						else if (nextChar2 == 'I')
						{
							this.ignoreDefaultPunctuationWaits = !this.ignoreDefaultPunctuationWaits;
						}
						else if (nextChar2 == 'w')
						{
							this.waitMulti = (float)int.Parse(this.GetNextChar().ToString());
						}
						else if (nextChar2 == 'W')
						{
							this.waitMulti = (float)(int.Parse(this.GetNextChar().ToString()) * 10);
						}
						else if (nextChar2 == 'M')
						{
							this.waitMultiPersistent = (float)int.Parse(this.GetNextChar().ToString());
						}
						else if (nextChar2 == 'H')
						{
							this.waitMultiPersistent = 0.0001f;
						}
						else if (nextChar2 == 'Z')
						{
							this.waitMultiPersistent = 1.25f;
						}
						else if (nextChar2 == 'D')
						{
							this.baseSpeed++;
						}
						else if (nextChar2 == '>')
						{
							this.writingSpeed++;
						}
						else if (nextChar2 == '<')
						{
							this.writingSpeed--;
							if (this.writingSpeed < 1)
							{
								this.writingSpeed = 1;
							}
						}
						else if (nextChar2 == 'E')
						{
							this.waitsForEnter = true;
						}
						else if (nextChar2 == 'm')
						{
							this.waitMultiPersistent = (float)(1 / int.Parse(this.GetNextChar().ToString()));
						}
						else if (nextChar2 == 'F' && this.parent != null && this.parent is SHGUIguruchatwindow)
						{
							(this.parent as MCDSHGUIguruchatwindow).SetFrameColor(this.GetNextChar());
						}
					}
					else
					{
						this.charBuffer--;
						if (this.output[this.currentPrintChar] == '\n' && !char.IsWhiteSpace(nextChar))
						{
							this.currentPrintChar++;
						}
						this.currentPrintChar++;
						if (this.currentPrintChar < this.output.Length && this.output[this.currentPrintChar] == '\u007f')
						{
							this.charBuffer--;
							this.currentPrintChar++;
							nextChar = this.GetNextChar();
						}
						if (nextChar == ' ' || nextChar == '\n')
						{
							if (!this.ignoreDefaultPunctuationWaits)
							{
								this.waitMulti *= 4f;
							}
						}
						else if (nextChar == '.' || nextChar == '!' || nextChar == '?')
						{
							if (!this.ignoreDefaultPunctuationWaits)
							{
								this.waitMulti *= 5f;
							}
							if (!this.muteSounds)
							{
								if (this.color != 'r')
								{
									SHGUI.current.PlaySound(SHGUIsound.tick);
								}
								else
								{
									SHGUI.current.PlaySound(SHGUIsound.redtick);
								}
							}
						}
						else if (nextChar == ',')
						{
							if (!this.ignoreDefaultPunctuationWaits)
							{
								this.waitMulti *= 5f;
							}
							if (!this.muteSounds)
							{
								if (this.color != 'r')
								{
									SHGUI.current.PlaySound(SHGUIsound.tick);
								}
								else
								{
									SHGUI.current.PlaySound(SHGUIsound.redtick);
								}
							}
						}
						else if (!this.muteSounds)
						{
							SHGUI.current.PlaySound((this.color != 'r') ? SHGUIsound.tick : SHGUIsound.redtick);
						}
					}
				}
				else
				{
					this.charBuffer--;
				}
				this.currentCharDelay = this.baseCharDelay;
				this.currentCharDelay *= this.waitMulti * this.waitMultiPersistent;
				if (this.manualUpdate)
				{
					this.currentCharDelay = this.baseCharDelay;
				}
			}
			if (this.drawCarriage && !this.IsFinished())
			{
				char value = (Time.unscaledTime * 4f % 2f <= 1f) ? ' ' : '█';
				if (this.currentPrintChar == 0)
				{
					this.text = value.ToString();
				}
				else if (this.currentPrintChar < this.output.Length)
				{
					char value2 = this.output[this.currentPrintChar];
					this.output[this.currentPrintChar] = value;
					this.text = this.output.ToString(0, this.currentPrintChar + 1);
					this.output[this.currentPrintChar] = value2;
				}
				else
				{
					this.output.Append(value);
					this.text = this.output.ToString();
					this.output.Length--;
				}
			}
			else
			{
				this.text = this.output.ToString(0, this.currentPrintChar);
			}
			this.longestLineAfterSmartBreak = base.CountMaxLineLenght(this.text, this.currentPrintChar);
		}

		// Token: 0x060020E2 RID: 8418 RVA: 0x000F5B08 File Offset: 0x000F3F08
		private char GetNextChar()
		{
			return this.input[this.currentChar++];
		}

		// Token: 0x060020E3 RID: 8419 RVA: 0x000F5B34 File Offset: 0x000F3F34
		public bool IsFinished()
		{
			return this.currentChar >= this.input.Length && (!this.manualUpdate || this.confirmed) && this.currentCharDelay < 0f;
		}

		// Token: 0x060020E4 RID: 8420 RVA: 0x000F5B83 File Offset: 0x000F3F83
		public bool IsAlmostFinished()
		{
			return this.currentChar >= this.input.Length;
		}

		// Token: 0x060020E5 RID: 8421 RVA: 0x000F5B9C File Offset: 0x000F3F9C
		public int GetFirstLineLengthWithoutSpecialSigns()
		{
			int num = 0;
			for (int i = 0; i < this.input.Length; i++)
			{
				if (this.input[i] == '^')
				{
					num--;
					num--;
				}
				else
				{
					num++;
				}
				if (this.input[i] == '\n')
				{
					break;
				}
			}
			return num;
		}

		// Token: 0x060020E6 RID: 8422 RVA: 0x000F5C06 File Offset: 0x000F4006
		public void SwitchToManualInputMode()
		{
			this.charBuffer = 0;
			this.baseCharDelay = 0.016f;
			this.baseSpeed = 1;
			this.drawCarriage = true;
			this.manualUpdate = true;
		}

		// Token: 0x060020E7 RID: 8423 RVA: 0x000F5C30 File Offset: 0x000F4030
		public void ReactToInputKeyboard(SHGUIinput key)
		{
			if (this.fadingOut)
			{
				return;
			}
			base.ReactToInputKeyboard(key);
			if (key == SHGUIinput.enter && this.manualUpdate)
			{
				this.ReactionEnter();
			}
			if (this.charBuffer < 3 && (key == SHGUIinput.any || key == SHGUIinput.down || key == SHGUIinput.up || key == SHGUIinput.left || key == SHGUIinput.right))
			{
				this.ReactionTyping(false);
			}
			this.SpeedUpTyping(key);
		}

		// Token: 0x060020E8 RID: 8424 RVA: 0x000F5CA8 File Offset: 0x000F40A8
		public void ReactToInputMouse(int x, int y, bool clicked, SHGUIinput scroll)
		{
			if (this.fadingOut)
			{
				return;
			}
			bool flag = false;
			if ((this.lastXpos != x || this.lastYpos != y) && !this.mouseVirgin)
			{
				flag = true;
			}
			this.mouseVirgin = false;
			this.lastXpos = x;
			this.lastYpos = y;
			if (!clicked)
			{
				if (flag)
				{
					this.ReactionTyping(true);
				}
			}
			else if (clicked && this.IsAlmostFinished())
			{
				this.ReactionEnter();
			}
			base.ReactToInputMouse(x, y, clicked, scroll);
		}

		// Token: 0x060020E9 RID: 8425 RVA: 0x000F5D38 File Offset: 0x000F4138
		private void SpeedUpTyping(SHGUIinput key)
		{
			/*if (this.baseSpeed == 0)
			{
				return;
			}
			if (key == SHGUIinput.space)
			{
				this.baseSpeed = 3;
				if (this.manualUpdate)
				{
					this.manualUpdate = false;
					this.speedUpOfManual = true;
				}
				if (this.IsAlmostFinished() && InputManager.ActiveDevice.Name == SHInput.keyboardMouse)
				{
					this.ReactionEnter();
				}
			}
			else if (Input.GetKeyUp(KeyCode.Space) || ((InputManager.ActiveDevice.Button1.WasReleased || InputManager.ActiveDevice.Action1.WasReleased) && InputManager.ActiveDevice.Name != SHInput.keyboardMouse))
			{
				if (this.speedUpOfManual)
				{
					this.manualUpdate = true;
					this.charBuffer = 0;
					this.baseSpeed = 1;
				}
				else
				{
					this.baseSpeed = 1;
				}
			}
			if (this.IsAlmostFinished() && this.speedUpOfManual)
			{
				this.manualUpdate = true;
			}*/
		}

		// Token: 0x060020EA RID: 8426 RVA: 0x000F5E40 File Offset: 0x000F4240
		private void ReactionEnter()
		{
			if (this.IsAlmostFinished())
			{
				if (!this.confirmed && !this.muteSounds)
				{
					SHGUI.current.PlaySound(SHGUIsound.confirm);
				}
				this.confirmed = true;
			}
			if (this.waitsForEnter)
			{
				if (!this.muteSounds)
				{
					SHGUI.current.PlaySound(SHGUIsound.confirm);
				}
				this.waitsForEnter = false;
				this.charBuffer = 1;
			}
			this.noInteractionTimer = 0f;
		}

		// Token: 0x060020EB RID: 8427 RVA: 0x000F5EBA File Offset: 0x000F42BA
		public void SetConfirmed()
		{
			this.confirmed = true;
		}

		// Token: 0x060020EC RID: 8428 RVA: 0x000F5EC4 File Offset: 0x000F42C4
		private void ReactionTyping(bool slow = false)
		{
			if (this.charBuffer < this.writingSpeed)
			{
				if (!slow)
				{
					this.charBuffer += this.writingSpeed;
				}
				else
				{
					this.charBuffer++;
				}
				if (!this.IsAlmostFinished())
				{
					this.noInteractionTimer = 0f;
				}
			}
		}

		// Token: 0x040022A6 RID: 8870
		public string input = string.Empty;

		// Token: 0x040022A7 RID: 8871
		public StringBuilder output;

		// Token: 0x040022A8 RID: 8872
		public float baseCharDelay = 0.035f;

		// Token: 0x040022A9 RID: 8873
		public bool drawCarriage;

		// Token: 0x040022AA RID: 8874
		public float currentCharDelay;

		// Token: 0x040022AB RID: 8875
		private float waitMulti = 1f;

		// Token: 0x040022AC RID: 8876
		private float waitMultiPersistent = 1f;

		// Token: 0x040022AD RID: 8877
		public int currentChar;

		// Token: 0x040022AE RID: 8878
		public int currentPrintChar;

		// Token: 0x040022AF RID: 8879
		public int baseSpeed = 1;

		// Token: 0x040022B0 RID: 8880
		private bool ignoreDefaultPunctuationWaits;

		// Token: 0x040022B1 RID: 8881
		public int charBuffer = int.MaxValue;

		// Token: 0x040022B2 RID: 8882
		public bool manualUpdate;

		// Token: 0x040022B3 RID: 8883
		private bool confirmed;

		// Token: 0x040022B4 RID: 8884
		public int maxLineLength = 30;

		// Token: 0x040022B5 RID: 8885
		public int maxSmartBreakOffset = 7;

		// Token: 0x040022B6 RID: 8886
		public float initDelay = 0.3f;

		// Token: 0x040022B7 RID: 8887
		public Action thisConsoleCallback;

		// Token: 0x040022B8 RID: 8888
		public float noInteractionTimer;

		// Token: 0x040022B9 RID: 8889
		public bool muteSounds;

		// Token: 0x040022BA RID: 8890
		private bool speedUpOfManual;

		// Token: 0x040022BB RID: 8891
		public bool waitsForEnter;

		// Token: 0x040022BC RID: 8892
		private int lastXpos;

		// Token: 0x040022BD RID: 8893
		private int lastYpos;

		// Token: 0x040022BE RID: 8894
		private bool mouseVirgin = true;

		// Token: 0x040022BF RID: 8895
		public int writingSpeed = 3;
	}
}