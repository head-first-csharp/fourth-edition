using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace Unity.Cloud.Collaborate.Common
{
    //http://answers.unity3d.com/answers/809221/view.html

    [Serializable]
    internal class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField]
        List<TKey> m_Keys = new List<TKey>();
        [SerializeField]
        List<TValue> m_Values = new List<TValue>();

        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> input) : base(input)
        {
        }

        // Save the dictionary to lists
        public void OnBeforeSerialize()
        {
            m_Keys.Clear();
            m_Values.Clear();
            foreach (var pair in this)
            {
                m_Keys.Add(pair.Key);
                m_Values.Add(pair.Value);
            }
        }

        // load dictionary from lists
        public void OnAfterDeserialize()
        {
            Clear();

            if (m_Keys.Count != m_Values.Count)
            {
                throw new SerializationException($"there are {m_Keys.Count} keys and {m_Values.Count} " +
                    "values after deserialization. Make sure that both key and value types are serializable.");
            }

            for (var i = 0; i < m_Keys.Count; i++)
            {
                Add(m_Keys[i], m_Values[i]);
            }
        }
    }
}
