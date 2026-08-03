using Object = UnityEngine.Object;

namespace AnkleBreaker.Utils.Inspector.Editor
{
    /// <summary>
    /// Which editor spells object identity which way, for the drawers in this assembly.
    /// </summary>
    /// <remarks>
    /// <para><b>6000.4 is the boundary because two facts flip on that line.</b>
    /// <c>Object.GetInstanceID()</c> is clean from 2022.3 through 6000.3, obsolete in 6000.4 and a
    /// compile error in 6000.5 — <c>CS0619</c>, not a warning. Going the other way,
    /// <c>EntityId.ToULong</c> does not exist before 6000.4 and <c>Object.GetEntityId()</c> is absent
    /// from the assemblies of 6000.0 and 6000.1, let alone the 2022.3 this package supports. Neither
    /// call spans the range; the fork does.</para>
    ///
    /// <para><c>ToULong</c> rather than the implicit conversion to <c>int</c>: an identity is four bytes
    /// through 6000.3 and eight from 6000.5, and the implicit form still hands back an <c>int</c> — it
    /// truncates exactly where the width started to matter. The replacement spells it <c>EntityId</c>
    /// with a lowercase <c>d</c>; the <c>EntityID</c> form is not a member of any shipped editor.</para>
    ///
    /// <para>Internal on purpose. This package draws inspectors; it is not the studio's compatibility
    /// layer, and a package needing the same fork carries its own rather than depending on this one.</para>
    /// </remarks>
    internal static class AB_ObjectCompat
    {
        /// <summary>
        /// The identity of <paramref name="target"/>, full width on every supported editor.
        /// </summary>
        /// <param name="target">Any Unity object. Null — a destroyed object included — answers zero.</param>
        internal static long StableId(Object target)
        {
            if (target == null) return 0L;

#if UNITY_6000_4_OR_NEWER
            return unchecked((long)UnityEngine.EntityId.ToULong(target.GetEntityId()));
#else
            return target.GetInstanceID();
#endif
        }
    }
}
