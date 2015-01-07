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
/// </summary>
public sealed class FTE : Singleton<FTE> 
{
    private class FTEStep
    {
        public string content;
        public string targetName;
        public string param;
    }

    private class FTEInfo
    {
        public int stepIndex;
        public FTEStep[] steps; 
    }

    private Dictionary<int, FTEInfo> mDicFTE = new Dictionary<int, FTEInfo>();
    private FTEInfo mCurrentFTE;
    private FTEStep mCurrentStep;
    private bool isNeedMask;
    private bool isRunning;
    private int mFTEId;
    private GameObject uiGuide;

    void Awake()
    {
        //TODO: parse config and initialize mDicFTE
    }

    /// <summary>
    /// The only interface exposed, called by FTE buddy or self.
    /// </summary>
    public void Trigger(IFTEBuddy buddy, GameObject go, string param)
    {
        if (isRunning)
        {
            if(go == null && param == null)
            {
                Display();
            }
            else if(go.name == mCurrentStep.targetName && param == mCurrentStep.param)
            {
                NextStep();
            }           
        }
    }

    /// <summary>
    /// This function is called when some event is triggered.
    /// </summary>
    private void OnGuideStart()
    {
        isRunning = true;
        mCurrentFTE = mDicFTE[mFTEId];
        mCurrentStep = mCurrentFTE.steps[mCurrentFTE.stepIndex];
    }

    /// <summary>
    /// 
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
    /// Display the ui guide.
    /// </summary>
    private void Display()
    {
        UIManager.instance.Show(mCurrentStep.content, mCurrentStep.param);
    }

    /// <summary>
    /// 
    /// </summary>
    private void Hide()
    {
        UIManager.instance.DestroyUI(mCurrentStep.content);
    }

    /// <summary>
    /// Guarantee the target is stable.
    /// </summary>
    private IEnumerator WaitForTarget()
    {
        while (!UIManager.instance.IsVisible(mCurrentStep.targetName) || 
            !UIManager.instance.IsStatic(mCurrentStep.targetName))
        {
            yield return new WaitForSeconds(0.2f);
        }
        Trigger(null, null, null);
    }
}