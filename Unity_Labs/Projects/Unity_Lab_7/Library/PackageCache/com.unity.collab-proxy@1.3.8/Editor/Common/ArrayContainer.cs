using System;
using UnityEngine;

namespace Unity.Cloud.Collaborate.Common {
    [Serializable]
    internal class ArrayContainer<T>
    {
        [SerializeField] public T[] Values = new T[0];
    }
}
