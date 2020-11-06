using System;
using System.Collections.Generic;
using System.Text;

namespace BeehiveManagementSystem
{
    abstract class Bee : IWorker
    {
        public abstract float CostPerShift { get; }
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

        protected abstract void DoJob();
    }

}
