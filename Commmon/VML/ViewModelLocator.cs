using System;
using System.Collections.Generic;

namespace Common.VML
{
    internal class ViewModelLocator
    {
        #region Private fields
        private readonly Dictionary<Type, object> viewModels = new Dictionary<Type, object>();
        #endregion

        #region Public methods
        public T CreateViewModel<T>() where T : class, new()
        {
            Type type = typeof(T);
            if (!this.viewModels.TryGetValue(type, out object existing))
            {
                existing = new T();
                this.viewModels[type] = existing;
            }
            return existing as T;
        }
        #endregion
    }
}