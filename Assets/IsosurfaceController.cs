using UnityEngine;
using System.Collections;

public class IsosurfaceController : MonoBehaviour
{
    MeshFilter meshFilter;
    const int division = 24;
    float[,,] voxels;
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
        voxels = new float [division * 2, division, division];

        MarchingCubes.SetWindingOrder (2, 1, 0);
        MarchingCubes.SetModeToCubes ();
//		MarchingCubes.SetModeToTetrahedrons ();
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
                for (var ix = 0; ix < division * 2; ix++)
                {
                    var pos = dx * ix + dy * iy + dz * iz + offset;
                    voxels [ix, iy, iz] = Perlin.Fbm(pos, octave) * noise + 1.0f * iz / division;
                }
            }
        }
        
        var oldMesh = meshFilter.sharedMesh;

        MarchingCubes.SetTarget (target);

        var mesh = MarchingCubes.CreateMesh (voxels);
        mesh.RecalculateNormals ();

        meshFilter.sharedMesh = mesh;

        if (oldMesh != null)
            Destroy (oldMesh);
    }
}
