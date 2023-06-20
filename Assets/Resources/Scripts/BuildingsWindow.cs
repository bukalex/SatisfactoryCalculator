using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildingsWindow : MonoBehaviour
{
    [SerializeField] Transform recipesContent;
    [SerializeField] Transform buildingsContent;

    [SerializeField] GameObject recipeTogglePrefab;
    [SerializeField] GameObject buildingPrefab;

    [SerializeField] Transform blueprintPlace;

    private Dictionary<Outline, GameObject> recipesOutlines = new Dictionary<Outline, GameObject>();
    private Dictionary<string, float> outputsSum = new Dictionary<string, float>();
    private Dictionary<string, Sprite> outputsSprite = new Dictionary<string, Sprite>();

    public void scanRecipes()
    {
        if (recipesOutlines.Count != 0)
        {
            foreach (Outline outline in recipesOutlines.Keys)
            {
                Destroy(outline.gameObject);
            }
            recipesOutlines.Clear();
        }

        for (int i = 0; i < blueprintPlace.childCount; i++)
        {
            Transform recipeFirstBranch = blueprintPlace.GetChild(i).GetChild(1);

            GameObject recipeToggle = Instantiate(recipeTogglePrefab, recipesContent);
            recipeToggle.transform.GetChild(0).GetComponent<Image>().sprite = recipeFirstBranch.Find("ItemImage").GetComponent<Image>().sprite;
            recipeToggle.transform.GetChild(1).GetComponent<Text>().text = recipeFirstBranch.Find("ItemImage").GetComponent<Image>().sprite.name;
            recipeToggle.GetComponent<Button>().onClick.AddListener(delegate { changeRecipeSelection(recipeToggle.GetComponent<Outline>()); });

            recipesOutlines.Add(recipeToggle.GetComponent<Outline>(), recipeFirstBranch.gameObject);
        }
    }

    public void fillTable()
    {
        List<GameObject> gameObjects = new List<GameObject>();
        for (int i = 0; i < buildingsContent.childCount; i++)
        {
            gameObjects.Add(buildingsContent.GetChild(i).gameObject);
        }

        foreach (GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }
        outputsSum.Clear();
        outputsSprite.Clear();

        foreach (Outline outline in recipesOutlines.Keys)
        {
            if (outline.enabled)
            {
                count(recipesOutlines[outline]);
            }
        }

        Dictionary<string, string[]> buildingsFile = GetComponent<FileReader>().readFile("Buildings");

        foreach (string recipeName in outputsSum.Keys)
        {
            string buildingName = "";
            float normalAmount = 0;
            foreach (string recipe in buildingsFile[outputsSprite[recipeName].name])
            {
                if (recipe.Split(" [")[0].Equals(recipeName))
                {
                    buildingName = recipe.Split(" [")[1].Replace("]", "").Split(", ")[0];
                    string normalAmountString = recipe.Split(" [")[1].Replace("]", "").Split(", ")[1];
                    normalAmount = float.Parse(normalAmountString.Split("/")[0]) / float.Parse(normalAmountString.Split("/")[1]);
                    break;
                }
            }

            GameObject building = Instantiate(buildingPrefab, buildingsContent);

            building.transform.Find("ItemInfo").GetChild(0).GetComponent<Image>().sprite = outputsSprite[recipeName];
            building.transform.Find("ItemInfo").GetChild(1).GetComponent<Text>().text = outputsSum[recipeName].ToString();
            building.transform.Find("ItemInfo").GetChild(3).GetComponent<Text>().text = normalAmount.ToString();
            building.transform.Find("ItemInfo").GetChild(4).GetComponent<Text>().text = recipeName;

            building.transform.Find("BuildingInfo").GetChild(0).GetComponent<Image>().sprite = (Sprite)Resources.Load("Images/Items/" + buildingName, typeof(Sprite));
            building.transform.Find("BuildingInfo").GetChild(1).GetComponent<Text>().text = Mathf.CeilToInt(outputsSum[recipeName]/normalAmount).ToString();
            building.transform.Find("BuildingInfo").GetChild(2).GetComponent<Text>().text = buildingName;
        }
    }

    private void count(GameObject branch)
    {
        Sprite sprite = branch.transform.Find("ItemImage").GetComponent<Image>().sprite;
        float amount = float.Parse(branch.transform.Find("Amount").GetComponent<InputField>().text);
        string name = branch.transform.Find("Title").GetComponent<Dropdown>().captionText.text;
        Dictionary<GameObject, float> branches = branch.GetComponentInChildren<BranchSettings>().getBranches();

        if (branches.Count > 0)
        {
            if (outputsSum.ContainsKey(name))
            {
                outputsSum[name] += amount;
            }
            else
            {
                outputsSum.Add(name, amount);
                outputsSprite.Add(name, sprite);
            }
        }

        foreach (GameObject key in branches.Keys)
        {
            if (key.tag.Equals("Branch"))
            {
                count(key);
            }
        }
    }

    public void openWindow()
    {
        if (!gameObject.activeSelf)
        {
            scanRecipes();
            fillTable();
        }
        gameObject.SetActive(!gameObject.activeSelf);
    }

    public void changeRecipeSelection(Outline outline)
    {
        outline.enabled = !outline.enabled;
        fillTable();
    }
}
