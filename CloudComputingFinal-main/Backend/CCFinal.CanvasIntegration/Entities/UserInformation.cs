using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace CCFinal.CanvasIntegration.Entities;

[PrimaryKey(nameof(UserID))]
public class UserInformation {
    public string Token { get; set; }

    [Key]
    public DateTime LastCanvasUpdateDateTime { get; set; }

    public DateTime LastRunDateTime { get; set; }
    public Guid UserID { get; set; }
    public string BaseUrl { get; set; }

    [Key]
    public DateTime? LockTime { get; set; }

    public Guid? LockId { get; set; }
}
