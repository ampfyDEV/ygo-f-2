using MDPro3;
using MDPro3.Servant;
using UnityEngine;

public class RpgCore : Servant
{
    public override int Depth => 0;
    protected override bool ShowLine => false;
    protected override bool NeedExitButton => false;

}
