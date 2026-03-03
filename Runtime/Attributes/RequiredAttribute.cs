using UnityEngine;

namespace AnkleBreaker.Utils.Inspector
{
    /// <summary>
    /// Displays an error HelpBox when an Object reference or string field is null/empty.
    /// Helps catch missing references before entering play mode.
    /// </summary>
    /// <example><code>
    /// [Required]
    /// public GameObject target;
    /// 
    /// [Required("AudioSource must be assigned!")]
    /// public AudioSource source;
    /// </code></example>
    public class RequiredAttribute : PropertyAttribute
    {
        public string Message { get; private set; }

        public RequiredAttribute(string message = null)
        {
            Message = message;
        }
    }
}
