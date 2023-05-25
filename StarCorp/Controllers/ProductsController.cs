using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StarCorp.Abstractions;
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
        private readonly IOrderDataService _orderDataService;

        public ProductsController(ILogger<ProductsController> logger, IProductDataService productDataService, IOrderDataService orderDataService)
        {
            _logger = logger;
            _productDataService = productDataService;
            _orderDataService = orderDataService;
        }

        [HttpPost("Order")]
        public async Task<IActionResult> PostOrder(string buyer, string deliveryAddress, Guid productId, uint amount, Guid orderId)
        {
            
            var products = await _productDataService.GetProductsAsync();

            var order = new Order();
            var orderLine = new OrderLine();
            
            foreach (var product in products)
            {
                if (product.Id == productId)
                {
                    if (product.Stock != 0 && product.Stock >= amount)
                    {
                        order.Lines = new List<OrderLine>();
                        order.Id = orderId;
                        order.Buyer = buyer;
                        order.DeliveryAddress = deliveryAddress;
                        order.TotalValue = amount * product.Price;
                        orderLine.OrderId = order.Id;
                        orderLine.ProductId = productId;
                        orderLine.Quantity = amount;
                        //order.Lines
                        //product.Stock = await _productDataService.UpdateProductAsync(-amount);
                    }
                    else
                    {
                        return BadRequest();
                    }
                }
            }


            return Ok(await _orderDataService.SaveOrder(order));
            
        }

        [HttpPost]
        public async Task<IActionResult> Post(Guid productId, Product product)
        {
            if (product.Id.Equals(productId))
            {
                return Ok(await _productDataService.CreateProductAsync(product));
            }
            return BadRequest();
        }
        
        [HttpGet]
        public async Task<IActionResult> Get(Guid productId, string itemName, string itemCategory, string itemBrand)
        {
            var products = await _productDataService.GetProductsAsync();

            if (products == null)
            {
                return NotFound();
            }

            if (productId != null)
            {
                foreach (var product in products)
                {
                    if (product.Id.Equals(productId))
                    {
                        return Ok(product);
                    }
                }
            }


            if (!String.IsNullOrEmpty(itemName))
            {
                products = products.Where(s => s.Name!.Contains(itemName));
            }
            if (!String.IsNullOrEmpty(itemBrand))
            {
                products = products.Where(x => x.Brand!.Contains(itemBrand));
            }
            if (!String.IsNullOrEmpty(itemCategory))
            {
                products = products.Where(q => q.Category!.Contains(itemCategory));
            }


            return Ok(products.ToList());


        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid productId, Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }
            await _productDataService.UpdateProductAsync(product);
            return NoContent();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(Guid productId)
        {
            var products = await _productDataService.GetProductsAsync();

            foreach (var product in products)
            {
                if (product.Id.Equals(productId))
                {
                    await _productDataService.DeleteProductAsync(product);
                    return NoContent();
                }

                /*
                if(product.Id == productId)
                {
                    await _productDataService.DeleteProductAsync(product);
                    return NoContent();
                }
                */
            }

            return BadRequest();

        }
    }
}