using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
/// <summary>
/// 算法案例类
/// </summary>
public class AlgorithmTester : MonoBehaviour
{
    public uint target;
    public uint[] list;
    public string strTarget;
    public string[] strList;
    public void BinarySearch()
    {
        Utility.Algorithm.SortByAscending( list, (v) => v);
        var result = Utility.Algorithm.BinarySearch(list, target, (v) => v);
        Utility.Debug.LogInfo("BinarySearch result index>>" + result);
    }
    public void StrBinarySearch()
    {
        Utility.Algorithm.SortByAscending( strList, (v) => v);
        var result = Utility.Algorithm.BinarySearch(strList, strTarget, (v) => v);
        Utility.Debug.LogInfo("BinarySearch result index>>" + result);
    }
    public void Sort()
    {
        Utility.Algorithm.SortByAscending( list, (v) => v);
    }
    public void Max()
    {
        var result= Utility.Algorithm.Max(list, (v) => v);
        Utility.Debug.LogInfo("Max result index>>" + result);
    }
    /// <summary>
    /// 返回第一个符合条件的对象
    /// </summary>
    public void Find()
    {
        var result = Utility.Algorithm.Find(list, (v)=>v>target);
        Utility.Debug.LogInfo("Max result >>" + result);
    }
}
