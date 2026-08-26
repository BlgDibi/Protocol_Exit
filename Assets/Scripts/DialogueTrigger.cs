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
    public string interactableID = "npc1";

    [Header("Animation & Movement")]
    public Animator npcAnimator;
    public string startAnimParameter = "hasSatDown";
    public string questVariableToWatch = "got_reward";

    [Header("Navigation Settings")]
    public Transform interactionPoint;
    public Transform[] patrolPoints;

    private NavMeshAgent agent;
    private bool isMovingToPoint = false;
    private bool hasReachedPoint = false;
    private int currentPatrolIndex = -1;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (agent != null && !agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agent.Warp(hit.position);
            }
        }

        StartCoroutine(InitializeNPCState());
    }

    private IEnumerator InitializeNPCState()
    {
        yield return new WaitForSeconds(0.2f);

        bool metInSaveList = SaveSystem.WasNPCMet(interactableID);
        string rewardState = DialogueManager.Instance != null ? DialogueManager.Instance.GetVariableState(questVariableToWatch) : "false";

        if (metInSaveList || rewardState.ToLower() == "true")
        {
            hasReachedPoint = true;
            if (agent != null) agent.enabled = false;

            if (interactionPoint != null)
            {
                transform.position = interactionPoint.position;
                transform.rotation = interactionPoint.rotation;
            }

            if (npcAnimator != null)
            {
                npcAnimator.SetBool(startAnimParameter, true);
            }
            Debug.Log("NPC ÜLVE MARAD:" + interactableID);
        }
        else
        {
            hasReachedPoint = false;
            if (agent != null) agent.enabled = true;
            Debug.Log("Új játék indult, az NPC elkezd sétálni: " + interactableID);
        }
    }

    public void Interact()
    {
        if (isMovingToPoint || DialogueManager.Instance == null || inkJSON == null) return;

        string metState = DialogueManager.Instance.GetVariableState("met_npc");
        string rewardState = DialogueManager.Instance.GetVariableState(questVariableToWatch);

        bool hasMet = (metState != null && metState.ToLower() == "true") || SaveSystem.WasNPCMet(interactableID);
        bool hasFinished = (rewardState != null && rewardState.ToLower() == "true");

     
        if (hasReachedPoint || hasMet || hasFinished)
        {
            StartDialogue();
            return;
        }

        if (agent == null || interactionPoint == null)
        {
            StartDialogue();
            return;
        }

        StartCoroutine(WalkToChairAndTalk());
    }

    private void StartDialogue()
    {
        SaveSystem.SaveNPCMet(interactableID);

        if (npcAnimator != null)
            npcAnimator.SetBool(startAnimParameter, true);

        DialogueManager.Instance.EnterDialogueMode(inkJSON, inkKnotName, GetComponent<Talk>());
    }

    private IEnumerator WalkToChairAndTalk()
    {
        isMovingToPoint = true;
        hasReachedPoint = true;

        while (agent == null || !agent.isOnNavMesh) yield return null;

        agent.isStopped = false;
        agent.SetDestination(interactionPoint.position);
        yield return new WaitForSeconds(0.1f);

        while (agent.pathPending || agent.remainingDistance > 0.1f) yield return null;

        agent.isStopped = true;
        agent.updateRotation = false;
        agent.velocity = Vector3.zero;

        Quaternion targetRotation = interactionPoint.rotation;
        Quaternion startRotation = transform.rotation;
        float turnProgress = 0f;
        while (turnProgress < 1f)
        {
            turnProgress += Time.deltaTime * 3f;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, turnProgress);
            yield return null;
        }

        transform.rotation = targetRotation;
        transform.position = interactionPoint.position;

        agent.enabled = false;
        StartDialogue();
        isMovingToPoint = false;
    }

    public void DialogueEnded()
    {
        if (npcAnimator != null)
        {
            npcAnimator.SetBool(startAnimParameter, true);
        }
    }

    private void Update()
    {
        if (hasReachedPoint || isMovingToPoint || agent == null || patrolPoints.Length == 0) return;
        if (!agent.enabled || !agent.isOnNavMesh) return;

        if (!agent.hasPath || (!agent.pathPending && agent.remainingDistance < 0.5f))
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    public KeyCode GetInteractKey() => interactionKey;
    public string GetInteractText() => isMovingToPoint ? "" : interactionPrompt.GetLocalizedString();
}