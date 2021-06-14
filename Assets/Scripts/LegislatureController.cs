using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using toolbox;

public class LegislatureController : MonoBehaviour
{
    public bool elective; //are members of the legislature elected?

    public int electionTime; //time (in years) inbetween elections for the legislature

    public int size; //how many members are in the legislature?

    public int maxSize; //what is the maximum number of members in the legislature? (-1 corresponds to no limit)

    
    public representativeSystem Representative; //are members of the legislature delegates or trustees?

    public electoralSystem electionType;

    public List<GameObject> members = new List<GameObject>();

    public chamber Chamber;


    //requirements
    public int minAge; //minimum member age 
    public int maxAge; //minimum member age
    public List<characteristic> characteristics = new List<characteristic>();

    public bool reviewPoposal()
    {
        int total = members.Count;
        int Y = 0;
        int N = 0;
        int Abs = 0;
        for(int x=0; x<total; x++)
        {
            vote V = members[x].GetComponent<PoliticianController>().policyPosition().GetComponent<PartyController>().getPolicyPosition();
            if (V == vote.Nay) N += 1;
            else if (V == vote.Yay) Y += 1;
            else if (V == vote.Abstain) Abs += 1;
        }

        if (Y > N) return true;
        else return false;
    }

    public void setup(chamber C, int constituencyNo, int termTime)
    {
        //basic semi-randomised set up
        minAge = gameObject.transform.parent.parent.gameObject.GetComponent<CountryController>().votingAge;
        maxAge = 90;
        //no required characteristics
        Chamber = C;

        maxSize = constituencyNo;
        size = maxSize;
        electionTime = termTime;
    }

    public void updateSeats(List<GameObject> parties)
    {
        members.Clear();

        for(int x = 0; x<parties.Count;x++)
        {
            PartyController partyScript = parties[x].GetComponent<PartyController>();
            int memberCount = partyScript.members.Count;
            for (int y = 0; y< memberCount; y++)
            {
                if(partyScript.members[y].GetComponent<PoliticianController>().isElected)
                {
                    members.Add(partyScript.members[y]);
                }
            }
        }
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
