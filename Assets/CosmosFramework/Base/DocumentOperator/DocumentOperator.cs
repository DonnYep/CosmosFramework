using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cosmos.IO
{
     public abstract class DocumentOperator<T>
        where T:new()
    {
        /// <summary>
        /// 解析文档
        /// </summary>
        public abstract void ParseDocument(TextAsset ta,CFAction<T> callBack=null);
        /// <summary>
        /// 创建空文档
        /// </summary>
        public abstract T CreateEmptyDocument(string fullPath,CFAction<T>callBack=null);
        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="callBack"></param>
        public abstract void DeleteDocument(string fullPath,CFAction<T> callBack=null);
        /// <summary>
        /// 编辑文档
        /// </summary>
        /// <typeparam name="T">文档类型</typeparam>
        /// <param name="doc">具体文档参数</param>
        /// <param name="callBack">可空回调函数</param>
        /// <returns></returns>
        public abstract T EditDocument(T doc ,CFAction<T>callBack=null);
        /// <summary>
        /// 存储文档
        /// </summary>
        public abstract void SaveDocument(T doc, string savePath,CFAction<T> callBack=null);
    }
}