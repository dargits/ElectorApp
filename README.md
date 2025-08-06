# 🗳️ Kế Hoạch Phát Triển Hệ Thống Bình Chọn và Bỏ Phiếu

## 📋 Thông Tin Dự Án

| Thông tin | Chi tiết |
|-----------|----------|
| **💻 Môn học** | Công nghệ .NET |
| **👥 Nhóm phát triển** | Bàn Hữu Sự, Hoàng Quý Ngọc, Ngô Văn Sâm |
| **📅 Ngày tạo** | August 2025 |
| **🎯 Mục tiêu** | Phát triển ứng dụng bình chọn và bỏ phiếu |

---

## 📝 Tổng Quan

Đây là bản phác thảo chi tiết về cấu trúc ứng dụng, thiết kế cơ sở dữ liệu và quản lý phiên dựa trên các yêu cầu của dự án.

---

## 🏗️ 1. Cấu Trúc Ứng Dụng

Ứng dụng sẽ có các form chính để xử lý các chức năng khác nhau:

### 🔐 Form Đăng nhập (Login Form)
- **Mục đích**: Giao diện khởi đầu để xác thực người dùng
- **Chức năng**: Xác thực bằng tên tài khoản và mật khẩu
- **Hành vi**: Sau khi đăng nhập thành công, form này sẽ ẩn đi

### 🏠 Form Chính (Main Dashboard)
- **Mục đích**: Trạm điều khiển trung tâm
- **Chức năng**: Hiển thị các chức năng khác nhau tùy thuộc vào vai trò người dùng
- **Cấu trúc**: Các chức năng được tổ chức trong tabs hoặc panels

#### 🔧 Quản lý Bình chọn (Admin)
- Tạo cuộc bình chọn mới
- Chỉnh sửa cuộc bình chọn hiện có
- Xóa cuộc bình chọn

#### 🗳️ Bỏ phiếu (User)
- Hiển thị danh sách các cuộc bình chọn đang mở
- Cho phép người dùng tham gia bỏ phiếu

#### 📊 Xem kết quả (All Users)
- Hiển thị kết quả các cuộc bình chọn đã kết thúc
- Thống kê các cuộc bình chọn đang diễn ra

### ➕ Form Tạo Bình chọn (Create Poll Form)
- **Đối tượng**: Chỉ dành cho Admin
- **Chức năng**: Nhập tiêu đề, mô tả và các lựa chọn cho cuộc bình chọn mới

### 📋 Form Chi tiết & Bỏ phiếu (Poll Details Form)
- **Chức năng**: Hiển thị nội dung chi tiết của cuộc bình chọn
- **Tương tác**: Cho phép người dùng thực hiện bỏ phiếu

---

## 🗄️ 2. Thiết Kế Cơ Sở Dữ liệu (SQL)

### 👤 Bảng Users (Lưu thông tin người dùng)

```sql
CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(50) UNIQUE,
    PasswordHash VARCHAR(256), -- Mã hóa mật khẩu
    UserRole VARCHAR(10) CHECK (UserRole IN ('Admin', 'User'))
);
```

**Mô tả các trường:**
- `UserID`: Khóa chính, tự động tăng
- `Username`: Tên đăng nhập duy nhất
- `PasswordHash`: Mật khẩu đã được mã hóa
- `UserRole`: Vai trò người dùng (Admin hoặc User)

### 📊 Bảng Polls (Lưu thông tin các cuộc bình chọn)

```sql
CREATE TABLE Polls (
    PollID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200),
    Description NVARCHAR(MAX),
    PollType VARCHAR(20), -- 'single_choice' hoặc 'multiple_choice'
    Status VARCHAR(10) CHECK (Status IN ('open', 'closed')),
    CreatorID INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedAt DATETIME
);
```

**Mô tả các trường:**
- `PollID`: Khóa chính của cuộc bình chọn
- `Title`: Tiêu đề cuộc bình chọn
- `Description`: Mô tả chi tiết
- `PollType`: Loại bình chọn (đơn hoặc đa lựa chọn)
- `Status`: Trạng thái (mở hoặc đóng)
- `CreatorID`: ID người tạo cuộc bình chọn
- `CreatedAt`: Thời gian tạo

### ⚙️ Bảng Options (Lưu các lựa chọn của mỗi cuộc bình chọn)

```sql
CREATE TABLE Options (
    OptionID INT PRIMARY KEY IDENTITY(1,1),
    PollID INT FOREIGN KEY REFERENCES Polls(PollID),
    OptionText NVARCHAR(200)
);
```

**Mô tả các trường:**
- `OptionID`: Khóa chính của lựa chọn
- `PollID`: Khóa ngoại tham chiếu đến bảng Polls
- `OptionText`: Nội dung của lựa chọn

### ✅ Bảng Votes (Lưu các lượt bỏ phiếu của người dùng)

```sql
CREATE TABLE Votes (
    VoteID INT PRIMARY KEY IDENTITY(1,1),
    PollID INT FOREIGN KEY REFERENCES Polls(PollID),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    OptionID INT FOREIGN KEY REFERENCES Options(OptionID),
    VotedAt DATETIME
);
```

**Mô tả các trường:**
- `VoteID`: Khóa chính của lượt bỏ phiếu
- `PollID`: Cuộc bình chọn được tham gia
- `UserID`: Người dùng thực hiện bỏ phiếu
- `OptionID`: Lựa chọn được bỏ phiếu
- `VotedAt`: Thời gian bỏ phiếu

---

## 🔐 3. Quản lý Phiên và Phân Quyền

Lớp tĩnh `SessionManager` là một giải pháp hiệu quả để quản lý phiên người dùng, cho phép truy cập thông tin từ bất cứ đâu trong ứng dụng.

```csharp
public static class SessionManager
{
    // 🆔 Lưu ID của người dùng đang đăng nhập
    public static int CurrentUserID { get; private set; }

    // 👤 Lưu tên người dùng
    public static string CurrentUsername { get; private set; }

    // 🛡️ Lưu vai trò của người dùng
    public static string CurrentUserRole { get; private set; }

    // ✅ Kiểm tra xem người dùng đã đăng nhập chưa
    public static bool IsLoggedIn => CurrentUserID > 0;

    // 🚀 Phương thức để thiết lập phiên đăng nhập
    public static void Login(int userID, string username, string userRole)
    {
        CurrentUserID = userID;
        CurrentUsername = username;
        CurrentUserRole = userRole;
    }

    // 🚪 Phương thức để kết thúc phiên đăng nhập
    public static void Logout()
    {
        CurrentUserID = 0;
        CurrentUsername = null;
        CurrentUserRole = null;
    }
}
```

**Lợi ích:**
- ✨ Quản lý phiên đơn giản và hiệu quả
- 🔒 Bảo mật thông tin người dùng
- 🚀 Truy cập nhanh chóng từ mọi nơi trong ứng dụng

---

## 🎯 4. Mở Rộng Ý Tưởng về Các Loại Bình Chọn

Cột `PollType` trong bảng Polls cho phép hỗ trợ nhiều loại bình chọn:

### 🔘 Single Choice (Bình chọn đơn)
- **Đặc điểm**: Người dùng chỉ có thể bỏ phiếu cho một lựa chọn duy nhất
- **Giao diện**: Sử dụng `RadioButton`
- **Ứng dụng**: Bầu cử, chọn lựa duy nhất

### ☑️ Multiple Choice (Bình chọn đa)
- **Đặc điểm**: Người dùng có thể bỏ phiếu cho nhiều lựa chọn
- **Giao diện**: Sử dụng `CheckBox`
- **Ứng dụng**: Khảo sát ý kiến, đánh giá đa tiêu chí

---

## 📁 5. Cấu Trúc Thư Mục Dự Án

Việc tổ chức code theo cấu trúc sau sẽ giúp quản lý dự án dễ dàng hơn:

```
📦 VotingApp/
├── 🖼️ Forms/                 // Chứa các Form (giao diện người dùng)
│   ├── MainForm.cs
│   ├── LoginForm.cs
│   ├── CreatePollForm.cs
│   └── PollDetailsForm.cs
├── 📋 Models/                // Chứa các lớp mô hình dữ liệu
│   ├── User.cs
│   ├── Poll.cs
│   ├── Option.cs
│   └── Vote.cs
├── ⚙️ Services/              // Chứa các lớp xử lý nghiệp vụ
│   ├── DatabaseService.cs
│   └── PasswordHasher.cs
├── 🔧 Utils/                 // Chứa các lớp tiện ích
│   └── SessionManager.cs
├── 🎨 Resources/             // Chứa tài nguyên tĩnh
│   └── icons/
└── 🚀 Program.cs             // Điểm khởi chạy của ứng dụng
```

### 📂 Chi tiết các thư mục:

**🖼️ Forms/**: Chứa tất cả các giao diện người dùng
- Quản lý các tương tác với người dùng
- Xử lý sự kiện và hiển thị dữ liệu

**📋 Models/**: Các lớp mô hình dữ liệu
- Ánh xạ với các bảng trong cơ sở dữ liệu
- Định nghĩa cấu trúc dữ liệu

**⚙️ Services/**: Logic xử lý nghiệp vụ
- Tương tác với cơ sở dữ liệu
- Xử lý các tác vụ phức tạp

**🔧 Utils/**: Các tiện ích hỗ trợ
- Quản lý phiên đăng nhập
- Các hàm tiện ích chung

**🎨 Resources/**: Tài nguyên tĩnh
- Biểu tượng, hình ảnh
- Tệp cấu hình

---

## 🎉 Kết Luận

Kế hoạch này cung cấp một framework hoàn chỉnh để phát triển hệ thống bình chọn và bỏ phiếu với:

- ✅ Cấu trúc rõ ràng và có tổ chức
- 🔒 Bảo mật và quản lý phiên hiệu quả
- 📊 Thiết kế cơ sở dữ liệu tối ưu
- 🎯 Khả năng mở rộng cao
- 👥 Phân quyền người dùng rõ ràng

**Nhóm phát triển**: Bàn Hữu Sự, Hoàng Quý Ngọc, Ngô Văn Sâm sẽ triển khai dự án này trong môn Công nghệ .NET.
