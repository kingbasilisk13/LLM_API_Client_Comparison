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
    private CharacterController controller;

    public InputField subjectField;
    public string[] ActionList;

    private float speed = 1f;

    DateTime StartTime;
    TimeSpan deltaTime;
    string Input;

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
    
    void Start()
    {
        subjectField.onSubmit.AddListener(onSubjectFieldSubmit);
        controller = gameObject.AddComponent<CharacterController>();
    }
    public void onSubjectFieldSubmit(string message)
    {
        Input = message;
        StartTime = System.DateTime.Now;
        HuggingFaceAPI.SentenceSimilarity(message, FindValues, Error, ActionList);
    }

    private void MoveNpc(Vector3 direction)
    {
        controller.Move(direction * Time.deltaTime * speed);

        if (direction != Vector3.zero)
        {
            gameObject.transform.forward = direction;
        }
    }
    void Update()
    {
        switch (state)
        {
            default:
            case State.Idle:
                break;

            case State.Up:
                MoveNpc(new Vector3(0, 0, 1));
                break;

            case State.Down:
                MoveNpc(new Vector3(0, 0, -1));
                transform.position += new Vector3(0, 0, -1) * speed * Time.deltaTime;
                break;

            case State.Left:
                MoveNpc(new Vector3(-1, 0, 0));
                break;

            case State.Right:
                MoveNpc(new Vector3(1, 0, 0));
                break;

            case State.Stop:
                state = State.Idle;
                break;

            case State.Confused:
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
        deltaTime = System.DateTime.Now - StartTime;
        
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
        Debug.Log($"Response time : {deltaTime.Seconds} seconds.");
        Debug.Log($"INPUT : {Input} | STATE : {state}");
    }
}
