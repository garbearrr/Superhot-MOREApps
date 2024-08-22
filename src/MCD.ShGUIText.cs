using System;
using System.Text;
using Topten.RichTextKit;
using UnityEngine;

using MCD.Audio;

// Token: 0x020005C5 RID: 1477
namespace MCD.ShGUIText {
	public class MCDSHGUItext : SHGUIview
	{
		public struct GlitchStruct
		{
			// Token: 0x0600207F RID: 8319 RVA: 0x0008343A File Offset: 0x0008183A
			public GlitchStruct(char letter, float delay, string glitches = "", bool silent = false)
			{
				this.ButtonLetter = letter;
				this.Letter = letter;
				this.Delay = delay;
				this.TTL = delay;
				this.Decoded = false;
				this.Silent = silent;
				this.glitches = glitches;
				this.GlitchLetter();
			}

			// Token: 0x06002080 RID: 8320 RVA: 0x00083474 File Offset: 0x00081874
			public void GlitchLetter()
			{
				if (this.glitches.Length > 0)
				{
					this.Letter = this.glitches[UnityEngine.Random.Range(0, this.glitches.Length)];
				}
				else
				{
					this.Letter = UnusedBlocks[UnityEngine.Random.Range(0, UnusedBlocks.Length)];
				}
				this.Delay = this.TTL;
			}

			public static readonly string UnusedBlocks = "1234567890ABCDEF";

			// Token: 0x04002213 RID: 8723
			public char Letter;

			// Token: 0x04002214 RID: 8724
			public char ButtonLetter;

			// Token: 0x04002215 RID: 8725
			public float Delay;

			// Token: 0x04002216 RID: 8726
			public float TTL;

			// Token: 0x04002217 RID: 8727
			public bool Decoded;

			// Token: 0x04002218 RID: 8728
			public string glitches;

			// Token: 0x04002219 RID: 8729
			public bool Silent;
		}
		// Token: 0x06002133 RID: 8499 RVA: 0x000F3064 File Offset: 0x000F1464
		public MCDSHGUItext(string Text, int X, int Y, char Col, bool drawSpaces = false)
		{
			base.Init();
			this.text = Text;
			this.x = X;
			this.y = Y;
			this.DrawSpaces = drawSpaces;
			this.SetColor(Col);
		}

		// Token: 0x06002134 RID: 8500 RVA: 0x000F30B4 File Offset: 0x000F14B4
		public override void Redraw(int offx, int offy)
		{
			if (this.hidden)
			{
				return;
			}
			if (this.ConstantScramble)
			{
				this.text = StringScrambler.GetScrambledString(this.text, 0.5f, "▀▄█▐░▒▓■▪");
			}
			SHGUI.current.DrawText(this.text, this.x + offx, this.y + offy, this.color, this.fade, this.backColor, this.DrawSpaces, false);
			base.Redraw(offx, offy);
		}

		// Token: 0x06002135 RID: 8501 RVA: 0x000F3134 File Offset: 0x000F1534
		public MCDSHGUItext SetBackColor(char c)
		{
			this.backColor = c;
			return this;
		}

		// Token: 0x06002136 RID: 8502 RVA: 0x000F313E File Offset: 0x000F153E
		public MCDSHGUItext GoFromRight()
		{
			this.x -= this.GetLineLength() - 1;
			return this;
		}

		// Token: 0x06002137 RID: 8503 RVA: 0x000F3158 File Offset: 0x000F1558
		public MCDSHGUItext SmartBreakTextForLineLength(int lenght)
		{
			this.longestLineAfterSmartBreak = this.CountMaxLineLenght(this.text, -1);
			if (this.longestLineAfterSmartBreak > lenght)
			{
				this.longestLineAfterSmartBreak = -1;
				MCDSHGUItext.sLineBreaker.Reset(this.text);
				LineBreak lineBreak = new LineBreak(-1, -1, false);
				int num = 0;
				MCDSHGUItext.sStringBuffer.Length = 0;
				LineBreak lineBreak2;
				while (MCDSHGUItext.sLineBreaker.NextBreak(out lineBreak2))
				{
					if (lineBreak2.Required || lineBreak2.PositionMeasure - num > lenght)
					{
						if (lineBreak2.Required)
						{
							if (lineBreak2.PositionMeasure - num > lenght)
							{
								if (lineBreak.PositionMeasure - num > 0)
								{
									MCDSHGUItext.sStringBuffer.Append(this.text, num, lineBreak.PositionMeasure - num);
								}
								MCDSHGUItext.sStringBuffer.Append('\n');
								num = lineBreak.PositionWrap;
							}
							lineBreak = lineBreak2;
						}
						int num2 = lineBreak.PositionMeasure - num;
						if (num2 > 0)
						{
							MCDSHGUItext.sStringBuffer.Append(this.text, num, num2);
						}
						MCDSHGUItext.sStringBuffer.Append('\n');
						num = lineBreak.PositionWrap;
					}
					lineBreak = lineBreak2;
				}
				if (num != lineBreak.PositionWrap)
				{
					MCDSHGUItext.sStringBuffer.Append(this.text, num, lineBreak.PositionWrap - num);
				}
				this.text = MCDSHGUItext.sStringBuffer.ToString();
				this.longestLineAfterSmartBreak = this.CountMaxLineLenght(this.text, -1);
			}
			return this;
		}

		// Token: 0x06002138 RID: 8504 RVA: 0x000F32C8 File Offset: 0x000F16C8
		protected int CountMaxLineLenght(string text, int endCharacterIndex = -1)
		{
			int num = 0;
			int num2 = 0;
			int num3 = (endCharacterIndex != -1) ? Mathf.Min(text.Length, endCharacterIndex) : text.Length;
			for (int i = 0; i < num3; i++)
			{
				if (text[i] == '\n')
				{
					if (num > num2)
					{
						num2 = num;
					}
					num = 0;
				}
				else
				{
					num++;
				}
			}
			if (num > num2)
			{
				num2 = num;
			}
			return num2;
		}

		// Token: 0x06002139 RID: 8505 RVA: 0x000F3338 File Offset: 0x000F1738
		public MCDSHGUItext BreakTextForLineLength(int length)
		{
			int num = 0;
			for (int i = 0; i < this.text.Length; i++)
			{
				num++;
				if (this.text[i] == '\n')
				{
					num = 0;
				}
				if (num > length)
				{
					this.text = this.text.Insert(i, "\n");
					i++;
					num = 1;
				}
			}
			return this;
		}

		// Token: 0x0600213A RID: 8506 RVA: 0x000F33A4 File Offset: 0x000F17A4
		public MCDSHGUItext CenterTextForLineLength(int length)
		{
			string str = string.Empty;
			int num = 0;
			string text = string.Empty;
			for (int i = 0; i < this.text.Length; i++)
			{
				num++;
				text += this.text[i];
				if (this.text[i] == '\n')
				{
					int num2 = (length - text.Length) / 2;
					for (int j = 0; j < num2; j++)
					{
						str += " ";
					}
					str += text;
					text = string.Empty;
					num = 0;
				}
			}
			this.text = str;
			return this;
		}

		// Token: 0x0600213B RID: 8507 RVA: 0x000F3454 File Offset: 0x000F1854
		public MCDSHGUItext SmartBreak(int lenght, int maxoffset = 7)
		{
			int num = 0;
			int num2 = 0;
			this.longestLineAfterSmartBreak = 0;
			for (int i = 0; i < this.text.Length; i++)
			{
				num++;
				if (this.text[i] == '\n')
				{
					num = 0;
				}
				if (num > lenght)
				{
					if (this.text[i] == ' ' || num2 >= maxoffset)
					{
						if (this.text[i] == ' ')
						{
							this.text = this.text.Insert(i + 1, "\n");
							this.text = this.text.Remove(i, 1);
						}
						else
						{
							this.text = this.text.Insert(i, "\n");
						}
						i++;
						num = 1;
						num2 = 0;
					}
					else
					{
						num2++;
					}
				}
				if (num > this.longestLineAfterSmartBreak)
				{
					this.longestLineAfterSmartBreak = num;
				}
			}
			return this;
		}

		// Token: 0x0600213C RID: 8508 RVA: 0x000F3544 File Offset: 0x000F1944
		public int GetLineLength()
		{
			int i;
			for (i = 0; i < this.text.Length; i++)
			{
				if (this.text[i] == '\n')
				{
					return i - 1;
				}
			}
			return i - 1;
		}

		// Token: 0x0600213D RID: 8509 RVA: 0x000F358C File Offset: 0x000F198C
		public int GetLongestLineLength()
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < this.text.Length; i++)
			{
				if (this.text[i] == '\n' || i == this.text.Length - 1)
				{
					if (num2 > num)
					{
						num = num2;
					}
					num2 = 0;
				}
				else
				{
					num2++;
				}
			}
			return num;
		}

		// Token: 0x0600213E RID: 8510 RVA: 0x000F35F4 File Offset: 0x000F19F4
		public MCDSHGUItext CutTextForLineLength(int lenght)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = 1;
			for (int i = 0; i < this.text.Length; i++)
			{
				if (this.text[i] == '\n')
				{
					num = 0;
				}
				if (num <= lenght)
				{
					stringBuilder.Append(this.text[i]);
				}
				num++;
			}
			this.text = stringBuilder.ToString();
			return this;
		}

		// Token: 0x0600213F RID: 8511 RVA: 0x000F366C File Offset: 0x000F1A6C
		public MCDSHGUItext CutTextForMaxLines(int lines)
		{
			int num = 0;
			for (int i = 0; i < this.text.Length; i++)
			{
				if (this.text[i] == '\n')
				{
					num++;
				}
				if (num > lines)
				{
					this.text = this.text.Substring(0, i);
					return this;
				}
			}
			return this;
		}

		// Token: 0x06002140 RID: 8512 RVA: 0x000F36CD File Offset: 0x000F1ACD
		public MCDSHGUItext BreakCut(int lenght, int height)
		{
			return this.BreakTextForLineLength(lenght).CutTextForMaxLines(height);
		}

		// Token: 0x06002141 RID: 8513 RVA: 0x000F36DC File Offset: 0x000F1ADC
		public int CountLines()
		{
			int num = 1;
			for (int i = 0; i < this.text.Length; i++)
			{
				if (this.text[i] == '\n')
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06002142 RID: 8514 RVA: 0x000F371F File Offset: 0x000F1B1F
		public override void Update()
		{
			base.Update();
			this.UpdateGlitchButton();
			this.UpdateDecodedButton();
		}

		// Token: 0x06002143 RID: 8515 RVA: 0x000F3734 File Offset: 0x000F1B34
		public void MarkAsGlitched(string glitches = "", float minDelay = 0.4f, float maxDelay = 0.7f)
		{
			this.IsGlitched = true;
			this.color = 'z';
			this.GlitchButtonLenght = this.text.Length;
			this.Glitches = new GlitchStruct[this.GlitchButtonLenght];
			for (int i = 0; i < this.Glitches.Length; i++)
			{
				char letter = this.text[i];
				this.Glitches[i] = new GlitchStruct(letter, UnityEngine.Random.Range(minDelay, maxDelay), glitches, false);
			}
		}

		// Token: 0x06002144 RID: 8516 RVA: 0x000F37B8 File Offset: 0x000F1BB8
		public void MarkAsGlitchedWithText(string glitches = "", float minDelay = 0.4f, float maxDelay = 0.7f)
		{
			this.IsGlitched = true;
			this.color = 'z';
			this.GlitchButtonLenght = glitches.Length;
			this.Glitches = new GlitchStruct[this.GlitchButtonLenght];
			for (int i = 0; i < this.Glitches.Length; i++)
			{
				char letter = ' ';
				this.Glitches[i] = new GlitchStruct(letter, UnityEngine.Random.Range(minDelay, maxDelay), glitches[i].ToString(), false);
			}
		}

		// Token: 0x06002145 RID: 8517 RVA: 0x000F3841 File Offset: 0x000F1C41
		public void MarkAsDecoded(float timeToDecode, char toColor = 'w')
		{
			this.ButtonGlitch = this.text;
			this.TimeToDecode = timeToDecode;
			this.StartDecoding = true;
		}

		// Token: 0x06002146 RID: 8518 RVA: 0x000F3860 File Offset: 0x000F1C60
		private void UpdateGlitchButton()
		{
			if (!this.IsGlitched)
			{
				return;
			}
			char[] array = new char[this.GlitchButtonLenght];
			for (int i = 0; i < this.Glitches.Length; i++)
			{
				GlitchStruct[] glitches = this.Glitches;
				int num = i;
				glitches[num].Delay = glitches[num].Delay - Time.unscaledDeltaTime;
				if (this.Glitches[i].Delay < 0f)
				{
					this.PlayRandomGlitchSound();
					this.Glitches[i].GlitchLetter();
				}
				array[i] = this.Glitches[i].Letter;
			}
			this.text = new string(array);
			if (this.StartDecoding)
			{
				this.TimeToDecode -= Time.unscaledDeltaTime;
				if (this.TimeToDecode < 0f)
				{
					this.IsGlitched = false;
					this.IsDecoding = true;
				}
			}
		}

		// Token: 0x06002147 RID: 8519 RVA: 0x000F3948 File Offset: 0x000F1D48
		private void PlayRandomGlitchSound()
		{
			if (this.SilentGlitch)
			{
				return;
			}
			AudioManager.Instance.PlayClip(MCDAudio.LoadWavFromEmbeddedResource("AC_NonTerminal_Hackpool_Char1.wav"), null);
			//AudioManager.Instance.PlayClip(AudioResources.NonTerminal.HackPoolChar, null);
		}

		// Token: 0x06002148 RID: 8520 RVA: 0x000F3980 File Offset: 0x000F1D80
		private void UpdateDecodedButton()
		{
			if (!this.IsDecoding)
			{
				return;
			}
			char[] array = new char[this.GlitchButtonLenght];
			for (int i = 0; i < this.Glitches.Length; i++)
			{
				GlitchStruct[] glitches = this.Glitches;
				int num = i;
				glitches[num].Delay = glitches[num].Delay - Time.unscaledDeltaTime;
				if (this.Glitches[i].Delay < 0f)
				{
					array[i] = this.Glitches[i].ButtonLetter;
					this.Glitches[i].Decoded = true;
				}
				else
				{
					array[i] = this.Glitches[i].Letter;
				}
			}
			this.text = new string(array);
			this.IsDecoding = false;
			foreach (GlitchStruct glitchStruct in this.Glitches)
			{
				if (!glitchStruct.Decoded)
				{
					this.IsDecoding = true;
				}
			}
		}

		// Token: 0x06002149 RID: 8521 RVA: 0x000F3A88 File Offset: 0x000F1E88
		public void ResetGlitch()
		{
			this.IsGlitched = false;
			this.IsDecoding = false;
			this.StartDecoding = false;
		}

		// Token: 0x0600214A RID: 8522 RVA: 0x000F3A9F File Offset: 0x000F1E9F
		public MCDSHGUItext ChangePositionX(int newX)
		{
			this.x = newX;
			return this;
		}

		// Token: 0x040022F6 RID: 8950
		public string text;

		// Token: 0x040022F7 RID: 8951
		public int longestLineAfterSmartBreak;

		// Token: 0x040022F8 RID: 8952
		public char backColor = ' ';

		// Token: 0x040022F9 RID: 8953
		public bool DrawSpaces;

		// Token: 0x040022FA RID: 8954
		public bool ConstantScramble;

		// Token: 0x040022FB RID: 8955
		private bool IsGlitched;

		// Token: 0x040022FC RID: 8956
		private int GlitchButtonLenght;

		// Token: 0x040022FD RID: 8957
		private GlitchStruct[] Glitches;

		// Token: 0x040022FE RID: 8958
		public bool SilentGlitch = true;

		// Token: 0x040022FF RID: 8959
		private string ButtonGlitch;

		// Token: 0x04002300 RID: 8960
		private float TimeToDecode;

		// Token: 0x04002301 RID: 8961
		private bool StartDecoding;

		// Token: 0x04002302 RID: 8962
		private bool IsDecoding;

		// Token: 0x04002303 RID: 8963
		private static LineBreaker sLineBreaker = new LineBreaker();

		// Token: 0x04002304 RID: 8964
		private static StringBuilder sStringBuffer = new StringBuilder(128);
	}
}