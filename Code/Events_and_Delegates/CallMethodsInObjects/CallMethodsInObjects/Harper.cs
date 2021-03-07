using System;
using System.Collections.Generic;
using System.Text;

namespace CallMethodsInObjects
{
    class Harper
    {
        public GetSecretIngredient HarpersSecretIngredientMethod
        {
            get
            {
                return AddHarpersSecretIngredient;
            }
        }
        private int total = 20;
        private string AddHarpersSecretIngredient(int amount)
        {
            if (total - amount < 0)
                return $"I don't have {amount} cans of sardines!";
            else
            {
                total -= amount;
                return $"{amount} cans of sardines";
            }
        }
    }
}
