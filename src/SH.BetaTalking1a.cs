extern alias SHSharp;

using SH.SHGUI.Extended;

namespace SH.BetaTalking1a {
    public class SHBetaTalking1a
    {
        // Token: 0x06000067 RID: 103 RVA: 0x000046C4 File Offset: 0x000028C4
        public SHBetaTalking1a()
        {
            ExAPPguruchat appguruchat = new ExAPPguruchat();
            SHSharp::SHGUI.current.AddViewOnTop(appguruchat);
            appguruchat.AddOtherMessage("on", "Hey, made any progress yet?");
            appguruchat.AddMyMessage("asksdjas;kj", "Nah, I'm stuck on this one level. With a prison or something.");
            appguruchat.AddOtherMessage("on", "The one in Hawaii? With palm trees and wooden cages?");
            appguruchat.AddMyMessage("asksdjas;kj", "What?? No! It's like an underground facility. Dumpy place with some guys treating me like a dog.");
            appguruchat.AddOtherMessage("on", "Ah, this prison! With a grate overhead?");
            appguruchat.AddMyMessage("asksdjas;kj", "Yup. I've been sitting there for ages. At least it feels that way :P Is there a way to break out of there? Or is it another unfinished thing?");
            appguruchat.AddOtherMessage("on", "It works all right. Have some patience :>");
            appguruchat.AddMyMessage("asksdjas;kj", "Right. I'l log in again, maybe something changed.");
            appguruchat.AddMyQuit();
            //SHSharp::SHGUI.current.AddViewToQueue(new SHSharp::APPnextlevel(""));
        }
    }
}