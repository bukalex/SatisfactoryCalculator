using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BranchSettings : MonoBehaviour
{
    [SerializeField] Transform lineMiddle;
    [SerializeField] Transform lineCornerEnd;
    [SerializeField] Transform lineEnd;
    [SerializeField] Transform sideGroupCanvas;
    [SerializeField] Transform groupCanvas;
    [SerializeField] Image itemImage;
    [SerializeField] InputField inputField;

    private GameObject branchPrefab;
    private string branchPrefabPath = "Prefabs/Branch";

    private GameObject sideBranchPrefab;
    private string sideBranchPrefabPath = "Prefabs/SideBranch";

    private string imagesPath = "Images/Items/";

    private Dropdown dropdown;
    private Dictionary<string, string[]> recipes;
    private Dictionary<string, string[]> ingredientsByRecipeTitle = new Dictionary<string, string[]>();
    private Dictionary<string, string[]> sideProductsByRecipeTitle = new Dictionary<string, string[]>();

    private Dictionary<GameObject, float> branches = new Dictionary<GameObject, float>();

    private bool inLoad = true;

    void Start()
    {
        branchPrefab = (GameObject)Resources.Load(branchPrefabPath, typeof(GameObject));
        sideBranchPrefab = (GameObject)Resources.Load(sideBranchPrefabPath, typeof(GameObject));
        dropdown = GetComponent<Dropdown>();
        recipes = GetComponent<FileReader>().readFile("Recipes");

        createOptions();
    }

    void Update()
    {
        if (inputField.text == "")
        {
            inputField.text = "1";
        }

        foreach (GameObject branch in branches.Keys)
        {
            if (branch.tag.Equals("Branch"))
            {
                createLine(branch.transform.Find("LineStart"), branch.transform.Find("LineCornerStart"), lineCornerEnd, lineEnd);
            }
            else
            {
                createLine(branch.transform.Find("LineStart"), branch.transform.Find("LineCornerStart"), lineMiddle, lineEnd);
            }
        }
    }

    private void createOptions()
    {
        dropdown.options.Clear();
        foreach (string recipe in recipes[itemImage.sprite.name])
        {
            string recipeTitle = recipe.Split(" [")[0];
            dropdown.options.Add(new Dropdown.OptionData(recipeTitle));

            if (!recipeTitle.Equals("nature"))
            {
                ingredientsByRecipeTitle.Add(recipeTitle, recipe.Split(" [")[1].Replace("]", "").Split(" -> ")[0].Split(", "));
                if (recipe.Contains(" -> "))
                {
                    sideProductsByRecipeTitle.Add(recipeTitle, recipe.Split(" [")[1].Replace("]", "").Split(" -> ")[1].Split(", "));
                }
            }
        }

        dropdown.RefreshShownValue();
        changeOption();
    }

    public void changeOption()
    {
        inLoad = true;
        foreach (GameObject branch in branches.Keys)
        {
            Destroy(branch);
        }
        branches = new Dictionary<GameObject, float>();

        string recipeTitle = dropdown.captionText.text;
        if (!recipeTitle.Equals("nature"))
        {
            foreach (string ingredient in ingredientsByRecipeTitle[recipeTitle])
            {
                GameObject newBranch = Instantiate(branchPrefab, groupCanvas);
                newBranch.transform.Find("ItemImage").GetComponent<Image>().sprite = (Sprite)Resources.Load(imagesPath + ingredient.Split(" x ")[0], typeof(Sprite));

                float a = float.Parse(ingredient.Split(" x ")[1].Split("/")[0]);
                float b = float.Parse(ingredient.Split(" x ")[1].Split("/")[1]);

                branches.Add(newBranch, a/b);
            }

            if (sideProductsByRecipeTitle.ContainsKey(recipeTitle))
            {
                foreach (string ingredient in sideProductsByRecipeTitle[recipeTitle])
                {
                    GameObject newBranch = Instantiate(sideBranchPrefab, sideGroupCanvas);
                    newBranch.transform.Find("ItemImage").GetComponent<Image>().sprite = (Sprite)Resources.Load(imagesPath + ingredient.Split(" x ")[0], typeof(Sprite));

                    float a = float.Parse(ingredient.Split(" x ")[1].Split("/")[0]);
                    float b = float.Parse(ingredient.Split(" x ")[1].Split("/")[1]);

                    branches.Add(newBranch, a / b);
                }
            }
        }
        changeAmount();
    }

    private void createLine(Transform start, Transform cornerStart, Transform cornerEnd, Transform end)
    {
        LineRenderer lineRenderer = start.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, new Vector3(start.position.x, start.position.y, -1));
        lineRenderer.SetPosition(1, new Vector3(cornerStart.position.x, cornerStart.position.y, -1));
        lineRenderer.SetPosition(2, new Vector3(cornerEnd.position.x, cornerEnd.position.y, -1));
        lineRenderer.SetPosition(3, new Vector3(end.position.x, end.position.y, -1));
    }

    public void changeAmount()
    {
        foreach (GameObject branch in branches.Keys)
        {
            branch.transform.Find("Amount").GetComponent<InputField>().text = (float.Parse(inputField.text) * branches[branch]).ToString();
        }
        inLoad = false;
    }

    public Dictionary<GameObject, float> getBranches()
    {
        return branches;
    }

    public string getStringCode()
    {
        string stringCode = dropdown.value.ToString() + "-[";
        foreach (GameObject branch in branches.Keys)
        {
            if (branch.GetComponentInChildren<BranchSettings>() != null)
            {
                stringCode += branch.GetComponentInChildren<BranchSettings>().getStringCode();
            }
        }
        stringCode += "]";

        return stringCode;
    }

    public void setStringCode(string stringCode)
    {
        StartCoroutine(setStringCodeCoroutine(stringCode));
    }

    private IEnumerator setStringCodeCoroutine(string stringCode)
    {
        yield return new WaitWhile(() => inLoad);

        dropdown.value = int.Parse(stringCode.Substring(0, stringCode.IndexOf("-")));
        string substring = stringCode.Substring(stringCode.IndexOf("-") + 1);
        string nextBranch = new string("");
        List<string> nextBranches = new List<string>();
        int brackets = 0;
        for (int i = 1; i < substring.Length - 1; i++)
        {
            nextBranch += substring[i];
            if (substring[i] == '[')
            {
                brackets++;
            }
            if (substring[i] == ']')
            {
                brackets--;
            }
            if (brackets == 0 && nextBranch.Contains("["))
            {
                nextBranches.Add(nextBranch);
                nextBranch = new string("");
            }
        }

        int index = 0;
        foreach (GameObject branch in branches.Keys)
        {
            if (branch.GetComponentInChildren<BranchSettings>() != null)
            {
                branch.GetComponentInChildren<BranchSettings>().setStringCode(nextBranches[index]);
                index++;
            }
        }
    }

    public void setAmount(string newAmount)
    {
        inputField.text = newAmount;
    }
}
