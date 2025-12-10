using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveRegistry
{
    private static readonly List<ISaveable> _saveables = new();

    public static IEnumerable<ISaveable> All => _saveables;

    public static void Register(ISaveable saveable)
    {
        if (!_saveables.Contains(saveable))
            _saveables.Add(saveable);
    }

    public static void Unregister(ISaveable saveable)
    {
        _saveables.Remove(saveable);
    }
}
