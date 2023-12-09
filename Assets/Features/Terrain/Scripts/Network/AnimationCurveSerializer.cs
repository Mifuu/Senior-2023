using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AnimationCurveSerializer : MonoBehaviour
{
    public static SerializedAnimationCurve Serialize(AnimationCurve curve)
    {
        SerializedAnimationCurve serializedCurve = new SerializedAnimationCurve();

        serializedCurve.keyFrameTimes = new float[curve.keys.Length];
        serializedCurve.keyFrameValues = new float[curve.keys.Length];

        for (int i = 0; i < curve.keys.Length; i++)
        {
            serializedCurve.keyFrameTimes[i] = curve.keys[i].time;
            serializedCurve.keyFrameValues[i] = curve.keys[i].value;
        }

        return serializedCurve;
    }

    public static AnimationCurve Deserialize(SerializedAnimationCurve serializedCurve)
    {
        AnimationCurve curve = new AnimationCurve();

        for (int i = 0; i < serializedCurve.keyFrameTimes.Length; i++)
        {
            curve.AddKey(serializedCurve.keyFrameTimes[i], serializedCurve.keyFrameValues[i]);
        }

        return curve;
    }
}

[System.Serializable]
public struct SerializedAnimationCurve : INetworkSerializable
{
    public float[] keyFrameTimes;
    public float[] keyFrameValues;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref keyFrameTimes);
        serializer.SerializeValue(ref keyFrameValues);
    }
}