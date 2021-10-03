using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace ETools.Serialization
{
	//	A simple integer that can be created in the project, serialized in data, and referenced/changed in gameplay.
	//	Intended purpose is to quickly save and reference player state data to be saved and loaded.

	[CreateAssetMenu(fileName = "New Milestone", menuName = "Gameplay/Milestone", order = 31)]
	[Serializable]
	public class Milestone : BObject
	{
		#region Variables

		[SerializeField]
		private string _name;

		[SerializeField]
		private int _value;
		public string description;

		#endregion

		#region Properties

		public string Name { get { return _name; } }
		public int Value
		{
			get
			{
				return _value;
			}
			internal set
			{
				_value = value;
			}
		}

		#endregion

		#region Constructors

		public static Milestone New()
		{
			return Milestone.CreateInstance<Milestone>();
		}

		#endregion

		#region Public Methods

		public void SetValue(int value)
		{
			Value = value;
			Apply();
		}

		public void Apply()
		{
			//	Interface with the serialization system here

			//SaveManager.Instance.SetMilestoneValue(this, _value);
		}

		public override string ToString()
		{
			return Name + ":" + Value;
		}

		#endregion
	}
}