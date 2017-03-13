using System;
using System.Collections.Generic;

namespace LlapiExample
{
    public class OwnTable
    {
        private Dictionary<Guid, int> entityOwners;

        public OwnTable()
        {
            entityOwners = new Dictionary<Guid, int>();
        }

        public void Obtain(Guid id, int owner)
        {
            if (!entityOwners.ContainsKey(id))
                entityOwners.Add(id, -1);

            entityOwners[id] = owner;
        }

        public void Release(Guid id)
        {
            if (entityOwners.ContainsKey(id))
                entityOwners.Remove(id);
        }

        public int Owner(Guid id)
        {
            if (entityOwners.ContainsKey(id))
                return entityOwners[id];
            else
                return -1;
        }
    }
}