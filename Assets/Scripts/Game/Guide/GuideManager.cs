using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using GDM.Global;
using GDM.UI;


/// <summary>
/// The reason of using Interface is extensible. 
/// </summary>
public interface IGuideComponent 
{
    void OnGuideProcess();
    void OnGuideClear();
    string instanceName { get; set; }
}

/// <summary>
/// Guide Manager, known as Beginner's guide.
/// </summary>
public sealed class GuideManager : Singleton<GuideManager> 
{
    /// <summary>
    /// 新手引导的触发的类型.
    /// </summary>
    public enum GuideType
    {
        OnGameStart,
        OnTaskGet,
        OnFuncOpen,
    }

    private Dictionary<string, IGuideComponent> mDicMembers = new Dictionary<string, IGuideComponent>();
    
    /// <summary>
    /// 当前新手引导队列.
    /// </summary>
    private List<IGuideComponent> mGuideQueue = new List<IGuideComponent>();
   
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

    private XmlDocument xmlDoc = new XmlDocument();

    void Start()
    {
        //TODO:Parse config
    }

    public void Trigger(int id)
    {
        //TODO: Init mGuideQueue
        mGuideQueue = new List<IGuideComponent>();
        Startup(); 
    }

    public void Register(IGuideComponent instance)
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
    /// Pause the GuideManager.
    /// </summary>
    private void Pause()
    {
        if(!isPause)
        {
            isPause = true;
        }
    }

    /// <summary>
    /// Resume the GuideManager.
    /// </summary>
    private void Resume()
    {
        if(isPause)
        {
            isPause = false;
            mGuideQueue[mNextStep].OnGuideProcess();
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
            IGuideComponent component = mGuideQueue[mNextStep];
            if (component == null){
                Pause();
                return;
            }
            component.OnGuideProcess();
            mCurrenStep = mNextStep;
        }
        else
        {
            Finish();
        }
    }
}