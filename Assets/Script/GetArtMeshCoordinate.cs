using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Rendering;

// Live2DのArtMeshのワールド座標を取得するためのクラス
public class GetArtMeshCoordinate : MonoBehaviour {

    public Transform ArtMesh;  // ワールド座標を取得したいArtMesh
    public int vertexNum;      // 頂点の番号。ゲームを再生しながらinspectorの値を変えて確認しよう。
    private CubismRenderer _cubismRenderer; // ArtMeshのCubismRenderer。Start関数で自動的に取得します。

    Vector3 vertexPosition;
    Vector3 worldPosition;
    Transform parent;

    private void Start()
    {
        // ArtMeshのCubismRendererを取得
        _cubismRenderer = ArtMesh.GetComponent<CubismRenderer>();
    }

    private void FixedUpdate()
    {
        vertexPosition = _cubismRenderer.Mesh.vertices[vertexNum];

        // 親子関係を解除
        parent = ArtMesh.parent;
        ArtMesh.parent = null;

        // ワールド座標取得
        worldPosition = ArtMesh.TransformPoint(vertexPosition);
        transform.position = worldPosition;

        // 親を再設定
        ArtMesh.parent = parent;
    }
}
