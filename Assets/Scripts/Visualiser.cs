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
        float dist = 0;
        Vector3 last = positions[0];
        foreach (Vector3 v in positions)
        {
            dist +=Vector3.Distance(last, v);
            last = v;
        }

        if (dist < 0.5f || Vector3.Distance(positions[0], last) < 0.2f) return null;

        Arrow a = main.lines.Next();
        a.SetPositions(positions);

        Vector3 end = positions[positions.Length - 1];
        Vector3 penultimate = new Vector3();
        for (int i = positions.Length - 1; i >= 0; i--)
        {
            penultimate = positions[i];
            if (Vector3.Distance(end, penultimate) > 0.2f) break;
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

        bool canVisualise = LookFor == null;
        sim.TryFindBuilding(source, LookFor, (b, m) =>
        {
            b.VisualisePhysics(sim.Position(), sim.lastVelocity, sim.GetOutputDir());

            if (LookFor != null && b == LookFor) canVisualise = true;
        });

        if (canVisualise) sim.Visualise();
    }
}

public enum Icon
{
    ArrowHead,
    Cross
}