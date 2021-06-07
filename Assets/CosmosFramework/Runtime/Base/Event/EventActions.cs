using UnityEngine;
using System.Collections;
using System;
namespace Cosmos
{
    //1、使用out修饰符时，T只能作为返回值。此时泛型的实例是逆变的。意味着泛型类型参数可以从一个基类改为该类的派生类(父类可以转为子类)；
    // 2、使用in为修饰符时，T只能作为输入参数。此时泛型的实例是协变的。意味着泛型类型参数可以从一个派生类更改为它的基类(子类可以转为父类)；
    public delegate IEnumerator CoroutineHandler();
    public delegate IEnumerator CoroutineHandler<in T>(T arg);
    public delegate IEnumerator CoroutineHandler<in T1,in T2>(T1 arg1,T2 arg2);
}
