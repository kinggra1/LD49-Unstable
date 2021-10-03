using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Reflection;

namespace ETools.Utilities
{
	public static class SystemsUtility
	{
		/// <summary>
		/// Creates a new IEnumerable of type T with all empty entries removed
		/// </summary>
		/// <typeparam name="T">Any type</typeparam>
		/// <param name="collection">The collection to be pruned for empty entries</param>
		/// <returns>A new version of the collection with empty entries removed</returns>
		public static IEnumerable<T> RemoveEmptyEntriesFromCollection<T>(this IEnumerable<T> collection)
		{
			return (from item in collection where item != null select item);
		}

		/// <summary>
		/// Gets a Type given its full-name
		/// </summary>
		/// <param name="fullName">The full-name of the type</param>
		/// <returns>The type that matches the name, or null if none matched</returns>
		public static Type TypeFromString(string fullName)
		{
			Type t = Type.GetType(fullName);
			if (t == null)
			{
				foreach (Assembly A in AppDomain.CurrentDomain.GetAssemblies())
				{
					var types = from typ in A.GetTypes() where typ.FullName == fullName select typ;
					if (types.Count() >= 1)
						t = types.First();
				}
			}
			return t;
		}

		/// <summary>
		/// Generates a unique GUID given an existing list of GUIDs
		/// </summary>
		/// <param name="digits">How many digits the GUID should be</param>
		/// <param name="existingGUIDs">The list of existing GUIDs</param>
		/// <returns>A unique GUID</returns>
		public static int GetUniqueGUID(int digits = 6, IEnumerable<int> existingGUIDs = null)
		{
			int guid = UnityEngine.Random.Range(0, (int)Mathf.Pow(10, digits));
			if (existingGUIDs != null)
			{
				if (existingGUIDs.Contains(guid))
					return GetUniqueGUID(digits, existingGUIDs);
			}
			return guid;
		}

		/// <summary>
		/// Copies all serializable fields and properties from one Component onto another
		/// </summary>
		/// <typeparam name="T">The component type to copy</typeparam>
		/// <param name="comp">The component we want to copy the data onto</param>
		/// <param name="other">The component we want to copy the data from</param>
		public static T GetCopyFrom<T>(this Component comp, T other) where T : Component
		{
			Type type = comp.GetType();
			if (type != other.GetType()) return null; // type mis-match
			BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default ;
			PropertyInfo[] pinfos = type.GetProperties(flags);
			foreach (var pinfo in pinfos)
			{
				if (pinfo.CanWrite && pinfo.Name != "name")
				{
					try
					{
						pinfo.SetValue(comp, pinfo.GetValue(other, null), null);
					}
					catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
				}
			}
			FieldInfo[] finfos = type.GetFields(flags);
			foreach (var finfo in finfos)
			{
				finfo.SetValue(comp, finfo.GetValue(other));
			}
			return comp as T;
		}

		/// <summary>
		/// Selects a random element in a collection
		/// </summary>
		public static T SelectRandom<T>(this IEnumerable<T> coll)
		{
			return coll.ElementAt(UnityEngine.Random.Range(0, coll.Count()));
		}

		/// <summary>
		/// Gets components in a sibling GameObject (other children with the same immediate parent)
		/// </summary>
		/// <typeparam name="T">The component to get</typeparam>
		/// <param name="obj">The GameObject whose siblings we want to check</param>
		/// <returns>The first instance of the component found in sibling GameObjects</returns>
		public static T GetComponentInSiblings<T>(this GameObject obj)
		{
			return obj.transform.parent.GetComponentInChildren<T>();
		}

		/// <summary>
		/// Gets components in a sibling Transform (other children with the same immediate parent)
		/// </summary>
		/// <typeparam name="T">The component to get</typeparam>
		/// <param name="obj">The Transform whose siblings we want to check</param>
		/// <returns>The first instance of the component found in sibling GameObjects</returns>
		public static T GetComponentInSiblings<T>(this Transform obj)
		{
			return obj.parent.GetComponentInChildren<T>();
		}

		/// <summary>
		/// Gets components in a sibling GameObject (other children with the same immediate parent)
		/// </summary>
		/// <typeparam name="T">The component to get</typeparam>
		/// <param name="obj">The component whose GameObject's siblings we want to check</param>
		/// <returns>The first instance of the component found in sibling GameObjects</returns>
		public static T GetComponentInSiblings<T>(this Component comp)
		{
			return comp.transform.parent.GetComponentInChildren<T>();
		}
	}

}
