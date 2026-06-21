using UnityEngine;

public sealed class GameShootTeach : MonoBehaviour
{
    public Transform player;
    public RestorableDropBox targetBox;
    public GameObject arrowObject;
    public float arrowDistanceFromPlayer = 1.5f;
    public float arrowRotationOffsetZ;
    public bool createArrowIfMissing = true;
    public Color runtimeArrowColor = Color.yellow;
    public float runtimeArrowLength = 1.2f;
    public float runtimeArrowHeadLength = 0.35f;
    public float runtimeArrowWidth = 0.08f;

    private bool guideActive;
    private Material runtimeArrowMaterial;

    public bool IsGuideActive => guideActive;

    private void Awake()
    {
        SetArrowVisible(false);
        enabled = false;
    }

    private void Update()
    {
        if (!guideActive)
            return;

        UpdateArrow();
    }

    public void ShowGuide()
    {
        ResolveReferences();
        if (!arrowObject && createArrowIfMissing)
            CreateRuntimeArrow();

        guideActive = player && targetBox && arrowObject;
        SetArrowVisible(guideActive);
        enabled = guideActive;

        if (guideActive)
            UpdateArrow();
    }

    public void RegisterTargetHit(RestorableDropBox box, int hitCount, int hitsToDrop)
    {
        if (!guideActive)
            return;

        if (!targetBox)
            targetBox = box;

        if (box != targetBox)
            return;

        if (hitCount >= hitsToDrop)
            FinishGuide();
    }

    private void FinishGuide()
    {
        guideActive = false;
        SetArrowVisible(false);
        enabled = false;
    }

    private void UpdateArrow()
    {
        if (!player || !targetBox || !arrowObject)
        {
            FinishGuide();
            return;
        }

        Vector3 direction = targetBox.transform.position - player.position;
        if (direction.sqrMagnitude <= Mathf.Epsilon)
            return;

        Vector3 normalizedDirection = direction.normalized;
        arrowObject.transform.position = player.position + normalizedDirection * arrowDistanceFromPlayer;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + arrowRotationOffsetZ;
        arrowObject.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void ResolveReferences()
    {
        if (!player)
        {
            PlayerController controller = FindObjectOfType<PlayerController>();
            if (controller)
                player = controller.transform;
        }

        if (!targetBox)
            targetBox = GetComponent<RestorableDropBox>();

        if (!targetBox)
            targetBox = FindObjectOfType<RestorableDropBox>();
    }

    private void CreateRuntimeArrow()
    {
        arrowObject = new GameObject("ShootTeachArrow");

        float tipX = runtimeArrowLength * 0.5f;
        float tailX = -runtimeArrowLength * 0.5f;
        float headBaseX = tipX - runtimeArrowHeadLength;
        float headY = runtimeArrowHeadLength * 0.55f;

        CreateArrowLine("Body", new Vector3(tailX, 0f, 0f), new Vector3(tipX, 0f, 0f));
        CreateArrowLine("HeadUp", new Vector3(tipX, 0f, 0f), new Vector3(headBaseX, headY, 0f));
        CreateArrowLine("HeadDown", new Vector3(tipX, 0f, 0f), new Vector3(headBaseX, -headY, 0f));

        SetArrowVisible(false);
    }

    private void CreateArrowLine(string lineName, Vector3 start, Vector3 end)
    {
        GameObject lineObject = new GameObject(lineName);
        lineObject.transform.SetParent(arrowObject.transform, false);

        LineRenderer line = lineObject.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.positionCount = 2;
        line.SetPosition(0, start);
        line.SetPosition(1, end);
        line.startWidth = runtimeArrowWidth;
        line.endWidth = runtimeArrowWidth;
        line.startColor = runtimeArrowColor;
        line.endColor = runtimeArrowColor;
        line.sortingOrder = 100;

        Material material = GetRuntimeArrowMaterial();
        if (material)
            line.material = material;
    }

    private Material GetRuntimeArrowMaterial()
    {
        if (runtimeArrowMaterial)
            return runtimeArrowMaterial;

        Shader shader = Shader.Find("Sprites/Default");
        if (!shader)
            shader = Shader.Find("Unlit/Color");

        if (!shader)
            return null;

        runtimeArrowMaterial = new Material(shader);
        runtimeArrowMaterial.color = runtimeArrowColor;
        return runtimeArrowMaterial;
    }

    private void SetArrowVisible(bool visible)
    {
        if (arrowObject)
            arrowObject.SetActive(visible);
    }
}
