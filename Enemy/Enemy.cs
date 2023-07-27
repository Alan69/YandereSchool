using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    [Header("General Settings")]
    [Tooltip("Player gameobject from scene")]
    public PlayerController player;
    [Tooltip("Transform of enemy head")]
    public Transform head;
    [Tooltip("Layers by which the enemy will search for the player (Must include player layer, as well as obstacle layers (Default,Interact еtc))")]
    public LayerMask searchLayers;
    private UnityEngine.AI.NavMeshAgent agent;
    private float magnit;
    private Animator animator;

    [Header("AI Settings")]
    [Tooltip("Layer of doors (Interact by default)")]
    public LayerMask doorsLayer;
    [Tooltip("Distance through which the enemy sees the player")]
    public float seeDistance;
    [Tooltip("Enemy Field of View (x - minimal, y - maximal)")]
    public Vector2 enemyFOV;
    [Tooltip("Radius at which the enemy will notice the player if the player is too close (if player crouch this value = / 2) ")]
    public float closeDistance;
    [Tooltip("The time after which the enemy loses the player(after the time has passed, the player’s coordinates are transferred to the enemy and he goes to these coordinates)")]
    public float lostTime;
    [Tooltip("The time that the enemy spends at the point where he last saw or heard the player")]
    public float patrolTime;
    [Tooltip("Controll enemy walk speed")]
    public float walkSpeed;
    [HideInInspector]
    public bool seePlayer;
    private bool chasePlayer;
    private bool searchPlayer;
    private int searchState;
    private HidePlace playerHidePlace;


    [Header("Sound Settings")]
    [Tooltip("Sound of enemy footsteps")]
    public AudioClip[] footSteps;
    [Tooltip("The sound that the enemy makes when he catch a player")]
    public AudioClip catchSound;
    [Tooltip("Player Hitting Sound")]
    public AudioClip hitSound;
    private AudioSource AS;
    

    [Header("Patrol Settings")]
    [Tooltip("Transforms of way points for enemy patrolling")]
    public Transform[] wayPoints;
    [Tooltip("The time that the enemy waits on points")]
    public float wayPointWaitTime;
    private int wpID;

    [Header("Catch Settings")]
    [Tooltip("Sets the speed of the player’s camera turning to face the enemy at the moment when the enemy catches player")]
    public float playerLookSpeed;
    [Tooltip("The distance at which the enemy can catch player")]
    public float catchDistance;
    private int catchPlayerState;
    private Vector3 lastSawPoint;



    private void Awake()
    {
        AS = GetComponent<AudioSource>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.speed = walkSpeed;
        animator = GetComponent<Animator>();
        lastSawPoint = Vector3.zero;
        wpID = -1;
    }

    private void Update()
    {

        SearchPlayer();
        CatchingPlayer();
        DoorCheck();
        CheckLastPoint();
        PatrolWayPoints();
        CheckHidePlace();

        if (agent.enabled)
        {
            magnit = agent.velocity.magnitude;
            animator.SetFloat("Magnitude", magnit);
        }else
        {
            animator.SetFloat("Magnitude", 0f);
        }
    }

    public void SendHidePlace()
    {
        lastSawPoint = Vector3.zero;
        playerHidePlace = player.hidePlace;
    }

    private void PatrolWayPoints()
    {
        if (!seePlayer && !searchPlayer && wpID == -1)
        {
            StopAllCoroutines();
            int r = Random.Range(1, wayPoints.Length);
            EnemySetDestination(wayPoints[r].position);
            wpID = r;
            wayPoints[r].SetSiblingIndex(0);


        }

        if (wpID != -1)
        {
            float dist = Vector3.Distance(transform.position, wayPoints[wpID].position);
            if (dist <= agent.stoppingDistance)
            {
                EnemySetDestination(transform.position);
                StartCoroutine(WaitPatrolTime(0));
            }
        }
    }

    private void CheckHidePlace()
    {
        if (playerHidePlace && searchState != 2)
        {
            if (player.hidePlace != null && !seePlayer && player.hidePlace == playerHidePlace)
            {
                EnemySetDestination(playerHidePlace.enemyPosition.position);
                float dist = Vector3.Distance(transform.position, playerHidePlace.enemyPosition.position);

                if (dist <= agent.stoppingDistance)
                {
                    StartCoroutine(WaitOnHidePlace());
                    searchState = 2;
                    transform.rotation = playerHidePlace.enemyPosition.rotation;
                    agent.enabled = false;

                }
            }
            else
            {
                playerHidePlace = null;
            }
        }
    }

    private void CheckLastPoint()
    {
        if (!seePlayer && lastSawPoint != Vector3.zero && searchState == 0)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(lastSawPoint, out hit, 1.5f, NavMesh.AllAreas))
            {
                lastSawPoint = hit.position;
            }

            EnemySetDestination(lastSawPoint);

                float dist = Vector3.Distance(transform.position, lastSawPoint);

                if (dist <= 2f)
                {
                    EnemySetDestination(transform.position);
                    lastSawPoint = Vector3.zero;
                    searchState = 1;
                    animator.SetBool("Patrol", true);
                    StartCoroutine(WaitPatrolTime(1));
                }
            

        }
    }

    public void CallEnemy(Vector3 pos)
    {
        if(!seePlayer)
        {

            lastSawPoint = pos;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(lastSawPoint, out hit, 1f, NavMesh.AllAreas))
            {
                lastSawPoint = hit.position;               
            }
              searchState = 0;
        }
    }

    private void SearchPlayer()
    {

        if(!seePlayer)
        {
            float dist = Vector3.Distance(transform.position,player.transform.position);
            float radius = closeDistance;
            if(player.crouch)
            {
                radius /= 2f;
            }
            if(dist <= radius && player.hidePlace == null && player.playerMoving && PlayerRaycast())
            {
                lastSawPoint = player.transform.position;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(lastSawPoint, out hit, 1f, NavMesh.AllAreas))
                {
                    lastSawPoint = hit.position;
                }
            }
        }

        if (PlayerRaycast() && PlayerFOV() && player.hidePlace == null)
        {
            chasePlayer = true;
            seePlayer = true;
           
            ResetSearchStates();
            StopAllCoroutines();
        }else
        {
            if (seePlayer)
            {
                if (!searchPlayer)
                {
                    seePlayer = false;
                    searchPlayer = true;                           
                    StartCoroutine(WaitLostTime());
                }
            }
            else
            {
                seePlayer = false;
            }
        }

        if(seePlayer || chasePlayer)
        {
            EnemySetDestination(player.transform.position);
        }
    }

    private bool PlayerRaycast()
    {
        RaycastHit hit;
      
        if (Physics.Raycast(transform.position, (player.transform.position - transform.position), out hit, seeDistance))
        {
            if (hit.transform.gameObject == player.gameObject)
            {
                return true;
            }
        }

        return false;
    }

    private bool PlayerFOV()
    {
        Vector3 targetDir = player.transform.position - head.transform.position;
        float angleToPlayer = (Vector3.Angle(targetDir, head.forward));

        if (angleToPlayer >= enemyFOV.x && angleToPlayer <= enemyFOV.y) // 180° FOV

        {
            return true;
        }

        return false;
    }

    private void CatchingPlayer () {

        if (catchPlayerState == 0 && seePlayer && player.canBeCatchen)
        {
            
            float dist = Vector3.Distance(transform.position, player.transform.position);

            if (dist <= catchDistance)
            {
                AudioSource.PlayClipAtPoint(catchSound, transform.position);
                player.CatchPlayer(1,null);
                catchPlayerState = 1;
                agent.enabled = false;
            }
        }

        if(catchPlayerState == 1)
        {
            Vector3 lTargetDir = head.position - player.cameraTransform.position;
            player.cameraTransform.rotation = Quaternion.RotateTowards(player.cameraTransform.rotation, Quaternion.LookRotation(lTargetDir), Time.deltaTime * playerLookSpeed);

            Vector3 lookPos = player.transform.position - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            Quaternion rotation2 = Quaternion.LookRotation(-lookPos);


            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, Time.deltaTime * playerLookSpeed);
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, rotation2, Time.deltaTime * playerLookSpeed);


            float ang = Quaternion.Angle(player.cameraTransform.rotation, Quaternion.LookRotation(lTargetDir));
         
            if (ang <= 4.0)
            {
                transform.rotation = rotation;
                player.transform.rotation = rotation2;
                lTargetDir = head.position - player.cameraTransform.position;
                player.cameraTransform.rotation = Quaternion.LookRotation(lTargetDir);
                animator.SetInteger("HitID", 0);
                animator.SetTrigger("HitPlayer");
                catchPlayerState = 2;
                player.CatchPlayer(2,null);

            }
        }

        if(catchPlayerState == 3)
        {
            StopAllCoroutines();
          
            if (player.hidePlace == playerHidePlace)
            {
                if (playerHidePlace.hidePlaceType == HidePlace.HidePlaceType.bed)
                {
                    GetComponent<IKControl>().ikActive = true;
                    transform.position = playerHidePlace.enemyPosition.position;
                    transform.rotation = playerHidePlace.enemyPosition.rotation;
                    Vector3 lookPos = player.transform.position - transform.position;
                    lookPos.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(-lookPos);
                    player.transform.rotation = rotation;
                    player.cameraTransform.rotation = rotation;
                    animator.SetInteger("HitID", playerHidePlace.enemyAnimationState);
                    animator.SetTrigger("HitPlayer");
                    player.CatchPlayer(3,playerHidePlace.cameraAnimationName);
                    catchPlayerState = 4;
                }

                if(playerHidePlace.hidePlaceType == HidePlace.HidePlaceType.chest)
                {

                    AudioSource.PlayClipAtPoint(catchSound, transform.position);
                    player.CatchPlayer(1,null);
                    catchPlayerState = 1;
                    agent.enabled = false;

                    if(playerHidePlace.hidePlaceAnimation)
                    {
                        playerHidePlace.hidePlaceAnimation.Play(playerHidePlace.hidePlaceUnhideAnimationName);
                    }
                }

            }else
            {
                agent.enabled = true;
                playerHidePlace = null;
                catchPlayerState = 0;
            }
        }
        

		
	}

    private void DoorCheck()
    {
        RaycastHit hit;
     
        if (Physics.Raycast(head.transform.position, head.transform.forward, out hit, 1f, doorsLayer))
        {
            if(hit.transform.gameObject.GetComponent<DoorSiders>())
            {
                if (hit.transform.gameObject.GetComponent<DoorSiders>().genDoor.state == 0)
                {
                    if(hit.transform.gameObject.GetComponent<DoorSiders>().genDoor.locked)
                    {
                        hit.transform.gameObject.GetComponent<DoorSiders>().genDoor.UnlockDoor();
                    }
                    hit.transform.gameObject.GetComponent<DoorSiders>().InteractWithDoor();
                }
            }
        }
    }

    private void ResetSearchStates()
    {
     
        wpID = -1;
        searchState = 0;
        searchPlayer = false;
        animator.SetBool("Patrol", false);
    }

    private void EnemySetDestination(Vector3 pos)
    {
        if(agent.enabled)
        {
            animator.SetBool("Patrol", false);
            agent.SetDestination(pos);
        }
    }

    public void RestartEnemyStats()
    {
        StopAllCoroutines();
        GetComponent<IKControl>().ikActive = false;
        agent.enabled = true;
        catchPlayerState = 0;
    }

    public void FootStepPlay()
    {
        int r = Random.Range(1,footSteps.Length);
        AS.PlayOneShot(footSteps[r]);
    }

    public void PlayAnimationSound()
    {
        AudioSource.PlayClipAtPoint(hitSound, transform.position);
        player.CatchPlayer(4,null);
    }

    IEnumerator WaitLostTime()
    {
        yield return new WaitForSeconds(lostTime);
        lastSawPoint = player.transform.position;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(lastSawPoint, out hit, 1f, NavMesh.AllAreas))
        {
            lastSawPoint = hit.position;
        }
        chasePlayer = false;
    
    }

    IEnumerator WaitPatrolTime(int state)
    {
        yield return new WaitForSeconds(patrolTime);
        if(state == 1)
        {
            lastSawPoint = Vector3.zero;
        }
        ResetSearchStates();
    }

    IEnumerator WaitOnHidePlace()
    {
        yield return new WaitForSeconds(wayPointWaitTime);
        if(player.hidePlace != null && player.hidePlace == playerHidePlace)
        {
            catchPlayerState = 3;
        }
        else
        {
            agent.enabled = true;
            playerHidePlace = null;

        }
    }
}
