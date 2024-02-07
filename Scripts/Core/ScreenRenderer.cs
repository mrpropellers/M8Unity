using UnityEngine;
using System.Collections.Generic;

namespace M8 {

public sealed class ScreenRenderer
{
    Queue<DrawCommand> _buffer = new Queue<DrawCommand>();

    private Material _screenMat;
    private RenderTexture _target;

    (Mesh mesh, Material material) _quad;
    

    public ScreenRenderer(MeshFilter quadMesh, Material quadMaterial)
    {
        var mat = new Material(quadMaterial);
        quadMesh.GetComponent<MeshRenderer>().material = mat;
        _quad = (quadMesh.mesh, mat);
    }
    
    public ScreenRenderer(Material screenMat, RenderTexture targetTexture)
    {
        _screenMat = new Material(screenMat);
        var w = (float)targetTexture.width;
        var h = (float)targetTexture.height;
        _screenMat.SetVector("_TexParams", new Vector4(
            w, h, 1 + 1f/w, 1 + 1f/h));
        _target = targetTexture;
    }

    public void Push(in DrawCommand cmd)
      => _buffer.Enqueue(cmd);

    public void DrawBuffered()
    {
        var count = _buffer.Count;
        if (count > 0)
        {
            //Debug.Log($"Drawing {count} commands");
        }
        while (_buffer.Count > 0)
            Draw(_buffer.Dequeue());
    }

    public void Draw(in DrawCommand cmd)
    {
        //Debug.Log(cmd);
        var coords = new Vector4(cmd.x, cmd.y, cmd.w, cmd.h);
        _screenMat.SetInteger("_Code", cmd.code);
        _screenMat.SetVector("_Coords", coords);
        _screenMat.SetColor("_Background", cmd.bg);
        _screenMat.SetColor("_Foreground", cmd.fg);
        _screenMat.SetPass(0);
        Graphics.Blit(_screenMat.mainTexture, _target, _screenMat);
        //Graphics.DrawMeshNow(_quad.mesh, Matrix4x4.identity);
    }
}

} // namespace M8
