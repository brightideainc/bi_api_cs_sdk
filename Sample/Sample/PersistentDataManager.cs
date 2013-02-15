using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BISDK;

namespace Sample
{
    class PersistentDataManager : BISDK.PersistentDataManager
    {

        public void SetPersistentData(string key, string value)
        {
            
        }

        public string GetPersistentData(string key)
        {
            throw new NotImplementedException();
        }

        public void ClearPersistentData(string key)
        {
            throw new NotImplementedException();
        }

        public void ClearAllPersistentData()
        {
            
        }
    }
}
