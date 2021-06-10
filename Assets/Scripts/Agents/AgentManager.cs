using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    [Tooltip("Give percentage. e.g. 85, 5")]
    public float agentProbabilityToFollowGenPath = 100;
    public int agentsInGeneration = 2;
    public List<int> generationPath;
    public List<GameObject> agents;
    
    private int _agentsAtDeadEnd;
    private Vector3 _goalPos;

    private void Awake()
    {
        generationPath = new List<int>() { 3, 3, 3, 1, 1, 2 };
        foreach (GameObject agent in agents)
        {
            agent.GetComponent<AgentAI>().genPath.Clear();
            agent.GetComponent<AgentAI>().genPath = generationPath;
        }

        

        _goalPos = GameObject.FindGameObjectWithTag("goal").transform.position;
                Debug.Assert(_goalPos == null, "Goal not found! There is no game object with goal tag!");
    }
    public int GetStepInGenerationPath(int pStepNumber)
    {
        return generationPath[pStepNumber];
    }
    public void GoalFound(List<int> pPath)
    {
        string message = "GOAL FOUND in " + pPath.Count + " steps! Steps taken: ";
        int i = 0;
        foreach (int dir in pPath)
        {
            i++;
            message += i + " = " + dir + ", ";
        }
        Debug.Log(message);
    }
    public void AgentAtDeadEnd(Vector3 pLastCrossroad, int pNumOfStepsUntilLastCrossroad, List<int> pPath)
    {
        //CompareCrossroadDistance()

        _agentsAtDeadEnd++;
        
        if (_agentsAtDeadEnd == agentsInGeneration)
        {
            //all agents at dead end
            //restart generation:
            //generationCount ++;
        }
    }
    private void CompareCrossroadDistanceFromGoal()
    {

    }
}
