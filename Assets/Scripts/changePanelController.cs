using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using toolbox;

public class changePanelController : MonoBehaviour
{

    [Header("General Parameters")]
    public Text buttonText;
    public bool isExpanded;

    public Vector3 hidden;
    public Vector3 expanded;

    public GameObject playerCountry; //unassigned in unity editor
    private CountryController playerCountry_script;
    public GameObject countries;
    public EventController eventScript;
    bool setup = false;

    [Header("Primary Dropdown Menus")]
    public TMP_Dropdown ddmenu_legistlativeChange;
    [Header("Secondary Dropdown Menus")]
    public GameObject secondaryDropdownMenus;
    public TMP_Dropdown ddmenu_electoralChange;
    public TMP_Dropdown ddmenu_legislationChange;
    public TMP_Dropdown ddmenu_partiesChange;
    [Header("Tertiary Dropdown Menus")]
    public GameObject tertiaryDropdownMenus;
    public TMP_Dropdown ddmenu_electoralMethod;
    public TMP_Dropdown ddmenu_votingAge;
    public TMP_Dropdown ddmenu_legislativeSystem;
    public TMP_Dropdown ddmenu_termDuration;
    public TMP_Dropdown ddmenu_representation;
    public TMP_Dropdown ddmenu_outlawParty;
    public TMP_Dropdown ddmenu_legaliseParty;

    [Header("Error Messages")]
    public GameObject incompleteError;
    public GameObject inEffectError;

    public void movePanel()
    {
        if (!isExpanded)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition3D = expanded;
            buttonText.text = ">>";
            isExpanded = true;
        }
        else if (isExpanded)
        {
            gameObject.GetComponent<RectTransform>().anchoredPosition3D = hidden;
            buttonText.text = "<<";
            isExpanded = false;
        }
    }

    public void getPlayerCountry()
    {
        foreach (Transform child in countries.transform)
        {
            if (child.gameObject.GetComponent<CountryController>().isPlayer)
            {
                playerCountry = child.gameObject;
                playerCountry_script = playerCountry.GetComponent<CountryController>();
                break;
            }
        }
    }

    public void ValidateProposal()
    {
        //check what proposal is
        //check if proposal already in effect

        if (ddmenu_electoralMethod.transform.parent.gameObject.activeInHierarchy)
        {
            string proposal = ddmenu_electoralMethod.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            electoralSystem voting = playerCountry_script.LegislativeChambers[0].GetComponent<LegislatureController>().electionType;
            if (proposal == "None") displayIncompleteProposalMessage();
            else
            {
                if ((proposal== "First Past the Post" && voting== electoralSystem.FPTP) || (proposal == "Instant-runoff Voting" && voting == electoralSystem.IRV))
                {
                    displayProposalAlreadyInEffectMessage();
                }
                else
                {
                    proposeChange(ddmenu_electoralMethod.transform.parent.gameObject.name, proposal);
                }
            }
        }
        else if (ddmenu_votingAge.transform.parent.gameObject.activeInHierarchy)
        {
            string proposal = ddmenu_votingAge.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            if (proposal == "None") displayIncompleteProposalMessage();
            else
            {
                int proposedAge = int.Parse(proposal);
                if(proposedAge== playerCountry_script.votingAge)
                {
                    displayProposalAlreadyInEffectMessage();
                }
                else
                {
                    proposeChange(ddmenu_votingAge.transform.parent.gameObject.name, proposal);
                }
            }
        }
        else if (ddmenu_legislativeSystem.transform.parent.gameObject.activeInHierarchy)
        {
            string proposal = ddmenu_legislativeSystem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            if (proposal == "None") displayIncompleteProposalMessage();
            else
            {
                if((proposal=="Unicameral" && playerCountry_script.Legislature== legislativeSystem.unicameral) || (proposal == "Bicameral" && playerCountry_script.Legislature == legislativeSystem.bicameral))
                {
                    displayProposalAlreadyInEffectMessage();
                }
                else
                {
                    proposeChange(ddmenu_legislativeSystem.transform.parent.gameObject.name, proposal);
                }
            }
        }
        else if (ddmenu_termDuration.transform.parent.gameObject.activeInHierarchy)
        {
            string proposal = ddmenu_termDuration.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            if (proposal == "None") displayIncompleteProposalMessage();
            else
            {
                int proposedTerm = int.Parse(proposal);
                int term = playerCountry_script.LegislativeChambers[0].GetComponent<LegislatureController>().electionTime;
                if(proposedTerm==term)
                {
                    displayProposalAlreadyInEffectMessage();
                }
                else
                {
                    proposeChange(ddmenu_termDuration.transform.parent.gameObject.name, proposal);
                }
            }
        }
        else if (ddmenu_representation.transform.parent.gameObject.activeInHierarchy)
        {
            string proposal = ddmenu_representation.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            if (proposal == "None") displayIncompleteProposalMessage();
            else
            {
                if ((proposal == "Delegate" && playerCountry_script.LegislativeChambers[0].GetComponent<LegislatureController>().Representative == representativeSystem.Delegate) || (proposal == "Trustee" && playerCountry_script.LegislativeChambers[0].GetComponent<LegislatureController>().Representative == representativeSystem.Trustee))
                {
                    displayProposalAlreadyInEffectMessage();
                }
                else
                {
                    proposeChange(ddmenu_representation.transform.parent.gameObject.name, proposal);
                }
            }
        }
        else if (ddmenu_outlawParty.transform.parent.gameObject.activeInHierarchy)
        {
            string proposal = ddmenu_outlawParty.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            if (proposal == "None") displayIncompleteProposalMessage();
            else
            {
                List<GameObject> parties = playerCountry_script.PoliticalParties;
                int c = parties.Count;
                bool foundParty = false;
                for(int x =0; x<c;x++)
                {
                    if(parties[x].GetComponent<PartyController>().partyName== (proposal+" Party"))
                    {
                        foundParty = true;
                        displayProposalAlreadyInEffectMessage();
                        break;
                    }
                }
                if(!foundParty) proposeChange(ddmenu_outlawParty.transform.parent.gameObject.name, proposal);
            }
        }
        else if (ddmenu_legaliseParty.transform.parent.gameObject.activeInHierarchy)
        {
            string proposal = ddmenu_legaliseParty.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
            if (proposal == "None") displayIncompleteProposalMessage();
            else
            {
                List<GameObject> bannedParties = playerCountry_script.BannedPoliticalParties;
                int c = bannedParties.Count;
                bool foundParty = false;
                for (int x = 0; x < c; x++)
                {
                    if (bannedParties[x].GetComponent<PartyController>().partyName == (proposal + " Party"))
                    {
                        foundParty = true;
                        displayProposalAlreadyInEffectMessage();
                        break;
                    }
                }
                if (!foundParty) proposeChange(ddmenu_legaliseParty.transform.parent.gameObject.name, proposal);
            }
        }

        else //proposal is incomplete
        {
            displayIncompleteProposalMessage();
        }
    }

    public void proposeChange(string type, string prop)
    {
        Debug.Log("Propose Change");
        //run function in country controller class to have legislature vote on proposal and implement changes if it passes
        movePanel();
        playerCountry_script.reviewProposal(type, prop);
    }


    public void displayIncompleteProposalMessage()
    {
        //display error message box
        Debug.Log("Proposal Incomplete!");
        incompleteError.SetActive(true);
    }
    public void displayProposalAlreadyInEffectMessage()
    {
        //display error message box
        Debug.Log("Proposal Already in Effect!");
        inEffectError.SetActive(true);
    }

    public void updateDropDowns(TMP_Dropdown DD)
    {
        Debug.Log("DD menu update called");
        string dd_Menu = DD.transform.parent.gameObject.name;
        string dd_option = DD.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
        Debug.Log("Menu: " + dd_Menu);
        Debug.Log("Option: " + dd_option);
        //disableDropDowns();
        switch (dd_Menu)
        {
            case "Legislative Change":
                switch (dd_option)
                {
                    case "None":
                        secondaryDropdownMenus.SetActive(false);
                        tertiaryDropdownMenus.SetActive(false);
                        break;

                    case "Electoral System":
                        secondaryDropdownMenus.SetActive(true);
                        ddmenu_electoralChange.transform.parent.gameObject.SetActive(true);
                        ddmenu_legislationChange.transform.parent.gameObject.SetActive(false);
                        ddmenu_partiesChange.transform.parent.gameObject.SetActive(false);
                        tertiaryDropdownMenus.SetActive(false);
                        break;

                    case "Legislative System":
                        secondaryDropdownMenus.SetActive(true);
                        ddmenu_electoralChange.transform.parent.gameObject.SetActive(false);
                        ddmenu_legislationChange.transform.parent.gameObject.SetActive(true);
                        ddmenu_partiesChange.transform.parent.gameObject.SetActive(false);
                        tertiaryDropdownMenus.SetActive(false);
                        break;

                    case "Political Parties":
                        secondaryDropdownMenus.SetActive(true);
                        ddmenu_electoralChange.transform.parent.gameObject.SetActive(false);
                        ddmenu_legislationChange.transform.parent.gameObject.SetActive(false);
                        ddmenu_partiesChange.transform.parent.gameObject.SetActive(true);
                        tertiaryDropdownMenus.SetActive(false);
                        break;
                }
                break;

            case "Electoral Change":
                switch (dd_option)
                {
                    case "None":
                        secondaryDropdownMenus.SetActive(true);
                        tertiaryDropdownMenus.SetActive(false);
                        break;

                    case "Electoral Method":
                        secondaryDropdownMenus.SetActive(true);
                        tertiaryDropdownMenus.SetActive(true);
                        ddmenu_electoralMethod.transform.parent.gameObject.SetActive(true);
                        ddmenu_votingAge.transform.parent.gameObject.SetActive(false);
                        ddmenu_legislativeSystem.transform.parent.gameObject.SetActive(false);
                        ddmenu_termDuration.transform.parent.gameObject.SetActive(false);
                        ddmenu_representation.transform.parent.gameObject.SetActive(false);
                        ddmenu_outlawParty.transform.parent.gameObject.SetActive(false);
                        ddmenu_legaliseParty.transform.parent.gameObject.SetActive(false);
                        break;

                    case "Voting Age":
                        secondaryDropdownMenus.SetActive(true);
                        tertiaryDropdownMenus.SetActive(true);
                        ddmenu_electoralMethod.transform.parent.gameObject.SetActive(false);
                        ddmenu_votingAge.transform.parent.gameObject.SetActive(true);
                        ddmenu_legislativeSystem.transform.parent.gameObject.SetActive(false);
                        ddmenu_termDuration.transform.parent.gameObject.SetActive(false);
                        ddmenu_representation.transform.parent.gameObject.SetActive(false);
                        ddmenu_outlawParty.transform.parent.gameObject.SetActive(false);
                        ddmenu_legaliseParty.transform.parent.gameObject.SetActive(false);
                        break;
                }
                break;

            case "Legislation Change":
                switch (dd_option)
                {
                    case "None":
                        secondaryDropdownMenus.SetActive(true);
                        tertiaryDropdownMenus.SetActive(false);
                        break;

                    case "Legislative System":
                        secondaryDropdownMenus.SetActive(true);
                        tertiaryDropdownMenus.SetActive(true);
                        ddmenu_electoralMethod.transform.parent.gameObject.SetActive(false);
                        ddmenu_votingAge.transform.parent.gameObject.SetActive(false);
                        ddmenu_legislativeSystem.transform.parent.gameObject.SetActive(true);
                        ddmenu_termDuration.transform.parent.gameObject.SetActive(false);
                        ddmenu_representation.transform.parent.gameObject.SetActive(false);
                        ddmenu_outlawParty.transform.parent.gameObject.SetActive(false);
                        ddmenu_legaliseParty.transform.parent.gameObject.SetActive(false);
                        break;

                    case "Term Duration":
                        secondaryDropdownMenus.SetActive(true);
                        tertiaryDropdownMenus.SetActive(true);
                        ddmenu_electoralMethod.transform.parent.gameObject.SetActive(false);
                        ddmenu_votingAge.transform.parent.gameObject.SetActive(false);
                        ddmenu_legislativeSystem.transform.parent.gameObject.SetActive(false);
                        ddmenu_termDuration.transform.parent.gameObject.SetActive(true);
                        ddmenu_representation.transform.parent.gameObject.SetActive(false);
                        ddmenu_outlawParty.transform.parent.gameObject.SetActive(false);
                        ddmenu_legaliseParty.transform.parent.gameObject.SetActive(false);
                        break;

                    case "Representation":
                        secondaryDropdownMenus.SetActive(true);
                        tertiaryDropdownMenus.SetActive(true);
                        ddmenu_electoralMethod.transform.parent.gameObject.SetActive(false);
                        ddmenu_votingAge.transform.parent.gameObject.SetActive(false);
                        ddmenu_legislativeSystem.transform.parent.gameObject.SetActive(false);
                        ddmenu_termDuration.transform.parent.gameObject.SetActive(false);
                        ddmenu_representation.transform.parent.gameObject.SetActive(true);
                        ddmenu_outlawParty.transform.parent.gameObject.SetActive(false);
                        ddmenu_legaliseParty.transform.parent.gameObject.SetActive(false);
                        break;
                }
                break;

            case "Political Parties":
                switch (dd_option)
                {
                    case "None":
                        secondaryDropdownMenus.SetActive(true);
                        tertiaryDropdownMenus.SetActive(false);
                        break;

                    case "Outlaw Party":
                        secondaryDropdownMenus.SetActive(true);
                        tertiaryDropdownMenus.SetActive(true);
                        ddmenu_electoralMethod.transform.parent.gameObject.SetActive(false);
                        ddmenu_votingAge.transform.parent.gameObject.SetActive(false);
                        ddmenu_legislativeSystem.transform.parent.gameObject.SetActive(false);
                        ddmenu_termDuration.transform.parent.gameObject.SetActive(false);
                        ddmenu_representation.transform.parent.gameObject.SetActive(false);
                        ddmenu_outlawParty.transform.parent.gameObject.SetActive(true);
                        ddmenu_legaliseParty.transform.parent.gameObject.SetActive(false);
                        break;

                    case "Legalise Party":
                        secondaryDropdownMenus.SetActive(true);
                        tertiaryDropdownMenus.SetActive(true);
                        ddmenu_electoralMethod.transform.parent.gameObject.SetActive(false);
                        ddmenu_votingAge.transform.parent.gameObject.SetActive(false);
                        ddmenu_legislativeSystem.transform.parent.gameObject.SetActive(false);
                        ddmenu_termDuration.transform.parent.gameObject.SetActive(false);
                        ddmenu_representation.transform.parent.gameObject.SetActive(false);
                        ddmenu_outlawParty.transform.parent.gameObject.SetActive(false);
                        ddmenu_legaliseParty.transform.parent.gameObject.SetActive(true);
                        break;
                }
                break;
            /*
            case "Electoral Method":

                break;

            case "Voting Age":

                break;

            case "Legislative System":

                break;

            case "Term Duration":

                break;

            case "Representation":

                break;

            case "Outlaw Party":

                break;

            case "Legalise Party":

                break;*/
        }
    }

    void disableDropDowns()
    {
        ddmenu_electoralChange.transform.parent.gameObject.SetActive(false);
        ddmenu_legislationChange.transform.parent.gameObject.SetActive(false);
        ddmenu_partiesChange.transform.parent.gameObject.SetActive(false);
        ddmenu_electoralMethod.transform.parent.gameObject.SetActive(false);
        ddmenu_votingAge.transform.parent.gameObject.SetActive(false);
        ddmenu_legislativeSystem.transform.parent.gameObject.SetActive(false);
        ddmenu_termDuration.transform.parent.gameObject.SetActive(false);
        ddmenu_representation.transform.parent.gameObject.SetActive(false);
        ddmenu_outlawParty.transform.parent.gameObject.SetActive(false);
        ddmenu_legaliseParty.transform.parent.gameObject.SetActive(false);
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
