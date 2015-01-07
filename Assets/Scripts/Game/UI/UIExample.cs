using UnityEngine;
using System.Collections;
using GDM.UI;


public class UIExample : UIBase, IFTEBuddy
{
    protected override void OnAwake()
    {

    }

    protected override void OnStart()
    {

    }

    public override void OnCreate(object param)
    {

    }

    public override void OnVisible(object param)
    {

    }

    public override void OnInvisible(object param)
    {

    }

    public override void OnDestroy()
    {

    }

    protected override void RegisterEvent()
    {
        base.RegisterEvent();
    }

    public void OnFTETrigger(IFTEBuddy buddy, GameObject go, string param)
    {
        FTE.Instance.Trigger(buddy, go, param);
    }
}