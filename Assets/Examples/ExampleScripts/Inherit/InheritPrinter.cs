using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using System;
using System.Reflection;
public class InheritPrinter : MonoBehaviour
{
    /// <summary>
    /// 优雅的继承反射写法，规避掉抽象类与非类对象
    /// </summary>
    void Start()
    {
        var personType = typeof(Person);
        Type[] types = Assembly.GetAssembly(typeof(Person)).GetTypes();
        for (int i = 0; i < types.Length; i++)
        {
            if(personType.IsAssignableFrom(types[i]))
            {
                if (types[i].IsClass && !types[i].IsAbstract)
                {
                    var result= Utility.Assembly.GetTypeInstance(types[i]) as Person;
                    Utility.DebugLog(result.GetType().FullName);
                }
            }
        }
    }
}
