using System;
using System.Collections.Generic;
using System.Linq;

namespace OldiOS.Shared.Apps.UIKit
{
    public class ViewStack
    {
        private readonly List<Type> _views = new();

        public event Action OnChange;

        public Type CurrentView => _views.LastOrDefault();

        public bool CanGoBack => _views.Count > 1;

        public void Push(Type view)
        {
            _views.Add(view);
            NotifyStateChanged();
        }

        public void Pop()
        {
            if (CanGoBack)
            {
                _views.RemoveAt(_views.Count - 1);
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
