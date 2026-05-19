using MDPro3;
using MDPro3.UI.ServantUI;
using UnityEngine;

public class RpgSelectorUI : ServantUI
{


    #region Button Function
    public void OnNewGame()
    {
        if (Program.exitOnReturn)
            return;


        Program.instance.ShiftToServant(Program.instance.rpgCore);
    }
    public void OnContinue()
    {
        if (Program.exitOnReturn)
            return;

        Program.instance.ShiftToServant(Program.instance.rpgCore);
    }
    public void OnLoadGame()
    {
        if (Program.exitOnReturn)
            return;

        Program.instance.ShiftToServant(Program.instance.rpgCore);
    }

    #endregion
}
