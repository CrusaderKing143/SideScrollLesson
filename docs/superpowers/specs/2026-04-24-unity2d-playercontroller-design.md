## 目标

为横版 2D 动作平台角色提供最经典、最简单的移动能力（单脚本实现）：

- 跑动
- 跳跃
- 二段跳
- 冲刺
- 滑铲

约束：

- 使用 `Rigidbody2D`（以 `rb.velocity` 为主）
- 使用 `Collider2D` + 地面检测（OverlapBox/OverlapCircle）
- 使用旧输入 `Input.GetAxisRaw`（零额外配置）
- 不强依赖动画系统（有 `Animator` 则可选同步参数，没有也能跑）

## 非目标（刻意不做）

- 墙跳、攀爬、攻击、受击硬直等拓展能力
- 可变跳高、coyote time、jump buffer（如需手感优化再加）
- 滑铲时自动缩小碰撞体（保持最简；如需“钻低洞”再加）

## 场景/组件假设

角色 GameObject 上至少挂载：

- `Rigidbody2D`（Dynamic，Freeze Rotation Z）
- `Collider2D`（Capsule/Box 均可）
- `SpriteRenderer`（用于朝向翻转）
- 可选：`Animator`

地面检测需要：

- `Transform groundCheck`（一般放在脚底）
- `Vector2 groundCheckSize`（OverlapBox 尺寸，单位世界坐标）
- `LayerMask groundMask`

## 输入约定（旧输入）

- 水平移动：`Input.GetAxisRaw("Horizontal")`
- 跳跃：`Input.GetButtonDown("Jump")`
- 冲刺：`Input.GetKeyDown(KeyCode.LeftShift)`
- 滑铲：`Input.GetKeyDown(KeyCode.LeftControl)`

## 行为规则（经典实现）

### 跑动

- 非冲刺/滑铲状态下，水平速度由输入直接控制：
  - `rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y)`
- 冲刺/滑铲时锁定水平速度，不受水平输入影响。

### 跳跃 / 二段跳

- `isGrounded` 时按跳跃：设置 `rb.velocity.y = jumpVelocity`
- 空中允许再跳 1 次（`extraJumps = 1`）
- 重新落地时重置剩余跳次数

### 冲刺

- 触发后进入 `Dash` 状态，持续 `dashDuration`
- 期间水平速度强制为 `dashSpeed * facingDir`（`facingDir` 为 \(\pm 1\)）
- 可选：冷却 `dashCooldown`，以及“落地重置一次冲刺”策略

### 滑铲

- 仅在地面触发
- 触发后进入 `Slide` 状态，持续 `slideDuration`
- 期间水平速度强制为 `slideSpeed * facingDir`

## 状态与数据

### 状态枚举

- `Normal`
- `Dash`
- `Slide`

### 关键计时器

- `dashTimeLeft` / `dashCooldownLeft`
- `slideTimeLeft`

### 面向（facing）

- 若 `x != 0`，更新 `facingDir = sign(x)`
- 使用 `SpriteRenderer.flipX` 或 `transform.localScale.x` 翻转

## Animator（可选对接）

若存在 `Animator`，脚本可以（可选）同步常见参数：

- `Speed`（abs(x)）
- `Grounded`（bool）
- `YVel`（rb.velocity.y）
- `Dashing`（bool）
- `Sliding`（bool）

## 成功标准

- 在默认输入映射下可直接运行并完成：跑、跳、二段跳、冲刺、滑铲
- 冲刺/滑铲期间不会被水平输入“拉回”导致速度抖动
- 落地检测稳定，不会频繁误判（参数可在 Inspector 调整）

