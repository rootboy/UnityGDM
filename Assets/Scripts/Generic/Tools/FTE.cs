using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GDM.Global;
using GDM.UI;


/// <summary>
/// The reason of using Interface is extensible. 
/// </summary>
public interface IFTEBuddy 
{
    void OnFTETrigger(IFTEBuddy buddy, GameObject go,  string param);
}

/// <summary>
/// FTE(First Teching Experience) Manager, known as Beginner's guide.
/// Two Keys of FTE Design:
/// 1.FTE appearing time.
/// 2.FTE desired action.
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

    private Dictionary<int, FTEInfo> mDicFTE = new Dictionary<int, FTEInfo>();
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

    /// <summary>
    /// Trigger the FTE by specific action to increase the FTEStep.
    /// </summary>
    public void Trigger(IFTEBuddy buddy, GameObject go, string param, string action)
    {
        if(go.name == mCurrentStep.targetName && param == mCurrentStep.targetParam
            && action == mCurrentStep.targetAction)
        {
            NextStep();
        }           
    }
    
    /// <summary>
    /// Trigger tht FTE to startup.
    /// </summary>
    public void Trigger(FTEType type)
    {
        OnGuideStart();
    }

    /// <summary>
    /// This function is called when FTE event is triggered.
    /// </summary>
    private void OnGuideStart()
    {
        isRunning = true;
        mCurrentFTE = mDicFTE[mFTEId];
        mCurrentStep = mCurrentFTE.steps[mCurrentFTE.stepIndex];
    }

    /// <summary>
    /// This funciton is called when FTE ends. 
    /// </summary>
    private void OnGuideEnd()
    {
        isRunning = false;
        StopAllCoroutines();
    }

    /// <summary>
    /// Do the next step guide.
    /// </summary>
    private void NextStep()
    {
        if (++mCurrentFTE.stepIndex >= mCurrentFTE.steps.Length)
        {
            OnGuideEnd();
            return;
        }
        mCurrentStep = mCurrentFTE.steps[mCurrentFTE.stepIndex];
        StartCoroutine("WaitForTarget");
    }

    /// <summary>
    /// Do the previous step guide.
    /// </summary>
    private void PreviousStep()
    {
        if (--mCurrentFTE.stepIndex < 0)
            return;

        mCurrentStep = mCurrentFTE.steps[mCurrentFTE.stepIndex];
    }

    /// <summary>
    /// 
    /// </summary>
    private void Hide()
    {
    }


    private void Display()
    {
        int style = mCurrentStep.style;
        switch(style)
        {
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }
    }
}