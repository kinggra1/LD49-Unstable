using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETools.Utilities;
using System.IO;
using System;
using System.Linq;

namespace ETools.Strings
{
	public static class GameStringManager 
	{
		public const string folderPath = "Strings";
		public const string dialogueCsvFile = "dialogue";

		private static bool _initialized = false;

		private static List<GameString> dialogueStrings = new List<GameString>();

		public enum LocalizedStringCategory { Dialogue }
		public enum Language { English, Spanish, French, German }

		public static string[] AllLanguageStrings { get { return Enum.GetNames(typeof(Language)); } }
		public static Language[] AllLanguages { get { return (Language[])Language.GetValues(typeof(Language)); } }

#if UNITY_EDITOR

		/// <summary>
		/// Creates a GameString asset in the project with a given name
		/// </summary>
		/// <param name="name">The name to assign to the new GameString asset</param>
		/// <returns></returns>
		public static GameString CreateGameStringInProject(string name)
		{
			GameString gameString = GameString.CreateNew("");

			//	This is a (mostly) duplicate of EditorUtils.CreateAssetAndFoldersInDatabase, but I can't link that project so...
			string subFolderPath = "Assets/Data/GameStrings/";
			string rootPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
			string folderPath = rootPath + subFolderPath;
			System.IO.Directory.CreateDirectory(folderPath);
			string filePath = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(subFolderPath + name + ".asset");
			UnityEditor.AssetDatabase.CreateAsset(gameString, filePath);
			return gameString;
		}

		/// <summary>
		/// The proper method of combining paths
		/// </summary>
		private static string PathCombine(params string[] paths)
		{
			return System.IO.Path.Combine((from path in paths select path.Replace("\\", "/").Trim('/')).ToArray());
		}

#endif
	}
}