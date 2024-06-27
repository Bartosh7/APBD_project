using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APBDprojekt.Models;

[Table("ClientPerson")]
public class ClientPerson : Client
{
    [Column("name")]
    [MaxLength(50)]
    public string Name { get; set; }
        
    [Column("surname")]
    [MaxLength(50)]
    public string Surname { get; set; }
        
    [Column("peselNumber")]
    [MaxLength(11)]
    public string PeselNumber { get; set; }
    
    [Column("isDeleted")]
    public bool IsDeleted { get; set; }
}