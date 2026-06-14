# ShootController：Update 中 Debug.DrawLine 绘制瞄准线（设计说明）

## 背景与目标

当前 `Assets/Scripts/ShootController.cs` 的射击方向由 `GetShotDirection()` 计算并用于生成子弹刚体速度；脚本本身没有做 Physics 射线检测。因此这里的“射击线”定义为**用于调试的瞄准线**：从枪口（`bulletSpawnPoint`）到鼠标所指的世界坐标点。

目标：
- 在 **`Update()` 每帧**绘制一条调试线段，帮助确认鼠标世界坐标与枪口位置关系是否正确。
- 不影响现有射击与输入逻辑。
- 可通过 Inspector 开关控制是否绘制，避免发布/性能干扰。

## 非目标（Non-goals）

- 不新增/修改任何命中判定（不做 Raycast2D / Raycast3D）。
- 不在 Game 视图内渲染可见激光（不使用 `LineRenderer`）。
- 不改变 `Shoot()` 的调用频率与子弹生成逻辑。

## 设计概要

### 绘制时机
- 在 `Update()` 中，每帧执行（满足开关开启且 `bulletSpawnPoint != null`）。

### 线段端点
- **起点**：`bulletSpawnPoint.position`
- **终点**：`GetMouseWorldOnPlane(bulletSpawnPoint)`
  - 复用现有方法，使用 `shootCamera`（若未指定则使用 `Camera.main`），并以 `bulletSpawnPoint` 的屏幕深度作为 `screen.z` 参考，确保鼠标世界坐标与枪口处在同一深度平面。

### 可配置项（Inspector）
新增以下字段：
- `public bool debugDrawAimLine = true;`
- `public Color debugAimLineColor = Color.cyan;`
- （可选）`public float debugAimLineDuration = 0f;`（每帧重画可用 0；若想在停帧/低帧率时留痕可设一个小值）

### 绘制 API
使用 `Debug.DrawLine(start, end, debugAimLineColor, debugAimLineDuration)`。

## 验收标准
- 开关开启时，在 Scene 视图可以看到从枪口到鼠标指向点的线段，并随鼠标移动实时更新。
- 开关关闭时，不绘制任何线段。
- 不引入编译错误，不改变点击开火行为。

