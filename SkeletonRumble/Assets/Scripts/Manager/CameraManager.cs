using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
//using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    public List<Transform> targets;
    public Transform cameraHolder;
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera multipleCamera;
    public CinemachineVirtualCamera dialogCamera;
    public CinemachineVirtualCamera endingCamera;
    public CinemachineMixingCamera mixedCamera;
    public float minFOV;
    public float maxFOV;
    public float size;

    public float smoothTime;
    private Vector2 smoothPos;
    private CameraState cameraState;
    public float lerpSpeed;

    private void LateUpdate()
    {
        switch (this.cameraState)
        {
            case CameraState.Player:
                this.mixedCamera.SetWeight(this.playerCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.playerCamera), 1, this.lerpSpeed * Time.deltaTime));
                this.mixedCamera.SetWeight(this.multipleCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.multipleCamera), 0, this.lerpSpeed * Time.deltaTime));
                this.mixedCamera.SetWeight(this.dialogCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.dialogCamera), 0, this.lerpSpeed * Time.deltaTime));
                this.mixedCamera.SetWeight(this.endingCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.endingCamera), 0, this.lerpSpeed * Time.deltaTime));
                break;
            case CameraState.Multiple:
                this.Multiple();
                this.mixedCamera.SetWeight(this.playerCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.playerCamera), 0, this.lerpSpeed * Time.deltaTime));
                this.mixedCamera.SetWeight(this.multipleCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.multipleCamera), 1, this.lerpSpeed * Time.deltaTime));
                this.mixedCamera.SetWeight(this.dialogCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.dialogCamera), 0, this.lerpSpeed * Time.deltaTime));
                this.mixedCamera.SetWeight(this.endingCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.endingCamera), 0, this.lerpSpeed * Time.deltaTime));
                break;
            case CameraState.Dialog:
                this.Dialog();
                this.mixedCamera.SetWeight(this.playerCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.playerCamera), 0, this.lerpSpeed * Time.deltaTime));
                this.mixedCamera.SetWeight(this.multipleCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.multipleCamera), 0, this.lerpSpeed * Time.deltaTime));
                this.mixedCamera.SetWeight(this.dialogCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.dialogCamera), 1, this.lerpSpeed * Time.deltaTime));
                this.mixedCamera.SetWeight(this.endingCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.endingCamera), 0, this.lerpSpeed * Time.deltaTime));
                break;
            case CameraState.Ending:
                this.mixedCamera.SetWeight(this.playerCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.playerCamera), 0, this.lerpSpeed * Time.deltaTime));
                this.mixedCamera.SetWeight(this.multipleCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.multipleCamera), 0, this.lerpSpeed * Time.deltaTime));
                this.mixedCamera.SetWeight(this.dialogCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.dialogCamera), 0, this.lerpSpeed * Time.deltaTime));
                this.mixedCamera.SetWeight(this.endingCamera, Mathf.MoveTowards(this.mixedCamera.GetWeight(this.endingCamera), 1, this.lerpSpeed * Time.deltaTime));
                break;
            default:
                break;
        }
    }

    public void SwitchToPlayer()
    {
        this.cameraState = CameraState.Player;
        print("SwitchToPlayerCam");
    }

    public void SwitchToDialog()
    {
        this.cameraState = CameraState.Dialog;
    }

    public void SwitchToMultiple()
    {
        this.cameraState = CameraState.Multiple;
    }

    public void SwitchToEnding()
    {
        this.cameraState = CameraState.Ending;
    }

    private void Multiple()
    {
        if (this.targets.Count == 0)
        {
            this.cameraHolder.position = this.transform.position;
            return;
        }

        //Update Camera Position
        Bounds targetBounds = this.GetCenterPoint(this.targets);
        Vector2 centeredPoint = targetBounds.center;
        this.cameraHolder.position = Vector2.SmoothDamp(this.cameraHolder.position, centeredPoint, ref this.smoothPos, this.smoothTime);

        //Update Camera Zoom
        float distance = this.targets.Count == 2 ? targetBounds.size.x : Vector2.Distance(this.targets[0].position, this.targets[1].position);
        float newFOV = Mathf.Lerp(this.minFOV, this.maxFOV, distance / this.size);
        this.multipleCamera.m_Lens.FieldOfView = newFOV;
    }

    private void Dialog()
    {
        this.cameraHolder.transform.position = this.GetCenterPoint(this.targets).center;
    }

    private Bounds GetCenterPoint(List<Transform> targets)
    {
        Bounds bounds = new Bounds(targets[0].position, Vector2.zero);

        for (int i = 0; i < targets.Count; i++)
        {
            bounds.Encapsulate(targets[i].position);
        }

        return bounds;
    }

    public enum CameraState
    {
        Player,
        Multiple,
        Dialog,
        Ending
    }

    //public void OnJoinPlayer()
    //{
    //    PlayerCharacter[] players = FindObjectsOfType<PlayerCharacter>();

    //    foreach (PlayerCharacter item in players)
    //    {
    //        if (this.targetGroup.m_Targets[0].target == null)
    //        {
    //            this.targetGroup.m_Targets[0].target = item.transform;
    //            this.playerTransform = item.transform;
    //            float r = 0.208f;
    //            float g = 0.502f;
    //            float b = 0.831f;
    //            item.meshrender.material.color = new Color(r, g, b, 1);
    //        }

    //        if (this.targetGroup.m_Targets[1].target == null && this.targetGroup.m_Targets[0].target != item.transform)
    //        {
    //            this.targetGroup.m_Targets[1].target = item.transform;
    //            this.enemyTransform = item.transform;
    //            float r = 0.745f;
    //            float g = 0.694f;
    //            float b = 0.306f;
    //            item.meshrender.material.color = new Color(r, g, b, 1);
    //        }
    //    }
    //}
}
