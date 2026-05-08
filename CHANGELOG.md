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

## Update Notes

2026-05-08

Enhanced S key descent force + motor performance increases per round

Mode switching via 1/2/3 keys from round 3 (Fire Suppression / Normal / Speed)

Speed text + mode text UI added

Normal mode base speed caps at round 5, displays "Max Speed" upon reaching limit

From round 5: fire spreads every 30 seconds, 30% chance of nearby fire spawn, unhit fires recover 50% of lost HP

Max fire count capped at 100 (excludes spread fires)

Fire spawn restricted to terrain mesh only via raycast layer filtering

Prevented fire from spawning on helicopter

Fire floor system added (floor 1 / 2 / 3, max 3 floors)

Different spread probability per floor

Tail rotor strength scales with speed

## Bug Fixes

Fixed InvalidOperationException caused by using legacy Input System (UnityEngine.Input) instead of Input System package
Fixed camera facing backwards instead of the helicopter's forward direction
Fixed mouse sensitivity too high causing camera to flip vertically
Fixed S key descent force too weak at higher rounds (around round 5)
Fixed fire spawning on non-terrain objects including the helicopter
Fixed water particle not affecting fire after layer change on Fire prefab
Fixed water particle max angle exceeding 120 degrees (was going up to 180)
Fixed fire spread count not reflected in the remaining fire count UI
Fixed round clearing when base fire count reached 0 despite spread fires still remaining
Fixed fire stacking beyond 2 floors with no limit

