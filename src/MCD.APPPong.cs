extern alias SHSharp;

using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

using SHAM = SHSharp.AudioManager;
using MelonLoader;

namespace MCD.APPPong
{
    public class MCDAPPPong : SHSharp::SHGUIappbase
    {
        // Token: 0x06001406 RID: 5126 RVA: 0x0007649C File Offset: 0x0007489C
        public MCDAPPPong() : base("PONG-ported-from-mcdbeta-by-Garbear", true)
        {
            this.newHeight = this.windowHeight - 3;
            this.f1 = new MCDAPPPong.field[]
            {
            new MCDAPPPong.field(this.windowStartX + 1, 5),
            new MCDAPPPong.field(this.windowStartX + 1, 6),
            new MCDAPPPong.field(this.windowStartX + 1, 7)
            };
            this.p1 = new MCDAPPPong.player(this.f1, '█');
            this.f2 = new MCDAPPPong.field[]
            {
            new MCDAPPPong.field(this.windowWidth - 1, 5),
            new MCDAPPPong.field(this.windowWidth - 1, 6),
            new MCDAPPPong.field(this.windowWidth - 1, 7)
            };
            this.p2 = new MCDAPPPong.player(this.f2, '█');
            this.b1 = new MCDAPPPong.ball(this.windowWidth / 2, this.newHeight / 2, '█');
            this.lastUpdate = (double)Time.time;
            this.ballTrial = new MCDAPPPong.ball[]
            {
            new MCDAPPPong.ball(this.b1.X, this.b1.Y, '░'),
            new MCDAPPPong.ball(this.b1.X, this.b1.Y, '▒'),
            new MCDAPPPong.ball(this.b1.X, this.b1.Y, '▓')
            };
            this.ballMoves = 0;
            this.scoreP1 = 0;
            this.scoreP2 = 0;
            this.manualP1p1 = "↑ = up arrow";
            this.manualP1p2 = "↓ = down arrow";
            this.manualP2p1 = "↑ = left arrow";
            this.manualP2p2 = "↓ = right arrow";
            this.isAIactive = true;
            this.pauseTime = (double)Time.time;
            this.lastTick = Environment.TickCount;
            this.lastRedrawTick = Environment.TickCount;
            this.pauseStartTick = Environment.TickCount;
            this.accumlatedTime = 0;
            this.speed = new int[]
            {
            -1,
            1
            };
            this.speedX = this.speed[(int)Mathf.Round(UnityEngine.Random.Range(0f, 1f))];
            this.speedY = this.speed[(int)Mathf.Round(UnityEngine.Random.Range(0f, 1f))];
        }

        // Token: 0x06001407 RID: 5127 RVA: 0x00076740 File Offset: 0x00074B40
        public override void Update()
        {
            base.Update();

            if (this.isAIactive)
            {
                this.AI();
            }

            int currentTick = Environment.TickCount;

            if (currentTick - this.lastTick > redrawInterval) // 20 milliseconds
            {
                this.moveBall();
                this.checkBallColission();
                this.lastTick = currentTick; // Reset the last tick
            }

            if (currentTick - this.lastRedrawTick > redrawInterval)
            {
                this.Redraw(0, 0);
                this.lastRedrawTick = currentTick;
            }
        }

        // Token: 0x06001408 RID: 5128 RVA: 0x000767B4 File Offset: 0x00074BB4
        public override void Redraw(int offx, int offy)
        {
            base.Redraw(offx, offy);

            int currentTick = Environment.TickCount;
            if (currentTick - this.pauseStartTick > 5) // 5 milliseconds for pause duration
            {
                this.drawTail();
                SHSharp::SHGUI.current.SetPixelFront(this.b1.print, this.b1.X, this.b1.Y, 'w');
            }

            this.drawPoints();
            this.drawManual();

            for (int i = 0; i < this.p1.fields.Length; i++)
            {
                SHSharp::SHGUI.current.SetPixelFront(this.p1.print, this.p1.fields[i].X, this.p1.fields[i].Y, 'w');
                SHSharp::SHGUI.current.SetPixelFront(this.p2.print, this.p2.fields[i].X, this.p2.fields[i].Y, 'w');
            }

            SHSharp::SHGUI.current.DrawLine("├─┤", this.windowStartX - 1, this.windowWidth + 1, this.newHeight, true, '9', 1f);
            SHSharp::SHGUI.current.DrawLine("┬│┴", this.newHeight, this.windowHeight + 1, this.windowWidth / 2, false, '9', 1f);
        }

        // Token: 0x06001409 RID: 5129 RVA: 0x00076921 File Offset: 0x00074D21
        public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
        {
            if (this.fadingOut)
            {
                return;
            }
        }

        // Token: 0x0600140A RID: 5130 RVA: 0x00076930 File Offset: 0x00074D30
        public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
        {
            if (this.fadingOut)
            {
                return;
            }
            if (key == SHSharp::SHGUIinput.esc)
            {
                this.Kill();
            }
            if (key == SHSharp::SHGUIinput.enter)
            {
                this.isAIactive = !this.isAIactive;
            }
            if (Input.GetKey(KeyCode.U) && this.p1.fields[0].Y > this.windowStartY)
            {
                Debug.Log("p1");
                for (int i = 0; i < this.p1.fields.Length; i++)
                {
                    this.p1.fields[i].Y = this.p1.fields[i].Y - 1;
                }
            }
            else if (Input.GetKey(KeyCode.J) && this.p1.fields[this.p1.fields.Length - 1].Y < this.newHeight - 1)
            {
                for (int j = 0; j < this.p1.fields.Length; j++)
                {
                    this.p1.fields[j].Y = this.p1.fields[j].Y + 1;
                }
            }
            if (Input.GetKey(KeyCode.O) && this.p1.fields[0].Y > this.windowStartY)
            {
                Debug.Log("p1");
                for (int k = 0; k < this.p1.fields.Length; k++)
                {
                    this.p1.fields[k].Y = this.p1.fields[k].Y - 1;
                }
            }
            else if (Input.GetKey(KeyCode.P) && this.p1.fields[this.p1.fields.Length - 1].Y < this.newHeight - 1)
            {
                for (int l = 0; l < this.p1.fields.Length; l++)
                {
                    this.p1.fields[l].Y = this.p1.fields[l].Y + 1;
                }
            }
            if (key == SHSharp::SHGUIinput.up && this.p1.fields[0].Y > this.windowStartY)
            {
                for (int m = 0; m < this.p1.fields.Length; m++)
                {
                    this.p1.fields[m].Y = this.p1.fields[m].Y - 1;
                }
            }
            else if (key == SHSharp::SHGUIinput.down && this.p1.fields[this.p1.fields.Length - 1].Y < this.newHeight - 1)
            {
                for (int n = 0; n < this.p1.fields.Length; n++)
                {
                    this.p1.fields[n].Y = this.p1.fields[n].Y + 1;
                }
            }
            if (key == SHSharp::SHGUIinput.left && this.p2.fields[0].Y > this.windowStartY)
            {
                for (int num = 0; num < this.p2.fields.Length; num++)
                {
                    this.p2.fields[num].Y = this.p2.fields[num].Y - 1;
                }
            }
            else if (key == SHSharp::SHGUIinput.right && this.p2.fields[this.p2.fields.Length - 1].Y < this.newHeight - 1)
            {
                for (int num2 = 0; num2 < this.p2.fields.Length; num2++)
                {
                    this.p2.fields[num2].Y = this.p2.fields[num2].Y + 1;
                }
            }
        }

        // Token: 0x0600140B RID: 5131 RVA: 0x00076DA0 File Offset: 0x000751A0
        private void moveBall()
        {
            this.ballMoves++;
            if (this.ballMoves == 1)
            {
                this.ballTrial[2].X = this.b1.X;
                this.ballTrial[2].Y = this.b1.Y;
            }
            else if (this.ballMoves == 2)
            {
                this.ballTrial[1].X = this.ballTrial[2].X;
                this.ballTrial[1].Y = this.ballTrial[2].Y;
                this.ballTrial[2].X = this.b1.X;
                this.ballTrial[2].Y = this.b1.Y;
            }
            else if (this.ballMoves >= 3)
            {
                this.ballTrial[0].X = this.ballTrial[1].X;
                this.ballTrial[0].Y = this.ballTrial[1].Y;
                this.ballTrial[1].X = this.ballTrial[2].X;
                this.ballTrial[1].Y = this.ballTrial[2].Y;
                this.ballTrial[2].X = this.b1.X;
                this.ballTrial[2].Y = this.b1.Y;
            }
            this.b1.X = this.b1.X + this.speedX;
            this.b1.Y = this.b1.Y + this.speedY;
        }

        // Token: 0x0600140C RID: 5132 RVA: 0x00076F98 File Offset: 0x00075398
        private void resetBall()
        {
            this.pauseStartTick = Environment.TickCount;
            this.ballMoves = 0;
            this.b1.X = this.windowWidth / 2;
            for (int i = 0; i < this.ballTrial.Length; i++)
            {
                this.ballTrial[i].X = this.b1.X;
                this.ballTrial[i].Y = this.b1.Y;
            }
            this.speedY = this.speed[(int)Mathf.Round((float)UnityEngine.Random.Range(0, 1))];
        }

        // Token: 0x0600140D RID: 5133 RVA: 0x00077037 File Offset: 0x00075437
        private void checkBallColission()
        {
            this.colissionWithPlayer();
            this.colissionWithWall();
        }

        // Token: 0x0600140E RID: 5134 RVA: 0x00077048 File Offset: 0x00075448
        private void colissionWithPlayer()
        {
            if ((this.b1.X + this.speedX == this.p1.fields[0].X && this.b1.Y >= this.p1.fields[0].Y && this.b1.Y <= this.p1.fields[this.p1.fields.Length - 1].Y) || (this.b1.X + this.speedX == this.p2.fields[0].X && this.b1.Y >= this.p2.fields[0].Y && this.b1.Y <= this.p2.fields[this.p2.fields.Length - 1].Y))
            {
                this.speedX *= -1;
            }
            else if ((this.b1.X + this.speedX == this.p1.fields[0].X && this.b1.Y + this.speedY == this.p1.fields[0].Y && this.b1.X == this.p1.fields[0].X + 1 && this.b1.Y == this.p1.fields[0].Y - 1) || (this.b1.X + this.speedX == this.p2.fields[0].X && this.b1.Y + this.speedY == this.p2.fields[0].Y && this.b1.X == this.p2.fields[0].X - 1 && this.b1.Y == this.p2.fields[0].Y - 1))
            {
                this.speedX *= -1;
                this.speedY *= -1;
            }
            else if ((this.b1.X + this.speedX == this.p1.fields[this.p1.fields.Length - 1].X && this.b1.Y + this.speedY == this.p1.fields[this.p1.fields.Length - 1].Y && this.b1.X == this.p1.fields[this.p1.fields.Length - 1].X + 1 && this.b1.Y == this.p1.fields[this.p1.fields.Length - 1].Y + 1) || (this.b1.X + this.speedX == this.p2.fields[this.p2.fields.Length - 1].X && this.b1.Y + this.speedY == this.p2.fields[this.p2.fields.Length - 1].Y && this.b1.X == this.p2.fields[this.p2.fields.Length - 1].X - 1 && this.b1.Y == this.p2.fields[this.p2.fields.Length - 1].Y + 1))
            {
                this.speedX *= -1;
                this.speedY *= -1;
            }
        }

        // Token: 0x0600140F RID: 5135 RVA: 0x000774CC File Offset: 0x000758CC
        private void colissionWithWall()
        {
            if (this.b1.Y >= this.newHeight - 1 || this.b1.Y <= this.windowStartY)
            {
                this.speedY *= -1;
            }
            if (this.b1.X < this.windowStartX)
            {
                this.scoreP1++;
                this.resetBall();
            }
            else if (this.b1.X > this.windowWidth)
            {
                this.scoreP2++;
                this.resetBall();
            }
        }

        // Token: 0x06001410 RID: 5136 RVA: 0x00077570 File Offset: 0x00075970
        private void drawTail()
        {
            for (int i = 0; i < this.ballTrial.Length; i++)
            {
                SHSharp::SHGUI.current.SetPixelFront(this.ballTrial[i].print, this.ballTrial[i].X, this.ballTrial[i].Y, 'B');
            }
        }

        // Token: 0x06001411 RID: 5137 RVA: 0x000775D8 File Offset: 0x000759D8
        private void drawPoints()
        {
            SHSharp::SHGUI.current.DrawText(this.scoreP2.ToString(), this.windowWidth / 2 - this.scoreP2.ToString().Length - 1, this.newHeight + 1, 'w', 1f, ' ', false, false);
            SHSharp::SHGUI.current.DrawText(this.scoreP1.ToString(), this.windowWidth / 2 + 2, this.newHeight + 1, 'w', 1f, ' ', false, false);
        }

        // Token: 0x06001412 RID: 5138 RVA: 0x0007766C File Offset: 0x00075A6C
        private void drawManual()
        {
            SHSharp::SHGUI.current.DrawText(this.manualP1p1, 2, this.newHeight + 1, 'w', 1f, ' ', false, false);
            SHSharp::SHGUI.current.DrawText(this.manualP1p2, 2, this.newHeight + 2, 'w', 1f, ' ', false, false);
            SHSharp::SHGUI.current.DrawText(this.manualP2p1, this.windowWidth - this.manualP2p1.Length - 1, this.newHeight + 1, 'w', 1f, ' ', false, false);
            SHSharp::SHGUI.current.DrawText(this.manualP2p2, this.windowWidth - this.manualP1p2.Length - 1, this.newHeight + 2, 'w', 1f, ' ', false, false);
        }

        // Token: 0x06001413 RID: 5139 RVA: 0x00077730 File Offset: 0x00075B30
        private void AI()
        {
            if (this.p2.fields[1].Y < this.b1.Y && this.p2.fields[this.p2.fields.Length - 1].Y + 1 < this.newHeight)
            {
                for (int i = 0; i < this.p1.fields.Length; i++)
                {
                    this.p2.fields[i].Y = this.p2.fields[i].Y + 1;
                }
            }
            else if (this.p2.fields[1].Y > this.b1.Y && this.p2.fields[0].Y - 1 > this.windowStartY)
            {
                for (int j = 0; j < this.p1.fields.Length; j++)
                {
                    this.p2.fields[j].Y = this.p2.fields[j].Y - 1;
                }
            }
        }

        // Token: 0x06001414 RID: 5140 RVA: 0x0007787A File Offset: 0x00075C7A
        private void Restart()
        {
            MelonLogger.Msg("Restart");
            this.Kill();
            SHSharp::SHGUI.current.AddViewOnTop(new MCDAPPPong());
        }

        // Token: 0x040010DA RID: 4314
        private int speedX;

        // Token: 0x040010DB RID: 4315
        private int speedY;

        // Token: 0x040010DC RID: 4316
        private int windowWidth = SHSharp::SHGUI.current.resolutionX - 2;

        // Token: 0x040010DD RID: 4317
        private int windowHeight = SHSharp::SHGUI.current.resolutionY - 2;

        // Token: 0x040010DE RID: 4318
        private int windowStartX = 1;

        // Token: 0x040010DF RID: 4319
        private int windowStartY = 1;

        // Token: 0x040010E0 RID: 4320
        private int newHeight;

        // Token: 0x040010E1 RID: 4321
        public MCDAPPPong.field[] f1;

        // Token: 0x040010E2 RID: 4322
        public MCDAPPPong.player p1;

        // Token: 0x040010E3 RID: 4323
        public MCDAPPPong.field[] f2;

        // Token: 0x040010E4 RID: 4324
        public MCDAPPPong.player p2;

        // Token: 0x040010E5 RID: 4325
        public MCDAPPPong.ball b1;

        // Token: 0x040010E6 RID: 4326
        public double lastUpdate;

        // Token: 0x040010E7 RID: 4327
        public MCDAPPPong.ball[] ballTrial;

        // Token: 0x040010E8 RID: 4328
        public int ballMoves;

        // Token: 0x040010E9 RID: 4329
        public int scoreP1;

        // Token: 0x040010EA RID: 4330
        public int scoreP2;

        // Token: 0x040010EB RID: 4331
        public string manualP1p1;

        // Token: 0x040010EC RID: 4332
        public string manualP1p2;

        // Token: 0x040010ED RID: 4333
        public string manualP2p1;

        // Token: 0x040010EE RID: 4334
        public string manualP2p2;

        // Token: 0x040010EF RID: 4335
        public bool isAIactive;

        // Token: 0x040010F0 RID: 4336
        public double pauseTime;

        public double accumlatedTime = 0;

        private int lastTick;

        private int lastRedrawTick;
        private int redrawInterval = 40; // 20 milliseconds for redraw
        private int pauseStartTick;
        //private int moveBallInterval = 40; // 20 milliseconds for ball movement

        // Token: 0x040010F1 RID: 4337
        public int[] speed;

        // Token: 0x0200036B RID: 875
        public struct player
        {
            // Token: 0x06001415 RID: 5141 RVA: 0x00077892 File Offset: 0x00075C92
            public player(MCDAPPPong.field[] f, char p)
            {
                this.fields = f;
                this.print = p;
            }

            // Token: 0x040010F2 RID: 4338
            public MCDAPPPong.field[] fields;

            // Token: 0x040010F3 RID: 4339
            public char print;
        }

        // Token: 0x0200036C RID: 876
        public struct ball
        {
            // Token: 0x06001416 RID: 5142 RVA: 0x000778A2 File Offset: 0x00075CA2
            public ball(int x, int y, char p)
            {
                this.X = x;
                this.Y = y;
                this.print = p;
            }

            // Token: 0x040010F4 RID: 4340
            public int X;

            // Token: 0x040010F5 RID: 4341
            public int Y;

            // Token: 0x040010F6 RID: 4342
            public char print;
        }

        // Token: 0x0200036D RID: 877
        public struct field
        {
            // Token: 0x06001417 RID: 5143 RVA: 0x000778B9 File Offset: 0x00075CB9
            public field(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }

            // Token: 0x040010F7 RID: 4343
            public int X;

            // Token: 0x040010F8 RID: 4344
            public int Y;
        }
    }
}