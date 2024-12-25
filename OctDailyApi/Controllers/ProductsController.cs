using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OctDailyApi.Models;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ProductContext _context;

    public ProductsController(ProductContext context)
    {
        _context = context;
    }

   
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string? search, [FromQuery] string sort = "name", [FromQuery] string sortDirection = "asc", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        // Filter products by search term
        var query = _context.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(p => p.Name.Contains(search) || p.Description.Contains(search));
        }

        // Sort products by specified column and direction
        query = (sort, sortDirection.ToLower()) switch
        {
            ("name", "asc") => query.OrderBy(p => p.Name),
            ("name", "desc") => query.OrderByDescending(p => p.Name),
            ("price", "asc") => query.OrderBy(p => p.Price),
            ("price", "desc") => query.OrderByDescending(p => p.Price),
            ("quantity", "asc") => query.OrderBy(p => p.Quantity),
            ("quantity", "desc") => query.OrderByDescending(p => p.Quantity),
            _ => query.OrderBy(p => p.Id) // Default sorting
        };

        // Paginate results
        var totalCount = await query.CountAsync();
        var products = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return Ok(new { TotalCount = totalCount, Products = products });
    }
    // Create a new product
    [HttpPost]
    public async Task<IActionResult> CreateProduct(Product product)
    {
        if (product == null) return BadRequest("Invalid product data.");

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, product);
    }

    // Update an existing product
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, Product product)
    {
        if (id != product.Id) return BadRequest("Product ID mismatch.");

        var existingProduct = await _context.Products.FindAsync(id);
        if (existingProduct == null) return NotFound();

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;
        existingProduct.Quantity = product.Quantity;

        _context.Products.Update(existingProduct);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Delete a product
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
