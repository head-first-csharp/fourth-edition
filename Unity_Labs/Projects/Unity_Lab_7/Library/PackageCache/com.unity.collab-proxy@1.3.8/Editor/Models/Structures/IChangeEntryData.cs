using System;

namespace Unity.Cloud.Collaborate.Models.Structures
{
    internal interface IChangeEntryData
    {
        IChangeEntry Entry { get; }
        bool Toggled { get; }
        bool All { get; }
        bool ToggleReadOnly { get; }
        bool Conflicted { get; }
    }
}
