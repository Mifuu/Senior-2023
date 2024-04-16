using ObserverPattern;
using System.Threading.Tasks;

namespace CloudService
{
    public abstract class BaseCloudServiceSingleton<T> where T : class, new()
    {
        private static T _Singleton;
        private Subject<bool> _ready = new Subject<bool>(false);
        public static T Singleton
        {
            get
            {
                if (_Singleton == null)
                    _Singleton = new T();
                return _Singleton;
            }
            private set { }
        }
        public Subject<bool> isServiceReady { get => _ready; set { throw new System.InvalidOperationException(); } }
        public abstract Task Initialize();
    }
}
