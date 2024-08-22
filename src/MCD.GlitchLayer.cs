using System;
using UnityEngine;

using MCD.CurveValue;

namespace MCD.GlitchLayer
{
	// Token: 0x0200031E RID: 798
	[CreateAssetMenu(fileName = "GlitchLayer", menuName = "SHRL/GlitchLayer")]
	public class MCDGlitchLayer : ScriptableObject
	{
		// Token: 0x040010BF RID: 4287
		public bool Active = true;

		// Token: 0x040010C0 RID: 4288
		public string GlitchSymbols = "1234567890ABCDEF";

		// Token: 0x040010C1 RID: 4289
		public string GlitchColors = "2345";

		// Token: 0x040010C2 RID: 4290
		public GlitchSize TopSize = new GlitchSize();

		// Token: 0x040010C3 RID: 4291
		public GlitchSize BottomSize = new GlitchSize();

		// Token: 0x040010C4 RID: 4292
		public AnimationCurve PositionCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x040010C5 RID: 4293
		public float MinActiveDuration = 1f;

		// Token: 0x040010C6 RID: 4294
		public float MaxActiveDuration = 2f;

		// Token: 0x040010C7 RID: 4295
		public float MinInactiveDuration = 1f;

		// Token: 0x040010C8 RID: 4296
		public float MaxInactiveDuration = 2f;

		// Token: 0x040010C9 RID: 4297
		public MCDCurveValue Density = new MCDCurveValue();

		// Token: 0x040010CA RID: 4298
		public MCDCurveValue Frequency = new MCDCurveValue();

		// Token: 0x040010CB RID: 4299
		public bool ConstantGlitch;

		// Token: 0x040010CC RID: 4300
		public float DensityIncreaseOverLifetime;
	}

	public class GlitchSize
	{
		// Token: 0x040010CF RID: 4303
		public float StartWidth;

		// Token: 0x040010D0 RID: 4304
		public float EndWidth = 1f;

		// Token: 0x040010D1 RID: 4305
		public float StartHeight;

		// Token: 0x040010D2 RID: 4306
		public float EndHeight = 1f;
	}
}
