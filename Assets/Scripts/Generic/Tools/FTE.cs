using UnityEngine;
using System.Collections;
using GDM.Global;


/// <summary>
/// FTE(First Teching Experience) Manager, known as Beginner's guide.
/// </summary>
public sealed class FTE : Singleton<FTE> 
{
    private int mGuideId;
    private int mStep;

    public void OnGuideStart()
    {
        //TODO: Initialize
    }

    public void OnGuideEnd()
    {
        //TODO: Uninitialize
    }

    /// <summary>
    /// Next step for this guide.
    /// </summary>
    public void NextStep()
    {
        //TODO: display next guide step
    }

    /// <summary>
    /// Previous step for this guide.
    /// </summary>
    public void PreviousStep()
    {
        //TODO: display previous guide step
    }

    /// <summary>
    /// Display current guide for various of style.
    /// </summary>
    private void Display()
    {

    }
}