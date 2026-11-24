using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Reflection;
public enum direction
{
    right,
    up,
    left,
    down
}
public static class Direction
{
    public static string[] DirectionNames = new string[]
    {
        "Right",
        "Up",
        "Left",
        "Down"
    };
    public static direction Opposite(this direction d)
    {
        switch (d)
        {
            case direction.up:
                return direction.down;
            case direction.left:
                return direction.right;
            case direction.right:
                return direction.left;
            case direction.down:
                return direction.up;
        }
        return d;
    }
    public static Vector2Int ToVector(this direction d)
    {
        switch (d)
        {
            case direction.up:
                return Vector2Int.down;
            case direction.left:
                return Vector2Int.right;
            case direction.right:
                return Vector2Int.left;
            case direction.down:
                return Vector2Int.up;
        }
        return Vector2Int.up;
    }
    public static direction Rotate(this direction d)
    {
        return (direction)(((int)d + 1) % 4);
    }
    public static direction UnRotate(this direction d)
    {
        return (direction)Mathf.Repeat(((int)d - 1), 4);
    }
    public static string ToName(this direction d)
    {
        return DirectionNames[(int)d];
    }
}

public static class Vector3Extensions
{
    public static Vector3Int Round(this Vector3 vec)
    {
        return new Vector3Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y), Mathf.RoundToInt(vec.z));
    }
    public static Vector2Int Round(this Vector2 vec)
    {
        return new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
    }
    public static Vector3 Orbit(float t, float speed, float amplitude, float offset = 0)
    {
        return (t * speed + offset).toVector() * amplitude;
    }
    public static void Generate4Point(Vector3 a, Vector3 b, float dist, out Vector3 p1, out Vector3 p2)
    {
        Vector3 offset = (b - a).normalized.perpendicular();
        p1 = Vector3.Lerp(a, b, 0.33f) + offset * UnityEngine.Random.Range(-dist, dist);
        p2 = Vector3.Lerp(a, b, 0.33f) + offset * UnityEngine.Random.Range(-dist, dist);
    }
    public static Vector3 TwoDLerp(Vector3 p1, Vector3 p2, Vector3 t)
    {
        return new Vector3(MathEXT.realfLerp(p1.x, p2.x, t.x), MathEXT.realfLerp(p1.y, p2.y, t.y));
    }
    public static bool GreaterThan(this Vector3 p1, Vector3 p2)
    {
        return (p1.x > p2.x && p1.y > p2.y);
    }
    public static Vector3 Rand(this Vector3 p1, Vector3 p2)
    {

        return new Vector3(UnityEngine.Random.Range(p1.x, p2.x), UnityEngine.Random.Range(p1.y, p2.y));
    }
    public static void VisualiseThreePoint(Vector3 a, Vector3 b, Vector3 point, float t)
    {
        Vector3 last = a;
        for (float i = 0; i <= 1.01f; i += 0.1f)
        {
            Vector3 p = ThreePointBezier(a, b, point, i);
            globalGizmos.Draw(new lineGizmo(last, p) { color = Color.green });
            last = p;
        }
        globalGizmos.Draw(new circleGizmo(ThreePointBezier(a, b, point, t)) { color = Color.blue });
    }
    public static void VisualiseFourPoint(Vector3 a, Vector3 b, Vector3 point, Vector3 point2, float t)
    {
        Vector3 last = a;
        for (float i = 0; i <= 1.05f; i += 0.1f)
        {
            Vector3 p = FourPointBezier(a, b, point, point2, i);
            globalGizmos.Draw(new lineGizmo(last, p) { color = Color.green });
            last = p;
        }
        globalGizmos.Draw(new circleGizmo(ThreePointBezier(a, b, point, t)) { color = Color.blue });
    }
    public static float DistanceOfThreePoint(Vector3 a, Vector3 b, Vector3 point)
    {
        Vector3 last = a;
        float dist = 0;
        for (float i = 0; i <= 1; i += 0.1f)
        {
            Vector3 p = ThreePointBezier(a, b, point, i);
            dist += Vector3.Distance(p, last);
            last = p;
        }
        return dist;

    }
    public static Vector3 ThreePointBezier(Vector3 a, Vector3 b, Vector3 point, float t)
    {

        return Vector3.Lerp(Vector3.Lerp(a, point, t), Vector3.Lerp(point, b, t), t);
    }
    public static Vector3 FourPointBezier(Vector3 a, Vector3 b, Vector3 point, Vector3 point2, float t)
    {
        return Vector3.Lerp(ThreePointBezier(a, point2, point, t), ThreePointBezier(point, b, point2, t), t);
    }
    public static Vector3 toVector(this float angle)
    {
        return new Vector3(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    public static Vector3 toVectorDegrees(this float angle)
    {
        return new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
    public static Vector3 squareBy(this Vector3 vector, float by)
    {
        Vector3 _vector = new Vector3()
        {
            x = vector.x * (vector.x > 0 ? vector.x : -vector.x) * by,
            y = vector.y * (vector.y > 0 ? vector.y : -vector.y) * by,
        };

        return _vector;
    }
    public static Vector3 Abs(this Vector3 vector)
    {
        return new Vector3((vector.x > 0 ? vector.x : -vector.x), (vector.y > 0 ? vector.y : -vector.y));
    }
    public static Vector3 divide(this Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x / v2.x, v1.y / v2.y);
    }
    public static Vector3 multiply(this Vector3 v1, Vector3 v2)
    {
        return new Vector3(v1.x * v2.x, v1.y * v2.y);
    }
    public static Vector2 multiplyv2(this Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.x * v2.x, v1.y * v2.y);
    }
    public static Vector2 dividev2(this Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.x / v2.x, v1.y / v2.y);
    }
    public static Vector3 cutZ(this Vector3 v1)
    {
        return new Vector3(v1.x, v1.y);
    }
    public static Vector3 cutZCamera(this Vector3 v1)
    {
        return new Vector3(v1.x, v1.y, -10);
    }
    public static Vector3 setZ(this Vector3 v1, float z)
    {
        return new Vector3(v1.x, v1.y, z);
    }
    public static Vector3 td(this Vector3 v1)
    {
        return new Vector3(v1.x, v1.y, 0);
    }
    public static Vector3 v3(this Vector2 v1)
    {
        return new Vector3(v1.x, v1.y, 0);
    }
    public static Vector3 setY(this Vector3 v1, float y)
    {
        return new Vector3(v1.x, y, v1.z);
    }
    public static Vector3 setX(this Vector3 v1, float x)
    {
        return new Vector3(x, v1.y, v1.z);
    }
    public static Vector3 clampBetween(this Vector3 v, float min, float max)
    {
        return v.normalized * Mathf.Clamp(v.magnitude, min, max);
    }
    public static Vector2 clampBetween(this Vector2 v, float min, float max)
    {
        return v.normalized * Mathf.Clamp(v.magnitude, min, max);
    }

    public static Vector3 perpendicular(this Vector3 v)
    {
        return new Vector3(v.y, -v.x);
    }
    public static Vector3 perpendicularAbs(this Vector3 v)
    {
        return new Vector3(v.y, v.x).Abs();
    }

    public static Vector3 combineVectors(this Vector3 v1, Vector3 v2, Vector3 direction)
    {
        return v1.multiply(direction.Abs()) + v2.multiply(direction.perpendicularAbs());
    }
    public static Vector3 Sqrt(this Vector3 v)
    {
        return new Vector3(Mathf.Sqrt(v.x), Mathf.Sqrt(v.y));
    }
    public static Vector3 normalizeAxis(this Vector3 v)
    {

        Vector3 v1 = new Vector3();
        if (v.x != 0)
            v1.x = v.x / Mathf.Abs(v.x);
        if (v.y != 0)
            v1.y = v.y / Mathf.Abs(v.y);
        return v1;
    }

    public static Vector3 Clamp(this Vector3 v1, Vector3 lower, Vector3 upper)
    {
        return new Vector3(Mathf.Clamp(v1.x, lower.x, upper.x), Mathf.Clamp(v1.y, lower.y, upper.y));
    }
    public static Vector3 ClampVec2(this Vector2 v1, Vector2 lower, Vector2 upper)
    {
        return new Vector3(Mathf.Clamp(v1.x, lower.x, upper.x), Mathf.Clamp(v1.y, lower.y, upper.y));
    }
    public static Vector2Int ClampVec2(this Vector2Int v1, Vector2Int lower, Vector2Int upper)
    {
        return new Vector2Int(Mathf.RoundToInt(Mathf.Clamp(v1.x, lower.x, upper.x)), Mathf.RoundToInt(Mathf.Clamp(v1.y, lower.y, upper.y)));
    }
    public static Vector3 LowerLimit(this Vector3 v1, Vector3 lower)
    {
        return new Vector3(Mathf.Max(v1.x, lower.x), Mathf.Max(v1.y, lower.y));
    }

    public static bool GreaterThanEquals(this Vector3 v1, Vector3 v2)
    {
        return v1.x >= v2.x && v1.y >= v2.y;
    }
    public static bool LessThanEquals(this Vector3 v1, Vector3 v2)
    {
        return v1.x <= v2.x && v1.y <= v2.y ;
    }
    public static bool GreaterThanEquals(this Vector2Int v1, Vector2Int v2)
    {
        return v1.x >= v2.x && v1.y >= v2.y;
    }
    public static bool LessThanEquals(this Vector2Int v1, Vector2Int v2)
    {
        return v1.x <= v2.x && v1.y <= v2.y;
    }
    public static bool GreaterThan(this Vector2Int v1, Vector2Int v2)
    {
        return v1.x >= v2.x && v1.y >= v2.y;
    }
    public static bool LessThan(this Vector2Int v1, Vector2Int v2)
    {
        return v1.x <= v2.x && v1.y <= v2.y;
    }
    public static float SquareDistancePrioritisingAxis(Vector3 v1, Vector3 v2, Vector3 bias)
    {
        if (v1 == v2) return 0;
        Vector3 offset = (v1 - v2).multiply(bias);
        return offset.sqrMagnitude;
    }
    public static Vector3 size(this Camera c)
    {
        return new Vector3(c.orthographicSize * 2 * c.aspect, c.orthographicSize * 2);
    }

    public static Vector2Int inverseSign(this Vector2Int v1)
    {
        return new Vector2Int(1 - (int)Mathf.Sign(-v1.x), -(int)Mathf.Sign(-v1.y));
    }

    public static Vector3 rotateAround(Vector3 point, Vector3 origin, Quaternion angle)
    {
        return angle * (point - origin) + origin;
    }

    public static float angleTo(Vector3 v1, Vector3 v2)
    {
        Vector3 offset = v2 - v1;
        return Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
    }
}

public static class MathEXT
{
    public static float RoundTo(this float i, float to)
    {
        return Mathf.Round(i / to) * to;
    }
    public static float RoundToSF(this float value, int sigFigs)
    {
        if (value == 0f || sigFigs <= 0)
            return 0f;
        decimal dec = (decimal)value;
        int digits = (int)Math.Floor(Math.Log10((double)Math.Abs(dec))) + 1;
        int decimals = sigFigs - digits;

        // Convert to decimal, round, then cast back
        decimal rounded = Math.Round(dec, decimals, MidpointRounding.AwayFromZero);
        return (float)rounded;
    }
    public static float TendTo(this float i, float tend = 1)
    {
        return Mathf.Sqrt(Mathf.Abs(i) / tend) * Mathf.Sign(i) * tend;
    }
    public static float TendTo(this float i, float tendAmount, float tend = 1)
    {
        return Mathf.Pow(Mathf.Abs(i) / tend, 1 / (tendAmount + 1)) * Mathf.Sign(i) * tend;
    }
    public static int fLerp(float a, float b, float t) 
    {
        return Mathf.RoundToInt(a + (b - a) * t); 
    }
    public static float realfLerp(float a, float b, float t)
    {
        return (a + (b - a) * t);
    }
    public static float angularLerp(float a, float b, float t)
    {
        return (a + angularOffsetDeg(a, b) * t);
    }
    public static float angularOffset(float a, float b)
    {

        a *= Mathf.Rad2Deg;
        b *= Mathf.Rad2Deg;
        return (((b - a) % 360 + 540) % 360 - 180) * Mathf.Deg2Rad;
    }
    public static float customAngularOffset(float a, float b, float len)
    {

        return (((b - a) % len + len * 1.5f) % len - len / 2f);
    }
    public static float angularOffsetDeg(float a, float b)
    {

        return (((b - a) % 360 + 540) % 360 - 180);
    }

    public static uint recursiveMin(uint a, uint b, uint len)
    {
        uint halflen = len / 2;
        return (((b - a) % (halflen + len) + len) % len - halflen);
    }
    public static float AngleTo(Vector3 p1, Vector3 p2)
    {
        if (p1 == p2) return 0;
        Vector3 offset = p2 - p1;
        return Mathf.Atan2(offset.x, offset.y);
    }
    public static float AngleToProper(Vector3 p1, Vector3 p2)
    {
        if (p1 == p2) return 0;
        Vector3 offset = p2 - p1;
        return Mathf.Atan2(offset.y, offset.x);
    }
}

public static class MaterialEXT
{
    public static Material CopyAndReturn(this Renderer render)
    {
        render.material = GameObject.Instantiate(render.sharedMaterial);
        return render.material;
    }
    public static Material CopyAndReturn(this UnityEngine.UI.Image render)
    {
        render.material = GameObject.Instantiate(render.material);
        return render.material;
    }
}

public static class StringEXT
{
    public static string MakeOfLength(this string s, int length, string filler)
    {
        string toReturn = "";
        for (int i = 0; i < length - s.Length; i++)
        {
            toReturn += filler;
        }
        toReturn += s;
        return toReturn;
    }
}



public static class ColliderEXT
{
    public static Vector3 toVec(this direction d) => directions[(int)d];
    public static direction toDir(this Vector3 d) => directionsEnum[directions.ToList().FindIndex(v => v == d)];
    static Vector3[] directions = new Vector3[4] { new Vector3(1, 0),  new Vector3(0, 1), new Vector3(-1, 0), new Vector3(0, -1) };
    static direction[] directionsEnum = new direction[4] { direction.up, direction.left, direction.right, direction.down };
    public static Rect getEdge(this BoxCollider2D box, Vector3 direction, float size = 0.05f, float percentage = 1)
    {


        Rect r = new Rect();

        r.width = box.size.x * Mathf.Abs(direction.y) * percentage + size * Mathf.Abs(direction.x);
        r.height = box.size.y * Mathf.Abs(direction.x) * percentage + size * Mathf.Abs(direction.y);


        r.x = box.size.x / 2 * direction.x + (r.width / 2 + 0.01f) * direction.x;
        r.y = box.size.y / 2 * direction.y + (r.height / 2 + 0.01f) * direction.y;
        r.position += (Vector2)box.transform.position;

        return r;
    }
    public static Rect getEdge(this BoxCollider2D box, direction direction, float size = 0.05f, float percentage = 1)
    {


        return getEdge(box, directions[(int)direction], size, percentage);
    }

    public static bool TryImplicit<Q>(this object obj, out Q converted) where Q : class
    {
        converted = obj as Q;
        return converted == null;
    }
}
public static class RNG
{
    public static float Range(float min, float max) => UnityEngine.Random.Range(min, max);
    public static Vector3 insideUnitCircle => UnityEngine.Random.insideUnitCircle;

    // Get a value on the normal curve
    public static float RandomNormal(float mean, float stdDev)
    {
        float u = Range(0, 1);
        float x = GetNormalValue(u);
        return mean + stdDev * x;
    }

    private static float GetNormalValue(float p)
    {
        // Approximation of Normal distribution based on Abramowitz Stegun formula 26.2.23
        // I knew further maths/stats would come in useful!
        float a = 2.30753f;
        float b = 0.27061f;
        float c = 0.99229f;
        float d = 0.04481f;


        // Witchcraft below:
        float t = Mathf.Sqrt(-2 * Mathf.Log(1f - p));
        return t - ((a * t + b) / (c * t + d));

        // Thanks Abramowitz and Stegun!
    }
}
public static class MiscEXT
{
    public static Texture2D textureFromSprite(this Sprite sprite)
    {
        if (sprite.rect.width != sprite.texture.width)
        {
            Texture2D newText = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] newColors = sprite.texture.GetPixels((int)sprite.textureRect.x,
                                                         (int)sprite.textureRect.y,
                                                         (int)sprite.textureRect.width,
                                                         (int)sprite.textureRect.height);
            newText.SetPixels(newColors);
            newText.Apply();
            return newText;
        }
        else
            return sprite.texture;
    }

    // floating point suffers when equated due to precision errors
    public static bool FuzzyEquals(this float f1, float f2, float epsilon = 0.001f)
    {
        return f1 > f2 - epsilon && f1 < f2 + epsilon;
    }
    public static Vector3 WorldMouse
    {
        get
        {
            Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            v.z = 0;
            return v;
        }
    }
    public static T Choose<T>(this T[] obj)
    {
        return obj[UnityEngine.Random.Range(0, obj.Length)];
    }
    public static T[] Add<T>(this T[] list, T add)
    {
        T[] temp = new T[list.Length + 1];
        for (int i = 0; i < list.Length; i++)
        {
            temp[i] = list[i];
        }
        temp[list.Length] = add;
        return temp;
    }
    public static bool Contains(this string[] list, string check)
    {
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i] == check) return true;
        }
        return false;
        
    }
    public static T[] Remove<T>(this T[] list, int remove)
    {
        T[] temp = new T[list.Length - 1];
        int x = 0;
        for (int i = 0; i < list.Length; i++)
        {

            if (i != remove) 
            {
                temp[x++] = list[i];
            }
        }
        return temp;
    }
    public static void Set(this Image img, Sprite s)
    {
        img.sprite = s;
        img.SetNativeSize();
    }
    public static T ChooseWhere<T>(this T[] obj, Func<T, bool> Check, int safety)
    {
        return RandomWhere(() => obj[UnityEngine.Random.Range(0, obj.Length)], Check, safety);
    }
    public static T RandomWhere<T>(Func<T> Random, Func<T, bool> Check, int safety)
    {
        T toReturn = Random();
        if (!Check(toReturn))
        {
            for (int i = 0; i < safety - 1; i++)
            {
                toReturn = Random();
                if (Check(toReturn)) break;
            }
        }

        return toReturn;
    }
    public static T? RandomStructWhereOrNone<T>(Func<T> Random, Func<T, bool> Check, int safety) where T : struct
    {
        T toReturn = Random();
        if (Check(toReturn))
        {
            return toReturn;
        }
        for (int i = 0; i < safety - 1; i++)
        {
            toReturn = Random();

            if (Check(toReturn))
            {
                
                return toReturn;
            }
        }

        return null;
    }
    public static Vector3 MousePos()
    {
        return Camera.main.ScreenToWorldPoint(Input.mousePosition).setZ(0);
    }
    public static void GizmoBox(Vector3 v1, Vector3 v2, Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawWireCube(Vector3.Lerp(v1, v2, 0.5f), (v2 - v1).Abs());
    }
    public static Vector3 CamSize(this Camera c)
    {
        return new Vector3(c.orthographicSize * c.aspect, c.orthographicSize) * 2;
    }

    public static T[] GetComponentsInImmediateChildren<T>(this GameObject g) where T : Component
    {
        List<T> components = new List<T>();

        foreach (Transform child in g.transform)
        {
            T component = child.GetComponent<T>();
            if (component != null)
            {
                components.Add(component);
            }
        }

        return components.ToArray();
    }

    public static void SetEmission(this ParticleSystem sys, float rate)
    {
        ParticleSystem.EmissionModule e = sys.emission;
        e.rateOverTimeMultiplier = rate;
    }

    public static string FormatMoney(float Money)
    {
        float magnitude = Mathf.Floor(Mathf.Log10(Money));
        if (magnitude < 2) // 1.00 - 100.00
        {
            float m = Mathf.Round(Money * 100);

            bool haspointones = m % 100 != 0;
            bool haspointtens = m % 10 != 0;

            string toReturn = (m / 100).ToString();
            if (!haspointones && !haspointtens) toReturn += ".00";
            else if (!haspointtens) toReturn += "0";

            return toReturn;
        }
        else if (magnitude < 3) // 100
        {
            return Mathf.RoundToInt(Money).ToString();
        }
        else 
        {
            int denomination = Mathf.FloorToInt(magnitude / 3);
            string suffix = "";
            switch (denomination)
            {
                case 1:
                    suffix = "K";
                    break;
                case 2:
                    suffix = "M";
                    break;
                case 3:
                    suffix = "B";
                    break;
                case 4:
                    suffix = "T";
                    break;
            }

            float tens = Mathf.Pow(10, denomination * 3);
            return (FormatMoney(Money / tens)).ToString() + suffix;
        }

    }


    
}
public static class GizEXT
{
    public static void Line(Vector2 v1, Vector2 v2, Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawLine(v1, v2);
    }
    public static void Circle(Vector2 v1, float r, Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawSphere(v1, r);
    }
    public static void WireCircle(Vector2 v1, float r, Color c)
    {
        Gizmos.color = c;
        Gizmos.DrawWireSphere(v1, r);
    }
}
public static class WeightedRandomSelector
{
    public static int[] SelectWeightedRandomItems(float[] weights, int amount)
    {
        if (weights == null || weights == null || weights.Length != weights.Length || weights.Length == 0 || amount <= 0)
        {
            throw new ArgumentException("Invalid input parameters.");
        }

        int[] selectedItems = new int[amount];
        float[] cumulativeWeights = new float[weights.Length];
        float totalWeight = 0;

        // Calculate cumulative weights
        for (int i = 0; i < weights.Length; i++)
        {
            totalWeight += weights[i];
            cumulativeWeights[i] = totalWeight;
        }

        // Select items based on weights
        for (int i = 0; i < amount; i++)
        {
            float randomValue = UnityEngine.Random.Range(0, totalWeight);
            int selectedIndex = BinarySearch(cumulativeWeights, randomValue);
            selectedItems[i] = selectedIndex;
        }

        return selectedItems;
    }
    public static void FormatAsWeights(Func<int, float> getWeight, int count, out float[] cumulativeWeights, out float total)
    {
        cumulativeWeights = new float[count];
        total = 0;

        // Calculate cumulative weights
        for (int i = 0; i < count; i++)
        {
            float weight = getWeight(i);
            if (weight < 0.03f) weight = 0;
            total += weight;
            cumulativeWeights[i] = total;
        }
    }
    public static int GetWeight(float[] cumulativeWeights, float total)
    {
        float randomValue = UnityEngine.Random.Range(0, total);
        int selectedIndex = BinarySearch(cumulativeWeights, randomValue);
        return selectedIndex;
    }
    private static int BinarySearch(float[] cumulativeWeights, float value)
    {
        int left = 0;
        int right = cumulativeWeights.Length - 1;

        while (left < right)
        {
            int mid = (left + right) / 2;
            if (cumulativeWeights[mid] < value)
            {
                left = mid + 1;
            }
            else
            {
                right = mid;
            }
        }

        return left;
    }
}


public static class Reflect
{

    public static object GetPropertyValue(object obj, string propertyName, bool display = false)
    {
        if (obj == null || string.IsNullOrEmpty(propertyName))
        {
            Debug.LogError("NULL");
            return null;
        }
        if (display)
        {

            PropertyInfo[] all = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance); ;
            //Debug.Log(obj.GetType().GetProperties().Length);
            foreach (PropertyInfo p in all)
            {
                Debug.Log(p.Name);
            }
        }


        PropertyInfo propertyInfo = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (propertyInfo == null)
        {
            Debug.LogError(propertyName + " doesn't exist");
            return null;
        }

        return propertyInfo.GetValue(obj);
    }
    public static object GetListItemValue(object obj, int index)
    {
        if (obj == null || index < 0)
        {
            Debug.LogError("Null list");
            return null;
        }


        // Ensure the object is a list (IList or Array)
        if (obj is IList list)
        {
            if (index >= list.Count)
                return null;
            Debug.Log("FOUND in list");
            object o = list[index];
            return o;
        }

        // If obj is an array, use reflection to get the item
        if (obj is Array array)
        {
            if (index >= array.Length)
                return null;

            return array.GetValue(index);
        }

        return null;
    }

    public static object CallMethod(object obj, string methodName, params object[] parameters)
    {
        if (obj == null || string.IsNullOrEmpty(methodName))
            return null;

        MethodInfo methodInfo = obj.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (methodInfo == null)
            return null;

        // Call the method with the given parameters
        return methodInfo.Invoke(obj, parameters);
    }
}
public class FineControlAttribute : PropertyAttribute
{
    public float increment;

    public FineControlAttribute(float increment)
    {
        this.increment = increment;
    }
}


public static class CollisionsEXT
{
    public static Vector3 ClosestPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLengthSquared = lineDirection.sqrMagnitude;

        // Handle degenerate case where the line is a single point
        if (lineLengthSquared == 0f)
            return lineStart;

        // Project the point onto the line (normalised)
        float t = Vector3.Dot(point - lineStart, lineDirection) / lineLengthSquared;

        // Clamp t to ensure the closest point is on the segment
        t = Mathf.Clamp01(t);

        // Compute the closest point
        return lineStart + t * lineDirection;
    }
    public static bool DoLinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {

        float A1 = p2.y - p1.y;
        float B1 = p1.x - p2.x;
        float C1 = A1 * p1.x + B1 * p1.y;

        float A2 = p4.y - p3.y;
        float B2 = p3.x - p4.x;
        float C2 = A2 * p3.x + B2 * p3.y;

        float determinant = A1 * B2 - A2 * B1;

        if (Mathf.Abs(determinant) < Mathf.Epsilon)
        {
            // Lines are parallel
            return false;
        }

        float intersectX = (B2 * C1 - B1 * C2) / determinant;
        float intersectY = (A1 * C2 - A2 * C1) / determinant;

        // Check if the intersection point is on both line segments
        bool onLine1 = Mathf.Min(p1.x, p2.x) <= intersectX + 0.001f && intersectX <= Mathf.Max(p1.x, p2.x) + 0.001f &&
                       Mathf.Min(p1.y, p2.y) <= intersectY + 0.001f && intersectY <= Mathf.Max(p1.y, p2.y) + 0.001f;

        bool onLine2 = Mathf.Min(p3.x, p4.x) <= intersectX + 0.001f && intersectX <= Mathf.Max(p3.x, p4.x) + 0.001f &&
                       Mathf.Min(p3.y, p4.y) <= intersectY + 0.001f && intersectY <= Mathf.Max(p3.y, p4.y) + 0.001f;

        return onLine1 && onLine2;
    }
    public static bool DoBoxesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        // get closest point from centre of box 1 to box 2
        Vector3 centre = Vector3.Lerp(p1, p2, 0.5f);

        centre = Vector3Extensions.Clamp(centre, p3, p4);
        // if centre is still in box, the boxes collide

        return centre.GreaterThanEquals(p1) && centre.LessThanEquals(p2);
    }
    public static bool LineBoxCollision(Vector2 l1, Vector2 l2, Vector2 p1, Vector2 p2)
    {
        // get closest point from centre of box 1 to box 2
        Vector3 centre = Vector3.Lerp(p1, p2, 0.5f);

        Vector3 closest = ClosestPointOnLine(centre, l1, l2);

        // if centre is still in box, the boxes collide

        return closest.GreaterThanEquals(p1) && closest.LessThanEquals(p2);

    }
}