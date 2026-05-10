# 高中零基础 Unity 教学案：从 player 搭建抬枪瞄准与子弹发射系统

适用工程：`ScrollLesson`

教学对象：高中生，默认不了解 C#，不了解 Unity 的父子物体、Transform、组件、坐标等概念。

学生机起点：场景中已经有 `player`、地图、摄像机、角色素材、武器素材、子弹素材，但还没有 `Arm`、`Armweapon`、`ShootPoint`，也还没有 `ArmAimController` 和 `ShootController`。

最终完成效果：

```text
player
└── Arm                  手臂旋转点，脚本控制它旋转
    └── Armweapon        武器图片，只负责显示
        └── ShootPoint   枪口点，子弹从这里生成
```

运行后：

1. 鼠标移动时，`Arm` 带着武器上下瞄准。
2. 手臂旋转角度有限制，不会乱转一圈。
3. 点击鼠标左键，子弹从 `ShootPoint` 发射。
4. 子弹沿当前枪口方向飞，而不是直接飞向鼠标。

---

## 一、教师使用说明

这份教案的核心设计是“代码从小长大”。不要一上来让学生复制最终脚本。推荐按下面顺序上课：

```text
先搭层级
再写最小旋转代码
再读鼠标
再把鼠标方向变成角度
再加角度限制
再做最小子弹
再让子弹移动
再让子弹按枪口方向飞
最后整理成完整脚本
```

这样学生能看到每一段代码是为了解决一个具体问题而出现的。

建议课时：

```text
第 1 课时：搭层级，理解 Transform 和父子物体，完成手臂测试旋转。
第 2 课时：完成鼠标瞄准和角度限制。
第 3 课时：完成子弹生成、移动、按枪口方向发射。
```

如果只有 2 课时，可以把“代码版本演进”讲得更快，最终仍然保留学生实操。

---

## 二、课堂核心概念

### 1. GameObject 是物体

Unity 里场景中的东西基本都可以看成 GameObject。

```text
player 是一个 GameObject
Arm 是一个 GameObject
Armweapon 是一个 GameObject
ShootPoint 也是一个 GameObject
```

### 2. Component 是功能

GameObject 自己只是一个容器，真正的功能来自组件。

```text
SpriteRenderer   让物体显示图片
Rigidbody2D      让物体进入 2D 物理系统
ArmAimController 让 Arm 跟着鼠标旋转
ShootController  让 player 能发射子弹
```

### 3. Transform 是位置、旋转、缩放

每个 GameObject 都一定有 Transform。

```text
Position  位置
Rotation  旋转
Scale     缩放
```

2D 游戏里通常只改 `Rotation Z`。

### 4. 父子物体决定“跟随关系”

本课最重要的结构：

```text
Arm
└── Armweapon
    └── ShootPoint
```

教师讲法：

> 父物体动，子物体跟着动。父物体转，子物体绕着父物体转。所以我们要旋转 `Arm`，让 `Armweapon` 和 `ShootPoint` 跟着它转。

### 5. 为什么不直接旋转 Armweapon

错误结构：

```text
只旋转 Armweapon
```

问题：

```text
武器会绕自己的中心转，看起来像枪在手上打转。
```

正确结构：

```text
Arm 是看不见的转轴
Armweapon 是显示出来的武器
旋转 Arm，而不是旋转 Armweapon
```

---

## 三、学生机从零搭建层级

### 1. 起点检查

让学生确认场景里至少有：

```text
player
Main Camera
```

让学生确认项目资源里至少有：

```text
Assets/Art/Raw/handwear.png
Assets/Art/Raw/Bullets.png
```

如果学生工程里资源名称不同，教师要提前统一说明：

```text
武器图片用哪一个
子弹图片用哪一个
```

### 2. 创建 Arm

操作：

1. 在 Hierarchy 里找到 `player`。
2. 右键 `player`。
3. 选择 `Create Empty`。
4. 改名为 `Arm`。

结果：

```text
player
└── Arm
```

推荐初始 Transform：

```text
Position X = -0.425
Position Y = -0.211
Position Z = 0

Rotation X = 0
Rotation Y = 0
Rotation Z = 0

Scale X = 1
Scale Y = 1
Scale Z = 1
```

说明：

```text
Arm 放在角色手臂根部附近。
它不是图片。
它是旋转中心。
```

如果学生角色大小不同，不必死抄数值。让学生把 `Arm` 放到“希望枪绕着旋转”的位置。

### 3. 创建 Armweapon

操作：

1. 右键 `Arm`。
2. 选择 `Create Empty`。
3. 改名为 `Armweapon`。

结果：

```text
player
└── Arm
    └── Armweapon
```

推荐初始 Transform：

```text
Position X = 0
Position Y = 0
Position Z = 0
Rotation Z = 0
Scale X = 1
Scale Y = 1
Scale Z = 1
```

给 `Armweapon` 加显示图片：

1. 选中 `Armweapon`。
2. Inspector 点 `Add Component`。
3. 搜索 `Sprite Renderer`。
4. 添加 `Sprite Renderer`。
5. 把武器图片拖到 `Sprite`。

本工程通常使用：

```text
Assets/Art/Raw/handwear.png
```

如果图片被切成多个 Sprite，选择合适的子 Sprite，例如 `handwear_0`。

课堂提醒：

```text
Armweapon 负责显示。
Arm 负责旋转。
不要把脚本挂反。
```

### 4. 调整 Armweapon 位置

选中 `Armweapon`，把武器移动到角色手上。

教师要强调：

```text
移动 Armweapon 是调整武器相对 Arm 的偏移。
移动 Arm 是调整旋转中心。
```

建议让学生把 Scene 左上角切到：

```text
Pivot
Local 或 Global 都可以，初学先用 Global 更直观
```

常见问题：

```text
学生觉得移动 Armweapon 时 Arm 也动了。
```

解释：

```text
如果 Scene 视图处于 Center 模式，移动手柄会显示在可见包围盒中心，容易看错。
看 Inspector 里的 Arm Position，真正的 Arm 坐标没有被 Armweapon 改掉。
```

### 5. 创建 ShootPoint

操作：

1. 右键 `Armweapon`。
2. 选择 `Create Empty`。
3. 改名为 `ShootPoint`。

结果：

```text
player
└── Arm
    └── Armweapon
        └── ShootPoint
```

把 `ShootPoint` 移动到枪口最前端。

推荐初始 Transform：

```text
Position X = 1.172
Position Y = -0.3
Position Z = 0
Rotation Z = 0
Scale X = 1
Scale Y = 1
Scale Z = 1
```

说明：

```text
ShootPoint 是子弹出生点。
它必须跟着枪一起转。
所以 ShootPoint 要作为 Armweapon 的子物体。
```

---

## 四、ArmAimController 教学：从小代码扩展到完整代码

本节目标：让学生理解“鼠标位置如何变成手臂角度”。

文件名：

```text
Assets/Scripts/ArmAimController.cs
```

创建脚本：

1. 在 Project 面板打开 `Assets/Scripts`。
2. 右键空白处。
3. 选择 `Create > Scripting > MonoBehaviour Script`，或 `Create > C# Script`。
4. 输入 `ArmAimController`。
5. 打开脚本。

注意：

```text
文件名必须是 ArmAimController
类名也必须是 ArmAimController
```

---

### 版本 1：先让 Arm 固定旋转

教学目的：

```text
证明我们控制的是 Arm 的 Rotation Z。
先不讲鼠标，不讲三角函数。
```

代码：

```csharp
using UnityEngine;

public class ArmAimController : MonoBehaviour
{
    public float testAngle = 30f;

    private void Update()
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, testAngle);
    }
}
```

操作：

1. 把脚本挂到 `Arm`。
2. 运行游戏。
3. 改 `Test Angle`。

观察：

```text
testAngle = 30  -> 手臂向上转
testAngle = -30 -> 手臂向下转
testAngle = 0   -> 手臂回到水平
```

讲解：

```text
transform 表示这个脚本挂在哪个物体上。
脚本挂在 Arm 上，所以 transform 就是 Arm 的 Transform。
Quaternion.Euler(0, 0, angle) 表示 2D 里绕 Z 轴旋转 angle 度。
```

课堂提问：

```text
如果这个脚本挂到 Armweapon 上，会发生什么？
```

目标回答：

```text
会直接旋转武器图片，武器绕自己中心转，不是我们想要的。
```

---

### 版本 2：把要旋转的对象写成 public 引用

教学目的：

```text
让学生理解 public 变量会显示在 Inspector 里，可以拖引用。
```

代码：

```csharp
using UnityEngine;

public class ArmAimController : MonoBehaviour
{
    public Transform armPivot;
    public float testAngle = 30f;

    private void Update()
    {
        armPivot.localRotation = Quaternion.Euler(0f, 0f, testAngle);
    }
}
```

操作：

1. 选中 `Arm`。
2. 在 `Arm Aim Controller` 组件里找到 `Arm Pivot`。
3. 把 `Arm` 自己拖进去。
4. 运行游戏。

常见错误：

```text
没有拖 Arm Pivot。
```

结果：

```text
Console 可能出现 NullReferenceException。
```

教师解释：

```text
null 表示没有东西。
脚本想旋转 armPivot，但 armPivot 没有被拖引用，所以会报错。
```

---

### 版本 3：加防错，没拖引用也不报错

教学目的：

```text
让学生知道代码可以保护自己，避免空引用。
```

代码：

```csharp
using UnityEngine;

public class ArmAimController : MonoBehaviour
{
    public Transform armPivot;
    public float testAngle = 30f;

    private void Awake()
    {
        if (armPivot == null)
            armPivot = transform;
    }

    private void Update()
    {
        armPivot.localRotation = Quaternion.Euler(0f, 0f, testAngle);
    }
}
```

讲解：

```text
Awake 在游戏开始时执行一次。
如果 armPivot 没拖，就自动使用脚本所在物体。
```

课堂提问：

```text
这个脚本挂在 Arm 上时，transform 是谁？
```

目标回答：

```text
transform 是 Arm。
```

---

### 版本 4：读取鼠标屏幕坐标

教学目的：

```text
先让学生知道鼠标有坐标。
还不急着旋转。
```

代码：

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmAimController : MonoBehaviour
{
    private void Update()
    {
        Vector3 mouseScreen = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : Input.mousePosition;

        Debug.Log(mouseScreen);
    }
}
```

运行后打开 Console，移动鼠标。

讲解：

```text
mouseScreen.x 是鼠标在屏幕上的横向位置。
mouseScreen.y 是鼠标在屏幕上的纵向位置。
这是屏幕坐标，还不是 Unity 场景里的世界坐标。
```

如果学生的工程没有启用 Input System，`using UnityEngine.InputSystem;` 可能报错。本工程使用新输入系统，所以保留这句。

---

### 版本 5：把鼠标屏幕坐标变成世界坐标

教学目的：

```text
让学生知道屏幕坐标不能直接拿来给场景物体用。
必须通过摄像机转换。
```

代码：

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmAimController : MonoBehaviour
{
    public Transform armPivot;
    public Camera aimCamera;

    private void Awake()
    {
        if (armPivot == null)
            armPivot = transform;
    }

    private void Update()
    {
        Camera cam = aimCamera != null ? aimCamera : Camera.main;
        if (cam == null)
            return;

        Vector3 mouseScreen = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : Input.mousePosition;

        mouseScreen.z = cam.WorldToScreenPoint(armPivot.position).z;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

        Debug.DrawLine(armPivot.position, mouseWorld, Color.red);
    }
}
```

操作：

1. 运行游戏。
2. 打开 Scene 视图。
3. 移动鼠标。

观察：

```text
Scene 里会出现从 Arm 指向鼠标世界位置的红线。
```

讲解：

```text
ScreenToWorldPoint 的作用：
把鼠标在屏幕上的点，换算成 Unity 世界里的点。
```

---

### 版本 6：方向变角度，手臂跟随鼠标

教学目的：

```text
实现最核心功能：鼠标在哪里，手臂大概指向哪里。
暂时不限制角度。
```

代码：

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class ArmAimController : MonoBehaviour
{
    public Transform armPivot;
    public Camera aimCamera;

    private void Awake()
    {
        if (armPivot == null)
            armPivot = transform;
    }

    private void Update()
     
        Camera cam = aimCamera != null ? aimCamera : Camera.main;
        if (cam == null)
            return;

        Vector3 mouseScreen = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : Input.mousePosition;

        mouseScreen.z = cam.WorldToScreenPoint(armPivot.position).z;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

        Vector2 direction = mouseWorld - armPivot.position;
        if (direction.sqrMagnitude < 0.0001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        armPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
```

讲解：

```text
direction = mouseWorld - armPivot.position
意思是：从手臂指向鼠标的方向。

Mathf.Atan2(direction.y, direction.x)
意思是：把方向换成角度。

Mathf.Rad2Deg
意思是：把弧度换成角度。
```

课堂只需要学生记住：

```text
方向 -> Atan2 -> 角度
```

---

### 版本 7：增加角度限制

教学目的：

```text
解决手臂乱转的问题。
```

在版本 6 基础上增加：

```csharp
public float minAngle = -55f;
public float maxAngle = 55f;
public float angleOffset = 0f;
```

把计算角度部分改成：

```csharp
float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset;
angle = Mathf.Clamp(angle, minAngle, maxAngle);
armPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
```

讲解：

```text
Clamp 像夹子。
比 minAngle 小，就变成 minAngle。
比 maxAngle 大，就变成 maxAngle。
在范围内，就保持原值。
```

例子：

```text
minAngle = -55
maxAngle = 55

angle = 80   -> 55
angle = -90  -> -55
angle = 20   -> 20
```

课堂练习：

```text
Min Angle = -20，Max Angle = 20，观察手臂范围。
Min Angle = -80，Max Angle = 80，观察手臂范围。
```

---

### 版本 8：处理角色左右翻面，整理成最终 ArmAimController

教学目的：

```text
让脚本适合当前横版角色。
角色会通过 player.localScale.x 左右翻面，所以角度计算放到 player 本地坐标里更稳定。
同时把瞄准逻辑整理成 RefreshAim，给 ShootController 开枪前调用。
```

最终代码：

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class ArmAimController : MonoBehaviour
{
    [Header("References")]
    public Transform armPivot;
    public Transform playerRoot;
    public Camera aimCamera;

    [Header("Aim")]
    public float minAngle = -55f;
    public float maxAngle = 55f;
    public float angleOffset = 0f;

    private void Reset()
    {
        armPivot = transform;
        playerRoot = transform.root;
    }

    private void Awake()
    {
        if (armPivot == null)
            armPivot = transform;

        if (playerRoot == null)
            playerRoot = transform.root;
    }

    private void LateUpdate()
    {
        RefreshAim();
    }

    public void RefreshAim()
    {
        Camera cam = aimCamera != null ? aimCamera : Camera.main;
        if (cam == null || armPivot == null || playerRoot == null)
            return;

        Vector3 mouseScreen = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : Input.mousePosition;

        mouseScreen.z = cam.WorldToScreenPoint(armPivot.position).z;
        Vector3 mouseWorld = cam.ScreenToWorldPoint(mouseScreen);

        Vector2 pivotLocal = playerRoot.InverseTransformPoint(armPivot.position);
        Vector2 mouseLocal = playerRoot.InverseTransformPoint(mouseWorld);
        Vector2 direction = mouseLocal - pivotLocal;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + angleOffset;
        angle = Mathf.Clamp(angle, minAngle, maxAngle);

        armPivot.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
```

挂载方式：

```text
脚本挂到 Arm
Arm Pivot   -> Arm
Player Root -> player
Aim Camera  -> None
Min Angle   -> -55
Max Angle   -> 55
Angle Offset -> 0
```

---

## 五、ShootController 教学：从小代码扩展到完整代码

本节目标：让学生理解“点击鼠标后，怎样从枪口生成子弹，并让子弹沿枪口方向飞”。

文件名：

```text
Assets/Scripts/ShootController.cs
```

创建脚本：

1. 在 Project 面板打开 `Assets/Scripts`。
2. 右键空白处。
3. 创建脚本 `ShootController`。
4. 打开脚本。

注意：

```text
文件名必须是 ShootController
类名也必须是 ShootController
```

---

### 版本 1：只检测鼠标点击

教学目的：

```text
让学生理解 Update 每帧执行，鼠标点击触发开枪逻辑。
```

代码：

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootController : MonoBehaviour
{
    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("开枪");
        }
    }
}
```

操作：

1. 把脚本挂到 `player`。
2. 运行游戏。
3. 点击鼠标左键。
4. 看 Console。

讲解：

```text
Update 每帧都会执行。
wasPressedThisFrame 表示这一帧刚刚按下。
```

---

### 版本 2：兼容旧输入系统

教学目的：

```text
当前工程使用新输入系统，但写一个兜底更稳。
```

代码：

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootController : MonoBehaviour
{
    private void Update()
    {
        if (!WasLeftClickPressedThisFrame())
            return;

        Debug.Log("开枪");
    }

    private static bool WasLeftClickPressedThisFrame()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;
        return Input.GetMouseButtonDown(0);
    }
}
```

讲解：

```text
return 表示提前退出。
如果没有点击，就不执行后面的开枪代码。
```

---

### 版本 3：点击后在 ShootPoint 位置创建一个空子弹

教学目的：

```text
先证明子弹会从枪口生成。
暂时不显示图片，不移动。
```

代码：

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootController : MonoBehaviour
{
    public Transform bulletSpawnPoint;

    private void Update()
    {
        if (!WasLeftClickPressedThisFrame())
            return;

        if (bulletSpawnPoint == null)
            return;

        GameObject bullet = new GameObject("Bullet");
        bullet.transform.position = bulletSpawnPoint.position;
    }

    private static bool WasLeftClickPressedThisFrame()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;
        return Input.GetMouseButtonDown(0);
    }
}
```

操作：

1. 选中 `player`。
2. 找到 `ShootController`。
3. 把 `ShootPoint` 拖到 `Bullet Spawn Point`。
4. 运行。
5. 点击鼠标。
6. 看 Hierarchy 是否出现 `Bullet`。

讲解：

```text
new GameObject("Bullet")
是在运行时创建一个新物体。

bullet.transform.position = bulletSpawnPoint.position
是把新物体放到枪口位置。
```

---

### 版本 4：让子弹显示图片

教学目的：

```text
理解 SpriteRenderer 是显示图片的组件。
```

代码：

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootController : MonoBehaviour
{
    public Sprite bulletSprite;
    public Transform bulletSpawnPoint;

    private void Update()
    {
        if (!WasLeftClickPressedThisFrame())
            return;

        if (bulletSpawnPoint == null || bulletSprite == null)
            return;

        GameObject bullet = new GameObject("Bullet");
        bullet.transform.position = bulletSpawnPoint.position;

        SpriteRenderer sr = bullet.AddComponent<SpriteRenderer>();
        sr.sprite = bulletSprite;
    }

    private static bool WasLeftClickPressedThisFrame()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;
        return Input.GetMouseButtonDown(0);
    }
}
```

操作：

```text
Bullet Sprite -> 拖一个子弹图片
Bullet Spawn Point -> 拖 ShootPoint
```

讲解：

```text
新建的 GameObject 默认看不见。
加 SpriteRenderer 并设置 sprite 后，才会显示图片。
```

---

### 版本 5：让子弹向右飞

教学目的：

```text
先让学生理解 Rigidbody2D 和速度。
```

在版本 4 基础上添加：

```csharp
public float bulletSpeed = 12f;
```

创建子弹后添加：

```csharp
Rigidbody2D rb = bullet.AddComponent<Rigidbody2D>();
rb.gravityScale = 0f;
rb.linearVelocity = Vector2.right * bulletSpeed;
```

讲解：

```text
Rigidbody2D 让子弹进入 2D 物理系统。
gravityScale = 0 表示子弹不受重力影响。
linearVelocity 是速度。
Vector2.right * bulletSpeed 表示向右飞。
```

课堂练习：

```text
把 bulletSpeed 改成 5。
把 bulletSpeed 改成 20。
观察子弹速度变化。
```

---

### 版本 6：让子弹朝鼠标方向飞

教学目的：

```text
先做一个容易理解的方向：枪口 -> 鼠标。
然后再说明它的问题。
```

核心代码：

```csharp
Vector2 direction = mouseWorld - bulletSpawnPoint.position;
Vector2 dir = direction.normalized;
rb.linearVelocity = dir * bulletSpeed;
```

教师讲解：

```text
鼠标位置 - 枪口位置 = 从枪口指向鼠标的方向。
normalized 表示只保留方向，不保留长度。
```

问题：

```text
如果手臂有角度限制，鼠标可以跑到限制范围外。
这时枪没指向鼠标，但子弹却飞向鼠标。
画面会不可信。
```

引出最终方案：

```text
子弹方向应该来自当前枪口方向，而不是直接来自鼠标方向。
```

---

### 版本 7：让子弹沿枪口方向飞

教学目的：

```text
解决“枪口方向”和“子弹方向”不一致的问题。
```

最终方向公式：

```text
发射方向 = ShootPoint 世界位置 - Arm 世界位置
```

代码概念：

```csharp
Vector2 spawnPosition = bulletSpawnPoint.position;
Vector2 direction = spawnPosition - (Vector2)aimPivot.position;
```

讲解：

```text
Arm 到 ShootPoint 的方向，就是当前枪口方向。
因为 ShootPoint 跟着枪转，所以这个方向会自动符合角度限制。
```

课堂对比：

```text
鼠标方向：ShootPoint -> Mouse
枪口方向：Arm -> ShootPoint
```

本课最终使用：

```text
Arm -> ShootPoint
```

---

### 版本 8：整理成完整 ShootController

最终代码：

```csharp
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class ShootController : MonoBehaviour
{
    public Sprite YellowBullet;
    public Sprite RedBullet;
    public Sprite BlueBullet;
    public Sprite GreenBullet;

    public Transform bulletSpawnPoint;
    public ArmAimController armAimController;
    public Transform aimPivot;

    [Header("瞄准 / 飞行")]
    [Tooltip("不指定则用 Camera.main")]
    public Camera shootCamera;

    [Tooltip("子弹速度（世界单位/秒）")]
    public float bulletSpeed = 12f;

    [Tooltip("几秒后销毁子弹，避免无限堆积")]
    public float bulletLifetime = 5f;

    [Tooltip("素材本身斜向时的 Z 角补正（度），叠在飞行方向上；例如 -37.45 让贴图与弹道对齐")]
    public float bulletVisualRotationOffsetZ = -37.45f;

    private GameObject SpawnBulletFromSprite(Sprite sprite, Vector2 direction, Vector2 spawnPosition)
    {
        if (sprite == null)
            return null;

        Vector2 dir = direction.sqrMagnitude > 1e-6f ? direction.normalized : Vector2.right;

        var go = new GameObject(sprite.name + "_Bullet");
        go.transform.position = spawnPosition;
        float flyAngleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.Euler(0f, 0f, flyAngleDeg + bulletVisualRotationOffsetZ);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.linearVelocity = dir * bulletSpeed;

        Destroy(go, bulletLifetime);
        return go;
    }

    public void Shoot(string bulletColor)
    {
        if (bulletSpawnPoint == null)
            return;

        Sprite s = bulletColor switch
        {
            "Yellow" => YellowBullet,
            "Red" => RedBullet,
            "Blue" => BlueBullet,
            "Green" => GreenBullet,
            _ => null
        };

        if (s == null)
            return;

        ArmAimController aim = ResolveArmAimController();
        if (aim != null)
            aim.RefreshAim();

        Vector2 dir = GetShotDirection();
        SpawnBulletFromSprite(s, dir, bulletSpawnPoint.position);
    }

    private Vector2 GetShotDirection()
    {
        Vector2 spawnPosition = bulletSpawnPoint.position;
        Transform pivot = GetAimPivot();

        if (pivot != null)
        {
            Vector2 pivotToMuzzle = spawnPosition - (Vector2)pivot.position;
            if (pivotToMuzzle.sqrMagnitude > 1e-6f)
                return pivotToMuzzle;
        }

        if (bulletSpawnPoint.parent != null)
        {
            Vector2 parentToMuzzle = spawnPosition - (Vector2)bulletSpawnPoint.parent.position;
            if (parentToMuzzle.sqrMagnitude > 1e-6f)
                return parentToMuzzle;
        }

        Vector2 localRight = bulletSpawnPoint.TransformDirection(Vector3.right);
        if (localRight.sqrMagnitude > 1e-6f)
            return localRight;

        return GetMouseWorldOnPlane(bulletSpawnPoint) - spawnPosition;
    }

    private Transform GetAimPivot()
    {
        if (aimPivot != null)
            return aimPivot;

        ArmAimController aim = ResolveArmAimController();
        if (aim == null)
            return null;

        return aim.armPivot != null ? aim.armPivot : aim.transform;
    }

    private ArmAimController ResolveArmAimController()
    {
        if (armAimController != null)
            return armAimController;

        if (bulletSpawnPoint == null)
            return null;

        for (Transform current = bulletSpawnPoint; current != null; current = current.parent)
        {
            if (current.TryGetComponent(out ArmAimController aim))
            {
                armAimController = aim;
                return armAimController;
            }
        }

        return null;
    }

    private Vector2 GetMouseWorldOnPlane(Transform depthReference)
    {
        Camera cam = shootCamera != null ? shootCamera : Camera.main;
        if (cam == null || depthReference == null)
            return depthReference != null ? depthReference.position : Vector2.zero;

        Vector3 screen = ReadMouseScreenPosition();
        screen.z = cam.WorldToScreenPoint(depthReference.position).z;
        return cam.ScreenToWorldPoint(screen);
    }

    private static Vector3 ReadMouseScreenPosition()
    {
        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();
        return Input.mousePosition;
    }

    private void Update()
    {
        if (!WasLeftClickPressedThisFrame())
            return;

        Shoot("Yellow");
    }

    private static bool WasLeftClickPressedThisFrame()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;
        return Input.GetMouseButtonDown(0);
    }
}
```

挂载方式：

```text
脚本挂到 player

Yellow Bullet      -> 黄色子弹 Sprite
Red Bullet         -> 红色子弹 Sprite
Blue Bullet        -> 蓝色子弹 Sprite
Green Bullet       -> 绿色子弹 Sprite
Bullet Spawn Point -> ShootPoint
Arm Aim Controller -> Arm 上的 ArmAimController
Aim Pivot          -> Arm
Shoot Camera       -> None
Bullet Speed       -> 12
Bullet Lifetime    -> 5
Bullet Visual Rotation Offset Z -> -37.45
```

最低要求：

```text
Yellow Bullet 必须拖。
因为当前 Update 调用的是 Shoot("Yellow")。
```

---

## 六、完整学生实操流程

这一节可以作为教师投屏带做清单。

### 阶段 1：搭建层级

```text
1. 右键 player，Create Empty，命名 Arm。
2. 把 Arm 放到手臂根部。
3. 右键 Arm，Create Empty，命名 Armweapon。
4. 给 Armweapon 添加 SpriteRenderer。
5. 把 handwear 武器图片拖到 Sprite。
6. 调整 Armweapon 的位置和大小，让它在角色手上。
7. 右键 Armweapon，Create Empty，命名 ShootPoint。
8. 把 ShootPoint 移动到枪口。
```

检查：

```text
player
└── Arm
    └── Armweapon
        └── ShootPoint
```

### 阶段 2：创建并挂 ArmAimController

```text
1. 创建 ArmAimController.cs。
2. 先写版本 1，测试 Arm 能不能固定旋转。
3. 再逐步写到最终版本。
4. 把脚本挂到 Arm。
5. 拖引用。
```

最终引用：

```text
Arm Pivot   -> Arm
Player Root -> player
Aim Camera  -> None
Min Angle   -> -55
Max Angle   -> 55
Angle Offset -> 0
```

### 阶段 3：创建并挂 ShootController

```text
1. 创建 ShootController.cs。
2. 先写版本 1，只检测点击。
3. 再写创建空子弹。
4. 再加 SpriteRenderer。
5. 再加 Rigidbody2D。
6. 再改成按枪口方向飞。
7. 最后替换成完整版本。
8. 把脚本挂到 player。
9. 拖引用。
```

最终引用：

```text
Bullet Spawn Point -> ShootPoint
Arm Aim Controller -> Arm 上的 ArmAimController
Aim Pivot          -> Arm
Yellow Bullet      -> 黄色子弹 Sprite
Shoot Camera       -> None
```

### 阶段 4：运行检查

点击 Play。

检查手臂：

```text
鼠标上移，手臂上抬。
鼠标下移，手臂下压。
鼠标移得很高，手臂不会无限旋转。
```

检查子弹：

```text
点击左键，有子弹生成。
子弹从 ShootPoint 生成。
子弹沿枪口方向飞。
```

最后保存：

```text
Windows: Ctrl + S
macOS: Command + S
```

---

## 七、教师讲解主线

### 开场 3 分钟

教师话术：

> 今天我们做一个横版射击游戏里很常见的系统。第一部分是抬枪瞄准，第二部分是从枪口发射子弹。我们不会一上来复制最终代码，而是让代码一步一步长大。每加一段代码，都解决一个具体问题。

### 讲层级 8 分钟

教师话术：

> `Arm` 是旋转点，像肩膀或手腕。`Armweapon` 是武器图片。`ShootPoint` 是枪口。旋转 `Arm` 时，武器和枪口都会跟着转，这就是父子物体的作用。

板书：

```text
父物体转，子物体跟着绕父物体转。

Arm 转
Armweapon 跟着转
ShootPoint 跟着转
```

### 讲抬枪 15 到 25 分钟

板书：

```text
鼠标屏幕坐标 -> 鼠标世界坐标
方向 = 鼠标位置 - 手臂位置
角度 = Atan2(方向.y, 方向.x)
角度 = Clamp(角度, 最小角度, 最大角度)
Arm.localRotation = 角度
```

### 讲发射 15 到 25 分钟

板书：

```text
点击鼠标
选择子弹图片
刷新手臂角度
方向 = ShootPoint - Arm
创建子弹
添加 SpriteRenderer
添加 Rigidbody2D
设置速度
几秒后销毁
```

---

## 八、关键代码解释词典

### public

```csharp
public Transform bulletSpawnPoint;
```

解释：

```text
public 变量会显示在 Inspector 里，可以把物体拖进去。
```

### null

```csharp
if (bulletSpawnPoint == null)
    return;
```

解释：

```text
null 表示没有东西。
如果没有枪口，就不继续执行。
```

### return

解释：

```text
从当前方法退出，后面的代码不执行。
```

### Vector2

解释：

```text
二维向量，有 x 和 y。
可以表示位置，也可以表示方向。
```

### normalized

```csharp
Vector2 dir = direction.normalized;
```

解释：

```text
只保留方向，不保留长度。
这样子弹速度不会因为距离远近而改变。
```

### Quaternion.Euler

```csharp
Quaternion.Euler(0f, 0f, angle)
```

解释：

```text
把普通角度转换成 Unity Transform 可以使用的旋转。
2D 里主要改 Z。
```

### AddComponent

```csharp
go.AddComponent<Rigidbody2D>();
```

解释：

```text
运行时给物体添加组件。
```

### Destroy

```csharp
Destroy(go, bulletLifetime);
```

解释：

```text
过 bulletLifetime 秒后删除这个物体。
防止子弹无限堆积。
```

---

## 九、常见错误排查

### 1. 手臂完全不动

检查：

```text
Arm 上有没有 ArmAimController？
Arm Pivot 有没有拖 Arm？
Player Root 有没有拖 player？
场景里有没有 Main Camera？
鼠标是不是在 Game 视图里？
Console 有没有红色报错？
```

### 2. 枪绕自己中心转

原因：

```text
脚本挂到了 Armweapon，或者旋转的是 Armweapon。
```

正确做法：

```text
脚本挂 Arm。
旋转 Arm。
Armweapon 只是 Arm 的子物体。
```

### 3. 子弹没有生成

检查：

```text
ShootController 有没有挂到 player？
Yellow Bullet 有没有拖 Sprite？
Bullet Spawn Point 有没有拖 ShootPoint？
有没有点击 Game 视图？
Console 有没有红色报错？
```

### 4. 子弹从奇怪位置出来

原因：

```text
ShootPoint 位置不对，或 Bullet Spawn Point 拖错。
```

解决：

```text
把 ShootPoint 移到枪口。
把 Bullet Spawn Point 重新拖成 ShootPoint。
```

### 5. 子弹方向和枪口不一致

检查：

```text
Aim Pivot 是否是 Arm？
Arm Aim Controller 是否引用了 Arm 上的脚本？
ShootPoint 是否在 Armweapon 下面？
GetShotDirection 是否使用 Arm -> ShootPoint？
```

### 6. 子弹飞行方向对，但图片歪

原因：

```text
子弹图片素材本身不是朝右的。
```

解决：

```text
调整 Bullet Visual Rotation Offset Z。
当前工程可先用 -37.45。
```

教师强调：

```text
飞行方向错，改方向逻辑。
图片朝向歪，改视觉偏移。
不要混在一起改。
```

### 7. Play 模式里改完，退出后没了

原因：

```text
Unity 的 Play Mode 改动通常不会保存到场景。
```

解决：

```text
退出 Play Mode 后再正式调整参数。
调整好后 Ctrl + S 或 Command + S 保存。
```

---

## 十、课堂练习

### 基础练习

1. 把 `Min Angle` 改成 `-20`，观察手臂最低角度。
2. 把 `Max Angle` 改成 `80`，观察手臂最高角度。
3. 把 `Bullet Speed` 改成 `5` 和 `20`，观察速度变化。
4. 把 `ShootPoint` 移到枪口最前端。

### 理解练习

回答：

```text
1. 为什么脚本要旋转 Arm，而不是旋转 Armweapon？
2. 为什么 ShootPoint 要放在 Armweapon 下面？
3. 为什么最终子弹方向要用 Arm -> ShootPoint？
4. Yellow Bullet 没拖时，为什么点击没有子弹？
```

### 进阶练习

1. 把默认发射从 `Shoot("Yellow")` 改成 `Shoot("Red")`。
2. 加键盘数字键，按 `1` 发黄子弹，按 `2` 发红子弹。
3. 给子弹添加 `CircleCollider2D`，为碰撞做准备。
4. 加开火冷却，让玩家不能无限快速连发。

---

## 十一、评价标准

### C 等级

学生能完成：

```text
创建 Arm、Armweapon、ShootPoint
挂上两个脚本
拖好基本引用
运行后能看到手臂转、子弹发射
```

### B 等级

学生能解释：

```text
Arm 是旋转点
Armweapon 是显示图片
ShootPoint 是枪口
子弹方向来自 Arm -> ShootPoint
```

### A 等级

学生能独立说清楚：

```text
鼠标坐标如何转换为世界坐标
方向如何通过 Atan2 变成角度
Clamp 如何限制角度
Rigidbody2D.linearVelocity 如何让子弹移动
为什么开枪前要调用 RefreshAim
```

---

## 十二、教师收尾总结

可直接照读：

> 今天我们完成了一个完整的小系统。`ArmAimController` 负责瞄准：它读取鼠标，把鼠标位置转换成世界坐标，算出手臂到鼠标的方向，把方向变成角度，再限制角度，最后旋转 `Arm`。`ShootController` 负责发射：它检测鼠标点击，选择子弹图片，开枪前刷新手臂角度，然后用 `Arm -> ShootPoint` 的方向生成子弹并设置速度。这个案例最重要的不是背代码，而是理解结构：`Arm` 是转轴，`Armweapon` 是图片，`ShootPoint` 是枪口。结构搭对了，代码才会清楚。

---

## 十三、后续扩展方向

本课完成后，可以继续扩展：

1. 子弹碰到敌人后销毁。
2. 给子弹添加伤害。
3. 不同颜色子弹有不同速度和伤害。
4. 加枪口火光。
5. 加射击音效。
6. 加弹药数量。
7. 加换弹。
8. 加敌人受击动画。
9. 加开火冷却。
10. 加 UI 显示当前子弹类型。

