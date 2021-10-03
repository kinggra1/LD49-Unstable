using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Dialogue
{
	//	Inherit from this script to write behaviors that can be called during dialogue Conversations

	public interface IDialogueCustomScript
	{
		void DoAction();
	}
}