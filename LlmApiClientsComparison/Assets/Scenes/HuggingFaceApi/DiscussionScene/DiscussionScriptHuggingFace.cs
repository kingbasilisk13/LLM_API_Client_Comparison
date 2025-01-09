using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HuggingFace.API;
using UnityEngine.UIElements;
using System;
using UnityEditor.VersionControl;
using LLMUnity;

public class DiscussionInteractionHuggingFace
{
    string Description;
    string Subject;
    Text AIText;
    DiscussionInteractionHuggingFace discussionPartner;
    string response;
    public bool StopTalking = false;

    string CharacterName;

    DateTime Start;

    public DiscussionInteractionHuggingFace(string description, Text AIText, string characterName)
    {
        this.Description = description;
        this.AIText = AIText;
        CharacterName = characterName;
    }

    public void SetPartner(DiscussionInteractionHuggingFace partner)
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
        string inputText = $"subject = {Subject}." +
            $"{Description}." +
            $"What do you think of this subject?";
        HuggingFaceAPI.TextGeneration(inputText, SetAIText, OnError);
    }

    public void SetAIText(string text)
    {
        AIText.text = text;
        response = text;

        AIReplyComplete();
    }
    void OnError(string error)
    {
        AIText.text = error;
        StopTalking = true;
    }

    public void AIReplyComplete()
    {
        TimeSpan deltaTime = System.DateTime.Now - Start;
        Debug.Log($"Response time : {deltaTime.Seconds} seconds.");
        Debug.Log($"{CharacterName} : {response}");

        if (StopTalking) return;

        discussionPartner.ReceiveReply(response);
    }

    public void ReceiveReply(string PartnersResponse)
    {
        Start = System.DateTime.Now;

        AIText.text = "...";
        string inputText = $"subject = {Subject}." + 
            $"{Description}." +
            $"The last thing they said was: {PartnersResponse}.";

        HuggingFaceAPI.TextGeneration(inputText, SetAIText, OnError);
    }
}

public class DiscussionScriptHuggingFace : MonoBehaviour
{
    public InputField subjectField;


    public string llmCharacter1_Description;
    public Text AIText1;
    DiscussionInteractionHuggingFace interaction1;

    public string llmCharacter2_Description;
    public Text AIText2;
    DiscussionInteractionHuggingFace interaction2;

    // Start is called before the first frame update
    void Start()
    {
        subjectField.onSubmit.AddListener(onSubjectFieldSubmit);


        interaction1 = new DiscussionInteractionHuggingFace(llmCharacter1_Description, AIText1, "Eve");
        interaction2 = new DiscussionInteractionHuggingFace(llmCharacter2_Description, AIText2, "Adam");

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
        interaction1.StopTalking = true;
        interaction2.StopTalking = true;

        subjectField.interactable = true;
    }

    public void ExitGame()
    {
        Debug.Log("Exit button clicked");
        Application.Quit();
    }
    private void OnApplicationQuit()
    {
        interaction1.StopTalking = true;
        interaction2.StopTalking = true;
    }
}
