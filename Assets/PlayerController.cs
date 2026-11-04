using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float velocidad = 5f;
    public Animator animator;

    public float fuerzaSalto = 10f;
    public float longitudRaycast = 0.1f;
    public LayerMask capaSuelo;
    private bool enSuelo;
    private Rigidbody2D rb;

    private bool recibiendoDanio;
    public float fuerzaRebote = 10f;

    void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float entradaX = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                entradaX = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                entradaX = 1f;
        }

        if (!recibiendoDanio)
        {
            Vector3 movimiento = new Vector3(entradaX, 0f, 0f) * velocidad * Time.deltaTime;
            transform.Translate(movimiento);

            if (entradaX > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (entradaX < 0) transform.localScale = new Vector3(-1, 1, 1);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, longitudRaycast, capaSuelo);
        enSuelo = hit.collider != null;

        if (enSuelo && Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame && !recibiendoDanio)
        {
            rb.AddForce(new Vector2(0f, fuerzaSalto), ForceMode2D.Impulse);
        }

        if (animator != null)
        {
            animator.SetFloat("movement", Mathf.Abs(entradaX));
            animator.SetBool("ensuelo", enSuelo);
            animator.SetBool("recibeDanio", recibiendoDanio);
        }
    }

    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            recibiendoDanio = true;
            Vector2 rebote = new Vector2(transform.position.x - direccion.x, 1).normalized;
            rb.AddForce(rebote * fuerzaRebote, ForceMode2D.Impulse);
        }
    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
        rb.velocity = Vector2.zero;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * longitudRaycast);
    }
}