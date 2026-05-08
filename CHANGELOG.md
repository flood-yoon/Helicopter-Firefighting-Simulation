# Changelog

# 🚁 Helicopter Firefighting Simulation (Unity)

## ver.1
<p align="center">
  <img src="./images/play2.gif" width="900">
  <img src="images/ingame.png" width="900"/>
</p>

## ver.2
<p align="center">
  <img src="./images/ver2.gif" width="900">
  <img src="images/ingame ver2.png" width="900"/>
</p>

## 📋 Update Notes

### 🚁 2026-05-08

#### ⚙️ Features
| Feature | Description |
|---|---|
| 🔽 Descent Force | Enhanced S key descent force + motor performance increases per round |
| 🎮 Mode System | Mode switching via `1` / `2` / `3` keys from round 3 |
| | ▸ `1` Fire Suppression Mode — increased spray, reduced speed |
| | ▸ `2` Normal Mode — balanced stats |
| | ▸ `3` Speed Mode — no water spray, maximum speed |
| 📊 Speed UI | Speed text + mode text UI added |
| 🏁 Speed Cap | Normal mode base speed caps at round 5, displays **Max Speed** upon reaching limit |
| 🔥 Fire Spread | From round 5: fire spreads every 30 seconds |
| | ▸ 30% chance of nearby fire spawn per fire object |
| | ▸ Unhit fires recover 50% of lost HP after 30 seconds |
| 🔢 Fire Limit | Max fire count capped at **100** *(excludes spread fires)* |
| 🌋 Terrain Filter | Fire spawn restricted to terrain mesh only via raycast layer filtering |
| 🏢 Floor System | Fire floor system added — max **3 floors** |
| | ▸ Floor 1 → 30% spread chance |
| | ▸ Floor 2 → 10% spread chance |
| | ▸ Floor 3 → no spread |
| 🌀 Tail Rotor | Tail rotor strength now scales with helicopter speed |

---

## 🐛 Bug Fixes

| # | Fixed |
|---|---|
| 1 | `InvalidOperationException` caused by using legacy `UnityEngine.Input` instead of Input System package |
| 2 | Camera facing backwards instead of the helicopter's forward direction |
| 3 | Mouse sensitivity too high causing camera to flip vertically |
| 4 | S key descent force too weak at higher rounds (around round 5) |
| 5 | Fire spawning on non-terrain objects including the helicopter |
| 6 | Water particle not affecting fire after layer change on Fire prefab |
| 7 | Water particle max angle exceeding 120° (was going up to 180°) |
| 8 | Fire spread count not reflected in the remaining fire count UI |
| 9 | Round clearing when base fire count reached 0 despite spread fires still remaining |
| 10 | Fire stacking beyond 2 floors with no limit |
