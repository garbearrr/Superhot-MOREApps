using System;

using MCD.ShGUIguruchatwindow;
using MCD.ShGUIPrompter;
using MCD.ShGUIView;

namespace MCD.ShGUIchatpositioner
{
	// Token: 0x020005A9 RID: 1449
	public class MCDSHGUIchatpositioner : MCDSHGUIview
	{
		// Token: 0x06002042 RID: 8258 RVA: 0x000F295A File Offset: 0x000F0D5A
		public MCDSHGUIchatpositioner(SHGUIchatposition position = SHGUIchatposition.Center)
		{
			this.position = position;
		}

		// Token: 0x06002043 RID: 8259 RVA: 0x000F2970 File Offset: 0x000F0D70
		public MCDSHGUIchatpositioner AddChat(MCDSHGUIguruchatwindow chat)
		{
			this.chat = chat;
			base.AddSubView(chat);
			return this;
		}

		// Token: 0x06002044 RID: 8260 RVA: 0x000F2982 File Offset: 0x000F0D82
		public MCDSHGUIchatpositioner AddPrompter(MCDSHGUIprompter prompter)
		{
			this.prompter = prompter;
			base.AddSubView(prompter);
			return this;
		}

		// Token: 0x06002045 RID: 8261 RVA: 0x000F2994 File Offset: 0x000F0D94
		public override void Update()
		{
			base.Update();
			if (this.chat != null)
			{
				this.chat.y = 0;
				if (this.position == SHGUIchatposition.Left)
				{
					this.chat.x = 0;
				}
				else if (this.position == SHGUIchatposition.Center)
				{
					this.chat.x = -(this.chat.width / 2);
					this.chat.y = -(this.chat.height / 2);
				}
				else if (this.position == SHGUIchatposition.CenterOnlyX)
				{
					this.chat.x = -(this.chat.width / 2);
				}
				else if (this.position == SHGUIchatposition.Right)
				{
					this.chat.x = -this.chat.width;
				}
				if (this.KillOnCompleteChat && this.chat.textElement.IsFinished())
				{
					this.Kill();
				}
			}
			if (this.prompter != null)
			{
				this.prompter.y = 0;
				if (this.position == SHGUIchatposition.Left)
				{
					this.prompter.x = 0;
				}
				else if (this.position == SHGUIchatposition.Center)
				{
					this.prompter.x = -(this.prompter.GetLongestLineLength() / 2);
					this.prompter.y = -1;
				}
				else if (this.position == SHGUIchatposition.CenterOnlyX)
				{
					this.prompter.x = -(this.prompter.GetLongestLineLength() / 2);
				}
				else if (this.position == SHGUIchatposition.Right)
				{
					this.prompter.x = -this.prompter.GetLongestLineLength();
				}
				if (this.KillOnCompleteChat && this.prompter.IsFinished())
				{
					this.Kill();
				}
			}
		}

		// Token: 0x040021A8 RID: 8616
		private SHGUIchatposition position = SHGUIchatposition.Center;

		// Token: 0x040021A9 RID: 8617
		public MCDSHGUIguruchatwindow chat;

		// Token: 0x040021AA RID: 8618
		public MCDSHGUIprompter prompter;

		// Token: 0x040021AB RID: 8619
		public bool KillOnCompleteChat;
	}
}