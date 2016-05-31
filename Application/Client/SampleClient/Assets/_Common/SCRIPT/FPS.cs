//-------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
//-------------------------------------------------------------------------------------------------

public class FPS : MonoBehaviour
{
    void Update()
    {
        mDeltaTime += (Time.deltaTime - mDeltaTime) * 0.1f;
    }

    public float GetFPS()
    {
        return 1.0f / mDeltaTime;
    }

    void OnGUI()
    {
        GUI.TextField(new Rect(0, 0, 100, 20), "FPS : " + GetFPS().ToString(".00"));
    }


    private float mDeltaTime;
}
//-------------------------------------------------------------------------------------------------