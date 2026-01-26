using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProForm.Models
{
    public class TrainerSchedule
    {
        [Key]
        public int ScheduleId { get; set; }

        [Required]
        [Display(Name = "Тренер")]
        public int TrainerId { get; set; }

        [Required]
        [Display(Name = "День недели")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [Display(Name = "Время начала")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "Время окончания")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        // Навигационные свойства
        [ForeignKey("TrainerId")]
        public virtual Trainer Trainer { get; set; } = null!;
    }
}

