extern alias SHSharp;

using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Topten.RichTextKit;
using SHSharp::BackwardCompatibility;
using System.Collections.Generic;

namespace SH.SHGUI.Extended
{
   public class SHGUItextEx : SHSharp::SHGUItext
    {
        public SHGUItextEx(string Text, int X, int Y, char Col, bool drawSpaces = false)
            : base(Text, X, Y, Col, drawSpaces)
        {

        }

        public SHGUItextEx(SHSharp::LocalizableText localizableText, int X, int Y, char Col, bool drawSpaces = false)
            : base(localizableText.Get(), X, Y, Col, drawSpaces)
        {
        }

        public SHGUItextEx SmartBreakTextForLineLength(int lenght)
        {
            this.longestLineAfterSmartBreak = this.CountMaxLineLenght(this.text, -1);
            if (this.longestLineAfterSmartBreak > lenght)
            {
                this.longestLineAfterSmartBreak = -1;
                SHGUItextEx.sLineBreaker.Reset(this.text);
                LineBreak lineBreak = new LineBreak(-1, -1, false);
                int num = 0;
                SHGUItextEx.sStringBuffer.Length = 0;
                LineBreak lineBreak2;
                while (SHGUItextEx.sLineBreaker.NextBreak(out lineBreak2))
                {
                    if (lineBreak2.Required || lineBreak2.PositionMeasure - num > lenght)
                    {
                        if (lineBreak2.Required)
                        {
                            if (lineBreak2.PositionMeasure - num > lenght)
                            {
                                if (lineBreak.PositionMeasure - num > 0)
                                {
                                    SHGUItextEx.sStringBuffer.Append(this.text, num, lineBreak.PositionMeasure - num);
                                }
                                SHGUItextEx.sStringBuffer.Append('\n');
                                num = lineBreak.PositionWrap;
                            }
                            lineBreak = lineBreak2;
                        }
                        int num2 = lineBreak.PositionMeasure - num;
                        if (num2 > 0)
                        {
                            SHGUItextEx.sStringBuffer.Append(this.text, num, num2);
                        }
                        SHGUItextEx.sStringBuffer.Append('\n');
                        num = lineBreak.PositionWrap;
                    }
                    lineBreak = lineBreak2;
                }
                if (num != lineBreak.PositionWrap)
                {
                    SHGUItextEx.sStringBuffer.Append(this.text, num, lineBreak.PositionWrap - num);
                }
                this.text = SHGUItextEx.sStringBuffer.ToString();
                this.longestLineAfterSmartBreak = this.CountMaxLineLenght(this.text, -1);
            }
            return this;
        }

        protected int CountMaxLineLenght(string text, int endCharacterIndex = -1)
        {
            int num = 0;
            int num2 = 0;
            int num3 = (endCharacterIndex != -1) ? Mathf.Min(text.Length, endCharacterIndex) : text.Length;
            for (int i = 0; i < num3; i++)
            {
                if (text[i] == '\n')
                {
                    if (num > num2)
                    {
                        num2 = num;
                    }
                    num = 0;
                }
                else
                {
                    num++;
                }
            }
            if (num > num2)
            {
                num2 = num;
            }
            return num2;
        }

        private static LineBreaker sLineBreaker = new LineBreaker();

        // Token: 0x04002304 RID: 8964
        private static StringBuilder sStringBuffer = new StringBuilder(128);
    }


    public class ExAPPguruchat : SHSharp::SHGUIview
    {
        // Token: 0x06001188 RID: 4488 RVA: 0x00068098 File Offset: 0x00066298
        public ExAPPguruchat()
        {
            base.Init();
            this.allowCursorDraw = false;
            this.chats = new List<SHSharp::SHGUIguruchatwindow>();
            this.frame = base.AddSubView(new SHSharp::SHGUIframe(0, 0, SHSharp::SHGUI.current.resolutionX - 1, SHSharp::SHGUI.current.resolutionY - 1, 'z', null));
            this.appname = base.AddSubView(new SHSharp::SHGUItext("guruCHAT", 3, 0, 'w', false));
            this.instructions = (base.AddSubView(new SHSharp::SHGUItext("", SHSharp::SHGUI.current.resolutionX - 22, SHSharp::SHGUI.current.resolutionY - 1, 'w', false)) as SHSharp::SHGUItext);
            this.clock = base.AddSubView(new SHSharp::SHGUIclock(77, 0, 'w'));
            this.messageQueue = new List<gurumessage>();
        }

        // Token: 0x06001189 RID: 4489 RVA: 0x00068172 File Offset: 0x00066372
        public void AddMyMessage(string sender, string message)
        {
            this.messageQueue.Add(new gurumessage(sender, message, true, true, false, false));
        }

        // Token: 0x0600118A RID: 4490 RVA: 0x0006818A File Offset: 0x0006638A
        public void AddOtherMessage(string sender, string message)
        {
            this.messageQueue.Add(new gurumessage(sender, message, false, false, false, false));
        }

        // Token: 0x0600118B RID: 4491 RVA: 0x000681A2 File Offset: 0x000663A2
        public void AddMySystemMessage(string sender, string message)
        {
            this.messageQueue.Add(new gurumessage(sender, message, true, false, false, true));
        }

        // Token: 0x0600118C RID: 4492 RVA: 0x000681BA File Offset: 0x000663BA
        public void AddOtherSystemMessage(string sender, string message)
        {
            this.messageQueue.Add(new gurumessage(sender, message, false, false, false, false));
        }

        // Token: 0x0600118D RID: 4493 RVA: 0x000681D2 File Offset: 0x000663D2
        public void AddMyQuit()
        {
            this.messageQueue.Add(new gurumessage("User56755548", "^CrUSER LEFT CHAT^W8^W8", true, false, true, true));
        }

        // Token: 0x0600118E RID: 4494 RVA: 0x000681F4 File Offset: 0x000663F4
        public override void Update()
        {
            base.Update();
            if (this.fadingOut)
            {
                return;
            }
            if (this.lastChat == null || this.lastChat.finished)
            {
                if (this.messageQueue.Count <= 0)
                {
                    this.Kill();
                    SHSharp::SHGUI.current.PopView();
                    return;
                }
                gurumessage gurumessage = this.messageQueue[0];
                this.messageQueue.RemoveAt(0);
                this.AddChatMessage(gurumessage.sender, gurumessage.message, gurumessage.leftright, gurumessage.interactive, gurumessage.isPoor);
                if (gurumessage.isQuit)
                {
                    this.quiting = true;
                }
            }
            if (this.quiting)
            {
                this.instructions.Kill();
                this.appname.Kill();
                this.clock.Kill();
                SHSharp::SHGUI.current.PopView();
                return;
            }
            if (this.lastChat == null)
            {
                SHSharp::SHGUI.current.PopView();
                return;
            }
            if (base.FixedUpdater(0.01f))
            {
                int num = 14;
                this.lastChat.x = num;
                if (this.lastChat.Align == SHSharp::SHAlign.Right)
                {
                    this.lastChat.x = SHSharp::SHGUI.current.resolutionX - num - this.lastChat.width - 1;
                }
            }
            if (this.lines + this.lastChat.height - this.totalOff > SHSharp::SHGUI.current.resolutionY - 1)
            {
                for (int i = 0; i < this.chats.Count; i++)
                {
                    this.chats[i].y--;
                    if (this.chats[i].y < -this.chats[i].height)
                    {
                        this.chats[i].remove = true;
                    }
                }
                this.totalOff++;
            }
        }

        // Token: 0x0600118F RID: 4495 RVA: 0x000683A4 File Offset: 0x000665A4
        public void SetInstructions(string text)
        {
            this.instructions.text = text;
            this.instructions.x = SHSharp::SHGUI.current.resolutionX - 5;
            this.instructions.GoFromRight();
        }

        // Token: 0x06001190 RID: 4496 RVA: 0x000683D8 File Offset: 0x000665D8
        public void AddChatMessage(string sender, string message, bool leftright, bool interactive, bool poor)
        {
            sender = "";
            if (this.lastChat != null)
            {
                if (this.lastChat.height < 3)
                {
                    this.lines += 3;
                }
                else
                {
                    this.lines += this.lastChat.height;
                }
            }
            this.lastChat = new SHSharp::SHGUIguruchatwindow(null);
            this.lastChat.SetAlign(leftright ? SHSharp::SHAlign.Left : SHSharp::SHAlign.Right);
            if (interactive)
            {
                this.lastChat.SetInteractive();
            }
            if (poor)
            {
                this.lastChat.poorMode = true;
            }
            this.lastChat.SetWidth(35);
            this.lastChat.SetContent(message);
            this.lastChat.SetLabel(sender);
            this.chats.Add(base.AddSubViewBottom(this.lastChat) as SHSharp::SHGUIguruchatwindow);
            this.lastChat.y = this.lines - this.totalOff;
            int num = 14;
            this.lastChat.x = num;
            if (!leftright)
            {
                this.lastChat.x = 80 - num - this.lastChat.width - 1;
            }
            if (this.skippable)
            {
                if (leftright)
                {
                    this.SetInstructions("press-ESC-to-leave-chat");
                }
                else
                {
                    this.SetInstructions("press-ESC-to-leave-chat");
                }
            }
            else
            {
                this.SetInstructions("");
            }
            if (leftright && interactive && !poor)
            {
                this.lastChat.PunchIn(0.7f);
            }
            if (!poor)
            {
                SHSharp::SHGUI.current.PlaySound(SHSharp::SHGUIsound.confirm);
                return;
            }
            SHSharp::SHGUI.current.PlaySound(SHSharp::SHGUIsound.wrong);
        }

        // Token: 0x06001191 RID: 4497 RVA: 0x0006855A File Offset: 0x0006675A
        public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
        {
            if (this.fadingOut)
            {
                return;
            }
            if (key == SHSharp::SHGUIinput.esc)
            {
                this.QuitChat();
            }
            base.ReactToInputKeyboard(key);
        }

        // Token: 0x06001192 RID: 4498 RVA: 0x00068578 File Offset: 0x00066778
        private void QuitChat()
        {
            if (this.lastChat != null && !this.quiting && this.skippable)
            {
                this.lastChat.Stop();
                this.quiting = true;
                this.lastChat.PunchIn(0.7f);
                this.messageQueue = new List<gurumessage>();
                this.AddMyQuit();
            }
        }

        // Token: 0x06001193 RID: 4499 RVA: 0x000685D0 File Offset: 0x000667D0
        public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
        {
            if (this.fadingOut)
            {
                return;
            }
            bool flag = this.quiting;
            base.ReactToInputMouse(x, y, clicked, scroll);
        }

        // Token: 0x04000EE0 RID: 3808
        private SHSharp::SHGUIguruchatwindow lastChat;

        // Token: 0x04000EE1 RID: 3809
        private SHSharp::SHGUItext instructions;

        // Token: 0x04000EE2 RID: 3810
        private SHSharp::SHGUIview appname;

        // Token: 0x04000EE3 RID: 3811
        private SHSharp::SHGUIview clock;

        // Token: 0x04000EE4 RID: 3812
        private SHSharp::SHGUIview frame;

        // Token: 0x04000EE5 RID: 3813
        private List<SHSharp::SHGUIguruchatwindow> chats;

        // Token: 0x04000EE6 RID: 3814
        private int totalOff;

        // Token: 0x04000EE7 RID: 3815
        private List<gurumessage> messageQueue;

        // Token: 0x04000EE8 RID: 3816
        private int lines = 1;

        // Token: 0x04000EE9 RID: 3817
        private bool quiting;

        // Token: 0x04000EEA RID: 3818
        public bool skippable = true;

        internal class gurumessage
        {
            // Token: 0x06001187 RID: 4487 RVA: 0x00068062 File Offset: 0x00066262
            public gurumessage(string Sender, string Message, bool LeftRight, bool Interactive, bool IsQuit, bool IsPoor)
            {
                this.sender = Sender;
                this.message = Message;
                this.leftright = LeftRight;
                this.interactive = Interactive;
                this.isQuit = IsQuit;
                this.isPoor = IsPoor;
            }

            // Token: 0x04000EDA RID: 3802
            public string sender;

            // Token: 0x04000EDB RID: 3803
            public string message;

            // Token: 0x04000EDC RID: 3804
            public bool leftright;

            // Token: 0x04000EDD RID: 3805
            public bool interactive;

            // Token: 0x04000EDE RID: 3806
            public bool isQuit;

            // Token: 0x04000EDF RID: 3807
            public bool isPoor;
        }
    }

}