using UnityEngine;
using System.Collections; 

[RequireComponent(typeof(BoundsCheck), typeof(Rigidbody))]
public class PowerUp : MonoBehaviour
{
    [Header("Inscribed")]
    [Tooltip("x holds a min value and y a max value for a Random.Range() call.")]
    public Vector2 rotMinMax = new Vector2(15, 90);
    [Tooltip("x holds a min value and y a max value for a Random.Range() call")]
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 10; // Powerup will exist for # seconds
    public float fadeTime = 4;  // Then it fades over # seconds

    [Header("Dynamic")]
    [SerializeField] private eWeaponType _type; // Backing field for type property
    public GameObject cube; // Reference to powercube child
    public TextMesh letter; // Reference to TextMesh component
    public Vector3 rotPerSecond; // Euler rotation speed for powercube
    public float birthTime; // The Time.time this was instantiated

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Material cubeMat;

    void Awake()
    {
        // Find the cube reference (there's only a single child)
        cube = transform.GetChild(0).gameObject;
        
        // Find the TextMesh and other components
        letter = GetComponentInChildren<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeMat = cube.GetComponent<Renderer>().material;

        // Set a random velocity
        Vector3 vel = Random.onUnitSphere;
        vel.z = 0; // Flatten the velocity to XY plane
        vel.Normalize();
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.linearVelocity = vel;

        // Set initial rotation
        transform.rotation = Quaternion.identity;

        // Randomize rotation speed
        rotPerSecond = new Vector3(
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y),
            Random.Range(rotMinMax.x, rotMinMax.y)
        );

        birthTime = Time.time;
    }

    void Update()
    {
        // Rotate the cube
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);

        // Handle fading
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        
        if (u >= 1)
        {
            Destroy(gameObject);
            return;
        }

        if (u > 0)
        {
            // Fade cube
            Color c = cubeMat.color;
            c.a = 1f - u;
            cubeMat.color = c;

            // Fade letter (slower than cube)
            if (letter != null)
            {
                c = letter.color;
                c.a = 1f - (u * 0.5f);
                letter.color = c;
            }
        }

        // Destroy if off-screen
        if (bndCheck != null && !bndCheck.isOnScreen)
        {
            Destroy(gameObject);
        }
    }

    public eWeaponType type
    {
        get { return _type; }
        set { SetType(value); }
    }

    public void SetType(eWeaponType wt)
    {
        WeaponDefinition def = Main.GET_WEAPON_DEFINITION(wt);
        cubeMat.color = def.powerUpColor;
        
        if (letter != null)
        {
            letter.text = def.letter;
        }
        
        _type = wt;
    }

    public void AbsorbedBy(GameObject target)
    {
        Destroy(gameObject);
    }
}
