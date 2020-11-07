using System;
using System.Collections.Generic;
using System.Text;

namespace AmazeballsSerum
{
    sealed class OrdinaryHuman
    {
        private int age;
        int weight;
        public OrdinaryHuman(int weight)
        {
            this.weight = weight;
        }
        public void GoToWork() { /* code to go to work */ }
        public void PayBills() { /* code to pay bills */ }
    }
}
