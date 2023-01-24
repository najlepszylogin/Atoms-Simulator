using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



public class AtomInfo
{

    public string name;
    public int num_protons;
    public int num_neutrons;
    public int[] num_elect = new int[7];
    public string info;
    public string symbol;
    public float mass;
    public int gruop;
    public int okres;
    public string character;
    public Color color;
    public bool enabled = false;

    public AtomInfo()
    {

    }

    public AtomInfo(string name, int num_proto, int num_neutro, int[] elect, string info, string symbol, float mass, int group, int okres, string chara, Color color)
    {
        this.name = name;  
        this.num_protons = num_proto;
        this.num_neutrons = num_neutro;
        this.num_elect = elect;
        this.info = info;
        this.symbol = symbol;
        this.mass = mass;
        this.gruop = group;
        this.okres = okres;
        this.character = chara;
        this.color = color;
    }
    
    public void enable()
    {
        this.enabled = true;
    }
}


public class AtomsContainer
{
    public AtomInfo[] atoms = new AtomInfo[294];

    public void loadJSONFile()
    {
        string[] json = File.ReadAllLines(Application.dataPath + "/atoms.json");
        for (int i = 0; i < json.Length; i++)
        {
            if (json[i] == "")
                return;
            atoms[i] = JsonUtility.FromJson<AtomInfo>(json[i]);
        }
    }

    public void saveAtomsToJSON()
    {
        string json = "";
        for (int i = 0; i < atoms.Length; i++)
        {
            json += JsonUtility.ToJson(atoms[i]) + "\n";
        }
        File.WriteAllText(Application.dataPath + "/atoms.json", json);
    }

    public AtomInfo getAtom(int p, int n, int[] e)
    {
        if (p < 1)
            return null;
        p--;
        Debug.Log("P === " + p);
        Debug.Log("n === " + n + " ||| " + atoms[p].num_neutrons);
        Debug.Log("elect: " + e[0] + " ||| " + atoms[p].num_elect[0]);
        if (p > 293 || p < 0)
            return null;
        if (n == atoms[p].num_neutrons && checkElectrons(e,atoms[p]))
        {
            atoms[p].enable();
            return atoms[p];
        }
            
        else return null;
    }

    bool checkElectrons(int[] e, AtomInfo inf)
    {
        for(int i=0;i<7;i++)
        {
            if (e[i] != inf.num_elect[i])
                return false;
        }
        return true;
    }
}