//Controls the Prefab and Template variables by working with PCG.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HUD : MonoBehaviour
{
    //Unity UI script to handle all parameters.
    public PCG generatorScript; public Camera Camera;
    public TextEffect textEffectScript;
    public int templateLoop = 0;
    public TextMeshProUGUI templateText; public TMP_InputField templateInputText;
    public GameObject loading;

    //Updates the first road points to create the houses.
    void Start() { StartCoroutine(LateStart()); }
    IEnumerator LateStart()
    {
        yield return new WaitUntil(() => textEffectScript.skip);
        loading.SetActive(true);
        generatorScript.AddPoint(new Vector3(50, 0, 0)); generatorScript.AddPoint(new Vector3(-50, 0, 0));
        generatorScript.AddBuildings();
        yield return new WaitUntil(() => generatorScript.Drawing); yield return new WaitUntil(() => !generatorScript.Drawing);
        Camera.transform.Rotate(180, 0, 0);
        loading.SetActive(false);
        Camera.GetComponent<Camera>().orthographicSize = (generatorScript.roadsParent.GetComponent<MeshFilter>().mesh.bounds.size.x + generatorScript.roadsParent.GetComponent<MeshFilter>().mesh.bounds.size.z) / 2;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) { Split(); }
        if (Input.GetKeyDown(KeyCode.R)) { Reset(); }
        if (Input.GetKeyDown(KeyCode.T)) { AddTemplate(); }
    }
    //Splits up the Roads to make plenty more, but first Resets the current template.
    public void Split() { if (generatorScript.Drawing || !textEffectScript.skip || loading.activeSelf) return; StartCoroutine(split()); }
    IEnumerator split()
    {
        Camera.transform.Rotate(180, 0, 0);
        loading.SetActive(true);

        generatorScript.Reset();

        if (templateLoop == 0)
        {
            generatorScript.AddPoint(new Vector3(50, 0, 0)); generatorScript.AddPoint(new Vector3(-50, 0, 0));
        }
        else if (templateLoop == 1)
        {
            generatorScript.AddPoint(new Vector3(0, 0, 50)); generatorScript.AddPoint(new Vector3(0, 0, -50));
        }
        else if (templateLoop == 2)
        {
            generatorScript.AddPoint(new Vector3(25, 0, 25)); generatorScript.AddPoint(new Vector3(-25, 0, 25));
            generatorScript.AddPoint(new Vector3(25, 0, 25)); generatorScript.AddPoint(new Vector3(25, 0, -25));
            generatorScript.AddPoint(new Vector3(25, 0, -25)); generatorScript.AddPoint(new Vector3(-25, 0, -25));
            generatorScript.AddPoint(new Vector3(-25, 0, 25)); generatorScript.AddPoint(new Vector3(-25, 0, -25));
        }
        else if (templateLoop == 3)
        {
            generatorScript.AddPoint(new Vector3(25, 0, 25)); generatorScript.AddPoint(new Vector3(-25, 0, 25));
            generatorScript.AddPoint(new Vector3(25, 0, 25)); generatorScript.AddPoint(new Vector3(-25, 0, -25));
            generatorScript.AddPoint(new Vector3(25, 0, -25)); generatorScript.AddPoint(new Vector3(-25, 0, -25));
        }        

        generatorScript.roadManagerScript.FindCentre(generatorScript.Points);
        
        for (int i = 0; i < Random.Range(10, 75); i++) { generatorScript.roadManagerScript.Split(); }
        generatorScript.CreateRoad();

        generatorScript.AddBuildings();
        yield return new WaitUntil(() => generatorScript.Drawing); yield return new WaitUntil(() => !generatorScript.Drawing);
        Camera.transform.Rotate(180, 0, 0);
        loading.SetActive(false);
        Camera.GetComponent<Camera>().orthographicSize = (generatorScript.roadsParent.GetComponent<MeshFilter>().mesh.bounds.size.x + generatorScript.roadsParent.GetComponent<MeshFilter>().mesh.bounds.size.z) / 2;
    }
    //Conditions to reset the iterations or scene.
    public void Reset() { if (!textEffectScript.skip) return; StartCoroutine(reset()); }
    IEnumerator reset()
    {
        Camera.transform.Rotate(180, 0, 0);
        loading.SetActive(true);

        generatorScript.Reset();

        generatorScript.AddPoint(new Vector3(50, 0, 0)); generatorScript.AddPoint(new Vector3(-50, 0, 0));
        generatorScript.AddBuildings();
        yield return new WaitUntil(() => generatorScript.Drawing); yield return new WaitUntil(() => !generatorScript.Drawing);
        Camera.transform.Rotate(180, 0, 0);
        loading.SetActive(false);
        Camera.GetComponent<Camera>().orthographicSize = (generatorScript.roadsParent.GetComponent<MeshFilter>().mesh.bounds.size.x + generatorScript.roadsParent.GetComponent<MeshFilter>().mesh.bounds.size.z) / 2;

        templateLoop = 0;
        templateText.text = "> C:\\Template\\Number\\Input.GetKey('t'): " + templateLoop + ".0f";
    }
    public void HardReset() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    //Loads from the templates.
    public void AddTemplate() { if (generatorScript.Drawing || !textEffectScript.skip || loading.activeSelf) return; if (!loading.activeSelf) { templateLoop++; if (templateLoop > 3) templateLoop = 0; StartCoroutine(Template()); } }
    public void Template(string newText)
    {
        if (!generatorScript.Drawing && !loading.activeSelf && !loading.activeSelf && newText != "" && !newText.Contains(">") && !newText.Contains("-"))
        {
            if (int.Parse(newText) < 0) newText = 0.ToString(); else if (int.Parse(newText) > 3) newText = 3.ToString();
            templateLoop = int.Parse(newText);
            templateInputText.text = "";
            StartCoroutine(Template());
        }
        else templateInputText.text = "";
    }
    IEnumerator Template()
    {
        Camera.transform.Rotate(180, 0, 0);
        loading.SetActive(true);

        generatorScript.Reset();

        if (templateLoop == 0)
        {
            generatorScript.AddPoint(new Vector3(50, 0, 0)); generatorScript.AddPoint(new Vector3(-50, 0, 0));
            generatorScript.AddBuildings();
            yield return new WaitUntil(() => generatorScript.Drawing); yield return new WaitUntil(() => !generatorScript.Drawing);
        }
        else if (templateLoop == 1)
        {
            generatorScript.AddPoint(new Vector3(0, 0, 50)); generatorScript.AddPoint(new Vector3(0, 0, -50));
            generatorScript.AddBuildings();
            yield return new WaitUntil(() => generatorScript.Drawing); yield return new WaitUntil(() => !generatorScript.Drawing);
        }
        else if (templateLoop == 2)
        {
            generatorScript.AddPoint(new Vector3(25, 0, 25)); generatorScript.AddPoint(new Vector3(-25, 0, 25));
            generatorScript.AddPoint(new Vector3(25, 0, 25)); generatorScript.AddPoint(new Vector3(25, 0, -25));
            generatorScript.AddPoint(new Vector3(25, 0, -25)); generatorScript.AddPoint(new Vector3(-25, 0, -25));
            generatorScript.AddPoint(new Vector3(-25, 0, 25)); generatorScript.AddPoint(new Vector3(-25, 0, -25));
            generatorScript.AddBuildings();
            yield return new WaitUntil(() => generatorScript.Drawing); yield return new WaitUntil(() => !generatorScript.Drawing);
        }
        else if (templateLoop == 3)
        {
            generatorScript.AddPoint(new Vector3(25, 0, 25)); generatorScript.AddPoint(new Vector3(-25, 0, 25));
            generatorScript.AddPoint(new Vector3(25, 0, 25)); generatorScript.AddPoint(new Vector3(-25, 0, -25));
            generatorScript.AddPoint(new Vector3(25, 0, -25)); generatorScript.AddPoint(new Vector3(-25, 0, -25));
            generatorScript.AddBuildings();
            yield return new WaitUntil(() => generatorScript.Drawing); yield return new WaitUntil(() => !generatorScript.Drawing);
        }
        //Resets the Cameras variables.
        Camera.transform.Rotate(180, 0, 0);
        loading.SetActive(false);
        Camera.GetComponent<Camera>().orthographicSize = (generatorScript.roadsParent.GetComponent<MeshFilter>().mesh.bounds.size.x + generatorScript.roadsParent.GetComponent<MeshFilter>().mesh.bounds.size.z) / 2;
        templateText.text = "> C:\\Template\\Number\\Input.GetKey('t'): " + templateLoop + ".0f";
    }
}