using System.Collections.Generic;
using System.Linq;

namespace Installer.Services
{
    internal class Notifier
    {
        #region Constructors
        public Notifier()
        {
            listeners = new HashSet<IListener>();
        }
        #endregion

        #region Private fields
        private readonly HashSet<IListener> listeners;
        #endregion

        #region Public methods
        public void AddListener(IListener listener)
        {
            if (listener != null)
            {
                this.listeners.Add(listener);
            }
        }
        public void Notify()
        {
            this.listeners.ToList().ForEach(listener => listener.Update());
        }
        #endregion
    }
}