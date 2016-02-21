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
using AdminPanel.Areas.Admin.Helpers;
using AdminPanel.Abstract;

namespace AdminPanel.Areas.Admin.Controllers
{
    public class HomeController : AdminController
    {
        AdminPanelContext db = new AdminPanelContext();

        /// <summary>
        /// GET: Admin/Home
        /// </summary>
        /// <returns> display products from database </returns>
        public ActionResult Index()
        {
            // confirmation message for edited or created new product
            ViewBag.Message = (string)TempData["message"];
            TempData.Remove("message");

            return View();
        }

        /// <summary>
        /// left side of Index() view
        /// </summary>
        /// <returns> display tree menu with categories </returns>
        public PartialViewResult Menu()
        {
            var categories = db.Categories.ToList();
            return PartialView(categories);
        }

        /// <summary>
        /// products' sublist for specified category
        /// </summary>
        /// <param name="category"> category name from Menu() </param>
        public PartialViewResult subAMenu(string category)
        {
            var products = db.Products.Where(c => c.Category.CategoryName == category).
                DistinctBy(p => p.ProductName).ToList();
            return PartialView(products);
        }

        /// <summary>
        /// colors' (string) sublist for specified product name
        /// </summary>
        /// <param name="productName"> product name from subAMenu() </param>
        public PartialViewResult subBMenu(string productName, string category)
        {
            IEnumerable<string> colors = db.Products.Where(c => c.Category.CategoryName == category).
                Where(p => p.ProductName == productName).Select(c => c.Color.ColorName).ToList();

            return PartialView(colors);
        }

        /// <summary>
        /// right side of Index() view
        /// </summary>
        /// <param name="category"> category name from Menu() </param>
        /// <param name="productName"> product name from subAMenu() </param>
        /// <returns> display table with products </returns>
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

            return PartialView(products);
        }

        /// <summary>
        /// GET: Admin/Home/Create
        /// </summary>
        /// <returns> products's add form </returns>
        [HttpGet]
        public ActionResult Create()
        {
            Product newproduct = new Product();

            // set every size default as product size in checkboxes
            newproduct.Sizes = new List<Size>();
            PopulateAssignedSizeData(newproduct);

            ViewBag.CategoryId = new SelectList(Retriever.GetCategories(), "CategoryId", "CategoryName", null);
            ViewBag.ColorId = new SelectList(Retriever.GetColors(), "ColorId", "ColorName", null);

            return View(newproduct);
        }

        /// <summary>
        /// add new product to database
        /// </summary>
        /// <param name="newproduct"> product from GET method </param>
        /// <param name="uploads"> input files </param>
        /// <param name="selectedSizes"> product's sizes </param>
        /// <returns> add product or display errors </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product newproduct, IEnumerable<HttpPostedFileBase> uploads, string[] selectedSizes)
        {
            // remake if something went wrong
            ViewBag.CategoryId = new SelectList(Retriever.GetCategories(), "CategoryId", "CategoryName", null);
            ViewBag.ColorId = new SelectList(Retriever.GetColors(), "ColorId", "ColorName", null);

            if (ModelState.IsValid)
            {
                // assign sizes to product
                newproduct.Sizes = new List<Size>();

                foreach (string size in selectedSizes)
                {
                    Size sizeToAdd = db.Sizes.Find(int.Parse(size));
                    newproduct.Sizes.Add(sizeToAdd);
                }

                // set product images list
                newproduct.FilePaths = new List<FilePath>();

                foreach (HttpPostedFileBase upload in uploads)
                {
                    // check each file upload
                    // if any exists add this to product images list,
                    // save on server
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

                // calculate product price
                if (newproduct.Discount > 0)
                {
                    newproduct.Price = newproduct.NormalPrice - newproduct.NormalPrice * ((decimal)newproduct.Discount / 100);
                }
                else
                {
                    newproduct.Price = newproduct.NormalPrice;
                }

                // set create date and save changes
                newproduct.CreateDate = DateTime.Now;
                db.Products.Add(newproduct);
                await db.SaveChangesAsync();

                // set confirmation message to Index() View
                TempData["message"] = string.Format("{0} has been created.", newproduct.ProductName);

                return RedirectToAction("Index");
            }

            // return if invalid
            return View(newproduct);
        }

        /// <summary>
        /// GET: Admin/Home/Edit
        /// </summary>
        /// <param name="id"> product's id </param>
        /// <returns> product's edit form </returns>
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            { return new HttpStatusCodeResult(HttpStatusCode.BadRequest); }

            Product product = db.Products.Include(p => p.Sizes).Include(p => p.FilePaths).Where(i => i.ProductId == id).Single();

            // set selected checkbox for each product size
            PopulateAssignedSizeData(product);

            if (product == null)
            {
                return HttpNotFound();
            }

            // set default view model values
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
                getCategories = new SelectList(Retriever.GetCategories(), "CategoryId", "CategoryName", product.Category.CategoryName),
                getColors = new SelectList(Retriever.GetColors(), "ColorId", "ColorName", product.Color.ColorName)
            };

            return View(model);
        }

        /// <summary>
        /// edit product in database
        /// </summary>
        /// <param name="model"> view model from GET Method </param>
        /// <param name="uploads"> input files ( if exists delete old product's images) </param>
        /// <param name="selectedSizes"> product's sizes </param>
        /// <returns> save product changes or display errors </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditProductViewModel model, IEnumerable<HttpPostedFileBase> uploads, string[] selectedSizes)
        {
            Product prod = db.Products.Include(i => i.Sizes).Include(p => p.FilePaths).Where(p => p.ProductId == model.Id).Single();
            List<FilePath> actualImages = db.FilePaths.Where(p => p.ProductId == model.Id).ToList();

            if (ModelState.IsValid)
            {
                // check files upload
                // if exists 
                // delete old images and add new to product images list,
                // save on server
                if (uploads != null)
                {
                    foreach (FilePath file in actualImages)
                    {
                        string filePath = Request.MapPath("~/Content/Images/" + file.FileName);
                        System.IO.File.Delete(filePath);
                    }
                    db.FilePaths.RemoveRange(actualImages);


                    //prod.FilePaths = new List<FilePath>();

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

                // calculate product price
                if (prod.Discount > 0)
                {
                    prod.Price = model.NormalPrice - model.NormalPrice * ((decimal)model.Discount / 100);
                }
                else
                {
                    prod.Price = model.NormalPrice;
                }

                //attach model properties as product properties
                prod.ProductId = model.Id;
                prod.ProductName = model.ProductName;
                prod.Desc = model.Desc;
                prod.Material = model.Material;
                prod.NormalPrice = model.NormalPrice;
                prod.Discount = model.Discount;
                prod.CategoryId = model.CategoryId;
                prod.ColorId = model.ColorId;
                prod.EditDate = DateTime.Now;

                //save changes
                db.Entry(prod).State = EntityState.Modified;
                await db.SaveChangesAsync();

                // set confirmation message to Index() View
                TempData["message"] = string.Format("{0} has been edited", prod.ProductName);

                return RedirectToAction("Index");
            }

            // return form if invalid
            return View(model);
        }

        /// <summary>
        /// delete product from database
        /// </summary>
        /// <param name="productID"> product's id </param>
        /// <returns> GET: Admin/Home </returns>
        [HttpPost]
        public async Task<ActionResult> Delete(int productID)
        {
            Product product = db.Products.Include(p => p.Sizes).Include(p => p.FilePaths).Where(p => p.ProductId == productID).Single();
            List<FilePath> images = db.FilePaths.Where(s => s.ProductId == productID).ToList();

            foreach (FilePath image in images)
            {
                string filePath = Request.MapPath("~/Content/Images/" + image.FileName);
                System.IO.File.Delete(filePath);
            }

            db.FilePaths.RemoveRange(images);
            db.Products.Remove(product);
            await db.SaveChangesAsync();

            // set confirmation message to Index() View
            TempData["message"] = string.Format("{0} has been deleted.", product.ProductName);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// calculate product price on Create() or Edit() View
        /// </summary>
        /// <param name="total"> total price </param>
        /// <param name="discount"> discount </param>
        /// <returns> price with discount </returns>
        [HttpGet]
        public ActionResult calcPrice(decimal total, decimal discount)
        {
            decimal afterDiscount;

            if (discount > 0)
            {
                afterDiscount = total - total * (discount / 100);
            }
            else
            {
                afterDiscount = total;
            }

            return Json(afterDiscount, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// update product sizes
        /// </summary>
        /// <param name="selectedSizes"> list of sizes </param>
        /// <param name="product"> product with updated sizes </param>
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

        /// <summary>
        /// mark product size checkboxes in view
        /// </summary>
        /// <param name="product"> specified product </param>
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

        /// <summary>
        /// close database connections
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}