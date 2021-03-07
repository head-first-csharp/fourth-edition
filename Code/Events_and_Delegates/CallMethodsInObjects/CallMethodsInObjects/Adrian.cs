using System;
using System.Collections.Generic;
using System.Text;

namespace CallMethodsInObjects
{
    class Adrian
    {
        public GetSecretIngredient MySecretIngredientMethod
        {
            get
            {
                return AddAdriansSecretIngredient;
            }
        }
        private string AddAdriansSecretIngredient(int amount)
        {
            return $"{amount} ounces of cloves";
        }
    }
}