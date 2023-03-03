using System;
using UnityEngine;

public class Cinematics : MonoBehaviour
{
    private DollySystem[] dollySystems;
    private int currentSystemIndex;
    private DollySystem currentDollySystem;

    public event Action OnSequenceComplete;

    private void Awake()
    {
        dollySystems = GetComponentsInChildren<DollySystem>();
    }

    private void OnEnable()
    {
        if (dollySystems.Length == 0)
            return;

        foreach (var dollySystem in dollySystems)
            dollySystem.gameObject.SetActive(false);

        currentSystemIndex = 0;
        UpdateCurrentDollySystem();
    }

    private void DollySytemOnComplete()
    {
        currentSystemIndex++;
        if (currentSystemIndex == dollySystems.Length)
        {
            OnSequenceComplete?.Invoke();
            return;
        }

        UpdateCurrentDollySystem();
    }

    private void UpdateCurrentDollySystem()
    {
        if (currentDollySystem != null)
        {
            currentDollySystem.OnComplete -= DollySytemOnComplete;
            currentDollySystem.gameObject.SetActive(false);
        }
        currentDollySystem = dollySystems[currentSystemIndex];
        currentDollySystem.gameObject.SetActive(true);
        currentDollySystem.OnComplete += DollySytemOnComplete;
    }
}
