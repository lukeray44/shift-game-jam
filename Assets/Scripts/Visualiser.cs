using UnityEngine;

public class Visualiser : MonoBehaviour
{

    public static Visualiser main;
    void Awake()
    {
        main = this;
    }


    public Sprite ArrowHead, Cross;

    public Arrow LinePrefab;
    public SpriteRenderer SpritePrefab;

    pool<Arrow> lines;
    pool<SpriteRenderer> sprites;

    void Start()
    {
        lines = new pool<Arrow>(LinePrefab, gameObject);
        sprites = new pool<SpriteRenderer>(SpritePrefab, gameObject);
    }

    public static void HideAll()
    {
        main.lines.HideAll();
        main.sprites.HideAll();
    }

    public static Arrow DrawLine(params Vector3[] positions)
    {
        Arrow a = main.lines.Next();
        a.SetPositions(positions);

        Vector3 end = positions[positions.Length - 1];
        Vector3 penultimate = new Vector3();
        for (int i = positions.Length - 1; i >= 0; i--)
        {
            penultimate = positions[i];
            if (Vector3.Distance(end, penultimate) > 0.5f) break;
        }

        DrawSpritePointer(penultimate, end);
        return a;
    }
    public static SpriteRenderer DrawSprite(Vector3 position, Sprite s)
    {
        if (s == null) return null;
        SpriteRenderer sr = main.sprites.Next();
        sr.sprite = s;
        sr.transform.localEulerAngles = new Vector3();
        sr.transform.position = position;
        return sr;
    }
    public static SpriteRenderer DrawSprite(Vector3 position, Icon icon)
    {
        Sprite s = null;
        switch (icon)
        {
            case Icon.ArrowHead:
                s = main.ArrowHead;
                break;
            case Icon.Cross:
                s = main.Cross;
                break;
        }
        return DrawSprite(position, s);
    }
    public static SpriteRenderer DrawSpritePointer(Vector3 p1, Vector3 p2)
    {
        Vector3 delta = p2 - p1;
        float angle = Mathf.Atan2(delta.y, delta.x);
        SpriteRenderer s = DrawSprite(p2, Icon.ArrowHead);
        s.transform.localEulerAngles = Vector3.forward * angle * Mathf.Rad2Deg;
        return s;
    }

    class pool<T> where T : UnityEngine.Component
    {
        T[] cache;

        T prefab;

        int cacheLength;

        GameObject parent;
        public pool(T prefab, GameObject root, int length = 5)
        {
            this.prefab = prefab;
            parent = new GameObject(prefab.GetType().ToString() + " pool");
            parent.transform.parent = root.transform;

            cacheLength = length;
            SetLength(length);
        }
        public void SetLength(int length)
        {
            T[] newCache = new T[length];


            for (int i = 0; i < newCache.Length; i++)
            {
                if (cache != null && i < cache.Length) newCache[i] = cache[i];
                else
                {
                      T obj = Instantiate(prefab);
                obj.transform.parent = parent.transform;
                obj.gameObject.SetActive(false);
                newCache[i] = obj;
                }
              
            }

            cache = newCache;
        }

        int visible = 0;

        public T Next()
        {

            if (visible >= cache.Length)
            {
                SetLength(cache.Length + cacheLength);
            }
            T obj = cache[visible];
            obj.gameObject.SetActive(true);
            visible++;
            return obj;
        }

        public void HideAll()
        {
            for (int i = 0; i < visible; i++)
            {
                cache[i].gameObject.SetActive(false);
            }
            visible = 0;
        }
    }

    public static void VisualisePhysics(Building source, Vector3 pos, Vector3 velocity, Building LookFor = null)
    {
        itemSimulation sim = new itemSimulation(pos, velocity);
        
        BuildingManager build = BuildingManager.main;
        sim.NewTile += tile =>
        {
            bool tryGetBuilding(out Building b)
            {
                if (LookFor != null && LookFor.ContainsTile(tile))
                {
                    b = LookFor;
                    return true;
                }
                return build.PositionMap.TryGetValue(tile, out b);
            }
            if (tryGetBuilding(out Building b))
            {
                if (b != null && b != source && b.MainSimulator.CanRecieveFromPosition(sim.GetOutputDir(), sim.Position()))
                {

                   sim.Break();
                   sim.ShiftEnd(b.MainSimulator.GetItemTarget(sim.GetOutputDir()));
                   b.MainSimulator.Visualise(source.MainSimulator);

                }
            }
        };

        sim.SimulateTime(build, 25);

        sim.Visualise();
    }
}

public enum Icon
{
    ArrowHead,
    Cross
}