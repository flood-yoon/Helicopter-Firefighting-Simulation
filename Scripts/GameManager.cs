using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject firePrefab;
    public GameObject mountainObject;
    public GameObject helicopter;
    public Transform startPoint;

    public TMP_Text roundText;
    public TMP_Text fireCountText;

    public Image fadeImage;

    private HelicopterController1 helicopterController;

    private int round = 1;
    private int aliveFireCount;

    private bool roundTransition = false;

    void Start()
    {
        helicopterController =
            helicopter.GetComponent<HelicopterController1>();

        StartRound();
    }

    void StartRound()
    {
        roundTransition = false;

        int fireCount =
            2 * (int)Mathf.Pow(2, round - 1);

        float fireHealth =
            100 * Mathf.Pow(2, round - 1);

        aliveFireCount = fireCount;

        UpdateFireUI();

        roundText.text = "Round " + round;

        for (int i = 0; i < fireCount; i++)
        {
            SpawnFire(fireHealth);
        }
    }

    void SpawnFire(float health)
    {
        MeshCollider meshCollider =
            mountainObject.GetComponent<MeshCollider>();

        Bounds bounds = meshCollider.bounds;

        float randomX =
            Random.Range(bounds.min.x, bounds.max.x);

        float randomZ =
            Random.Range(bounds.min.z, bounds.max.z);

        Ray ray = new Ray(
            new Vector3(
                randomX,
                bounds.max.y + 100f,
                randomZ
            ),
            Vector3.down
        );

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 500f))
        {
            GameObject fire = Instantiate(
                firePrefab,
                hit.point,
                Quaternion.identity
            );

            Fire fireScript =
                fire.GetComponent<Fire>();

            fireScript.firePower = health;
        }
    }

    public void FireExtinguished()
    {
        if (roundTransition)
            return;

        aliveFireCount--;

        UpdateFireUI();

        if (aliveFireCount <= 0)
        {
            roundTransition = true;

            StartCoroutine(RoundClearSequence());
        }
    }

    void UpdateFireUI()
    {
        fireCountText.text =
            "Fires Left : " + aliveFireCount;
    }

    IEnumerator RoundClearSequence()
    {
        roundText.text =
            "Round " + round + " Clear!";

        yield return StartCoroutine(FadeIn());

        helicopter.transform.position =
            startPoint.position;

        helicopter.transform.rotation =
            startPoint.rotation;

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

            color.a =
                Mathf.Lerp(0, 1, time);

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

            color.a =
                Mathf.Lerp(1, 0, time);

            fadeImage.color = color;

            yield return null;
        }
    }
}