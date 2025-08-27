using UnityEngine;

public class cutcutcut : MonoBehaviour
{
    private GameObject objectToThrow;

    void Start()
    {
        objectToThrow = Resources.Load<GameObject>("ChaeunArrow");
    }

    void Throw()
    {
        Instantiate(objectToThrow);
    }

}
