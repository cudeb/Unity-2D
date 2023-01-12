using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMap : MonoBehaviour
{
    public string transferMapName;

    public Transform target;
    public BoxCollider2D targetBound;

    public Animator anim_1;
    public Animator anim_2;

    public int door_count;

    [Tooltip("UP, DOWN, LEFT, RIGHT")]
    public string direction;
    private Vector2 vector;

    [Tooltip("¹®ÀÌ ÀÖÀ¸¸é true, ¹®ÀÌ ¾øÀ¸¸é false")]
    public bool door;

    private PlayerManager thePlayer;
    private CameraManager theCamera;
    private FadeManager theFade;
    private OrderManager theOrder;

    // Start is called before the first frame update
    void Start()
    {
        theCamera = FindObjectOfType<CameraManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theFade = FindObjectOfType<FadeManager>();
        theOrder = FindObjectOfType<OrderManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!door)
        {
            if (collision.gameObject.name == "Player")
            {
                StartCoroutine(TransferCoroutine());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(door)
        {
            if (collision.gameObject.name == "Player")
            {
                if (Input.GetKey(KeyCode.Z))
                {
                    vector.Set(thePlayer.animator.GetFloat("DirX"), thePlayer.animator.GetFloat("DirY"));
                    switch(direction)
                    {
                        case "UP":
                            if(vector.y ==  1f)
                            {
                                StartCoroutine(TransferCoroutine());
                            }
                            break;
                        case "DOWN":
                            if (vector.y == -1f)
                            {
                                StartCoroutine(TransferCoroutine());
                            }
                            break;
                        case "RIGHT":
                            if (vector.x == 1f)
                            {
                                StartCoroutine(TransferCoroutine());
                            }
                            break;
                        case "LEFT":
                            if (vector.x == -1f)
                            {
                                StartCoroutine(TransferCoroutine());
                            }
                            break;
                        default:
                            StartCoroutine(TransferCoroutine());
                            break;
                    }
                    
                }      
            }
        }
    }

    IEnumerator TransferCoroutine()
    {
        theOrder.PreLoadCharacter();
        theOrder.NotMove();
        theFade.FadeOut();
        if(door)
        {
            anim_1.SetBool("Open", true);
            if (door_count == 2)
            {
                anim_2.SetBool("Open", true);
            }
        }
        yield return new WaitForSeconds(0.3f);

        theOrder.SetTransparent("Player");
        if (door)
        {
            anim_1.SetBool("Open", false);
            if (door_count == 2)
            {
                anim_2.SetBool("Open", false);
            }
        }

        yield return new WaitForSeconds(0.7f);
        if (door)
        {
            anim_1.SetBool("Normal", true);
            if (door_count == 2)
            {
                anim_2.SetBool("Normal", true);
            }
            yield return new WaitForSeconds(0.1f);
            anim_1.SetBool("Normal", false);
            if (door_count == 2)
            {
                anim_2.SetBool("Normal", false);
            }
        }
        theOrder.SetUnTransparent("Player");
        thePlayer.currentMapName = transferMapName;
        theCamera.SetBound(targetBound);
        theCamera.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, theCamera.transform.position.z);
        thePlayer.transform.position = target.transform.position;
        theFade.FadeIn();
        yield return new WaitForSeconds(0.5f);
        theOrder.Move();
    }
}
