using System;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StarCorp.Data;
using StarCorp.Models;

namespace StarCorp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IProductDataService _productDataService;

        public ProductsController(ILogger<ProductsController> logger, IProductDataService productDataService)
        {
            _logger = logger;
            _productDataService = productDataService;
        }

        //Create Products
        [HttpPost]
        public async Task<IActionResult> Post(Guid productId, Product product)
        {
            await _productDataService.CreateProductAsync(product);
            return Ok();
        }

        //Get Products
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _productDataService.GetProductsAsync());
        }


        //Update Products
        [HttpPut]
        public async Task<IActionResult> Update(Guid productId, Product product)
        {
            

            //Denna biten funkar också 
            
            //var existingProduct = await _productDataService.UpdateProductAsync(product);
            return Ok(await _productDataService.UpdateProductAsync(product));
            


            //Denna biten funkar just nu
            /*
            var updatedProduct = await _productDataService.UpdateProductAsync(product);
            return Ok(await _productDataService.UpdateProductAsync(product));
            */
        }
        //Delete Products
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid productId, Product product)
        {
            await _productDataService.DeleteProductAsync(product);
            return Ok();
        }
    }
}