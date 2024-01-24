using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBibliotecaMVC.Models;

public partial class Empleado
{
    public int IdEmpleado { get; set; }

    public string Nombre { get; set; } = null!;
    [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
    [RegularExpression("^[a-zA-Z ]+$", ErrorMessage = "Por favor, introduzca solo letras.")]

    public string Apellidos { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Cargo { get; set; } = null!;

    public double Salario { get; set; }

    public string UsuarioRegistro { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public short Estado { get; set; }

    public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
}
