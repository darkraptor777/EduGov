using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using toolbox;

public class SetupGameUI : MonoBehaviour
{
    public Image CountryIcon;
    public TextMeshProUGUI CountryName;
    public changePanelController proposeChangePanel;
    public List<Sprite> Icons;

    public GameObject GameControlScript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PrepareUI(GameObject countryObject)
    {
        CountryController country = countryObject.GetComponent<CountryController>();
        CountryIcon.sprite = Icons[country.ID];
        CountryName.text = country.countryName;
        proposeChangePanel.getPlayerCountry();
        GameControlScript.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
