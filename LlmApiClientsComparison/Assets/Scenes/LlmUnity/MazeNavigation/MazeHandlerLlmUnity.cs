using LLMUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeHandlerLlmUnity : MonoBehaviour
{
    private CharacterController controller;

    public LLMCharacter llmCharacter;
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
        controller = gameObject.AddComponent<CharacterController>();

    }

    string ConstructPrompt(string message)
    {
        string prompt = "Which of the following choices matches best the input?\n\n";
        prompt += "Input:" + message + "\n\n";
        prompt += "Choices:\n";
        foreach (string stateName in Enum.GetNames(typeof(State))) prompt += $"- {stateName}\n";
        prompt += "\nAnswer directly with the choice";
        return prompt;
    }

    async void onSubjectFieldSubmit(string message)
    {
        
        string verb = await llmCharacter.Chat(ConstructPrompt(message));
        state = (State)System.Enum.Parse(typeof(State), verb, true);
        subjectField.text = "";
    }

    public void SetAIText(string text)
    {
        Debug.Log(text);
    }

    private void MoveNpc(Vector3 direction) 
    {
        controller.Move(direction * Time.deltaTime * speed);

        if (direction != Vector3.zero)
        {
            gameObject.transform.forward = direction;
        }
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

        }
    }
    private void OnApplicationQuit()
    {
        llmCharacter.CancelRequests();
    }
}
