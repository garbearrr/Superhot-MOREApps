using System;

using MCD.ShGUIView;

namespace MCD.ShGUIRect
{
	// Token: 0x02000375 RID: 885
	public class MCDSHGUIrect : MCDSHGUIview
	{
		// Token: 0x060016AF RID: 5807 RVA: 0x0009CC7C File Offset: 0x0009AE7C
		public MCDSHGUIrect(int Startx, int Starty, int Endx, int Endy, char Col = '0', char C = ' ', int Mode = 2)
		{
			base.Init();
			this.startx = Startx;
			this.starty = Starty;
			this.endx = Endx;
			this.endy = Endy;
			this.SetColor(Col);
			this.c = C;
			this.mode = Mode;
		}

		// Token: 0x060016B0 RID: 5808 RVA: 0x0009CCCB File Offset: 0x0009AECB
		public MCDSHGUIrect SetChar(char fillChar)
		{
			this.c = fillChar;
			return this;
		}

		// Token: 0x060016B1 RID: 5809 RVA: 0x0009CCD8 File Offset: 0x0009AED8
		public override void Redraw(int offx, int offy)
		{
			if (this.hidden)
			{
				return;
			}
			int num = (int)((float)this.startx + (float)this.endx * (1f - this.fade)) + offx;
			int num2 = (int)((float)this.endx * this.fade) + offx;
			int num3 = (int)((float)this.starty + (float)this.endy * (1f - this.fade)) + offy;
			int num4 = (int)((float)this.endy * this.fade) + offy;
			for (int i = num; i <= num2; i++)
			{
				for (int j = num3; j <= num4; j++)
				{
					if (this.mode == 0 || this.mode == 2)
					{
						SHGUI.current.SetPixelFront(this.c, i, j, this.color);
					}
					if (this.mode == 1 || this.mode == 2)
					{
						SHGUI.current.SetPixelBack(this.c, i, j, this.color);
					}
				}
			}
			base.Redraw(this.x + offx, this.y + offy);
		}

		// Token: 0x0400147D RID: 5245
		public int startx;

		// Token: 0x0400147E RID: 5246
		public int starty;

		// Token: 0x0400147F RID: 5247
		public int endx;

		// Token: 0x04001480 RID: 5248
		public int endy;

		// Token: 0x04001481 RID: 5249
		public char c;

		// Token: 0x04001482 RID: 5250
		public int mode;
	}
}