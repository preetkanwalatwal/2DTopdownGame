using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;


public class VB3_Animation : MonoBehaviour
{
    Animator vb3_anim;
    VB3_HealthSystem vB3_HealthSystem;

    public AnimationClip deathEast;
    public AnimationClip deathNorth;
    public AnimationClip deathWest;
    public AnimationClip deathSouth;

    PlayableGraph deathGraph;
    AnimationMixerPlayable deathMixer;
    AnimationClipPlayable[] deathPlayables;

    public float deathTime = 0f;
    public float deathLength = 0f;
    bool deathGraphStarted = false;
    public bool deathFinished = false;
    void Start()
    {
        vB3_HealthSystem = GetComponent<VB3_HealthSystem>();
        vb3_anim = GetComponentInChildren<Animator>();
    }

    // Movement & Attack
    public void UpdateAnims(Vector2 moveDir, bool isAttack)
    {
        if (vB3_HealthSystem.isDead)
        {
            moveDir = Vector2.zero;
            return;
        }
        
        
        vb3_anim.SetFloat("MoveX", moveDir.x);
        vb3_anim.SetFloat("MoveY", moveDir.y);
        vb3_anim.SetFloat("Speed", moveDir.sqrMagnitude);
        // vb3_anim.SetBool("isRunning", isRun);
        // if (isRun)
        // {
        // if(isFly) vb3_anim.SetTrigger("Run");
        // }
        // Debug.Log(isRun);
        if (isAttack)
        {
            vb3_anim.SetFloat("LastMoveX", moveDir.x);
            vb3_anim.SetFloat("LastMoveY", moveDir.y);
            vb3_anim.SetFloat("Speed", 0);
        }
        if (moveDir != Vector2.zero)
        {
            vb3_anim.SetFloat("LastMoveX", moveDir.x);
            vb3_anim.SetFloat("LastMoveY", moveDir.y);
        }
        
    }

    public void PlayFly()
    {
        vb3_anim.SetFloat("LastMoveX", 0);
        vb3_anim.SetFloat("LastMoveY", -1);
        vb3_anim.SetBool("isRunning", true);
    }

    public void PlayAttackAnim()
    {
        vb3_anim.SetTrigger("Attack");
    }

    public void PlayMoveFly(Vector2 moveDir, bool isFly)
    {
        vb3_anim.SetFloat("MoveX", moveDir.x);
        vb3_anim.SetFloat("MoveY", moveDir.y);
        vb3_anim.SetBool("isRunning", isFly);
        vb3_anim.SetTrigger("Run");
    }

    // On Hit
    public void UpdateHurtAnim(Vector2 faceDir, bool hit)
    {
        if (!hit) return;

        if (faceDir != Vector2.zero)
        {
            vb3_anim.SetFloat("LastMoveX", faceDir.x);
            vb3_anim.SetFloat("LastMoveY", faceDir.y);
        }
        vb3_anim.SetTrigger("IsHit");
    }

    // On Death
    public void PlayDeathAnim(Vector2 faceDir, bool isDead)
    {
        if (!isDead) return;
        if (deathFinished) return;

        // Orient last move direction
        if (faceDir != Vector2.zero)
        {
            vb3_anim.SetFloat("LastMoveX", faceDir.x);
            vb3_anim.SetFloat("LastMoveY", faceDir.y);
        }

        vb3_anim.speed = 0f;
        vb3_anim.SetFloat("Speed", 0f);
        vb3_anim.SetTrigger("IsDead");

        // Build Playable Graph on first use
        if (!deathGraphStarted)
        {
            StartDeathGraph();
            deathGraphStarted = true;
             // Calculate the true length of the death animation
            deathLength = Mathf.Max(
                deathEast.length,
                deathNorth.length,
                deathWest.length,
                deathSouth.length
            );
        }

        if (deathTime < deathLength)
        {
            deathTime += Time.deltaTime * 0.33f;  // your playback rate
            deathTime = Mathf.Min(deathTime, deathLength);
        }
        else
        {
            deathFinished = true;
            // Destroy(gameObject);
            return;
        }

        deathTime += Time.deltaTime * 0.25f;

        foreach (var p in deathPlayables)
            p.SetTime(deathTime);
        Set2DSimpleDirectionalWeights(faceDir);


    }

    // 4-clip mixer    
    void StartDeathGraph()
    {
        deathGraph = PlayableGraph.Create("DeathGraph");
        deathPlayables = new AnimationClipPlayable[4];

        deathMixer = AnimationMixerPlayable.Create(deathGraph, 4);

        AnimationClip[] clips =
        {
            deathEast,  // index 0
            deathNorth, // index 1
            deathWest,  // index 2
            deathSouth  // index 3
        };

        for (int i = 0; i < 4; i++)
        {
            deathPlayables[i] = AnimationClipPlayable.Create(deathGraph, clips[i]);
            deathPlayables[i].SetApplyFootIK(false);
            deathPlayables[i].SetApplyPlayableIK(false);
            deathGraph.Connect(deathPlayables[i], 0, deathMixer, i);
        }

        var output = AnimationPlayableOutput.Create(deathGraph, "DeathOutput", vb3_anim);
        output.SetSourcePlayable(deathMixer);

        deathGraph.Play();
    }

    void Set2DSimpleDirectionalWeights(Vector2 dir)
    {
        if (dir == Vector2.zero)
            dir = Vector2.down;

        dir.Normalize();

        // Directions for each clip
        Vector2 east  = new Vector2( 1, 0);
        Vector2 north = new Vector2( 0, 1);
        Vector2 west  = new Vector2(-1, 0);
        Vector2 south = new Vector2( 0,-1);

        float w0 = Mathf.Max(0f, Vector2.Dot(dir, east));
        float w1 = Mathf.Max(0f, Vector2.Dot(dir, north));
        float w2 = Mathf.Max(0f, Vector2.Dot(dir, west));
        float w3 = Mathf.Max(0f, Vector2.Dot(dir, south));

        float sum = w0 + w1 + w2 + w3;
        if (sum < 0.0001f) sum = 1f;

        deathMixer.SetInputWeight(0, w0 / sum);
        deathMixer.SetInputWeight(1, w1 / sum);
        deathMixer.SetInputWeight(2, w2 / sum);
        deathMixer.SetInputWeight(3, w3 / sum);
    }

    public void PlayPhase1Anim()
    {
        Vector2 faceSouth = new Vector2(0, -1);
        vb3_anim.SetFloat("LastMoveX", faceSouth.x);
        vb3_anim.SetFloat("LastMoveY", faceSouth.y);
        vb3_anim.SetTrigger("Attack");
    }

    void OnDestroy()
    {
        if (deathGraph.IsValid())
            deathGraph.Destroy();
    }
}
