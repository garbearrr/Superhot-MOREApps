using System;
using System.Collections.Generic;
using UnityEngine;

using MCD.ShGUIView;

namespace MCD.ShGUISprite
{

	// Token: 0x0200037B RID: 891
	public class MCDSHGUIsprite : MCDSHGUIview
	{
		// Token: 0x060016CD RID: 5837 RVA: 0x0009DB75 File Offset: 0x0009BD75
		public MCDSHGUIsprite()
		{
			base.Init();
			this.frames = new List<string>();
			this.frameSounds = new List<string>();
		}

		// Token: 0x060016CE RID: 5838 RVA: 0x0009DB99 File Offset: 0x0009BD99
		public MCDSHGUIsprite AddFrame(string frame, string sound = null)
		{
			this.frames.Add(frame);
			this.frameSounds.Add(sound);
			return this;
		}

		// Token: 0x060016CF RID: 5839 RVA: 0x0009DBB4 File Offset: 0x0009BDB4
		public MCDSHGUIsprite AddFramesFromFile(string filename, int rowsPerFrame)
		{
			string[] array = SHGUI.current.GetASCIIartByName(filename).Split(new char[]
			{
			'\n'
			});
			int num = 0;
			string text = "";
			for (int i = 0; i < array.Length; i++)
			{
				text = text + array[i] + "\n";
				num++;
				if (num > rowsPerFrame - 1)
				{
					num = 0;
					this.AddFrame(text, null);
					text = "";
				}
			}
			return this;
		}

		// Token: 0x060016D0 RID: 5840 RVA: 0x0009DC20 File Offset: 0x0009BE20
		public MCDSHGUIsprite AddSpecyficFrameFromFile(string filename, int rowsPerFrame, int addFrame, string sound = null)
		{
			string[] array = SHGUI.current.GetASCIIartByName(filename).Split(new char[]
			{
			'\n'
			});
			int num = 0;
			string text = "";
			int num2 = 0;
			for (int i = 0; i < array.Length; i++)
			{
				text = text + array[i] + "\n";
				num++;
				if (num > rowsPerFrame - 1)
				{
					num = 0;
					if (addFrame == num2)
					{
						this.AddFrame(text, sound);
					}
					text = "";
					num2++;
				}
			}
			return this;
		}

		// Token: 0x060016D1 RID: 5841 RVA: 0x0009DC9C File Offset: 0x0009BE9C
		public override void Update()
		{
			base.Update();
			if (!this.playedSoundForCurrentFrame)
			{
				this.PlaySoundForCurrentFrame(this.currentFrame);
				this.playedSoundForCurrentFrame = true;
			}
			if (this.fade < 0.999f)
			{
				return;
			}
			this.currentAnimationTimer += Time.unscaledDeltaTime;
			if (this.currentAnimationTimer > this.animationSpeed && this.animationSpeed > 0f)
			{
				this.currentAnimationTimer -= this.animationSpeed;
				this.currentAnimationTimer = 0f;
				this.playedSoundForCurrentFrame = false;
				this.currentFrame++;
				if (this.currentFrame >= this.frames.Count)
				{
					if (this.loops)
					{
						this.currentFrame = 0;
						return;
					}
					this.currentFrame = this.frames.Count - 1;
				}
			}
		}

		// Token: 0x060016D2 RID: 5842 RVA: 0x0009DD70 File Offset: 0x0009BF70
		public void PlaySoundForCurrentFrame(int frame)
		{
			if (frame < 0 || frame >= this.frameSounds.Count)
			{
				return;
			}
			if (this.frameSounds[frame] != null)
			{
				AudioManager.Instance.PlayClip(AudioManager.AudioClips[this.frameSounds[frame]], null, 1f, AudioManager.Instance.mixerInterfaceSFX, 128, true, false, true, false);
			}
		}

		// Token: 0x060016D3 RID: 5843 RVA: 0x0009DDE0 File Offset: 0x0009BFE0
		public void RedrawBlack(int offx, int offy)
		{
			SHGUI.current.DrawBlack(this.frames[this.currentFrame], this.x + offx, this.y + offy);
		}

		// Token: 0x060016D4 RID: 5844 RVA: 0x0009DE10 File Offset: 0x0009C010
		public override void Redraw(int offx, int offy)
		{
			base.Redraw(offx, offy);
			if (this.frames.Count > 0)
			{
				SHGUI.current.DrawTextSkipSpaces(this.frames[this.currentFrame], this.x + offx, this.y + offy, this.color, this.fade, ' ');
			}
		}

		// Token: 0x040014A7 RID: 5287
		public List<string> frames;

		// Token: 0x040014A8 RID: 5288
		public List<string> frameSounds;

		// Token: 0x040014A9 RID: 5289
		public int currentFrame;

		// Token: 0x040014AA RID: 5290
		public float animationSpeed;

		// Token: 0x040014AB RID: 5291
		public float currentAnimationTimer;

		// Token: 0x040014AC RID: 5292
		public bool killOnAnimationComplete;

		// Token: 0x040014AD RID: 5293
		public bool loops;

		// Token: 0x040014AE RID: 5294
		private bool playedSoundForCurrentFrame;
	}
}
