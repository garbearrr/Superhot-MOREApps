using System;
using System.Collections.Generic;
using UnityEngine;

using MCD.GlitchLayer;

namespace MCD.MCDView
{
	// Token: 0x0200031F RID: 799
	[CreateAssetMenu(fileName = "GlitchLayerContainer", menuName = "SHRL/GlitchLayerContainer")]
	public class GlitchLayerContainer : ScriptableObject
	{
		// Token: 0x040010CD RID: 4301
		[HideInInspector]
		public char SkipSymbol = '!';

		// Token: 0x040010CE RID: 4302
		public List<MCDGlitchLayer> GlitchLayers = new List<MCDGlitchLayer>();
	}
}
