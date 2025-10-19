using System.Collections;
using UnityEngine;

public class WorkstationProcessor : MonoBehaviour
{
    [Header("Workstation Data")]
    public WorkstationRecipe workstationData;

    [Header("Spawn Settings")]
    public Transform outputSpawnPoint;  
    public bool isBusy = false;

    
    public void TryProcessItem(GameObject inputItem)
    {
        if (isBusy)
        {
            Debug.Log($"{workstationData.workstationName} is currently busy!");
            return;
        }

       
        foreach (var recipe in workstationData.recipes)
        {
            if (inputItem.name.Contains(recipe.inputPrefab.name))
            {
                StartCoroutine(ProcessItem(recipe, inputItem));
                return;
            }
        }

        Debug.Log($"No valid recipe found for {inputItem.name} in {workstationData.workstationName}!");
    }

    private IEnumerator ProcessItem(GameObjectRecipe recipe, GameObject inputItem)
    {
        isBusy = true;

       
        Debug.Log($"[{workstationData.workstationName}] Processing {inputItem.name}...");
        Destroy(inputItem);

        // Wait for crafting time
        yield return new WaitForSeconds(recipe.craftingTime);

        // Spawn output
        if (recipe.outputPrefab != null)
        {
            Vector3 spawnPos = outputSpawnPoint != null ? outputSpawnPoint.position : transform.position + Vector3.up * 1f;
            Instantiate(recipe.outputPrefab, spawnPos, Quaternion.identity);
            Debug.Log($"[{workstationData.workstationName}] Created {recipe.outputPrefab.name}!");
        }

        isBusy = false;
    }
}
