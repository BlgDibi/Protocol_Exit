using System.Collections;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.AI;

public class DialogueTrigger : MonoBehaviour, InterfaceInteractable
{
    [Header("Ink Settings")]
    public TextAsset inkJSON;
    public string inkKnotName = "book_intro";

    [Header("Localization - Interaction")]
    public LocalizedString interactionPrompt;

    [Header("Settings")]
    [SerializeField] private KeyCode interactionKey = KeyCode.T;

    [Header("Animation & Movement")]
    public Animator npcAnimator;
    public string startAnimParameter = "";
    public string questVariableToWatch = "";
    public string finishAnimParameter = "";

    [Header("Navigation Settings")]
    public Transform chairPosition;
    public Transform[] patrolPoints;

    private NavMeshAgent agent;
    private bool isWalkingToChair = false;
    private bool hasSatDownOnce = false;
    private int currentPatrolIndex = -1;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (hasSatDownOnce || isWalkingToChair || agent == null || patrolPoints.Length == 0) return;

        if (!agent.isOnNavMesh) return;

        if (npcAnimator != null && !string.IsNullOrEmpty(finishAnimParameter) && npcAnimator.GetBool(finishAnimParameter))
        {
            agent.isStopped = true;
            return;
        }

        if (!agent.hasPath || (!agent.pathPending && agent.remainingDistance < 0.5f))
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    public KeyCode GetInteractKey() => interactionKey;

    public string GetInteractText()
    {
        if (isWalkingToChair) return "";
        return interactionPrompt.GetLocalizedString();
    }

    public void Interact()
    {
        if (isWalkingToChair || DialogueManager.Instance == null || inkJSON == null) return;

        string questState = DialogueManager.Instance.GetVariableState(questVariableToWatch);
        bool hasKey = (questState != null && questState.ToLower() == "true");

        if (hasKey)
        {
           
            DialogueManager.Instance.EnterDialogueMode(inkJSON, "told_everything", GetComponent<Talk>());
        }
        else if (chairPosition != null && agent != null && !hasSatDownOnce)
        {
            StartCoroutine(WalkToChairAndTalk());
        }
        else
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        if (npcAnimator != null && !string.IsNullOrEmpty(startAnimParameter))
        {
            npcAnimator.SetBool(startAnimParameter, true);
        }

        Talk npcScript = GetComponent<Talk>();

        DialogueManager.Instance.EnterDialogueMode(inkJSON, inkKnotName, npcScript);
    }
    private IEnumerator WalkToChairAndTalk()
    {
        isWalkingToChair = true;
        hasSatDownOnce = true;

        while (!agent.isOnNavMesh) yield return null;

        agent.isStopped = false;
        agent.SetDestination(chairPosition.position);

        yield return new WaitForSeconds(0.1f);

        while (agent.pathPending || agent.remainingDistance > 0.1f)
        {
            yield return null;
        }

        agent.isStopped = true;
        agent.updateRotation = false; 
        agent.velocity = Vector3.zero;

        float perfectYRotation = -20f;

        Quaternion targetRotation = Quaternion.Euler(0, perfectYRotation, 0);
        Quaternion startRotation = transform.rotation;

        float turnProgress = 0f;
        while (turnProgress < 1f)
        {
            turnProgress += Time.deltaTime * 3f;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, turnProgress);
            yield return null;
        }

        transform.rotation = targetRotation;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.11f, transform.position.z);
        agent.enabled = false;
        StartDialogue();
        isWalkingToChair = false;

    }

    public void DialogueEnded()
    {
        string questState = DialogueManager.Instance.GetVariableState(questVariableToWatch);
        if (questState != null && questState.ToLower() == "true")
        {
            if (agent != null)
            {
                agent.enabled = true;
                agent.updateRotation = true;
            }

            if (npcAnimator != null && !string.IsNullOrEmpty(finishAnimParameter))
            {
                npcAnimator.SetBool(finishAnimParameter, true);
                npcAnimator.SetBool(startAnimParameter, false);
            }
        }
    }