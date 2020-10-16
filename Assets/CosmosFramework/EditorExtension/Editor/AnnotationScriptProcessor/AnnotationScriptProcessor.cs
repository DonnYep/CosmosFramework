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
                    var str = File.ReadAllText(path);
                    if (!str.Contains(providerAnnoTitle))
                    {
                        str = providerAnnoTitle + str;
                    }
                    str = str.Replace("#CreateTime#", System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss"));
                    str = str.Replace("#Author#", EditorConst.AnnotationAuthor);
                    File.WriteAllText(path, str);
                }
            }
            else
            {
                string path = name.Replace(".meta", "");
                if (path.EndsWith(".cs"))
                {
                    var str = File.ReadAllText(path);
                    if (!str.Contains(annotationStr))
                    {
                        str = annotationStr + str;
                    }
                    str = str.Replace("#CreateTime#", System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss"));
                    str = str.Replace("#Author#", EditorConst.AnnotationAuthor);
                    File.WriteAllText(path, str);
                }
            }
        }
    }
}
