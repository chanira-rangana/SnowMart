﻿using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;
public class ProductRepository(StoreContext context) : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand,string? type,string? sort)
    {
        var query = context.Products.AsQueryable();
        if (!string.IsNullOrWhiteSpace(brand))
            query = query.Where(a => a.Brand == brand);
        if (!string.IsNullOrWhiteSpace(type))
            query = query.Where(a => a.Type == type);
        if (!string.IsNullOrWhiteSpace(sort))
        {
            query = sort switch
            {
                "priceAsc" => query.OrderBy(a => a.Price),
                "PriceDesc" => query.OrderByDescending(a => a.Price),
                _ => query.OrderBy(a => a.Name)
            };
        }
        return await query.ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        return await context.Products.FindAsync(id);
    }

    public async Task<IReadOnlyList<string>> GetBrandsAsync()
    {
        return await context.Products.Select(a=>a.Brand).Distinct().ToListAsync();
    }

    public async Task<IReadOnlyList<string>> GetTypesAsync()
    {
        return await context.Products.Select(a=>a.Type).Distinct().ToListAsync();
    }

    public void AddProduct(Product product)
    {
        context.Products.Add(product);
    }

    public void UpdateProduct(Product product)
    {
        context.Entry(product).State = EntityState.Modified;
    }

    public void DeleteProduct(Product product)
    {
        context.Products.Remove(product);
    }

    public bool ProductExists(int id)
    {
        return context.Products.Any(e => e.Id == id);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}