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
    [Tooltip("Id")]
    public string interactableID = "npc1";

    [Header("Animation & Movement")]
    public Animator npcAnimator;
    public string startAnimParameter = "hasSatDown";

    [Tooltip("Keep Anim Bool")]
    public bool keepAnimBoolTrueAtEnd = true;

    [Tooltip("End Anim trigger")]
    public string endAnimTrigger = "";

    [Header("Navigation Settings")]
    public Transform interactionPoint;
    public Transform[] patrolPoints;

    private NavMeshAgent agent;
    private bool isMovingToPoint = false;
    private bool hasReachedPoint = false;
    private int currentPatrolIndex = -1;

    [Header("Quest Settings")]
    public string questVariableToWatch = "got_reward";

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
        yield return new WaitForSeconds(0.5f);

        bool metInSaveList = SaveSystem.WasNPCMet(interactableID);
        string rewardState = DialogueManager.Instance != null ? DialogueManager.Instance.GetVariableState(questVariableToWatch) : "false";

        bool hasC4Already = C4_Pickup.HasC4 || (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition("HasC4"));

        if (metInSaveList || rewardState.ToLower() == "true" || hasC4Already)
        {
            hasReachedPoint = true;
            if (agent != null)
                agent.enabled = false;

            if (interactionPoint != null)
            {
                transform.position = interactionPoint.position;
                transform.rotation = interactionPoint.rotation;
            }

            if (npcAnimator != null)
            {
                npcAnimator.SetBool(startAnimParameter, true);
                npcAnimator.Play("breathing", -1, 0f);
            }
        }
        else
        {
            hasReachedPoint = false;
            if (agent != null)
                agent.enabled = true;
        }
    }

    public void Interact()
    {
        if (isMovingToPoint || DialogueManager.Instance == null || inkJSON == null) return;

        string metState = DialogueManager.Instance.GetVariableState("met_npc");
        string rewardState = DialogueManager.Instance.GetVariableState("got_reward");

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
        if (npcAnimator != null)
        {
            npcAnimator.SetBool(startAnimParameter, true);
        }

        Talk talkScript = GetComponent<Talk>();
        string currentKnot = inkKnotName;

        if (DialogueManager.Instance != null)
        {
            bool hasMetBefore = SaveSystem.WasNPCMet(interactableID);

            if (interactableID == "npc1" || interactableID == "mechanic" || interactableID == "janitor")
            {
                string inkHasC4 = DialogueManager.Instance.GetVariableState("HasC4");
                string inkHasDetonator = DialogueManager.Instance.GetVariableState("HasDetonator");

                bool hasDetonator = Detonator_Pickup.HasDetonator || (inkHasDetonator != null && inkHasDetonator.ToLower() == "true");
                bool hasC4 = C4_Pickup.HasC4 || (inkHasC4 != null && inkHasC4.ToLower() == "true") || (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition("HasC4"));

                if (hasC4)
                {
                    currentKnot = "mechanic_leave_me";
                }
                else if (hasDetonator)
                {
                    currentKnot = "mechanic_done";
                }
                else if (hasMetBefore)
                {
                    currentKnot = "mechanic_waiting";
                }
            }
            else if (interactableID == "receptionist")
            {
                if (hasMetBefore)
                {
                    currentKnot = "told_everything";
                }
            }
            else if (interactableID == "chef")
            {
                bool hasValve = (ConditionManager.Instance != null && ConditionManager.Instance.CheckCondition("HasValve"));

                if (hasValve)
                {
                    currentKnot = "chef_leave_me";
                }
                else if (DiningPuzzle.IsTableSet)
                {
                    currentKnot = "chef_done";
                }
                else if (hasMetBefore)
                {
                    currentKnot = "chef_waiting";
                }
                else
                {
                    currentKnot = inkKnotName;
                }
            }
        }

        SaveSystem.SaveNPCMet(interactableID);
        DialogueManager.Instance.EnterDialogueMode(inkJSON, currentKnot, talkScript);
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
            npcAnimator.SetBool(startAnimParameter, keepAnimBoolTrueAtEnd);

            if (!string.IsNullOrEmpty(endAnimTrigger))
            {
                npcAnimator.SetTrigger(endAnimTrigger);
            }
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
    public string GetInteractText()
    {
        if (isMovingToPoint)
        {
            return "";
        }
        else
        {
            return interactionPrompt.GetLocalizedString();
        }
    }
}