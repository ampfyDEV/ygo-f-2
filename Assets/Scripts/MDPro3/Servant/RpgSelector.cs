using MDPro3;
using MDPro3.Servant;
using MDPro3.UI;
using UnityEngine;

public class RpgSelector : Servant
{
    public override int Depth => 3;
    protected override bool ShowLine => false;

    [HideInInspector] public SelectionButton_MainMenu lastSelectedButton;

    public override void Initialize()
    {
        base.Initialize();
        returnServant = Program.instance.menu;
    }

    public override void Select(bool forced = false)
    {
        if (!forced && !UserInput.NeedDefaultSelect())
            return;

        if (lastSelectedButton != null)
            lastSelectedButton.GetSelectable().Select();
        else
            GetUI<RpgSelectorUI>().SelectDefaultSelectable();
    }
}
