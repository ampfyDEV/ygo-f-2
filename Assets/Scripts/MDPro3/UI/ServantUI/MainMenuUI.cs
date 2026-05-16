using MDPro3.Net;
using MDPro3;
using UnityEngine;
using UnityEngine.Networking;
using YgomSystem.ElementSystem;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using MDPro3.Servant;

namespace MDPro3.UI.ServantUI
{
    public class MainMenuUI : ServantUI
    {
        public override void Initialize(Servant.Servant servant)
        {
            base.Initialize(servant);

            Title.text = "MDPro3 v" + Application.version;
        }

        #region Button Function

        public void OnSolo()
        {
            if (Program.exitOnReturn)
                return;

            Program.instance.solo.SwitchCondition(SoloSelector.Condition.ForSolo);
            Program.instance.ShiftToServant(Program.instance.solo);
        }

        public void OnReplay()
        {
            if (Program.exitOnReturn)
                return;

            Program.instance.ShiftToServant(Program.instance.replay);
        }


        public void OnDeck()
        {
            if (Program.exitOnReturn)
                return;

            Program.instance.deckSelector.SwitchCondition(DeckSelector.Condition.ForEdit);
            Program.instance.ShiftToServant(Program.instance.deckSelector);
        }

        public void OnSetting()
        {
            if (Program.exitOnReturn)
                return;

            Program.instance.ShiftToServant(Program.instance.setting);
        }

        public void OnExit()
        {
            if (Program.exitOnReturn)
                return;

            List<string> selections = new()
            {
                InterString.Get("确认退出"),
                InterString.Get("即将退出应用程序，@n是否确认？"),
                InterString.Get("确认"),
                InterString.Get("取消"),
                Config.STRING_YES
            };
            UIManager.ShowPopupYesOrNo(selections, Program.GameQuit, null);
        }

        #endregion
    }
}