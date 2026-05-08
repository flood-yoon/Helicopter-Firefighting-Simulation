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

Update Notes

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
