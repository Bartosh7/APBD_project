using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APBDprojekt.Models;

public class AppUserModel
{
    [Key]
    [Column("id")]
    [Required]
    public int IdUser { get; set; }
    
    [Column("email")]
    [Required]
    public string UserName { get; set; }
    [Column("password_hashed")]
    [Required]
    public string PasswordHashed { get; set; }
    [Column("salt")]
    [Required]
    public string Salt { get; set; }
    
    [Column("role")]
    [Required]
    public string Roles { get; set; }
    
    
}