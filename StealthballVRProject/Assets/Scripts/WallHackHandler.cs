using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallHackHandler : MonoBehaviour {

    private Camera headCam;
    private Material material;

    [SerializeField]
    private LayerMask insideWallMask;
    [SerializeField]
    private LayerMask outsideWallMask;

    public static WallHackHandler singleton;

    private int _active = 0;
    public bool active {
        get {
            return (_active == 1);
        }
        set {
            if (value) {
                _active = 1;
            }
            else {
                _active = 0;
            }
        }
    }

    void Awake () {
        JBirdEngine.Singleton.ManageSingleton(this, ref singleton);
        headCam = GetComponent<Camera>();
        if (headCam == null) {
            Debug.LogError("WallHackHandler: Head camera refernece not set!");
        }
        material = new Material(Shader.Find("Hidden/InsideWallShader"));
    }

    void OnRenderImage (RenderTexture source, RenderTexture destination) {
        material.SetInt("_Active", _active);
        Graphics.Blit(source, destination, material);
    }

    void OnTriggerEnter () {
        active = true;
        headCam.cullingMask = insideWallMask;
    }

    void OnTriggerStay () {
        active = true;
        headCam.cullingMask = insideWallMask;
    }

    void OnTriggerExit () {
        active = false;
        headCam.cullingMask = outsideWallMask;
    }

}
