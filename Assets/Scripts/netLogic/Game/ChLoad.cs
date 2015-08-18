using UnityEngine;
using System.Collections;
using netLogic;
using System.Collections.Generic;
using netLogic.Constants;

public class ChLoad : MonoBehaviour {

    public GameObject _chList;
    public GameObject _ch;
    public GameObject _level;
    public GameObject _name;
    public GameObject _class;
    public GameObject _selected;

    GameObject g;
    UIGrid _ugrid;
	// Use this for initialization

    void Awake()
    {
        List<CharacterListExt> chl = Global.GetInstance().GetWSession().GetCharList();
        foreach (CharacterListExt _chl in chl)
        {
            _level.GetComponent<UILabel>().text = _chl.p.Level.ToString();
            _class.GetComponent<UILabel>().text = ((Classes)_chl.p.Class).ToString() ;
            _name.GetComponent<UILabel>().text = _chl.p.Name.ToString();
            _ugrid = _chList.GetComponent<UIGrid>();
            NGUITools.AddChild(_chList, _ch);
        }
    }

	void Start () {
       
        
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
