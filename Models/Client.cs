using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProForm.Models
{
    public class Client
    {
        [Key]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "ФИО обязательно для заполнения")]
        [StringLength(255)]
        [RegularExpression(@"^[а-яА-ЯёЁ\s]+$", ErrorMessage = "ФИО должно содержать только буквы и пробелы")]
        [Display(Name = "ФИО")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Телефон обязателен для заполнения")]
        [StringLength(20)]
        [Display(Name = "Телефон")]
        [RegularExpression(@"^\+7\d{10}$", ErrorMessage = "Неверный формат телефона (+7XXXXXXXXXX)")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        public string? Email { get; set; }

        [Display(Name = "Статус")]
        public ClientStatus Status { get; set; } = ClientStatus.Potential;

        // Навигационные свойства
        public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
        public virtual ICollection<ClassRegistration> ClassRegistrations { get; set; } = new List<ClassRegistration>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    public enum ClientStatus
    {
        [Display(Name = "Активный")]
        Active,
        [Display(Name = "Неактивный")]
        Inactive,
        [Display(Name = "Потенциальный")]
        Potential
    }
}

