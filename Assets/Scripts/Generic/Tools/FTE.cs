using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GDM.Global;
using GDM.UI;


public interface IFTETrigger
{
    void OnInput();





}


/// <summary>
/// FTE(First Teching Experience) Manager, known as Beginner's guide.
/// </summary>
public sealed class FTE : Singleton<FTE> 
{
    private class FTEStep
    {
        public string target;
        public int style;
        public object param;
    }

    private class FTEInfo
    {
        public int stepIndex;
        public FTEStep[] steps; 
    }


    private Dictionary<int, FTEInfo> mDicFTE = new Dictionary<int, FTEInfo>();
    private FTEInfo mCurrentFTE;
    private bool isNeedMask;
    private int mFTEId;
    private GameObject uiGuide;

    private void OnGuideStart()
    {
        mCurrentFTE = mDicFTE[mFTEId];
        UIManager.instance.CreateUI("UIGuide", null, false);
    }


    private void OnGuideEnd()
    {
        StopAllCoroutines();
        UIManager.instance.DestroyUI("UIGuide");
    }


    /// <summary>
    /// Do the next step guide.
    /// </summary>
    private void NextStep()
    {
        if (mCurrentFTE.stepIndex < mCurrentFTE.steps.Length)
        {
            mCurrentFTE.stepIndex++; 
            return;
        }
        OnGuideEnd();
    }


    /// <summary>
    /// Do the previous step guide.
    /// </summary>
    private void PreviousStep()
    {
        if(mCurrentFTE.stepIndex > 0)
        {
            mCurrentFTE.stepIndex--;
        }
    }


    private IEnumerator Guide()
    {
        //TODO: 
        bool isVisible = UIManager.instance.IsVisible("");
        while(!isVisible)
        {
             yield return new WaitForSeconds(0.1f);
        }
        Display();
    }


    /// <summary>
    /// Display current guide for various of style.
    /// </summary>
    private void Display()
    {

    }
}