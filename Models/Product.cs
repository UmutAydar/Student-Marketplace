using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace UniMarket.Web.Models;

public class Product
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Başlık zorunludur")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Açıklama zorunludur")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Fiyat zorunludur")]
    [Range(1, 100000, ErrorMessage = "Fiyat 1 ile 100000 arasında olmalı")]
    public decimal Price { get; set; }

    [Display(Name = "Satıldı mı?")]
    public bool IsSold { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? ImagePath { get; set; } 

    public string? OwnerId { get; set; } 

    public IdentityUser? Owner { get; set; } 

    public bool IsApproved { get; set; } = false;

}
