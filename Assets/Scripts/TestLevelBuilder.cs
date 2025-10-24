using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HollowKnightLike.Level
{
    [ExecuteAlways]
    public class TestLevelBuilder : MonoBehaviour
    {
        private static readonly Vector3[] DefaultPlatforms =
        {
            new Vector3(-10f, 0.5f, 0f),
            new Vector3(-4f, 1.5f, 0f),
            new Vector3(2f, 2.5f, 0f),
            new Vector3(10f, 1.75f, 0f)
        };

        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private Tilemap hazardTilemap;
        [SerializeField] private TileBase groundTile;
        [SerializeField] private TileBase hazardTile;
        [SerializeField] private GameObject oneWayPlatformPrefab;
        [SerializeField] private Transform platformParent;
        [SerializeField] private Vector3[] platformPositions = DefaultPlatforms;

        [ContextMenu("Rebuild Level")]
        public void RebuildLevel()
        {
            if (groundTilemap == null || groundTile == null)
            {
                return;
            }

            BuildGround();
            BuildHazards();
            BuildPlatforms();
        }

        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                RebuildLevel();
            }
#endif
        }

        private void Start()
        {
            if (Application.isPlaying)
            {
                RebuildLevel();
            }
        }

        private void BuildGround()
        {
            groundTilemap.ClearAllTiles();

            for (int x = -30; x <= 30; x++)
            {
                groundTilemap.SetTile(new Vector3Int(x, -4, 0), groundTile);
                groundTilemap.SetTile(new Vector3Int(x, -3, 0), groundTile);
                if (x % 4 == 0)
                {
                    groundTilemap.SetTile(new Vector3Int(x, -2, 0), groundTile);
                }
            }

            for (int i = 0; i < 8; i++)
            {
                groundTilemap.SetTile(new Vector3Int(-18 + i, -2 + i / 3, 0), groundTile);
                groundTilemap.SetTile(new Vector3Int(18 - i, -2 + i / 3, 0), groundTile);
            }
        }

        private void BuildHazards()
        {
            if (hazardTilemap == null || hazardTile == null)
            {
                return;
            }

            hazardTilemap.ClearAllTiles();
            for (int x = -6; x <= -4; x++)
            {
                hazardTilemap.SetTile(new Vector3Int(x, -2, 0), hazardTile);
            }

            for (int x = 4; x <= 6; x++)
            {
                hazardTilemap.SetTile(new Vector3Int(x, -2, 0), hazardTile);
            }
        }

        private void BuildPlatforms()
        {
            if (oneWayPlatformPrefab == null)
            {
                return;
            }

            if (platformParent == null)
            {
                var parent = transform.Find("OneWayPlatforms");
                platformParent = parent != null ? parent : new GameObject("OneWayPlatforms").transform;
                platformParent.SetParent(transform, false);
            }

            ClearChildren(platformParent);

            if (platformPositions == null || platformPositions.Length == 0)
            {
                platformPositions = DefaultPlatforms;
            }

            foreach (var position in platformPositions)
            {
                InstantiatePrefab(oneWayPlatformPrefab, position, platformParent);
            }
        }

        private void ClearChildren(Transform parent)
        {
            for (int i = parent.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    UnityEditor.Undo.DestroyObjectImmediate(parent.GetChild(i).gameObject);
                    continue;
                }
#endif
                Destroy(parent.GetChild(i).gameObject);
            }
        }

        private GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Transform parent)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var instance = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, parent);
                instance.transform.position = position;
                return instance;
            }
#endif
            return Instantiate(prefab, position, Quaternion.identity, parent);
        }
    }
}
