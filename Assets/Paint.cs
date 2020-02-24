using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]
public class Paint : MonoBehaviour
{
    public static Paint Instance;
    public Material simpleInkPaintMat; 
    public Texture2D[] brushes;

    private Texture _brushTexture;
    private Texture _mainTexture;
    private RenderTexture _renderTexture;
    private RenderTexture _secondTexture;

    private int _mainTexturePropertyId;
    private int _secondTexturePropertyId;
    private int _paintUvPropertyId;
    private int _brushTexturePropertyId;
    private int _brushScalePropertyId;
    private int _brushColorPropertyId;

    public int brushId;



    public void SetId(int number)
    {
        brushId = number;
        _brushTexture = brushes[brushId];
    }

    public void Clear()
    {
        _renderTexture.Release();
    }

    void Awake()
    {
        Instance = this;
        _mainTexturePropertyId = Shader.PropertyToID("_MainTex");
        _secondTexturePropertyId = Shader.PropertyToID("_SecondTex");
        _paintUvPropertyId = Shader.PropertyToID("_PaintUV");
        _brushTexturePropertyId = Shader.PropertyToID("_Brush");
        _brushScalePropertyId = Shader.PropertyToID("_BrushScale");
        _brushColorPropertyId = Shader.PropertyToID("_ControlColor");

        _mainTexture = simpleInkPaintMat.GetTexture(_mainTexturePropertyId);
        _brushTexture = simpleInkPaintMat.GetTexture(_brushTexturePropertyId);

        _renderTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);
        _secondTexture = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Default);

        Graphics.Blit(_mainTexture, _renderTexture);

        simpleInkPaintMat.SetTexture(_mainTexturePropertyId, _renderTexture);
        simpleInkPaintMat.SetTexture(_secondTexturePropertyId, _secondTexture);
    }

    void OnDestroy()
    {
        simpleInkPaintMat.SetTexture(_mainTexturePropertyId, _mainTexture);
    }
    
    

    public Vector2 size = new Vector2(10, 10);
    public Vector3 pos;

    public List<Shape> objList;

    private void Start()
    {
        objList[objList.Count-1].GetComponent<Shape>().selectMe = true;
    }

    public void RenderAllExceptThisToPrevTex(Transform single)
    {
        _renderTexture.Release();

        foreach (var obj in objList)
        {
            if (obj.name != single.name)
            {
               RenderMultiple(obj.transform.position,obj.shapeId); 
            }
            else
            {
                print(obj.gameObject.name);
            }
        }
    }


    public void RenderSingle(Vector3 pos,int shapeId)
    {
        SetId(shapeId);
        var objSize = (new Vector2(-pos.x, -pos.z) + size / 2.0f) / size;
        PaintCurrent(objSize,shapeId);
        simpleInkPaintMat.SetVector(_paintUvPropertyId, new Vector2(100,100));
    }

    private void RenderMultiple(Vector3 pos,int shapeId)
    {
         SetId(shapeId);
         var uv = (new Vector2(-pos.x,-pos.z) + (size/2.0f))/ size;
         var renderTextureBuffer = RenderTexture.GetTemporary(_renderTexture.width, _renderTexture.height);
        simpleInkPaintMat.SetVector(_paintUvPropertyId, uv);
        simpleInkPaintMat.SetTexture(_brushTexturePropertyId, _brushTexture);
        simpleInkPaintMat.SetFloat(_brushScalePropertyId, objList[shapeId].size);
        _secondTexture.Release();
        Graphics.Blit(_renderTexture, renderTextureBuffer, simpleInkPaintMat);
        Graphics.Blit(renderTextureBuffer, _renderTexture); 
        RenderTexture.ReleaseTemporary(renderTextureBuffer);
        
        simpleInkPaintMat.SetVector(_paintUvPropertyId, new Vector2(100,100));

    }
    
    private void PaintCurrent(Vector2 uv,int shapeId)
    {
        simpleInkPaintMat.SetVector(_paintUvPropertyId, uv);
        simpleInkPaintMat.SetTexture(_brushTexturePropertyId, _brushTexture);
        simpleInkPaintMat.SetFloat(_brushScalePropertyId, objList[shapeId].size);

        Graphics.Blit(_secondTexture, _secondTexture, simpleInkPaintMat);
        Graphics.Blit(_secondTexture, _secondTexture); 

    }
}