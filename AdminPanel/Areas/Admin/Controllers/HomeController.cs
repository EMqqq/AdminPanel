using AdminPanel.DataAccessLayer;
using AdminPanel.Entities;
using AdminPanel.Areas.Admin.ViewModels;
using AdminPanel.Models;
using System;
using System.Collections.Generic;
using System.IO;
using MoreLinq;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;

namespace AdminPanel.Areas.Admin.Controllers
{
    public class HomeController : AdminController
    {
        AdminPanelContext db = new AdminPanelContext();

        public ActionResult Index()
        {
            return View();
        }

        public PartialViewResult Menu(string category, int? productId)
        {
            var categories = db.Categories.ToList();
            return PartialView(categories);
        }

        /// <summary>
        /// Return product's sublist for specified category
        /// </summary>
        public PartialViewResult subAMenu(string category)
        {
            var products = db.Products.Where(c => c.Category.CategoryName == category).
                DistinctBy(p => p.ProductName).ToList();
            return PartialView(products);
        }

        /// <summary>
        /// Return colors' (string) sublist for specified name product
        /// </summary>
        /// <param name="productName"> name from subAMenu </param>
        public PartialViewResult subBMenu(string productName, string category)
        {
            IEnumerable<string> colors = db.Products.Where(c => c.Category.CategoryName == category).
                Where(p => p.ProductName == productName).Select(c => c.Color.ColorName).ToList();

            return PartialView(colors);
        }

        public PartialViewResult GetProducts(string category, string productName)
        {
            IQueryable<Product> products = db.Products.Include(c => c.Color).Include(c => c.Category).
                                                            Include(c => c.Sizes).OrderBy(p => p.ProductName);

            if (category != null)
            {
                products = products.Where(p => p.Category.CategoryName.ToLower() == category.ToLower());
            }

            if (productName != null)
            {
                products = products.Where(p => p.ProductName.ToLower() == productName.ToLower());
            }

            ViewBag.Message = (string)TempData["message"];
            TempData.Remove("message");

            return PartialView(products);
        }

        [HttpGet]
        public ActionResult Create()
        {
            Product newproduct = new Product();

            newproduct.Category = new Category();
            newproduct.Sizes = new List<Size>();
            newproduct.FilePaths = new List<FilePath>();

            PopulateAssignedSizeData(newproduct);
            PopulateCategoriesDropDownList(newproduct);
            PopulateColorsDropDownList(newproduct);

            return View(newproduct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product newproduct, IEnumerable<HttpPostedFileBase> uploads, string[] selectedSizes)
        {
            if (selectedSizes != null)
            {
                newproduct.Sizes = new List<Size>();

                foreach (string size in selectedSizes)
                {
                    Size sizeToAdd = db.Sizes.Find(int.Parse(size));
                    newproduct.Sizes.Add(sizeToAdd);
                }
            }

            if (ModelState.IsValid)
            {
                newproduct.FilePaths = new List<FilePath>();

                foreach (HttpPostedFileBase upload in uploads)
                {
                    if (upload != null && upload.ContentLength > 0)
                    {
                        FilePath photo = new FilePath()
                        {
                            FileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName),
                            FileType = FileType.Photo
                        };

                        newproduct.FilePaths.Add(photo);
                        upload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images"), photo.FileName));
                    }
                }

                newproduct.CreateDate = DateTime.Now;

                if (newproduct.Discount > 0)
                {
                    newproduct.Price = newproduct.NormalPrice - newproduct.NormalPrice * ((decimal)newproduct.Discount / 100);
                }
                else
                {
                    newproduct.Price = newproduct.NormalPrice;
                }

                db.Products.Add(newproduct);
                await db.SaveChangesAsync();
                TempData["message"] = string.Format("{0} has been created.", newproduct.ProductName);
                return RedirectToAction("Index");
            }

            PopulateAssignedSizeData(newproduct);
            PopulateCategoriesDropDownList();
            PopulateColorsDropDownList();
            return View(newproduct);
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            Product product = db.Products.Include(p => p.Sizes).Include(p => p.FilePaths).Where(i => i.ProductId == id).Single();

            PopulateAssignedSizeData(product);

            if (product == null)
            { return HttpNotFound(); }

            EditProductViewModel model = new EditProductViewModel()
            {
                Id = product.ProductId,
                ProductName = product.ProductName,
                Desc = product.Desc,
                Material = product.Material,
                NormalPrice = product.NormalPrice,
                Discount = product.Discount,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ColorId = product.ColorId,
                FilePaths = product.FilePaths.ToList(),
                getCategories = PopulateCategories(product),
                getColors = PopulateColors(product)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditProductViewModel model, IEnumerable<HttpPostedFileBase> uploads, string[] selectedSizes)
        {
            Product prod = db.Products.Include(i => i.Sizes).Include(p => p.FilePaths).Where(p => p.ProductId == model.Id).Single();
            List<FilePath> actualImages = db.FilePaths.Where(p => p.ProductId == model.Id).ToList();

            PopulateAssignedSizeData(prod);

            model.FilePaths = prod.FilePaths.ToList();
            model.getCategories = PopulateCategories(prod);
            model.getColors = PopulateColors(prod);

            if (ModelState.IsValid)
            {
                try
                {
                    if (uploads.Any(c => c != null))
                    {
                        foreach (FilePath file in actualImages)
                        {
                            string filePath = Request.MapPath("~/Content/Images/" + file.FileName);
                            System.IO.File.Delete(filePath);
                        }

                        db.FilePaths.RemoveRange(actualImages);
                        prod.FilePaths = new List<FilePath>();

                        foreach (HttpPostedFileBase upload in uploads)
                        {
                            if (upload != null && upload.ContentLength > 0)
                            {
                                FilePath photo = new FilePath
                                {
                                    FileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName),
                                    FileType = FileType.Photo
                                };
                                prod.FilePaths.Add(photo);
                                upload.SaveAs(Path.Combine(Server.MapPath("~/Content/Images"), photo.FileName));
                            }
                        }
                    }

                    //update sizes
                    UpdateProductSizes(selectedSizes, prod);

                    //count price
                    if (prod.Discount > 0)
                    {
                        prod.Price = model.NormalPrice - model.NormalPrice * ((decimal)model.Discount / 100);
                    }
                    else
                    {
                        prod.Price = model.NormalPrice;
                    }

                    //attach model properties to product
                    prod.ProductId = model.Id;
                    prod.ProductName = model.ProductName;
                    prod.Desc = model.Desc;
                    prod.Material = model.Material;
                    prod.NormalPrice = model.NormalPrice;
                    prod.Discount = model.Discount;
                    prod.CategoryId = model.CategoryId;
                    prod.ColorId = model.ColorId;
                    prod.EditDate = DateTime.Now;

                    //save
                    db.Entry(prod).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    TempData["message"] = string.Format("{0} has been edited", prod.ProductName);
                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException)
                {
                    ModelState.AddModelError("", "Can not save content. Contact with administrator.");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(int productID)
        {
            Product product = db.Products.Include(p => p.Sizes).Include(p => p.FilePaths).Where(p => p.ProductId == productID).Single();

            TempData["message"] = string.Format("{0} has been deleted.", product.ProductName);

            List<FilePath> images = db.FilePaths.Where(s => s.ProductId == productID).ToList();

            foreach (FilePath image in images)
            {
                string filePath = Request.MapPath("~/Content/Images/" + image.FileName);
                System.IO.File.Delete(filePath);
            }

            db.FilePaths.RemoveRange(images);
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult calcPrice(decimal fullPrice, decimal discount)
        {
            decimal afterDiscount;

            if (discount > 0)
            {
                afterDiscount = fullPrice - fullPrice * (discount / 100);
            }
            else
            {
                afterDiscount = fullPrice;
            }
            return Json(afterDiscount, JsonRequestBehavior.AllowGet);
        }

        //CRUD helper methods

        private SelectList PopulateCategories(Product product)
        {
            IQueryable<Category> categoriesQuery = from p in db.Categories
                                                   orderby p.CategoryName
                                                   select p;

            return new SelectList(categoriesQuery, "CategoryId", "CategoryName", product.Category.CategoryName);
        }

        private void PopulateCategoriesDropDownList(object selectedCategory = null)
        {
            IOrderedQueryable<Category> categoriesQuery = from c in db.Categories
                                                          orderby c.CategoryName
                                                          select c;

            ViewBag.CategoryId = new SelectList(categoriesQuery, "CategoryId", "CategoryName", selectedCategory);
        }

        private void PopulateColorsDropDownList(object selectedColor = null)
        {
            IOrderedQueryable<Color> colorsQuery = from c in db.Colors
                                                   orderby c.ColorName
                                                   select c;

            ViewBag.ColorId = new SelectList(colorsQuery, "ColorId", "ColorName", selectedColor);
        }

        private SelectList PopulateColors(Product product)
        {
            IQueryable<Color> colorsQuery = from p in db.Colors
                                            orderby p.ColorName
                                            select p;

            return new SelectList(colorsQuery, "ColorId", "ColorName", product.Color.ColorName);
        }



        private void UpdateProductSizes(string[] selectedSizes, Product product)
        {

            if (selectedSizes == null)
            {
                product.Sizes = new List<Size>();
                return;
            }

            HashSet<string> selectedSizesHS = new HashSet<string>(selectedSizes);
            HashSet<int> productSizes = new HashSet<int>(product.Sizes.Select(s => s.SizeId));

            foreach (Size size in db.Sizes)
            {
                if (selectedSizesHS.Contains(size.SizeId.ToString()))
                {
                    if (!productSizes.Contains(size.SizeId))
                    {
                        product.Sizes.Add(size);
                    }
                }
                else
                {
                    if (productSizes.Contains(size.SizeId))
                    {
                        product.Sizes.Remove(size);
                    }
                }
            }
        }

        private void PopulateAssignedSizeData(Product product)
        {
            DbSet<Size> allSizes = db.Sizes;
            HashSet<int> productSizes = new HashSet<int>(product.Sizes.Select(s => s.SizeId));
            List<SizeChoiceViewModel> viewModel = new List<SizeChoiceViewModel>();

            if (productSizes == null)
            {
                ModelState.AddModelError("NoSelectedSize", "Wybierz rozmiar produktu.");
            }
            else
            {
                foreach (Size size in allSizes)
                {
                    viewModel.Add(new SizeChoiceViewModel
                    {
                        SizeId = size.SizeId,
                        Assigned = productSizes.Contains(size.SizeId)
                    });
                }
                ViewBag.Sizes = viewModel;
            }
        }
    }
}