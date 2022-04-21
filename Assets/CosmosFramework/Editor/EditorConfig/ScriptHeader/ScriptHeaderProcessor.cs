using System.IO;
using UnityEditor;
using System.Text;
namespace Cosmos.Editor.Config
{
    [InitializeOnLoad]
    public class ScriptHeaderProcessor : UnityEditor.AssetModificationProcessor
    {
        static string headerStr =
@"//====================================
//* Author :#Author#
//* CreateTime :#CreateTime#
//* Version :
//* Description :
//==================================== " + "\n";
        static IScriptHeaderProvider provider;
        static string providerAnnoTitle;
        static ScriptHeaderProcessor()
        {
            provider = Utility.Assembly.GetInstanceByAttribute<ImplementerAttribute, IScriptHeaderProvider>();
            providerAnnoTitle = provider?.HeaderContext;
        }

        public static void OnWillCreateAsset(string name)
        {
            if (!EditorConfigWindow.EditorConfigData.EnableScriptHeader)
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
                    str = str.Replace("#Author#", EditorConfigWindow.EditorConfigData.HeaderAuthor);
                    File.WriteAllText(path, str, Encoding.UTF8);
                }
            }
            else
            {
                string path = name.Replace(".meta", "");
                if (path.EndsWith(".cs"))
                {
                    var str = File.ReadAllText(path, Encoding.UTF8);
                    if (!str.Contains(headerStr))
                    {
                        str = headerStr + str;
                    }
                    str = str.Replace("#CreateTime#", System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss"));
                    str = str.Replace("#Author#", EditorConfigWindow.EditorConfigData.HeaderAuthor);
                    File.WriteAllText(path, str, Encoding.UTF8);
                }
            }
        }
    }
}
