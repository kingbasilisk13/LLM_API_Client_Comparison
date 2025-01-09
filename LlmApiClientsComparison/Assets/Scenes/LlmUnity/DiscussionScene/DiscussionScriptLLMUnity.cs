using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLMUnity;
using UnityEngine.UI;
using LLMUnitySamples;
using System;
using UnityEditor.VersionControl;


public class DiscussionInteraction
{
    public bool StopTalking = false;

    string Subject;
    
    Text AIText;
    LLMCharacter llmCharacter;
    DiscussionInteraction discussionPartner;
    string response;

    DateTime Start;
    
    public DiscussionInteraction(Text AIText, LLMCharacter llmCharacter)
    {
        this.AIText = AIText;
        this.llmCharacter = llmCharacter;
    }

    public void SetPartner(DiscussionInteraction partner) 
    {
        discussionPartner = partner;
    }

    public void SetSubject(string subject) 
    {
        Subject = subject;
    }

    public void StartDiscussion() 
    {
        Start = System.DateTime.Now;
        AIText.text = "...";
        _ = llmCharacter.Chat(
            $"What do you think of {Subject}.",
            SetAIText,
            AIReplyComplete);
    }

    public void SetAIText(string text)
    {
        AIText.text = text;
        response = text;
    }

    public void AIReplyComplete()
    {
        TimeSpan deltaTime = System.DateTime.Now - Start;
        Debug.Log($"Response time : {deltaTime.Seconds} seconds.");
        Debug.Log($"{llmCharacter.AIName} : {response}");

        if (StopTalking) return;

        discussionPartner.ReceiveReply(response);
    }

    public void ReceiveReply(string PartnersResponse) 
    {
        Start = System.DateTime.Now;
        AIText.text = "...";
        _ = llmCharacter.Chat(
            PartnersResponse,
            SetAIText,
            AIReplyComplete);
    }
}

public class DiscussionScriptLLMUnity : MonoBehaviour
{
    public InputField subjectField;


    public LLMCharacter llmCharacter1;
    public Text AIText1;
    DiscussionInteraction interaction1;

    public LLMCharacter llmCharacter2;
    public Text AIText2;
    DiscussionInteraction interaction2;

    void Start()
    {
        subjectField.onSubmit.AddListener(onSubjectFieldSubmit);


        interaction1 = new DiscussionInteraction(AIText1, llmCharacter1);
        interaction2 = new DiscussionInteraction(AIText2, llmCharacter2);

        interaction1.SetPartner(interaction2);
        interaction2.SetPartner(interaction1);
    }

    public void onSubjectFieldSubmit(string message)
    {
        subjectField.interactable = false;

        interaction1.StopTalking = false;
        interaction2.StopTalking = false;

        interaction1.SetSubject(message);
        interaction2.SetSubject(message);
        interaction1.StartDiscussion();
    }

    public void CancelRequests()
    {
        llmCharacter1.CancelRequests();
        llmCharacter2.CancelRequests();
        interaction1.StopTalking = true;
        interaction2.StopTalking = true;

        subjectField.interactable = true;
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }

    bool onValidateWarning = true;
    void OnValidate()
    {
        if (onValidateWarning && !llmCharacter1.remote && llmCharacter1.llm != null && llmCharacter1.llm.model == "")
        {
            Debug.LogWarning($"Please select a model in the {llmCharacter1.llm.gameObject.name} GameObject!");
            onValidateWarning = false;
        }
    }

    private void OnApplicationQuit()
    {
        llmCharacter1.CancelRequests();
        llmCharacter2.CancelRequests();
        interaction1.StopTalking = true;
        interaction2.StopTalking = true;
    }
}
