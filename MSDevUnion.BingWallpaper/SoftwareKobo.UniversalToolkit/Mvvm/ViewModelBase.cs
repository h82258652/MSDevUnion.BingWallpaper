using Windows.ApplicationModel;

namespace SoftwareKobo.UniversalToolkit.Mvvm
{
    public abstract class ViewModelBase : BindableBase
    {
        public static bool IsInDesignMode
        {
            get
            {
                return DesignMode.DesignModeEnabled;
            }
        }
    }
}