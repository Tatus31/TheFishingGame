using UnityEngine;
using Ink.Runtime;
using UnityEngine.UI;

public class InkTest : MonoBehaviour
{
    public TextAsset inkJSONAsset;

    Story story;


    private void Awake()
    {
        story = new Story(inkJSONAsset.text);

        while (story.canContinue)
        {
            Debug.Log(story.Continue());
        }

        if(story.currentChoices.Count > 0)
        {
            for (int i = 0; i < story.currentChoices.Count; ++i)
            {
                Choice choice = story.currentChoices[i];
                Debug.Log($"Choice {i + 1} {choice.text}");
            }
        }
    }
}
