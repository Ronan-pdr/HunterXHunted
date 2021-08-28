using System;
using Script.Manager;
using UnityEngine;

namespace Script.EntityPlayer
{
    public class HumanHeart : MonoBehaviour
    {
        // ------------ Attributs ------------
        
        private Humanoide _mySelf;
        private float _timeLastHit;
        
        // degat
        private int _degatHit;
        private int _degatDeadZone;

        // ------------ Constructeur ------------
        private void Start()
        {
            _mySelf = GetComponentInParent<Humanoide>();

            if (MasterManager.Instance.GetTypeScene() == MasterManager.TypeScene.Game)
            {
                _degatHit = 13;
            }
            else
            {
                _degatHit = 33;
            }

            _degatDeadZone = 10;
        }

        // ------------ Events ------------
        private void OnTriggerEnter(Collider other)
        {
            Hit(other);
        }

        private void OnTriggerStay(Collider other)
        {
            Hit(other);
        }

        private void Hit(Collider other)
        {
            if (!_mySelf)
            {
                Destroy(gameObject);
                return;
            }
            
            // Si c'est pas à toi, tu ne fais rien
            if (!_mySelf.GetPv().IsMine)
                return;
            
            // Si c'est une Entity, on s'en fout
            if (other.GetComponent<Entity>() || other.CompareTag("SetRun"))
                return;

            // On n'enlève des points de vie seulement tous les certains temps
            if (Time.time - _timeLastHit < 1)
                return;
            
            _timeLastHit = Time.time;

            if (other.gameObject.CompareTag("DeadZone"))
            {
                if (!_mySelf.HasTheMasterPower)
                {
                    _mySelf.TakeDamage(_degatDeadZone);
                }
            }
            else
            {
                Debug.Log($"Le coeur de {_mySelf} est rentré en collision avec {other.name}");
                
                _mySelf.TakeDamage(_degatHit);
            }
        }
    }
}
