using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float velocidad;
    public Transform player;
    private bool vivo;
    public Animator animacion;

    [Header("Combate")]
    public int fuerza;
    public float vision;
    public float rango;
    public float cooldown;
    public float temporizador;
    public float vida;
    // Update is called once per frame
    private void Awake()
    {
        animacion = GetComponent<Animator>();
    }

    private void Start()
    {
        vivo = true;
    }

    void FixedUpdate()
    {
        if (vivo)
        {
            temporizador += Time.deltaTime;
            float distancia = Vector2.Distance(player.position, transform.position);

            if (distancia < vision && distancia > rango)
            {
                
                transform.position =
                    Vector2.MoveTowards(transform.position, new Vector2(player.position.x, transform.position.y), velocidad * Time.deltaTime);
                animacion.SetBool("walking", true);
            }
            else
            {
                animacion.SetBool("walking", false);
            }

            if (player.position.x > transform.position.x)
            {
                transform.localScale = new Vector3(1,1,1);
            }
            else
            {
                transform.localScale = new Vector3(-1,1,1);
            }
            
            if (temporizador > cooldown && distancia < rango)
            {
                animacion.SetTrigger("attack");
                temporizador = 0;
            }

            if (gameObject.name == "EnemigoSwat")
            {
                if (distancia < 3f && temporizador > 2)
                {
                    transform.position = Vector2.MoveTowards(transform.position, player.position, -velocidad * Time.deltaTime);
                    temporizador = 0;
                }
            }

            if (vida <= 0)
            {
                vivo = false;
            }
        }
        else
        {
            animacion.SetBool("dead", true);
            Invoke("Muerto",2);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("AtaquePlayer"))
        {
            vida -= GameObject.Find("Player").GetComponent<PlayerController>().fuerza;
        }
    }

    private void Muerto()
    {
        Destroy(gameObject);
    }
}
