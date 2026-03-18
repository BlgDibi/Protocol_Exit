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
    public string npcID = "npc1";

    [Header("Animation & Movement")]
    public Animator npcAnimator;
    public string startAnimParameter = "hasSatDown";
    public string questVariableToWatch = "got_reward";

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
        StartCoroutine(RestoreStateAfterLoad());
    }

    private IEnumerator RestoreStateAfterLoad()
    {
        yield return new WaitForSeconds(0.2f);

        bool metInSaveList = SaveSystem.WasNPCMet(npcID);
        string rewardState = DialogueManager.Instance != null ? DialogueManager.Instance.GetVariableState(questVariableToWatch) : "false";

        if (metInSaveList || rewardState.ToLower() == "true")
        {
            hasSatDownOnce = true;
            if (agent != null) agent.enabled = false;

            if (chairPosition != null)
            {
                transform.position = chairPosition.position;
                transform.rotation = chairPosition.rotation;
            }

            if (npcAnimator != null)
            {
                npcAnimator.SetBool(startAnimParameter, true);
            }
            Debug.Log("<color=green>NPC ÜLVE MARAD: </color>" + npcID);
        }
    }

    public void Interact()
    {
        if (isWalkingToChair || DialogueManager.Instance == null || inkJSON == null) return;

        string metState = DialogueManager.Instance.GetVariableState("met_npc");
        string rewardState = DialogueManager.Instance.GetVariableState(questVariableToWatch);

        bool hasMet = (metState != null && metState.ToLower() == "true");
        bool hasFinished = (rewardState != null && rewardState.ToLower() == "true");

        if (hasSatDownOnce || hasMet || hasFinished)
        {
            if (hasFinished)
                DialogueManager.Instance.EnterDialogueMode(inkJSON, "told_everything", GetComponent<Talk>());
            else
                StartDialogue();
            return;
        }

        if (chairPosition != null && agent != null)
        {
            StartCoroutine(WalkToChairAndTalk());
        }
    }

    private void StartDialogue()
    {
        SaveSystem.SaveNPCMet(npcID);
        SaveSystem.Save();

        if (npcAnimator != null)
            npcAnimator.SetBool(startAnimParameter, true);

        DialogueManager.Instance.EnterDialogueMode(inkJSON, inkKnotName, GetComponent<Talk>());
    }

    private IEnumerator WalkToChairAndTalk()
    {
        isWalkingToChair = true;
        hasSatDownOnce = true;

        while (agent == null || !agent.isOnNavMesh) yield return null;

        agent.isStopped = false;
        agent.SetDestination(chairPosition.position);
        yield return new WaitForSeconds(0.1f);

        while (agent.pathPending || agent.remainingDistance > 0.1f) yield return null;

        agent.isStopped = true;
        agent.updateRotation = false;
        agent.velocity = Vector3.zero;

        Quaternion targetRotation = chairPosition.rotation;
        Quaternion startRotation = transform.rotation;
        float turnProgress = 0f;
        while (turnProgress < 1f)
        {
            turnProgress += Time.deltaTime * 3f;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, turnProgress);
            yield return null;
        }

        transform.rotation = targetRotation;
        transform.position = chairPosition.position;

        agent.enabled = false;
        StartDialogue();
        isWalkingToChair = false;
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
        if (hasSatDownOnce || isWalkingToChair || agent == null || patrolPoints.Length == 0) return;
        if (!agent.enabled || !agent.isOnNavMesh) return;

        if (!agent.hasPath || (!agent.pathPending && agent.remainingDistance < 0.5f))
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    public KeyCode GetInteractKey() => interactionKey;
    public string GetInteractText() => isWalkingToChair ? "" : interactionPrompt.GetLocalizedString();
}