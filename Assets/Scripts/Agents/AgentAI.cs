using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class AgentAI : MonoBehaviour
{
    public bool _rayDebug = false;
    public bool _availableDirCountDebug = true;
    //public bool _moveDirDebug = false;
    [HideInInspector]
    public Vector3 lastCrossroadPosition;
    public int lastCrossroadDirectionTaken;
    [HideInInspector]
    public List<int> pathTaken;
    [HideInInspector]
    public bool isInDeadEnd = false;

    private List<int> _currentlyAvailableDirections = new List<int>();
    private int _numOfStepsTaken = 0;
    private int _lastDirectionTaken = -1;
    public List<int> genPath;

    private RaySidesAndDown _raycastScript;
    private AgentManager _agentManagerScript;
    private GameObject _agentManagerObj;
    private MoveDirection _moveScript;
    static System.Random random = new System.Random();

    private void OnValidate()
    {
        _raycastScript = this.GetComponent<RaySidesAndDown>();
                Debug.Assert(_raycastScript, "Agent does not have a RaySidesAndDown script!");
        _moveScript = this.GetComponent<MoveDirection>();
                Debug.Assert(_moveScript, "Agent does not have a MoveDirection script!");
        _agentManagerObj = GameObject.FindGameObjectWithTag("agentManager");
             Debug.Assert(_agentManagerObj, "AgentManager object not found!");
        _agentManagerScript = GameObject.FindGameObjectWithTag("agentManager").GetComponent<AgentManager>();
                Debug.Assert(_agentManagerScript, "AgentManager script not found!");
    }
    private void Awake()
    {
        string message = "GEN PATH in " + genPath.Count + " steps! Steps taken: ";
        
        foreach (int dir in genPath)
        {
            message += dir + ", ";
        }
        Debug.Log(message);
    }
    private void Update()
    {
        if (!_raycastScript.StandingOnGoal())
        {
            if (!isInDeadEnd)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    UpdateAvailableDirections();
                    if (_currentlyAvailableDirections.Count > 1)
                    {
                        lastCrossroadPosition = this.transform.position;//note before you move from the crossroad

                        //Chose random:
                        int dirTaken = UnityEngine.Random.Range(0, _currentlyAvailableDirections.Count);
                        //Look what is the direction in generationPath at this currentStep + 1 --> because you move, then note that you made a step, so you have to look ahead 1 step than this agent's current number of steps
                        if(_agentManagerScript.generationPath != null && genPath.Count >= _numOfStepsTaken)// && _agentManager.generationPath.Count >= (_numOfStepsTaken + 1)
                        {
                            //choose path based on probability:
                            if (ShouldFollowGenerationPathStepWithProbability(_agentManagerScript.agentProbabilityToFollowGenPath))
                            {
                                Debug.Log("gen path at step"+_numOfStepsTaken+ " is " + genPath[_numOfStepsTaken]);
                                dirTaken = genPath[_numOfStepsTaken];//take the same direction as in the previous generation

                                _moveScript.MoveInDirection(dirTaken);
                                _lastDirectionTaken = dirTaken;
                                Debug.Log("More directions to go GenerationPath: " + dirTaken);
                            }
                            else
                            {
                                Debug.Log("1 More directions to go Random: " + _currentlyAvailableDirections[dirTaken]);
                                _moveScript.MoveInDirection(_currentlyAvailableDirections[dirTaken]);
                                _lastDirectionTaken = _currentlyAvailableDirections[dirTaken];
                            }
                        }
                        else
                        {
                            Debug.Log("2 More directions to go Random: " + _currentlyAvailableDirections[dirTaken]);
                            //just choose randomly:
                            _moveScript.MoveInDirection(_currentlyAvailableDirections[dirTaken]);
                            _lastDirectionTaken = _currentlyAvailableDirections[dirTaken];
                        }
                                                
                        
                        _numOfStepsTaken++;
                        Debug.Log("Steps taken = " + _numOfStepsTaken);
                    }
                    else if (_currentlyAvailableDirections.Count == 1)
                    {
                        Debug.Log("Just one direction to go: " + _currentlyAvailableDirections[0]);

                        //Just take the first availbale direction in the list
                        _moveScript.MoveInDirection(_currentlyAvailableDirections[0]);
                        _lastDirectionTaken = _currentlyAvailableDirections[0];
                        pathTaken.Add(_currentlyAvailableDirections[0]);
                        _numOfStepsTaken++;
                        Debug.Log("Steps taken = " + _numOfStepsTaken);
                    }
                    else if (_currentlyAvailableDirections.Count <= 0)
                    {
                        Debug.Log("No direction to go");
                        isInDeadEnd = true;
                        _agentManagerScript.AgentAtDeadEnd(lastCrossroadPosition, _numOfStepsTaken, pathTaken);
                    }
                }
            }
        }
        else
        {
            _agentManagerScript.GoalFound(pathTaken);
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
        if(_currentlyAvailableDirections.Count == 3)
        {
            Debug.Log($"Currently available directions count: {_currentlyAvailableDirections.Count} >>> {_currentlyAvailableDirections[0]}, {_currentlyAvailableDirections[1]}, {_currentlyAvailableDirections[2]}");
        }        
        else if (_currentlyAvailableDirections.Count == 2)
        {
            Debug.Log($"Currently available directions count: {_currentlyAvailableDirections.Count} >>> {_currentlyAvailableDirections[0]}, {_currentlyAvailableDirections[1]}");
        }
        else if(_currentlyAvailableDirections.Count == 1)
        {
            Debug.Log($"Currently available directions count: {_currentlyAvailableDirections.Count} >>> {_currentlyAvailableDirections[0]}");
        }
    }
    private void noteLastCrossroad(int pAvailableDirections)
    {
        if (pAvailableDirections > 1)
        {
            lastCrossroadPosition = this.transform.position;
        }
    }
    private void noteLastCrossroadDirectionTaken(int pDir)
    {
        lastCrossroadDirectionTaken = pDir;
    }
    private int GetOppositeOfDirection(int pDir)
    {
        //Left & Right
        if (pDir == 0)
        {
            return 1;
        }else
        if (pDir == 1)
        {
            return 0;
        }else
        //Forward & Back
        if (pDir == 2)
        {
            return 3;
        }else
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
        double rnd = random.Next(0,100);
        if (rnd <= pPercentage)
        {
            Debug.Log("Following gen path.");
            return true;
        }

        Debug.Log("NOT Following gen path.");
        return false;
    }
}
