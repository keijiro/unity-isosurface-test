using UnityEngine;
using System.Collections;

public class IsosurfaceController : MonoBehaviour
{
    MeshFilter meshFilter;
    const int division = 64;
    float[,,] voxels;
    Vector3[,,] gradient;
    public float target = 0.5f;
	public float noise = 1.0f;
    public float noiseScale = 0.1234f;
    public Vector3 noiseMove = Vector3.up;
	public int octave = 1;

    void Awake ()
    {
        meshFilter = GetComponent<MeshFilter> ();
    }

    void Start ()
    {
        voxels = new float [division, division, division];
        gradient = new Vector3[division, division, division];

        MarchingCubes.SetWindingOrder (2, 1, 0);
    }
    
    void Update ()
    {
        var offset = noiseMove * Time.time;
        var dx = Vector3.right * (noiseScale / division);
        var dy = Vector3.up * (noiseScale / division);
        var dz = Vector3.forward * (noiseScale / division);

        for (var iz = 0; iz < division; iz++)
        {
            for (var iy = 0; iy < division; iy++)
            {
                for (var ix = 0; ix < division; ix++)
                {
                    var pos = dx * ix + dy * iy + dz * iz + offset;
                    voxels [ix, iy, iz] = Perlin.Fbm(pos, octave) * noise + 1.0f * iz / division;
                }
            }
        }

        for (var iz = 1; iz < division - 1; iz++)
        {
            for (var iy = 1; iy < division - 1; iy++)
            {
                for (var ix = 1; ix < division - 1; ix++)
                {
                    var gx = voxels[ix + 1, iy, iz] - voxels[ix - 1, iy, iz];
                    var gy = voxels[ix, iy + 1, iz] - voxels[ix, iy - 1, iz];
                    var gz = voxels[ix, iy, iz + 1] - voxels[ix, iy, iz - 1];
                    gradient[ix, iy, iz] = new Vector3(gx, gy, gz);
                }
            }
        }

        var oldMesh = meshFilter.sharedMesh;

        MarchingCubes.SetTarget (target);

        var mesh = MarchingCubes.CreateMesh (voxels, gradient);

        meshFilter.sharedMesh = mesh;

        if (oldMesh != null)
            Destroy (oldMesh);
    }
}
