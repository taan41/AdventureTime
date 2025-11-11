using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/UpdaterConfig")]
public class UpdaterConfig : ScriptableObject {
    [System.Serializable]
    public class UpdaterOrder {
        [HideInInspector] public string typeName;
        public int order;
    }

    public List<UpdaterOrder> entries;
}
