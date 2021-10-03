using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Dialogue
{
	//	This class is the base class upon which all other Dialogue ScriptableObjects derive from
	public abstract class DialogueNode : BObject
	{
		public DialogueNode nextNode = null;
		public int guid;

		public override string ToString()
		{
			return name + " (" + guid + ")";
		}

	}
}
	