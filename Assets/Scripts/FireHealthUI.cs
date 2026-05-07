using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class FireHealthUI : MonoBehaviour
{
    public TMP_Text healthDisplayText;

    // 현재 표시 중인 불 : 마지막으로 물에 맞은 시간
    private Dictionary<Fire, float> activeFires = new Dictionary<Fire, float>();
    private float hideDelay = 4f;

    public void RegisterHit(Fire fire)
    {
        activeFires[fire] = Time.time;
    }

    public void UnregisterFire(Fire fire)
    {
        if (activeFires.ContainsKey(fire))
            activeFires.Remove(fire);
        UpdateDisplay();
    }

    void Update()
    {
        // 4초 이상 안 맞은 불 제거
        List<Fire> toRemove = new List<Fire>();
        foreach (var kvp in activeFires)
        {
            if (kvp.Key == null || Time.time - kvp.Value >= hideDelay)
                toRemove.Add(kvp.Key);
        }
        foreach (var fire in toRemove)
            activeFires.Remove(fire);

        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        if (activeFires.Count == 0)
        {
            healthDisplayText.text = "";
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (var kvp in activeFires)
        {
            if (kvp.Key != null)
                sb.AppendLine("Fire HP : " + Mathf.CeilToInt(kvp.Key.firePower));
        }
        healthDisplayText.text = sb.ToString().TrimEnd();
    }
}