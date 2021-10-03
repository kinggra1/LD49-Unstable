using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETools
{
    //  An abstraction layer overtop base Scriptable Objects, for the "New" function and other TBD functionality
    [System.Serializable]
    public class BObject : ScriptableObject
    {
        public static T New<T>() where T : BObject
        {
            return CreateInstance<T>();
        }
        //  I get the feeling I'll want this later
    }
}