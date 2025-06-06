using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting.APIUpdating;

[RequireComponent(typeof(BoundsCheck))]

public class Enemy : MonoBehaviour
{
    public Score scoreCounter;
    [Header("Inscribed")]

    public float speed = 10f; // the movement speed is 10m/s
    public float fireRate = 0.3f; // seconds/shot (unused)
    public float health = 10; // damage needd to destroy this enemy 
    public int score = 100; // points earned for destroying this
    public float powerUpDropChance = 1f; // chance to drop a PowerUp

    protected bool calledShipDestroyed = false;

    protected BoundsCheck bndCheck;

    void Start()
    {
        GameObject scoreGO = GameObject.Find("ScoreCounter");
        scoreCounter = scoreGO.GetComponent<Score>();
        
    }

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
    }



    // this is a property: a method that acts like a field
    public Vector3 pos {
        get {
            return this.transform.position;
        }

        set {
            this.transform.position = value;
        }
    }

    void Update()
    {
        Move();

        // check whether this enemy has gone off the bottom of the screen 
        if ( bndCheck.LocIs( BoundsCheck.eScreenLocs.offDown ) ) {
            Destroy( gameObject );
        }
    }

    public virtual void Move() {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    // void OnCollisionEnter( Collision coll ) 
    // {
    //     GameObject otherGO = coll.gameObject;
    //     if ( otherGO.GetComponent<ProjectileHero>() != null ) {
    //         Destroy( otherGO ); // destroy the projectile
    //         Destroy( gameObject ); // destory this enemy GameObject
    //     }
    //     else {
    //         Debug.Log( "Enemy hit by non-ProjectileHero: " + otherGO.name ); 
    //     }
    // }

    void OnCollisionEnter( Collision coll ){
        GameObject otherGO = coll.gameObject;

        ProjectileHero p = otherGO.GetComponent<ProjectileHero>();
        if (p != null){

            //Debug.Log("Weapon successfully subscribed to fireEvent.");

            if (bndCheck.isOnScreen){
                health -= Main.GET_WEAPON_DEFINITION(p.type).damageOnHit;
                if (health <= 0){
                    // tell main that this ship was destroyed
                    if (!calledShipDestroyed) {
                        calledShipDestroyed = true;
                        Main.SHIP_DESTROYED( this );
                    }
                    Destroy(this.gameObject);
                    scoreCounter.score += 100;
                }
            }
            Destroy(otherGO);
        }
        else{
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }
        
    }
}
