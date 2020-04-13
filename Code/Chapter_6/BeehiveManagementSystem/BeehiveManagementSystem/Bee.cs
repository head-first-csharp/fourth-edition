using System;
using System.Collections.Generic;
using System.Text;

namespace BeehiveManagementSystem
{
    class Bee
    {
        public virtual float CostPerShift { get; }

        public string Job { get; private set; }

        public Bee(string job)
        {
            Job = job;
        }

        public void WorkTheNextShift()
        {
            if (HoneyVault.ConsumeHoney(CostPerShift))
            {
                DoJob();
            }
        }

        protected virtual void DoJob() { /* the subclass overrides this */ }
    }
}
