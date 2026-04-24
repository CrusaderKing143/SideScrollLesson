# Unity 2D PlayerController Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:executing-plans (inline) to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 在 Unity 2D 项目中新增一个最经典、最简单的 `PlayerController`（`Rigidbody2D` 驱动），支持跑动/跳跃/二段跳/冲刺/滑铲。

**Architecture:** 单脚本 `PlayerController.cs` 负责输入读取（旧输入 API）、状态（Normal/Dash/Slide）、地面检测（OverlapBox）与 `Rigidbody2D.velocity` 写入；可选对接 `Animator` 参数但不强依赖。

**Tech Stack:** Unity 6 (6000.0.38f1), 2D Physics (`Rigidbody2D`, `Collider2D`), legacy input (`Input.GetAxisRaw`, `Input.GetButtonDown`).

---

## 文件结构（将创建/修改哪些文件）

**Create:**
- `Assets/Scripts/PlayerController.cs`：核心移动脚本（跑/跳/二段跳/冲刺/滑铲 + 地面检测）

**Create (if missing):**
- `Assets/Scripts/`：脚本目录（当前工程只有 `Assets/Scripts.meta`）

**No required modifications** to existing scene/assets（你把脚本挂到角色上并在 Inspector 里拖拽 `groundCheck` 即可运行）。

---

### Task 1: 建立脚本目录与 PlayerController 脚本骨架

**Files:**
- Create: `Assets/Scripts/PlayerController.cs`

- [ ] **Step 1: 创建目录 `Assets/Scripts`（如果不存在）**
- [ ] **Step 2: 新建 `PlayerController.cs`，包含必要字段与组件引用**

代码（完整文件内容将在 Task 2 一次性给出，避免中间态无法编译）。

- [ ] **Step 3: 进入 Unity，给角色挂载脚本并补齐组件**
  - 角色对象添加：`Rigidbody2D`、`Collider2D`、`SpriteRenderer`
  - `Rigidbody2D`：Body Type = Dynamic，Constraints 勾选 Freeze Rotation Z

---

### Task 2: 实现跑/跳/二段跳 + 地面检测

**Files:**
- Create/Modify: `Assets/Scripts/PlayerController.cs`

- [ ] **Step 1: 实现 OverlapBox 地面检测**
  - 字段：`Transform groundCheck`, `Vector2 groundCheckSize`, `LayerMask groundMask`
  - `isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundMask);`

- [ ] **Step 2: 实现水平移动（Normal 状态）**
  - `x = Input.GetAxisRaw("Horizontal")`
  - `rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y)`

- [ ] **Step 3: 实现跳跃与二段跳**
  - `Input.GetButtonDown("Jump")`
  - 落地重置 `extraJumpsRemaining = extraJumps`
  - 起跳：设置 `rb.velocity.y = jumpVelocity`

- [ ] **Step 4: 手动验证**
  - 预期：左右移动稳定；按 Space 起跳；空中再按一次可二段跳；落地后恢复二段跳次数

---

### Task 3: 实现冲刺（Dash）与滑铲（Slide）

**Files:**
- Modify: `Assets/Scripts/PlayerController.cs`

- [ ] **Step 1: Dash 状态与计时器**
  - 输入：`Input.GetKeyDown(KeyCode.LeftShift)`
  - 条件：`dashCooldownLeft <= 0 && !isDashing && !isSliding`
  - 行为：在 Dash 期间锁定 `rb.velocity.x = dashSpeed * facingDir`
  - 结束：恢复 Normal，并开始冷却 `dashCooldownLeft = dashCooldown`

- [ ] **Step 2: Slide 状态与计时器**
  - 输入：`Input.GetKeyDown(KeyCode.LeftControl)`
  - 条件：`isGrounded && !isDashing && !isSliding`
  - 行为：在 Slide 期间锁定 `rb.velocity.x = slideSpeed * facingDir`
  - 结束：恢复 Normal

- [ ] **Step 3: facingDir 与朝向翻转**
  - 若 `x != 0`：`facingDir = (x > 0 ? 1 : -1)`
  - `SpriteRenderer.flipX = facingDir < 0`

- [ ] **Step 4: 手动验证**
  - 预期：Shift 冲刺持续固定时间且不被方向键干扰；Ctrl 地面滑铲；两者不会互相打架

---

### Task 4: 可选 Animator 参数同步 + Gizmos 辅助

**Files:**
- Modify: `Assets/Scripts/PlayerController.cs`

- [ ] **Step 1: 如果存在 Animator，同步参数（不强制）**
  - `Speed`、`Grounded`、`YVel`、`Dashing`、`Sliding`

- [ ] **Step 2: OnDrawGizmosSelected 画出 groundCheck OverlapBox**
  - 便于在 Scene 视图中调 `groundCheckSize`

- [ ] **Step 3: 手动验证**
  - 预期：无 Animator 也不报错；有 Animator 则参数变化符合预期

---

## 验证清单（完成定义）

- [ ] `PlayerController.cs` 可编译（无 Console 错误）
- [ ] Inspector 可调参数：`moveSpeed`, `jumpVelocity`, `extraJumps`, `dashSpeed`, `dashDuration`, `dashCooldown`, `slideSpeed`, `slideDuration`, `groundCheck*`
- [ ] 跑、跳、二段跳、冲刺、滑铲全部可用

