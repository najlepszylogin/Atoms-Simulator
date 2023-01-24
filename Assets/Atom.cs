using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Atom : MonoBehaviour 
{
    static public AtomsContainer atomsContainer;

    static public int max_nucleons = 20;
    

    int num_of_protons = 0;
    int num_of_neutrons = 0;
    int[] num_of_electrons = new int[7];
    int okres = 1;

    public AudioSource UI_sounds;


    [Header("Prefab Objects")]
    public AudioClip[] sounds;
    public GameObject neutrine;
    public GameObject protone;
    public GameObject electron;
    public GameObject boom;

    GameObject[] neutrons = new GameObject[max_nucleons];
    GameObject[] protons = new GameObject[max_nucleons];
    GameObject[,] electrons = new GameObject[7, max_nucleons];

    GameObject boomObj = null;

    

    [Header("Informacje o edytorze")]
    public Text protons_txt;
    public Text neutron_txt;
    public Text okres_txt;

    //--------------------SYMBOL INFO--------------------//
    [Header("Informacje o atomie")]
    public Text symbol_a;
    public Text name_a;
    public Text info_a;



    [Header("Ustawienia")]
    bool booming = false;
    public float boomingTime = 4.0f;
    public float electron_speed = 1.0f;


    void Start()
    {
        for (int i=0;i<7;i++)
        {
            num_of_electrons[i] = 0;
        }

        if (atomsContainer == null)
        {
            atomsContainer = new AtomsContainer();
            atomsContainer.loadJSONFile();
        }
    }

    // Update is called once per frame
    public void Play(int i)
    {
        UI_sounds.clip = sounds[i];
        UI_sounds.Play();
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, electron_speed, 0) * Time.deltaTime);
        protons_txt.text = "Liczba protonów: " + num_of_protons;
        neutron_txt.text = "Liczba neutronów: " + num_of_neutrons;

        if(booming)
        {
            for(int i=0;i<7;i++)
            {
                for(int j=0;j<max_nucleons;j++)
                {
                    if(electrons[i,j]!=null)
                        electrons[i,j].transform.Translate(0, 0, -Time.deltaTime*3);
                }
            }
            boomingTime -= Time.deltaTime;
            if(boomingTime > 3.0f)
            {
                Light li = boomObj.GetComponentInChildren<Light>();
                li.intensity += Time.deltaTime * 10000000000;
            }else
            {
                if(boomingTime < 0.0f)
                {
                    boomingTime = 4.0f;
                    GameObject.Destroy(boomObj);
                    booming = false;
                    deleteAll();
                }else
                {
                    Light li = boomObj.GetComponentInChildren<Light>();
                    li.intensity -= Time.deltaTime * 3000000000;
                }
                
            }
               
        }
    }

    public void addNeutine()
    {
        if (num_of_neutrons > max_nucleons-1)
            return;
        GameObject go = GameObject.Instantiate(neutrine, new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 4), Quaternion.identity);
        num_of_neutrons++;
        neutrons[num_of_neutrons - 1] = go;
        Debug.Log(num_of_neutrons);
        Play(0);
    }

    public void addProtone()
    {
        if (num_of_protons > max_nucleons-1)
            return;
        GameObject go = GameObject.Instantiate(protone,new Vector3(Random.Range(-0.5f,0.5f),Random.Range(-0.5f,0.5f), 4),Quaternion.identity);
        
        num_of_protons++;
        protons[num_of_protons - 1] = go;
        Debug.Log(num_of_protons);
        Play(0);
    }

    public void deleteNeutrine()
    {
        if(num_of_neutrons > 0)
        {
            num_of_neutrons--;
            GameObject.Destroy(neutrons[num_of_neutrons]);
            Play(0);
        }
        if (num_of_neutrons == 0)
            deleteAllElectr();
    }

    public void deleteProtone()
    {
        if (num_of_protons > 0)
        {
            num_of_protons--;
            GameObject.Destroy(protons[num_of_protons]);
            Play(0);
        }
        if (num_of_protons == 0)
            deleteAllElectr();
    }

    public void pickOkres(float k)
    {
        
        okres = (int)k;
        okres_txt.text = "Okres: " + (int)k;
    }

    public void addElectron()
    {
        if (num_of_protons == 0)
            return;
        GameObject go = GameObject.Instantiate(electron);
        go.transform.parent = this.transform;
        go.transform.position = new Vector3(0,0,5 + okres * 4);
        electrons[okres - 1, num_of_electrons[okres - 1]] = go;
        num_of_electrons[okres - 1]++;
        Play(0);

    }

    public void AdjustSpeed(float k)
    {
        electron_speed = 400 * k;
    }

    public void Create()
    {
        AtomInfo inf = atomsContainer.getAtom(num_of_protons, num_of_neutrons, num_of_electrons);
        if (inf!=null)
        {
            symbol_a.text = inf.symbol;
            name_a.text = inf.name;
            info_a.text = inf.info;
            return;
        }
        symbol_a.text = "";
        name_a.text = "";
        info_a.text = "";
        Bomb();
        Play(0);
    }

    public void Bomb()
    {
        boomObj = GameObject.Instantiate(boom);
        booming = true;
    }

    void deleteAll()
    {
        int h = num_of_protons;
        int k = num_of_neutrons;

        for(int i=0;i<h;i++)
        {
            deleteProtone();
        }
        for (int i = 0; i < k; i++)
        {
            deleteNeutrine();
        }
        deleteAllElectr();

        Debug.Log("BOOOOM!!! - " + num_of_protons + ":::" + num_of_neutrons + ":::" + num_of_electrons);
        
    }

    public void deleteAllElectr()
    {
        for(int i=0;i<7;i++)
        {
            for(int j=0;j<max_nucleons;j++)
            {
                if(electrons[i,j] != null)
                {
                    GameObject.Destroy(electrons[i,j]);
                }
            }
            num_of_electrons[i] = 0;
        }
        Play(0);
    }

    public void switchScene()
    {
        Play(0);
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    
}
