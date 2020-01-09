using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapScript : MonoBehaviour
{
    [SerializeField] private int startNode = 1;
    [SerializeField] private float lerpFactor = 0.25f;
    [SerializeField] private Vector3 defaultCamPosition = Vector3.zero;
    [SerializeField] private GameObject fadeObjectBattle = null;
    [SerializeField] private GameObject fadeObjectDeck = null;
    [SerializeField] private GameObject playerNode = null;
    [SerializeField] private float nodeUIActivationTime = 6f;

    private bool displacedFollowCam = false;
    private bool zoomToNode = true;

    private GameObject cam;
    private Camera cameraForCoordsCalc;
    private Transform nodes;
    private Animator animator;

    private Vector3 startMousePosition = Vector3.zero;

    static private int moveToNode = -1;

    public void ToggleActivation(GameObject obj)
    {
        if (obj.transform.parent.parent == nodes.GetChild(moveToNode))
        {
            if (obj.activeSelf)
            {
                obj.GetComponent<Animator>().SetTrigger("hide");
                StartCoroutine(DisableAfterSeconds(obj, 0.5f));
            }
            else
            {
                obj.SetActive(true);
            }
        }
        else
        {
            zoomToNode = false;
        }
    }

    IEnumerator DisableAfterSeconds(GameObject obj, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }

    public void DisableAnimator()
    {
        animator.enabled = false;
        displacedFollowCam = true;
        zoomToNode = false;
    }

    void Start()
    {
        moveToNode = ProgressManager.Instance.MaxUnlockedLevel;
        moveToNode = moveToNode > ProgressManager.Instance.MaxLevels ? ProgressManager.Instance.MaxLevels : moveToNode;

        cam = GameObject.Find("FollowCam");
        nodes = gameObject.transform.GetChild(2);
        animator = GetComponent<Animator>();
        cameraForCoordsCalc = Camera.main;

        cam.transform.position = nodes.GetChild(moveToNode - 1).transform.position;

        Invoke( nameof( EnableNodeUI ), nodeUIActivationTime );
    }

    void OnEnable()
    {
        /*if (moveToNode >= 0) moveToNode++;

        for (int i = 1; i < moveToNode; i++)
        {
            if (nodes == null) nodes = gameObject.transform.GetChild(2);

            if (nodes.GetChild(i).GetChild(0).name == "EnemyNode")
            {
                Destroy(nodes.GetChild(i).GetChild(0).gameObject);
                Instantiate(playerNode, nodes.GetChild(i));
            }
        }*/
    }

    private void EnableNodeUI()
    {
        nodes.GetChild( moveToNode ).GetChild( 0 ).GetChild( 0 ).gameObject.SetActive( true );
    }

    void Update()
    {
        if (animator.enabled == false)
        {
            if (displacedFollowCam)
            {
                if (Mathf.Abs(cam.transform.position.x - nodes.GetChild(moveToNode - 1).position.x) < 0.2f
                && Mathf.Abs(cam.transform.position.y - nodes.GetChild(moveToNode - 1).position.y) < 0.2f
                && Vector3.Distance(transform.localScale, Vector3.one) < 0.15f)
                    displacedFollowCam = false;

                cam.transform.position = Vector3.Lerp(cam.transform.position,
                    nodes.GetChild(moveToNode - 1).transform.position, lerpFactor);


            }
            else if (zoomToNode)
            {
                if (fadeObjectBattle.GetComponent<SpriteRenderer>().color.a > 0.85f)
                {
                    zoomToNode = false;
                    cam.transform.position = defaultCamPosition;

                    animator.enabled = true;
                    gameObject.SetActive(false);
                }

                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 1.5f, lerpFactor);
                //fadeObject.SetActive(true);

                Vector3 mousePos = cameraForCoordsCalc.ScreenToViewportPoint(Input.mousePosition);
                mousePos.x -= 0.5f;
                mousePos.y -= 0.5f;
                mousePos *= 10.0f;

                if (Input.GetMouseButton(0))
                {
                    if (startMousePosition == Vector3.zero)
                    {
                        startMousePosition = mousePos;
                    }
                    else
                    {
                        cam.transform.position -= mousePos - startMousePosition;
                        startMousePosition = mousePos;
                    }
                }
                else
                {
                    startMousePosition = Vector3.zero;
                }
            }
            else
            {
                if (Mathf.Abs(cam.transform.position.x - nodes.GetChild(moveToNode).position.x) < 0.1f
                && Mathf.Abs(cam.transform.position.y - nodes.GetChild(moveToNode).position.y) < 0.1f)
                {
                    zoomToNode = true;
                    return;
                }

                cam.transform.position = Vector3.Lerp(
                    cam.transform.position, nodes.GetChild(moveToNode).transform.position, lerpFactor);
            }

            Vector3 pos = cam.transform.position;
            pos.z = -10f;
            cam.transform.position = pos;

            if (!zoomToNode)
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, lerpFactor);
        }
    }

    public void Play(int level)
    {
        ProgressManager.Instance.SelectedLevel = level;
        fadeObjectBattle.SetActive(true);
    }

    public void DeckBuilder(int level)
    {
        ProgressManager.Instance.SelectedLevel = level;
        fadeObjectDeck.SetActive(true);
    }
}
