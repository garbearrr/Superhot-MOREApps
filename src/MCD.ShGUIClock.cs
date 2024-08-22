using System;
using System.Text;

using MCD.ShGUIView;

namespace MCD.ShGUIClock
{
	// Token: 0x020005AB RID: 1451
	public class MCDSHGUIclock : MCDSHGUIview
	{
		// Token: 0x0600204C RID: 8268 RVA: 0x000F2EC6 File Offset: 0x000F12C6
		public MCDSHGUIclock(int X, int Y, char col)
		{
			this.x = X;
			this.y = Y;
			this.SetColor(col);
			this.lastDate = DateTime.Now;
			this.RebuildClockString();
		}

		// Token: 0x0600204D RID: 8269 RVA: 0x000F2F00 File Offset: 0x000F1300
		public override void Update()
		{
			base.Update();
			if (DateTime.Now.Hour != this.lastDate.Hour || DateTime.Now.Minute != this.lastDate.Minute)
			{
				base.PunchIn(0f);
				this.RebuildClockString();
			}
			this.lastDate = DateTime.Now;
		}

		// Token: 0x0600204E RID: 8270 RVA: 0x000F2F6C File Offset: 0x000F136C
		private void RebuildClockString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.Fill(DateTime.Now.Hour + string.Empty, 2));
			stringBuilder.Append(":");
			stringBuilder.Append(this.Fill(DateTime.Now.Minute + string.Empty, 2));
			this.clock = stringBuilder.ToString();
		}

		// Token: 0x0600204F RID: 8271 RVA: 0x000F2FEC File Offset: 0x000F13EC
		public override void Redraw(int offx, int offy)
		{
			if (this.hidden)
			{
				return;
			}
			SHGUI.current.DrawText(this.clock, this.x + offx - this.clock.Length, this.y + offy, this.color, this.fade, ' ', false, false);
		}

		// Token: 0x06002050 RID: 8272 RVA: 0x000F3041 File Offset: 0x000F1441
		private string Fill(string s, int len = 2)
		{
			while (s.Length < len)
			{
				s = "0" + s;
			}
			return s;
		}

		// Token: 0x040021B7 RID: 8631
		private DateTime lastDate;

		// Token: 0x040021B8 RID: 8632
		private string clock = string.Empty;
	}
}