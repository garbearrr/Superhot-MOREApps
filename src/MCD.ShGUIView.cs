using System;
using System.Collections.Generic;
using UnityEngine;

namespace MCD.ShGUIView
{
	// Token: 0x020005C6 RID: 1478
	public class MCDSHGUIview
	{
		// Token: 0x0600214C RID: 8524 RVA: 0x00024410 File Offset: 0x00022810
		public MCDSHGUIview()
		{
			this.Init();
		}

		// Token: 0x0600214D RID: 8525 RVA: 0x0002445C File Offset: 0x0002285C
		protected void Init()
		{
			this.id = SHGUI.GetId();
			this.children = new List<MCDSHGUIview>();
			this.fadingIn = true;
		}

		// Token: 0x0600214E RID: 8526 RVA: 0x0002447B File Offset: 0x0002287B
		public virtual void OnPop()
		{
		}

		// Token: 0x0600214F RID: 8527 RVA: 0x00024480 File Offset: 0x00022880
		public virtual void OnEnter()
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].OnEnter();
			}
		}

		// Token: 0x06002150 RID: 8528 RVA: 0x000244BC File Offset: 0x000228BC
		public virtual void OnExit()
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].OnExit();
			}
		}

		// Token: 0x06002151 RID: 8529 RVA: 0x000244F8 File Offset: 0x000228F8
		public virtual void Update()
		{
			if (this.UpdateOnlyWhenVisible && this.hidden)
			{
				return;
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Update();
				if (this.children[i].fade < 0.6f && this.fadingIn)
				{
					break;
				}
				if (this.children[i].remove)
				{
					this.children.RemoveAt(i);
					i--;
				}
			}
			this.UpdateColorPunch();
		}

		// Token: 0x06002152 RID: 8530 RVA: 0x000245A0 File Offset: 0x000229A0
		public virtual void LateUpdate()
		{
			if (this.UpdateOnlyWhenVisible && this.hidden)
			{
				return;
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].LateUpdate();
			}
		}

		// Token: 0x06002153 RID: 8531 RVA: 0x000245F4 File Offset: 0x000229F4
		public virtual void FadeUpdate(float inSpeedMulti = 1f, float outSpeedMulti = 1f)
		{
			if (this.UpdateOnlyWhenVisible && this.hidden)
			{
				return;
			}
			if (this.DisregardParentsFadeSpeeds)
			{
				inSpeedMulti = 1f;
				outSpeedMulti = 1f;
			}
			if (!this.hack && this.forcedFadeSpeedRegardless != 0f)
			{
				this.hack = true;
				this.fade = 0.5f;
			}
			if (this.fadingIn)
			{
				if (this.forcedFadeSpeedRegardless == 0f)
				{
					this.fade += 0.2f * Time.unscaledDeltaTime * 25f * inSpeedMulti * this.overrideFadeInSpeed;
				}
				else
				{
					this.fade += this.forcedFadeSpeedRegardless;
				}
				if (this.fade > 1f)
				{
					this.fade = 1f;
					this.fadingIn = false;
				}
			}
			if (this.fadingOut)
			{
				if (this.forcedFadeSpeedRegardless == 0f)
				{
					this.fade -= 0.3f * Time.unscaledDeltaTime * 25f * outSpeedMulti * this.overrideFadeOutSpeed;
				}
				else
				{
					this.fade -= this.forcedFadeSpeedRegardless;
				}
				if (this.fade < 0f)
				{
					this.fade = 0f;
					this.fadingOut = false;
					if (!this.DontKillOnFadeOut)
					{
						this.remove = true;
					}
				}
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].FadeUpdate(inSpeedMulti * this.overrideFadeInSpeed, outSpeedMulti * this.overrideFadeOutSpeed);
			}
		}

		// Token: 0x06002154 RID: 8532 RVA: 0x000247A8 File Offset: 0x00022BA8
		public void ForceFadeRecursive(float fade)
		{
			this.fade = fade;
			for (int i = 0; i < this.children.Count; i++)
			{
				MCDSHGUIview shguiview = this.children[i];
				shguiview.ForceFadeRecursive(fade);
			}
		}

		// Token: 0x06002155 RID: 8533 RVA: 0x000247EC File Offset: 0x00022BEC
		public virtual void Redraw(int offx, int offy)
		{
			if (this.hidden)
			{
				return;
			}
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Redraw(offx + this.x, offy + this.y);
			}
		}

		// Token: 0x06002156 RID: 8534 RVA: 0x00024842 File Offset: 0x00022C42
		public MCDSHGUIview AddSubView(MCDSHGUIview view)
		{
			this.children.Add(view);
			view.parent = this;
			return view;
		}

		// Token: 0x06002157 RID: 8535 RVA: 0x00024858 File Offset: 0x00022C58
		public MCDSHGUIview AddSubViewBottom(MCDSHGUIview view)
		{
			this.children.Insert(0, view);
			view.parent = this;
			return view;
		}

		// Token: 0x06002158 RID: 8536 RVA: 0x00024870 File Offset: 0x00022C70
		public void RemoveView(MCDSHGUIview v)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				if (this.children[i].id == v.id)
				{
					this.children.RemoveAt(i);
					break;
				}
			}
		}

		// Token: 0x06002159 RID: 8537 RVA: 0x000248C6 File Offset: 0x00022CC6
		public void ColorPunchIn(char colorSet, char colorBack, float delay)
		{
			this.IsColorPunch = true;
			this.lastColor = new char?(colorBack);
			this.ColorPunchTimer = Time.unscaledTime + delay;
			this.color = colorSet;
		}

		// Token: 0x0600215A RID: 8538 RVA: 0x000248F0 File Offset: 0x00022CF0
		public virtual void Kill()
		{
			this.fadingIn = false;
			this.fadingOut = true;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Kill();
			}
		}

		// Token: 0x0600215B RID: 8539 RVA: 0x00024938 File Offset: 0x00022D38
		public virtual void KillWithoutChildren()
		{
			this.fadingIn = false;
			this.fadingOut = true;
		}

		// Token: 0x0600215C RID: 8540 RVA: 0x00024948 File Offset: 0x00022D48
		public void KillChildren()
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Kill();
			}
		}

		// Token: 0x0600215D RID: 8541 RVA: 0x00024982 File Offset: 0x00022D82
		public void KillInstant()
		{
			this.remove = true;
		}

		// Token: 0x0600215E RID: 8542 RVA: 0x0002498B File Offset: 0x00022D8B
		public void KillChildrenInstant()
		{
			this.children = new List<MCDSHGUIview>();
		}

		// Token: 0x0600215F RID: 8543 RVA: 0x00024998 File Offset: 0x00022D98
		public virtual void ReactToInputKeyboard(SHGUIinput key)
		{
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].ReactToInputKeyboard(key);
			}
		}

		// Token: 0x06002160 RID: 8544 RVA: 0x000249D3 File Offset: 0x00022DD3
		public virtual void ReactToInputMouse(int x, int y, bool clicked, SHGUIinput scroll)
		{
		}

		// Token: 0x06002161 RID: 8545 RVA: 0x000249D8 File Offset: 0x00022DD8
		public void PunchIn(float startFade = 0f)
		{
			if (this.fadingOut)
			{
				return;
			}
			this.fade = startFade;
			this.fadingIn = true;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].PunchIn(startFade);
			}
		}

		// Token: 0x06002162 RID: 8546 RVA: 0x00024A30 File Offset: 0x00022E30
		public void ForcedSoftFadeIn()
		{
			this.fade = -1f;
			this.fadingIn = true;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].ForcedSoftFadeIn();
			}
		}

		// Token: 0x06002163 RID: 8547 RVA: 0x00024A7C File Offset: 0x00022E7C
		public void SpeedUpFadeIn()
		{
			if (!this.fadingIn)
			{
				return;
			}
			this.fade += 0.4f;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].SpeedUpFadeIn();
			}
		}

		// Token: 0x06002164 RID: 8548 RVA: 0x00024AD4 File Offset: 0x00022ED4
		public bool FixedUpdater(float delay = 0.05f)
		{
			this.timer -= Time.unscaledDeltaTime;
			if (this.timer < 0f)
			{
				this.timer = delay;
				return true;
			}
			return false;
		}

		// Token: 0x06002165 RID: 8549 RVA: 0x00024B02 File Offset: 0x00022F02
		public virtual MCDSHGUIview SetColor(char c)
		{
			this.color = c;
			return this;
		}

		// Token: 0x06002166 RID: 8550 RVA: 0x00024B0C File Offset: 0x00022F0C
		public MCDSHGUIview SetColorRecursive(char c)
		{
			this.color = c;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].SetColor(this.color);
			}
			return this;
		}

		// Token: 0x06002167 RID: 8551 RVA: 0x00024B58 File Offset: 0x00022F58
		public MCDSHGUIview SetColorRecursiveWithChildren(char c)
		{
			this.color = c;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].SetColorRecursiveWithChildren(this.color);
			}
			return this;
		}

		// Token: 0x06002168 RID: 8552 RVA: 0x00024BA1 File Offset: 0x00022FA1
		public MCDSHGUIview SetCursorDraw(bool v)
		{
			this.allowCursorDraw = v;
			return this;
		}

		// Token: 0x06002169 RID: 8553 RVA: 0x00024BAC File Offset: 0x00022FAC
		public virtual void Darken()
		{
			char? c = this.lastColor;
			if (c != null)
			{
				char? c2 = this.lastColor;
				if (((c2 == null) ? null : new int?((int)c2.Value)) != (int)this.color)
				{
					return;
				}
			}
			this.lastColor = new char?(this.color);
			this.color = '3';
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Darken();
			}
		}

		// Token: 0x0600216A RID: 8554 RVA: 0x00024C64 File Offset: 0x00023064
		public virtual void Brighten()
		{
			char? c = this.lastColor;
			if (c == null)
			{
				return;
			}
			char? c2 = this.lastColor;
			this.color = c2.Value;
			this.lastColor = null;
			for (int i = 0; i < this.children.Count; i++)
			{
				this.children[i].Brighten();
			}
		}

		// Token: 0x0600216B RID: 8555 RVA: 0x00024CD8 File Offset: 0x000230D8
		private void UpdateColorPunch()
		{
			if (!this.IsColorPunch)
			{
				return;
			}
			if (Time.unscaledTime > this.ColorPunchTimer)
			{
				char? c = this.lastColor;
				this.SetColor(c.Value);
				this.IsColorPunch = false;
			}
		}

		// Token: 0x04002305 RID: 8965
		public int x;

		// Token: 0x04002306 RID: 8966
		public int y;

		// Token: 0x04002307 RID: 8967
		public char color = 'w';

		// Token: 0x04002308 RID: 8968
		public char? lastColor;

		// Token: 0x04002309 RID: 8969
		public List<MCDSHGUIview> children;

		// Token: 0x0400230A RID: 8970
		public MCDSHGUIview parent;

		// Token: 0x0400230B RID: 8971
		public float fade;

		// Token: 0x0400230C RID: 8972
		public bool fadingIn;

		// Token: 0x0400230D RID: 8973
		public bool fadingOut;

		// Token: 0x0400230E RID: 8974
		public bool hidden;

		// Token: 0x0400230F RID: 8975
		public bool interactable = true;

		// Token: 0x04002310 RID: 8976
		public bool dontDrawViewsBelow = true;

		// Token: 0x04002311 RID: 8977
		public bool allowCursorDraw;

		// Token: 0x04002312 RID: 8978
		public bool remove;

		// Token: 0x04002313 RID: 8979
		public int id;

		// Token: 0x04002314 RID: 8980
		private float timer;

		// Token: 0x04002315 RID: 8981
		public float overrideFadeInSpeed = 1f;

		// Token: 0x04002316 RID: 8982
		public float overrideFadeOutSpeed = 1f;

		// Token: 0x04002317 RID: 8983
		public float forcedFadeSpeedRegardless;

		// Token: 0x04002318 RID: 8984
		public bool DisregardParentsFadeSpeeds;

		// Token: 0x04002319 RID: 8985
		public bool DontKillOnFadeOut;

		// Token: 0x0400231A RID: 8986
		public bool shouldCloseOnClick = true;

		// Token: 0x0400231B RID: 8987
		private bool IsColorPunch;

		// Token: 0x0400231C RID: 8988
		private char ColorPunch;

		// Token: 0x0400231D RID: 8989
		private float ColorPunchTimer;

		// Token: 0x0400231E RID: 8990
		public string RedVoiceViewKey;

		// Token: 0x0400231F RID: 8991
		public bool UpdateOnlyWhenVisible;

		// Token: 0x04002320 RID: 8992
		private bool hack;
	}
}
