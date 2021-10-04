using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools.Editor.Dialogue
{
	/// <summary>
	/// This is the base class that all "Connectable" objects in the Conversation node graph derive from
	/// </summary>
	[System.Serializable]
	public class Connectable : ScriptableObject
	{
		public Node outgoing;

		public Rect nodeRect;
		public bool isOutput;
		public bool connectionMarkedForDelete = false;

		public Rect InputRect
		{
			get
			{
				Rect inputRect = new Rect(GraphSpaceRect);
				inputRect.height *= 0.65f;
				inputRect.width = inputRect.height;
				inputRect.x -= inputRect.width * 0.5f;
				inputRect.y += inputRect.height * 0.25f;

				return inputRect;
			}
		}

		public Rect OutputRect
		{
			get
			{
				Rect outputRect = new Rect(GraphSpaceRect);
				outputRect.height *= .65f;
				outputRect.width = outputRect.height;
				outputRect.x += GraphSpaceRect.width - outputRect.width * 0.5f;
				outputRect.y += outputRect.height * 0.25f;

				return outputRect;
			}
		}

		public Rect GraphSpaceRect
		{
			get
			{
				return new Rect(
				(nodeRect.x + GUIController.lookPos.x) * GUIController.zoom,
				(nodeRect.y + GUIController.lookPos.y) * GUIController.zoom,
				nodeRect.width * GUIController.zoom,
				nodeRect.height * GUIController.zoom);
			}
		}

		public Vector2 OutputPosition
		{
			get
			{
				return OutputRect.center;
			}
		}

		public Vector2 InputPosition
		{
			get
			{
				return InputRect.center;
			}
		}


	}
}