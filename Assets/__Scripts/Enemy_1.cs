using UnityEngine;

public class Enemy_1 : Enemy
{

    //public Score scoreCounter;

    [Header("Enemy_1 Inscribed Fields")]
    [Tooltip("# of seconds for a full sine wave")]
    public float waveFrequency = 2;
    [Tooltip("Sine wave width in meters")]
    public float waveWidth = 4;
    [Tooltip("Amount the ship will roll left and right with the sine wave")]
    public float waveRotY = 45;

    private float x0;
    private float birthTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        x0 = pos.x;

        birthTime = Time.time;

         GameObject scoreGO = GameObject.Find("ScoreCounter");
        scoreCounter = scoreGO.GetComponent<Score>();
        
    }

    public override void Move(){
        Vector3 tempPos = pos;

        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        tempPos.x = x0 + waveWidth * sin;
        pos = tempPos;

        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        base.Move();

        print(bndCheck.isOnScreen);
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
                    scoreCounter.score += 200;
                }
            }
            Destroy(otherGO);
        }
        else{
            print("Enemy hit by non-ProjectileHero: " + otherGO.name);
        }

       
    }

   
}
