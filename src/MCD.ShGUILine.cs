using System;

using MCD.ShGUIView;

namespace MCD.ShGUILine
{

	// Token: 0x0200036C RID: 876
	public class MCDSHGUIline : MCDSHGUIview
	{
		// Token: 0x06001669 RID: 5737 RVA: 0x0009ABC6 File Offset: 0x00098DC6
		public MCDSHGUIline(int Startpos, int Endpos, int Colpos, bool Horizontal, char Col)
		{
			base.Init();
			this.startpos = Startpos;
			this.endpos = Endpos;
			this.colpos = Colpos;
			this.horizontal = Horizontal;
			this.SetColor(Col);
		}

		// Token: 0x0600166A RID: 5738 RVA: 0x0009AC05 File Offset: 0x00098E05
		public MCDSHGUIline SetStyle(string Style)
		{
			this.style = Style;
			return this;
		}

		// Token: 0x0600166B RID: 5739 RVA: 0x0009AC10 File Offset: 0x00098E10
		public override void Redraw(int offx, int offy)
		{
			if (this.horizontal)
			{
				SHGUI.current.DrawLine(this.style, this.startpos + offx, this.endpos + offx, this.colpos + offy, this.horizontal, this.color, this.fade);
				return;
			}
			SHGUI.current.DrawLine(this.style, this.startpos + offy, this.endpos + offy, this.colpos + offx, this.horizontal, this.color, this.fade);
		}

		// Token: 0x04001429 RID: 5161
		private string style = "+-+";

		// Token: 0x0400142A RID: 5162
		private int startpos;

		// Token: 0x0400142B RID: 5163
		internal int endpos;

		// Token: 0x0400142C RID: 5164
		internal int colpos;

		// Token: 0x0400142D RID: 5165
		private bool horizontal;
	}
}