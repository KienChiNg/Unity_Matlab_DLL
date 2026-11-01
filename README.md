# Unity × MATLAB — Mô phỏng cánh tay robot và điều chế BPSK

Hai bài mô phỏng trong Unity dùng **MATLAB Compiler (.NET DLL)** để tính toán. Unity lo hiển thị 3D, animation và tương tác; MATLAB lo phần toán.

| Bài | Unity | MATLAB (DLL) |
| --- | --- | --- |
| Cánh tay robot (`ROBOT_ARM`) | Mô hình 3D, chọn vị trí vật và đích bằng slider, animation gắp – thả | Động học thuận/nghịch (FK/IK), tính góc quay từng khớp theo quỹ đạo |
| Điều chế BPSK (`MATLAB_DLL`) | Vẽ dạng sóng và chòm sao tín hiệu bằng LineRenderer | Tổng hợp tín hiệu BPSK theo số bit N và bậc điều chế M |

## Demo

**Cánh tay robot**

https://github.com/user-attachments/assets/b6fd1f07-99a8-4b34-84b9-d46e900884bc

**Điều chế BPSK**

https://github.com/user-attachments/assets/32f8f4b7-bce6-4b13-9cad-7ac7ff2bbef7

## Cách hoạt động

### Cánh tay robot

```
Slider (vị trí vật, vị trí đích)
        │
        ▼
Unity ── toạ độ đầu / cuối ──▶ RobotArm.dll (MATLAB: IK)
        ◀── ma trận góc quay các khớp ──
        │
        ▼
Xoay lần lượt từng khớp → gắp vật → về vị trí chờ → đặt vật ở đích → về vị trí chờ
```

- `MatlabRobotArmManager` gọi `RobotArm.Class1.RobotArm(...)` qua `MWArray`, đổi kết quả sang góc quay rồi chạy coroutine xoay từng khớp. Khớp đang xoay được highlight.
- `RobotArmUIManager` giới hạn vùng toạ độ hợp lệ (tay robot không với tới quá gần gốc) và hiển thị góc các khớp sau mỗi lần tính.
- Có sẵn nhánh gọi MATLAB qua HTTP (`POST http://127.0.0.1:3000/matlab`) để thay cho DLL khi cần.

### Điều chế BPSK

- Nhập số bit **N** (1 < N < 1000) và chọn bậc **M** (2, 4, 8, 16, 32).
- `MATLABInterop` gọi `BPSKFuncionSynthesise.KienChiMATLAB.BPSKFuncionSynthesise(3, N, M)`; DLL trả về 2 dạng sóng và tập điểm của chòm sao.
- Unity vẽ 2 dạng sóng bằng `LineRenderer` và vẽ chòm sao bằng pool 1000 điểm dựng sẵn. Kết quả số được ghi ra file log.

## Yêu cầu

- Windows (DLL của MATLAB chỉ chạy trên Windows)
- **MATLAB Runtime R2021a (9.10)** hoặc MATLAB R2021a đã cài
- Unity **2022.3.38f1** nếu mở bằng Editor

## Chạy dự án

1. Cài MATLAB Runtime R2021a.
2. Mở thư mục bằng Unity 2022.3.38f1.
3. Mở scene `Assets/Scenes/ROBOT_ARM.unity` (cánh tay robot) hoặc `Assets/Scenes/MATLAB_DLL.unity` (BPSK) rồi bấm **Play**; hoặc build bản Windows (x86_64).

## Cấu trúc thư mục

```
Assets/
  Plugins/
    RobotArm.dll                 DLL MATLAB: động học cánh tay robot
    BPSKFuncionSynthesise.dll    DLL MATLAB: tổng hợp tín hiệu BPSK
    MWArray.dll                  Thư viện kiểu dữ liệu MATLAB cho .NET
  Scripts/
    Arm/      Điều khiển robot, gọi DLL, UI chọn toạ độ
    BPSK/     Gọi DLL, vẽ sóng và chòm sao
  Scenes/     ROBOT_ARM, MATLAB_DLL
  !Robots/    Mô hình robot, kho hàng
```
