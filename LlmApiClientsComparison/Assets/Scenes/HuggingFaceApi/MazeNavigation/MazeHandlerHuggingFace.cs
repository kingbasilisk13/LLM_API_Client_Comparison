using HuggingFace.API;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MazeHandlerHuggingFace : MonoBehaviour
{
    public InputField subjectField;
    public string[] ActionList;


    private float speed = 1f;

    private enum State
    {
        Idle,
        Up,
        Down,
        Left,
        Right,
        Stop,
        Confused
    }
    
    private State state;
    
    // Start is called before the first frame update
    void Start()
    {
        subjectField.onSubmit.AddListener(onSubjectFieldSubmit);
    }
    public void onSubjectFieldSubmit(string message)
    {
        HuggingFaceAPI.SentenceSimilarity(message, FindValues, Error, ActionList);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state) 
        {
            default:
            case State.Idle:
                break;

            case State.Up:
                transform.position += new Vector3(0, 0, 1) * speed * Time.deltaTime;
                break;

            case State.Down:
                transform.position += new Vector3(0, 0, -1) * speed * Time.deltaTime;
                break;

            case State.Left:
                transform.position += new Vector3(-1, 0, 0) * speed * Time.deltaTime;
                break;

            case State.Right:
                transform.position += new Vector3(1, 0, 0) * speed * Time.deltaTime;
                break;

            case State.Stop:
                state = State.Idle;
                break;

        }
    }

    private void Error(string error) 
    {
        Debug.Log(error);
    }

    private void FindValues(float[] list) 
    {
        if (list == null || list.Length == 0)
            throw new ArgumentException("The list cannot be null or empty");

        int highestIndex = 0;
        float highestValue = list[0];

        for (int i = 1; i < list.Length; i++)
        {
            if (list[i] > highestValue)
            {
                highestValue = list[i];
                highestIndex = i;
            }
        }

        Utility(highestValue, highestIndex);
    }


    public void Utility(float maxScore, int maxScoreIndex) 
    {
        if(maxScore < 0.2f) 
        {
            state = State.Confused;
            subjectField.text = "?";
        }
        else 
        {
            string verb = ActionList[maxScoreIndex].ToString();

            state = (State)System.Enum.Parse(typeof(State),verb, true);
            subjectField.text = "";

        }
    }
}
