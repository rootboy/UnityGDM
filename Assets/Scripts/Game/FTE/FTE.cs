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
    private List<IFTEComponent> mFTEQueue = new List<IFTEComponent>();
    private int mCurrenStep = -1;
    private int mNextStep;
    private bool isPause = false;
    private bool isStart = false;

 
    void Awake()
    {
        //TODO: parse config and initialize mDicFTE
    }

    public void Register(IFTEComponent instance)
    {
        string name = instance.instanceName;
        if(mDicMembers.ContainsKey(name))
        {
            return;
        }

        mDicMembers[name] = instance;
        if(isStart)
        {

        
        }


    }

    private void Startup()
    {
        if(!isStart)
        {
            isStart = true;
        }
    }

    private void Finish()
    {
        if(isStart)
        {
            isStart = false;
            mCurrenStep = -1;
            mFTEQueue.Clear();
        }
    }

    private void Pause()
    {
        if(!isPause)
        {
            isPause = true;
        }
    }

    private void Resume()
    {
        if(isPause)
        {
            isPause = false;
            mFTEQueue[mNextStep].OnFTEProcess();
            mCurrenStep = mNextStep;
        }
    }

    public void NextStep()
    {
        if(mNextStep < mFTEQueue.Count && mFTEQueue[mNextStep] != null)
        {
            mFTEQueue[mNextStep].OnFTEProcess();
            mCurrenStep = mNextStep;
        }
        else
        {
            if(mNextStep == mFTEQueue.Count)
            {
                Finish();
            }
        }
    }
}