using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddWindowDropdown : MonoBehaviour
{
    [SerializeField] GameObject addWindow;

    [SerializeField] InputField search;
    [SerializeField] GameObject blueprintPrefab;
    [SerializeField] GameObject recipePrefab;
    [SerializeField] Transform blueprintPlace;
    [SerializeField] Transform recipePlace;

    private Dropdown dropdown;
    private Dictionary<string, string[]> recipes;
    private string imagesPath = "Images/Items/";

    private List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();

    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        recipes = GetComponent<FileReader>().readFile("Recipes");

        createOptions();
    }

    private void createOptions()
    {
        dropdown.options.Clear();
        foreach (string key in recipes.Keys)
        {
            if (!(recipes[key][0].Equals("nature") && recipes[key].Length == 1))
            {
                options.Add(new Dropdown.OptionData(key));
                options[options.Count - 1].image = (Sprite)Resources.Load(imagesPath + key, typeof(Sprite));
            }
        }
        dropdown.options = options;
    }

    public void searchOptions(InputField search)
    {
        string text = search.text.ToLower();
        dropdown.options = new List<Dropdown.OptionData>();
        foreach (Dropdown.OptionData option in options)
        {
            if (option.text.Contains(text))
            {
                dropdown.options.Add(option);
            }
        }
        dropdown.RefreshShownValue();
    }

    public void createRecipe()
    {
        GameObject blueprint = Instantiate(blueprintPrefab);
        blueprint.transform.SetParent(blueprintPlace);
        blueprint.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
        blueprint.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
        blueprint.transform.localScale = new Vector3(1, 1, 1);
        blueprint.transform.localPosition = new Vector3(0, 0, 0);

        GameObject recipe = Instantiate(recipePrefab);
        recipe.transform.SetParent(recipePlace);
        recipe.transform.localScale = new Vector3(1, 1, 1);
        recipe.GetComponent<RecipeSettings>().settings(dropdown.options[dropdown.value].image, dropdown.options[dropdown.value].text, blueprint);

        addWindow.SetActive(false);
    }
}
