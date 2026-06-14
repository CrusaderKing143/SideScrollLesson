# ShootController Debug Aim Line Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 在 `Assets/Scripts/ShootController.cs` 的 `Update()` 中每帧用 `Debug.DrawLine` 画出“枪口到鼠标世界坐标”的调试瞄准线，并提供 Inspector 开关与颜色配置。

**Architecture:** 在不改变现有射击/输入逻辑的前提下，复用现有 `GetMouseWorldOnPlane(bulletSpawnPoint)` 计算终点；新增少量可配置字段控制是否绘制与线条颜色（可选持续时间）。

**Tech Stack:** Unity (C#), `Debug.DrawLine`, InputSystem/Mouse（现有代码已使用）

---

## File Structure

**Modify:**
- `Assets/Scripts/ShootController.cs`
  - 新增 2-3 个 public 字段（开关/颜色/可选 duration）
  - 在 `Update()` 中每帧绘制（条件：开关开启且 `bulletSpawnPoint != null`）

**No new files / tests**（Unity 场景调试线为纯调试辅助；验证以编辑器内观测与 Console 编译通过为准）

---

### Task 1: 在 Update() 每帧绘制 Debug 瞄准线

**Files:**
- Modify: `Assets/Scripts/ShootController.cs`

- [ ] **Step 1: 在类字段区新增 Debug 配置项（Inspector 可调）**

在 `ShootController` 类里（建议放在 `[Header("瞄准 / 飞行")]` 之后或 `Update()` 之前）加入：

```csharp
[Header("Debug")]
public bool debugDrawAimLine = true;
public Color debugAimLineColor = Color.cyan;
public float debugAimLineDuration = 0f;
```

- [ ] **Step 2: 在 Update() 中加入每帧绘制逻辑（不影响开火点击）**

将 `Update()` 调整为“先画线、再处理点击开火”（或保持点击逻辑不动，仅在前面插入画线段落）。目标代码形态如下：

```csharp
private void Update()
{
    if (debugDrawAimLine && bulletSpawnPoint != null)
    {
        Vector3 start = bulletSpawnPoint.position;
        Vector3 end = GetMouseWorldOnPlane(bulletSpawnPoint);
        Debug.DrawLine(start, end, debugAimLineColor, debugAimLineDuration);
    }

    if (!WasLeftClickPressedThisFrame())
        return;

    Shoot("Yellow");
}
```

注意点：
- `GetMouseWorldOnPlane` 返回 `Vector2`，赋给 `Vector3 end` 是安全的（隐式转换补 0）。
- 不做 Raycast，不更改 `Shoot()` / `GetShotDirection()`。

- [ ] **Step 3: 在 Unity 编辑器中验证（手动）**

在 Unity 中：
- 打开包含该脚本的场景并运行 Play
- 选中 `ShootController` 所在对象，在 Inspector 里确认 `debugDrawAimLine` 开启
- 移动鼠标：在 Scene 视图应看到从 `bulletSpawnPoint` 指向鼠标世界点的线段持续更新
- 关闭 `debugDrawAimLine`：线段应消失
- 点击开火：仍然会按原逻辑发射黄色子弹
- Console：无编译错误/异常

- [ ] **Step 4: （可选）把 duration 设为 0 以外的值验证“留痕”**

将 `debugAimLineDuration` 设为 `0.05f`（或你喜欢的值），观察线段在低帧率/暂停时是否更易观察。

