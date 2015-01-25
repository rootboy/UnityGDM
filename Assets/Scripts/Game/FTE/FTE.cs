using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GDM.Global;
using GDM.UI;


/// <summary>
/// The reason of using Interface is extensible. 
/// </summary>
public interface IFTEComponent 
{
    void OnFTEProcess();
    void OnFTEClear();
    string instanceName { get; set; }
}

/// <summary>
/// FTE(First Teching Experience) Manager, known as Beginner's guide.
/// </summary>
public sealed class FTE : Singleton<FTE> 
{
    public enum FTEType
    {
        OnGameStart,
        OnTaskGet,
        OnTaskFinish,
    }

    private class FTEStep
    {
        public int style;
        public string targetName;
        public string targetParam;
        public string targetAction;
    }

    private class FTEInfo
    {
        public int stepIndex;
        public FTEStep[] steps; 
    }

    private Dictionary<string, IFTEComponent> mDicMembers = new Dictionary<string, IFTEComponent>();

    private int mFTEId;
    private FTEInfo mCurrentFTE;
    private FTEStep mCurrentStep;
    private bool isNeedMask;
    private bool isRunning;
    private GameObject uiGuide;

    void Awake()
    {
        //TODO: parse config and initialize mDicFTE
    }

    public void Register(IFTEComponent component)
    {
        string name = component.instanceName;

    }

    private void Startup()
    {

    }

    private void Pause()
    {

    }

    private void Resume()
    {

    }

    private void Finish()
    {

    }
}