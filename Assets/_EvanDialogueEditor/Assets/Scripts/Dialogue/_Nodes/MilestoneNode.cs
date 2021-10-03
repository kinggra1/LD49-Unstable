using ETools.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Dialogue
{
	public class MilestoneNode : DialogueNode
	{
		public Milestone milestone;
		public int newValue = 0;

		public override string ToString()
		{
			return "Milestone Node - " + milestone?.name + " -> " + newValue;
		}
	}
}