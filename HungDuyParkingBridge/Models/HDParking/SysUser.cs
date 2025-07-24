using System;
using System.Collections.Generic;

namespace HungDuyParking.Models.HDParkingEntity;

public partial class SysUser
{
    public Guid Ma { get; set; }

    public Guid? MaNhanVien { get; set; }

    public Guid? BranchId { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? FullName { get; set; }

    public string? SoDienThoai { get; set; }

    public Guid? MaPhongBan { get; set; }

    public string? MaPhongBanKiemNhiem { get; set; }

    public string? HinhAnh { get; set; }

    public bool? EmailReceive { get; set; }

    public string? Fcm { get; set; }

    public string? Msnv { get; set; }

    public bool? Status { get; set; }

    public bool? GroupManager { get; set; }

    public bool? UserManager { get; set; }

    public bool? LdapLogin { get; set; }

    public string? Cchn { get; set; }

    public Guid? UserCreated { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? UserModified { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? Deleted { get; set; }

    public Guid? UserDeleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public bool? Reminder { get; set; }

    public string? ImgFaceDetection { get; set; }

    public Guid? MaBan { get; set; }

    public string? PushKitToken { get; set; }

    public string? Passcode { get; set; }
}
