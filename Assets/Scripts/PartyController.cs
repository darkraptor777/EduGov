using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toolbox;

public class PartyController : MonoBehaviour
{
    public string partyName; //party's name

    //party leader
    public GameObject leader;
    //party members are child objects
    public List<GameObject> members = new List<GameObject>();
    public platform Platform; 

    int supporters; //number of citizens who support the party
    public float popularity_LowerLegislature; //used for unified legislature also
    public float popularity_UpperLegislature;

    public float popularity; //mean of upper and lower popularity

    public int tallyLower, tallyUpper;

    vote policyPosition;

    public void setup(platform p)
    {
        Platform = p;
        setName();
        //calcPopularity();
    }

    public void decidePolicyPosition(string type, string prop)
    {
        if (type == "Outlaw Party" && (prop + " Party") == partyName) { policyPosition = vote.Nay; } //never in favor of outlawing self
        else { policyPosition = (vote)Random.Range(0, 3); }
    }

    public vote getPolicyPosition()
    {
        return policyPosition;
    }

    public void catalogueMembers()
    {
        foreach (Transform child in gameObject.transform)
        {
            members.Add(child.gameObject);
        }
        leader = members[Random.Range(0, members.Count - 1)];
    }

    //this measurement isn't great but it works
    public void calcPopularity()
    {
        CountryController country = transform.parent.parent.gameObject.GetComponent<CountryController>();
        tallyLower = 0;
        tallyUpper = 0;
        foreach (GameObject member in members)
        {
            PoliticianController memberScript = member.GetComponent<PoliticianController>();
            if (memberScript.isElected)
            { 
                if (memberScript.legislature == chamber.Lower || memberScript.legislature == chamber.Unified)
                {
                    tallyLower += 1;
                }
                else if (memberScript.legislature == chamber.Upper)
                {
                    tallyUpper += 1;
                }
            }
        }


        if(country.Legislature == legislativeSystem.unicameral)
        {
            popularity_LowerLegislature = (float)tallyLower / (float)country.LegislativeChambers[0].GetComponent<LegislatureController>().size;
            popularity_UpperLegislature = 0.0f;

            popularity = popularity_LowerLegislature;
            //Debug.Log("LegSize: "+ country.LegislativeChambers[0].GetComponent<LegislatureController>().size);
            //Debug.Log("TallyLower: " + tallyLower);
            //Debug.Log("PopLower: " + popularity_LowerLegislature);
            //Debug.Log(popularity);
        }

        else if (country.Legislature == legislativeSystem.bicameral)
        {
            popularity_LowerLegislature = (float)tallyLower / (float)country.LegislativeChambers[0].GetComponent<LegislatureController>().size;
            popularity_UpperLegislature = (float)tallyUpper / (float)country.LegislativeChambers[1].GetComponent<LegislatureController>().size;

            popularity = (popularity_LowerLegislature + popularity_UpperLegislature) / 2.0f;
            //Debug.Log("LegSize: " + country.LegislativeChambers[0].GetComponent<LegislatureController>().size);
            //Debug.Log("LegSize: " + country.LegislativeChambers[1].GetComponent<LegislatureController>().size);
            //Debug.Log("TallyLower: " + tallyLower);
            //Debug.Log("TallyUpper: " + tallyUpper);
            //Debug.Log("PopLower: " + popularity_LowerLegislature);
            //Debug.Log("PopUpper: " + popularity_UpperLegislature);
            //Debug.Log(popularity);
        }
        
    }

    public void setName()
    {
        string countryName = gameObject.transform.parent.parent.GetComponent<CountryController>().countryName;
        string name = "Unnamed";
        List<string> start;
        List<string> end;
        switch (Platform)
        {
            case platform.Liberalism:
                start = new List<string>() { "Liberal" };
                end = new List<string>() { "Party" };
                name = start[Random.Range(0, start.Count - 1)] + " " + end[Random.Range(0, end.Count - 1)];
                break;
            case platform.Conservatism:
                start = new List<string>() { "Conservative" };
                end = new List<string>() { "Party" };
                name = start[Random.Range(0, start.Count - 1)] + " " + end[Random.Range(0, end.Count - 1)];
                break;
            case platform.Socialism:
                start = new List<string>() { "Socialist" };
                end = new List<string>() { "Party" };
                name = start[Random.Range(0, start.Count - 1)] + " " + end[Random.Range(0, end.Count - 1)];
                break;
            case platform.Nationalism:
                start = new List<string>() { "Nationalist" };//, countryName +" First", countryName + " National" };
                end = new List<string>() { "Party" };
                name = start[Random.Range(0, start.Count - 1)] + " " + end[Random.Range(0, end.Count - 1)];
                break;
            case platform.Fascism:
                start = new List<string>() { "Fascist" };
                end = new List<string>() { "Party" };
                name = start[Random.Range(0, start.Count - 1)] + " " + end[Random.Range(0, end.Count - 1)];
                break;
            case platform.Communism:
                start = new List<string>() { "Communist" };
                end = new List<string>() { "Party" };
                name = start[Random.Range(0, start.Count - 1)] + " " + end[Random.Range(0, end.Count - 1)];
                break;
            case platform.Syndicalism:
                start = new List<string>() { "Syndicalist" };
                end = new List<string>() { "Party" };
                name = start[Random.Range(0, start.Count - 1)] + " " + end[Random.Range(0, end.Count - 1)];
                break;
            case platform.Environmentalism:
                start = new List<string>() { "Enviromentalist" };//, "Green" };
                end = new List<string>() { "Party" };
                name = start[Random.Range(0, start.Count - 1)] + " " + end[Random.Range(0, end.Count - 1)];
                break;
            case platform.Internationalism:
                start = new List<string>() { "Internationalist" };//, "International", "Co-operation" };
                end = new List<string>() { "Party" };
                name = start[Random.Range(0, start.Count - 1)] + " " + end[Random.Range(0, end.Count - 1)];
                break;
            case platform.Isolationist:
                start = new List<string>() { "Isolationist" };//, countryName + " Independance" };
                end = new List<string>() { "Party" };
                name = start[Random.Range(0, start.Count - 1)] + " " + end[Random.Range(0, end.Count - 1)];
                break;
            case platform.Progressism:
                start = new List<string>() { "Progressive" };
                end = new List<string>() { "Party" };
                name = start[Random.Range(0, start.Count - 1)] + " " + end[Random.Range(0, end.Count - 1)];
                break;
            case platform.Libertarianism:
                start = new List<string>() { "Libertarian" };
                end = new List<string>() { "Party" };
                name = start[Random.Range(0, start.Count - 1)] + " " + end[Random.Range(0, end.Count - 1)];
                break;
            case platform.SocialDemocratic:
                start = new List<string>() { "Social Democracy" };//, "Social Democratic", "Social Union" };
                end = new List<string>() { "Party" };
                name = start[Random.Range(0, start.Count - 1)] + " " + end[Random.Range(0, end.Count - 1)];
                break;
        }

        gameObject.name = name;
        partyName = name;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
