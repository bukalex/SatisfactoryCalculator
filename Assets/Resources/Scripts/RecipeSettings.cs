using UnityEngine;
using UnityEngine.UI;

public class RecipeSettings : MonoBehaviour
{
    private GameObject blueprint;
    private GameObject branchPrefab;
    private GameObject newBranch;
    private string branchPrefabPath = "Prefabs/Branch";

    public void settings(Sprite image, string text, GameObject blueprint)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = image;
        transform.GetChild(1).GetComponent<Text>().text = text;
        this.blueprint = blueprint;

        branchPrefab = (GameObject)Resources.Load(branchPrefabPath, typeof(GameObject));
        newBranch = Instantiate(branchPrefab, blueprint.transform);
        newBranch.transform.Find("ItemImage").GetComponent<Image>().sprite = image;
        newBranch.transform.localPosition = blueprint.transform.Find("StartPoint").localPosition;
        newBranch.GetComponentInChildren<InputField>().readOnly = false;

        changeBlueprint();
    }

    public void changeBlueprint()
    {
        Transform blueprintPlace = GameObject.Find("BlueprintPlace").transform;
        Transform recipePlace = GameObject.Find("RecipeListContent").transform;
        for (int i = 0; i < blueprintPlace.childCount; i++)
        {
            blueprintPlace.GetChild(i).gameObject.SetActive(false);
            recipePlace.GetComponentsInChildren<Outline>()[i].enabled = false;
        }
        blueprint.SetActive(true);
        GetComponent<Outline>().enabled = true;
    }

    public void removeBlueprint()
    {
        Destroy(blueprint);
    }

    public bool isChosen()
    {
        return blueprint.activeSelf;
    }

    public GameObject getNewBranch()
    {
        return newBranch;
    }
}
