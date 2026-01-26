using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProForm.Models
{
    public class MembershipType
    {
        [Key]
        public int MembershipTypeId { get; set; }

        [Required(ErrorMessage = "Название обязательно для заполнения")]
        [StringLength(255)]
        [Display(Name = "Название")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Стоимость обязательна для заполнения")]
        [Range(0, double.MaxValue, ErrorMessage = "Стоимость должна быть положительной")]
        [Display(Name = "Стоимость")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Display(Name = "Тип абонемента")]
        public MembershipCategory Category { get; set; }

        [Display(Name = "Срок действия (дней)")]
        public int? DurationDays { get; set; }

        [Display(Name = "Количество посещений")]
        public int? VisitCount { get; set; }

        [StringLength(1000)]
        [Display(Name = "Доступные услуги")]
        public string? AvailableServices { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;

        // Навигационные свойства
        public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    }

    public enum MembershipCategory
    {
        [Display(Name = "Срочный")]
        TimeBased,
        [Display(Name = "Пакетный")]
        VisitBased
    }
}

