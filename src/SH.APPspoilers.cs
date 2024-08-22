extern alias SHSharp;

using System;
using UnityEngine;
using MOREApps;
using SHSharp::Assets.Scripts.Utilities;

namespace SH.APPspoilers
{
    public class SHAPPspoilers : SHSharp::SHGUIview
    {
        public SHAPPspoilers()
        {
            this.allowCursorDraw = false;
            this.dontDrawViewsBelow = false;
            base.AddSubView(new SHSharp::SHGUIrect(0, 0, SHSharp::SHGUI.current.resolutionX, SHSharp::SHGUI.current.resolutionY, '0', ' ', 2));
            this.timeline = new SHSharp::SHGUIanimationtimeline();
            base.AddSubView(this.timeline);

            SHSharp::SHGUIchoice choice = null;

            Action onYes = delegate ()
            {
                choice.Kill();
                this.SpoilersTimeline();  // Start the spoilers timeline
            };

            Action onNo = delegate ()
            {
                choice.Kill();
                this.timeline.GetPhaseByName("dontWantPhase").Start();  // Start the "Do you really care to know?" phase
            };

            // Initial choice phase
            this.timeline.NewPhase().OnStart(delegate
            {
                choice = new SHSharp::SHGUIchoice("APPspoilers.01.DO_YOU_WANT_TO_KNOW_THE_ENDING".T(Array.Empty<string>()), 32, 10);
                choice.SetOnYes(onYes);
                choice.SetOnNo(onNo);
                this.timeline.AddSubView(choice);
            }).EndCondition(() => choice.remove).OnEnd(delegate
            {
                this.timeline.RemoveView(choice);
            });

            // You Don’t Care phase
            this.AddYouDontCarePhase();

            // Final phase
            this.timeline.NewPhase().EndAfter(0.1f);
            this.timeline.NewPhase().OnStart(delegate
            {
                this.Kill();
            });
        }

        public override void ReactToInputKeyboard(SHSharp::SHGUIinput key)
        {
            if (this.fadingOut)
            {
                return;
            }
            base.ReactToInputKeyboard(key);
            if (key == SHSharp::SHGUIinput.esc)
            {
                this.Kill();
            }
        }

        public override void ReactToInputMouse(int x, int y, bool clicked, SHSharp::SHGUIinput scroll)
        {
            if (this.fadingOut)
            {
                return;
            }
            base.ReactToInputMouse(x, y, clicked, scroll);
            if (clicked)
            {
                SHSharp::SHGUI.current.PopView();
            }
        }

        private void SpoilersTimeline()
        {
            SHSharp::AppHotswitch scrambleViewIn = new SHSharp::AppHotswitch();
            scrambleViewIn.IsFilling = false;
            scrambleViewIn.SetFillWithTransparency(false);
            scrambleViewIn.dontDrawViewsBelow = false;
            scrambleViewIn.PlayScrambleOutSound();
            SHSharp::SHGUI.current.AddViewOnTop(scrambleViewIn);
            DelayedInvokeMarshal.Instance.Enqueue(delegate
            {
                scrambleViewIn.KillInstant();
            }, 1f, true);

            this.console = new SHSharp::APPscrollconsole();
            this.doYouReally = new SHSharp::SHGUItempview(3f);

            // Here, we use AddChoice to present another prompt or decision point
            this.AddChoice();

            //this.AddScrambleConsole();
            string text = "APPspoilers.03.IF_YOU_REALLY_WANT_TO_KNOW_YOU".T(Array.Empty<string>());
            this.doYouReally.AddSubView(new SHSharp::SHGUItext(text, 32 - text.Length / 2, 11, 'w', false));
            SHSharp::SHGUI.current.AddViewToQueue(this.doYouReally);

            // This might be where we trigger the spoilers phase after additional animations
            this.timeline.NewPhase().OnStart(delegate
            {
                this.timeline.GetPhaseByName("spoilersPhase").Start();  // Start the spoilers phase
            }).EndCondition(() => true);
        }

        private void AddChoice()
        {
            SHSharp::SHGUIview v = new SHSharp::SHGUIview();
            SHSharp::SHGUIchoice shguichoice = new SHSharp::SHGUIchoice("APPspoilers.04.DO_YOU_WANT_TO_KNOW_WHAT_YOU_ARE".T(Array.Empty<string>()), 32, 10);
            shguichoice.SetOnYes(delegate
            {
                v.Kill();
            });
            shguichoice.SetOnNo(delegate
            {
                v.Kill();
                this.console.Kill();
                this.doYouReally.Kill();
            });
            v.AddSubView(shguichoice);
            SHSharp::SHGUI.current.AddViewToQueue(v);
        }

        private void AddScrambleConsole()
        {
            this.console.AddWait(0.5f);
            this.AsciiArtLineByLine(this.console, "APPspoilers.02.SPOILERS".T(Array.Empty<string>()), 1E-05f, 'z', false);
            SHSharp::SHGUI.current.AddViewToQueue(this.console);
        }

        protected void AsciiArtLineByLine(SHSharp::APPscrollconsole console, string artname, float lineDelay, char color, bool centered)
        {
            string[] array = SHSharp::SHGUI.current.GetASCIIartByName(artname).Split(new char[]
            {
                '\n'
            });
            int offset = SHSharp::SHGUI.current.resolutionX / 2 - array[0].Length / 2;
            for (int i = 0; i < array.Length; i++)
            {
                if (centered)
                {
                    console.AddTextToQueue(array[i], lineDelay, color, offset);
                }
                else
                {
                    console.AddTextToQueue(array[i], lineDelay, color, 0);
                }
            }
        }

        private void AddYouDontCarePhase()
        {
            float duration = 3f;
            string text = "APPspoilers.06.DO_YOU_REALLY_CARE_TO_KNOW".T(Array.Empty<string>());
            SHSharp::SHGUIview view = new SHSharp::SHGUIview();
            view.x = SHSharp::SHGUI.current.resolutionX / 2 - text.Length / 2;
            view.y = SHSharp::SHGUI.current.resolutionY / 2 - 1;
            view.AddSubView(new SHSharp::SHGUItext(text, 0, 0, 'w', false));
            this.timeline.NewPhase().OnStart(delegate
            {
                this.timeline.AddSubView(view);
            }).OnEnd(delegate
            {
                view.Kill();
            }).EndAfter(duration).SetName("dontWantPhase");
        }

        private SHSharp::APPscrollconsole console;
        private SHSharp::SHGUIview doYouReally;
        private SHSharp::SHGUIanimationtimeline timeline;
    }
}
