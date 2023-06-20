using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SaveAndLoad : MonoBehaviour
{
    [SerializeField] GameObject blueprintPrefab;
    [SerializeField] GameObject recipePrefab;
    [SerializeField] Transform blueprintPlace;
    [SerializeField] Transform recipePlace;

    [SerializeField] GameObject loadOptionPrefab;
    [SerializeField] Transform loadOptionPlace;
    [SerializeField] GameObject loadWindow;
    [SerializeField] GameObject saveWindow;
    [SerializeField] Image image;
    [SerializeField] InputField inputField;
    [SerializeField] Button loadWindowOkButton;

    private string savesPath = "Assets/Resources/Saves/";
    private bool saveConfirmed = false;
    private bool saveDenied = false;

    void Update()
    {
        if (loadOptionPlace.childCount > 0)
        {
            loadWindowOkButton.interactable = true;
        }
        else
        {
            loadWindowOkButton.interactable = false;
        }
    }

    public void save()
    {
        saveWindow.SetActive(true);

        GameObject firstBranch = new GameObject();
        foreach (Outline outline in GetComponentsInChildren<Outline>())
        {
            if (outline.enabled)
            {
                firstBranch = outline.GetComponent<RecipeSettings>().getNewBranch();
                break;
            }
        }

        string name = firstBranch.transform.Find("ItemImage").GetComponent<Image>().sprite.name;
        string stringCode = firstBranch.GetComponentInChildren<BranchSettings>().getStringCode();
        string amount = firstBranch.transform.Find("Amount").GetComponent<InputField>().text;

        image.sprite = firstBranch.transform.Find("ItemImage").GetComponent<Image>().sprite;
        inputField.placeholder.GetComponent<Text>().text = name;

        StartCoroutine(saveCoroutine(name, stringCode, amount));
    }

    private IEnumerator saveCoroutine(string name, string stringCode, string amount)
    {
        yield return new WaitUntil(() => saveConfirmed || saveDenied);

        if (saveConfirmed)
        {
            string fileName;
            if (inputField.text.Length > 0)
            {
                fileName = inputField.text;
            }
            else
            {
                fileName = name;
            }
            FileStream fileStream = File.Create(savesPath + fileName + ".txt");

            fileStream.Write(new UTF8Encoding(true).GetBytes(name + "\n"));
            fileStream.Write(new UTF8Encoding(true).GetBytes(stringCode + "\n"));
            fileStream.Write(new UTF8Encoding(true).GetBytes(amount + "\n"));
            fileStream.Close();
        }

        saveConfirmed = false;
        saveDenied = false;
        saveWindow.SetActive(false);
    }

    public void load()
    {
        List<GameObject> gameObjects = new List<GameObject>();
        for (int i = 0; i < loadOptionPlace.childCount; i++)
        {
            gameObjects.Add(loadOptionPlace.GetChild(i).gameObject);
        }

        foreach (GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }

        loadWindow.SetActive(true);

        bool isEnabled = false;
        FileInfo[] files = new DirectoryInfo(savesPath).GetFiles("*.txt");
        foreach (FileInfo file in files)
        {
            GameObject newLoadOption = Instantiate(loadOptionPrefab, loadOptionPlace);
            newLoadOption.GetComponentInChildren<Text>().text = file.Name.Replace(".txt", "");
            if (!isEnabled)
            {
                newLoadOption.GetComponent<Outline>().enabled = true;
                isEnabled = true;
            }
        }
        isEnabled = false;

        StartCoroutine(loadCoroutine());
    }

    private IEnumerator loadCoroutine()
    {
        yield return new WaitUntil(() => saveConfirmed || saveDenied);

        if (saveConfirmed)
        {
            string fileName = "";
            foreach (Outline outline in loadOptionPlace.GetComponentsInChildren<Outline>())
            {
                if (outline.enabled)
                {
                    fileName = outline.GetComponentInChildren<Text>().text;
                    break;
                }
            }

            StreamReader file = new StreamReader(savesPath + fileName + ".txt");
            string[] lines = new string[File.ReadAllLines(savesPath + fileName + ".txt").Length];
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i] = file.ReadLine();
            }
            file.Close();

            GameObject blueprint = Instantiate(blueprintPrefab);
            blueprint.transform.SetParent(blueprintPlace);
            blueprint.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
            blueprint.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
            blueprint.transform.localScale = new Vector3(1, 1, 1);
            blueprint.transform.localPosition = new Vector3(0, 0, 0);

            GameObject recipe = Instantiate(recipePrefab);
            recipe.transform.SetParent(recipePlace);
            recipe.transform.localScale = new Vector3(1, 1, 1);
            recipe.GetComponent<RecipeSettings>().settings((Sprite)Resources.Load("Images/Items/" + lines[0], typeof(Sprite)), lines[0], blueprint);

            recipe.GetComponent<RecipeSettings>().getNewBranch().GetComponentInChildren<BranchSettings>().setStringCode(lines[1]);
            recipe.GetComponent<RecipeSettings>().getNewBranch().GetComponentInChildren<BranchSettings>().setAmount(lines[2]);
        }

        saveConfirmed = false;
        saveDenied = false;
        loadWindow.SetActive(false);
    }

    public void pressOk()
    {
        saveConfirmed = true;
    }

    public void pressCancel()
    {
        saveDenied = true;
    }
}
