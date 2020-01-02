using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMapScript : MonoBehaviour
{
    [SerializeField] private int startNode = 1;
    [SerializeField] private float lerpFactor = 0.25f;
    [SerializeField] private Vector3 defaultCamPosition = Vector3.zero;
    [SerializeField] private GameObject fadeObject = null;
    [SerializeField] private GameObject playerNode = null;
    [SerializeField] private DeckBuilder deckBuilder = null;

    private bool displacedFollowCam = false;
    private bool zoomToNode = true;

    private GameObject cam;
    private Transform nodes;
    private Animator animator;

    static private int moveToNode = -1;

    public void DisableAnimator()
    {
        animator.enabled = false;
        displacedFollowCam = true;
        zoomToNode = false;
    }

    void Start()
    {
        if(moveToNode < 0) moveToNode = startNode;

        cam = GameObject.Find("FollowCam");
        nodes = gameObject.transform.GetChild(2);
        animator = GetComponent<Animator>();

        cam.transform.position = nodes.GetChild(moveToNode - 1).transform.position;
    }

    void OnEnable()
    {
        if(moveToNode >= 0) moveToNode++;

        for (int i = 1; i < moveToNode; i++)
        {
            if(nodes == null) nodes = gameObject.transform.GetChild(2);

            if (nodes.GetChild(i).GetChild(0).name == "EnemyNode")
            {
                Destroy(nodes.GetChild(i).GetChild(0).gameObject);
                Instantiate(playerNode, nodes.GetChild(i));
            }
        }
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
                if (fadeObject.GetComponent<SpriteRenderer>().color.a > 0.85f)
                {
                    zoomToNode = false;
                    cam.transform.position = defaultCamPosition;

                    animator.enabled = true;
                    gameObject.SetActive(false);
                }

                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 3f, lerpFactor);
                //fadeObject.SetActive(true);
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

    public void Play( int level )
    {
        //Debug.Log( $"Clicked play: {level}" );

        Vector3 pos = defaultCamPosition;
        pos.z = -10f;
        cam.transform.position = pos;

        fadeObject.SetActive( true );
    }

    public void DeckBuilder( int level )
    {
        //Debug.Log( $"Clicked deck: {level}" );

        Vector3 pos = defaultCamPosition;
        pos.z = -10f;
        cam.transform.position = pos;

        deckBuilder.Show( );
        gameObject.SetActive( false );
    }
}
