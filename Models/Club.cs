using System.ComponentModel.DataAnnotations;

namespace UniMarket.Web.Models;

public class Club
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kulüp adı zorunludur")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Açıklama zorunludur")]
    public string Description { get; set; } = null!;

    // opsiyonel
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
