using Unity.VisualScripting;
using UnityEngine;

public class BlueprintBoxScript : MonoBehaviour
{
    public AssemblyRecipeData[] productRecipe;



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            Debug.Log("Get Item");
        }
    }
}
