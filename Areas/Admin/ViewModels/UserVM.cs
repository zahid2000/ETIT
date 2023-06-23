using System.ComponentModel.DataAnnotations;

namespace ETIT.Areas.Admin.ViewModels;

public class UserVM
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string Phone { get; set; }
    public IList<string> Roles { get; set; }
}
