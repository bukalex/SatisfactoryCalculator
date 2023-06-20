using System.Collections;
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

        StartCoroutine(removeRecipeCoroutine());
    }

    private IEnumerator removeRecipeCoroutine()
    {
        yield return new WaitForEndOfFrame();

        if (transform.childCount > 0)
        {
            GetComponentInChildren<RecipeSettings>().changeBlueprint();
        }
    }
}
