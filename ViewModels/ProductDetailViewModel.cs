using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using INVApp.Models;
using INVApp.Services;

namespace INVApp.ViewModels
{
    public class ProductDetailViewModel : BaseViewModel
    {
        #region Declarations

        // Declare database
        private readonly DatabaseService _databaseService;

        // Declare events
        public event Action ProductUpdated;
        public event Action ProductDeleted;

        // Declare Product
        private Product _product;
        public Product Product
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
            }
        }

        // Declare Commands
        public ICommand CloseCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand DeleteCommand { get; }

        #endregion

        public ProductDetailViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            CloseCommand = new Command(CloseModal);
            UpdateCommand = new Command(UpdateProduct);
            DeleteCommand = new Command(DeleteProduct);
        }

        private async void CloseModal()
        {
            await Application.Current.MainPage.Navigation.PopModalAsync();
        }

        private async void UpdateProduct()
        {
            if (Product.CurrentStockLevel < 0)
            {
                await Application.Current.MainPage.DisplayAlert("Invalid Stock Level", "Stock level cannot be negative. Please correct the stock level before updating.", "OK");
                return; 
            }

            bool answer = await Application.Current.MainPage.DisplayAlert("Update Product", "Are you sure you want to update the product details?", "Yes", "No");
            if (answer)
            {
                await _databaseService.UpdateProductAsync(Product);
                ProductUpdated?.Invoke();

                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
        }

        private async void DeleteProduct()
        {
            bool answer = await Application.Current.MainPage.DisplayAlert("Delete Product", "Are you sure you want to delete this product?", "Yes", "No");
            if (answer)
            {
                App.NotificationService.Notify($"Deleting product: {Product.ProductName}");
                await _databaseService.DeleteProductAsync(Product);

                ProductDeleted?.Invoke();

                await Application.Current.MainPage.Navigation.PopModalAsync();
            }
        }
    }
}
