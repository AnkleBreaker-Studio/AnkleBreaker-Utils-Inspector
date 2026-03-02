#if ODIN_INSPECTOR
    using BaseButtonAttribute = Sirenix.OdinInspector.ButtonAttribute;
#else
    using BaseButtonAttribute = EasyButtons.ButtonAttribute;
#endif

namespace AnkleBreaker.Utils.Inspector
{
    public class ButtonAttribute : BaseButtonAttribute
    {
        
    }
}