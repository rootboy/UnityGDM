using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using GDM.Global;
using GDM.UI;


public interface IFTEComponent 
{
    void OnFTEProcess();
    void OnFTEClear();
    string instanceName { get; set; }
}


/// <summary>
/// Guide Manager, known as Beginner's guide.
/// </summary>
public sealed class FTE : Singleton<FTE> 
{
    /// <summary>
    /// 新手引导的触发类型.
    /// </summary>
    public enum FTEType
    {
        OnGameStart,
        OnTaskGet,
        OnFuncOpen,
    }

    /// <summary>
    /// IFTEComponent注册表.
    /// </summary>
    private Dictionary<string, IFTEComponent> mDicMembers = new Dictionary<string, IFTEComponent>();
    
    /// <summary>
    /// 当前新手引导队列.
    /// </summary>
    private List<IFTEComponent> mGuideQueue = new List<IFTEComponent>();
   
    /// <summary>
    /// 当前引导步.
    /// </summary>
    private int mCurrenStep = -1;
    
    /// <summary>
    /// 下一引导步.
    /// </summary>
    private int mNextStep;
    
    /// <summary>
    /// 新手引导是否暂停?
    /// </summary>
    private bool isPause = false;

    /// <summary>
    /// 新手引导是否启动?
    /// </summary>
    private bool isStart = false;


    void Start()
    {
        //TODO: Parse config file.
    }

    public void Trigger(int id)
    {
        //TODO: init mGuideQueue
        mGuideQueue = new List<IFTEComponent>();
        Startup(); 
    }

    public void Register(IFTEComponent instance)
    {
        if(instance != null)
        {
            string name = instance.instanceName;
            if (mDicMembers.ContainsKey(name))
            {
                return;
            }

            mDicMembers[name] = instance;
            if (isStart)
            {

            }
        }
    }

    /// <summary>
    /// Startup the guide.
    /// </summary>
    private void Startup()
    {
        if(!isStart)
        {
            isStart = true;
            NextStep();
        }
    }

    /// <summary>
    /// Pause the FTE.
    /// </summary>
    private void Pause()
    {
        if(!isPause)
        {
            isPause = true;
        }
    }

    /// <summary>
    /// Resume the FTE.
    /// </summary>
    private void Resume()
    {
        if(isPause)
        {
            isPause = false;
            mGuideQueue[mNextStep].OnFTEProcess();
            mCurrenStep = mNextStep;
        }
    }

    private void Finish()
    {
        if (isStart)
        {
            isStart = false;
            mCurrenStep = -1;
            mGuideQueue.Clear();
        }
    }


    public void NextStep(int designedStep = -1)
    {
        if(isPause){
            Resume();
            return;
        }

        if (designedStep < 0) mNextStep = mCurrenStep + 1;
        else mNextStep = designedStep;

        if(mNextStep < mGuideQueue.Count)
        {
            IFTEComponent component = mGuideQueue[mNextStep];
            if (component == null){
                Pause();
                return;
            }
            component.OnFTEProcess();
            mCurrenStep = mNextStep;
        }
        else
        {
            Finish();
        }
    }
}