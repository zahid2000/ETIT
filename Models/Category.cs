﻿using Microsoft.AspNetCore.Components.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace ETIT.Models;

public class Category
{
    public Category()
    {
        Products = new List<Product>();
    }
    public int Id { get; set; }
    [Required,MaxLength(100)]
    public string Name { get; set; }
    [DefaultValue(false)]
    public bool IsDeleted{ get; set; }
    public List<Product> Products { get; set; }
}
