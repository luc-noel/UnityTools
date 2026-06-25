using PhantomCompass;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Randomise activation, rotation, scale, (what-ever else I feel like), for groups of objects.
// Runs on enable
public class VisualRandomiser : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> _objects;

    [Header("Enable")]
    [SerializeField]
    private bool _randomiseEnabled;
    [SerializeField, Tooltip("Range of how many of objects can be randomly enabled. If set to -1 then the limit is ignored, this can result in 0 or all components being activated.")]
    private Vector2Int _minMaxEnabled = new Vector2Int(-1, -1);

    [Header("Scale")]
    [SerializeField]
    private bool _randomiseScale;
    [SerializeField, Tooltip("Multiply the current scale instead of setting the value?")]
    private bool _multiplyScale;
    [SerializeField, Tooltip("Range of how much to change the object's scale by. Uniformly resizes.")]
    private Vector2 _scaleRange = new Vector2(0.85f, 1.15f);

    [Header("Flip")]
    [SerializeField, Tooltip("Randomly flip the game object? Achieved by negating the scale axis.")]
    private bool _randomiseFlip;
    [SerializeField, Tooltip("Setting an axis to 1 will allow that axis to be randomly flipped. Any other value ignores that axis.")]
    private Vector3 _flipAxis = Vector3.zero;

    [Header("Rotation")]
    [SerializeField, Tooltip("Add to the current world rotation of each object within a range? Rotation Range Minimum and Rotation Range Maximum set the rotation range values.")]
    private bool _randomiseRotation;
    [SerializeField]
    private Vector3 _rotationRangeMinimum = Vector3.zero;
    [SerializeField]
    private Vector3 _rotationRangeMaximum = Vector3.zero;

    void Awake()
    {
        RandomiseAll();
    }

    private void RandomiseEnable()
    {
        if (!_randomiseEnabled)
            return;

        foreach (GameObject obj in _objects)
            obj.SetActive(false);

        // How many objects will get randomised, totally not doing any checks on the number being provided being safe
        int range = Random.Range(_minMaxEnabled.x == -1 ? 0 : _minMaxEnabled.x, _minMaxEnabled.y == -1 ? _objects.Count : _minMaxEnabled.y + 1);
        List<int> indexes = Enumerable.Range(0, _objects.Count).ToList();

        for (int i = 0; i < range; i++)
        {
            // Pick a random index in the list, activate it, then remove its index
            int rnd = Random.Range(0, indexes.Count);
            _objects[indexes[rnd]].SetActive(true);
            indexes.Remove(indexes[rnd]);
        }
    }

    private void RandomiseScale()
    {
        if (!_randomiseScale)
            return;

        foreach (GameObject obj in _objects)
        {
            if (_multiplyScale)
                obj.transform.localScale *= Random.Range(_scaleRange.x, _scaleRange.y);
            else
                obj.transform.localScale = Vector3.one * Random.Range(_scaleRange.x, _scaleRange.y);
        }
    }

    private void RandomiseFlip()
    {
        if (!_randomiseFlip)
            return;

        foreach (GameObject obj in _objects)
        {
            Vector3 flip = new Vector3
            (
                Random.value > 0.5 ? (_flipAxis.x == 1.0f ? -1.0f : 1.0f) : 1.0f,
                Random.value > 0.5 ? (_flipAxis.y == 1.0f ? -1.0f : 1.0f) : 1.0f,
                Random.value > 0.5 ? (_flipAxis.z == 1.0f ? -1.0f : 1.0f) : 1.0f
            );

            obj.transform.localScale = Vector3.Scale(obj.transform.localScale, flip);
        }
    }

    private void RandomiseWorldRotation()
    {
        if (!_randomiseRotation)
            return;

        foreach (GameObject obj in _objects)
        {
            Vector3 start = obj.transform.eulerAngles;
            Vector3 rndAngle = new Vector3
            (
                Random.Range(_rotationRangeMinimum.x, _rotationRangeMaximum.x),
                Random.Range(_rotationRangeMinimum.y, _rotationRangeMaximum.y),
                Random.Range(_rotationRangeMinimum.z, _rotationRangeMaximum.z)
            );

            obj.transform.rotation = Quaternion.Euler(start + rndAngle);
        }
    }

    [InspectorButton("Randomise All")]
    public void RandomiseAll()
    {
        RandomiseEnable();
        RandomiseScale();
        RandomiseFlip();
        RandomiseWorldRotation();
    }
}