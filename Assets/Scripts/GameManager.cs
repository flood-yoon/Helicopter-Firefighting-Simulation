using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Header("기본 설정")]
    public GameObject firePrefab;
    public GameObject mountainObject;
    public GameObject helicopter;
    public Transform startPoint;

    [Header("UI")]
    public TMP_Text roundText;
    public TMP_Text fireCountText;
    public TMP_Text modeText;
    public TMP_Text speedText;
    public Image fadeImage;

    [Header("불 번짐 설정")]
    public float spreadInterval = 30f;
    public float spreadRadius = 15f;
    public int maxBaseFireCount = 100;

    [Header("레이어 설정")]
    public LayerMask terrainLayer;      // 지형 메쉬 레이어만 선택
    public LayerMask fireLayer;         // Fire 오브젝트 레이어

    private HelicopterController1 helicopterController;
    private WaterSpray waterSpray;

    private int round = 1;
    private int aliveFireCount;
    private bool roundTransition = false;
    private int currentMode = 2;

    private float baseEmissionRate = 10f;
    private float baseAngle = 10f;
    private float roundAngle;
    private float roundEmissionRate;

    private Coroutine spreadCoroutine;

    void Start()
    {
        helicopterController = helicopter.GetComponent<HelicopterController1>();
        waterSpray = helicopter.GetComponentInChildren<WaterSpray>();
        SetMode(2);
        StartRound();
    }

    void StartRound()
    {
        roundTransition = false;

        int fireCount = Mathf.Min(2 * (int)Mathf.Pow(2, round - 1), maxBaseFireCount);
        float fireHealth = 100f * Mathf.Pow(1.2f, round - 1);

        aliveFireCount = fireCount;
        UpdateFireUI();
        roundText.text = "Round " + round;

        helicopterController.currentRound = round;
        helicopterController.modeUnlocked = round >= 3;
        helicopterController.roundSpeedBonus = (round - 1) * 5f;

        roundAngle = baseAngle * Mathf.Pow(1.5f, round - 1);
        roundEmissionRate = baseEmissionRate * Mathf.Pow(1.5f, round - 1);

        ApplyModeSettings(currentMode);
        UpdateModeUI();

        for (int i = 0; i < fireCount; i++)
            SpawnFireOnTerrain(fireHealth);

        if (round >= 5)
        {
            if (spreadCoroutine != null) StopCoroutine(spreadCoroutine);
            spreadCoroutine = StartCoroutine(FireSpreadLoop());
        }
    }

    // ── 지형에만 불 생성 ───────────────────────────────
    void SpawnFireOnTerrain(float health)
    {
        MeshCollider meshCollider = mountainObject.GetComponent<MeshCollider>();
        Bounds bounds = meshCollider.bounds;

        int maxAttempts = 10;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomZ = Random.Range(bounds.min.z, bounds.max.z);

            Ray ray = new Ray(new Vector3(randomX, bounds.max.y + 100f, randomZ), Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, 500f, terrainLayer))
            {
                CreateFire(hit.point, health, 1, null);
                return;
            }
        }
    }

    // ── 불 오브젝트 생성 공통 함수 ─────────────────────
    Fire CreateFire(Vector3 position, float health, int floorLevel, Fire parent)
    {
        GameObject fireObj = Instantiate(firePrefab, position, Quaternion.identity);
        Fire fireScript = fireObj.GetComponent<Fire>();
        fireScript.firePower = health;
        fireScript.maxFirePower = health;
        fireScript.floorLevel = floorLevel;
        fireScript.parentFire = parent;

        if (parent != null)
            parent.childFire = fireScript;

        return fireScript;
    }

    // ── 번짐 루프 ──────────────────────────────────────
    IEnumerator FireSpreadLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spreadInterval);
            if (roundTransition) yield break;

            float fireHealth = 100f * Mathf.Pow(1.2f, round - 1);
            Fire[] fires = FindObjectsByType<Fire>(FindObjectsInactive.Exclude);

            foreach (Fire fire in fires)
            {
                if (fire == null) continue;

                // 체력 회복
                fire.TryRecover();

                // 층수별 확산
                TrySpread(fire, fireHealth);
            }
        }
    }

    void TrySpread(Fire fire, float health)
    {
        if (fire.floorLevel == 3) return; // 3층은 확산 없음

        if (fire.floorLevel == 1)
        {
            // 1층: 30% 확률로 인근 지형에 생성
            if (Random.value < 0.3f)
            {
                bool success = TrySpawnNearOnTerrain(fire.transform.position, health);
                if (!success)
                {
                    // 지형 자리 없으면 30% 확률로 2층 생성
                    if (Random.value < 0.3f && !fire.HasChild())
                        SpawnOnTopOfFire(fire, health, 2);
                }
            }
        }
        else if (fire.floorLevel == 2)
        {
            // 2층: 10% 확률로 인근 1층 불 위 또는 지형에 생성
            if (Random.value < 0.1f)
            {
                bool success = TrySpawnOnNearby1F(fire.transform.position, health);
                if (!success)
                    success = TrySpawnNearOnTerrain(fire.transform.position, health);

                if (!success)
                {
                    // 자리 없으면 10% 확률로 3층 생성
                    if (Random.value < 0.1f && !fire.HasChild())
                        SpawnOnTopOfFire(fire, health, 3);
                }
            }
        }
    }

    // 인근 지형에 불 생성 시도
    bool TrySpawnNearOnTerrain(Vector3 origin, float health)
    {
        for (int i = 0; i < 5; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spreadRadius;
            Vector3 spawnPos = origin + new Vector3(randomCircle.x, 100f, randomCircle.y);

            Ray ray = new Ray(spawnPos, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 200f, terrainLayer))
            {
                // 헬리콥터 위치와 너무 가까우면 스킵
                if (Vector3.Distance(hit.point, helicopter.transform.position) < 5f)
                    continue;

                Fire newFire = CreateFire(hit.point, health, 1, null);
                aliveFireCount++;
                UpdateFireUI();
                return true;
            }
        }
        return false;
    }

    // 인근 1층 불 위에 2층 생성 시도
    bool TrySpawnOnNearby1F(Vector3 origin, float health)
    {
        Fire[] fires = FindObjectsByType<Fire>(FindObjectsInactive.Exclude);
        List<Fire> candidates = new List<Fire>();

        foreach (Fire f in fires)
        {
            if (f.floorLevel == 1 && !f.HasChild() &&
                Vector3.Distance(f.transform.position, origin) < spreadRadius)
                candidates.Add(f);
        }

        if (candidates.Count == 0) return false;

        Fire target = candidates[Random.Range(0, candidates.Count)];
        Vector3 pos = target.transform.position + Vector3.up * 1.5f;
        Fire newFire = CreateFire(pos, health, 2, target);
        aliveFireCount++;
        UpdateFireUI();
        return true;
    }

    // 불 위에 불 생성 (2층, 3층)
    void SpawnOnTopOfFire(Fire baseFire, float health, int level)
    {
        Vector3 pos = baseFire.transform.position + Vector3.up * 1.5f;
        Fire newFire = CreateFire(pos, health, level, baseFire);
        aliveFireCount++;
        UpdateFireUI();
    }

    // ── 모드 전환 ──────────────────────────────────────
    public void SetMode(int mode)
    {
        if (mode != 2 && round < 3) return;
        currentMode = mode;
        helicopterController.currentMode = mode;
        ApplyModeSettings(mode);
        UpdateModeUI();
    }

    void ApplyModeSettings(int mode)
    {
        if (waterSpray == null) return;

        if (mode == 1)
        {
            waterSpray.ApplyParticleSettings(roundAngle * 0.6f, roundEmissionRate * 1.5f);
            waterSpray.SetWaterDisabled(false);
            helicopterController.modeSpeedMultiplier = 0.6f;
        }
        else if (mode == 2)
        {
            waterSpray.ApplyParticleSettings(roundAngle, roundEmissionRate);
            waterSpray.SetWaterDisabled(false);
            helicopterController.modeSpeedMultiplier = 1f;
        }
        else if (mode == 3)
        {
            waterSpray.SetWaterDisabled(true);
            helicopterController.modeSpeedMultiplier = 2f;
        }
    }

    void UpdateModeUI()
    {
        bool maxed = helicopterController.IsNormalSpeedMaxed();

        string modeName;
        string speedLabel;

        if (currentMode == 1)
        {
            modeName = "1. Fire Suppression Mode";
            speedLabel = "Low Speed";
        }
        else if (currentMode == 2)
        {
            modeName = "2. Normal Mode";
            speedLabel = maxed ? "Max Speed" : "Normal Speed";
        }
        else
        {
            modeName = "3. Speed Mode";
            speedLabel = maxed ? "Extreme Speed" : "High Speed";
        }

        if (modeText != null) modeText.text = round >= 3 ? modeName : "2. Normal Mode";
        if (speedText != null) speedText.text = round >= 3 ? speedLabel : "Normal Speed";
    }

    public void FireExtinguished()
    {
        if (roundTransition) return;

        aliveFireCount--;
        UpdateFireUI();

        if (aliveFireCount <= 0)
        {
            roundTransition = true;
            if (spreadCoroutine != null) StopCoroutine(spreadCoroutine);
            StartCoroutine(RoundClearSequence());
        }
    }

    void UpdateFireUI()
    {
        fireCountText.text = "Fires Left : " + aliveFireCount;
    }

    // 개발자 테스트용 옵션
    void Update()
    {
        if (Keyboard.current.equalsKey.wasPressedThisFrame && !roundTransition)
        {
            roundTransition = true;
            if (spreadCoroutine != null) StopCoroutine(spreadCoroutine);

            // 씬에 남아있는 불 전부 제거
            Fire[] fires = FindObjectsByType<Fire>(FindObjectsInactive.Exclude);
            foreach (Fire fire in fires)
                Destroy(fire.gameObject);

            StartCoroutine(RoundClearSequence());
        }
    }

    IEnumerator RoundClearSequence()
    {
        roundText.text = "Round " + round + " Clear!";
        yield return StartCoroutine(FadeIn());

        helicopter.transform.position = startPoint.position;
        helicopter.transform.rotation = startPoint.rotation;
        yield return new WaitForSeconds(1f);

        round++;
        helicopterController.tiltMoveForce *= 1.5f;
        StartRound();

        yield return StartCoroutine(FadeOut());
    }

    IEnumerator FadeIn()
    {
        Color color = fadeImage.color;
        float time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, time);
            fadeImage.color = color;
            yield return null;
        }
    }

    IEnumerator FadeOut()
    {
        Color color = fadeImage.color;
        float time = 0;
        while (time < 1f)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, time);
            fadeImage.color = color;
            yield return null;
        }
    }
}