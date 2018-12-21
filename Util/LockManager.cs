using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Auctus.Util
{
    public static class LockManager
    {
        private static ConcurrentDictionary<int, object> _lock = new ConcurrentDictionary<int, object>();
        public static object GetLock(int userId)
        {
            return _lock.GetOrAdd(userId, new object());
        }
    }
}
