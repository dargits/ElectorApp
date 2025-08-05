Kế Hoạch Phát Triển Hệ Thống Bình Chọn và Bỏ PhiếuĐây là bản phác thảo chi tiết về cấu trúc ứng dụng, thiết kế cơ sở dữ liệu và quản lý phiên dựa trên các ý tưởng của bạn.1. Cấu Trúc Ứng DụngỨng dụng sẽ có các form chính để xử lý các chức năng khác nhau:Form Đăng nhập (Login Form): Giao diện khởi đầu để xác thực người dùng bằng tên tài khoản và mật khẩu. Sau khi đăng nhập, form này sẽ ẩn đi.Form Chính (Main Dashboard): Trạm điều khiển trung tâm, hiển thị các chức năng khác nhau tùy thuộc vào vai trò của người dùng (Admin hoặc User). Các chức năng có thể được tổ chức trong các tab hoặc panel.Quản lý Bình chọn (Admin): Cho phép người quản trị tạo, chỉnh sửa và xóa các cuộc bình chọn.Bỏ phiếu (User): Hiển thị danh sách các cuộc bình chọn đang mở để người dùng có thể tham gia.Xem kết quả (All): Hiển thị kết quả của các cuộc bình chọn đã kết thúc hoặc đang diễn ra.Form Tạo Bình chọn (Create Poll Form): Giao diện dành cho Admin để nhập tiêu đề, mô tả và các lựa chọn cho một cuộc bình chọn mới.Form Chi tiết & Bỏ phiếu (Poll Details Form): Hiển thị nội dung chi tiết của một cuộc bình chọn và cho phép người dùng thực hiện bỏ phiếu.2. Thiết Kế Cơ Sở Dữ liệu (SQL)Các bảng sau sẽ được sử dụng để lưu trữ dữ liệu một cách có tổ chức:Users (Lưu thông tin người dùng)CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username VARCHAR(50) UNIQUE,
    PasswordHash VARCHAR(256), -- Mã hóa mật khẩu
    UserRole VARCHAR(10) CHECK (UserRole IN ('Admin', 'User'))
);
Polls (Lưu thông tin các cuộc bình chọn)CREATE TABLE Polls (
    PollID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(200),
    Description NVARCHAR(MAX),
    PollType VARCHAR(20), -- 'single_choice' hoặc 'multiple_choice'
    Status VARCHAR(10) CHECK (Status IN ('open', 'closed')),
    CreatorID INT FOREIGN KEY REFERENCES Users(UserID),
    CreatedAt DATETIME
);
Options (Lưu các lựa chọn của mỗi cuộc bình chọn)CREATE TABLE Options (
    OptionID INT PRIMARY KEY IDENTITY(1,1),
    PollID INT FOREIGN KEY REFERENCES Polls(PollID),
    OptionText NVARCHAR(200)
);
Votes (Lưu các lượt bỏ phiếu của người dùng)CREATE TABLE Votes (
    VoteID INT PRIMARY KEY IDENTITY(1,1),
    PollID INT FOREIGN KEY REFERENCES Polls(PollID),
    UserID INT FOREIGN KEY REFERENCES Users(UserID),
    OptionID INT FOREIGN KEY REFERENCES Options(OptionID),
    VotedAt DATETIME
);
3. Quản lý Phiên và Phân QuyềnLớp tĩnh SessionManager là một giải pháp hiệu quả để quản lý phiên người dùng, cho phép truy cập thông tin từ bất cứ đâu trong ứng dụng.public static class SessionManager
{
    // Lưu ID của người dùng đang đăng nhập
    public static int CurrentUserID { get; private set; }

    // Lưu tên người dùng
    public static string CurrentUsername { get; private set; }

    // Lưu vai trò của người dùng
    public static string CurrentUserRole { get; private set; }

    // Kiểm tra xem người dùng đã đăng nhập chưa
    public static bool IsLoggedIn => CurrentUserID > 0;

    // Phương thức để thiết lập phiên đăng nhập
    public static void Login(int userID, string username, string userRole)
    {
        CurrentUserID = userID;
        CurrentUsername = username;
        CurrentUserRole = userRole;
    }

    // Phương thức để kết thúc phiên đăng nhập
    public static void Logout()
    {
        CurrentUserID = 0;
        CurrentUsername = null;
        CurrentUserRole = null;
    }
}
4. Mở Rộng Ý Tưởng về Các Loại Bình ChọnCột PollType trong bảng Polls cho phép bạn hỗ trợ nhiều loại bình chọn:single_choice (Bình chọn đơn): Người dùng chỉ có thể bỏ phiếu cho một lựa chọn duy nhất. Giao diện có thể sử dụng RadioButton.multiple_choice (Bình chọn đa): Người dùng có thể bỏ phiếu cho nhiều lựa chọn. Giao diện có thể sử dụng CheckBox.5. Cấu Trúc Thư Mục Dự ÁnViệc tổ chức code theo cấu trúc sau sẽ giúp quản lý dự án dễ dàng hơn:VotingApp/
├── Forms/                 // Chứa các Form (giao diện người dùng)
│   ├── MainForm.cs
│   ├── LoginForm.cs
│   ├── CreatePollForm.cs
│   └── PollDetailsForm.cs
├── Models/                // Chứa các lớp mô hình dữ liệu (ánh xạ với các bảng)
│   ├── User.cs
│   ├── Poll.cs
│   ├── Option.cs
│   └── Vote.cs
├── Services/              // Chứa các lớp xử lý nghiệp vụ và tương tác CSDL
│   ├── DatabaseService.cs
│   └── PasswordHasher.cs
├── Utils/                 // Chứa các lớp tiện ích, ví dụ như SessionManager
│   └── SessionManager.cs
├── Resources/             // Chứa tài nguyên tĩnh như biểu tượng, hình ảnh
│   └── icons/
└── Program.cs             // Điểm khởi chạy của ứng dụng

