using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProForm.Models
{
    public class FitnessClass
    {
        [Key]
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Тип занятия обязателен")]
        [Display(Name = "Тип занятия")]
        public ClassType Type { get; set; }

        [Required(ErrorMessage = "Название обязательно для заполнения")]
        [StringLength(255)]
        [Display(Name = "Название")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Тренер")]
        public int? TrainerId { get; set; }

        [StringLength(100)]
        [Display(Name = "Зал")]
        public string? Hall { get; set; }

        [Required(ErrorMessage = "Дата и время начала обязательны")]
        [Display(Name = "Начало")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Дата и время окончания обязательны")]
        [Display(Name = "Окончание")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        [Display(Name = "Максимальное количество участников")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество участников должно быть больше 0")]
        public int? MaxParticipants { get; set; }

        // Навигационные свойства
        [ForeignKey("TrainerId")]
        public virtual Trainer? Trainer { get; set; }

        public virtual ICollection<ClassRegistration> Registrations { get; set; } = new List<ClassRegistration>();
    }

    public enum ClassType
    {
        [Display(Name = "Групповое")]
        Group,
        [Display(Name = "Персональное")]
        Personal
    }

    public class ClassRegistration
    {
        [Key]
        public int RegistrationId { get; set; }

        [Required]
        public int ClassId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Display(Name = "Посетил")]
        public bool Attended { get; set; }

        [Display(Name = "Дата регистрации")]
        public DateTime RegistrationDate { get; set; } = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

        // Навигационные свойства
        [ForeignKey("ClassId")]
        public virtual FitnessClass Class { get; set; } = null!;

        [ForeignKey("ClientId")]
        public virtual Client Client { get; set; } = null!;
    }
}

