using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AgentAI : MonoBehaviour
{
    [Space(5)]
    public bool rayDebug = false;
    public bool availableDirDebug = false;
    public bool directionDebug = false;
    public bool genPathDebug = false;
    [HideInInspector]
    public bool isInDeadEnd = false;
    [HideInInspector]
    public bool shouldInitNewAgent = true;

    [HideInInspector]
    public Vector3 lastCrossroadPosition;
    [HideInInspector]
    public List<int> pathHistory;
    [HideInInspector]
    public List<int> _genPath;
    
    private int _numOfStepsTaken = 0;
    private int _lastDirectionTaken = -1;//deciding where you can move
    private List<int> _currentlyAvailableDirections = new List<int>();

    static System.Random rnd = new System.Random();
    private RaySidesAndDown _raycastScript;
    private AgentManager _agentManagerScript;
    private GameObject _agentManagerObj;
    private MoveDirection _moveScript;

    private void OnValidate()
    {
        _raycastScript = this.GetComponent<RaySidesAndDown>();
        Debug.Assert(_raycastScript, "Agent does not have a RaySidesAndDown script!");
        _moveScript = this.GetComponent<MoveDirection>();
        Debug.Assert(_moveScript, "Agent does not have a MoveDirection script!");
        _agentManagerObj = GameObject.FindGameObjectWithTag("agentManager");
        Debug.Assert(_agentManagerObj, "AgentManager object not found!");
        _agentManagerScript = _agentManagerObj.GetComponent<AgentManager>();
        Debug.Assert(_agentManagerScript, "AgentManager script not found!");
    }
    private void Start()
    {
        initAgent();

        if (genPathDebug)
        {
            string message = "GEN PATH in " + _genPath.Count + " steps! Steps taken: ";

            foreach (int dir in _genPath)
            {
                message += dir + ", ";
            }
            Debug.Log(message);
        }
    }
    private void Update()
    {
        initAgent();

        if (!this._raycastScript.StandingOnGoal())
        {
            if (!this.isInDeadEnd)
            {
                UpdateAvailableDirections();
                if (_agentManagerScript.stepByStep)
                {
                    if (Input.GetKeyDown(KeyCode.Space)) aiAlgorithm();
                }
                else
                {
                    aiAlgorithm();
                }
            }
        }
        else
        {
            _agentManagerScript.GoalFound(pathHistory);
        }
    }
    private void aiAlgorithm()
    {
        if (_currentlyAvailableDirections.Count > 1)
        {
            lastCrossroadPosition = this.transform.position;//note before you move from the crossroad

            //Chose random:
            int dirTaken = UnityEngine.Random.Range(0, _currentlyAvailableDirections.Count);
            //Look what is the direction in generationPath at this currentStep + 1 --> because you move, then note that you made a step, so you have to look ahead 1 step than this agent's current number of steps
            if (_genPath != null && _genPath.Count >= _numOfStepsTaken)// && _agentManager.generationPath.Count >= (_numOfStepsTaken + 1)
            {
                //choose path based on probability:
                if (ShouldFollowGenerationPathStepWithProbability(_agentManagerScript.agentProbabilityToFollowGenPath))
                {
                    dirTaken = _genPath[_numOfStepsTaken];//take the same direction as in the previous generation
                    moveAndNoteVariables(dirTaken, true);
                    if (directionDebug) Debug.Log("More directions to go GenerationPath: " + dirTaken);
                }
                else
                {
                    if (directionDebug) Debug.Log("1 More directions to go Random: " + _currentlyAvailableDirections[dirTaken]);
                    moveAndNoteVariables(_currentlyAvailableDirections[dirTaken], true);
                }
            }
            else
            {
                if (directionDebug) Debug.Log("2 More directions to go Random: " + _currentlyAvailableDirections[dirTaken]);
                moveAndNoteVariables(_currentlyAvailableDirections[dirTaken], true);
            }
            if (directionDebug) Debug.Log("Steps taken = " + _numOfStepsTaken);
        }
        else if (_currentlyAvailableDirections.Count == 1)
        {
            if (directionDebug) Debug.Log("Just one direction to go: " + _currentlyAvailableDirections[0]);
            moveAndNoteVariables(_currentlyAvailableDirections[0], false);
            if (directionDebug) Debug.Log("Steps taken = " + _numOfStepsTaken);
        }
        else if (_currentlyAvailableDirections.Count <= 0)
        {
            if (directionDebug) Debug.Log("No direction to go");
            isInDeadEnd = true;
            _agentManagerScript.AgentAtDeadEnd(lastCrossroadPosition, pathHistory);
        }
    }
    private void UpdateAvailableDirections()
    {
        //reset list
        if (_currentlyAvailableDirections.Count > 0) _currentlyAvailableDirections.Clear();

        //Find for each direction
        if (_raycastScript.CanGoLeft())
        {
            if (GetOppositeOfDirection(_lastDirectionTaken) != (int)DirectionsEnum.directions.Left)
            {
                /*add this direction (as a number) as available direction to choose from*/
                _currentlyAvailableDirections.Add((int)DirectionsEnum.directions.Left);
            }
        }

        if (_raycastScript.CanGoRight())
        {
            if (GetOppositeOfDirection(_lastDirectionTaken) != (int)DirectionsEnum.directions.Right)
            {
                _currentlyAvailableDirections.Add((int)DirectionsEnum.directions.Right);
            }
        }

        if (_raycastScript.CanGoForward())
        {
            if (GetOppositeOfDirection(_lastDirectionTaken) != (int)DirectionsEnum.directions.Forward)
            {
                _currentlyAvailableDirections.Add((int)DirectionsEnum.directions.Forward);
            }
        }

        if (_raycastScript.CanGoBack())
        {
            if (GetOppositeOfDirection(_lastDirectionTaken) != (int)DirectionsEnum.directions.Back)
            {
                _currentlyAvailableDirections.Add((int)DirectionsEnum.directions.Back);
            }
        }

        //print debug info
        if (availableDirDebug)
        {
            if (_currentlyAvailableDirections.Count == 3)
            {
                Debug.Log($"Currently available directions count: {_currentlyAvailableDirections.Count} >>> {_currentlyAvailableDirections[0]}, {_currentlyAvailableDirections[1]}, {_currentlyAvailableDirections[2]}");
            }
            else if (_currentlyAvailableDirections.Count == 2)
            {
                Debug.Log($"Currently available directions count: {_currentlyAvailableDirections.Count} >>> {_currentlyAvailableDirections[0]}, {_currentlyAvailableDirections[1]}");
            }
            else if (_currentlyAvailableDirections.Count == 1)
            {
                Debug.Log($"Currently available directions count: {_currentlyAvailableDirections.Count} >>> {_currentlyAvailableDirections[0]}");
            }
        }
    }
    private void moveAndNoteVariables(int pMovingInDirection, bool pNoteLastCrossroad)
    {
        _numOfStepsTaken++;
        _moveScript.MoveInDirection(pMovingInDirection);
        _lastDirectionTaken = pMovingInDirection;
        pathHistory.Add(pMovingInDirection);

        if (pNoteLastCrossroad) lastCrossroadPosition = this.transform.position;
    }
    private int GetOppositeOfDirection(int pDir)
    {
        //Left & Right
        if (pDir == 0)
        {
            return 1;
        }
        else
        if (pDir == 1)
        {
            return 0;
        }
        else
        //Forward & Back
        if (pDir == 2)
        {
            return 3;
        }
        else
        if (pDir == 3)
        {
            return 2;
        }
        else
        {
            return -1;
        }
    }
    /// <summary>
    /// Returns true if a step from generation path was chosen based on given probability.
    /// Returns false if the step from generation path was not chosen.
    /// </summary>
    private bool ShouldFollowGenerationPathStepWithProbability(float pPercentage)
    {
        double rnd = AgentAI.rnd.Next(0, 100);
        if (rnd <= pPercentage)
        {
            Debug.Log("Following gen path.");
            return true;
        }

        Debug.Log("NOT Following gen path.");
        return false;
    }
    private void initAgent()
    {
        if (shouldInitNewAgent)
        {
            if (_genPath != null)
            {
                _genPath.Clear();
                _genPath = _agentManagerObj.GetComponent<AgentManager>().generationPath;
            }
            
            if (pathHistory != null) pathHistory.Clear();

            Debug.Assert(_genPath == null, "GenPath is null");
            _lastDirectionTaken = -1;
            _numOfStepsTaken = 0;
            isInDeadEnd = false;

            shouldInitNewAgent = false;
        }
    }
}
