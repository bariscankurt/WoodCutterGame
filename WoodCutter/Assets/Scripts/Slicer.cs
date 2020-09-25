using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EzySlice;

public class Slicer : MonoBehaviour
{
    public Touch touch;
    public Material mat;
    public LayerMask mask;
    public GameObject effect;
    public CameraShake cameraShake;
    public GameObject[] wood;
    public float rotationSpeed;
    
    private bool isCutting=false;
    int i = 0;
    private List<List<GameObject>> woods = new List<List<GameObject>>();
    // Start is called before the first frame update
    void Start()
    {

        

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f, 1000f * Time.deltaTime, 0f));
        if(isCutting == false)
        {

            
            
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x + 5f * Time.deltaTime, -1, 1), transform.position.y, transform.position.z);
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                transform.position = new Vector3(Mathf.Clamp(transform.position.x - 5f * Time.deltaTime, -1, 1), transform.position.y, transform.position.z);
            }
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    transform.position = new Vector3(transform.position.x + touch.deltaPosition.x * 0.01f, transform.position.y, transform.position.z);
                }
            }

        }

        if (Time.time > i)
        {
            i += 3;
            int rnd = Random.Range(0, 3);
            Instantiate(wood[rnd], new Vector3(-0.22f, 6.65f, 2.258223f), Quaternion.Euler(0, 0, 90));
        }
        
        
        Collider[] cuttingObjects = Physics.OverlapBox(transform.position,new Vector3(1f,0.1f,1f),transform.rotation,mask);
        foreach(Collider nesne in cuttingObjects)
        {
            
            isCutting = true;
            StartCoroutine(isCuttingWood());
                Debug.Log("trigger");
                
                SlicedHull cuttingObject = Cut(nesne.gameObject, mat);
                GameObject kesilmisUst = cuttingObject.CreateUpperHull(nesne.gameObject, mat);
                GameObject kesilmisAlt = cuttingObject.CreateLowerHull(nesne.gameObject, mat);
            Instantiate(effect, new Vector3(transform.position.x + 0.001f, transform.position.y - 0.4f,transform.position.z+ 0.632f), Quaternion.Euler(-40f, 180f, 0f));

                AddBody(kesilmisUst);
                AddBody(kesilmisAlt);
            
            Destroy(nesne.gameObject);
            FindObjectOfType<AudioManager>().Play("SawWorking");
            //GameObject.Find("SawIdle").GetComponent<AudioSource>().Play();
            Handheld.Vibrate();
            StartCoroutine(cameraShake.Shake(.8f, .1f));

        }
        

    }

    public SlicedHull Cut(GameObject obj, Material mat = null)
    {
        return obj.Slice(transform.position, transform.up, mat);
    }
    
    
    void AddBody(GameObject obj)
    {
        obj.AddComponent<MeshCollider>().convex = true;
        obj.AddComponent<Rigidbody>();
        obj.GetComponent<Rigidbody>().interpolation = RigidbodyInterpolation.Interpolate;
        obj.GetComponent<Rigidbody>().AddExplosionForce(100, obj.transform.position, 15);
        obj.GetComponent<Rigidbody>().velocity = obj.GetComponent<Rigidbody>().velocity / 2;
        StartCoroutine(DestroyAfterwards(obj, 3));
    }

    IEnumerator DestroyAfterwards(GameObject obj,float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(obj);
    }

    IEnumerator isCuttingWood()
    {
        
        yield return new WaitForSeconds(.8f);
        isCutting = !isCutting;
    }

    
}
