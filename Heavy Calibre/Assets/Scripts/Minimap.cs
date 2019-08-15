using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour
{
    public float setMapSize;

    public static float mapSize;
    public static Image map;

    private void Start()
    {
        mapSize = setMapSize;
        map = transform.Find("Map").GetComponent<Image>();
    }

    private void Update()
    {
        map.rectTransform.anchoredPosition = -WorldToMiniMapPos(GameController.players[0].transform.position);
    }

    public static Vector3 WorldToMiniMapPos(Vector3 pos)
    {
        return (new Vector3(pos.x, pos.z, 0f) / (mapSize)) * map.rectTransform.sizeDelta.x;
    }
}