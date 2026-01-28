using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProForm.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        [Display(Name = "Клиент")]
        public int ClientId { get; set; }

        [Display(Name = "Абонемент")]
        public int? MembershipId { get; set; }

        [Required(ErrorMessage = "Сумма обязательна для заполнения")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Сумма должна быть больше 0")]
        [Display(Name = "Сумма")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Способ оплаты обязателен")]
        [Display(Name = "Способ оплаты")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [Display(Name = "Дата оплаты")]
        [DataType(DataType.DateTime)]
        public DateTime PaymentDate { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

        [StringLength(500)]
        [Display(Name = "Примечание")]
        public string? Notes { get; set; }

        // Навигационные свойства
        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; } = null!;

        [ForeignKey("MembershipId")]
        public virtual Membership? Membership { get; set; }
    }

    public enum PaymentMethod
    {
        [Display(Name = "Наличные")]
        Cash,
        [Display(Name = "Карта")]
        Card,
        [Display(Name = "СБП")]
        SBP
    }
}

