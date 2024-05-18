using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit : MonoBehaviour
{
    //movement
    private Rigidbody2D rb;
    public float speed= 3;

    public Vector2 minPos;
    public Vector2 maxPos;
    
    private Vector3 nextPos;

    [Space]
    //infections
    public bool infected = false;
    public bool removed = false;
    public bool vaccinated = false;

    public float infectionRadius = 0.4f;
    public float removedTime = 30f;

    private bool infect = true;
    public bool recovery = false;
    public bool unitZero = false;

    public LayerMask whatIsInfected;

    [Space]
    //depends on mask rate
    public float probabilityOfInfection = 0.5f; //where 1 is 100%

    //display
    private SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        sr.color = Color.blue;
        infect = true;

    nextPos = new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), 0);
    }

    // Update is called once per frame
    void Update()
    {
        //random movement
        if(transform.position == nextPos)
        {
            nextPos = new Vector3(Random.Range(minPos.x, maxPos.x), Random.Range(minPos.y, maxPos.y), 0);
        }
        transform.position = Vector3.MoveTowards(transform.position,nextPos,speed*Time.deltaTime);

        //infected
        if (removed) { sr.color = Color.black; }
        else if (infected) { sr.color = Color.yellow; }
        else { sr.color = Color.white; }
        
        
        //every 10sec would be one day, getting infected would be by the end of the day
        if (infected && recovery)
        {
            StartCoroutine("RemovedTime");
        }
        
    }
    private void FixedUpdate()
    {
        if (Physics2D.OverlapCircle(transform.position, infectionRadius, whatIsInfected) &&(infect))
        {
            if (Random.Range(0f, 1f) < probabilityOfInfection)
            {
                infected = true;
                infect = false;
                //gameObject.layer = LayerMask.NameToLayer("Unit");//currently cannot infect people - set to infected unit when can
                StartCoroutine("Timer");
            }
        }

    }
    private IEnumerator RemovedTime()
    {
        yield return new WaitForSeconds(removedTime);
        removed = true;
        gameObject.layer = LayerMask.NameToLayer("Unit");
    }
    //debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, infectionRadius);
    }


    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(5f * speed); //tweak time for waiting
        infect = true;
    }
}
