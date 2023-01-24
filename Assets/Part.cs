using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[System.Serializable]
public class ConnectionInfo
{
    public string name;
    public int num_lines;
    public string parts;

    public ConnectionInfo(string name, int num_lines, string parts)
    {
        this.name = name;
        this.num_lines = num_lines;
        this.parts = parts;
    }
}

public class PartInfo
{
    public string info;
    public int num_parts;
    public int num_lines;
    public ConnectionInfo[] connections;

    public PartInfo()
    {

    }

    public PartInfo(string info, int num_parts, int num_lines, ConnectionInfo[] connections)
    {
        this.info = info;
        this.num_parts = num_parts;
        this.num_lines = num_lines;
        this.connections = connections;
    }
}

public class Part : MonoBehaviour
{
    private PartInfo[] partsInfo = new PartInfo[100];

    private const int max_con = 20;

    //1 wymiar = fizyczny obiekt atomu, 2 wymiar - referencje do połączonych atomów, 3 wymiar - referencja do obiektu lini
    private GameObject[,,] parts = new GameObject[10,max_con,2];
    private GameObject[] lines = new GameObject[max_con];
    private int[] parts_num_con = new int[10];
    private int num_parts = 0;
    private int num_lines = 0;
    public GameObject prefab_atom;
    public GameObject prefab_line;
    private GameObject clicked_atom = null;
    private GameObject line_add = null;
    private Color old_col;

    private GameObject currentAtom = null;
    public Text name_of_atom_text;
    
    public Text info_a;
    public GameObject prefab_button;
    public Canvas canvas;


    void savePartsToJSON()
    {
        string json = "";
        for (int i = 0; i < partsInfo.Length; i++)
        {
            json += JsonUtility.ToJson(partsInfo[i]) + "\n";
        }
        File.WriteAllText(Application.dataPath + "/parts.json", json);
    }

    void loadJSONFile()
    {
        if (Atom.atomsContainer == null)
        {
            Atom.atomsContainer = new AtomsContainer();
            Atom.atomsContainer.loadJSONFile();
        }

        string[] json = File.ReadAllLines(Application.dataPath + "/parts.json");
        for (int i = 0; i < json.Length; i++)
        {
            if (json[i] == "")
                return;
            partsInfo[i] = JsonUtility.FromJson<PartInfo>(json[i]);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        loadJSONFile();

        int i = 0;
        while (Atom.atomsContainer.atoms[i] != null)
        {
            GameObject partAtom = Instantiate(prefab_button, prefab_button.transform.position + new Vector3(i * 35, 0, 0), Quaternion.identity);
            partAtom.transform.SetParent(canvas.transform, false);
            partAtom.name = "PartAtom" + i;
            partAtom.SetActive(Atom.atomsContainer.atoms[i].enabled);
            int p = i;
            partAtom.GetComponent<Button>().onClick.AddListener(() => addItem(p));
            Text text = partAtom.GetComponentInChildren<Text>();
            text.text = Atom.atomsContainer.atoms[i].symbol;
            text.color = Atom.atomsContainer.atoms[i].color;

            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                name_of_atom_text.text = hit.transform.gameObject.name;
            }           
        }else
            name_of_atom_text.text = "";

        //poruszanie atomem na środkowy przycisk myszy
        if (Input.GetMouseButton(2) && currentAtom == null && line_add == null && Physics.Raycast(ray, out hit))
        {
           GameObject c_move = hit.transform.gameObject;
           Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
           mouse = Camera.main.ScreenToWorldPoint(mouse);
           c_move.transform.position = mouse;
        }

        //ruszanie krańcem lini podczas dodawania połaćzenia
        if(line_add != null)
        {
            
            Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            mouse = Camera.main.ScreenToWorldPoint(mouse);
            line_add.GetComponent<LineRenderer>().SetPosition(1, mouse);
        }

        //poruszanie atomem podczas dodawania go
        if (currentAtom != null)
        {
            Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            mouse = Camera.main.ScreenToWorldPoint(mouse);
            currentAtom.transform.position = mouse;
        }

        //anulacja dodawania lini
        if (line_add != null && Input.GetKeyDown(KeyCode.Mouse1))
        {
            GameObject.Destroy(line_add);
            line_add = null;
        }

        //anulowanie dodawania atomu
        if (currentAtom != null && Input.GetKeyDown(KeyCode.Mouse1))
        {
            GameObject.Destroy(currentAtom);
            currentAtom = null;
        }

        //dodawanie nowego atomu po kliknieciu myszy
        if (currentAtom != null && Input.GetKeyDown(KeyCode.Mouse0))
        {
            parts[num_parts,0,0] = currentAtom;
            currentAtom.gameObject.GetComponent<Gravity>().enabled = false;
            num_parts++;
            currentAtom = null;
            return;
        }

        //aktualizacja pozycji z atomu
        for(int i=0;i<num_parts;i++)
        {
            parts[i,0, 0].transform.position = new Vector3(parts[i,0, 0].transform.position.x, parts[i,0, 0].transform.position.y, 0);
            for(int j=1;j<=parts_num_con[i];j++)
            {
                parts[i, j, 1].GetComponent<LineRenderer>().SetPosition(Random.Range(0, 2), parts[i, 0, 0].transform.position);
            }
        }

        //dodawanie nowego połaćzenia
        if(currentAtom == null && clicked_atom == null && Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(ray, out hit))
        {
            clicked_atom = hit.transform.gameObject;            
            old_col = clicked_atom.gameObject.GetComponentInChildren<Light>().color;
            clicked_atom.GetComponentInChildren<Light>().color = Color.white;

            GameObject go = GameObject.Instantiate(prefab_line);          
            Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
            mouse = Camera.main.ScreenToWorldPoint(mouse);
            LineRenderer lr = go.GetComponent<LineRenderer>();

            lr.SetPosition(0, clicked_atom.transform.position);
            lr.SetPosition(1,mouse);

            line_add = go;
        }

        //anulacja zaznaczenia atomu
        if(currentAtom == null && clicked_atom != null && Input.GetKeyDown(KeyCode.Mouse1))
        {
            clicked_atom.gameObject.GetComponentInChildren<Light>().color = old_col;
            clicked_atom = null;
            line_add = null;
        }

        //akceptacja dodawania nowej lini
        if (currentAtom == null && line_add != null && Input.GetKeyDown(KeyCode.Mouse0) && Physics.Raycast(ray, out hit))
        {           
            if (clicked_atom != hit.transform.gameObject)
            {
                int x = getIndex(clicked_atom);
                int y = getIndex(hit.transform.gameObject);
                Debug.Log(x+ " -=-=- "+ y);
                if (x >= 0 && y >= 0)
                {
                    lines[num_lines] = line_add;
              
                    num_lines++;
                    clicked_atom.gameObject.GetComponentInChildren<Light>().color = old_col;        
                    parts[x, parts_num_con[x]+1, 0] = hit.transform.gameObject;
                    parts[x, parts_num_con[x]+1, 1] = line_add;
                    parts_num_con[x]++;
                    parts[y, parts_num_con[y]+1, 0] = clicked_atom;
                    parts[y, parts_num_con[y]+1, 1] = line_add;
                    parts_num_con[y]++;

                    

                    clicked_atom = null;
                    line_add = null;
                }
               


                
            }           
        }

        info_a.text = getPartText();
        
    }

    private string getPartText()
    {
        //dopasowywanie do czasteczek
        if (num_parts >= 2 && num_lines >= 1)
        {
            for (int i = 0; partsInfo[i] != null; i++)
            {
                if (partsInfo[i].num_parts == num_parts && partsInfo[i].num_lines == num_lines)
                {
                    bool[] found_connections = new bool[num_parts];
                    for (int j = 0; j < num_parts; j++)
                    {
                        found_connections[j] = false;
                    }
                    bool found = false;
                    for (int j = 0; j < num_parts; j++)
                    {
                        if (parts_num_con[j] == 0)
                        {
                            return "";
                        }
                        found = false;
                        for (int k = 0; k < partsInfo[i].connections.Length; k++)
                        {
                            if (!found_connections[k] && isConnectionEqual(partsInfo[i].connections[k], j))
                            {
                                found_connections[k] = true;
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            break;
                        }
                    }
                    if (found)
                    {
                        return partsInfo[i].info;
                    }
                }
            }
        }

        return "";
    }

    private bool isConnectionEqual(ConnectionInfo connection, int part_index)
    {
        if (connection.name != parts[part_index, 0, 0].name || connection.num_lines != parts_num_con[part_index])
        {
            return false;
        }
        string[] connection_parts = connection.parts.Split(';');
        for (int i = 1; i <= parts_num_con[part_index]; i++)
        {
            bool found = false;
            for (int j = 0; j < connection_parts.Length; j++)
            {
                if (connection_parts[j] == parts[part_index, i, 0].name)
                {
                    connection_parts[j] = "";
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                return false;
            }
        }
        return true;
    }


    public void switchScene()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    //dodawanie atomu, gdzie i to index w tabeli Atoms
    public void addItem(int i)
    {
        if (currentAtom != null)
            return;
        GameObject obj = GameObject.Instantiate(prefab_atom);
        Vector3 mouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        obj.transform.position = mouse;
        obj.GetComponentInChildren<Light>().color = Atom.atomsContainer.atoms[i].color;
        obj.name = Atom.atomsContainer.atoms[i].name;
        currentAtom = obj;
        if (num_parts > 0)
            obj.GetComponent<AudioSource>().enabled = false;
    }

    //niszczy wszystko (włącznie z komputerem)
    public void destroayAll()
    {
        for(int i=0;i<num_lines;i++)
        {
            GameObject.Destroy(lines[i]);
            lines[i] = null;
        }

        for(int i=0;i<num_parts;i++)
        {
            GameObject.Destroy(parts[i, 0, 0]);
            parts[i,0, 0] = null;
            parts[i, 0, 1] = null;
            for (int j=0;j<max_con;j++)
            {
                parts[i, j, 0] = null;
                parts[i, j, 1] = null;
            }
            parts_num_con[i] = 0;
        }
        num_lines = 0;
        num_parts = 0;
    }

    //zwraca index w tablicy danego obiektu
    int getIndex(GameObject ob)
    {
        for(int i=0;i<num_parts;i++)
        {
            if (ob == parts[i,0,0])
                return i;
        }
        return -1;
    }


    //funckje pomagajace w sprawdzaniu czy pasuje do wzorca - - - - - - - - - - //
    int getNumOfConnections(GameObject first, GameObject second)
    {
        int x = getIndex(first);
        int sum = 0;
        for(int i=0;i<max_con;i++)
        {
            if (parts[x, i, 0] == second)
                sum++;
        }
        return sum;
    }

    int getNumOfConnections(GameObject first, string second)
    {
        int x = getIndex(first);
        int sum = 0;
        for (int i = 0; i < max_con; i++)
        {
            if (parts[x, i, 0].name == second)
                sum++;
        }
        return sum;
    }

    bool isConnection(GameObject first, GameObject second)
    {
        int x = getIndex(first);
        for (int i = 0; i < max_con; i++)
        {
            if (parts[x, i, 0] == second)
                return true;
        }
        return false;
    }

    bool isConnection(GameObject first, string second)
    {
        int x = getIndex(first);
        for (int i = 0; i < max_con; i++)
        {
            if (parts[x, i, 0].name == second)
                return true;
        }
        return false;
    }

    // - - - - - - - - - -  koniec funkcji sprawdzajacych - - - - - - - - - - //
}
