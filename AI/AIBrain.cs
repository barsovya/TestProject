//The class does not contain a full implementation of components, just an example
//Implementation of object movement and point search

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using DG.Tweening;

public class AIBrain : AIBrainHandler
{
    private protected Sequence AnimationSequence;

    private protected RaycastHit hit = new RaycastHit();

    [SerializeField]
    private protected List<AIPathPoint> PathPoints = new List<AIPathPoint>();

    private protected AIPathPoint LastPoint;

    [SerializeField]
    private protected WheelsParameters Wheels;

    [SerializeField]
    private protected IDrivingCharacteristics DrivingCharacteristics;

    [SerializeField]
    private protected IDefinitionObjectsNearby DefinitionObjectsNearby;

    private protected void Start()
    {
        StartCoroutine(nameof(MoveToPoint));
    }

    private protected IEnumerator MoveToPoint()
    {
        while (true)
        {
            yield return null;

            if (AnimationSequence != DOTween.Sequence())
                AnimationSequence = DOTween.Sequence();

            if (PathPoints.Count <= 0) continue;

            if (LastPoint != null)
                if (LastPoint == PathPoints[0])
                {
                    PathPoints.Remove(PathPoints[0]);
                    continue;
                }

            float smoothMovementTime = transform.position.CalculateDistanceToObject(PathPoints[0].transform.position) / DrivingCharacteristics.Speed;

            AnimationSequence.Insert(0, transform.DOMove(PathPoints[0].transform.position, smoothMovementTime).SetEase(Ease.Linear));
            AnimationSequence.Insert(0, transform.DOLookAt(PathPoints[0].transform.position, smoothMovementTime - DrivingCharacteristics.IntensityOfRotation).SetEase(Ease.Linear));

            yield return AnimationSequence.WaitForCompletion();

            LastPoint = PathPoints[0];
            PathPoints.Remove(PathPoints[0]);
        }
    }

    private protected void FixedUpdate()
    {
        if (PathPoints.Count >= 2)
            return;

        if (Time.time >= DefinitionObjectsNearby.NextCheckTime)
        {
            DefinitionObjectsNearby.NextCheckTime = Time.time + DefinitionObjectsNearby.CheckCollisionTimeInterval;

            Array.Clear(DefinitionObjectsNearby.Buffer, 0, DefinitionObjectsNearby.Buffer.Length);

            int count = Physics.OverlapBoxNonAlloc(DefinitionObjectsNearby.ObjectsCheckZone.position, DefinitionObjectsNearby.ObjectsCheckZone.localScale / 2,
                DefinitionObjectsNearby.Buffer, DefinitionObjectsNearby.ObjectsCheckZone.rotation, DefinitionObjectsNearby.LayerMask);

            float minDistance = 9000;

            DefinitionObjectsNearby.AllFoundPoints.Clear();

            for (int i = 0; i < DefinitionObjectsNearby.Buffer.Count(); i++)
            {
                if (DefinitionObjectsNearby.Buffer[i] == null)
                    continue;

                DefinitionObjectsNearby.TempPoint = DefinitionObjectsNearby.Buffer[i].gameObject.GetComponent<AIPathPoint>();

                if (!DetermineCorrectPoint(DefinitionObjectsNearby.TempPoint))
                {
                    DefinitionObjectsNearby.Buffer[i] = null;
                    count--;
                    continue;
                }

                if (PathPoints.SingleOrDefault(targetPoint => targetPoint == DefinitionObjectsNearby.TempPoint))
                    continue;

                float distance = transform.position.CalculateDistanceToObject(DefinitionObjectsNearby.TempPoint.transform.position);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    DefinitionObjectsNearby.BestPoint = DefinitionObjectsNearby.TempPoint;
                    DefinitionObjectsNearby.AllFoundPoints.Add(DefinitionObjectsNearby.TempPoint);
                }
            }

            DefinitionObjectsNearby.AllFoundPoints.Reverse();
            PathPoints.AddRange(DefinitionObjectsNearby.AllFoundPoints);
        }
    }
}
