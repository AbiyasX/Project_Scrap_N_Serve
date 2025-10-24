using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingMenu : MonoBehaviour
{
    public AssemblyRecipeData[] recipes;
    public GameObject prefab;
    public GameObject ingridientPrefab;
    public Transform craftingMenuParent;
    
    void Start()
    {

        foreach (var recipeList in recipes)
        {
            GameObject recipeSlot = Instantiate(prefab, craftingMenuParent);
            Image icon = recipeSlot.transform.Find("ResultIcon").GetComponent<Image>();
            icon.sprite = recipeList.product.materialIcon;
            TextMeshProUGUI resultText = recipeSlot.transform.Find("ResultIcon/ResultText").GetComponent<TextMeshProUGUI>();
            resultText.text = recipeList.product.materialName;

            Transform ingridientParent = recipeSlot.transform.Find("IngridientsList");

            
            foreach (var ingridient in recipeList.itemDatas)
            {
                GameObject ingridientsSlot = Instantiate(ingridientPrefab, ingridientParent);
                Image indgridientIcon = ingridientsSlot.GetComponent <Image>();
                indgridientIcon.sprite = ingridient.item.materialIcon;
                TextMeshProUGUI ingridientsText = ingridientsSlot.transform.Find("IngridientText").GetComponent <TextMeshProUGUI>();
                ingridientsText.text = ingridient.item.materialName;
            }
        }
        

    }

    void Update()
    {
        
    }
}
