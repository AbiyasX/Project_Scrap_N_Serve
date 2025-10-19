using UnityEngine;

[System.Serializable]
public class GameObjectRecipe
{
    public GameObject inputPrefab;    
    public GameObject outputPrefab;   
    public float craftingTime = 2f;  
}

[CreateAssetMenu(fileName = "NewWorkstation", menuName = "Workstation/GameObject Workstation")]
public class WorkstationRecipe : ScriptableObject
{
    public string workstationName;
    public GameObjectRecipe[] recipes;
}
