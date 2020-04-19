this floder is CosmosFramework core.
CosmosFramework depend on LitJson.dll,don't delete LitJson .
LitJson path:Assets\CosmosFramework\Plugins\LitJson.dll.

"FileExtensionList.xml" in Assets\CosmosFramework\Resources\FileExtensionList.xml. don't change it.
here is a  file that is used when IO Sth.

开发阶段可打包成DLL文件放入正在开发项目的Plugins目录下。若需要打包发布，则复制CosmosFramework整个文件夹到Assets根目录下即可。Dll状态下打包会造成失败。