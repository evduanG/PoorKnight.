﻿using System;
using System.Collections;
using UnityEngine;

namespace Assets.player
{
    internal class PlayerController : MonoBehaviour
    {
        public bool isCollidingWithEnemy;
        public bool isOnGround = true;
        private Rigidbody2D rigidbodyPlayer;
        [SerializeField] float speed = 15f;
        public GameObject arrowPrefab;
        private Transform arrowSpawnPoint;
        private SpriteRenderer flipPlayer;
        private Animator anim;
        private float lastPressTime=0;
        private float timeSinceLastPress;
        private void Awake()
        {
            rigidbodyPlayer = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            arrowSpawnPoint = transform.Find("ArrowSpawnPoint");
            anim = GetComponentInChildren<Animator>();
            flipPlayer = GetComponentInChildren<SpriteRenderer>();
        }

        private void Update()
        {
            Walk();
            if (Input.GetKey(KeyCode.Space) && isOnGround)
            {
                anim.SetBool("isJumping", true);
                Jump();
            } else anim.SetBool("isJumping", false);

            if (Input.GetKeyDown(KeyCode.E)) 
            {
                timeSinceLastPress = Time.time - lastPressTime;
                lastPressTime = Time.time;
                if (timeSinceLastPress > 0.45f && flipPlayer.flipX == false)
                {
                    StartCoroutine(ArrowGenerator());
                    anim.SetBool("isShooting", true);
                }
            } else anim.SetBool("isShooting", false);
        }

        IEnumerator ArrowGenerator()
        {
            yield return new WaitForSeconds(0.3f);
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.Euler(0, 0, -90));
            arrow.GetComponent<Rigidbody2D>().velocity = new Vector2(60f, 3f);
            StopCoroutine(ArrowGenerator());
        }

        private void Walk()
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            rigidbodyPlayer.velocity = new Vector2(moveHorizontal * speed, rigidbodyPlayer.velocity.y);
            if (moveHorizontal > 0.01f)
            {
                flipPlayer.flipX = false;
                //anim.SetBool("isWalking", true);
            }
            if (moveHorizontal < -0.01f)
            {
                flipPlayer.flipX = true;
                //anim.SetBool("isWalking", true);
            }
            if (moveHorizontal == 0)
            {
                //anim.SetBool("isWalking", false);
            }
        }

        private void Jump()
        {
            rigidbodyPlayer.velocity = new Vector2(0, 10);
            isOnGround = false;
        }

        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                anim.SetBool("isDie", true);
                isCollidingWithEnemy = true;
                Destroy(gameObject, 0.3f);
            }
            if (collision.gameObject.CompareTag("Ground"))
                isOnGround = true;
            else isOnGround = false;
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                isCollidingWithEnemy = false;
            }
        }
    }
}
