#if ODIN_INSPECTOR
using BaseEditor = Sirenix.OdinInspector.Editor.OdinEditor;
#else
using BaseEditor = UnityEditor.Editor;
#endif

namespace AnkleBreaker.Utils.Inspector.Editor
{
    public class ABEditor : BaseEditor
    {
        #if !ODIN_INSPECTOR
        
        protected virtual void OnEnable()
        {
            
        }
        
        #endif
    }
}