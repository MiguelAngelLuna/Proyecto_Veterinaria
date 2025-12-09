namespace VeterinariaAPI.Models.Usuario.Cliente;

public class CitaCliente
{
    public long ide_cit { get; set; }
    public DateTime cal_cit { get; set; }
    public int con_cit { get; set; }
    public string veterinario { get; set; } 
    public string especialidad { get; set; }
    public string mascota { get; set; }    
    public string especie { get; set; }   
    public decimal mon_pag { get; set; }
}