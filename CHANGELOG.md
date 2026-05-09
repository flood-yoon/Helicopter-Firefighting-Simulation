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

---

## 📋 Current Update Notes (Summary)
*Key features of the latest build.*

| Feature | Description |
|:--- |:--- |
| 🔽 Descent Force | Enhanced S-key descent force + scaling motor performance per round. |
| 🎮 Mode System | On-the-fly mode switching (`1` / `2` / `3`) starting from Round 3. |
| 📊 UI System | Integrated real-time Speedometer, Mode indicator, and Fire counter. |
| 🏁 Speed Cap | Base speed caps at Round 5 with a **"Max Speed"** status indicator. |
| 🔥 Fire Spread | From Round 5: 30% chance for fires to spread to nearby areas every 30s. |
| 🌋 Terrain Filter | Fire spawn logic restricted to terrain meshes via Raycast Layer Filtering. |
| 🏢 Floor System | Implemented vertical stacking logic for fire objects (Max **3 floors**). |
| 🌀 Tail Rotor | Tail rotor RPM dynamically scales with the main rotor and flight speed. |

---

## 🐛 Bug Fixes
*Technical issues resolved during development.*

| # | Fixed Issues |
|:---:|:--- |
| 1 | `InvalidOperationException`: Migrated from Legacy Input to **Input System Package**. |
| 2 | Camera Orientation: Fixed the camera facing backward upon initialization. |
| 3 | Camera Physics: Resolved vertical flipping issues caused by excessive mouse sensitivity. |
| 4 | Descent Physics: Overrode hovering lift to fix weak descent force in high-speed rounds. |
| 5 | Spawn Logic: Prevented fires from spawning on the helicopter or invalid objects. |
| 6 | Interaction: Fixed water particles ignoring fire colliders after layer updates. |
| 7 | Particle Tuning: Clamped water spray max angle to 120° for realistic trajectory. |
| 8 | UI Sync: Fixed remaining fire counter to include dynamically spread fires. |
| 9 | Logic Error: Prevented premature round clearing when spread fires were still active. |
| 10 | Floor Limit: Capped fire stacking to 3 floors to maintain performance and logic. |

---

## 📈 Development Journey (Growth Log)
*A chronological log showing the evolution of the project from Day 1.*

### 🌱 Day 1: Foundation & Physics Setup
- **Asset Integration:** Selected and imported high-quality helicopter and terrain assets.
- **Physics Implementation:** Configured `Rigidbody` and collision layers. 
- **Collision Optimization:** Initially used a full-body `BoxCollider`, which caused unnatural interactions with mountains. Resolved this by applying **precision colliders to the landing gear**, significantly improving the "touchdown" feel.
- **Custom VFX:** Developed a unique **Water Spray System** using Unity's Particle System to overcome the lack of suitable external assets.

### 🌿 Day 2: Interaction & Core Loop
- **Collision Logic:** Identified an issue where water particles passed through fire objects. Resolved this by switching the Particle Collision mode from **Local to World**, enabling accurate interaction.
- **Gameplay Design:** Established the core loop where clearing fires triggers the next round.
- **Scaling Difficulty:** Implemented initial scaling logic where fire count and HP doubled each round, alongside a 1.5x helicopter speed increase.

### 🌳 Day 3: Optimization & Balancing
- **Balance Tuning:** **[Feedback]** Playtesting revealed that the difficulty ramp was too steep. **Adjusted Fire HP scaling to 1.2x** per round to improve engagement and "fun factor."
- **Advanced Systems:** Added the **3-Mode Flight System** and **Dynamic Fire Spread** logic.
- **Physics Refinement:** Fixed a critical "Descent Issue" where high motor power made landing difficult by **bypassing hovering lift** when the descent key is pressed.
- **Final Polish:** Synchronized tail rotor speed with the main rotor for better control stability and increased water particle lifetime to 3.0s to fix hit-detection issues in later rounds.

---

## 🚀 Future Roadmap
- **Optimization:** Implement `Object Pooling` to handle massive fire/particle counts efficiently.
- **Environment:** Add water refilling mechanics and wind physics affecting spray trajectory.
