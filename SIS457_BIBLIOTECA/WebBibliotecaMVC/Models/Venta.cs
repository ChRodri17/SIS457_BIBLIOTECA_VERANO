using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBibliotecaMVC.Models;

public partial class Venta
{
    public int Id { get; set; }

    public int IdUsuario { get; set; }

    public int IdCliente { get; set; }

    public decimal TotalVenta { get; set; }

    [Display(Name = "Fecha de Venta")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

    public DateTime FechaVenta { get; set; }

    public string UsuarioRegistro { get; set; } = null!;

    public DateTime FechaRegistro { get; set; }

    public short Estado { get; set; }

    public virtual Cliente IdClienteNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
}
