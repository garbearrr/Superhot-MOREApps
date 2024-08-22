using System;
using Assets.Scripts;

using MelonLoader;

using MCD.ShGUIFrame;
using MCD.ShGUIRect;
using MCD.ShGUIText;
using MCD.ShGUIView;
using MCD.StoryApps.Base;


namespace MCD.ShGUIappbase
{
	// Token: 0x020005A6 RID: 1446
	public class MCDSHGUIappbase : MCDSHGUIview
	{
		// Token: 0x0600203A RID: 8250 RVA: 0x0007BCB0 File Offset: 0x0007A0B0
		public MCDSHGUIappbase(string title, bool showFrame = true)
		{
			base.Init();
			base.AddSubView(new MCDSHGUIrect(0, 0, SHGUI.current.resolutionX - 10, SHGUI.current.resolutionY - 1, '0', ' ', 2));
			if (showFrame)
			{
				if (base.GetType() == typeof(SecretChatView))
				{
					this.APPFRAME = (base.AddSubView(new MCDSHGUIframe(12, 0, SHGUI.current.resolutionX - 13, SHGUI.current.resolutionY - 1, 'z', null, string.Empty, 'w')) as MCDSHGUIframe);
				}
				else
				{
					this.APPFRAME = (base.AddSubView(new MCDSHGUIframe(0, 0, SHGUI.current.resolutionX - 1, SHGUI.current.resolutionY - 1, 'z', null, string.Empty, 'w')) as MCDSHGUIframe);
				}
			}
			this.APPLABEL = (base.AddSubView(new MCDSHGUItext(title, 3, 0, 'w', false)) as MCDSHGUItext);
			string key = "esc"; //SHGame.Instance.ControlsSettings.GetKey(SHControlsSettings.SHGUIKeys.Exit);
			string text = string.Format("Press esc to Exit App".T(), key);
			if (base.GetType() == typeof(SecretChatView))
			{
				this.APPINSTRUCTION = (base.AddSubView(new MCDSHGUItext(text, SHGUI.current.resolutionX - 14, SHGUI.current.resolutionY - 1, 'z', false).GoFromRight()) as MCDSHGUItext);
			}
			else
			{
				this.APPINSTRUCTION = (base.AddSubView(new MCDSHGUItext(text, SHGUI.current.resolutionX - 5, SHGUI.current.resolutionY - 1, 'z', false).GoFromRight()) as MCDSHGUItext);
			}
			this.allowCursorDraw = false;
			this.overrideFadeInSpeed = 0.35f;
			this.overrideFadeOutSpeed = 0.5f;
		}

		// Token: 0x0600203B RID: 8251 RVA: 0x0007BE73 File Offset: 0x0007A273
		public override void ReactToInputKeyboard(SHGUIinput key)
		{
			if (this.fadingOut)
			{
				return;
			}
			if (key == SHGUIinput.esc)
			{
				SHGUI.current.PopView();
			}
			if (key == SHGUIinput.enter)
			{
				SHGUI.current.PopView();
			}
		}

		// Token: 0x0600203C RID: 8252 RVA: 0x0007BEA3 File Offset: 0x0007A2A3
		public override void ReactToInputMouse(int x, int y, bool clicked, SHGUIinput scroll)
		{
			if (this.fadingOut)
			{
				return;
			}
			if (clicked && this.shouldCloseOnClick)
			{
				SHGUI.current.PopView();
			}
		}

		// Token: 0x0600203D RID: 8253 RVA: 0x0007BECC File Offset: 0x0007A2CC
		protected void MoveFrameToTop()
		{
			this.children.Remove(this.APPFRAME);
			this.children.Add(this.APPFRAME);
			this.children.Remove(this.APPLABEL);
			this.children.Add(this.APPLABEL);
		}

		// Token: 0x0600203E RID: 8254 RVA: 0x0007BF1F File Offset: 0x0007A31F
		protected void RedrawAppGui(int offx, int offy)
		{
			this.APPFRAME.Redraw(offx, offy);
			this.APPLABEL.Redraw(offx, offy);
			this.APPINSTRUCTION.Redraw(offx, offy);
		}

		// Token: 0x0400219C RID: 8604
		public MCDSHGUItext APPLABEL;

		// Token: 0x0400219D RID: 8605
		public MCDSHGUItext APPINSTRUCTION;

		// Token: 0x0400219E RID: 8606
		public MCDSHGUIframe APPFRAME;
	}
}
