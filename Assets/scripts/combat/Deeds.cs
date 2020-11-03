﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.LoveHate;


public class Deeds : MonoBehaviour {

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void Disappointed(GameObject target)
    {
        GetComponent<DeedReporter>().ReportDeed("disappointed", target.GetComponent<FactionMember>());

        float affinity = Judgement(target);
        target.GetComponent<FactionMember>().ModifyPersonalAffinity(0, -affinity);
   
    }

    public virtual void Flatter(GameObject target)
    {
        GetComponent<DeedReporter>().ReportDeed("flatter", target.GetComponent<FactionMember>());
        float affinity = Judgement(target);
        target.GetComponent<FactionMember>().ModifyPersonalAffinity(0, affinity);
    }

    public virtual void Insult(GameObject target)
    {
        GetComponent<DeedReporter>().ReportDeed("insult", target.GetComponent<FactionMember>());
        float affinity = Judgement(target);

        if(target.name == "Master of Strings" || target.name == "Chace")
        {
            target.GetComponent<FactionMember>().ModifyPersonalAffinity(0, affinity);

        } else
        {
            target.GetComponent<FactionMember>().ModifyPersonalAffinity(0, -affinity);
        } 
    }


    /// <summary>
    /// Negotiations - type of negotiation sets relationship trait)
    /// </summary>
    /// <param name="target"></param>
    /// <param name="action"></param>
    public virtual void Negotiate(GameObject target, float scale)
    {
        GameObject player = GameObject.Find("Player");
        var factionID = player.GetComponent<FactionMember>().factionManager.GetFactionID(target.name);
        float newRelationshipNumber = player.GetComponent<FactionMember>().factionManager.GetFaction(factionID).GetPersonalRelationshipTrait(0, 0) + scale;
        player.GetComponent<FactionMember>().factionManager.factionDatabase.SetPersonalRelationshipTrait(factionID, 0, 0, newRelationshipNumber);
    }

    /// <summary>
    /// Judgement tests overall alignment and creates affinity bonus based on that
    /// </summary>
    /// <param name="target"></param>
    /// <returns>Affinity score</returns>
    private static float Judgement(GameObject target)
    {
        float[] traits = new float[] { 0, 1, 2, 3, 4, 5, 6 };

        var affinity = target.GetComponent<FactionMember>().DefaultGetTraitAlignment(traits);
        if (affinity < 1)
        {
            affinity *= 10000;
        }

        return affinity;
    }
}
