using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourceSystem : StaticInstance<ResourceSystem>
{
    //public List<ExampleScriptable> ScriptableData { get; private set; }
    //private Dictionary<int, ExampleScriptable> _ObjectDir;

    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources()
    {
        //ScriptableData = Resources.LoadAll<ExampleScriptable>("Example Items").ToList();
        //_ObjectDir = ScriptableData.ToDictionary(r => r.Id, r => r);
    }
}