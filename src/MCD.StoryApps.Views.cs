extern alias SHSharp;

using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

using MCD.StoryApps.Enums;
using SH.SHGUI.Extended;


namespace MCD.StoryApps.Views
{
    public abstract class AStoryAppsView : SHSharp::SHGUIview
    {
        // Token: 0x060027AE RID: 10158 RVA: 0x001117D8 File Offset: 0x0010FBD8
        protected AStoryAppsView(int x, int y, int width, int height) : base()
        {
            this.Active = true;
            this.PositionX = x;
            this.PositionY = y;
            this.Width = width;
            this.Height = height;
        }

        // Token: 0x1700021A RID: 538
        // (get) Token: 0x060027AF RID: 10159 RVA: 0x0011183A File Offset: 0x0010FC3A
        // (set) Token: 0x060027B0 RID: 10160 RVA: 0x00111842 File Offset: 0x0010FC42
        public int PositionX
        {
            get
            {
                return this.positionX;
            }
            set
            {
                this.LocalPositionDelta.x = (float)(value - this.positionX);
                this.LocalPositionDelta.y = 0f;
                this.positionX = value;
                this.UpdateLocalPosition();
            }
        }

        // Token: 0x1700021B RID: 539
        // (get) Token: 0x060027B1 RID: 10161 RVA: 0x00111875 File Offset: 0x0010FC75
        // (set) Token: 0x060027B2 RID: 10162 RVA: 0x0011187D File Offset: 0x0010FC7D
        public int PositionY
        {
            get
            {
                return this.positionY;
            }
            set
            {
                this.LocalPositionDelta.y = (float)(value - this.positionY);
                this.LocalPositionDelta.x = 0f;
                this.positionY = value;
                this.UpdateLocalPosition();
            }
        }

        // Token: 0x1700021C RID: 540
        // (get) Token: 0x060027B3 RID: 10163 RVA: 0x001118B0 File Offset: 0x0010FCB0
        // (set) Token: 0x060027B4 RID: 10164 RVA: 0x001118B8 File Offset: 0x0010FCB8
        public bool Active
        {
            get
            {
                return this.active;
            }
            set
            {
                this.active = value;
                for (int i = 0; i < this.Elements.Count; i++)
                {
                    SHSharp::SHGUIview shguiview = this.Elements[i];
                    shguiview.hidden = !this.active;
                }
            }
        }

        // Token: 0x1700021D RID: 541
        // (get) Token: 0x060027B5 RID: 10165 RVA: 0x00111904 File Offset: 0x0010FD04
        // (set) Token: 0x060027B6 RID: 10166 RVA: 0x0011190C File Offset: 0x0010FD0C
        public int Width { get; set; }

        // Token: 0x1700021E RID: 542
        // (get) Token: 0x060027B7 RID: 10167 RVA: 0x00111915 File Offset: 0x0010FD15
        // (set) Token: 0x060027B8 RID: 10168 RVA: 0x0011191D File Offset: 0x0010FD1D
        public int Height { get; set; }

        // Token: 0x060027B9 RID: 10169 RVA: 0x00111928 File Offset: 0x0010FD28
        public void UpdateLocalPosition()
        {
            for (int i = 0; i < this.Elements.Count; i++)
            {
                SHSharp::SHGUIview shguiview = this.Elements[i];
                shguiview.x += (int)this.LocalPositionDelta.x;
                shguiview.y += (int)this.LocalPositionDelta.y;
            }
        }

        // Token: 0x060027BA RID: 10170 RVA: 0x00111990 File Offset: 0x0010FD90
        public void ResetElementsPositions()
        {
            for (int i = 0; i < this.Elements.Count; i++)
            {
                SHSharp::SHGUIview shguiview = this.Elements.ElementAt(i);
                shguiview.x = (int)this.ElementsPositions.ElementAt(i).x;
                shguiview.x = (int)this.ElementsPositions.ElementAt(i).y;
            }
        }

        // Token: 0x060027BB RID: 10171 RVA: 0x001119FC File Offset: 0x0010FDFC
        public void SetElementsColor(char color)
        {
            for (int i = 0; i < this.Elements.Count; i++)
            {
                SHSharp::SHGUIview shguiview = this.Elements[i];
                shguiview.SetColorRecursive(color);
            }
        }

        // Token: 0x060027BC RID: 10172 RVA: 0x00111A3A File Offset: 0x0010FE3A
        public new virtual SHSharp::SHGUIview AddSubView(SHSharp::SHGUIview view)
        {
            this.Elements.Add(view);
            this.ElementsPositions.Add(new Vector2((float)view.x, (float)view.y));
            return base.AddSubView(view);
        }

        // Token: 0x060027BD RID: 10173 RVA: 0x00111A6D File Offset: 0x0010FE6D
        public new virtual void RemoveView(SHSharp::SHGUIview view)
        {
            this.Elements.Remove(view);
            this.ElementsPositions.Remove(new Vector2((float)view.x, (float)view.y));
            base.RemoveView(view);
        }

        // Token: 0x04002A34 RID: 10804
        public List<SHSharp::SHGUIview> Elements = new List<SHSharp::SHGUIview>();

        // Token: 0x04002A35 RID: 10805
        protected List<Vector2> ElementsPositions = new List<Vector2>();

        // Token: 0x04002A36 RID: 10806
        protected Vector2 LocalPositionDelta = new Vector2(0f, 0f);

        // Token: 0x04002A37 RID: 10807
        private int positionX;

        // Token: 0x04002A38 RID: 10808
        private int positionY;

        // Token: 0x04002A39 RID: 10809
        private bool active;
    }

    public static class Extensions
    {
        // Token: 0x060024BB RID: 9403 RVA: 0x00112654 File Offset: 0x00110A54
        public static bool SetTimerAction(this object o, Action timerAction, ref float passedTime, float targetTime = 1f)
        {
            if (passedTime >= targetTime)
            {
                timerAction();
                passedTime = 0f;
                return true;
            }
            passedTime += Time.unscaledDeltaTime;
            return false;
        }

        // Token: 0x060024BC RID: 9404 RVA: 0x00112678 File Offset: 0x00110A78
        public static Action BlinkView(this SHSharp::SHGUIview view, char startColor = 'w', char blinkColor = 'r', bool playSound = true)
        {
            return delegate ()
            {
                //view.SetColorRecursiveWithChildren((view.color != blinkColor) ? blinkColor : startColor);
                if (view.color == blinkColor && playSound)
                {
                    SHSharp::SHGUI.current.PlaySound(SHSharp::SHGUIsound.restrictedpopup);
                }
            };
        }

        // Token: 0x060024BD RID: 9405 RVA: 0x001126B4 File Offset: 0x00110AB4
        public static Action KillViewWithSound(this SHSharp::SHGUIview view, SHSharp::SHGUIsound soundToPlay = SHSharp::SHGUIsound.pong)
        {
            return delegate ()
            {
                SHSharp::SHGUI.current.PlaySound(soundToPlay);
                view.Kill();
            };
        }

        // Token: 0x060024BE RID: 9406 RVA: 0x001126E4 File Offset: 0x00110AE4
        public static bool MoveToPosition(this AStoryAppsView view, int targetX, int targetY, ref float moveTimer, float speed = 1f)
        {
            if (view.PositionX == targetX && view.PositionY == targetY)
            {
                return true;
            }
            view.SetTimerAction(delegate
            {
                if (view.PositionX < targetX)
                {
                    view.PositionX++;
                }
                else if (view.PositionX > targetX)
                {
                    view.PositionX--;
                }
                if (view.PositionY < targetY)
                {
                    view.PositionY++;
                }
                else if (view.PositionY > targetY)
                {
                    view.PositionY--;
                }
            }, ref moveTimer, 1f / speed);
            return false;
        }

        // Token: 0x060024BF RID: 9407 RVA: 0x0011275C File Offset: 0x00110B5C
        public static bool WaitForSeconds(this object o, ref float waitTimer, float waitTime)
        {
            if (waitTimer >= waitTime)
            {
                return true;
            }
            waitTimer += Time.unscaledDeltaTime;
            return false;
        }

        // Token: 0x060024C0 RID: 9408 RVA: 0x00112774 File Offset: 0x00110B74
        /*public static ChatSequence FindChatSequence(this ChatMessage message)
        {
            List<ChatSequence> chatSequences = StoryAppsManager.Instance.ChatSequences;
            for (int i = 0; i < chatSequences.Count; i++)
            {
                ChatSequence chatSequence = chatSequences[i];
                if (chatSequence.MessageSequence.Contains(message))
                {
                    return chatSequence;
                }
            }
            return null;
        }*/
    }

    public class StoryAppsPopup : AStoryAppsView
    {
        // Token: 0x0600252B RID: 9515 RVA: 0x00114A68 File Offset: 0x00112E68
        public StoryAppsPopup(int x, int y, int width, int height, string content = "default", Popup popupType = Popup.Default) : base(x, y, width, height)
        {
            this.scrollView = new StoryAppsScrollView(x, y, width, height, true, true, false);
            this.rect = new StoryAppsRect(x, y, width, height, '0', ' ', 2);
            this.content = new StoryAppsText(new SHGUItextEx(content, x, y, 'w', false), this.scrollView);
            this.startPositionX = x;
            this.startPositionY = y;
            this.AddSubView(this.rect);
            this.AddSubView(this.scrollView);
            this.scrollView.AddScrollItem(this.content);
            this.dontDrawViewsBelow = false;
            this.shouldCloseOnClick = false;
            this.popupType = popupType;
        }

        // Token: 0x0600252C RID: 9516 RVA: 0x00114B1F File Offset: 0x00112F1F
        public Popup GetPopupType()
        {
            return this.popupType;
        }

        // Token: 0x0600252D RID: 9517 RVA: 0x00114B28 File Offset: 0x00112F28
        public override void Update()
        {
            /*base.Update();
            if (!this.blink && this.lifetime == 0f)
            {
                return;
            }
            if (this.lifetime > 0f && Extensions.SetTimerAction(Extensions.KillViewWithSound(SHGUIsound.pong), ref this.lifetimeTimer, this.lifetime))
            {
                StoryAppsManager.Instance.RemovePopup(this);
            }
            if (this.blink)
            {
                this.Blinked = this.SetTimerAction(this.BlinkView('w', 'r', true), ref this.BlinkTimer, this.BlinkTime);
            }
            if (this.move)
            {
                if (!this.movementDone)
                {
                    this.movementDone = this.MoveToPosition(this.moveTargetX, this.moveTargetY, ref this.moveTimer, StoryAppsData.NewChatMoveSpeed);
                }
                else if (this.movementDone && this.moveWithReturn && !this.waitDone)
                {
                    this.waitDone = this.WaitForSeconds(ref this.moveTimer, StoryAppsData.NewChatStayTime);
                }
                else if (this.moveWithReturn && this.waitDone && !this.returnDone)
                {
                    this.returnDone = this.MoveToPosition(this.startPositionX, this.startPositionY, ref this.moveTimer, StoryAppsData.NewChatMoveSpeed);
                }
                else
                {
                    this.move = false;
                }
            }*/
        }

        // Token: 0x0600252E RID: 9518 RVA: 0x00114C8C File Offset: 0x0011308C
        public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
        {
            if (!this.interactable)
            {
                return;
            }
            base.ReactToInputKeyboard(key);
            if (this.fadingOut)
            {
                return;
            }
            if (key == SHSharp::SHGUIinput.esc)
            {
                //SHSharp::SHGUI.current.PlaySound(SHGUIsound.pong);
                SHSharp::SHGUI.current.PopView();
            }
        }

        // Token: 0x0600252F RID: 9519 RVA: 0x00114CDC File Offset: 0x001130DC
        public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
        {
            if (!this.interactable)
            {
                return;
            }
            if (this.fadingOut)
            {
                return;
            }
            if (clicked)
            {
                //SHSharp::SHGUI.current.PlaySound(SHGUIsound.pong);
                SHSharp::SHGUI.current.PopView();
            }
        }

        // Token: 0x06002530 RID: 9520 RVA: 0x00114D11 File Offset: 0x00113111
        public void SetContent(string content)
        {
            this.content.Text.text = content;
            this.content.Text.SmartBreakTextForLineLength(base.Width);
        }

        // Token: 0x06002531 RID: 9521 RVA: 0x00114D3B File Offset: 0x0011313B
        public string GetContent()
        {
            return this.content.Text.text;
        }

        // Token: 0x06002532 RID: 9522 RVA: 0x00114D4D File Offset: 0x0011314D
        public void Move(int targetPositionX, int targetPositionY, bool withReturn = false)
        {
            this.move = true;
            this.moveWithReturn = withReturn;
            this.moveTargetX = targetPositionX;
            this.moveTargetY = targetPositionY;
        }

        // Token: 0x06002533 RID: 9523 RVA: 0x00114D6C File Offset: 0x0011316C
        public void Show(bool interactable, float lifetime = 0f, bool blink = false, bool addToQueue = false)
        {
            //SHSharp::SHGUI.current.PlaySound(SHGUIsound.ping);
            this.interactable = interactable;
            if (addToQueue)
            {
                SHSharp::SHGUI.current.AddViewToQueue(this);
            }
            else
            {
                SHSharp::SHGUI.current.AddViewOnTop(this);
            }
            this.lifetime = lifetime;
            this.blink = blink;
        }

        // Token: 0x040021B0 RID: 8624
        public readonly float BlinkTime = 0f;//StoryAppsData.NewChatPopupBlinkTime;

        // Token: 0x040021B1 RID: 8625
        private readonly StoryAppsText content;

        // Token: 0x040021B2 RID: 8626
        private readonly Popup popupType;

        // Token: 0x040021B3 RID: 8627
        private readonly StoryAppsRect rect;

        // Token: 0x040021B4 RID: 8628
        private readonly StoryAppsScrollView scrollView;

        // Token: 0x040021B5 RID: 8629
        private readonly int startPositionX;

        // Token: 0x040021B6 RID: 8630
        private readonly int startPositionY;

        // Token: 0x040021B7 RID: 8631
        private bool blink;

        // Token: 0x040021B8 RID: 8632
        public bool Blinked;

        // Token: 0x040021B9 RID: 8633
        public float BlinkTimer;

        // Token: 0x040021BA RID: 8634
        private bool inCommanderView;

        // Token: 0x040021BB RID: 8635
        private float lifetime;

        // Token: 0x040021BC RID: 8636
        private float lifetimeTimer;

        // Token: 0x040021BD RID: 8637
        private bool move;

        // Token: 0x040021BE RID: 8638
        private bool movementDone;

        // Token: 0x040021BF RID: 8639
        private int moveTargetX;

        // Token: 0x040021C0 RID: 8640
        private int moveTargetY;

        // Token: 0x040021C1 RID: 8641
        private float moveTimer;

        // Token: 0x040021C2 RID: 8642
        private bool moveWithReturn;

        // Token: 0x040021C3 RID: 8643
        private bool waitDone;

        // Token: 0x040021C4 RID: 8644
        private bool returnDone;
    }

    public sealed class StoryAppsRect : AStoryAppsView
    {
        // Token: 0x06002537 RID: 9527 RVA: 0x00114EE3 File Offset: 0x001132E3
        public StoryAppsRect(int x, int y, int width, int height, char color = '0', char c = ' ', int mode = 2) : base(x, y, width, height)
        {
            this.x = x;
            this.y = y;
            base.Init();
            this.SetColor(color);
            this.c = c;
            this.mode = mode;
        }

        // Token: 0x06002538 RID: 9528 RVA: 0x00114F1D File Offset: 0x0011331D
        public StoryAppsRect SetChar(char fillChar)
        {
            this.c = fillChar;
            return this;
        }

        // Token: 0x06002539 RID: 9529 RVA: 0x00114F28 File Offset: 0x00113328
        public override void Redraw(int offx, int offy)
        {
            if (this.hidden)
            {
                return;
            }
            int num = (int)((float)this.x + (float)(this.x + base.Width) * (1f - this.fade)) + offx;
            int num2 = (int)((float)(this.x + base.Width) * this.fade) + offx;
            int num3 = (int)((float)this.y + (float)(this.y + base.Height) * (1f - this.fade)) + offy;
            int num4 = (int)((float)(this.y + base.Height) * this.fade) + offy;
            for (int i = num; i <= num2; i++)
            {
                for (int j = num3; j <= num4; j++)
                {
                    if (this.mode == 0 || this.mode == 2)
                    {
                        SHSharp::SHGUI.current.SetPixelFront(this.c, i, j, this.color);
                    }
                    if (this.mode == 1 || this.mode == 2)
                    {
                        SHSharp::SHGUI.current.SetPixelBack(this.c, i, j, this.color);
                    }
                }
            }
            base.Redraw(this.x + offx, this.y + offy);
        }

        // Token: 0x040021C6 RID: 8646
        private readonly int mode;

        // Token: 0x040021C7 RID: 8647
        private char c;
    }

    public class StoryAppsScrollView : AStoryAppsView
    {
        // Token: 0x0600253A RID: 9530 RVA: 0x00115068 File Offset: 0x00113468
        public StoryAppsScrollView(int x, int y, int width, int height, bool isFrameEnabled = true, bool isSliderEnabled = true, bool rectsEnabled = true) : base(x, y, width, height)
        {
            this.upperScrollRect = new SHSharp::SHGUIrect(x, 0, x + width, y - 1, '0', ' ', 2);
            this.bottomScrollRect = new SHSharp::SHGUIrect(x, y + height, x + width, 32, '0', ' ', 2);
            this.scrollViewFrame = new SHSharp::SHGUIframe(x - 1, y - 1, x + width, y + height, 'w');
            this.IsFrameEnabled = isFrameEnabled;
            if (rectsEnabled)
            {
                this.AddSubView(this.upperScrollRect);
                this.AddSubView(this.bottomScrollRect);
            }
            this.AddSubView(this.scrollViewFrame);
            this.CreateSlider();
            this.IsSliderEnabled = isSliderEnabled;
            this.IsSliderHidden = !this.IsSliderEnabled;
            this.highestElementDummy = new StoryAppsText(new SHGUItextEx(" ", base.PositionX, base.PositionY, 'w', false), 1);
            this.lowestElement = this.highestElementDummy;
            this.AddScrollItem(this.highestElementDummy);
        }

        // Token: 0x17000229 RID: 553
        // (get) Token: 0x0600253B RID: 9531 RVA: 0x0011516F File Offset: 0x0011356F
        // (set) Token: 0x0600253C RID: 9532 RVA: 0x0011517F File Offset: 0x0011357F
        public bool IsFrameEnabled
        {
            get
            {
                return !this.scrollViewFrame.hidden;
            }
            set
            {
                this.scrollViewFrame.hidden = !value;
            }
        }

        // Token: 0x1700022A RID: 554
        // (get) Token: 0x0600253D RID: 9533 RVA: 0x00115190 File Offset: 0x00113590
        // (set) Token: 0x0600253E RID: 9534 RVA: 0x001151BB File Offset: 0x001135BB
        private bool IsSliderHidden
        {
            get
            {
                return this.slider.hidden && this.sliderBar.hidden && this.IsSliderEnabled;
            }
            set
            {
                this.slider.hidden = value;
                this.sliderBar.hidden = value;
            }
        }

        // Token: 0x1700022B RID: 555
        // (get) Token: 0x0600253F RID: 9535 RVA: 0x001151D5 File Offset: 0x001135D5
        // (set) Token: 0x06002540 RID: 9536 RVA: 0x001151DD File Offset: 0x001135DD
        public bool IsSliderEnabled
        {
            get
            {
                return this.isSliderEnabled;
            }
            set
            {
                this.isSliderEnabled = value;
                this.UpdateSlider();
            }
        }

        // Token: 0x1700022C RID: 556
        // (get) Token: 0x06002541 RID: 9537 RVA: 0x001151EC File Offset: 0x001135EC
        public bool Finished
        {
            get
            {
                return this.GetContentEndingY() <= base.PositionY + base.Height;
            }
        }

        // Token: 0x06002542 RID: 9538 RVA: 0x00115208 File Offset: 0x00113608
        private void CreateSlider()
        {
            string text = "▲";
            for (int i = 0; i < base.Height; i++)
            {
                text += "\n|";
            }
            text += "\n▼";
            this.slider = new SHSharp::SHGUItext(text, base.PositionX + base.Width, base.PositionY - 1, 'w', false);
            this.sliderBar = new SHSharp::SHGUItext("█", base.PositionX + base.Width, base.PositionY, 'w', false);
            this.AddSubView(this.slider);
            this.slider.AddSubView(this.sliderBar);
        }

        // Token: 0x06002543 RID: 9539 RVA: 0x001152B4 File Offset: 0x001136B4
        public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
        {
            /*if (key == SHSharp::SHGUIinput.end)
            {
                this.ScrollToEnd();
            }
            else if (key == SHSharp::SHGUIinput.home)
            {
                this.ScrollToStart();
            }*/
            if (key != SHSharp::SHGUIinput.up)
            {
                if (key == SHSharp::SHGUIinput.down)
                {
                    this.ScrollDown();
                }
            }
            else
            {
                this.ScrollUp();
            }
            base.ReactToInputKeyboard(key);
        }

        // Token: 0x06002544 RID: 9540 RVA: 0x00115317 File Offset: 0x00113717
        public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
        {
            if (scroll != SHSharp::SHGUIinput.scrollUp)
            {
                if (scroll == SHSharp::SHGUIinput.scrollDown)
                {
                    this.ScrollDown();
                }
            }
            else
            {
                this.ScrollUp();
            }
        }

        // Token: 0x06002545 RID: 9541 RVA: 0x00115345 File Offset: 0x00113745
        public override void Redraw(int offx, int offy)
        {
            base.Redraw(offx, offy);
            this.upperScrollRect.Redraw(offx, offy);
            this.bottomScrollRect.Redraw(offx, offy);
            this.scrollViewFrame.Redraw(offx, offy);
            this.RedrawSlider(offx, offy);
        }

        // Token: 0x06002546 RID: 9542 RVA: 0x0011537E File Offset: 0x0011377E
        public void RedrawSlider(int offx, int offy)
        {
            if (this.IsSliderEnabled)
            {
                this.slider.Redraw(offx, offy);
                this.sliderBar.Redraw(offx, offy);
            }
        }

        // Token: 0x06002547 RID: 9543 RVA: 0x001153A8 File Offset: 0x001137A8
        private void ScrollDown()
        {
            if (this.scrollContentList.Count == 0)
            {
                return;
            }
            int contentEndingY = this.GetContentEndingY();
            if (contentEndingY > base.PositionY + base.Height)
            {
                for (int i = 0; i < this.scrollContentList.Count; i++)
                {
                    AStoryAppsView astoryAppsView = this.scrollContentList[i];
                    astoryAppsView.PositionY--;
                }
                this.UpdateSlider();
                this.UpdateElementsVisibility();
            }
        }

        // Token: 0x06002548 RID: 9544 RVA: 0x00115424 File Offset: 0x00113824
        private void ScrollUp()
        {
            if (this.scrollContentList.Any<AStoryAppsView>() && this.GetContentStartY() < base.PositionY)
            {
                foreach (AStoryAppsView astoryAppsView in this.scrollContentList)
                {
                    astoryAppsView.PositionY++;
                }
                this.UpdateSlider();
                this.UpdateElementsVisibility();
            }
        }

        // Token: 0x06002549 RID: 9545 RVA: 0x001154B4 File Offset: 0x001138B4
        private void UpdateSlider()
        {
            if (!this.IsSliderEnabled || this.scrollContentList.Count == 0)
            {
                this.IsSliderHidden = true;
                return;
            }
            int num = this.GetContentEndingY() - this.GetContentStartY();
            if (num <= base.Height)
            {
                this.IsSliderHidden = true;
            }
            else
            {
                this.IsSliderHidden = false;
                int num2 = base.Height - (num - base.Height);
                num2 = Mathf.Clamp(num2, 1, base.Height - 1);
                this.sliderBar.text = string.Empty;
                for (int i = 0; i < num2; i++)
                {
                    SHSharp::SHGUItext shguitext = this.sliderBar;
                    shguitext.text += "█\n";
                }
                float num3 = Math.Abs((float)(this.GetContentStartY() - base.PositionY) / (float)(num - base.Height));
                this.sliderBar.y = base.PositionY + Mathf.FloorToInt((float)(base.Height - num2) * num3);
            }
        }

        // Token: 0x0600254A RID: 9546 RVA: 0x001155AE File Offset: 0x001139AE
        public int GetContentEndingY()
        {
            return this.lowestElement.PositionY + this.lowestElement.Height;
        }

        // Token: 0x0600254B RID: 9547 RVA: 0x001155C7 File Offset: 0x001139C7
        public int GetContentStartY()
        {
            return this.highestElementDummy.PositionY;
        }

        // Token: 0x0600254C RID: 9548 RVA: 0x001155D4 File Offset: 0x001139D4
        public void AddScrollItem(AStoryAppsView view)
        {
            StoryAppsText storyAppsText = view as StoryAppsText;
            if (storyAppsText != null && storyAppsText.Height > 1)
            {
                this.AddSplitText(storyAppsText);
            }
            else
            {
                this.scrollContentList.Add(view);
                this.AddSubView(view);
                this.SetLowestPosition(view);
            }
            this.UpdateSlider();
            this.UpdateElementsVisibility();
        }

        // Token: 0x0600254D RID: 9549 RVA: 0x00115630 File Offset: 0x00113A30
        private void AddSplitText(StoryAppsText text)
        {
            string text2 = text.Text.text;
            int num = 0;
            int y = text.PositionY;
            for (int i = 0; i < text2.Length; i++)
            {
                if (text2[i] == '\n')
                {
                    StoryAppsText storyAppsText = new StoryAppsText(new SHGUItextEx(text2.Substring(num, i - num), text.PositionX, y, text.color, false), text.Width);
                    this.scrollContentList.Add(storyAppsText);
                    this.AddSubView(storyAppsText);
                    this.SetLowestPosition(storyAppsText);
                    num = i + 1;
                    y = storyAppsText.PositionY + storyAppsText.Height;
                }
            }
            StoryAppsText storyAppsText2 = new StoryAppsText(new SHGUItextEx(text2.Substring(num, text2.Length - num), text.PositionX, y, text.color, false), text.Width);
            this.scrollContentList.Add(storyAppsText2);
            this.AddSubView(storyAppsText2);
            this.SetLowestPosition(storyAppsText2);
        }

        // Token: 0x0600254E RID: 9550 RVA: 0x00115720 File Offset: 0x00113B20
        public void RemoveScrollItem(AStoryAppsView view)
        {
            this.scrollContentList.Remove(view);
            this.RemoveView(view);
            this.UpdateSlider();
        }

        // Token: 0x0600254F RID: 9551 RVA: 0x0011573C File Offset: 0x00113B3C
        public void ScrollToEnd()
        {
            if (!this.scrollContentList.Any<AStoryAppsView>())
            {
                return;
            }
            int contentEndingY = this.GetContentEndingY();
            if (contentEndingY > base.PositionY + base.Height)
            {
                int num = contentEndingY - base.PositionY - base.Height;
                for (int i = 0; i < this.scrollContentList.Count; i++)
                {
                    AStoryAppsView astoryAppsView = this.scrollContentList[i];
                    astoryAppsView.PositionY -= num;
                }
                this.UpdateSlider();
                this.UpdateElementsVisibility();
            }
        }

        // Token: 0x06002550 RID: 9552 RVA: 0x001157C8 File Offset: 0x00113BC8
        public void ScrollToStart()
        {
            if (!this.scrollContentList.Any<AStoryAppsView>())
            {
                return;
            }
            int contentStartY = this.GetContentStartY();
            if (this.GetContentStartY() < base.PositionY)
            {
                int num = base.PositionY - contentStartY;
                for (int i = 0; i < this.scrollContentList.Count; i++)
                {
                    AStoryAppsView astoryAppsView = this.scrollContentList[i];
                    astoryAppsView.PositionY += num;
                }
                this.UpdateSlider();
                this.UpdateElementsVisibility();
            }
        }

        // Token: 0x06002551 RID: 9553 RVA: 0x0011584C File Offset: 0x00113C4C
        public List<StoryAppsText> GetAllTextElements()
        {
            List<StoryAppsText> list = new List<StoryAppsText>();
            for (int i = 0; i < this.scrollContentList.Count; i++)
            {
                SHSharp::SHGUIview shguiview = this.scrollContentList[i];
                if (shguiview is StoryAppsText)
                {
                    list.Add(shguiview as StoryAppsText);
                }
            }
            return list;
        }

        // Token: 0x06002552 RID: 9554 RVA: 0x001158A0 File Offset: 0x00113CA0
        private void UpdateElementsVisibility()
        {
            for (int i = 0; i < this.scrollContentList.Count; i++)
            {
                AStoryAppsView astoryAppsView = this.scrollContentList[i];
                bool flag = astoryAppsView.PositionY + astoryAppsView.Height > base.PositionY && astoryAppsView.PositionY < base.PositionY + base.Height;
                if (astoryAppsView.Active != flag)
                {
                    astoryAppsView.Active = flag;
                }
            }
        }

        // Token: 0x06002553 RID: 9555 RVA: 0x0011591A File Offset: 0x00113D1A
        private void SetLowestPosition(AStoryAppsView view)
        {
            if (view.PositionY > this.lowestElement.PositionY)
            {
                this.lowestElement = view;
            }
        }

        // Token: 0x040021C8 RID: 8648
        private readonly SHSharp::SHGUIrect bottomScrollRect;

        // Token: 0x040021C9 RID: 8649
        private readonly StoryAppsText highestElementDummy;

        // Token: 0x040021CA RID: 8650
        private readonly List<AStoryAppsView> scrollContentList = new List<AStoryAppsView>();

        // Token: 0x040021CB RID: 8651
        private readonly SHSharp::SHGUIframe scrollViewFrame;

        // Token: 0x040021CC RID: 8652
        private readonly SHSharp::SHGUIrect upperScrollRect;

        // Token: 0x040021CD RID: 8653
        private bool isSliderEnabled;

        // Token: 0x040021CE RID: 8654
        private SHSharp::SHGUItext slider;

        // Token: 0x040021CF RID: 8655
        private SHSharp::SHGUItext sliderBar;

        // Token: 0x040021D0 RID: 8656
        private AStoryAppsView lowestElement;
    }

    public class StoryAppsText : AStoryAppsView
    {
        // Token: 0x06002554 RID: 9556 RVA: 0x0011593C File Offset: 0x00113D3C
        public StoryAppsText(SHGUItextEx text, int width) : base(text.x, text.y, text.GetLongestLineLength(), text.CountLines())
        {
            this.Text = text;
            this.Text.SmartBreakTextForLineLength(width);
            base.Height = text.CountLines();
            this.color = text.color;
            this.AddSubView(this.Text);
        }

        // Token: 0x06002555 RID: 9557 RVA: 0x001159A0 File Offset: 0x00113DA0
        public StoryAppsText(SHGUItextEx text, StoryAppsScrollView scrollView) : base(text.x, text.y, text.GetLongestLineLength(), text.CountLines())
        {
            this.Text = text;
            this.AdjustWidthToScrollView(scrollView);
            base.Height = text.CountLines();
            this.AddSubView(this.Text);
        }

        // Token: 0x1700022D RID: 557
        // (get) Token: 0x06002556 RID: 9558 RVA: 0x001159F2 File Offset: 0x00113DF2
        // (set) Token: 0x06002557 RID: 9559 RVA: 0x001159FA File Offset: 0x00113DFA
        public SHGUItextEx Text { get; set; }

        // Token: 0x06002558 RID: 9560 RVA: 0x00115A03 File Offset: 0x00113E03
        public void AdjustWidthToScrollView(StoryAppsScrollView scrollView)
        {
            this.Text.SmartBreakTextForLineLength(scrollView.PositionX + scrollView.Width - base.PositionX);
        }
    }
}
