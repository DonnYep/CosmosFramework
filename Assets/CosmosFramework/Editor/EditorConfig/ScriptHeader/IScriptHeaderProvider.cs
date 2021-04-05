using System.Collections;
using System.Collections.Generic;
namespace Cosmos.CosmosEditor
{

    /// <summary>
    /// 代码注释提供者接口；
    /// 已提供：
    /// 作者：#Author#  
    /// 创建时间：#CreateTime#
    /// </summary>
    public interface IScriptHeaderProvider
    {
        /* 参考，需要保留带 ##  符号的文字；
        @"//====================================
        //* Author :#Author#
        //* CreateTime :#CreateTime#
        //* Version :
        //* Description :
        //==================================== " + "\n";
        */
        string HeaderContext { get; }
    }
}