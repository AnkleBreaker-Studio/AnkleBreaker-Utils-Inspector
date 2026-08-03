using UnityEditor;
#if UNITY_6000_4_OR_NEWER
using UnityEngine;
#endif

namespace AnkleBreaker.Utils.Inspector.Editor
{
    /// <summary>
    /// The editor half of the object-identity fork — see <see cref="AB_ObjectCompat"/> for the runtime
    /// one and the full reasoning.
    /// </summary>
    /// <remarks>
    /// Same shape, same boundary, different API: <c>SerializedProperty.objectReferenceInstanceIDValue</c>
    /// works through 6000.3 and is <c>CS0619</c> in 6000.5, while its replacement
    /// <c>objectReferenceEntityIdValue</c> does not exist before 6000.4.
    /// </remarks>
    public static class AB_EditorObjectCompat
    {
        /// <summary>
        /// True when <paramref name="property"/> still names an object that no longer resolves — a
        /// reference kept whose asset is gone, which is what tells a missing slot from an empty one.
        /// </summary>
        /// <remarks>Only meaningful once <c>objectReferenceValue</c> has been found null: an id survives
        /// where the object does not, and that surviving id IS the distinction.</remarks>
        public static bool HoldsALostReference(SerializedProperty property)
        {
            if (property == null) return false;

#if UNITY_6000_4_OR_NEWER
            return property.objectReferenceEntityIdValue != EntityId.None;
#else
            return property.objectReferenceInstanceIDValue != 0;
#endif
        }
    }
}
