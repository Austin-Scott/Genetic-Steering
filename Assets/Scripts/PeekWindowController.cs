using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PeekWindowController : MonoBehaviour {

    public GameObject panel;
    public Text factionText;
    public Text stateText;
    public Text fuelText;

    private static bool isVisible=false;
    private static Boid boidSelected = null;
    private static Material material;

    public static void drawSelected()
    {
        if(isVisible && boidSelected!=null && boidSelected.getObj() != null)
        {
            Vector2 pos = boidSelected.getPosition();

            GL.PushMatrix();
            material.SetPass(0);
            GL.LoadOrtho();

            float theta = 0f;
            float radius = 1.5f;
            float thetaStep = (2f * Mathf.PI) / (20 - 1);

            GL.Begin(GL.LINE_STRIP);
            GL.Color(Color.cyan);
            for (int i = 0; i < 20; i++)
            {
                float x = radius * Mathf.Cos(theta);
                float y = radius * Mathf.Sin(theta);
                x += pos.x;
                y += pos.y;
                Vector3 screenPoint = Camera.main.WorldToScreenPoint(new Vector3(x, y));
                screenPoint.x /= Screen.width;
                screenPoint.y /= Screen.height;
                screenPoint.z = 0;
                GL.Vertex(screenPoint);
                theta += thetaStep;
            }
            GL.End();
            GL.PopMatrix();
        } 
    }

    public void hide()
    {
        Debug.Log("Hide called");
        isVisible = false;
        panel.SetActive(false);
        boidSelected = null;
    }

    public void show()
    {
        isVisible = true;
        panel.SetActive(true);
    }

	// Use this for initialization
	void Start () {
        material = new Material(Shader.Find("Particles/Additive"));
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            boidSelected = World.world.getClosestBoid(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (boidSelected != null)
            {
                show();
            }
        }
        if (isVisible)
        {
            if(boidSelected!=null && boidSelected.getObj()!=null)
            {
                factionText.text = boidSelected.getFaction();
                stateText.text = boidSelected.getState();
                fuelText.text = "Fuel: " + boidSelected.getFuel();
            } else
            {
                hide();
            }
        }
	}
}
