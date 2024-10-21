using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using INVApp.Models;

namespace INVApp.Services
{
    public class DatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public DatabaseService()
        {
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "InventoryDB.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            _database.CreateTableAsync<Product>().Wait();
            _database.CreateTableAsync<Category>().Wait();
            _database.CreateTableAsync<AudioSettings>().Wait();
        }

        // PRODUCT METHODS
        #region Product Methods

        // Insert or update a product
        public Task<int> SaveProductAsync(Product product)
        {
            if (product.ProductID != 0)
            {
                return _database.UpdateAsync(product);
            }
            else
            {
                return _database.InsertAsync(product);
            }
        }

        // Retrieve all products
        public Task<List<Product>> GetProductsAsync()
        {
            return _database.Table<Product>().ToListAsync();
        }

        // Retrieve a specific product by ID
        public Task<Product> GetProductByIDAsync(int id)
        {
            return _database.Table<Product>().Where(i => i.ProductID == id).FirstOrDefaultAsync();
        }

        // Retrieve a specific product by barcode
        public Task<Product> GetProductByBarcodeAsync(string barcode)
        {
            return _database.Table<Product>().Where(i => i.EAN13Barcode == barcode).FirstOrDefaultAsync();
        }

        // Update product
        public async Task UpdateProductAsync(Product product)
        {
            // Check if the product exists in the database
            var existingProduct = await _database.FindAsync<Product>(product.ProductID);
            if (existingProduct != null)
            {
                // Update existing product details
                existingProduct.ProductName = product.ProductName;
                existingProduct.Category = product.Category;
                existingProduct.ProductWeight = product.ProductWeight;
                existingProduct.CurrentStockLevel = product.CurrentStockLevel;
                existingProduct.MinimumStockLevel = product.MinimumStockLevel;
                existingProduct.WholesalePrice = product.WholesalePrice;
                existingProduct.Price = product.Price;

                await _database.UpdateAsync(existingProduct);
            }
        }

        // Delete a product
        public Task<int> DeleteProductAsync(Product product)
        {
            return _database.DeleteAsync(product);
        }

        #endregion

        // CATEGORY METHODS
        #region Category Methods

        // Retrieve all categories
        public Task<List<Category>> GetCategoriesAsync()
        {
            return _database.Table<Category>().ToListAsync();
        }

        // Insert or update a category
        public Task<int> SaveCategoryAsync(Category category)
        {
            if (category.CategoryID != 0)
            {
                return _database.UpdateAsync(category);
            }
            else
            {
                return _database.InsertAsync(category);
            }
        }

        // Delete a category
        public Task<int> DeleteCategoryAsync(Category category)
        {
            return _database.DeleteAsync(category);
        }

        // Save default category
        public Task SaveDefaultCategoryAsync(string category)
        {
            Preferences.Set("DefaultCategory", category);
            return Task.CompletedTask;
        }

        // Load default category
        public Task<string?> GetDefaultCategoryAsync()
        {
            return Task.FromResult(Preferences.Get("DefaultCategory", string.Empty));
        }

        #endregion

        public async Task<AudioSettings> GetAudioSettingsAsync()
        {
            var settings = await _database.Table<AudioSettings>().FirstOrDefaultAsync();
            return settings ?? new AudioSettings(); // Return default settings if none exist
        }

        public async Task SaveAudioSettingsAsync(AudioSettings settings)
        {
            var existingSettings = await _database.Table<AudioSettings>().FirstOrDefaultAsync();
            if (existingSettings != null)
            {
                // Update existing settings
                settings.Id = existingSettings.Id;
                await _database.UpdateAsync(settings);
            }
            else
            {
                // Insert new settings
                await _database.InsertAsync(settings);
            }
        }
    }
}
