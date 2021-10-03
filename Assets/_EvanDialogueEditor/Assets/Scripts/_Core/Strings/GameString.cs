using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETools.Utilities;
using System;

namespace ETools.Strings
{
	//	A string container meant to support Localization - only English is hooked up at the moment

	[CreateAssetMenu(fileName = "new String", menuName = "Gameplay/Game String", order = 1)]
	public class GameString : BObject
	{
		public int guid;

		[SerializeField]
		public LocalizedStringTable stringLUT = new LocalizedStringTable();

		public string EnglishString 
		{ 
			get 
			{ 
				return stringLUT[GameStringManager.Language.English.ToString()]; 
			}
			set
			{
				stringLUT[GameStringManager.Language.English.ToString()] = value;
			}
		}

		public string SpanishString
		{
			get
			{
				return stringLUT[GameStringManager.Language.Spanish.ToString()];
			}
			set
			{
				stringLUT[GameStringManager.Language.Spanish.ToString()] = value;
			}
		}
		public string FrenchString
		{
			get
			{
				return stringLUT[GameStringManager.Language.French.ToString()];
			}
			set
			{
				stringLUT[GameStringManager.Language.French.ToString()] = value;
			}
		}
		public string GermanString
		{
			get
			{
				return stringLUT[GameStringManager.Language.German.ToString()];
			}
			set
			{
				stringLUT[GameStringManager.Language.German.ToString()] = value;
			}
		}

		public int HashCode { get { return EnglishString.GetHashCode(); } }

		public static GameString CreateNew(string englishText)
		{
			GameString loc = GameString.CreateInstance<GameString>();
			loc.stringLUT[GameStringManager.Language.English.ToString()] = englishText;

			loc.PopulateDict();

			loc.guid = SystemsUtility.GetUniqueGUID(8);
			return loc;
		}

		public void PopulateDict()
		{
			foreach (string lang in GameStringManager.AllLanguageStrings)
			{
				if (!stringLUT.ContainsKey(lang))
					stringLUT[lang] = "";
			}
		}

		public override string ToString()
		{
			return guid + " - " + EnglishString;
		}
	}

	[System.Serializable]
	public class LocalizedStringTable : SerializableDictionaryBase<string, string> { }
}