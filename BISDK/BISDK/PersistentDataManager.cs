using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BISDK
{
    public interface PersistentDataManager
    {
        void SetPersistentData(string key, string value);

        string GetPersistentData(string key);

        void ClearPersistentData(string key);

        void ClearAllPersistentData();
    }
}
