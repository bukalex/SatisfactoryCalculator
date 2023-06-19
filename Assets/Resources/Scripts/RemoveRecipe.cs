using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveRecipe : MonoBehaviour
{
    public void removeRecipe()
    {
        if (transform.childCount > 0)
        {
            foreach (RecipeSettings recipeSettings in GetComponentsInChildren<RecipeSettings>())
            {
                if (recipeSettings.isChosen())
                {
                    recipeSettings.removeBlueprint();
                    Destroy(recipeSettings.gameObject);
                    break;
                }
            }
        }

        if (transform.childCount > 0)
        {
            GetComponentInChildren<RecipeSettings>(true).changeBlueprint();
        }
    }
}
