using System;
using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Restricts an Object reference field to accept only scene objects (no project assets).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class SceneObjectOnlyAttribute : PropertyAttribute { }
}
