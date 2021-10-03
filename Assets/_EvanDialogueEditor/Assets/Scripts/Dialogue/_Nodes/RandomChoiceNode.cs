using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Dialogue
{
	public class RandomChoiceNode : DialogueNode
	{
		public int[] randomWeighting;
		public DialogueNode[] nextNodes;

		public override string ToString()
		{
			string sum = "Random Choice Node - ";
			foreach (int randomWeight in randomWeighting)
			{
				sum += randomWeight.ToString() + ", ";
			}
			return sum.TrimEnd().TrimEnd(',');
		}

        internal int GetChoiceIndex() {
            throw new NotImplementedException();
        }
    }
}