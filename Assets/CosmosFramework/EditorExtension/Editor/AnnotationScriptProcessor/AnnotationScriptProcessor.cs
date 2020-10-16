using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Cosmos;
using UnityEditor;
namespace Cosmos.CosmosEditor
{
    [InitializeOnLoad]
    public class AnnotationScriptProcessor : UnityEditor.AssetModificationProcessor
    {
        static string annotationStr =
@"//====================================
//* Author :#Author#
//* CreateTime :#CreateTime#
//* Version :
//* Description :
//==================================== " + "\n";
        static IAnnotationProvider provider;
        static string providerAnnoTitle;
        static AnnotationScriptProcessor()
        {
            provider = Utility.Assembly.GetInstanceByAttribute<ImplementProviderAttribute, IAnnotationProvider>();
            providerAnnoTitle = provider?.AnnotationTitle;
        }

        public static void OnWillCreateAsset(string name)
        {
            if (!EditorConst.EnableScriptTemplateAnnotation)
                return;
            if (provider != null)
            {
                string path = name.Replace(".meta", "");
                if (path.EndsWith(".cs"))
                {
                    providerAnnoTitle = providerAnnoTitle.Replace("#CreateTime#", System.DateTime.Now.ToString("yyy-MM-dd HH:ss"));
                    providerAnnoTitle = providerAnnoTitle.Replace("#Author#", EditorConst.AnnotationAuthor);
                    providerAnnoTitle += File.ReadAllText(path);
                    File.WriteAllText(path, providerAnnoTitle);
                }
            }
            else
            {
                string path = name.Replace(".meta", "");
                if (path.EndsWith(".cs"))
                {
                    annotationStr = annotationStr.Replace("#CreateTime#", System.DateTime.Now.ToString("yyy-MM-dd HH:ss"));
                    annotationStr = annotationStr.Replace("#Author#", EditorConst.AnnotationAuthor);
                    annotationStr += File.ReadAllText(path);
                    File.WriteAllText(path, annotationStr);
                }
            }
        }
    }
}
