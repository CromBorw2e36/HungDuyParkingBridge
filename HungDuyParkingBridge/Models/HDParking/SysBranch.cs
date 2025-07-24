using System;
using System.Collections.Generic;

namespace HungDuyParking.Models.HDParkingEntity;

public partial class SysBranch
{
    public Guid Ma { get; set; }

    public string? Code { get; set; }

    public string? MaSoThue { get; set; }

    public string? Ten { get; set; }

    public string? PhapNhan { get; set; }

    public string? DiaChi { get; set; }

    public string? Logo { get; set; }

    public bool? Status { get; set; }

    public bool? CoSoQuy { get; set; }

    public Guid? UserCreated { get; set; }

    public DateTime? CreatedDate { get; set; }

    public Guid? UserModified { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public bool? Deleted { get; set; }

    public Guid? UserDeleted { get; set; }

    public DateTime? DeletedDate { get; set; }

    public int? NId { get; set; }

    public int? SoGioTheoLuat { get; set; }
}
