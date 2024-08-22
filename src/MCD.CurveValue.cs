using System;
using UnityEngine;

namespace MCD.CurveValue
{
	// Token: 0x0200031B RID: 795
	[Serializable]
	public class MCDCurveValue
	{
		// Token: 0x040010A9 RID: 4265
		public AnimationCurve Curve = AnimationCurve.Linear(0f, 1f, 1f, 1f);

		// Token: 0x040010AA RID: 4266
		public float TopValue = 1f;

		// Token: 0x040010AB RID: 4267
		public float BottomValue;
	}
}