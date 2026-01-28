using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProForm.Models
{
    public class Trainer
    {
        [Key]
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "ФИО обязательно для заполнения")]
        [StringLength(255)]
        [Display(Name = "ФИО")]
        public string FullName { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Специализация")]
        public string? Specialization { get; set; }

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
        public TrainerStatus Status { get; set; } = TrainerStatus.Active;

        // Навигационные свойства
        public virtual ICollection<FitnessClass> Classes { get; set; } = new List<FitnessClass>();
        public virtual ICollection<TrainerSchedule> Schedules { get; set; } = new List<TrainerSchedule>();
    }

    public enum TrainerStatus
    {
        [Display(Name = "Активен")]
        Active,
        [Display(Name = "Неактивен")]
        Inactive
    }
}

