using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static NotificationCenter;
using static MessageHelper;

public class Demand : MonoBehaviour
{
    [SerializeField] Slider[] sliders;

    [SerializeField] TextMeshProUGUI priorityText;
    [SerializeField] TextMeshProUGUI firstclassText;
    [SerializeField] TextMeshProUGUI fragileText;

    int labelBonus = 50;

    private void OnEnable()
    {
        AddObserver("Bonus", Bonus);
        AddObserver("TutorialFinished", TutorialFinished);
    }

    private void OnDisable()
    {
        RemoveObserver("Bonus", Bonus);
        RemoveObserver("TutorialFinished", TutorialFinished);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        priorityText.text = sliders[0].value.ToString() + "%";
        firstclassText.text = sliders[1].value.ToString() + "%";
        fragileText.text = sliders[2].value.ToString() + "%";
    }

    public void Bonus(Notification notification)
    {
        int label = (int)notification.Object;

        if (label != 4) {
            int roundedBonus = Mathf.RoundToInt(labelBonus * sliders[label].value / 100);
            PostNotification("AwardMoney", roundedBonus);
        }
    }

    public void TutorialFinished(Notification notification)
    {
        InvokeRepeating(nameof(RandomizeSliders), 30, 30);
    }

    void RandomizeSliders()
    {
        foreach (Slider slider in sliders)
        {
            int newValue = Random.Range(0, 101);
            StartCoroutine(Transition(slider, newValue));
        }
    }

    IEnumerator Transition(Slider slider, float newValue)
    {
        Vector3 start = new Vector3(slider.value, 0);
        Vector3 finish = new Vector3(newValue, 0);
        while (start != finish)
        {
            start = Vector3.MoveTowards(start, finish, 1);
            slider.value = start.x;
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
}
