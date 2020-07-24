using UnityEngine;
using System.Collections;
using System;
namespace Cosmos
{
    //1、使用out修饰符时，T只能作为返回值。此时泛型的实例是逆变的。意味着泛型类型参数可以从一个基类改为该类的派生类(父类可以转为子类)；
    // 2、使用in为修饰符时，T只能作为输入参数。此时泛型的实例是协变的。意味着泛型类型参数可以从一个派生类更改为它的基类(子类可以转为父类)；
    //CF=CosmosFramework
    /// <summary>
    /// cosmos function 只出现在返回类型
    ///CosmosFramework
    /// </summary>
    public delegate Result CFResultAction<out Result>();
    /// <summary>
    /// out是返回值类型,输入T，则返回Result类型
    /// CosmosFramework
    /// </summary>
    public delegate Result CFResultAction<in T, out Result>(T arg);



    public delegate Result CFFunction<in T, out Result>(T arg);
    /// <summary>
    /// 判断委托，系统默认自带Predicate
    ///CosmosFramework
    /// </summary>
    public delegate bool CFPredicateAction< T>(T arg);
    public delegate bool CFPredicateAction();

    public delegate IEnumerator CoroutineHandler();
    public delegate IEnumerator CoroutineHandler<in T>(T arg);
    public delegate IEnumerator CoroutineHandler<in T1,in T2>(T1 arg1,T2 arg2);
}
