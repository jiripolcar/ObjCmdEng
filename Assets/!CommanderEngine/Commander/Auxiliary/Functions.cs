using UnityEngine;

namespace CommanderEngine
{
    public static class Functions
    {
        public static float Dist2D(GameObject a, GameObject b)
        {
            return Dist2D(a.transform.position, b.transform.position);
        }

        public static float Dist2D(Vector3 a, Vector3 b)
        {
            Vector3 d = b - a;
            d.y = 0;
            return d.magnitude;
        }

        public static bool InReach(GameObject a, GameObject b, float range)
        {
            return (a.transform.position - b.transform.position).magnitude < range;
        }
    }
}