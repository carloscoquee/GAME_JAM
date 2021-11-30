using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour
{
    public Animator animacion;
    public uiControler controlador;
    
    [Header("Movimiento")] 
    public float moveSpeed;
    private Vector2 inputActualMovimiento;
    private Rigidbody2D rb;

    [Header("Salto")] 
    public float potenciaSalto;
    public LayerMask capaSuelo;
    private bool tocaSuelo;
    public Transform pies;
    public float radio;

    [Header("Atributos")] 
    public float fuerza;
    public int vida;
    public float infeccion;
    public float duracionMut;
    public float fortaleza;

    [Header("Mutaciones")] 
    public GameObject mutAcido;

    public bool ataqueAcido;
    public GameObject mutEscudo;
    public GameObject mutFortaleza;
    public int cerebros;
    
    [Header("Combate")]
    public bool melee;
    public float rangoAtaque;

    public bool vivo;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animacion = GetComponent<Animator>();
        ataqueAcido = false;
        vivo = true;
    }

    private void FixedUpdate()
    {
        if (vivo)
        {
            //Movimiento
            Move();
            tocaSuelo = Physics2D.OverlapCircle(pies.position, radio, capaSuelo);
            animacion.SetFloat("speed", Mathf.Abs(rb.velocity.x));
            animacion.SetBool("salta", !tocaSuelo);
            animacion.SetBool("ataqueAcido", ataqueAcido);

            melee = Physics2D.OverlapCircle(transform.position, rangoAtaque, LayerMask.GetMask("Enemigo"));

            //Cambiar de dirección al caminar
            if (rb.velocity.x < 0)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = true;
                GameObject.Find("AtaquePlayer").GetComponent<CircleCollider2D>().offset = new Vector2(1, 0);
            }

            if (rb.velocity.x > 0)
            {
                gameObject.GetComponent<SpriteRenderer>().flipX = false;
                GameObject.Find("AtaquePlayer").GetComponent<CircleCollider2D>().offset = new Vector2(-1, 0);
                
            }

            if (cerebros == 50)
            {
                RandomPowerUp();
                cerebros = 0;
            }

            if (duracionMut > 0)
            {
                duracionMut -= Time.deltaTime;
            }

            if (duracionMut <= 0)
            {
                ModNormal();
            }

            if (vida <= 0)
            {
                vivo = false;
            }
        }
        else
        {
            Muerto();
        }
    }

    public void Move()
    {
        Vector2 dir = new Vector2(inputActualMovimiento.x * moveSpeed, rb.velocity.y);
        rb.velocity = dir;
    }

    public void CapturarTeclaMovimiento(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            inputActualMovimiento = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            inputActualMovimiento = Vector2.zero;
        }
    }
    
    public void CapturarTeclaSalto(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (tocaSuelo)
            {
                rb.AddForce(Vector2.up * potenciaSalto, ForceMode2D.Impulse);
            }
        }
    }

    public void CapturarTeclaGolpe(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            animacion.SetTrigger("golpe");
        }
    }

    public void RandomPowerUp()
    {
        int aleatorio = Random.Range(0, 3);
        if (aleatorio == 1)
        {
            Instantiate(mutAcido, transform.position + Vector3.up * 7.5f, Quaternion.identity);
        }
        if (aleatorio == 2)
        {
            Instantiate(mutEscudo, transform.position + Vector3.up * 7.5f, Quaternion.identity);
        }
        if (aleatorio == 3)
        {
            Instantiate(mutFortaleza, transform.position + Vector3.up * 7.5f, Quaternion.identity);
        }
    }

    public void ModNormal()
    {
        ataqueAcido = false;
        fuerza = 1;
        moveSpeed = 5;
        fortaleza = 1;
        potenciaSalto = 5;
    }
    public void ModAcido()
    {
        ataqueAcido = true;
        duracionMut = 15;
        if (melee)
        {
            fuerza = 2;
        }
        else
        {
            fuerza = 1;
        }
    }

    public void ModFortaleza()
    {
        duracionMut = 15;
        fuerza = 3;
        fortaleza = 2;
        potenciaSalto = 10;
        moveSpeed = 2.5f;
        
    }

    public void ModEscudo()
    {
        duracionMut = 5;
        fortaleza = 999999;
        moveSpeed = 10f;
    }

    public void recibeDanyo(int danyo)
    {
        vida -= danyo;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ataque"))
        {
            float danyo = other.gameObject.GetComponentInParent<EnemyController>().fuerza / fortaleza;
            recibeDanyo((int)danyo);
        }
    }
    

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Proveta"))
        {
            switch (other.gameObject.name)
            {
                case "ProvetaDef":
                    ModFortaleza();
                    break;
                case "ProvetaVerde":
                    ModAcido();
                    break;
                case "ProvetaEscudo":
                    ModEscudo();
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    public void Muerto()
    {
        controlador.menuGO.SetActive(true);
    }
}
