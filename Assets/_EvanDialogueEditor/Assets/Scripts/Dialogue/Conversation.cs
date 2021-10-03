using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Dialogue
{
	//	The overarching object that holds all data for a dialogue conversation

	public class Conversation : BObject
	{
		public enum ConversationType { Standard, Simple, Diagetic }
		public ConversationType conversationType;

		public DialogueNode firstNode;
		public List<Speaker> speakers = new List<Speaker>();
		public List<DialogueNode> allNodes = new List<DialogueNode>();

#if UNITY_EDITOR

		/// <summary>
		/// Destroys all children assets of this conversation in the Unity project.
		/// </summary>
		public void DestroyAllChildrenInProject()
		{
			foreach(DialogueNode node in allNodes)
			{
				if(node is ChoiceNode)
				{
					foreach (Choice choice in (node as ChoiceNode).choices)
						DestroyImmediate(choice, true);
				}
				Object.DestroyImmediate(node, true);
			}
		}

#endif
	}
}