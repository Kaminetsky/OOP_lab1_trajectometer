using UnityEngine;
using UnityEditor;
using System.IO;


// To enable the integration with GIT Extensions, add VPP_GIT_EXTENSIONS_INTEGRATION 
// to Project Settings > Player > Other Settings > Scripting Define Symbols.


#if VPP_GIT_EXTENSIONS_INTEGRATION


public class GitExtensionsIntegration : MonoBehaviour
	{
	private static string pathToGitExtensions = "c:\\Program Files (x86)\\Coding\\GitExtensions\\";
	
	
	private static string GetAssetSystemPath (Object assetObject)
		{
		string sAssetFolderPath = AssetDatabase.GetAssetPath(assetObject);
		string sDataPath  = Application.dataPath;
		
		if (sAssetFolderPath != "")
			return sDataPath.Substring(0, sDataPath.Length-6) + sAssetFolderPath;
		else
			return sDataPath.Substring(0, sDataPath.Length-7);
		}
		
		
	private static bool IsGitRepository (Object assetObject)
		{
		string sRepositoryPath = GetAssetSystemPath(assetObject) + "/.git";		
		// Submodules may contain a .git file instead of .git folder
		return File.Exists(sRepositoryPath) || Directory.Exists(sRepositoryPath);
		}
		
		
	private static void RunGitExtensions (string arguments)
		{
		try
			{
			System.Diagnostics.Process gitExt = new System.Diagnostics.Process();
			gitExt.StartInfo.FileName = pathToGitExtensions + "GitExtensions.exe";
			gitExt.StartInfo.Arguments = arguments;
			gitExt.Start();
			}
		catch (System.Exception e)
			{
			Debug.LogError(e);
			}
		}
		
		
	// Git Extensions Browse
	
	
	[MenuItem ("Assets/Git Extensions")]
	static void GitExtensions ()
		{
		RunGitExtensions("browse \"" + GetAssetSystemPath(Selection.activeObject) + "\"");
		}
		
	[MenuItem ("Assets/Git Extensions", true)]
	static bool ValidateGitExtensions () 
		{
		return IsGitRepository(Selection.activeObject);
		}
		
		
	// GitExt Commit
		
		
	[MenuItem ("Assets/GitExt Commit")]
	static void GitExtensionsCommit ()
		{
		RunGitExtensions("commit \"" + GetAssetSystemPath(Selection.activeObject) + "\"");
		}
		
	[MenuItem ("Assets/GitExt Commit", true)]
	static bool ValidateGitExtensionsCommit () 
		{
		return IsGitRepository(Selection.activeObject);
		}
		
		
	// GitExt File History
	
	
	[MenuItem ("Assets/GitExt File History")]
	static void GitExtensionsFileHistory ()
		{
		RunGitExtensions("filehistory \"" + GetAssetSystemPath(Selection.activeObject) + "\"");
		}
		
	[MenuItem ("Assets/GitExt File History", true)]
	static bool ValidateGitExtensionsFileHistory ()
		{
		return !Directory.Exists(GetAssetSystemPath(Selection.activeObject));
		}
		
	}

#endif