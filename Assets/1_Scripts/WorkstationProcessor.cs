using System;
using System.Collections;
using UnityEngine;

public class WorkstationProcessor : MonoBehaviour
{
    [Header("Workstation Data")]
    public WorkstationsData workstationData;
    public WorkstationSlotScript currentWorkshop;
    [Header("Spawn Settings")]
    public Transform outputSpawnPoint;  
    public bool isBusy = false;

    private void Start()
    {
        currentWorkshop = GetComponentInParent<WorkstationSlotScript>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        TryProcessItem(collision.gameObject);
    }

    public void GetWorkStationData(WorkstationsData workstationRecipe)
    {
        workstationData = workstationRecipe;
    }

    public void TryProcessItem(GameObject inputItem)
    {
        if (isBusy)
        {
            Debug.Log($"{workstationData.workstationName} is currently busy!");
            return;
        }

        foreach (var recipe in workstationData.recipes)
        {
            if (inputItem.name.Replace("(Clone)", "").Trim()
                .Equals(recipe.outputPrefab.name, StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"[{workstationData.workstationName}] Ignoring output item: {inputItem.name}");
                return;
            }
        }

        foreach (var recipe in workstationData.recipes)
        {
            if (inputItem.name.Replace("(Clone)", "").Trim()
                .Equals(recipe.inputPrefab.name, StringComparison.OrdinalIgnoreCase))
            {
                StartCoroutine(ProcessItem(recipe, inputItem));
                return;
            }
        }
    }

    private IEnumerator ProcessItem(RecipeList recipe, GameObject inputItem)
    {
        isBusy = true;
        currentWorkshop.timerStart(recipe.outputPrefab.materialIcon, recipe.craftingTime, isBusy);


        Debug.Log($"[{workstationData.workstationName}] Processing {inputItem.name}...");
        Destroy(inputItem);

        yield return new WaitForSeconds(recipe.craftingTime);

        if (recipe.outputPrefab != null)
        {
            
            GameObject Product = Instantiate(recipe.outputPrefab.materialPrefab, outputSpawnPoint.position, outputSpawnPoint.rotation);
            Product.name = recipe.outputPrefab.name;
            Vector3 Direction = outputSpawnPoint.forward;
            Product.GetComponent<Rigidbody>().AddForce(Direction * 1.5f, ForceMode.Impulse);
            Debug.Log($"[{workstationData.workstationName}] Created {recipe.outputPrefab.name}!");
        }

        isBusy = false;
    }
}
