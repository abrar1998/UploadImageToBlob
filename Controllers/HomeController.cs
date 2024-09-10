using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using UploadImageToBlob.Models;

namespace UploadImageToBlob.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IConfiguration configuration;
        private readonly BlobServiceClient blobServiceClient;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, BlobServiceClient blobServiceClient)
        {
            this.logger = logger;
            this.configuration = configuration;
            this.blobServiceClient = blobServiceClient;
        }

        public static List<Student> StudentData ;

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(Student model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if the model contains a photo
                    if (model.photo != null && model.photo.Length > 0)
                    {
                        // Get the connection string for Azure Blob Storage from the configuration
                        //string connectionString = configuration.GetConnectionString("AzureBlobStorage")!;

                        // Get the container client for the 'images' container
                        //already added connection string in Program.cs file
                        BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("images");

                        // Check if the container exists, otherwise create it
                        if (!containerClient.Exists())
                        {
                            await containerClient.CreateAsync();
                            await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                        }

                        // Generate a unique file name with a GUID and the file extension
                        string blobName = Guid.NewGuid().ToString() + Path.GetExtension(model.photo.FileName);

                        // Upload the photo to Azure Blob Storage
                        BlobClient blobClient = containerClient.GetBlobClient(blobName);
                        using (var stream = model.photo.OpenReadStream())
                        {
                            await blobClient.UploadAsync(stream, overwrite: true);
                        }

                        // Get the URI of the uploaded blob
                        string blobUri = blobClient.Uri.ToString();

                        // Optionally, pass the URI to the view or save it to the database
                        ViewBag.ImageUrl = blobUri;
                        StudentData = new List<Student>()
                        {
                            new Student()
                            {
                                Id = model.Id,
                                Name = model.Name,
                                Address = model.Address,
                                photoUrl = blobUri
                            }
                        };
                        return RedirectToAction("GetDetails");
                        // You can also assign the URL to a property in the model (e.g., model.PhotoUrl)
                        // model.PhotoUrl = blobUri;
                        // Save the student to the database here if needed
                    }
                }
                catch (Exception ex)
                {
                    // Handle the error, e.g., logging, displaying a message, etc.
                    ViewBag.ErrorMessage = "An error occurred while uploading the photo.";
                    return BadRequest(ex.Message);
                    // Log the exception (use a logging framework like Serilog or NLog)
                }
            }

            return View(model);
        }

        public IActionResult GetDetails()
        {
            return View(StudentData);
        }
    

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
