using UnityEngine;

public class Enemy_2 : Enemy
{
    //public Score scoreCounter;

    [Header("Enemy_2 Inscribed Fields")]
    public float lifeTime = 10;
    [Tooltip("Determines how much the sine wave will ease the interpolation")]
    public float sinEccentricity = 0.6f;
    public AnimationCurve rotCurve;
    [Header("Enemy_2 Private Fields")]
    [SerializeField] private float birthTime;
    private Quaternion baseRotation;
    [SerializeField] private Vector3 p0, p1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        p0 = Vector3.zero;
        p0.x = -bndCheck.camWidth - bndCheck.radius;
        p0.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        p1 = Vector3.zero;
        p1.x = bndCheck.camWidth + bndCheck.radius;
        p1.y = Random.Range(-bndCheck.camHeight, bndCheck.camHeight);

        if (Random.value > 0.5f){
            p0.x *= -1;
            p1.x *= -1;
        }

        birthTime = Time.time;

        transform.position = p0;
        transform.LookAt(p1, Vector3.back);
        baseRotation = transform.rotation;

         GameObject scoreGO = GameObject.Find("ScoreCounter");
        scoreCounter = scoreGO.GetComponent<Score>();

        
    }

public override void Move(){
    float u = (Time.time - birthTime) / lifeTime;

    if (u > 1){
        Destroy(this.gameObject);
        return;
    }

    float shipRot = rotCurve.Evaluate(u) * 360;
    //if (p0.x > p1.x) shipRot = -shipRot;
    //transform.rotation = Quaternion.Euler(0,shipRot, 0);
    transform.rotation = baseRotation * Quaternion.Euler(-shipRot, 0, 0);

    u = u + sinEccentricity*(Mathf.Sin(u*Mathf.PI*2));

    pos = (1-u)*p0 + u*p1;
}

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
                    scoreCounter.score += 300;
                }
            }
            Destroy(otherGO);
        }
        else{
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }

       
    }
}
