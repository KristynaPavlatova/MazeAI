using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentManager : MonoBehaviour
{
    [Tooltip("Execute step by step by pressing SPACE.")]
    public bool stepByStep = false;
    [Space(10)]
    public float spawnY = 0.6f;
    public float heightDiff = 0.3f;
    public GameObject agentPrefab;
    [Space(5)]
    public bool genPathDebug = false;
    public bool debugInfo = false;
    [Space(5)]
    [Tooltip("Give percentage. e.g. 85, 5")]
    public float agentProbabilityToFollowGenPath = 100;
    public int agentsInGeneration = 3;
    public List<int> generationPath;
    public int generationNum;
    [Space(10)]
    public Text currentDataText;
    public Text finishedText;

    [SerializeField]
    private GameObject[] agents;
    private int _agentsAtDeadEnd;
    private float _shortestDistanceToGoal;
    private Vector3 _goalPos = Vector3.zero;
    private Vector3 _startPos = Vector3.zero;

    private void Awake()
    {
        _goalPos = GameObject.FindGameObjectWithTag("goal").transform.position;
        _startPos = GameObject.FindGameObjectWithTag("start").transform.position;
        agents = GameObject.FindGameObjectsWithTag("agent");

        initNewGenerationOfAgents(_startPos);
    }
    public void GoalFound(List<int> pPath)
    {
        if (generationPath != null)
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
        

        int stepsNeeded = pPath.Count;

        if(debugInfo) Debug.Log($"Goal found! {pPath.Count}, {generationNum}");
        if (debugInfo) Debug.Log("PRESS 'R' TO RESTART");

        if (Input.GetKeyDown(KeyCode.R))
        {
            generationNum = 0;
            stepsNeeded = 0;
            initNewGenerationOfAgents(_startPos);
        }

        currentDataText.enabled = false;
        finishedText.enabled = true;
        finishedText.text = "GOAL FOUND! Generations: " + generationNum + " Agents: " + agents.Length + " R-RESTART";
    }
    public void AgentAtDeadEnd(Vector3 pLastCrossroad, List<int> pPath)
    {
        CompareCrossroadDistanceFromGoal(pLastCrossroad, pPath);
        _agentsAtDeadEnd++;

        if (_agentsAtDeadEnd == agents.Length)
        {
            if (debugInfo) Debug.Log("ALL AGENTS AT DEAD END: new generation started.");
            initNewGenerationOfAgents(_startPos, pPath);
        }
    }
    private void CompareCrossroadDistanceFromGoal(Vector3 pCrossroad, List<int> pAgentPath)
    {
        float distanceToGoal = (_goalPos - pCrossroad).magnitude;
        if (distanceToGoal < _shortestDistanceToGoal)
        {
            _shortestDistanceToGoal = distanceToGoal;
            
            generationPath.Clear();
            generationPath = pAgentPath;
        }
    }
    private void initNewGenerationOfAgents(Vector3 pPosition, List<int> pNewGenPath = null)
    {
        generationPath = pNewGenPath;
        generationNum++;
        _agentsAtDeadEnd = 0;

        foreach (GameObject agent in agents)
        {
            agent.transform.position = new Vector3(pPosition.x, agent.transform.position.y, pPosition.z);
            agent.GetComponent<AgentAI>().shouldInitNewAgent = true;
        }

        finishedText.enabled = false;
        currentDataText.enabled = true;
        currentDataText.text = "Generation: " +generationNum+ " Agents: " +agents.Length;

        if (genPathDebug)
        {
            if (generationPath != null)
            {
                string message = "Agent manager genPath: " + generationPath.Count + " steps! Steps taken: ";
                foreach (int dir in generationPath)
                {
                    message += dir + ", ";
                }
                Debug.Log(message);
            }
        }
    }
}
