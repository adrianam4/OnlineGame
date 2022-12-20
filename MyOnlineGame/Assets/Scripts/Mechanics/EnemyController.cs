using System.Collections;
using System.Collections.Generic;
using Platformer.Gameplay;
using UnityEngine;
using static Platformer.Core.Simulation;

namespace Platformer.Mechanics
{
    /// <summary>
    /// A simple controller for enemies. Provides movement control over a patrol path.
    /// </summary>
    [RequireComponent(typeof(AnimationController), typeof(Collider2D))]
    public class EnemyController : MonoBehaviour
    {
        public bool isDying = false;
        public PatrolPath path;
        public AudioClip ouch;
        public int id;
        internal PatrolPath.Mover mover;
        internal AnimationController control;
        internal Collider2D _collider;
        internal AudioSource _audio;
        SpriteRenderer spriteRenderer;
        GameObject data;
        public Bounds Bounds => _collider.bounds;
        Vector3 newPos;
        Quaternion newrot;
        void Awake()
        {
            data = GameObject.Find("CHAT/Data");
            control = GetComponent<AnimationController>();
            _collider = GetComponent<Collider2D>();
            _audio = GetComponent<AudioSource>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            newPos=new Vector3(0,0,0);
            newrot = Quaternion.identity;
        }
        public void MakeDead()
        {
            this.GetComponent<Health>().currentHP = 0;
            this.GetComponent<Health>().Decrement();
        }
        void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                var ev = Schedule<PlayerEnemyCollision>();
                ev.player = player;
                ev.enemy = this;
                data.GetComponent<DataSerialization>().yourenemyDownID = id; ;
            }
        }

        void Update()
        {
            if (isDying == true)
            {
                path = null;
                newPos.Set(this.gameObject.transform.position.x, this.gameObject.transform.position.y-0.05f, 0);
                this.gameObject.transform.SetPositionAndRotation(newPos, newrot);
                if (this.gameObject.transform.position.y < -8)
                {
                    this.gameObject.SetActive(false);
                }
            }
            if (path != null)
            {
                if (mover == null) mover = path.CreateMover(control.maxSpeed * 0.5f);
                control.move.x = Mathf.Clamp(mover.Position.x - transform.position.x, -1, 1);
            }
        }

    }
}