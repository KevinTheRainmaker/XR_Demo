using UnityEngine;
using System.Collections;

public class TreeGrowthController : MonoBehaviour
{
    [Header("Growth Settings")]
    public float growthDuration = 2f;
    public float scaleMultiplier = 1f;

    [Header("Growth Anchor")]
    [Tooltip("성장 기준점: Pivot = 프리팹 pivot 기준, Center = 오브젝트 중심 기준, Bottom = 바닥 고정")]
    public GrowthAnchor growthAnchor = GrowthAnchor.Bottom;

    public enum GrowthAnchor { Pivot, Center, Bottom }

    private GameObject currentTree;
    private GrowthAnchor currentTreeAnchor; // 현재 나무의 anchor 추적
    private bool isGrowing = false;

    public IEnumerator GrowTree(GameObject treePrefab, Vector3 position, float targetScale = 1f, float startScale = 0f, GrowthAnchor? anchor = null)
    {
        isGrowing = true;

        if (currentTree != null)
        {
            yield return StartCoroutine(FadeOutTree(currentTree));
        }

        currentTree = Instantiate(treePrefab, position, Quaternion.identity);
        currentTree.transform.SetParent(transform);
        currentTree.transform.localScale = Vector3.one * startScale;

        GrowthAnchor useAnchor = anchor ?? growthAnchor;
        currentTreeAnchor = useAnchor; // 현재 anchor 저장

        yield return StartCoroutine(ScaleUp(currentTree, position, startScale, targetScale, useAnchor));

        isGrowing = false;
    }

    public IEnumerator ReplaceTree(GameObject newTreePrefab, float targetScale = 1f, float startScale = 0f, GrowthAnchor? anchor = null)
    {
        if (currentTree == null)
        {
            Debug.LogWarning("교체할 나무가 없습니다");
            yield break;
        }

        isGrowing = true;

        GrowthAnchor useAnchor = anchor ?? growthAnchor;
        Vector3 anchorPosition = GetAnchorPosition(currentTree, useAnchor);

        Destroy(currentTree);

        currentTree = Instantiate(newTreePrefab, anchorPosition, Quaternion.identity);
        currentTree.transform.SetParent(transform);
        currentTree.transform.localScale = Vector3.one * startScale;

        currentTreeAnchor = useAnchor; // 현재 anchor 저장
        yield return StartCoroutine(ScaleUp(currentTree, anchorPosition, startScale, targetScale, useAnchor));

        isGrowing = false;
    }

    private IEnumerator ScaleUp(GameObject tree, Vector3 anchorPosition, float startScale, float targetScale, GrowthAnchor anchor)
    {
        float elapsed = 0;
        Vector3 startScaleVector = Vector3.one * startScale * scaleMultiplier;
        Vector3 targetScaleVector = Vector3.one * targetScale * scaleMultiplier;

        tree.transform.localScale = Vector3.one;
        Bounds bounds = GetCombinedBounds(tree);
        tree.transform.localScale = startScaleVector;

        Vector3 anchorOffset = Vector3.zero;
        switch (anchor)
        {
            case GrowthAnchor.Center:
                anchorOffset = bounds.center - tree.transform.position;
                break;
            case GrowthAnchor.Bottom:
                anchorOffset = new Vector3(bounds.center.x, bounds.min.y, bounds.center.z) - tree.transform.position;
                break;
            case GrowthAnchor.Pivot:
                anchorOffset = Vector3.zero;
                break;
        }

        while (elapsed < growthDuration)
        {
            float t = elapsed / growthDuration;
            Vector3 currentScale = Vector3.Lerp(startScaleVector, targetScaleVector, t);
            tree.transform.localScale = currentScale;

            if (anchor != GrowthAnchor.Pivot)
            {
                float currentScaleFactor = Mathf.Lerp(startScale, targetScale, t);
                tree.transform.position = anchorPosition - anchorOffset * currentScaleFactor;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        tree.transform.localScale = targetScaleVector;
        if (anchor != GrowthAnchor.Pivot)
        {
            tree.transform.position = anchorPosition - anchorOffset * targetScale;
        }
    }

    private IEnumerator FadeOutTree(GameObject tree)
    {
        float elapsed = 0;
        float fadeTime = 0.5f;
        Vector3 startScale = tree.transform.localScale;

        while (elapsed < fadeTime)
        {
            tree.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / fadeTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(tree);
    }

    // GrowMore 수정: 저장된 anchor 사용
    public IEnumerator GrowMore(float additionalScale = 0.3f, GrowthAnchor? anchor = null)
    {
        if (currentTree == null) yield break;

        isGrowing = true;

        float elapsed = 0;
        Vector3 startScale = currentTree.transform.localScale;
        Vector3 targetScale = startScale + (Vector3.one * additionalScale);

        // anchor 지정되지 않으면 현재 나무의 anchor 사용
        GrowthAnchor useAnchor = anchor ?? currentTreeAnchor;
        Vector3 anchorPosition = GetAnchorPosition(currentTree, useAnchor);

        while (elapsed < growthDuration)
        {
            float t = elapsed / growthDuration;
            currentTree.transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            if (useAnchor != GrowthAnchor.Pivot)
            {
                Bounds bounds = GetCombinedBounds(currentTree);
                Vector3 offset = (useAnchor == GrowthAnchor.Center) ?
                    bounds.center :
                    new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
                currentTree.transform.position += (anchorPosition - offset);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        currentTree.transform.localScale = targetScale;
        isGrowing = false;
    }

    private Vector3 GetAnchorPosition(GameObject tree, GrowthAnchor anchor)
    {
        Bounds bounds = GetCombinedBounds(tree);
        switch (anchor)
        {
            case GrowthAnchor.Center:
                return bounds.center;
            case GrowthAnchor.Bottom:
                return new Vector3(bounds.center.x, bounds.min.y, bounds.center.z);
            default:
                return tree.transform.position;
        }
    }

    private Bounds GetCombinedBounds(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return new Bounds(obj.transform.position, Vector3.zero);
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        return bounds;
    }
}