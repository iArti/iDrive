using System;
using System.Collections.Generic;
using UnityEngine;

public partial class HqRenderer : MonoBehaviour
{
    public RenderWindow Renderer;

    public Camera targetCamera;
    public SpriteRenderer BG;
    public SpriteRenderer Plane;
    public SpriteRenderer FG;
    public int PPU;

    public TrackObject track;

    public Material grass1;
    public Material grass2;
    public Material rumble1;
    public Material rumble2;
    public Material road1;
    public Material road2;
    public Material dashline;

    public int screenWidthRef = 640;
    public int screenHeightRef = 360;
    public float cameraDepth = 0.84f; //camera depth [0..1]
    public int DravingDistance = 300; //segments
    public int cameraHeight = 2000; //pixels?
    public float centrifugal = 0.1f;
    public bool drawRoad;
    public bool drawSprites;
    public int rumbleWidth;
    public float SpriteScale;
    public int TicksPerSecond = 60;
    private float _t;
    //public Sprite[] carSprites;

    [NonSerialized]
    int screenWidth2;
    [NonSerialized]
    int screenHeight2;
    [NonSerialized]
    Mesh[] combined;
    [NonSerialized]
    Dictionary<Material, Quad> dictionary = new Dictionary<Material, Quad>();
    private Material[] materials;

    [NonSerialized]
    public static int trip = 0; //pixels
    [NonSerialized]
    public static float playerX = 0;
    [NonSerialized]
    public static float playerY = 0;
    [NonSerialized]
    float playerZ = 0;

    [NonSerialized]
    private int startPos;
    [NonSerialized]
    private int playerPos;
    [NonSerialized]
    public static int accel;
    [NonSerialized]
    public static float spriteX;


    Quad[] quad;
    [NonSerialized]
    private RenderTexture _renderTexture;
    [NonSerialized]
    public static int speed;
    [NonSerialized]
    public static int maxspeed = 300;
    [NonSerialized]
    public static int offroadmaxspeed = maxspeed/3;
    [NonSerialized]
    public static bool uphill;
    [NonSerialized]
    public static float lineY;
    [NonSerialized]
    public static float prevY;


    private void OnEnable()
    {
        QualitySettings.vSyncCount = 1;
        Camera.onPostRender += PostRender;
        _t = 0f;
    }
    private void OnDisable()
    {
        Camera.onPostRender -= PostRender;
    }

    void Awake()
    {
        Renderer = new RenderWindow();

        //carSprites = Resources.LoadAll<Sprite>("car");
        //GameObject car = new GameObject();

        Texture2D tex1 = new Texture2D(screenWidthRef, screenHeightRef, TextureFormat.RGBA32, false);
        tex1.filterMode = FilterMode.Point;
        FG.sprite = Sprite.Create(tex1, new Rect(0, 0, screenWidthRef, screenHeightRef), new Vector2(0.5f,0.5f), PPU);
        FG.sprite.name = "runtimeFG";

        Texture2D tex2 = new Texture2D(screenWidthRef, screenHeightRef, TextureFormat.RGBA32, false);
        tex2.filterMode = FilterMode.Point;
        Plane.sprite = Sprite.Create(tex2, new Rect(0, 0, screenWidthRef, screenHeightRef), new Vector2(0.5f, 0.5f), PPU);
        Plane.sprite.name = "runtimePlane";

        quad = new Quad[] { new Quad(), new Quad(), new Quad(), new Quad(), new Quad(), new Quad(), new Quad() };
        combined = new Mesh[] { new Mesh(), new Mesh(),new Mesh(),new Mesh(),new Mesh(),new Mesh(),new Mesh(),new Mesh(),new Mesh()};
        dictionary = new Dictionary<Material, Quad>()
        {
            { grass1, quad[0]},
            { grass2, quad[1]},
            { rumble1, quad[2]},
            { rumble2, quad[3]},
            { road1, quad[4]},
            { road2, quad[5]},
            { dashline, quad[6]},
        };
        materials = new Material[] { grass1, grass2, rumble1, rumble2, road1, road2, dashline };
    }
    public void drawSprite(ref Line line)
    {
        if (line.Y < -screenHeight2) { return; }
        Sprite s = line.sprite;
        if (s == null) { return; }
        var w = s.rect.width;
        var h = s.rect.height;

        lineY = line.Y;
        spriteX = line.spriteX;

        float destX = line.X + line.W * line.spriteX + screenWidth2;
        float destY = -line.Y + screenHeight2;
        float destW = w * line.scale * screenWidth2 * SpriteScale;
        float destH = h * line.scale * screenWidth2 * SpriteScale;

        destX += destW * Mathf.Sign(line.spriteX) / 2; //offsetX
        destY += destH * (-1);    //offsetY

        float clipH = -line.Y + line.clip;
        if (clipH < 0) clipH = 0;

        if (clipH >= destH) return;

        Rect target = new Rect(destX, destY, destW, destH);
        Rect source = new Rect(Vector2Int.zero, new Vector2(1, 1 - clipH / destH));
        Renderer.draw(source, s, target);
    }
    private void addQuad(Material c, float x1, float y1, float w1, float x2, float y2, float w2)
    {
        dictionary[c].SetQuad(x1 / PPU, y1 / PPU, w1 / PPU, x2 / PPU, y2 / PPU, w2 / PPU);
    }

    private void DrawObjects()
    {
        ////////draw objects////////
        if (drawSprites)
        {
            _renderTexture = RenderTexture.GetTemporary(screenWidthRef, screenHeightRef);
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = _renderTexture;
            //Work in the pixel matrix of the texture resolution.
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, screenWidthRef, screenHeightRef, 0);
            GL.Clear(false, true, new Color(0, 0, 0, 0));
            for (int n = startPos + DravingDistance; n > startPos; n--)
            {
                drawSprite(ref track.lines[n % track.Length]);
            }
            Graphics.CopyTexture(_renderTexture, FG.sprite.texture);
            //Revert the matrix and active render texture.
            GL.PopMatrix();
            RenderTexture.active = currentActiveRT;
            RenderTexture.ReleaseTemporary(_renderTexture);
        }
    }
    private void DrawRoad()
    {
        if (drawRoad)
        {
            _renderTexture = RenderTexture.GetTemporary(screenWidthRef, screenHeightRef);
            RenderTexture currentActiveRT = RenderTexture.active;
            Graphics.SetRenderTarget(_renderTexture);
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0, 0));
            GL.PushMatrix();
            //GL.LoadOrtho();
            //var proj = Camera.main.projectionMatrix;
            ////// If Camera.current is set, multiply our matrix by the inverse of its view matrix
            //if (Camera.current != null) proj = proj * Camera.current.worldToCameraMatrix.inverse;
            ////// Use instead of LoadOrtho
            //GL.LoadProjectionMatrix(proj);
            float refH = targetCamera.orthographicSize * PPU * 2;
            float refHScale = refH / screenHeightRef;
            float HScale = ((float)screenHeightRef) / targetCamera.pixelHeight;
            float unscaledAspectRatio = (HScale * targetCamera.pixelWidth) / screenWidthRef;

            var m = Matrix4x4.Scale(new Vector3(unscaledAspectRatio * refHScale, refHScale, 1));

            int i = 0;
            foreach (var material in materials)
            {
                Renderer.draw(dictionary[material].ToMesh(combined[i++]), material, m);
            }
            Graphics.CopyTexture(_renderTexture, Plane.sprite.texture);
            GL.PopMatrix();
            Graphics.SetRenderTarget(currentActiveRT);
            RenderTexture.ReleaseTemporary(_renderTexture);
        }
    }

    private void DrawBackground()
    {
        if (speed != 0 && track.lines[playerPos].curve != 0)
        {
            //doesnt work with sprite renderer, while still posted as sollution
            //BG.material.mainTextureOffset += new Vector2(Mathf.Sign(speed) * track.lines[startPos].curve * Time.deltaTime * paralaxSpeed, 0);

            //Not the best solution
            //BG.transform.localPosition += new Vector3(Mathf.Sign(speed) * track.lines[startPos].curve / PPU, 0);

            //Good enough
            _renderTexture = RenderTexture.GetTemporary(BG.sprite.texture.width, BG.sprite.texture.height);
            RenderTexture currentActiveRT = RenderTexture.active;
            RenderTexture.active = _renderTexture;
            //Work in the pixel matrix of the texture resolution.
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, screenWidthRef, screenHeightRef, 0);

            Vector2 offset = new Vector2(0.3f * Time.deltaTime * Mathf.Sign(speed) * track.lines[playerPos].curve, 0);
            Graphics.Blit(BG.sprite.texture, _renderTexture, Vector2.one, offset, 0, 0);

            Graphics.CopyTexture(_renderTexture, BG.sprite.texture);

            GL.PopMatrix();
            Graphics.SetRenderTarget(currentActiveRT);
            RenderTexture.ReleaseTemporary(_renderTexture);
        }
    }

    private void PostRender(Camera cam)
    {
        DrawRoad();
    }

    private void FixedUpdate()
    {
        CalculateProjection();
    }
    void Update()
    {
        DrawBackground();
        DrawObjects();
       // if (speed < 0) speed = 0;
        float dur = 1f / this.TicksPerSecond;
        _t += Time.deltaTime;
        // 1 - ускорение, -1 - торможение, 0 - инерция ... 10, -10, 100 - ускорение/торможение/инерция по бездорожью
        if (Input.GetKey(KeyCode.UpArrow))
        {
            accel = 1;
            while (_t >= dur && speed < maxspeed)
            {

                _t -= dur;
                this.SmoothMovement(accel);
            }
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            accel = -1;
            while (_t >= dur)
            {
                _t -= dur;
                this.SmoothMovement(accel);
            }
        }
        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow))
        {
            accel = 0;
            while (_t >= dur)
            {
                _t -= dur;
                this.SmoothMovement(accel);
            }
        }
        if (playerY > lineY && playerY != 0) uphill = true;
        else uphill = false;
        //if (prevY - playerY < 0) uphill = true;
        // Замедление за пределами дороги
//        if (playerX < -1 || playerX > 1)
  //      {
    //       accel = -2;
     //  }
    }

    public void SmoothMovement(int movement)
    {
        if (movement == 1) //ускорение
        {
            if (playerX > -1 && playerX < 1)
                speed += Convert.ToInt32(Math.Log(speed + 2));
            else
            {
                if (speed == maxspeed) speed = speed - 5;
                if (speed > offroadmaxspeed) speed = speed - 5;
                if (speed < offroadmaxspeed) speed = speed + 2;
            }
        }
        if (movement == -1) //торможение
        {
            if (speed > 0) speed -= 5;
            if (speed < 0) speed = 0;
        }
        if (movement == 0) //инерция по дороге 
        {
            if (speed > 0) speed = speed -1;
            if (speed < 0) speed = 0;
        }
    }


    void CalculateProjection()
    {
        //curspeed = 0;
        //if (Input.GetKey(KeyCode.UpArrow)) ;
        //speed = 200;
        if (Input.GetKey(KeyCode.RightArrow) && speed != 0)
        {
            playerX += 0.05f;
            //spriteRenderer.sprite
            
        }
        if (Input.GetKey(KeyCode.LeftArrow) && speed != 0) playerX -= 0.05f;
        //if (Input.GetKey(KeyCode.DownArrow)) speed = -200;
        if (Input.GetKey(KeyCode.Tab)) speed *= 3;
        if (Input.GetKey(KeyCode.W)) cameraHeight += 100;
        if (Input.GetKey(KeyCode.S)) cameraHeight -= 100;
        //if (destX == playerX && destY == playerY) speed = 0;

        trip += speed;
        while (trip >= track.Length * track.segmentLength) trip -= track.Length * track.segmentLength;
        while (trip < 0) trip += track.Length * track.segmentLength;

        startPos = trip / track.segmentLength;
        playerZ = trip + cameraHeight * cameraDepth; // car is in front of cammera
        playerPos = (int)(playerZ / track.segmentLength) % track.lines.Length;
        playerY = track.lines[playerPos].y;
        int camH = (int)(playerY + cameraHeight);
        playerX = playerX - track.lines[playerPos].curve * centrifugal * speed * Time.fixedDeltaTime;
        playerX = Mathf.Clamp(playerX, -2, 2);

        screenWidth2 = screenWidthRef / 2;
        screenHeight2 = screenHeightRef / 2;

        float maxy = -screenHeight2;
        int counter = 0;
        float x = 0, dx = 0;
        float res = 1f / PPU;

        foreach (var q in quad) { q.Clear(); }
        foreach (var m in combined) { m.Clear(); }
        ///////draw road////////
        for (int n = startPos + 1; n < startPos + DravingDistance; n++)
        {
            ref Line l = ref track.lines[n % track.Length];
            l.project(
                (int)(playerX * track.roadWidth - x),
                camH,
                startPos * track.segmentLength - (n >= track.Length ? track.Length * track.segmentLength : 0),
                screenWidth2,
                screenHeight2,
                cameraDepth);
            x += dx;
            dx += l.curve;

            l.clip = maxy;
            if (l.Y <= maxy)
            {
                continue;
            }
            maxy = l.Y;

            Material grass = (n / 3 / 3) % 2 == 0 ? grass1 : grass2;
            Material rumble = (n / 3) % 2 == 0 ? rumble1 : rumble2;
            Material road = (n / 3 / 2) % 2 == 0 ? road1 : road2;

            ref Line p = ref track.lines[(n - 1) % track.Length]; //previous line

            if (Mathf.Abs(l.Y - p.Y) < res)
            {
                continue;
            }

            //lastY = l.Y;
            prevY = p.Y;



            addQuad(grass, 0, p.Y, screenWidthRef, 0, l.Y, screenWidthRef);
            addQuad(rumble, p.X, p.Y, p.W + p.scale * rumbleWidth * screenWidth2, l.X, l.Y, l.W + l.scale * rumbleWidth * screenWidth2);
            addQuad(road, p.X, p.Y, p.W, l.X, l.Y, l.W);

            if ((n / 3) % 2 == 0)
            {
                addQuad(dashline, p.X, p.Y * 1.1f, p.W * 0.05f, l.X, l.Y * 1.1f, l.W * 0.05f);
            }

            counter++;
        }
    }

}
