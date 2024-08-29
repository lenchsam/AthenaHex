using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public Team playerTeam;
    public void EndTurn()
    {
        switch (playerTeam)
        {
            case Team.Team1:
                playerTeam = Team.Team2;
                break;
            case Team.Team2:
                playerTeam = Team.Team3;
                break;

            case Team.Team3:
                playerTeam = Team.Team4;
                break;

            case Team.Team4:
                playerTeam = Team.Team1;
                break;
        }
    }
}
