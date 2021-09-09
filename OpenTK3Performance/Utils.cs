using OpenTK;

using System;
using System.Collections.Generic;
using System.IO;

namespace OpenTK3Performance
{
    static class Utils
    {
        public static void GetWavefrontData(string file, out Vector3[] vertices, out Vector2[] uvMaps, out Vector3[] normals, out int[] indices)
        {
            List<Vector3> tempVertices = new List<Vector3>();
            List<Vector2> tempUVs = new List<Vector2>();
            List<Vector3> tempNormals = new List<Vector3>();

            List<int> vertexIndices = new List<int>();
            List<int> uvIndices = new List<int>();
            List<int> normalIndices = new List<int>();

            using (StreamReader sr = new StreamReader(file))
            {
                while (sr.Peek() >= 0)
                {
                    string line = sr.ReadLine();
                    int firstSpaceIndex = line.IndexOf(' ');
                    string type = line.Substring(0, firstSpaceIndex);
                    Span<string> data = line.Substring(firstSpaceIndex + 1).Split(' ');
                    switch (type)
                    {
                        case "v":
                            if (float.TryParse(data[0], out float vertexX) &&
                                float.TryParse(data[1], out float vertexY) &&
                                float.TryParse(data[2], out float vertexZ))
                            {
                                tempVertices.Add(new Vector3(vertexX, vertexY, vertexZ));
                            }
                            break;
                        case "vt":
                            if (float.TryParse(data[0], out float u) &&
                                float.TryParse(data[1], out float v))
                            {
                                tempUVs.Add(new Vector2(u, -v));
                            }
                            break;
                        case "vn":
                            if (float.TryParse(data[0], out float normalX) &&
                                float.TryParse(data[1], out float normalY) &&
                                float.TryParse(data[2], out float normalZ))
                            {
                                tempNormals.Add(new Vector3(normalX, normalY, normalZ));
                            }
                            break;
                        case "f":
                            if (data.Length < 3) throw new Exception();

                            for (byte i = 0; i < data.Length; i++)
                            {
                                Span<string> vertexValues = data[i].Split('/');

                                if (int.TryParse(vertexValues[0], out int index) &&
                                    int.TryParse(vertexValues[1], out int texIndex) &&
                                    int.TryParse(vertexValues[2], out int normalIndex))
                                {
                                    vertexIndices.Add(index);
                                    uvIndices.Add(texIndex);
                                    normalIndices.Add(normalIndex);
                                }
                            }
                            break;
                        default:
                            continue;
                    }
                }
            }

            List<Vector3> in_vertices = new List<Vector3>();
            List<Vector2> in_uvs = new List<Vector2>();
            List<Vector3> in_normals = new List<Vector3>();

            int vertexIndicesLength = vertexIndices.Count;
            for (int i = 0; i < vertexIndicesLength; i++)
            {
                int vertexIndex = vertexIndices[i];
                int texIndex = uvIndices[i];
                int normalIndex = normalIndices[i];

                Vector3 vertex = tempVertices[vertexIndex - 1];
                Vector3 normal = tempNormals[normalIndex - 1];
                Vector2 texCoord = tempUVs[texIndex - 1];

                in_vertices.Add(vertex);
                in_normals.Add(normal);
                in_uvs.Add(texCoord);
            }

            List<Vector3> out_vertices = new List<Vector3>();
            List<Vector2> out_uvs = new List<Vector2>();
            List<Vector3> out_normals = new List<Vector3>();
            List<int> out_indices = new List<int>();
            int indexX = 0;
            int inVerticesLength = in_vertices.Count;
            int outVerticesLength = out_vertices.Count;
            for (int i = 0; i < inVerticesLength; i++)
            {
                bool found = false;
                for (int p = 0; p < outVerticesLength; p++)
                {
                    if (Vector3ApproximatelyEqual(in_vertices[i], out_vertices[p]) &&
                        Vector3ApproximatelyEqual(in_normals[i], out_normals[p]) &&
                        Vector2ApproximatelyEqual(in_uvs[i], out_uvs[p]))
                    {
                        indexX = p;
                        found = true;
                        break;
                    }

                }

                if (found)
                {
                    out_indices.Add(indexX);
                }
                else
                {
                    out_vertices.Add(in_vertices[i]);
                    out_uvs.Add(in_uvs[i]);
                    out_normals.Add(in_normals[i]);
                    out_indices.Add(out_vertices.Count - 1);
                }
            }

            vertices = out_vertices.ToArray();
            normals = out_normals.ToArray();
            uvMaps = out_uvs.ToArray();
            indices = out_indices.ToArray();
        }

        public static bool Vector3ApproximatelyEqual(Vector3 a, Vector3 b)
        {
            return MathHelper.ApproximatelyEqualEpsilon(a.X, b.X, 0.01f) &&
                   MathHelper.ApproximatelyEqualEpsilon(a.Y, b.Y, 0.01f) &&
                   MathHelper.ApproximatelyEqualEpsilon(a.Z, b.Z, 0.01f);
        }

        public static bool Vector2ApproximatelyEqual(Vector2 a, Vector2 b)
        {
            return MathHelper.ApproximatelyEqualEpsilon(a.X, b.X, 0.01f) &&
                   MathHelper.ApproximatelyEqualEpsilon(a.Y, b.Y, 0.01f);
        }
    }
}
