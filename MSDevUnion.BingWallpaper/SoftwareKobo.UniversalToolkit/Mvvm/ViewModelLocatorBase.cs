using Microsoft.Practices.Unity;

namespace SoftwareKobo.UniversalToolkit.Mvvm
{
    public abstract class ViewModelLocatorBase : UnityContainer
    {
        protected virtual void Register<T>() where T : class
        {
            this.RegisterType<T>();
        }

        protected virtual void Register<TFrom, TTo>() where TTo : TFrom
        {
            this.RegisterType<TFrom, TTo>();
        }

        protected virtual T GetInstance<T>()
        {
            return this.Resolve<T>();
        }
    }
}