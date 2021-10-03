using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Conditionals
{
	[CreateAssetMenu(fileName = "New Conditional", menuName = "Gameplay/Conditionals", order = 11)]
	public class Conditional : BObject, IConditional
	{
		public virtual bool Evaluate()
		{
			Debug.LogError("Don't use plain conditionals, they don't do anything!");
			return false;
		}

		public virtual string ToReadabilityString(bool richText = true) { return ""; }

	}
}

