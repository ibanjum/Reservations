using System;
using System.Collections.Generic;
using Constructivity.Core;

namespace cvtandroid
{
    public static class Tessellation
    {

        /// <summary>
        /// Converts a polygonal mesh into a triangular mesh with normals if needed
        /// </summary>
        /// <param name="meshSource"></param>
        /// <returns></returns>
        public static Mesh Tesselate(Mesh meshSource)
        {
            if (!meshSource.HasPolygons)
                return meshSource;

            List<double> verts = new List<double>();
            List<long> vertindex = new List<long>();

            BuildFaces(verts, vertindex, meshSource);

            Mesh meshTarget = new Mesh(null, meshSource.Material, verts.ToArray(), vertindex.ToArray(), true, false, false, null);
            return meshTarget;
        }

        public static int GetMeshCellCount(Constructivity.Core.Mesh mesh)
        {
            int count = 3;
            if (mesh.HasNormalVectors)
                count += 3;

            if (mesh.HasTextureCoordinates)
                count += 2;

            return count;
        }

        /// <summary>
        /// Builds faces from polygonal mesh
        /// </summary>
        /// <param name="verts">Target vertices to fill</param>
        /// <param name="vertindex">Target faces to fill</param>
        /// <param name="cfs">Source mesh</param>
        public static void BuildFaces(
            List<double> verts,
            List<long> vertindex,
            Constructivity.Core.Mesh cfs)
        {
            int nSourceCells = GetMeshCellCount(cfs); // normals, but no texture coordinates
            int nTargetCells = 6; // position, normal

            int i = 0;
            while (i < cfs.Faces.Length)
            {
                int nVerts = (int)cfs.Faces[i];
                if (nVerts == 0)
                    return;

                i++;

                // assume convex for now...
                bool isconvex = true;
                if (isconvex)
                {
                    // vertices
                    int iBaseVert = verts.Count / nTargetCells;

                    // calculate normal based on cross product of the first two points
                    int index = (int)cfs.Faces[i + 0];
                    double ax = cfs.Vertices[index * nSourceCells + 0];
                    double ay = cfs.Vertices[index * nSourceCells + 1];
                    double az = cfs.Vertices[index * nSourceCells + 2];
                    index = (int)cfs.Faces[i + 1];
                    double bx = cfs.Vertices[index * nSourceCells + 0];
                    double by = cfs.Vertices[index * nSourceCells + 1];
                    double bz = cfs.Vertices[index * nSourceCells + 2];
                    double nx = ay * bz - az * by;
                    double ny = az * bx - ax * bz;
                    double nz = ax * by - ay * bx;

                    for (int j = 0; j < nVerts; j++)
                    {
                        index = (int)cfs.Faces[i + j];
                        double x = cfs.Vertices[index * nSourceCells + 0];
                        double y = cfs.Vertices[index * nSourceCells + 1];
                        double z = cfs.Vertices[index * nSourceCells + 2];

                        verts.Add(x);
                        verts.Add(y);
                        verts.Add(z);

                        verts.Add(nx);
                        verts.Add(ny);
                        verts.Add(nz);
                    }

                    // faces based on triangle strips
                    for (int iThisVert = 2; iThisVert < nVerts; iThisVert++)
                    {
                        vertindex.Add(iBaseVert);
                        vertindex.Add(iBaseVert + iThisVert);
                        vertindex.Add(iBaseVert + iThisVert - 1);
                    }
                }
                i += nVerts;
            }

        }
    }
}
