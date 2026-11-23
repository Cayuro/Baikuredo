using System.ComponentModel.DataAnnotations;
using Baikuredo.Web.Data.Abstractions;

namespace Baikuredo.Web.Data.Entities
{
    // Moto: ficha técnica del vehículo. No cambia aunque cambie de dueño.
    public class Motorcycle : IId
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Plate { get; set; } = null!;

        public int Year { get; set; }

        // Cilindraje en cc
        public int Displacement { get; set; }

        [Required]
        [MaxLength(30)]
        public string Color { get; set; } = null!;

        public Guid MotorcycleModelId { get; set; }
        public MotorcycleModel? MotorcycleModel { get; set; }

        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }

        public Guid OwnerId { get; set; }
        public Owner? Owner { get; set; }

        // Usuario que registró la moto (F006). Es opcional porque las motos del seeder
        // se crean antes que los usuarios.
        [MaxLength(450)]
        public string? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<MotorcyclePublication>? Publications { get; set; }
    }
}
