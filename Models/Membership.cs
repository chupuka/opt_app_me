using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProForm.Models
{
    public class Membership
    {
        [Key]
        public int MembershipId { get; set; }

        [Required]
        [Display(Name = "Клиент")]
        public int ClientId { get; set; }

        [Required]
        [Display(Name = "Тип абонемента")]
        public int MembershipTypeId { get; set; }

        [Required]
        [Display(Name = "Дата начала")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Today;

        [Display(Name = "Дата окончания")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = "Оставшиеся посещения")]
        public int? RemainingVisits { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; } = true;

        // Навигационные свойства
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; } = null!;

        [ForeignKey("MembershipTypeId")]
        public virtual MembershipType MembershipType { get; set; } = null!;

        public virtual ICollection<MembershipFreeze> Freezes { get; set; } = new List<MembershipFreeze>();
    }

    public class MembershipFreeze
    {
        [Key]
        public int FreezeId { get; set; }

        [Required]
        public int MembershipId { get; set; }

        [Required]
        [Display(Name = "Дата начала заморозки")]
        [DataType(DataType.Date)]
        public DateTime FreezeStartDate { get; set; }

        [Required]
        [Display(Name = "Дата окончания заморозки")]
        [DataType(DataType.Date)]
        public DateTime FreezeEndDate { get; set; }

        [StringLength(500)]
        [Display(Name = "Причина")]
        public string? Reason { get; set; }

        [ForeignKey("MembershipId")]
        public virtual Membership Membership { get; set; } = null!;
    }
}

