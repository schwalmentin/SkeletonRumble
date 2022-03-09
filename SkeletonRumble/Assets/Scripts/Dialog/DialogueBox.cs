using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public bool appearAnimation = false;
    public bool anglesSet = false;

    [HideInInspector] public float startAngle;
    [HideInInspector] public float endAngle;
    [HideInInspector] public float rotationSpeed;



    private void OnDisable()
    {
        anglesSet = false;

    }
    void Update()
    {
        
        if (!anglesSet)
        {
            anglesSet = true;
            appearAnimation = true;

            Transform[] ChildObjects = this.GetComponentsInChildren<Transform>();
            ChildObjects[1].transform.eulerAngles = new Vector3(0, 0, startAngle);
        }

        if (appearAnimation)
        {
            Transform[] ChildObjects = this.GetComponentsInChildren<Transform>();
            //print("Pivot Rotation(X:" + ChildObjects[1].transform.eulerAngles.x + " Y:" + ChildObjects[1].transform.eulerAngles.y + " Z:" + ChildObjects[1].transform.eulerAngles.z + ")");

            ChildObjects[1].rotation = Quaternion.Lerp(ChildObjects[1].rotation, Quaternion.Euler(0, 0, endAngle), rotationSpeed * Time.deltaTime);
            if (ChildObjects[1].localEulerAngles.z == endAngle) { appearAnimation = false; }
        }
    }
}
