using UnityEngine;
using System.Collections;
using System.Collections.Generic;

    [RequireComponent( typeof(BoundsCheck) )]
    public class ProjectileHero : MonoBehaviour {
        private BoundsCheck bndCheck;
        private Renderer rend;

        [Header("Dynamic")]
        public Rigidbody rigid;
        [SerializeField]
        private eWeaponType _type;

        public eWeaponType type{
            get{return(_type);}
            set{SetType(value);}
        }


        void Awake()
        {
            bndCheck = GetComponent<BoundsCheck>();
            rend = GetComponent<Renderer>();
            rigid = GetComponent<Rigidbody>();
        }

        void Update()
        {
            if ( bndCheck.LocIs(BoundsCheck.eScreenLocs.offUp) ) {
                Destroy( gameObject );
            }
        }

        public void SetType(eWeaponType eType){
            _type = eType;
            WeaponDefinition def = Main.GET_WEAPON_DEFINITION(_type);
            rend.material.color = def.projectileColor;
        }

        public Vector3 vel{
            get{return rigid.linearVelocity;}
            set{rigid.linearVelocity = value;}
        }
    }

