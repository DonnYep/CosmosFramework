using UnityEngine;
using Cosmos;
/// <summary>
/// 算法案例类
/// </summary>
public class AlgorithmTester : MonoBehaviour
{
    public int target;
    public int[] list;
    public string strTarget;
    public string[] strList;
    public int[] distinctList;
    public void BinarySearch()
    {
        Utility.Algorithm.SortByAscend(list, (v) => v, 0, list.Length - 1);
        var result = Utility.Algorithm.BinarySearch(list, target, (v) => v);
        Utility.Debug.LogInfo("BinarySearch result index>>" + result);
    }
    public void StrBinarySearch()
    {
        Utility.Algorithm.SortByAscend(strList, (v) => v);
        var result = Utility.Algorithm.BinarySearch(strList, strTarget, (v) => v);
        Utility.Debug.LogInfo("BinarySearch result index>>" + result);
    }
    public void Sort()
    {
        Utility.Algorithm.SortByDescend(list, (v) => v, 0, list.Length - 1);
    }
    public void Max()
    {
        var result = Utility.Algorithm.Max(list, (v) => v);
        Utility.Debug.LogInfo("Max result index>>" + result);
    }
    /// <summary>
    /// 返回第一个符合条件的对象
    /// </summary>
    public void Find()
    {
        var result = Utility.Algorithm.Find(list, (v) => v > target);
        Utility.Debug.LogInfo("Max result >>" + result);
    }
    public void Distinct()
    {
        distinctList = Utility.Algorithm.Distinct(list);
    }
}
