using System;
using UnityEngine;

using MCD.ShGUIView;

namespace MCD.ShGUIblinkview
{
	// Token: 0x020005A7 RID: 1447
	public class MCDSHGUIblinkview : MCDSHGUIview
	{
		// Token: 0x0600203F RID: 8255 RVA: 0x000F28C6 File Offset: 0x000F0CC6
		public MCDSHGUIblinkview(float blinkTime)
		{
			this.timer = blinkTime;
			this.blinkTime = blinkTime;
		}

		// Token: 0x06002040 RID: 8256 RVA: 0x000F28E7 File Offset: 0x000F0CE7
		public MCDSHGUIblinkview SetFlipped()
		{
			this.hidden = !this.hidden;
			return this;
		}

		// Token: 0x06002041 RID: 8257 RVA: 0x000F28FC File Offset: 0x000F0CFC
		public override void Update()
		{
			base.Update();
			this.timer -= Time.unscaledDeltaTime;
			if (this.timer < 0f)
			{
				this.BlinkCounter++;
				this.timer = this.blinkTime;
				this.hidden = !this.hidden;
			}
		}

		// Token: 0x0400219F RID: 8607
		private float timer;

		// Token: 0x040021A0 RID: 8608
		public float blinkTime = 0.2f;

		// Token: 0x040021A1 RID: 8609
		private bool clicker;

		// Token: 0x040021A2 RID: 8610
		public int BlinkCounter;
	}
}