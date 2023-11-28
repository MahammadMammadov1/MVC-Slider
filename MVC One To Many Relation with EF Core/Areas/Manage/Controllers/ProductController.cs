using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC_One_To_Many_Relation_with_EF_Core.DAL;
using MVC_One_To_Many_Relation_with_EF_Core.Models;

namespace MVC_One_To_Many_Relation_with_EF_Core.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _appDb;

        public ProductController(AppDbContext appDb)
        {
            _appDb = appDb;
        }
        public IActionResult Index()
        {

            List<Product> products = _appDb.Products.ToList();
            return View(products);
        }
        public IActionResult Create()
        {
            
            ViewBag.Tags = _appDb.Tags.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult Create(Product product)
        {
            
            ViewBag.Tags = _appDb.Tags.ToList();

            if (!ModelState.IsValid) return View(product);
            
            var check = false;
            if (product.TagIds != null)
            {
                foreach (var tagId in product.TagIds)
                {
                    if (!_appDb.Tags.Any(x => x.Id == tagId))
                        check = true;
                }
            }
            if (check)
            {
                ModelState.AddModelError("TagId", "Tag not found!");
                return View(product);
            }
            else
            {
                if (product.TagIds != null)
                {
                    foreach (var tagId in product.TagIds)
                    {
                        ProductTag bookTag = new ProductTag
                        {
                            Product = product,
                            TagId = tagId
                        };
                        _appDb.ProductTags.Add(bookTag);
                    }
                }
            }

            _appDb.Products.Add(product);
            _appDb.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Update(int id)
        {
            
            ViewBag.Tags = _appDb.Tags.ToList();

            if (!ModelState.IsValid) return View();
            var existProduct = _appDb.Products.FirstOrDefault(x => x.Id == id);
            return View(existProduct);
        }

        [HttpPost]
        public IActionResult Update(Product product)
        {
            
            ViewBag.Tags = _appDb.Tags.ToList();


            var existProduct = _appDb.Products.Include(x => x.ProductTag).FirstOrDefault(b => b.Id == product.Id);
            if (existProduct == null) return NotFound();
            if (!ModelState.IsValid) return View(product);


            existProduct.ProductTag.RemoveAll(bt => !product.TagIds.Contains(bt.TagId));

            foreach (var tagId in product.TagIds.Where(tagId => !existProduct.ProductTag.Any(pt => pt.TagId == tagId)))
            {
                ProductTag productTag = new ProductTag
                {
                    TagId = tagId
                };
                existProduct.ProductTag.Add(productTag);
            }


            existProduct.Name = product.Name;
            existProduct.Description = product.Description;
            existProduct.CostPrice = product.CostPrice;
            existProduct.DiscountedPrice = product.DiscountedPrice;
            existProduct.Code = product.Code;
            existProduct.SalePrice = product.SalePrice;
            existProduct.Tax = product.Tax;
            existProduct.IsAvailable = product.IsAvailable;
           
            _appDb.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            var wanted = _appDb.Products.FirstOrDefault(x => x.Id == id);
            if (wanted == null) return NotFound();
            return View(wanted);
        }
        [HttpPost]
        public IActionResult Delete(Product product)
        {
            var wanted = _appDb.Products.FirstOrDefault(x => x.Id == product.Id);
            if (wanted == null) return NotFound();
            _appDb.Products.Remove(wanted);
            _appDb.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}