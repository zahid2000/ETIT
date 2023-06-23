using AutoMapper;
using ETIT.Areas.Admin.ViewModels.Products;
using ETIT.DAL;
using ETIT.Models;
using ETIT.Utilities.Constants;
using ETIT.Utilities.Extensions;
using ETIT.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ETIT.Areas.Admin.Controllers;
[Area("admin")]
[Authorize(Roles = "Admin")]

public class ProductsController : Controller
{
    private readonly AppDbContext _context;
    private readonly List<Category> _categories;
    private readonly List<Company> _companies;
    private string? _errorMessages;
    private readonly IWebHostEnvironment _enviroment;
    private readonly IMapper _mapper;
    public ProductsController(AppDbContext context, IWebHostEnvironment enviroment, IMapper mapper)
    {
        _context = context;
        _categories = _context.Categories.ToList();
        _companies = _context.Companies.ToList();
        _enviroment = enviroment;
        _mapper = mapper;
    }
    public async Task<IActionResult> Index(int page = 1, int take = 10)
    {
        List<Product> products = await _context.Products
            .OrderByDescending(s => s.Id)
            .Skip((page - 1) * take)
            .Take(take)
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .ToListAsync();
        int pageCount = await GetPageCount(take);
        PaginateVM<Product> model = new PaginateVM<Product>
        {
            Data = products,
            CurrentPage = page,
            PageCount = pageCount,
            HasNext = page < pageCount,
            HasPreview = page > 1,
            Take = take
        };

        return View(model);
    }
    private async Task<int> GetPageCount(int take)
    {
        int serviceCount = await _context.Products.CountAsync();
        return (int)Math.Ceiling((double)serviceCount / take);
    }
    public IActionResult Create()
    {
        CreateProductVM createServiceVm = new CreateProductVM()
        {
            Categories = _categories,
            Companies = _companies
        };
        return View(createServiceVm);
    }
    //[Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Create(CreateProductVM productVM)
    {
        productVM.Categories = _categories;
        productVM.Companies = _companies;
        if (!ModelState.IsValid) return View(productVM);
        if (!CheckPhoto(productVM.Images)) ModelState.AddModelError("Photos", _errorMessages);
        string rootPath = Path.Combine(_enviroment.WebRootPath, "assets", "img");
        List<ProductImage> images = await CreateFileAndGetServiceImages(productVM.Images, rootPath);
        Product product = _mapper.Map<Product>(productVM);
        product.ProductImages = images;
        product.IsDeleted = false;
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
   
    public async Task<IActionResult> ChangeDeleteStatus(int id, bool status)
    {
        Product? product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        product.IsDeleted = status;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Delete(int id)
    {
        Product? product=await _context.Products.FindAsync(id);
        if (product == null) return NotFound();
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Update(int id)
    {
       Product? product=await _context.Products.Where(p=>p.Id==id).Include(p=>p.ProductImages).FirstOrDefaultAsync();
        if (product == null) return NotFound();
        UpdateProductVM updateProductVM = _mapper.Map<UpdateProductVM>(product);
        updateProductVM.Categories=_categories;
        updateProductVM.Companies=_companies;
        updateProductVM.OldProductImages=product.ProductImages;
        return View(updateProductVM);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]

    public async Task<IActionResult> Update(UpdateProductVM productVM)
    {
        productVM.Categories = _categories;
        productVM.Companies = _companies;
        Product? product = await _context.Products.Where(p=>p.Id==productVM.Id).Include(p=>p.ProductImages).FirstOrDefaultAsync();
        if (product == null) return NotFound();
        if (productVM.Images!=null)
            _context.ProductImages.RemoveRange(product.ProductImages.ToArray());
        else
            productVM.OldProductImages = product.ProductImages;
        if (!ModelState.IsValid) return View(productVM);
        if (!CheckPhoto(productVM.Images)) ModelState.AddModelError("Photos", _errorMessages);
        string rootPath = Path.Combine(_enviroment.WebRootPath, "assets", "img");
        DeleteFiles(rootPath, productVM.OldProductImages);
        List<ProductImage> images = await CreateFileAndGetServiceImages(productVM.Images, rootPath);
        product.Name = productVM.Name;
        product.Description = productVM.Description;
        product.Price= productVM.Price;
        product.CategoryId= productVM.CategoryId;
        product.CompanyId= productVM.CompanyId;
        product.Stock= productVM.Stock;       
        product.ProductImages = images;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    private void DeleteFiles(string root,List<ProductImage> images)
    {
        string filepath = root;
        foreach (var image in images)
        {
            filepath = Path.Combine(root,image.Path);
            if (System.IO.File.Exists(filepath))
                System.IO.File.Delete(filepath);
        }
    }
    private async Task<List<ProductImage>> CreateFileAndGetServiceImages(List<IFormFile> photos, string rootPath)
    {
        List<ProductImage> images = new List<ProductImage>();
        foreach (var photo in photos)
        {
            string fileName = await photo.SaveAsync(rootPath);
            ProductImage serviceImage = new ProductImage() { Path = fileName };
            if (!images.Any(i => i.IsMain))
            {
                serviceImage.IsMain = true;
            }
            images.Add(serviceImage);
        }

        return images;
    }

    private bool CheckPhoto(List<IFormFile> photos)
    {
        foreach (var photo in photos)
        {
            if (!photo.CheckContentType("image/"))
            {
                _errorMessages = $"{photo.FileName} - {Messages.FileTypeMustBeImage}";
                return false;
            }
            if (!photo.CheckFileSize(200))
            {
                _errorMessages = $"{photo.FileName} - {Messages.FileSizeMustBe200KB}";
                return false;
            }
        }
        return true;
    }
}
