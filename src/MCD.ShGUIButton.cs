using System;

using MCD.ShGUIText;

namespace MCD.ShGUIButton
{
	// Token: 0x020006B1 RID: 1713
	public class MCDSHGUIButton : MCDSHGUItext
	{
		// Token: 0x060027CB RID: 10187 RVA: 0x0011CFA5 File Offset: 0x0011B3A5
		public MCDSHGUIButton(string Text, int X, int Y, char Col) : base(Text, X, Y, Col, false)
		{
		}

		// Token: 0x060027CC RID: 10188 RVA: 0x0011CFB4 File Offset: 0x0011B3B4
		public override void Redraw(int offx, int offy)
		{
			if (this.hidden)
			{
				return;
			}
			this.xGlobal = offx + this.x;
			this.yGlobal = offy + this.y;
			SHGUI.current.DrawText(this.text, this.x + offx, this.y + offy, this.color, this.fade, this.backColor, false, false);
			base.Redraw(offx, offy);
		}

		// Token: 0x060027CD RID: 10189 RVA: 0x0011D028 File Offset: 0x0011B428
		public override void ReactToInputMouse(int x, int y, bool clicked, SHGUIinput scroll)
		{
			base.ReactToInputMouse(x, y, clicked, scroll);
			if (x >= this.xGlobal && x <= this.xGlobal + base.GetLongestLineLength() && y >= this.yGlobal && y < this.yGlobal + base.CountLines())
			{
				for (int i = this.xGlobal; i <= this.xGlobal + base.GetLongestLineLength(); i++)
				{
					for (int j = this.yGlobal; j < this.yGlobal + base.CountLines(); j++)
					{
						SHGUI.current.SetPixelBack('█', i, j, 'r');
						if (clicked && this.OnActivate != null)
						{
							this.OnActivate();
						}
					}
				}
				if (clicked && this.OnActivate != null)
				{
					this.OnActivate();
				}
			}
			else
			{
				for (int k = this.xGlobal; k <= this.xGlobal + base.GetLongestLineLength(); k++)
				{
					for (int l = this.yGlobal; l < this.yGlobal + base.CountLines(); l++)
					{
						SHGUI.current.SetPixelBack(' ', k, l, 'w');
					}
				}
			}
		}

		// Token: 0x060027CE RID: 10190 RVA: 0x0011D16A File Offset: 0x0011B56A
		public override void ReactToInputKeyboard(SHGUIinput key)
		{
			base.ReactToInputKeyboard(key);
			if (key == this.ActionKey && this.OnActivate != null)
			{
				this.OnActivate();
			}
		}

		// Token: 0x060027CF RID: 10191 RVA: 0x0011D195 File Offset: 0x0011B595
		public MCDSHGUIButton SetActionKey(SHGUIinput key)
		{
			this.ActionKey = key;
			return this;
		}

		// Token: 0x060027D0 RID: 10192 RVA: 0x0011D19F File Offset: 0x0011B59F
		public MCDSHGUIButton SetOnActivate(Action func)
		{
			this.OnActivate = func;
			return this;
		}

		// Token: 0x04002A57 RID: 10839
		public Action OnActivate;

		// Token: 0x04002A58 RID: 10840
		public SHGUIinput ActionKey;

		// Token: 0x04002A59 RID: 10841
		private int xGlobal;

		// Token: 0x04002A5A RID: 10842
		private int yGlobal;
	}
}
