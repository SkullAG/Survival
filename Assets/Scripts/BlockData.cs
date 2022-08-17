using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockData : MonoBehaviour
{
    public string id = "sk:air";
    public NormalBlockFaces faces;

    public bool transparent { get { return (id == "sk:air"); } }

    private Chunk _parent;
    public Chunk parent { get { return _parent; } set { 
            _parent = value;

            if(transform.parent != value.transform)
            {
                transform.parent = value.transform;
                transform.localPosition = Vector3Int.FloorToInt(transform.localPosition);
                transform.forward = value.transform.forward;
                transform.localScale = Vector3.one;
            }
        } }

    private void Awake()
    {
        parent = gameObject.transform.parent.GetComponent<Chunk>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(id == "sk:air")
        {
            this.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [System.Serializable]
    public class NormalBlockFaces
    {
        public GameObject left, right, back, front, up, bottom;

        void updateFaces()
        {
            /*left.SetActive(false);
            right.SetActive(false);
            back.SetActive(false);
            front.SetActive(false);
            up.SetActive(false);
            bottom.SetActive(false);*/
        }
    }
}
