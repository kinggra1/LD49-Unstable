using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Dialogue
{
	//	Represents a Speaker, or participant in a dialogue conversation

	[CreateAssetMenu(fileName = "New Speaker", menuName = "Gameplay/Dialogue/Speaker", order = 21)]
	public class Speaker : BObject
	{
		#region Variables

		//	Public
		public string characterName;
		public Texture characterPortrait;
		public AudioClip[] blips;

	    #endregion
	
	}
}