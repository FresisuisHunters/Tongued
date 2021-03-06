﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649
[RequireComponent(typeof(LineRenderer), typeof(RopeCollider))]
public class TongueVisualizer : MonoBehaviour
{
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform headSpriteTransform;
    [SerializeField] private Vector2 defaultHeadSpriteFacing;

    private Transform swingerTransform;
    private LineRenderer lineRenderer;
    private RopeCollider ropeCollider;

    private Vector3[] points;


    private void FixedUpdate()
    {
        points = ropeCollider.GetRopePoints();
    }

    private void Update()
    {
        if (points == null) return;

        lineRenderer.enabled = true;

        points[0] = headTransform.position;
        points[points.Length - 1] = swingerTransform.position;

        lineRenderer.positionCount = points.Length;
        lineRenderer.SetPositions(points);

        Vector2 lastSegmentU = points[0] - points[1];
        float headSpriteAngle = Vector2.SignedAngle(defaultHeadSpriteFacing, lastSegmentU);
        headSpriteTransform.rotation = Quaternion.Euler(0, 0, headSpriteAngle);
    }

    private void OnClearedContacts()
    {
        points = null;
        lineRenderer.enabled = false;
    }


    private void Start()
    {
        swingerTransform = GetComponentInParent<Hook>().ConnectedBody.GetComponent<Transform>();
    }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        ropeCollider = GetComponent<RopeCollider>();

        ropeCollider.OnClearedContacts += OnClearedContacts;
    }
}
