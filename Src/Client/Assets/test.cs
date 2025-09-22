using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    private IEnumerator Start()
    {
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(PathUtil.BundleOutPath + "/ui/prefab/test.prefab.ab");
        yield return request;

        AssetBundleCreateRequest request1 = AssetBundle.LoadFromFileAsync(PathUtil.BundleOutPath + "/ui/res/chouka07_tex.png.ab");
        yield return request1;

        AssetBundleRequest bundleRequest = request.assetBundle.LoadAssetAsync("Assets/XLua热更框架/BuildResources/UI/Prefab/Test.prefab");
        yield return bundleRequest;

        GameObject go = Instantiate(bundleRequest.asset) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }

}
