using ETIT.Models;

namespace ETIT.Areas.Admin.ViewModels;

public class DashboardVM
{
    public int? UserCount { get; set; }
    public int ProductCount { get; set; }
    public int CompanyCount { get; set; }
    public int OrderCount { get; set; }
    public List<Order> Orders { get; set; }
}
