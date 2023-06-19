using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemsWindow : MonoBehaviour
{
    [SerializeField] Transform recipesContent;
    [SerializeField] Transform outputsContent;
    [SerializeField] Transform intermediatesContent;
    [SerializeField] Transform inputsContent;
    [SerializeField] Transform sideProductsContent;

    [SerializeField] GameObject recipeTogglePrefab;
    [SerializeField] GameObject totalItemPrefab;

    [SerializeField] Transform blueprintPlace;

    private Dictionary<Outline, GameObject> recipesOutlines = new Dictionary<Outline, GameObject>();
    private Dictionary<Sprite, float> outputsSum = new Dictionary<Sprite, float>();
    private Dictionary<Sprite, float> intermediatesSum = new Dictionary<Sprite, float>();
    private Dictionary<Sprite, float> inputsSum = new Dictionary<Sprite, float>();
    private Dictionary<Sprite, float> sideProductsSum = new Dictionary<Sprite, float>();

    private void scanRecipes()
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

    private void clearColumn(Transform column, Dictionary<Sprite, float> dictionary)
    {
        List<GameObject> gameObjects = new List<GameObject>();
        for (int i = 0; i < column.childCount; i++)
        {
            gameObjects.Add(column.GetChild(i).gameObject);
        }

        foreach (GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }
        dictionary.Clear();
    }

    private void count(GameObject branch)
    {
        Sprite name = branch.transform.Find("ItemImage").GetComponent<Image>().sprite;
        float amount = float.Parse(branch.transform.Find("Amount").GetComponent<InputField>().text);
        Dictionary<GameObject, float> branches = new Dictionary<GameObject, float>();

        if (!branch.tag.Equals("SideBranch"))
        {
            branches = branch.GetComponentInChildren<BranchSettings>().getBranches();
        }

        if (branch.transform.parent.tag.Equals("Blueprint"))
        {
            addInDictionary(outputsSum, name, amount);
        }
        else if (branch.tag.Equals("Branch"))
        {
            if (branches.Count > 0)
            {
                addInDictionary(intermediatesSum, name, amount);
            }
            else
            {
                addInDictionary(inputsSum, name, amount);
            }
        }
        else if (branch.tag.Equals("SideBranch"))
        {
            addInDictionary(sideProductsSum, name, amount);
        }

        foreach (GameObject key in branches.Keys)
        {
            count(key);
        }
    }

    private void addInDictionary(Dictionary<Sprite, float> dictionary, Sprite key, float value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] += value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }

    private void createTotalItem(Transform content, Dictionary<Sprite, float> dictionary)
    {
        foreach (Sprite sprite in dictionary.Keys)
        {
            GameObject totalItem = Instantiate(totalItemPrefab, content);
            totalItem.transform.GetChild(0).GetComponent<Image>().sprite = sprite;
            totalItem.transform.GetChild(1).GetComponent<Text>().text = dictionary[sprite].ToString();
            totalItem.transform.GetChild(2).GetComponent<Text>().text = sprite.name;
        }
    }

    private void fillTable()
    {
        clearColumn(outputsContent, outputsSum);
        clearColumn(intermediatesContent, intermediatesSum);
        clearColumn(inputsContent, inputsSum);
        clearColumn(sideProductsContent, sideProductsSum);

        foreach (Outline outline in recipesOutlines.Keys)
        {
            if (outline.enabled)
            {
                count(recipesOutlines[outline]);
            }
        }

        createTotalItem(outputsContent, outputsSum);
        createTotalItem(intermediatesContent, intermediatesSum);
        createTotalItem(inputsContent, inputsSum);
        createTotalItem(sideProductsContent, sideProductsSum);
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
