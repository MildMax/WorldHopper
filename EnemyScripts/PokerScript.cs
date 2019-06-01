using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokerScript : EnemyBase
{

    //[SerializeField]
    [HideInInspector]
    public int actionIndex;

    private int oldIndex = 4;

    public delegate void ActionMethod();
    public ActionMethod actionMethod;

    //0: PokerAscending
    //1: PokerDescending
    //2: PokerUp
    //3: PokerDown
    public AnimationClip[] anims;

    //values for timer-based movement
    public float timer;
    public float delay;

    public float dangerClose;

    private bool start = false;

    private bool startA = false;
    private bool startD = false;

    private bool isHurt = false;

    Coroutine routineP = null;
    Coroutine hurtRoutine = null;

    Animator anim;
    SpriteRenderer rend;
    Transform player;
    PlayerController playerController;

    public AnimatorOverrideController controllerMad;
    public AnimatorOverrideController controllerSad;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("IsDown", true); 
        rend = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        SetActionMethod();
        actionMethod();
        //Debug.Log(actionIndex);
    }

    private void Update()
    {
        CheckHealth();
        ChangeAnims();
        SetActionMethod();
        actionMethod();
    }

    //:::::::::GENERIC::::::::::://

    private void SetActionMethod()
    {
        if (actionIndex != oldIndex)
        {
            switch (actionIndex)
            {
                case 0:
                    actionMethod = Continuous;
                    return;
                case 1:
                    actionMethod = Timed;
                    return;
                case 2:
                    actionMethod = Proximity;
                    return;
                case 3:
                    actionMethod = Up;
                    return;
            }
            oldIndex = actionIndex;
            ResetStart();
        }
    }

    private void ResetStart()
    {
        start = false;
        startA = false;
        startD = false;
        anim.SetBool("IsContinuous", false);
    }

    private void ChangeAnims()
    {
        if(isHurt)
        {
            anim.runtimeAnimatorController = controllerSad;
        }
        else
        {
            anim.runtimeAnimatorController = controllerMad;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Shot")
        {
            health -= collision.gameObject.GetComponent<ShotScript>().damage;
            if(hurtRoutine != null)
            {
                StopCoroutine(hurtRoutine);
            }
            hurtRoutine = StartCoroutine(HurtTimer());
        }
    }

    private IEnumerator HurtTimer()
    {
        isHurt = true;
        yield return new WaitForSeconds(1);
        isHurt = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log("Collision with Player");

            playerController.GetHurt();
        }
    }

    private void CheckHealth()
    {
        if(health <= 0)
        {
            Destroy(gameObject);
        }
    }

    //::::::::::::CONTINOUS::::::::://
    private void Continuous()
    {
        anim.SetBool("IsDown", false);
        anim.SetBool("IsContinuous", true);
        if (!start)
        {
            StartCoroutine(ContinousAnimation());
        }
    }

    private IEnumerator ContinousAnimation()
    {
        start = true;
        while (actionIndex == 0)
        {
            anim.SetTrigger("IsAscending");
            yield return new WaitForSeconds(anims[0].length);
            anim.SetTrigger("IsDescending");
            yield return new WaitForSeconds(anims[1].length);
        }
        start = false;
    }


    //:::::::::::TIMED::::::::::::://
    private void Timed()
    {
        anim.SetBool("IsDown", false);
        if(start == false)
        {
            StartCoroutine(TimedAnimation());
        }
    }

    private IEnumerator TimedAnimation()
    {
        start = true;
        yield return new WaitForSeconds(delay);
        while(actionIndex == 1)
        {
            anim.SetTrigger("IsAscending");
            yield return new WaitForSeconds(anims[0].length);
            anim.SetBool("IsUp", true);
            yield return new WaitForSeconds(timer);
            anim.SetBool("IsUp", false);
            anim.SetTrigger("IsDescending");
            yield return new WaitForSeconds(anims[1].length);
            anim.SetBool("IsDown", true);
            yield return new WaitForSeconds(timer);
            anim.SetBool("IsDown", false);
        }
        start = false;
    }

    //::::::::::PROXIMITY:::::::::://
    private void Proximity()
    {
        bool withinDistance = FindMagnitude();

        //Debug.Log("Get Bool - " + anim.GetBool("IsDown") + " : " + anim.GetBool("IsUp"));
        //Debug.Log("WD + SA - " + withinDistance + " : " + startA);

        if(!anim.GetBool("IsUp") && !anim.GetBool("IsDown"))
        {
            //
            //Debug.Log("not up or down");
        }
        else if (withinDistance && startA == false && !anim.GetBool("IsUp"))
        {
            if (routineP != null)
            {
                StopCoroutine(routineP);
            }
            routineP = StartCoroutine(ProximityA());
        }
        else if (!withinDistance && startD == false && !anim.GetBool("IsDown"))
        {
            if (routineP != null)
            {
                StopCoroutine(routineP);
            }
            routineP = StartCoroutine(ProximityD());
        }
    }

    private IEnumerator ProximityA()
    {
        Debug.Log("Starting Coroutine A:");
        startD = false;
        startA = true;
        anim.SetBool("IsDown", false);
        anim.SetBool("IsUp", false);
        anim.SetTrigger("IsAscending");
        yield return new WaitForSeconds(anims[0].length);
        anim.SetBool("IsUp", true);
    }

    private IEnumerator ProximityD()
    {
        Debug.Log("Starting Coroutine D");
        startA = false;
        startD = true;
        anim.SetBool("IsUp", false);
        anim.SetBool("IsDown", false);
        anim.SetTrigger("IsDescending");
        yield return new WaitForSeconds(anims[1].length);
        anim.SetBool("IsDown", true);
    }

    private bool FindMagnitude()
    {
        bool withinDistance = false;

        Vector3 offset = player.position - transform.position;
        float magnitude = Mathf.Sqrt(Mathf.Pow(offset.x, 2) + Mathf.Pow(offset.y, 2));

        if (magnitude < dangerClose)
        {
            withinDistance = true;
        }

        return withinDistance;
    }

    //::::::::::::UP::::::::::::://

    private void Up()
    {
        anim.SetBool("IsDown", false);
        anim.SetBool("IsUp", true);
    }
}
