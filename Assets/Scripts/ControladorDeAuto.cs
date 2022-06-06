using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class ControladorDeAuto : MonoBehaviour
{
    // INPUT
    private AutoInputAsset autoAciones;
    private InputAction movimiento;
    private InputAction freno;
    private InputAction resp;
    private InputAction respEse;
    
    // COLLIDERS y TRANSFORMS
    public WheelCollider ColliderRuedaDelanteraIzq;
    public WheelCollider ColliderRuedaDelanteraDer;
    public WheelCollider ColliderRuedaTraceraIzq;
    public WheelCollider ColliderRuedaTraceraDer;
    public Transform TransformRuedaDelanteraIzq;
    public Transform TransformRuedaDelanteraDer;
    public Transform TransformRuedaTraceraIzq;
    public Transform TransformRuedaTraceraDer;

    // FACTORES FISICOS
    private Rigidbody rb;
    private float anguloDireccion;
    public float anguloDireccionMax = 30f;
    public float motorFueza = 50f;
    public float frenoFuerza = 0f;

    //TEXTO UI (fps coso)
    private int avgFps;
    public TMP_Text fps;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //Inicializamos el Input System
        autoAciones = new AutoInputAsset();

        // Hacer al mouse invisible y bloquearlo en el centro
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        //Inicializamos los input acctions y los inicializamos
        freno = autoAciones.Auto.Freno;
        movimiento = autoAciones.Auto.Movimiento;
        resp = autoAciones.Auto.Respawn;
        respEse = autoAciones.Auto.ResEsena;
        autoAciones.Auto.Enable();
    }

    private void OnDisable()
    {
        //Deshabilitamos el input system para evitar leaks de memoria
        autoAciones.Auto.Disable();
    }

    private void Update()
    {
        float act = (int)(1f / Time.unscaledDeltaTime);
        avgFps = (int)act;
        fps.text = "FPS: " + avgFps.ToString();
    }

    private void FixedUpdate()
    {
        ManejarMotor();
        ManejarDireccion();
        ActualizarRuedas();
        verRespawn();
    }

    private void verRespawn()
    {
        if (resp.IsPressed())
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }else if (respEse.IsPressed())
        {
            SceneManager.LoadScene("test_auto_facha");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rbC = collision.gameObject.GetComponent<Rigidbody>();
        if (rbC != null)
        {
            rbC.AddForce(rb.velocity, ForceMode.Impulse);
        }
    }

    private void ManejarDireccion()
    {
        float velX = movimiento.ReadValue<Vector2>().x;
        anguloDireccion = velX == 0f ? 0f : velX * anguloDireccionMax;
        ColliderRuedaDelanteraIzq.steerAngle = anguloDireccion;
        ColliderRuedaDelanteraDer.steerAngle = anguloDireccion;
    }

    private void ManejarMotor()
    {
        float velY = movimiento.ReadValue<Vector2>().y;
        ColliderRuedaDelanteraIzq.motorTorque = velY == 0f ? 0f : velY * motorFueza;
        ColliderRuedaDelanteraDer.motorTorque = velY == 0f ? 0f : velY * motorFueza;

        frenoFuerza = freno.IsPressed() ? 1500f : 0f;
        ColliderRuedaTraceraIzq.brakeTorque = frenoFuerza;
        ColliderRuedaTraceraDer.brakeTorque = frenoFuerza;
    }

    private void ActualizarRuedas()
    {
        ActualizarRuedasPos(ColliderRuedaDelanteraIzq, TransformRuedaDelanteraIzq);
        ActualizarRuedasPos(ColliderRuedaDelanteraDer, TransformRuedaDelanteraDer);
        ActualizarRuedasPos(ColliderRuedaTraceraIzq, TransformRuedaTraceraIzq);
        ActualizarRuedasPos(ColliderRuedaTraceraDer, TransformRuedaTraceraDer);
    }

    private void ActualizarRuedasPos(WheelCollider wheelCollider, Transform trans)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        trans.rotation = rot;
        trans.position = pos;
    }

}