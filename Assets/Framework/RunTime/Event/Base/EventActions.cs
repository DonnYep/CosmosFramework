using UnityEngine;
using System.Collections;
using System;
namespace Cosmos
{
    //1、使用out修饰符时，T只能作为返回值。此时泛型的实例是协变的。即该泛型的实例可赋值给T的所有父级类。
    // 2、使用in为修饰符时，T只能作为输入参数。此时泛型的实例是逆变的。即该泛型的实例可赋值给T的所有子类。
    //CF=CosmosFramework

    /// <summary>
    ///CosmosFrameworkAction
    /// </summary>
    public delegate void CFAction();
    /// <summary>
    /// 适用于带一个参数的回调函数或者普通委托
    /// CosmosFrameworkAction
    /// </summary>
    public delegate void CFAction<in T>(T arg);
    /// <summary>
    ///CosmosFrameworkAction
    /// 适用于带两个参数的回调函数或者普通委托
    /// </summary>
    public delegate void CFAction<in T1, in T2>(T1 arg1, T2 arg2);
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
    /// <summary>
    /// 判断委托，系统默认自带Predicate
    ///CosmosFramework
    /// </summary>
    public delegate bool CFPredicateAction<T>(T arg);
}
