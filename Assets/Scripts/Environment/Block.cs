using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Block : MonoBehaviour
{
    public enum BlockType
    {
        Wall,
        Floor,
        Start,
        Goal
    }

    public BlockType cubeType = BlockType.Floor;

    private void OnValidate()
    {
        switch (cubeType)
        {
            case BlockType.Wall:
                this.gameObject.tag = "wall";
                this.gameObject.name = "Wall";
                this.transform.localScale = new Vector3(1, 3, 1);
                this.transform.position = new Vector3(this.transform.position.x, 1.5f, this.transform.position.z);
                break;
            case BlockType.Floor:                
                this.gameObject.tag = "floor";
                this.gameObject.name = "Floor";
                this.transform.localScale = new Vector3(1, 1, 1);
                this.transform.position = new Vector3(this.transform.position.x, 0, this.transform.position.z);
                break;
            case BlockType.Start:
                this.gameObject.tag = "start";
                this.gameObject.name = "Start";
                this.transform.localScale = new Vector3(1, 1, 1);
                this.transform.position = new Vector3(this.transform.position.x, 0, this.transform.position.z);
                break;
            case BlockType.Goal:
                this.gameObject.tag = "goal";
                this.gameObject.name = "Goal";
                this.transform.localScale = new Vector3(1, 1, 1);
                this.transform.position = new Vector3(this.transform.position.x, 0, this.transform.position.z);
                break;
        }
    }
}
