using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class Test
{
    public int a;
    public int b;

    public void Print()
    {
        Debug.Log("Hello World!");
    }
}

public class BaseEditor : MonoBehaviour
{
    void Start()
    {
        Assembly info = typeof(System.Int32).Assembly;
        Debug.Log(info);
    }
}