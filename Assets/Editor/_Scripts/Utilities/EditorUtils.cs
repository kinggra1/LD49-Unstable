using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ETools.Editor
{
	public static class EditorUtils
	{
		/// <summary>
		/// Like AssetDatabase.CreateAsset, but also creates the folders necessary for the path
		/// </summary>
		/// <param name="asset">The asset to create in the database</param>
		/// <param name="path">The desired path to the asset</param>
		public static void CreateAssetAndFoldersInDatabase(Object asset, string path)
		{
			string rootPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
			string fullPath = PathCombine(rootPath, path).Replace("\\", "/");
			string folderPath = fullPath.Substring(0, fullPath.LastIndexOf('/'));
			System.IO.Directory.CreateDirectory(folderPath);
			AssetDatabase.CreateAsset(asset, path);
		}

		/// <summary>
		/// Creates an empty object in the Editor viewport to mimic Unity's default editor object-creation behavior
		/// </summary>
		/// <param name="name">The name to be assigned to the object</param>
		/// <param name="autoSelectCreated">Should we select the new object upon creation?</param>
		/// <returns>The created GameObject</returns>
		public static GameObject CreateEmptyInEditorViewport(string name = "New GameObject", GameObject parent = null, bool autoSelectCreated = true)
		{
			GameObject newObj = new GameObject(name);

			if (parent != null)
			{
				GameObjectUtility.SetParentAndAlign(newObj, parent);
			}
			else
			{
				var view = UnityEditor.SceneView.lastActiveSceneView;
				newObj.transform.position = view.camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 10f));
			}

			Undo.RegisterCreatedObjectUndo(newObj, "Create " + newObj.name);
			if (autoSelectCreated)
				Selection.activeObject = newObj;
			return newObj;

		}

		/// <summary>
		/// Creates an empty object in the Editor viewport to mimic Unity's default editor object-creation behavior and assigns a component T to it
		/// </summary>
		/// <typeparam name="T">The Component to assign to the object</typeparam>
		/// <param name="name">The name to be assigned to the object</param>
		/// <param name="autoSelectCreated">Should we select the new object upon creation?</param>
		/// <returns>The Component (T) attached to the created GameObjec/returns>
		public static T CreateInEditorViewport<T>(string name = "New GameObject", GameObject parent = null, bool autoSelectCreated = true) where T : Component
		{
			GameObject newObj = CreateEmptyInEditorViewport(name, parent, autoSelectCreated);
			return newObj.AddComponent<T>();
		}

		/// <summary>
		/// Gets the folder a file is in given the filename
		/// </summary>
		/// <param name="filePath">The path for the file</param>
		/// <returns>The folder that file is in</returns>
		public static string GetFolder(this string filePath)
		{
			return filePath.Substring(0, filePath.Replace('/', '\\').LastIndexOf('\\'));
		}

		/// <summary>
		/// Returns the path minus the "Assets" keyword
		/// </summary>
		public static string PathMinusAssets(this string filePath)
		{
			return filePath.Replace('/', '\\').Replace("\\Assets", "").Replace("Assets\\", "\\");
		}

		/// <summary>
		/// The proper method of combining paths
		/// </summary>
		public static string PathCombine(params string[] paths)
		{
			return System.IO.Path.Combine((from path in paths select path.Replace("\\", "/").Trim('/')).ToArray());
		}
	}
}