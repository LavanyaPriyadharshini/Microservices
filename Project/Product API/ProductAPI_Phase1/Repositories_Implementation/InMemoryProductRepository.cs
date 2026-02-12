using System.Collections.Concurrent;
using System.Threading;
using ProductAPI_Phase1.Models;
using ProductAPI_Phase1.Repositories;

namespace ProductAPI_Phase1.Repositories_Implementation
{
    /// <summary>
    /// In-memory implementation using ConcurrentDictionary for thread safety
    /// see the explanation in Microservises intro folder word file - page 3
    /// </summary>
    public class InMemoryProductRepository : IProductRepository
    {
        private readonly ConcurrentDictionary<int, Product> _products;
        private int _nextId = 4; //represents the next available ID for new products
        //used for auto_incrementing primary key pattern
        //here 4 is mentioned because ,the constructor pre-loads 3 products (IDs 1,2,3) 
        //so next product added will get ID 4

        public InMemoryProductRepository()
        {
            _products = new ConcurrentDictionary<int, Product>(
                
                new Dictionary<int, Product>
                {
                    [1] = new Product
                    {
                        Id = 1,
                        ProductId = 12,
                        Prod_Name = "Laptop Dell XPS 15",
                        Description = "High performance laptop with 16GB RAM and 512GB SSD",
                        Price = 1299.99m,
                        CostPrice = 950.00m,
                        Stock = 25,
                        Category = "Electronics",
                        ProdImageUrl = "https://example.com/laptop.jpg",
                        CreatedAt = DateTime.Now.AddDays(-30),
                        SupplierId = 101
                    },

                    [2] = new Product
                    {
                        Id = 2,
                        ProductId=15,
                        Prod_Name = "Wireless Mouse Logitech MX Master 3",
                        Description = "Ergonomic wireless mouse with precision scrolling",
                        Price = 99.99m,
                        CostPrice = 60.00m,
                        Stock = 150,
                        Category = "Electronics",
                        ProdImageUrl = "https://example.com/mouse.jpg",
                        CreatedAt = DateTime.Now.AddDays(-20),
                        SupplierId = 102
                    },

                    [3] = new Product
                    {
                        Id = 3,
                        ProductId=18,
                        Prod_Name = "Herman Miller Aeron Chair",
                        Description = "Premium ergonomic office chair with lumbar support",
                        Price = 1495.00m,
                        CostPrice = 890.00m,
                        Stock = 15,
                        Category = "Furniture",
                        ProdImageUrl = "https://example.com/chair.jpg",
                        CreatedAt = DateTime.UtcNow.AddDays(-15),
                        SupplierId = 103
                    }
                }
                
                
                );
        }

        public Task<IEnumerable<Product>> GetAllProductAsync()
        {
            return Task.FromResult(_products.Values.AsEnumerable());
        }

        public Task<Product?> GetProductByIdAsync(int id)
        {
            _products.TryGetValue(id, out var product);
            return Task.FromResult(product);
        }


        public Task<Product?> GetProductByProdIdAsync(int Prodid)
        {
            _products.TryGetValue(Prodid, out var product);
            return Task.FromResult(product);
        }

        public Task<Product> CreateProductAsync(Product product)
        {
            //interlocked is a  is a static -----classes  , 
           // Interlocked is a static class in System.Threading that provides atomic operations for variables that are shared by multiple threads.
           // It ensures operations complete as a single, uninterruptible unit, no other thread can interfere midway
           //explanation refer word file "Mictoservices intro pg 14
            product.Id = Interlocked.Increment(ref _nextId);
            product.CreatedAt = DateTime.UtcNow;

            _products.TryAdd(product.Id, product);

            return Task.FromResult(product);
        }


        public Task<Product?> UpdateProductAsync(Product product)
        {
            if (!_products.ContainsKey(product.Id))
                return Task.FromResult<Product?>(null);

            _products[product.Id] = product;
            return Task.FromResult<Product?>(product);
        }

        public Task<bool> DeleteProductAsync(int id)
        {
            return Task.FromResult(_products.TryRemove(id, out _));
        }


        //this method checks whether the product with the given id exists in the _products dictionary
        //returns true if the product exist, or false if it does not
        //Task.FromResult -- it wraps a synchronous result into a completed Task
        public Task<bool> ProductExistsAsync(int id)
        {
            return Task.FromResult(_products.ContainsKey(id));
        }


        //IEnumerable - represents the a collection that can be iterated over the loop
        //read-only.
        //Move forward only from start to end
        //items are accessed only when needed(deferred execustion)
        //no indexing - like arrays these cannot be indexed
        public Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
        {
            var products = _products.Values
                .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
                .AsEnumerable();

            // ASEnumerable, casts the output to Ienumerable hiding additional details
            //or it converts the result to IEnumerable<Product>

            return Task.FromResult(products);

            //strincomaparison --This compares p.Category with category in a case-insensitive way using ordinal
            //(binary) comparison.
        }
    }
}

